#!/bin/sh
sed -i 's|window.ENV = {}|window.ENV = { FINANCE_BFF_BASE_URI: "'$FINANCE_BFF_BASE_URI'" }|g' ./index.html
cat ./index.html
nginx -g 'daemon off;'