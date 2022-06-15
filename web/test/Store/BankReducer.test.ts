import { SearchItem } from "@/Models/SearchItem";
import { bankReducer, BankState, setBankFetchError, setBanksAdded, setBanksFetched, setBanksProgressing, setBanksRemoved } from "@/Store/BankReducer";

const createBank = (id: string = ''): SearchItem => {
    return { id: id, primaryText: 'a bank name' };
}

describe('bankReducer', () => {
    test('setting an empty list of banks returns the equivelant of the initial state', () => {
        const state = bankReducer(undefined, setBanksFetched([]));

        expect(state.error).toBeUndefined();
        expect(state.progressing).toBeFalsy();
        expect(state.banks).toHaveLength(0);
    });

    test('setting banks progressing does nothing to the existing banks, but sets the progressing flag', () => {
        const initialState: BankState = {
            error: 'an error',
            progressing: false,
            banks: [
                createBank('1')
            ]
        };

        const state = bankReducer(initialState, setBanksProgressing());

        expect(state.error).toBeUndefined();
        expect(state.progressing).toBeTruthy();
        expect(state.banks).toHaveLength(initialState.banks.length);
    });

    test('setting a non empty list of banks sets the list and clears the progressing flag', () => {
        const initialState: BankState = {
            error: undefined,
            progressing: true,
            banks: []
        };
        const banks: SearchItem[] = [
            createBank('1')
        ]
        const state = bankReducer(initialState, setBanksFetched(banks));

        expect(state.error).toBeUndefined();
        expect(state.progressing).toBeFalsy();
        expect(state.banks).toHaveLength(banks.length);
    });

    test('removing a list of banks removes the corresponding banks from state', () => {
        const initialState: BankState = {
            error: undefined,
            progressing: false,
            banks: [
                createBank('1'), createBank('2'), createBank('3')
            ]
        };

        const state = bankReducer(initialState, setBanksRemoved(['1', '3']));

        expect(state.error).toBeUndefined();
        expect(state.progressing).toBeFalsy();
        expect(state.banks).toHaveLength(1);
    });

    test('setting bank fetch error sets error and clears the fetching flag', () => {
        const error = 'An error occured';
        const state = bankReducer(undefined, setBankFetchError(error));

        expect(state.error).toBeDefined();
        expect(state.progressing).toBeFalsy();
    });

    test('adding a list of banks increases the list in state', () => {
        const initialState: BankState = {
            error: undefined,
            progressing: false,
            banks: [
                createBank('1'), createBank('2'), createBank('3')
            ]
        };

        const state = bankReducer(initialState, setBanksAdded([
            createBank('4'),
            createBank('5')
        ]));

        expect(state.banks).toHaveLength(5);
    })
});