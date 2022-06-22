import React, { useEffect } from 'react';
import styles from '@/App.module.css';
import { Route, BrowserRouter, Routes, Navigate } from 'react-router-dom';
import PrivateComponent from '@/Components/PrivateComponent';
import HomePage from '@/Components/HomePage';
import Navbar, { NavbarItem } from '@/Components/Navbar';
import AccountsPage from '@/Components/Accounts/AccountsPage';
import { connect } from 'react-redux';
import { DispatchType, RootState } from '@/Store/Store';
import {
  CurrencyState,
  setCurrencyFetchError,
  setCurrencysFetched,
  setCurrencysProgressing,
} from '@/Store/CurrencyReducer';
import {
  AccountTypeState,
  setAccountTypeFetchError,
  setAccountTypesFetched,
  setAccountTypesProgressing,
} from '@/Store/AccountTypeReducer';
import {
  BankState,
  setBankFetchError,
  setBanksFetched,
  setBanksProgressing,
} from '@/Store/BankReducer';
import financeQueryService from '@/Services/FinanceQueryService';
import { useAuth } from '@mark.davison/zeno-common-client';

const WrapPrivate = (element: JSX.Element): JSX.Element => (
  <PrivateComponent>{element}</PrivateComponent>
);

const navbarItems: NavbarItem[] = [
  { name: 'Finance', route: '/' },
  { name: 'Accounts', route: '/accounts' },
];

interface StateProps {
  accountTypeState: AccountTypeState;
  bankState: BankState;
  currencyState: CurrencyState;
}

interface DispatchProps {
  fetchStartup: () => void;
}

type Props = StateProps & DispatchProps;

const _App: React.FC<Props> = ({
  accountTypeState,
  bankState,
  currencyState,
  fetchStartup,
}: Props) => {

  const {
    isLoggedIn,
    user
  } = useAuth();

  useEffect(() => {
    if (
      accountTypeState.progressing &&
      bankState.progressing &&
      currencyState.progressing
    ) {
      return;
    }

    if (
      accountTypeState.error !== undefined &&
      bankState.error !== undefined &&
      currencyState.error !== undefined
    ) {
      return;
    }

    if (
      accountTypeState.accountTypes.length > 0 &&
      bankState.banks.length > 0 &&
      currencyState.currencies.length > 0
    ) {
      return;
    }

    fetchStartup();
  }, [fetchStartup, accountTypeState, bankState, currencyState]);

  return (
    <div className={styles['app-container']}>
      <BrowserRouter>
        <Navbar items={navbarItems} />
        <div className={styles['app-content']}>
          <Routes>
            <Route path="/" element={WrapPrivate(<HomePage />)} />
            <Route path="/accounts" element={WrapPrivate(<AccountsPage />)} />
            <Route path="*" element={<Navigate to="/" />} />
          </Routes>
        </div>
      </BrowserRouter>
    </div>
  );
};
const mapStateToProps = (state: RootState): StateProps => {
  return {
    accountTypeState: state.accountTypes,
    bankState: state.banks,
    currencyState: state.currencies,
  };
};

/* istanbul ignore next */
const mapDispatchToProps = (dispatch: DispatchType): DispatchProps => {
  return {
    fetchStartup: () => {
      dispatch(setAccountTypesProgressing());
      dispatch(setBanksProgressing());
      dispatch(setCurrencysProgressing());

      financeQueryService
        .startup({})
        .then((response) => {
          dispatch(setAccountTypesFetched(response.accountTypes));
          dispatch(setBanksFetched(response.banks));
          dispatch(setCurrencysFetched(response.currencies));
        })
        .catch((e) => {
          dispatch(setAccountTypeFetchError(e.message));
          dispatch(setBankFetchError(e.message));
          dispatch(setCurrencyFetchError(e.message));
        });
    },
  };
};

const App = connect(mapStateToProps, mapDispatchToProps)(_App);
export default App;
