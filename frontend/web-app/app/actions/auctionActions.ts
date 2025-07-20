'use server';
import { auth } from "@/auth";
import { Auction, PagedResult } from "@/types";

export async function getData(query: string): Promise<PagedResult<Auction>> { // To be able to use async, a component must be a server component
    const res = await fetch(`http://localhost:6001/search${query}`);
    if (!res.ok) {
        throw new Error("Failed to fetch data");
    }
    return res.json();
}

export async function updateAuctionTest(): Promise<{status: number, message: string}> {
    const data = {
        mileage: Math.floor(Math.random() * 10000) + 1
    }

    const session = await auth();
    const res = await fetch('http://localhost:6001/auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json', // We're already sending JSON, so this is just for clarity
            'Authorization': `Bearer ${session?.accessToken}` // Any spelling here will result in a 401 Unauthorized error.
            // This access token will travel from Next js toGateway to Auction service to Identity Server, which will validate it and return the user information.
        },
        body: JSON.stringify(data)
    });
    if (!res.ok) return {status: res.status, message: res.statusText};
    return {status: res.status, message: res.statusText};
}