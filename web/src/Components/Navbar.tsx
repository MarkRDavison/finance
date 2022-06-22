import React, { useState } from 'react';
import styles from '@/Components/Navbar.module.css';
import icons from '@/Utility/Icons';
import { NavLink } from 'react-router-dom';

export interface NavbarItem {
  name: string;
  route: string;
}

interface NavbarProps {
  items: NavbarItem[];
}

const _Navbar: React.FC<NavbarProps> = (props: NavbarProps): JSX.Element => {
  const [navbarOpen, setNavbarOpen] = useState<boolean>(false);

  const closeNavbar = (): void => setNavbarOpen(false);

  return (
    <div
      data-testid="Navbar"
      className={`${styles.navbar} ${navbarOpen && styles.responsive}`}
    >
      {props.items.map((_) => (
        <NavLink key={_.name} onClick={closeNavbar} to={_.route}>
          {_.name}
        </NavLink>
      ))}
      <a className={styles['icon']} onClick={() => setNavbarOpen(!navbarOpen)}>
        <i className={icons.bars}></i>
      </a>
    </div>
  );
};

const Navbar = _Navbar;
export default Navbar;

/*
https://www.w3schools.com/howto/tryit.asp?filename=tryhow_css_sidebar_responsive
https://www.w3schools.com/howto/howto_css_fixed_sidebar.asp
*/
