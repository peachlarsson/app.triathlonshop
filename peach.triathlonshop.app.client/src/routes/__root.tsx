import { createRootRoute } from '@tanstack/react-router'
import { NotFoundError } from '@/features/errors/not-found-error'
import App from '../App'

export const Route = createRootRoute({
    component: App,
    notFoundComponent: NotFoundError,
})