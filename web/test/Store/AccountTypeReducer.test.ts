import { SearchItem } from "@/Models/SearchItem";
import { accountTypeReducer, AccountTypeState, setAccountTypeFetchError, setAccountTypesAdded, setAccountTypesFetched, setAccountTypesProgressing, setAccountTypesRemoved } from "@/Store/AccountTypeReducer";

const createAccountType = (id: string = ''): SearchItem => {
    return { id: id, primaryText: 'a accountType name' };
}

describe('accountTypeReducer', () => {
    test('setting an empty list of accountTypes returns the equivelant of the initial state', () => {
        const state = accountTypeReducer(undefined, setAccountTypesFetched([]));

        expect(state.error).toBeUndefined();
        expect(state.progressing).toBeFalsy();
        expect(state.accountTypes).toHaveLength(0);
    });

    test('setting accountTypes progressing does nothing to the existing accountTypes, but sets the progressing flag', () => {
        const initialState: AccountTypeState = {
            error: 'an error',
            progressing: false,
            accountTypes: [
                createAccountType('1')
            ]
        };

        const state = accountTypeReducer(initialState, setAccountTypesProgressing());

        expect(state.error).toBeUndefined();
        expect(state.progressing).toBeTruthy();
        expect(state.accountTypes).toHaveLength(initialState.accountTypes.length);
    });

    test('setting a non empty list of accountTypes sets the list and clears the progressing flag', () => {
        const initialState: AccountTypeState = {
            error: undefined,
            progressing: true,
            accountTypes: []
        };
        const accountTypes: SearchItem[] = [
            createAccountType('1')
        ]
        const state = accountTypeReducer(initialState, setAccountTypesFetched(accountTypes));

        expect(state.error).toBeUndefined();
        expect(state.progressing).toBeFalsy();
        expect(state.accountTypes).toHaveLength(accountTypes.length);
    });

    test('removing a list of accountTypes removes the corresponding accountTypes from state', () => {
        const initialState: AccountTypeState = {
            error: undefined,
            progressing: false,
            accountTypes: [
                createAccountType('1'), createAccountType('2'), createAccountType('3')
            ]
        };

        const state = accountTypeReducer(initialState, setAccountTypesRemoved(['1', '3']));

        expect(state.error).toBeUndefined();
        expect(state.progressing).toBeFalsy();
        expect(state.accountTypes).toHaveLength(1);
    });

    test('setting accountType fetch error sets error and clears the fetching flag', () => {
        const error = 'An error occured';
        const state = accountTypeReducer(undefined, setAccountTypeFetchError(error));

        expect(state.error).toBeDefined();
        expect(state.progressing).toBeFalsy();
    });

    test('adding a list of accountTypes increases the list in state', () => {
        const initialState: AccountTypeState = {
            error: undefined,
            progressing: false,
            accountTypes: [
                createAccountType('1'), createAccountType('2'), createAccountType('3')
            ]
        };

        const state = accountTypeReducer(initialState, setAccountTypesAdded([
            createAccountType('4'),
            createAccountType('5')
        ]));

        expect(state.accountTypes).toHaveLength(5);
    })
});