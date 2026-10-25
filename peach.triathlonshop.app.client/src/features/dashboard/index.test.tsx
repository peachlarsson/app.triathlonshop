import { StrictMode, type PropsWithChildren } from 'react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import { render } from 'vitest-browser-react'
import { Dashboard } from './index'

const mocks = vi.hoisted(() => ({
  start: vi.fn<() => Promise<void>>(),
}))

vi.mock('./components/overview', () => ({
  Overview: () => null,
}))

vi.mock('@/components/layout/header', () => ({
  Header: ({ children }: PropsWithChildren) => <header>{children}</header>,
}))

vi.mock('@/components/theme-switch', () => ({
  ThemeSwitch: () => null,
}))

vi.mock('@microsoft/signalr', () => ({
  HubConnectionBuilder: class {
    withUrl() {
      return this
    }
    withAutomaticReconnect() {
      return this
    }
    build() {
      return {
        on: vi.fn(),
        onreconnecting: vi.fn(),
        onreconnected: vi.fn(),
        onclose: vi.fn(),
        start: mocks.start,
        stop: vi.fn().mockResolvedValue(undefined),
      }
    }
  },
}))

const endpoint = '/api/dashboard/developer/create-random-orders'
const payload = {
  OrderRows: 20,
  ArticlesCountIntervalStart: 1,
  ArticlesCountIntervalEnd: 10,
  QuantityIntervalStart: 1,
  QuantityIntervalEnd: 5,
  UseRandomOrderStatus: true,
  UseRandomCreatedDate: true,
  UseRandomDelay: true,
  DelayMsIntervalStart: 0,
  DelayMsIntervalEnd: 3000,
}

