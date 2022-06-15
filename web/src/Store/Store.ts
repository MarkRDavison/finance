/* istanbul ignore file */

import { Action, applyMiddleware, combineReducers, createStore, Dispatch, Store } from "redux";
import thunk from "redux-thunk";
import { accountTypeReducer } from "./AccountTypeReducer";
import { bankReducer } from "./BankReducer";

const rootReducer = combineReducers({
    accountTypes: accountTypeReducer,
    banks: bankReducer
});

export type RootState = ReturnType<typeof rootReducer>;
export type GetStateType = () => RootState;
export type DispatchType = Dispatch<Action<unknown>>;

const createReduxStore = (): Store => createStore(
    rootReducer,
    applyMiddleware(thunk)
);

export default createReduxStore;