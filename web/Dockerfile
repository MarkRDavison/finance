FROM node:alpine AS BUILD_IMAGE

WORKDIR /usr/src/app

ENV PATH /usr/src/app/node_modules/.bin:$PATH

COPY . .
RUN npm ci
RUN npm run build
RUN npm run test
RUN npm run lint

FROM nginx:alpine

RUN rm -rf /usr/share/nginx/html/*

COPY --from=BUILD_IMAGE /usr/src/app/build/* /usr/share/nginx/html/
COPY --from=BUILD_IMAGE /usr/src/app/entry.sh /usr/share/nginx/html/
COPY --from=BUILD_IMAGE /usr/src/app/nginx.conf /etc/nginx/nginx.conf

RUN chmod +x /usr/share/nginx/html/entry.sh

WORKDIR /usr/share/nginx/html

CMD ["./entry.sh"]