describe('Dashboard random order generation', () => {
  const fetchMock = vi.fn<typeof fetch>()
  let queryClient: QueryClient
  const dashboardData = {
    totalRevenue: 0,
    totalCustomers: 0,
    totalOrders: 0,
    activeOrders: 0,
    monthlyRevenue: [],
    recentOrdersLast30Days: [],
  }

  function TestDashboard() {
    return (
      <QueryClientProvider client={queryClient}>
        <Dashboard />
      </QueryClientProvider>
    )
  }

  const orderRequests = () =>
    fetchMock.mock.calls.filter(([url]) => url === endpoint)

  beforeEach(() => {
    vi.clearAllMocks()
    mocks.start.mockResolvedValue(undefined)
    queryClient = new QueryClient({
      defaultOptions: { queries: { retry: false } },
    })
    vi.spyOn(queryClient, 'invalidateQueries')
    fetchMock.mockReset()
    fetchMock.mockImplementation(async (url) => {
      if (url === endpoint) return new Response('Created')
      if (url === '/api/dashboard') return Response.json(dashboardData)
      if (url === '/api/homepage/negotiation/HomePage') {
        return Response.json({ url: 'http://localhost/hub', accessToken: '' })
      }
      throw new Error(`Unexpected request: ${String(url)}`)
    })
    vi.stubGlobal('fetch', fetchMock)
  })

  afterEach(() => {
    queryClient.clear()
    vi.unstubAllGlobals()
  })

  it('posts the exact payload once in Strict Mode and again on a new visit', async () => {
    const firstVisit = await render(
      <StrictMode>
        <TestDashboard />
      </StrictMode>
    )

    await vi.waitFor(() => expect(orderRequests()).toHaveLength(1))
    expect(orderRequests()[0]).toEqual([
      endpoint,
      {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      },
    ])

    await firstVisit.rerender(
      <StrictMode>
        <TestDashboard />
      </StrictMode>
    )
    expect(orderRequests()).toHaveLength(1)

    await firstVisit.unmount()
    await render(<TestDashboard />)
    await vi.waitFor(() => expect(orderRequests()).toHaveLength(2))
  })

  it('refreshes dashboard data when order generation completes', async () => {
    let completeRequest!: (response: Response) => void
    const pendingRequest = new Promise<Response>((resolve) => {
      completeRequest = resolve
    })
    fetchMock.mockImplementation(async (url) => {
      if (url === endpoint) return pendingRequest
      if (url === '/api/dashboard') return Response.json(dashboardData)
      return Response.json({ url: 'http://localhost/hub', accessToken: '' })
    })

    await render(<TestDashboard />)
    await vi.waitFor(() => expect(orderRequests()).toHaveLength(1))
    vi.mocked(queryClient.invalidateQueries).mockClear()

    completeRequest(new Response('Created'))
    await vi.waitFor(() =>
      expect(queryClient.invalidateQueries).toHaveBeenCalledExactlyOnceWith({
        queryKey: ['dashboard'],
      })
    )
  })

  it.each([
    [
      'HTTP failure',
      () => Promise.resolve(new Response(null, { status: 500 })),
      'Could not create random orders (HTTP 500).',
    ],
    [
      'network failure',
      () => Promise.reject(new Error('Network unavailable')),
      'Network unavailable',
    ],
  ])('shows an error on %s without retrying', async (_, respond, message) => {
    fetchMock.mockImplementation(async (url) => {
      if (url === endpoint) return respond()
      if (url === '/api/dashboard') return Response.json(dashboardData)
      return Response.json({ url: 'http://localhost/hub', accessToken: '' })
    })

    const screen = await render(<TestDashboard />)
    await expect.element(screen.getByRole('alert')).toHaveTextContent(message)
    expect(
      fetchMock.mock.calls.filter(([url]) => url === endpoint)
    ).toHaveLength(1)
  })

  it('waits for SignalR to connect after dashboard HTTP 200', async () => {
    let connect!: () => void
    mocks.start.mockImplementation(
      () =>
        new Promise<void>((resolve) => {
          connect = resolve
        })
    )

    await render(<TestDashboard />)
    await vi.waitFor(() => {
      expect(queryClient.getQueryState(['dashboard'])?.status).toBe('success')
      expect(mocks.start).toHaveBeenCalledOnce()
    })
    expect(orderRequests()).toHaveLength(0)

    connect()
    await vi.waitFor(() => expect(orderRequests()).toHaveLength(1))
  })

  it('waits for dashboard HTTP 200 after SignalR connects, even with cached data', async () => {
    queryClient.setQueryData(['dashboard'], dashboardData)
    let completeDashboard!: (response: Response) => void
    const pendingDashboard = new Promise<Response>((resolve) => {
      completeDashboard = resolve
    })
    fetchMock.mockImplementation(async (url) => {
      if (url === endpoint) return new Response('Created')
      if (url === '/api/dashboard') {
        return pendingDashboard.then((response) => response.clone())
      }
      return Response.json({ url: 'http://localhost/hub', accessToken: '' })
    })

    await render(<TestDashboard />)
    await vi.waitFor(() =>
      expect(queryClient.invalidateQueries).toHaveBeenCalled()
    )
    expect(orderRequests()).toHaveLength(0)

    completeDashboard(Response.json(dashboardData))
    await vi.waitFor(() => expect(orderRequests()).toHaveLength(1))
  })

  it.each([201, 500])(
    'does not create orders for dashboard HTTP %s',
    async (status) => {
      fetchMock.mockImplementation(async (url) => {
        if (url === '/api/dashboard')
          return Response.json(dashboardData, { status })
        if (url === endpoint) return new Response('Created')
        return Response.json({ url: 'http://localhost/hub', accessToken: '' })
      })

      const screen = await render(<TestDashboard />)
      await expect
        .element(screen.getByRole('alert'))
        .toHaveTextContent(`Could not load dashboard data (HTTP ${status}).`)
      expect(orderRequests()).toHaveLength(0)
    }
  )

  it('does not create orders when SignalR negotiation fails', async () => {
    fetchMock.mockImplementation(async (url) => {
      if (url === '/api/dashboard') return Response.json(dashboardData)
      return new Response(null, { status: 500 })
    })

    const screen = await render(<TestDashboard />)
    await expect
      .element(screen.getByRole('alert'))
      .toHaveTextContent(
        'Could not connect to dashboard live updates (HTTP 500).'
      )
    expect(mocks.start).not.toHaveBeenCalled()
    expect(orderRequests()).toHaveLength(0)
  })

  it('does not create orders when SignalR startup fails', async () => {
    mocks.start.mockRejectedValue(new Error('SignalR startup failed'))

    const screen = await render(<TestDashboard />)
    await expect
      .element(screen.getByRole('alert'))
      .toHaveTextContent('SignalR startup failed')
    expect(orderRequests()).toHaveLength(0)
  })
})
