#!/bin/sh
sed -i 's|window.ENV = {}|window.ENV = { MARKDAVISON_KIWI_BFF_BASE_URI: "'$MARKDAVISON_KIWI_BFF_BASE_URI'" }|g' ./index.html
cat ./index.html
nginx -g 'daemon off;'