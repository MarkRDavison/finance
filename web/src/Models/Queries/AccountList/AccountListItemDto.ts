export interface AccountListItemDto {
  id: string;
  name: string;
  accountType: string;
  accountNumber: string;
  currentBalance: number;
  balanceDifference: number;
  active: boolean;
  lastModified: Date;
}
