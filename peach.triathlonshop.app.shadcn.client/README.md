# Peach Triathlon Shop UI

The client uses the shadcn-admin dashboard UI with React, TypeScript, Vite,
Tailwind CSS, and TanStack Router. The dashboard sample data and admin pages are
from the included shadcn-admin project; weather forecast functionality is not
included.

## Run locally

Install dependencies with `npm install`, then start the Vite development server
with `npm run dev`. The server uses the ASP.NET development HTTPS certificate,
which can be created with `dotnet dev-certs https --trust`.

Run `npm run build` to type-check and create a production build.

The UI includes demo dashboards and routes. Clerk-based routes require a
`VITE_CLERK_PUBLISHABLE_KEY` environment variable to connect to Clerk.
