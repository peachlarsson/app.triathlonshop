import { Area, AreaChart, ResponsiveContainer, XAxis, YAxis } from 'recharts'

const data = [
  {
    name: 'Mon',
    clicks: 0,
    uniques: 0,
  },
  {
    name: 'Tue',
    clicks: 0,
    uniques: 0,
  },
  {
    name: 'Wed',
    clicks: 0,
    uniques: 0,
  },
  {
    name: 'Thu',
    clicks: 0,
    uniques: 0,
  },
  {
    name: 'Fri',
    clicks: 0,
    uniques: 0,
  },
  {
    name: 'Sat',
    clicks: 0,
    uniques: 0,
  },
  {
    name: 'Sun',
    clicks: 0,
    uniques: 0,
  },
]

export function AnalyticsChart() {
  return (
    <ResponsiveContainer width='100%' height={300}>
      <AreaChart data={data}>
        <XAxis
          dataKey='name'
          stroke='#888888'
          fontSize={12}
          tickLine={false}
          axisLine={false}
        />
        <YAxis
          stroke='#888888'
          fontSize={12}
          tickLine={false}
          axisLine={false}
        />
        <Area
          type='monotone'
          dataKey='clicks'
          stroke='currentColor'
          className='text-primary'
          fill='currentColor'
          fillOpacity={0.15}
        />
        <Area
          type='monotone'
          dataKey='uniques'
          stroke='currentColor'
          className='text-muted-foreground'
          fill='currentColor'
          fillOpacity={0.1}
        />
      </AreaChart>
    </ResponsiveContainer>
  )
}
