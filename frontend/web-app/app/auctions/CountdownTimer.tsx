'use client'; // If there is a functionality that requires client-side rendering, this file should be a client component.

import React from 'react'
import Countdown, { zeroPad } from 'react-countdown';

const renderer = ({ days, hours, minutes, seconds, completed }: 
    { days: number, hours: number, minutes: number, seconds: number, completed: boolean }) => {

        return (
            <div className={`border-2 border-white text-white py-1 px-2 rounded-lg flex justify-center 
            ${completed ? 'bg-red-600' : (days === 0 && hours < 10) ? 'bg-amber-600' :' bg-green-600'} `}>
                {completed ? (
                    <span>Auction finished</span>) : 
                    (<span suppressHydrationWarning={true}> 
                        {days}:{zeroPad(hours)}:{zeroPad(minutes)}:{zeroPad(seconds)}
                    </span>)} 
            </div>
        )
  }; // suppressHydrationWarning is necessary because whenever the countdown is rendered, it will show a different value on the server and client side, causing a hydration error.

type Props = {
    auctionEnd: string;
}

export default function CountdownTimer({auctionEnd}: Props) {
  return (
    <div>
        <Countdown date={auctionEnd} renderer={renderer}/>
    </div>
  )
}
