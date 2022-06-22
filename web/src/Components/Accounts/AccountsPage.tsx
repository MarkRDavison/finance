import React, { useEffect } from 'react';
import * as zui from '@mark.davison/zeno-ui';
import styles from '@/Components/Accounts/AccountsPage.module.css';
import { connect } from 'react-redux';
import { DispatchType, RootState } from '@/Store/Store';
import {
  BankState
} from '@/Store/BankReducer';
import {
  AccountTypeState
} from '@/Store/AccountTypeReducer';
import { CurrencyState } from '@/Store/CurrencyReducer';
import CreateAccount from '@/Components/Accounts/CreateAccount/CreateAccount';
import { TableColumnProps } from '@mark.davison/zeno-ui/build/components/Table/Table.types';
import financeQueryService from '@/Services/FinanceQueryService';
import {
  AccountSummaryState,
  setAccountSummaryFetchError,
  setAccountSummarysFetched,
  setAccountSummarysProgressing,
} from '@/Store/AccountSummaryReducer';

interface StateProps {
  bankState: BankState;
  accountTypeState: AccountTypeState;
  currencyState: CurrencyState;
  accountSummaryState: AccountSummaryState;
}

interface DispatchProps {
  fetchAccounts: () => void;
}

type Props = StateProps & DispatchProps;

const _AccountsPage: React.FC<Props> = ({
  accountTypeState,
  bankState,
  currencyState,
  accountSummaryState,
  fetchAccounts,
}: Props) => {
  const [createOpen, setCreateOpen] = React.useState(false);

  useEffect(() => {
    if (
      accountSummaryState.progressing ||
      accountSummaryState.error !== undefined ||
      accountSummaryState.accountSummaries.length > 0
    ) {
      return;
    }

    if (accountSummaryState.lastUpdated !== undefined) {
      const now = Date.now();
      const offset = now - accountSummaryState.lastUpdated;
      if (offset <= 5000) {
        return;
      }
    }

    fetchAccounts();
  }, [fetchAccounts, accountSummaryState]);

  const currency = currencyState.currencies.find((_) => _.code === 'NZD');
  const accountTypes = accountTypeState.accountTypes.map((_) => ({
    id: _.id,
    text: _.type,
  }));
  const banks = bankState.banks.map((_) => ({
    id: _.id,
    text: _.name,
  }));
  const currencies = currencyState.currencies.map((_) => ({
    id: _.id,
    text: _.name,
  }));

  if (currency === undefined) {
    return <zui.Loading type="circular-infinite" />;
  }

  const ActiveTemplate = (active: boolean): JSX.Element => {
    return (
      <>{active ? 'ACTIVE' : 'INACTIVE'}</>
    );
  }

  const accountColumns: TableColumnProps[] = [
    { title: 'Name', width: '20%', type: 'string', field: 'name', align: 'right' },
    { title: 'Account Number', width: '20%', type: 'string', field: 'accountNumber', align: 'right' },
    { title: 'Account Type', width: '20%', type: 'string', field: 'accountType', align: 'right' },
    { title: 'Balance', width: '15%', type: 'number', field: 'currentBalance', align: 'right' },
    { title: 'Balance Difference', width: '15%', type: 'number', field: 'balanceDifference', align: 'right' },
    { title: 'Active', width: '10%', type: 'string', field: 'active', template: ActiveTemplate, align: 'right' }
  ];

  return (
    <div data-testid="AccountsPage" className={styles.accounts_container}>
      <h1>ACCOUNTS</h1>
      <zui.Button onClick={() => setCreateOpen(true)}>Add</zui.Button>
      <zui.Table
        idField="id"
        data={accountSummaryState.accountSummaries}
        columns={accountColumns}
      ></zui.Table>
      <zui.Modal
        open={createOpen}
        setOpen={setCreateOpen}
        closeWhenClickedOut={false}
      >
        <zui.ModalHeader title="Create account" />
        <zui.ModalContent>
          <div
            style={{
              margin: '0px 16px',
            }}
          >
            <CreateAccount
              currency={currency}
              accountTypes={accountTypes}
              banks={banks}
              currencies={currencies}
              close={() => setCreateOpen(false)}
            />
          </div>
        </zui.ModalContent>
      </zui.Modal>
    </div>
  );
};

const mapStateToProps = (state: RootState): StateProps => {
  return {
    accountTypeState: state.accountTypes,
    bankState: state.banks,
    currencyState: state.currencies,
    accountSummaryState: state.accountSummaries,
  };
};

/* istanbul ignore next */
const mapDispatchToProps = (dispatch: DispatchType): DispatchProps => {
  return {
    fetchAccounts: () => {
      dispatch(setAccountSummarysProgressing());
      financeQueryService
        .accountList({
          showActive: true,
        })
        .then((response) => {
          dispatch(setAccountSummarysFetched(response.accounts));
        })
        .catch((e) => {
          dispatch(setAccountSummaryFetchError(e.message));
        });
    },
  };
};

const AccountsPage = connect(
  mapStateToProps,
  mapDispatchToProps,
)(_AccountsPage);
export default AccountsPage;
