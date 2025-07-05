'use client';
import React from 'react'
import Image from 'next/image'

type Props = {
    imageUrl: string;
    }
export default function CarImage({imageUrl}: Props) {
    const [loading, setLoading] = React.useState(true);
  return (
    <Image
          src={imageUrl}
          alt='image'
          fill // Fill the parent container
          priority // Preload the image
          className={
            `object-cover duration-700 ease-in-out ${loading ? 'opacity-0 scale-110' : 'opacity-100 scale-100'}`
          }
          sizes='(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 25vw' // Responsive sizes
          unoptimized
            onLoad={() => setLoading(false)}
        />
  )
}
