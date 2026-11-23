import triathlonShopLogo from '@/assets/logo-anime-001.png'
import {
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from '@/components/ui/sidebar'

export function SidebarLogo() {
  return (
    <SidebarMenu>
      <SidebarMenuItem>
        <SidebarMenuButton
          size='lg'
          className='h-14 cursor-default gap-2 hover:bg-transparent active:bg-transparent'
          asChild
        >
          <div>
            <img
              src={triathlonShopLogo}
              alt='Triathlon Shop'
              className='size-12 shrink-0 rounded-lg bg-black object-contain group-data-[collapsible=icon]:size-8'
            />
            <span className='truncate font-semibold'>Triathlon Shop</span>
          </div>
        </SidebarMenuButton>
      </SidebarMenuItem>
    </SidebarMenu>
  )
}
