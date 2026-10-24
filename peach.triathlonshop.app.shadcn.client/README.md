# Peach Triathlon Shop UI

The client uses the shadcn-admin dashboard UI with React, TypeScript, Vite,
Tailwind CSS, and TanStack Router. The dashboard sample data and admin pages are
from the included shadcn-admin project; weather forecast functionality is not
included.

## Run locally

Install dependencies with `npm install`, then start the Vite development server
with `npm run dev`. Vite serves the UI at `http://localhost:5173` and fails if
that port is already in use, keeping it aligned with the ASP.NET SPA proxy and
VS Code browser launch settings.

When starting the ASP.NET server, the SPA proxy starts Vite automatically if it
is not already running. Restart the ASP.NET server after changing its SPA proxy
settings.

API requests under `/api` are proxied to `https://localhost:7135` by default.
Start the ASP.NET server with its `https` launch profile, and trust its development
certificate with `dotnet dev-certs https --trust`. To use another backend address,
set `VITE_API_PROXY_TARGET` before starting Vite.

Run `npm run build` to type-check and create a production build.

The UI includes demo dashboards and routes. Clerk-based routes require a
`VITE_CLERK_PUBLISHABLE_KEY` environment variable to connect to Clerk.
