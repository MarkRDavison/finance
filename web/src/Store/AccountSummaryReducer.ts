import { AccountListItemDto } from '@/Models/Queries/AccountList/AccountListItemDto';

const SET_ACCOUNT_SUMMARIES_PROGRESSING = 'SET_ACCOUNT_SUMMARIES_PROGRESSING';
const SET_ACCOUNT_SUMMARIES_FETCHED = 'SET_ACCOUNT_SUMMARIES_FETCHED';
const SET_ACCOUNT_SUMMARIES_ADDED = 'SET_ACCOUNT_SUMMARIES_ADDED';
const SET_ACCOUNT_SUMMARIES_REMOVED = 'SET_ACCOUNT_SUMMARIES_REMOVED';
const SET_ACCOUNT_SUMMARY_PROGRESS_ERROR = 'SET_ACCOUNT_SUMMARY_PROGRESS_ERROR';

export interface AccountSummaryState {
  accountSummaries: AccountListItemDto[];
  progressing: boolean;
  error: string | undefined;
  lastUpdated: number | undefined;
}

const initialState: AccountSummaryState = {
  accountSummaries: [],
  progressing: false,
  error: undefined,
  lastUpdated: undefined,
};

interface AccountSummarysFetchedAction {
  type: typeof SET_ACCOUNT_SUMMARIES_FETCHED;
  payload: AccountListItemDto[];
}

interface AccountSummarysAddedAction {
  type: typeof SET_ACCOUNT_SUMMARIES_ADDED;
  payload: AccountListItemDto[];
}

interface AccountSummarysFetchingAction {
  type: typeof SET_ACCOUNT_SUMMARIES_PROGRESSING;
}

interface AccountSummarysRemovedAction {
  type: typeof SET_ACCOUNT_SUMMARIES_REMOVED;
  payload: string[];
}

interface AccountSummaryFetchErrorAction {
  type: typeof SET_ACCOUNT_SUMMARY_PROGRESS_ERROR;
  payload: string;
}

export type AccountSummaryActionTypes =
  | AccountSummarysFetchedAction
  | AccountSummarysAddedAction
  | AccountSummarysFetchingAction
  | AccountSummarysRemovedAction
  | AccountSummaryFetchErrorAction;

export function setAccountSummarysFetched(
  accountSummaries: AccountListItemDto[],
): AccountSummaryActionTypes {
  return {
    type: SET_ACCOUNT_SUMMARIES_FETCHED,
    payload: accountSummaries,
  };
}

export function setAccountSummarysAdded(
  accountSummaries: AccountListItemDto[],
): AccountSummaryActionTypes {
  return {
    type: SET_ACCOUNT_SUMMARIES_ADDED,
    payload: accountSummaries,
  };
}

export function setAccountSummarysProgressing(): AccountSummaryActionTypes {
  return {
    type: SET_ACCOUNT_SUMMARIES_PROGRESSING,
  };
}

export function setAccountSummarysRemoved(
  ids: string[],
): AccountSummaryActionTypes {
  return {
    type: SET_ACCOUNT_SUMMARIES_REMOVED,
    payload: ids,
  };
}

export function setAccountSummaryFetchError(
  error: string,
): AccountSummaryActionTypes {
  return {
    type: SET_ACCOUNT_SUMMARY_PROGRESS_ERROR,
    payload: error,
  };
}

export function accountSummaryReducer(
  state = initialState,
  action: AccountSummaryActionTypes,
): AccountSummaryState {
  /* istanbul ignore next */
  if (action === undefined || action === null) {
    return state;
  }

  switch (action.type) {
    case SET_ACCOUNT_SUMMARIES_PROGRESSING:
      return {
        ...state,
        error: undefined,
        progressing: true,
        lastUpdated: Date.now(),
      };
    case SET_ACCOUNT_SUMMARIES_FETCHED:
      return {
        ...state,
        progressing: false,
        accountSummaries: action.payload,
        lastUpdated: Date.now(),
      };
    case SET_ACCOUNT_SUMMARIES_ADDED:
      return {
        ...state,
        accountSummaries: [...state.accountSummaries.concat(action.payload)],
        lastUpdated: Date.now(),
      };
    case SET_ACCOUNT_SUMMARIES_REMOVED:
      return {
        ...state,
        accountSummaries: [
          ...state.accountSummaries.filter(
            (o) => !action.payload.includes(o.id),
          ),
        ],
        lastUpdated: Date.now(),
      };
    case SET_ACCOUNT_SUMMARY_PROGRESS_ERROR:
      return {
        ...state,
        error: action.payload,
        progressing: false,
        lastUpdated: Date.now(),
      };
  }

  /* istanbul ignore next */
  return state;
}
