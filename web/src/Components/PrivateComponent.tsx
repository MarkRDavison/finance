import React from 'react';
import { useAuth } from '@mark.davison/zeno-common-client';

interface PrivateComponentProps {
  children?: React.ReactNode;
}

const PrivateComponent: React.FC<PrivateComponentProps> = (
  props: PrivateComponentProps,
): JSX.Element => {
  const { isLoggedIn, isLoggingIn, login } = useAuth();

  if (isLoggedIn) {
    return <React.Fragment>{props.children}</React.Fragment>;
  }

  if (!isLoggingIn) {
    login();
  }

  return <React.Fragment />;
};

export default PrivateComponent;
