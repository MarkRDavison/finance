import React from 'react';
import axios from 'axios';
import * as zcc from '@mark.davison/zeno-common-client';

const healthready = (): void => {
  axios.get('/health/ready').then((_) => alert(_.data));
};

const createAccountTest = (): void => {
  axios
    .post('/api/create-account', {
      name: 'from react',
    })
    .then((_) => alert(JSON.stringify(_.data)));
};

const _HomePage: React.FC = () => {
  const { logout, isLoggedIn } = zcc.useAuth();
  return (
    <div data-testid="HomePage">
      <button data-testid="LOGOUT" disabled={!isLoggedIn} onClick={logout}>
        LOGOUT
      </button>
      <button onClick={healthready}>/health/ready</button>
      <button onClick={createAccountTest}>Create Account Test</button>
    </div>
  );
};

const HomePage = _HomePage;
export default HomePage;
