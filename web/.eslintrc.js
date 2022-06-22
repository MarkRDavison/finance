module.exports = {
    parser: '@typescript-eslint/parser',
    extends: [
        'plugin:react/recommended',
        'plugin:@typescript-eslint/recommended',
        'plugin:prettier/recommended',
    ],
    parserOptions: {
        ecmaVersion: 2018,
        sourceType: 'module',
    },
    plugins: ["prettier"],
    rules: {
        "prettier/prettier": [
            "error",
            {
                singleQuote: true,
                trailingComma: 'all',
            }
        ],
        'react/display-name': 'off',
        '@typescript-eslint/no-non-null-assertion': 'off',
        'react/prop-types': [
            1,
            {
                ignore: ['context', 'tracking'],
            },
        ],
        "no-restricted-imports": ["error", {
            "patterns": [".*"]
        }],
    },
    settings: {
        "react": {
            "version": "detect",
        },
    },
    overrides: [
        {
            files: ['*.ts', '*.tsx'],
            rules: {
                'react/prop-types': 'off',
            },
        },
    ],
};