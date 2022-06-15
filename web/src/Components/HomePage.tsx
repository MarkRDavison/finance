import React from 'react';
import axios from 'axios';
import * as zcc from '@mark.davison/zeno-common-client';

interface HomePageProps { }

const healthready = (): void => {
  axios.get('/health/ready').then(_ => alert(_.data))

}

const createAccountQueryTest = (): void => {
  axios.get('/api/create-account-query?name1=name1value&name2=name2value').then(_ => alert(JSON.stringify(_.data, null, 2)))
}
const createAccountTest = (): void => {
  axios.post('/api/create-account', {
    name: 'from react'
  }).then(_ => alert(JSON.stringify(_.data)))
}

const _HomePage: React.FC<HomePageProps> = (
  props: HomePageProps,
): JSX.Element => {
  const { logout, isLoggedIn } = zcc.useAuth();
  return (
    <div data-testid="HomePage">
      <button data-testid="LOGOUT" disabled={!isLoggedIn} onClick={logout}>
        LOGOUT
      </button>
      <button onClick={healthready}>
        /health/ready
      </button>
      <button onClick={createAccountTest}>
        Create Account Test
      </button>
      <button onClick={createAccountQueryTest}>
        Create Account Query Test
      </button>
    </div>
  );
};

const HomePage = _HomePage;
export default HomePage;
