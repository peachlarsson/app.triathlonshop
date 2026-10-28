import { useState } from 'react'
import { type SortingState } from '@tanstack/react-table'
import { getRouteApi } from '@tanstack/react-router'
import { Header } from '@/components/layout/header'
import { Main } from '@/components/layout/main'
import { ThemeSwitch } from '@/components/theme-switch'
import { UsersDialogs } from './components/users-dialogs'
import { UsersPrimaryButtons } from './components/users-primary-buttons'
import { UsersProvider } from './components/users-provider'
import { UsersTable } from './components/users-table'
import { useCustomers } from './hooks/use-customers'

const route = getRouteApi('/customers/')

export function Users() {
  const search = route.useSearch()
  const navigate = route.useNavigate()
  const [sorting, setSorting] = useState<SortingState>([])
  const page = search.page ?? 1
  const pageSize = search.pageSize ?? 10
  const customerName = search.name ?? ''
  const sort = sorting[0]
  const sortKeys: Record<string, string> = {
    fullName: 'Name',
    email: 'Email',
    createdAt: 'CreatedAt',
  }

  const sortKey = sortKeys[sort?.id ?? ''] ?? 'Id'
  const sortDirection = sort?.desc ? 'Desc' : 'Asc'
  const {
    data: customers,
    error,
    isPending,
  } = useCustomers({
    page,
    pageSize,
    customerName,
    sortKey,
    sortDirection,
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
