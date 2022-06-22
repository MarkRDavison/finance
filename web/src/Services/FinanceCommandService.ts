import axios from 'axios';
import { CreateAccountRequest } from "@/Models/Commands/CreateAccount/CreateAccountRequest";
import { CreateAccountResponse } from "@/Models/Commands/CreateAccount/CreateAccountResponse";

export interface FinanceCommandService {
    createAccount: (request: CreateAccountRequest) => Promise<CreateAccountResponse>
}

const financeCommandService: FinanceCommandService = {
    createAccount: async (request: CreateAccountRequest): Promise<CreateAccountResponse> => {
        const response = await axios.post('api/create-account', request);
        return response.data as CreateAccountResponse;
    }
};

export default financeCommandService;