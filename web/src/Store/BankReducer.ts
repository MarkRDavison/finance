import { SearchItem } from "@/Models/SearchItem";

const SET_BANKS_PROGRESSING = 'SET_BANKS_PROGRESSING';
const SET_BANKS_FETCHED = 'SET_BANKS_FETCHED';
const SET_BANKS_ADDED = 'SET_BANKS_ADDED';
const SET_BANKS_REMOVED = 'SET_BANKS_REMOVED';
const SET_BANK_PROGRESS_ERROR = 'SET_BANK_PROGRESS_ERROR';

export interface BankState {
    banks: SearchItem[]
    progressing: boolean
    error: string | undefined
};

const initialState: BankState = {
    banks: [],
    progressing: false,
    error: undefined
};

interface BanksFetchedAction {
    type: typeof SET_BANKS_FETCHED,
    payload: SearchItem[]
};

interface BanksAddedAction {
    type: typeof SET_BANKS_ADDED,
    payload: SearchItem[]
};

interface BanksFetchingAction {
    type: typeof SET_BANKS_PROGRESSING
};

interface BanksRemovedAction {
    type: typeof SET_BANKS_REMOVED
    payload: string[]
};

interface BankFetchErrorAction {
    type: typeof SET_BANK_PROGRESS_ERROR
    payload: string
}

export type BankActionTypes =
    BanksFetchedAction |
    BanksAddedAction |
    BanksFetchingAction |
    BanksRemovedAction |
    BankFetchErrorAction;

export function setBanksFetched(banks: SearchItem[]): BankActionTypes {
    return {
        type: SET_BANKS_FETCHED,
        payload: banks
    };
};

export function setBanksAdded(banks: SearchItem[]): BankActionTypes {
    return {
        type: SET_BANKS_ADDED,
        payload: banks
    };
};

export function setBanksProgressing(): BankActionTypes {
    return {
        type: SET_BANKS_PROGRESSING
    };
};

export function setBanksRemoved(ids: string[]): BankActionTypes {
    return {
        type: SET_BANKS_REMOVED,
        payload: ids
    };
}

export function setBankFetchError(error: string): BankActionTypes {
    return {
        type: SET_BANK_PROGRESS_ERROR,
        payload: error
    };
};

export function bankReducer(
    state = initialState,
    action: BankActionTypes
): BankState {
    /* istanbul ignore next */
    if (action === undefined || action === null) {
        return state;
    }

    switch (action.type) {
        case SET_BANKS_PROGRESSING:
            return {
                ...state,
                error: undefined,
                progressing: true
            };
        case SET_BANKS_FETCHED:
            return {
                ...state,
                progressing: false,
                banks: action.payload
            };
        case SET_BANKS_ADDED:
            return {
                ...state,
                banks: [...state.banks.concat(action.payload)]
            };
        case SET_BANKS_REMOVED:
            return {
                ...state,
                banks: [...state.banks.filter(o => !action.payload.includes(o.id))]
            };
        case SET_BANK_PROGRESS_ERROR:
            return {
                ...state,
                error: action.payload,
                progressing: false
            };
    }

    /* istanbul ignore next */
    return state;
}