import React from 'react';
import * as zui from '@mark.davison/zeno-ui';
import { CreateAccountRequest } from '@/Models/Commands/CreateAccount/CreateAccountRequest';
import { CurrencyDto } from '@/Models/Dtos/CurrencyDto';
import financeCommandService from '@/Services/FinanceCommandService';

interface OwnProps {
  currency: CurrencyDto;
  close: () => void;
  banks: zui.DropdownItem[];
  accountTypes: zui.DropdownItem[];
  currencies: zui.DropdownItem[];
}

type Props = OwnProps;

const Validator: zui.Validate<CreateAccountRequest> = (
  values,
  touched,
  required,
): zui.ValidationResult<CreateAccountRequest> => {
  const errors: zui.Errors<CreateAccountRequest> = {
    name: undefined,
    accountNumber: undefined,
    virtualBalance: undefined,
    bankId: undefined,
    accountTypeId: undefined,
    currencyId: undefined,
  };

  return {
    errors: errors,
    required,
  };
};

const FormContentMethod =
  (
    currency: CurrencyDto,
    banks: zui.DropdownItem[],
    accountTypes: zui.DropdownItem[],
    currencies: zui.DropdownItem[],
    close: () => void,
  ) =>
    ({ }: zui.FormProviderProps<CreateAccountRequest>) => {
      return (
        <zui.BaseForm>
          <zui.BaseTextInput name="name" label="Name" maxLength={255} />
          <zui.BaseErrorMessage name="accountNumber" />
          <zui.BaseTextInput
            name="accountNumber"
            label="Account Number"
            maxLength={255}
          />
          <zui.BaseErrorMessage name="name" />
          <zui.BaseDropdownListInput
            name="bankId"
            placeholder="Select bank"
            label="Bank"
            items={banks}
          />
          <zui.BaseErrorMessage name="bankId" />
          <zui.BaseDropdownListInput
            name="accountTypeId"
            placeholder="Select account type"
            label="Account Type"
            items={accountTypes}
          />
          <zui.BaseErrorMessage name="accountTypeId" />
          <zui.BaseDropdownListInput
            name="currencyId"
            placeholder="Select currency"
            label="Currency"
            items={currencies}
          />
          <zui.BaseErrorMessage name="currencyId" />
          <zui.BaseCurrencyInput
            name="virtualBalance"
            symbol={currency?.symbol ?? '$'}
            decimalPlaces={currency?.decimalPlaces}
            label="Virtual Balance"
          />
          <zui.BaseErrorMessage name="virtualBalance" />
          <div
            style={{
              display: 'flex',
              flexDirection: 'row',
              justifyContent: 'space-between',
            }}
          >
            <zui.BaseButton
              onClick={close}
              type="button"
              variant="contained"
              colour="danger"
            >
              Cancel
            </zui.BaseButton>
            <zui.BaseButton type="submit" variant="contained" colour="success" />
          </div>
        </zui.BaseForm>
      );
    };

const _CreateAccount: React.FC<Props> = ({
  accountTypes,
  banks,
  currency,
  currencies,
  close,
}: Props) => {
  const initialValues: CreateAccountRequest = {
    name: '',
    accountNumber: '',
    bankId: '',
    accountTypeId: '',
    currencyId: '',
    virtualBalance: undefined,
  };
  const required: zui.Required<CreateAccountRequest> = {
    name: true,
    accountNumber: false,
    virtualBalance: false,
    bankId: true,
    accountTypeId: true,
    currencyId: true,
  };

  const handleSubmit = async (
    values: CreateAccountRequest,
  ): Promise<boolean> => {
    await new Promise((resolve) => setTimeout(resolve, 2000));
    const response = await financeCommandService.createAccount(values);

    console.log(JSON.stringify(response, null, 2));
    if (response.success) {

    }
    return response.success;
  };

  return (
    <zui.Form<CreateAccountRequest>
      initialValues={initialValues}
      required={required}
      handleSubmit={handleSubmit}
      validator={Validator}
      childData={FormContentMethod(
        currency,
        banks,
        accountTypes,
        currencies,
        close,
      )}
    />
  );
};

const CreateAccount = _CreateAccount;
export default CreateAccount;
