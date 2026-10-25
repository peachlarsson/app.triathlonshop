import { useLayoutEffect, useRef } from 'react'

interface AnimatedNumberProps {
  value: number
  format?: (value: number) => string
}

export function AnimatedNumber({ value, format }: AnimatedNumberProps) {
  const text = format ? format(value) : value.toLocaleString()
  const previous = useRef({ value, text })
  const currentElement = useRef<HTMLSpanElement>(null)
  const previousElement = useRef<HTMLSpanElement>(null)
  const entranceAnimation = useRef<Animation | undefined>(undefined)

  useLayoutEffect(() => {
    if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) return

    const animation = currentElement.current?.animate(
      [
        { transform: 'translateY(100%)', opacity: 0 },
        { transform: 'translateY(0)', opacity: 1 },
      ],
      {
        duration: 450,
        easing: 'cubic-bezier(0.22, 1, 0.36, 1)',
      }
    )
    entranceAnimation.current = animation

    return () => animation?.cancel()
  }, [])

  useLayoutEffect(() => {
    const old = previous.current
    previous.current = { value, text }

    if (
      old.value === value ||
      old.text === text ||
      window.matchMedia('(prefers-reduced-motion: reduce)').matches
    ) {
      return
    }

    const current = currentElement.current
    const outgoing = previousElement.current
    if (!current || !outgoing) return

    entranceAnimation.current?.cancel()
    outgoing.textContent = old.text
    const offset = value > old.value ? '100%' : '-100%'
    const exitOffset = value > old.value ? '-100%' : '100%'
    const options: KeyframeAnimationOptions = {
      duration: 450,
      easing: 'cubic-bezier(0.22, 1, 0.36, 1)',
    }
    const incomingAnimation = current.animate(
      [
        { transform: `translateY(${offset})`, opacity: 0 },
        { transform: 'translateY(0)', opacity: 1 },
      ],
      options
    )
    const outgoingAnimation = outgoing.animate(
      [
        { transform: 'translateY(0)', opacity: 1 },
        { transform: `translateY(${exitOffset})`, opacity: 0 },
      ],
      options
    )

    return () => {
      incomingAnimation.cancel()
      outgoingAnimation.cancel()
    }
  }, [value, text])

  return (
    <span className='relative inline-flex overflow-hidden align-bottom tabular-nums'>
      <span className='sr-only'>{text}</span>
      <span ref={currentElement} aria-hidden='true' className='inline-block'>
        {text}
      </span>
      <span
        ref={previousElement}
        aria-hidden='true'
        className='pointer-events-none absolute inset-0 inline-block whitespace-nowrap opacity-0'
      />
    </span>
  )
}
