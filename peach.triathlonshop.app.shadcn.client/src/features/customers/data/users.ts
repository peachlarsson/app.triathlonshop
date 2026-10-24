import { faker } from '@faker-js/faker'
import { roles } from './data'
import { type User } from './schema'

// Set a fixed seed for consistent data generation
faker.seed(67890)

export const users: User[] = Array.from({ length: 500 }, () => {
  return {
    id: faker.string.uuid(),
    fullName: faker.person.firstName(),
    email: faker.internet.email({ firstName: faker.person.firstName(), lastName: faker.person.lastName() }).toLocaleLowerCase(),
    role: faker.helpers.arrayElement(roles).value,
    createdAt: faker.date.past(),
    updatedAt: faker.date.recent(),
  }
})
