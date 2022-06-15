import React from "react";
import { render, screen } from '@testing-library/react';
import PrivateComponent from '@/Components/PrivateComponent';
import {AuthProvider} from '@mark.davison/zeno-common-client';

const authProps = {
    isLoggedIn: false,
    isLoggingIn: false,
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
describe('PrivateComponent', function () {
    it('should not render children when not logged in', function () {
        render(
            <AuthProvider value={authProps}>
                <PrivateComponent>
                    <div data-testid="CHILD" />
                </PrivateComponent>
            </AuthProvider>
        );
        const childComponent = screen.queryByTestId('CHILD');
        expect(childComponent).toBeNull();
    });

    it('should render children when logged in', function () {
        let localAuth = authProps;
        localAuth.isLoggedIn = true;
        render(
            <AuthProvider value={localAuth}>
                <PrivateComponent>
                    <div data-testid="CHILD" />
                </PrivateComponent>
            </AuthProvider>
        );
        const childComponent = screen.queryByTestId('CHILD');
        expect(childComponent).toBeValid();
    });


    it('should call login when not logged in or logging in', function () {
        let localAuth = authProps;
        localAuth.isLoggedIn = false;
        localAuth.isLoggingIn = false;
        localAuth.login = jest.fn();

        render(
            <AuthProvider value={localAuth}>
                <PrivateComponent>
                    <div data-testid="CHILD" />
                </PrivateComponent>
            </AuthProvider>
        );
           
        expect(localAuth.login).toBeCalledTimes(1);
    });

});