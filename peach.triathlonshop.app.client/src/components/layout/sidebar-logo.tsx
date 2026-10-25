import triathlonShopLogo from '@/assets/tri-logo-002.png'
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
          className='cursor-default gap-2 hover:bg-transparent active:bg-transparent'
          asChild
        >
          <div>
            <img
              src={triathlonShopLogo}
              alt='Triathlon Shop'
              className='size-10 shrink-0 rounded-lg bg-black object-contain'
            />
            <span className='truncate font-semibold'>Triathlon Shop</span>
          </div>
        </SidebarMenuButton>
      </SidebarMenuItem>
    </SidebarMenu>
  )
}
