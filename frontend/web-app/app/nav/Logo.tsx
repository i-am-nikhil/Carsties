'use client'
import { useParamsStore } from '@/hooks/useParamsStore';
import { usePathname, useRouter } from 'next/navigation';
import React from 'react'
import { IoCarSportOutline } from 'react-icons/io5'

export default function Logo() {
  const router = useRouter();
  const pathName = usePathname();
  const reset = useParamsStore(state => state.reset);

  function handleReset() {
    if (pathName !== '/') {
      reset();
      router.push('/');
    }
  }

  return (
    <div onClick={handleReset} className="flex items-center gap-2 text-3xl font-semibold text-red-500 cursor-pointer">
            <IoCarSportOutline />
            <div>Cartsies Auctions</div>
          </div>
  )
}
