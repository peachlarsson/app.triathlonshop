import { useQuery } from '@tanstack/react-query'
import { type User } from '../data/schema'

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

export type CustomerSearchResponse = {
  data: { id: number; name: string | null }[]
}

type CustomerQuery = {
  page: number
  pageSize: number
  customerName: string
  sortKey: string
  sortDirection: string
}

export function useCustomers(query: CustomerQuery) {
  const { page, pageSize, customerName, sortKey, sortDirection } = query

  return useQuery({
    queryKey: [
      'customers',
      page,
      pageSize,
      customerName,
      sortKey,
      sortDirection,
    ],
    queryFn: async ({ signal }): Promise<PaginatedResponse<User>> => {
      const params = new URLSearchParams({
        pageNumber: String(page),
        pageSize: String(pageSize),
        customerName,
        sortKey,
        sortDirection,
      })
      const response = await fetch(`/api/customers?${params.toString()}`, {
        signal,
      })
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
}

export function useCustomerSearch(searchTerm: string, enabled: boolean) {
  return useQuery({
    queryKey: ['order-customer-search', searchTerm],
    enabled,
    queryFn: async ({ signal }): Promise<CustomerSearchResponse> => {
      const params = new URLSearchParams({
        customerName: searchTerm,
        pageNumber: '1',
        pageSize: '10',
      })
      const response = await fetch(`/api/customers?${params.toString()}`, {
        signal,
      })
      if (!response.ok) {
        throw new Error(`Could not search customers (HTTP ${response.status}).`)
      }
      return (await response.json()) as CustomerSearchResponse
    },
  })
}
