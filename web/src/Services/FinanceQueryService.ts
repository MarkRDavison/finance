import axios from 'axios';
import { AccountListQueryRequest } from '@/Models/Queries/AccountList/AccountListQueryRequest';
import { AccountListQueryResponse } from '@/Models/Queries/AccountList/AccountListQueryResponse';
import { StartupQueryRequest } from '@/Models/Queries/StartupQuery/StartupQueryRequest';
import { StartupQueryResponse } from '@/Models/Queries/StartupQuery/StartupQueryResponse';

export interface FinanceQueryService {
  accountList: (
    request: AccountListQueryRequest,
  ) => Promise<AccountListQueryResponse>;
  startup: (request: StartupQueryRequest) => Promise<StartupQueryResponse>;
}

const financeQueryService: FinanceQueryService = {
  accountList: async (
    request: AccountListQueryRequest,
  ): Promise<AccountListQueryResponse> => {
    const response = await axios.get(
      `api/account-list-query?showActive=${request.showActive}`,
    );
    return response.data as AccountListQueryResponse;
  },
  startup: async (
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    request: StartupQueryRequest,
  ): Promise<StartupQueryResponse> => {
    const response = await axios.get(`api/startup-query`);
    return response.data as StartupQueryResponse;
  },
};

export default financeQueryService;
