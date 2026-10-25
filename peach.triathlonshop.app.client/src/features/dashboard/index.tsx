import { useEffect, useState } from 'react'
import { HubConnectionBuilder, type HubConnection } from '@microsoft/signalr'
import { useQuery, useQueryClient } from '@tanstack/react-query'
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { Header } from '@/components/layout/header'
import { Main } from '@/components/layout/main'
import { ThemeSwitch } from '@/components/theme-switch'
import { Analytics } from './components/analytics'
import { Overview } from './components/overview'
import { RecentSales } from './components/recent-sales'
import type { DashboardData } from './data/schema'

interface SignalRConnectionInfo {
  url: string
  accessToken: string
}

interface HomePageUpdate {
  update?: boolean
  Update?: boolean
}

export function Dashboard() {
  const queryClient = useQueryClient()
  const [liveUpdateError, setLiveUpdateError] = useState<string>()

  useEffect(() => {
    const abortController = new AbortController()
    let connection: HubConnection | undefined

    const refreshDashboard = () => {
      void queryClient.invalidateQueries({ queryKey: ['dashboard'] })
    }

    const negotiate = async (): Promise<SignalRConnectionInfo> => {
      const response = await fetch('/api/homepage/negotiation/HomePage', {
        signal: abortController.signal,
      })
      if (!response.ok) {
        throw new Error(
          `Could not connect to dashboard live updates (HTTP ${response.status}).`
        )
      }

      return (await response.json()) as SignalRConnectionInfo
    }

    const connect = async () => {
      try {
        const connectionInfo = await negotiate()
        if (abortController.signal.aborted) return

        connection = new HubConnectionBuilder()
          .withUrl(connectionInfo.url, {
            accessTokenFactory: async () => (await negotiate()).accessToken,
          })
          .withAutomaticReconnect()
          .build()

        connection.on('homepage', (update: HomePageUpdate) => {
          if (!abortController.signal.aborted && (update.update ?? update.Update)) {
            refreshDashboard()
          }
        })
        connection.onreconnecting(() => {
          if (!abortController.signal.aborted) {
            setLiveUpdateError('Dashboard live updates are reconnecting.')
          }
        })
        connection.onreconnected(() => {
          if (!abortController.signal.aborted) {
            setLiveUpdateError(undefined)
            refreshDashboard()
          }
        })
        connection.onclose(() => {
          if (!abortController.signal.aborted) {
            setLiveUpdateError(
              'Dashboard live updates disconnected. Reload the page to reconnect.'
            )
          }
        })

        await connection.start()
        if (!abortController.signal.aborted) {
          setLiveUpdateError(undefined)
          refreshDashboard()
        }
      } catch (error) {
        if (!abortController.signal.aborted) {
          setLiveUpdateError(
            error instanceof Error
              ? error.message
              : 'Could not connect to dashboard live updates.'
          )
        }
      }
    }

    void connect()

    return () => {
      abortController.abort()
      if (connection) {
        void connection.stop().catch((error: unknown) => {
          console.error('Could not stop dashboard live updates.', error)
        })
      }
    }
  }, [queryClient])

  const {
    data: dashboardData,
    error,
    isPending,
  } = useQuery({
    queryKey: ['dashboard'],
    queryFn: async (): Promise<DashboardData> => {
      const response = await fetch('/api/dashboard')
      if (!response.ok) {
        throw new Error(
          `Could not load dashboard data (HTTP ${response.status}).`
        )
      }

      return (await response.json()) as DashboardData
    },
  })

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat('sv-SE', {
      style: 'currency',
      currency: 'SEK',
    }).format(value)

  return (
    <>
      {/* ===== Top Heading ===== */}
      <Header>
        <div className='ms-auto'>
          <ThemeSwitch />
      </div>
      </Header>

      {/* ===== Main ===== */}
      <Main>
        {liveUpdateError && (
          <p role='alert' className='mb-4 text-sm text-destructive'>
            {liveUpdateError}
          </p>
        )}
        <Tabs
          orientation='vertical'
          defaultValue='overview'
          className='space-y-4'
        >
          <div className='w-full overflow-x-auto pb-2'>
            <TabsList>
              <TabsTrigger value='overview'>Overview</TabsTrigger>
            </TabsList>
          </div>
          <TabsContent value='overview' className='space-y-4'>
            {error && (
              <p role='alert' className='text-sm text-destructive'>
                {error instanceof Error
                  ? error.message
                  : 'Could not load dashboard data.'}
              </p>
            )}
            <div className='grid gap-4 sm:grid-cols-2 lg:grid-cols-4'>
              <Card>
                <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                  <CardTitle className='text-sm font-medium'>
                    Total Revenue
                  </CardTitle>
                  <svg
                    xmlns='http://www.w3.org/2000/svg'
                    viewBox='0 0 24 24'
                    fill='none'
                    stroke='currentColor'
                    strokeLinecap='round'
                    strokeLinejoin='round'
                    strokeWidth='2'
                    className='h-4 w-4 text-muted-foreground'
                  >
                    <path d='M12 2v20M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6' />
                  </svg>
                </CardHeader>
                <CardContent>
                  <div className='text-2xl font-bold'>
                    {isPending
                      ? 'Loading...'
                      : dashboardData
                        ? formatCurrency(dashboardData.totalRevenue)
                        : '—'}
                  </div>
                  <p className='text-xs text-muted-foreground'>
                    Total revenue from shipped orders
                  </p>
                </CardContent>
              </Card>
              <Card>
                <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                  <CardTitle className='text-sm font-medium'>
                    Customers
                  </CardTitle>
                  <svg
                    xmlns='http://www.w3.org/2000/svg'
                    viewBox='0 0 24 24'
                    fill='none'
                    stroke='currentColor'
                    strokeLinecap='round'
                    strokeLinejoin='round'
                    strokeWidth='2'
                    className='h-4 w-4 text-muted-foreground'
                  >
                    <path d='M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2' />
                    <circle cx='9' cy='7' r='4' />
                    <path d='M22 21v-2a4 4 0 0 0-3-3.87M16 3.13a4 4 0 0 1 0 7.75' />
                  </svg>
                </CardHeader>
                <CardContent>
                  <div className='text-2xl font-bold'>
                    {isPending
                      ? 'Loading...'
                      : dashboardData
                        ? dashboardData.totalCustomers.toLocaleString()
                        : '—'}
                  </div>
                  <p className='text-xs text-muted-foreground'>
                    Registered customers
                  </p>
                </CardContent>
              </Card>
              <Card>
                <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                  <CardTitle className='text-sm font-medium'>Orders</CardTitle>
                  <svg
                    xmlns='http://www.w3.org/2000/svg'
                    viewBox='0 0 24 24'
                    fill='none'
                    stroke='currentColor'
                    strokeLinecap='round'
                    strokeLinejoin='round'
                    strokeWidth='2'
                    className='h-4 w-4 text-muted-foreground'
                  >
                    <rect width='20' height='14' x='2' y='5' rx='2' />
                    <path d='M2 10h20' />
                  </svg>
                </CardHeader>
                <CardContent>
                  <div className='text-2xl font-bold'>
                    {isPending
                      ? 'Loading...'
                      : dashboardData
                        ? dashboardData.totalOrders.toLocaleString()
                        : '—'}
                  </div>
                  <p className='text-xs text-muted-foreground'>
                    Total orders
                  </p>
                </CardContent>
              </Card>
              <Card>
                <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                  <CardTitle className='text-sm font-medium'>
                    Active Orders
                  </CardTitle>
                  <svg
                    xmlns='http://www.w3.org/2000/svg'
                    viewBox='0 0 24 24'
                    fill='none'
                    stroke='currentColor'
                    strokeLinecap='round'
                    strokeLinejoin='round'
                    strokeWidth='2'
                    className='h-4 w-4 text-muted-foreground'
                  >
                    <path d='M22 12h-4l-3 9L9 3l-3 9H2' />
                  </svg>
                </CardHeader>
                <CardContent>
                  <div className='text-2xl font-bold'>
                    {isPending
                      ? 'Loading...'
                      : dashboardData
                        ? dashboardData.activeOrders.toLocaleString()
                        : '—'}
                  </div>
                  <p className='text-xs text-muted-foreground'>
                    Created or confirmed
                  </p>
                </CardContent>
              </Card>
            </div>
            <div className='grid grid-cols-1 gap-4 lg:grid-cols-7'>
              <Card className='col-span-1 lg:col-span-4'>
                <CardHeader>
                  <CardTitle>Overview</CardTitle>
                  <CardDescription>Monthly revenue this year</CardDescription>
                </CardHeader>
                <CardContent className='ps-2'>
                  {isPending ? (
                    <p className='py-8 text-center text-sm text-muted-foreground'>
                      Loading revenue data...
                    </p>
                  ) : dashboardData ? (
                    <Overview monthlyRevenue={dashboardData.monthlyRevenue} />
                  ) : (
                    <p className='py-8 text-center text-sm text-muted-foreground'>
                      Revenue data is unavailable.
                    </p>
                  )}
                </CardContent>
              </Card>
              <Card className='col-span-1 lg:col-span-3'>
                <CardHeader>
                  <CardTitle>Recent Orders</CardTitle>
                  <CardDescription>
                    Orders placed in the last 30 days
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  {isPending ? (
                    <p className='py-8 text-center text-sm text-muted-foreground'>
                      Loading recent orders...
                    </p>
                  ) : dashboardData ? (
                    <RecentSales orders={dashboardData.recentOrdersLast30Days} />
                  ) : (
                    <p className='py-8 text-center text-sm text-muted-foreground'>
                      Recent orders are unavailable.
                    </p>
                  )}
                </CardContent>
              </Card>
            </div>
          </TabsContent>
          <TabsContent value='analytics' className='space-y-4'>
            <Analytics />
          </TabsContent>
        </Tabs>
      </Main>
    </>
  )
}
