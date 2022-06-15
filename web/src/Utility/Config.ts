/* istanbul ignore file */
/* istanbul ignore next */
export interface Config {
  // If you update this you need to also update the entry.sh script
  ZENO_FINANCE_BFF_BASE_URI: string;
}

/* istanbul ignore next */
declare global {
  interface Window {
    ENV: {
      ZENO_FINANCE_BFF_BASE_URI: string;
    };
  }
}

/* istanbul ignore next */
const createConfig = (): Config => {
  return {
    ZENO_FINANCE_BFF_BASE_URI:
      window?.ENV?.ZENO_FINANCE_BFF_BASE_URI ?? 'https://localhost:40000',
  };
};

/* istanbul ignore next */
const config = createConfig();

/* istanbul ignore next */
export default config;
