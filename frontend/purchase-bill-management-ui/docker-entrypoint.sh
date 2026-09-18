#!/bin/sh
set -e

: "${API_URL:=http://api:8080}"
envsubst '${API_URL}' \
    < /etc/nginx/templates/default.conf.template \
    > /etc/nginx/conf.d/default.conf

exec "$@"