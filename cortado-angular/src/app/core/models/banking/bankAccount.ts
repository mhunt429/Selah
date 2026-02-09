export type BankAccount = {
  id: number;
  currentBalance: number;
  accountMask: string;
  displayName: string;
  officialName: string;
  subtype: AccountSubtype;
  lastApiSync: Date;
};

export enum AccountSubtype {
  Checking = 'Checking',
  Savings = 'Savings',
}
