/* istanbul ignore file */

import {
  Action,
  applyMiddleware,
  combineReducers,
  createStore,
  Dispatch,
  Store,
} from 'redux';
import thunk from 'redux-thunk';
import { accountTypeReducer } from '@/Store/AccountTypeReducer';
import { bankReducer } from '@/Store/BankReducer';
import { currencyReducer } from '@/Store/CurrencyReducer';
import { accountSummaryReducer } from '@/Store/AccountSummaryReducer';

const rootReducer = combineReducers({
  accountTypes: accountTypeReducer,
  banks: bankReducer,
  currencies: currencyReducer,
  accountSummaries: accountSummaryReducer,
});

export type RootState = ReturnType<typeof rootReducer>;
export type GetStateType = () => RootState;
export type DispatchType = Dispatch<Action<unknown>>;

const createReduxStore = (): Store =>
  createStore(rootReducer, applyMiddleware(thunk));

export default createReduxStore;
