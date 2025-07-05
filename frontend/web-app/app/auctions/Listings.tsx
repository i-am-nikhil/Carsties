'use client';

import React, { useEffect, useState } from 'react'
import AuctionCard from './AuctionCard';
import AppPagination from '../components/AppPagination';
import { Auction, PagedResult } from '@/types';
import { getData } from '../actions/auctionActions';
import Filters from './Filters';
import { useParamsStore } from '@/hooks/useParamsStore';
import qs from 'query-string';
import { useShallow } from 'zustand/shallow';
import EmptyFilter from '../components/EmptyFilter';



export default function Listings() { // Removed async here since we made this a client component. Client components cannot be async, but they can use hooks like useEffect to fetch data asynchronously.
  const [data, setData] = useState<PagedResult<Auction>>();
  const params = useParamsStore(useShallow(state => ({ // useShallow is used to select only the properties we need from the store, which helps in performance by preventing unnecessary re-renders.
    pageNumber: state.pageNumber, // the value of state comes from the zustand store.
    pageSize: state.pageSize,
    searchTerm: state.searchTerm,
    orderBy: state.orderBy,
    filterBy: state.filterBy
  }))); // params will get updated every time the state in the zustand store changes. If only pageNumber changes, then only the pageNumber will be updated in params, and the component will re-render with the new value. This is because we are using useShallow to select only the properties we need from the store.
  const setParams = useParamsStore(state => state.setParams);
  const url = qs.stringifyUrl({url: '', query: params}, {skipEmptyString: true}); // when params gets updated, the url will also get updated.

  function setPageNumber(pageNumber: number) { 
    console.log('pageNumber', pageNumber);
    setParams({pageNumber});
    } 

  useEffect(() => { // useEffect is used to fetch data when the component mounts. It helps FE to be in sync with the server.
    getData(url).then(data => {
      setData(data);
    })
  }, [url]); //  The depency array mentions the variables that the effect depends on. If any of them changes, the effect will run again.

  if (!data) return <p>Loading...</p>
  return (
    <>
      <Filters />
      {data.totalCount === 0 ? (
        <EmptyFilter showReset title = 'No matches for this filter'/>
      ) : (
          <>
            <div className='grid grid-cols-4 gap-6'>
              {data && data.results.map((auction) => (
                <AuctionCard auction={auction} key={auction.id} />
              ))}
            </div>
            <div className='flex justify-center my-4'>
              <AppPagination pageChanged={setPageNumber}
                currentPage={params.pageNumber} pageCount={data.pageCount} />
            </div>
          </>
      )}
    </>
  )
}
