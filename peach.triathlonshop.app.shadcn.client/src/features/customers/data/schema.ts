import { z } from 'zod'

const userRoleSchema = z.union([
  z.literal('companyCustomer'),
  z.literal('privateCustomer'),
])

const _userSchema = z.object({
  id: z.string(),
  fullName: z.string(),
  email: z.string(),
  role: userRoleSchema,
  createdAt: z.coerce.date(),
  updatedAt: z.coerce.date(),
})
export type User = z.infer<typeof _userSchema>
