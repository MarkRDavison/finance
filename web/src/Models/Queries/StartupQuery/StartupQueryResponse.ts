import { AccountTypeDto } from '@/Models/Dtos/AccountTypeDto';
import { BankDto } from '@/Models/Dtos/BankDto';
import { CurrencyDto } from '@/Models/Dtos/CurrencyDto';

export interface StartupQueryResponse {
  accountTypes: AccountTypeDto[];
  banks: BankDto[];
  currencies: CurrencyDto[];
}
