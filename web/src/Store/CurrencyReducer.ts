import { CurrencyDto } from '@/Models/Dtos/CurrencyDto';

const SET_CURRENCIES_PROGRESSING = 'SET_CURRENCIES_PROGRESSING';
const SET_CURRENCIES_FETCHED = 'SET_CURRENCIES_FETCHED';
const SET_CURRENCIES_ADDED = 'SET_CURRENCIES_ADDED';
const SET_CURRENCIES_REMOVED = 'SET_CURRENCIES_REMOVED';
const SET_CURRENCY_PROGRESS_ERROR = 'SET_CURRENCY_PROGRESS_ERROR';

export interface CurrencyState {
  currencies: CurrencyDto[];
  progressing: boolean;
  error: string | undefined;
}

const initialState: CurrencyState = {
  currencies: [],
  progressing: false,
  error: undefined,
};

interface CurrencysFetchedAction {
  type: typeof SET_CURRENCIES_FETCHED;
  payload: CurrencyDto[];
}

interface CurrencysAddedAction {
  type: typeof SET_CURRENCIES_ADDED;
  payload: CurrencyDto[];
}

interface CurrencysFetchingAction {
  type: typeof SET_CURRENCIES_PROGRESSING;
}

interface CurrencysRemovedAction {
  type: typeof SET_CURRENCIES_REMOVED;
  payload: string[];
}

interface CurrencyFetchErrorAction {
  type: typeof SET_CURRENCY_PROGRESS_ERROR;
  payload: string;
}

export type CurrencyActionTypes =
  | CurrencysFetchedAction
  | CurrencysAddedAction
  | CurrencysFetchingAction
  | CurrencysRemovedAction
  | CurrencyFetchErrorAction;

export function setCurrencysFetched(
  currencies: CurrencyDto[],
): CurrencyActionTypes {
  return {
    type: SET_CURRENCIES_FETCHED,
    payload: currencies,
  };
}

export function setCurrencysAdded(
  currencies: CurrencyDto[],
): CurrencyActionTypes {
  return {
    type: SET_CURRENCIES_ADDED,
    payload: currencies,
  };
}

export function setCurrencysProgressing(): CurrencyActionTypes {
  return {
    type: SET_CURRENCIES_PROGRESSING,
  };
}

export function setCurrencysRemoved(codes: string[]): CurrencyActionTypes {
  return {
    type: SET_CURRENCIES_REMOVED,
    payload: codes,
  };
}

export function setCurrencyFetchError(error: string): CurrencyActionTypes {
  return {
    type: SET_CURRENCY_PROGRESS_ERROR,
    payload: error,
  };
}

export function currencyReducer(
  state = initialState,
  action: CurrencyActionTypes,
): CurrencyState {
  /* istanbul ignore next */
  if (action === undefined || action === null) {
    return state;
  }

  switch (action.type) {
    case SET_CURRENCIES_PROGRESSING:
      return {
        ...state,
        error: undefined,
        progressing: true,
      };
    case SET_CURRENCIES_FETCHED:
      return {
        ...state,
        progressing: false,
        currencies: action.payload,
      };
    case SET_CURRENCIES_ADDED:
      return {
        ...state,
        currencies: [...state.currencies.concat(action.payload)],
      };
    case SET_CURRENCIES_REMOVED:
      return {
        ...state,
        currencies: [
          ...state.currencies.filter((o) => !action.payload.includes(o.code)),
        ],
      };
    case SET_CURRENCY_PROGRESS_ERROR:
      return {
        ...state,
        error: action.payload,
        progressing: false,
      };
  }

  /* istanbul ignore next */
  return state;
}
