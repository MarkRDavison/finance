import React, { useEffect } from 'react';
import * as zui from '@mark.davison/zeno-ui';
import styles from '@/Components/Accounts/AccountsPage.module.css';
import { SearchItem } from '@/Models/SearchItem';
import axios from 'axios';
import { connect } from 'react-redux';
import { DispatchType, RootState } from '@/Store/Store';
import { BankState, setBankFetchError, setBanksFetched, setBanksProgressing } from '@/Store/BankReducer';
import { AccountTypeState, setAccountTypeFetchError, setAccountTypesFetched, setAccountTypesProgressing } from '@/Store/AccountTypeReducer';

interface StateProps {
    bankSearchState: BankState,
    accountTypeSearchState: AccountTypeState
}

interface DispatchProps {
    fetchAccountTypesAndBanks: () => void
}

interface OwnProps {

}

type Props = StateProps & DispatchProps & OwnProps;

interface CreateAccountFormData {
    name: string
    accountNumber: string
    virtualBalance?: number
    bankId: string
    accountTypeId: string
}

interface CreateAccountResponse {
    success: boolean
    error: string[]
    warning: string[]
}


const Validator: zui.Validate<CreateAccountFormData> = (
    values,
    touched,
    required,
    reason
): zui.ValidationResult<CreateAccountFormData> => {
    const errors: zui.Errors<CreateAccountFormData> = {
        name: undefined,
        accountNumber: undefined,
        virtualBalance: undefined,
        bankId: undefined,
        accountTypeId: undefined
    };

    return {
        errors: errors,
        required,
    };
}

export interface DropdownItem {
    id: string;
    text?: number | string;
}

const _AccountsPage: React.FC<Props> = ({
    fetchAccountTypesAndBanks,
    accountTypeSearchState,
    bankSearchState
}: Props): JSX.Element => {
    const initialValues: CreateAccountFormData = {
        name: '',
        accountNumber: '',
        virtualBalance: undefined,
        bankId: '',
        accountTypeId: ''
    };
    const required: zui.Required<CreateAccountFormData> = {
        name: true,
        accountNumber: false,
        virtualBalance: false,
        bankId: true,
        accountTypeId: true
    };

    const bankDropdownItems: DropdownItem[] = bankSearchState.banks.map(_ => {
        return {
            id: _.id,
            text: _.primaryText
        };
    });
    const accountTypeDropdownItems: DropdownItem[] = accountTypeSearchState.accountTypes.map(_ => {
        return {
            id: _.id,
            text: _.primaryText
        };
    });

    const handleSubmit = (values: CreateAccountFormData): Promise<boolean> => {
        axios.post('/api/create-account', {
            ...values,
            bankId: bankDropdownItems.find(_ => _.text === values.bankId)?.id,
            accountTypeId: accountTypeDropdownItems.find(_ => _.text === values.accountTypeId)?.id
        }).then(_ => {
            const createAccountResponse = _.data as CreateAccountResponse;
            console.log(JSON.stringify(createAccountResponse, null, 2));
        });
        return Promise.resolve(false);
    }

    const FormContent = ({

    }: zui.FormProviderProps<CreateAccountFormData>) => {
        return (
            <zui.BaseForm>
                <zui.BaseTextInput name="name" label="Name" maxLength={255} />
                <zui.BaseErrorMessage name="accountNumber" />
                <zui.BaseTextInput name="accountNumber" label="Account Number" maxLength={255} />
                <zui.BaseErrorMessage name="name" />
                <zui.BaseDropdownListInput name='bankId' placeholder='Select bank' label='Bank' items={bankDropdownItems} />
                <zui.BaseErrorMessage name="bankId" />
                <zui.BaseDropdownListInput name='accountTypeId' placeholder='Select account type' label='Account Type' items={accountTypeDropdownItems} />
                <zui.BaseErrorMessage name="accountTypeId" />
                <zui.BaseNumericInput name='virtualBalance' decimalPlaces={2 /*TODO: Currency*/} label='Virtual Balance' />
                <zui.BaseErrorMessage name='virtualBalance' />
                <zui.BaseButton type="submit" />
            </zui.BaseForm>
        );
    };

    useEffect(() => {
        if (accountTypeSearchState.progressing ||
            bankSearchState.progressing) {
            console.log('account types / banks already in progress');
            return;
        }

        if (accountTypeSearchState.error !== undefined ||
            bankSearchState.error !== undefined) {
            console.log('account types / banks in error');
            return;
        }

        if (accountTypeSearchState.accountTypes.length > 0 ||
            bankSearchState.banks.length > 0) {
            console.log('account types / banks already fetched');
            return;
        }

        console.log('fetching account types and banks');
        fetchAccountTypesAndBanks()
    }, [fetchAccountTypesAndBanks, accountTypeSearchState, bankSearchState]);

    return (
        <div data-testid="AccountsPage" className={styles.accounts_container}>
            <h1>ACCOUNTS</h1>
            <div data-testid='TempAddAccount'>
                <zui.Form<CreateAccountFormData>
                    initialValues={initialValues}
                    required={required}
                    handleSubmit={handleSubmit}
                    validator={Validator}
                    childData={FormContent} />
            </div>
        </div>
    );
};

const mapStateToProps = (state: RootState): StateProps => {
    return {
        accountTypeSearchState: state.accountTypes,
        bankSearchState: state.banks
    };
};

/* istanbul ignore next */
const mapDispatchToProps = (dispatch: DispatchType): DispatchProps => {
    return {
        fetchAccountTypesAndBanks: () => {
            dispatch(setBanksProgressing())
            dispatch(setAccountTypesProgressing())
            axios.get('/api/create-account-query').then(response => {
                dispatch(setAccountTypesFetched(response.data.accountTypes));
                dispatch(setBanksFetched(response.data.banks));
            }).catch(e => {
                dispatch(setAccountTypeFetchError(e.message));
                dispatch(setBankFetchError(e.message));
            });
        }
    };
}

const AccountsPage = connect(
    mapStateToProps,
    mapDispatchToProps
)(_AccountsPage);
export default AccountsPage;