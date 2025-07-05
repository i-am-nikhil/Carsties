import { create } from "zustand";

type State = {
    pageNumber: number;
    pageSize: number;
    pageCount: number;
    searchTerm: string;
    orderBy: string;
    filterBy: string;
}

type Actions = {
    setParams: (params: Partial<State>) => void; // Partial is a type that makes all properties of State optional, allowing us to update only the properties we want.
    reset: () => void; // Reset the state to initial values.
}

const initialState: State = {
    pageNumber: 1,
    pageSize: 12,
    pageCount: 1,
    searchTerm: '',
    orderBy: 'make',
    filterBy: 'live'
}

export const useParamsStore = create<State & Actions>((set) => ({
    ...initialState,
    setParams: (newParams: Partial<State>) => {
        console.log('newParams', newParams);
        set((state) => { // state here is the current state of the store and is provided by zustand.
            if (newParams.pageNumber) {
                return { ...state, pageNumber: newParams.pageNumber };
            } else {
                return { ...state, ...newParams, pageNumber: 1}
            }
        })
    },
    reset: () => set(initialState)
}))