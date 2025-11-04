FROM node:18-alpine AS build
WORKDIR /app
COPY src/Web/package.json ./
RUN npm install
COPY src/Web/ ./
RUN npm run build

FROM node:18-alpine AS final
WORKDIR /app
COPY --from=build /app/dist ./dist
RUN npm install -g serve
EXPOSE 5173
CMD ["serve","-s","dist","-l","5173"]
