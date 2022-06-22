export interface CreateAccountRequest {
  name: string;
  accountNumber: string;
  virtualBalance?: number;
  bankId: string;
  accountTypeId: string;
  currencyId: string;
}
