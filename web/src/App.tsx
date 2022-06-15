import React from 'react';
import styles from '@/App.module.css';
import { Route, BrowserRouter, Routes, Navigate } from 'react-router-dom';
import PrivateComponent from '@/Components/PrivateComponent';
import HomePage from '@/Components/HomePage';
import Navbar, { NavbarItem } from '@/Components/Navbar';
import { createBrowserHistory } from 'history';
import AccountsPage from '@/Components/Accounts/AccountsPage';

const history = createBrowserHistory();

const WrapPrivate = (element: JSX.Element): JSX.Element => (
  <PrivateComponent>{element}</PrivateComponent>
);

const navbarItems: NavbarItem[] = [
  { name: 'Finance', route: '/' },
  { name: 'Accounts', route: '/accounts' }
];

const _App: React.FC = (): JSX.Element => {
  return (
    <div className={styles['app-container']}>
      <BrowserRouter>
        <Navbar items={navbarItems} />
        <div className={styles['app-content']}>
          <Routes>
            <Route path="/" element={WrapPrivate(<HomePage />)} />
            <Route path="/accounts" element={WrapPrivate(<AccountsPage />)} />
            <Route path="*" element={<Navigate to="/" />} />
          </Routes>
        </div>
      </BrowserRouter>
    </div>
  );
};

const App = _App;
export default App;
