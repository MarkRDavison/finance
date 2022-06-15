/* istanbul ignore file */

import axios from 'axios';
import React from 'react';
import ReactDom from 'react-dom';
import App from '@/App';
import { AuthContext, AuthEndpoints } from '@mark.davison/zeno-common-client';
import config from '@/Utility/Config';
import styles from '@/Index.module.css';
import { Provider } from 'react-redux';
import createReduxStore from '@/Store/Store';

const authEndpoints: AuthEndpoints = {
  loginEndpoint: config.ZENO_FINANCE_BFF_BASE_URI + '/auth/login',
  logoutEndpoint: config.ZENO_FINANCE_BFF_BASE_URI + '/auth/logout',
  userEndpoint: config.ZENO_FINANCE_BFF_BASE_URI + '/auth/user',
};

axios.defaults.baseURL = config.ZENO_FINANCE_BFF_BASE_URI;
axios.defaults.withCredentials = true;

const store = createReduxStore();

ReactDom.render(
  <div className={styles.main}>
    <Provider store={store}>
      <AuthContext {...authEndpoints}>
        <App />
      </AuthContext>
    </Provider>
  </div>,
  document.getElementById('root') as HTMLElement,
);
