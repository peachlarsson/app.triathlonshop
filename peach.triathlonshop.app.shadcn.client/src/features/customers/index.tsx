import { useState } from 'react'
import { type SortingState } from '@tanstack/react-table'
import { useQuery } from '@tanstack/react-query'
import { getRouteApi } from '@tanstack/react-router'
import { Header } from '@/components/layout/header'
import { Main } from '@/components/layout/main'
import { ThemeSwitch } from '@/components/theme-switch'
import { UsersDialogs } from './components/users-dialogs'
import { UsersPrimaryButtons } from './components/users-primary-buttons'
import { UsersProvider } from './components/users-provider'
import { UsersTable } from './components/users-table'
import { type User } from './data/schema'

const route = getRouteApi('/_authenticated/customers/')

type CustomerResponse = {
  id: number
  name: string | null
  email: string | null
  isCompany: boolean
  createdAt: string
  updatedAt: string | null
}

type PaginatedResponse<T> = {
  data: T[]
  totalCount: number
}

export function Users() {
  const search = route.useSearch()
  const navigate = route.useNavigate()
  const [sorting, setSorting] = useState<SortingState>([])
  const page = search.page ?? 1
  const pageSize = search.pageSize ?? 10
  const customerName = search.name ?? ''
  const sort = sorting[0]
  const sortBy =
    sort?.id === 'fullName'
      ? 'Name'
      : sort?.id === 'email'
        ? 'Email'
        : sort?.id === 'createdAt'
          ? 'CreatedAt'
          : 'Id'
  const sortOrder = sort?.desc ? 'Desc' : 'Asc'
  const {
    data: customers,
    error,
    isPending,
  } = useQuery({
    queryKey: [
      'customers',
      page,
      pageSize,
      customerName,
      sortBy,
      sortOrder,
    ],
    queryFn: async (): Promise<PaginatedResponse<User>> => {
      const params = new URLSearchParams({
        pageNumber: String(page),
        pageSize: String(pageSize),
        customerName,
        sortBy,
        sortOrder,
      })

      const response = await fetch(`/api/customers?${params.toString()}`)
      if (!response.ok) {
        throw new Error(`Could not load customers (HTTP ${response.status}).`)
      }

      const result =
        (await response.json()) as PaginatedResponse<CustomerResponse>
      return {
        ...result,
        data: result.data.map((customer) => ({
          id: String(customer.id),
          fullName: customer.name ?? '',
          email: customer.email ?? '',
          role: customer.isCompany ? 'companyCustomer' : 'privateCustomer',
          createdAt: new Date(customer.createdAt),
          updatedAt: customer.updatedAt ? new Date(customer.updatedAt) : null,
        })),
      }
    },
  })

  return (
    <UsersProvider>
      <Header>
        <div className='ms-auto'>
          <ThemeSwitch />
        </div>
      </Header>

      <Main className='flex flex-1 flex-col gap-4 sm:gap-6'>
        <div className='flex flex-wrap items-end justify-between gap-2'>
          <div>
            <h2 className='text-2xl font-bold tracking-tight'>Customers List</h2>
            <p className='text-muted-foreground'>
              Manage customers.
            </p>
          </div>
          <UsersPrimaryButtons />
        </div>
        <UsersTable
          data={customers?.data ?? []}
          totalCount={customers?.totalCount ?? 0}
          isLoading={isPending}
          error={error instanceof Error ? error.message : undefined}
          sorting={sorting}
          onSortingChange={setSorting}
          search={search}
          navigate={navigate}
        />
      </Main>

      <UsersDialogs />
    </UsersProvider>
  )
}
