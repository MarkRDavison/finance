#!/bin/sh
echo 'entry.sh - starting - 1'
cat ./index.html
echo 'entry.sh - starting - 2'
echo 'entry.sh - starting - 3'
echo 'entry.sh - starting - 4'
sed -i 's|window.ENV = {}|window.ENV = { FINANCE_BFF_BASE_URI: "'$FINANCE_BFF_BASE_URI'" }|g' ./index.html
cat ./index.html
nginx -g 'daemon off;'