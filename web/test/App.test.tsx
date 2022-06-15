import React from "react";
import { render, screen } from '@testing-library/react';
import App from '@/App';
import {AuthProvider} from '@mark.davison/zeno-common-client';
import createTodoStore from "@/Store/Store";
import { Provider } from "react-redux";

const authProps = {
    isLoggedIn: true,
    isLoggingIn: true,
    user: {
        preferred_username: 'first.last',
        email: 'first.last@something.com',
        email_verified: true,
        family_name: 'last',
        given_name: 'first',
        name: 'first last',
        sub: '5B9BCCDC-CC4F-30F0-DC20-1AD58355D35F'
    },
    login: () => { },
    logout: () => { }
};
describe('App', function () {
    const store = createTodoStore();
    it('can be rendered', function () {
        render(
            <Provider store={store}>
                <AuthProvider value={authProps}>
                    <App />
                </AuthProvider>
            </Provider>
        );
    });
});