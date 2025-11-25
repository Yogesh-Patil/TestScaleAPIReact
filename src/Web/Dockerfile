FROM node:18-alpine
WORKDIR /app
COPY src/Web/package.json ./
RUN npm install
COPY src/Web/ ./
EXPOSE 5173
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]
