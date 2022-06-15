import { SearchItem } from "@/Models/SearchItem";

const SET_ACCOUNT_TYPES_PROGRESSING = 'SET_ACCOUNT_TYPES_PROGRESSING';
const SET_ACCOUNT_TYPES_FETCHED = 'SET_ACCOUNT_TYPES_FETCHED';
const SET_ACCOUNT_TYPES_ADDED = 'SET_ACCOUNT_TYPES_ADDED';
const SET_ACCOUNT_TYPES_REMOVED = 'SET_ACCOUNT_TYPES_REMOVED';
const SET_ACCOUNT_TYPE_PROGRESS_ERROR = 'SET_ACCOUNT_TYPE_PROGRESS_ERROR';

export interface AccountTypeState {
    accountTypes: SearchItem[]
    progressing: boolean
    error: string | undefined
};

const initialState: AccountTypeState = {
    accountTypes: [],
    progressing: false,
    error: undefined
};

interface AccountTypesFetchedAction {
    type: typeof SET_ACCOUNT_TYPES_FETCHED,
    payload: SearchItem[]
};

interface AccountTypesAddedAction {
    type: typeof SET_ACCOUNT_TYPES_ADDED,
    payload: SearchItem[]
};

interface AccountTypesFetchingAction {
    type: typeof SET_ACCOUNT_TYPES_PROGRESSING
};

interface AccountTypesRemovedAction {
    type: typeof SET_ACCOUNT_TYPES_REMOVED
    payload: string[]
};

interface AccountTypeFetchErrorAction {
    type: typeof SET_ACCOUNT_TYPE_PROGRESS_ERROR
    payload: string
}

export type AccountTypeActionTypes =
    AccountTypesFetchedAction |
    AccountTypesAddedAction |
    AccountTypesFetchingAction |
    AccountTypesRemovedAction |
    AccountTypeFetchErrorAction;

export function setAccountTypesFetched(accountTypes: SearchItem[]): AccountTypeActionTypes {
    return {
        type: SET_ACCOUNT_TYPES_FETCHED,
        payload: accountTypes
    };
};

export function setAccountTypesAdded(accountTypes: SearchItem[]): AccountTypeActionTypes {
    return {
        type: SET_ACCOUNT_TYPES_ADDED,
        payload: accountTypes
    };
};

export function setAccountTypesProgressing(): AccountTypeActionTypes {
    return {
        type: SET_ACCOUNT_TYPES_PROGRESSING
    };
};

export function setAccountTypesRemoved(ids: string[]): AccountTypeActionTypes {
    return {
        type: SET_ACCOUNT_TYPES_REMOVED,
        payload: ids
    };
}

export function setAccountTypeFetchError(error: string): AccountTypeActionTypes {
    return {
        type: SET_ACCOUNT_TYPE_PROGRESS_ERROR,
        payload: error
    };
};

export function accountTypeReducer(
    state = initialState,
    action: AccountTypeActionTypes
): AccountTypeState {
    /* istanbul ignore next */
    if (action === undefined || action === null) {
        return state;
    }

    switch (action.type) {
        case SET_ACCOUNT_TYPES_PROGRESSING:
            return {
                ...state,
                error: undefined,
                progressing: true
            };
        case SET_ACCOUNT_TYPES_FETCHED:
            return {
                ...state,
                progressing: false,
                accountTypes: action.payload
            };
        case SET_ACCOUNT_TYPES_ADDED:
            return {
                ...state,
                accountTypes: [...state.accountTypes.concat(action.payload)]
            };
        case SET_ACCOUNT_TYPES_REMOVED:
            return {
                ...state,
                accountTypes: [...state.accountTypes.filter(o => !action.payload.includes(o.id))]
            };
        case SET_ACCOUNT_TYPE_PROGRESS_ERROR:
            return {
                ...state,
                error: action.payload,
                progressing: false
            };
    }

    /* istanbul ignore next */
    return state;
}