FROM node:18-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

FROM node:18-alpine
WORKDIR /app

COPY --from=build /app/.next/standalone ./
COPY --from=build /app/.next/static ./.next/static

COPY --from=build /app/public ./public

COPY --from=build /app/package*.json ./

EXPOSE 3000
CMD ["node", "server.js"]
