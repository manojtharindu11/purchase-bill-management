FROM node:24-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build -- --configuration production

FROM nginx:1.27-alpine AS final
RUN apk add --no-cache gettext
COPY nginx.conf /etc/nginx/templates/default.conf.template
COPY docker-entrypoint.sh /usr/local/bin/purchase-bill-entrypoint.sh
RUN chmod +x /usr/local/bin/purchase-bill-entrypoint.sh
COPY --from=build /app/dist/purchase-bill-management-ui/browser /usr/share/nginx/html

EXPOSE 80
ENTRYPOINT ["/usr/local/bin/purchase-bill-entrypoint.sh"]
CMD ["nginx", "-g", "daemon off;"]
