import EmptyFilter from '@/app/components/EmptyFilter'
import React from 'react'

export default function SignIn({searchParams}: {searchParams: {callbackUrl: string}}) {
  return (
    <EmptyFilter
        title='You must be signed in to access this page'
        subtitle='Please click the button below to sign in'
        showLogin
        callbackUrl={searchParams.callbackUrl}
    />
  )
}
