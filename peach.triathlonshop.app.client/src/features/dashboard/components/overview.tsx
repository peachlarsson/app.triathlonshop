import { Bar, BarChart, ResponsiveContainer, XAxis, YAxis } from 'recharts'
import type { MonthlyRevenue } from '../data/schema'

const months = [
  'Jan',
  'Feb',
  'Mar',
  'Apr',
  'May',
  'Jun',
  'Jul',
  'Aug',
  'Sep',
  'Oct',
  'Nov',
  'Dec',
]

interface OverviewProps {
  monthlyRevenue: MonthlyRevenue[]
}

export function Overview({ monthlyRevenue }: OverviewProps) {
  const revenueByMonth = new Map(
    monthlyRevenue.map(({ month, revenue }) => [month, revenue])
  )
  const data = months.map((name, index) => ({
    name,
    total: revenueByMonth.get(index + 1) ?? 0,
  }))

  return (
    <ResponsiveContainer width='100%' height={350}>
      <BarChart data={data}>
        <XAxis
          dataKey='name'
          stroke='#888888'
          fontSize={12}
          tickLine={false}
          axisLine={false}
        />
        <YAxis
          direction='ltr'
          stroke='#888888'
          fontSize={12}
          tickLine={false}
          axisLine={false}
          tickFormatter={(value) =>
            new Intl.NumberFormat('sv-SE', {
              style: 'currency',
              currency: 'SEK',
              maximumFractionDigits: 0,
            }).format(Number(value))
          }
        />
        <Bar
          dataKey='total'
          fill='currentColor'
          radius={[4, 4, 0, 0]}
          className='fill-primary'
        />
      </BarChart>
    </ResponsiveContainer>
  )
}
