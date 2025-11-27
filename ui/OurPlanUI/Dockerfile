# Dockerfile pentru frontend Vue.js/Vite
FROM node:20-alpine AS build

WORKDIR /app

# Copiem fișierele de dependențe
COPY package*.json ./

# Instalăm dependențele
RUN npm ci

# Copiem restul codului sursă
COPY . .

# Build aplicația
RUN npm run build

# Stage pentru producție - folosim nginx pentru a servi fișierele statice
FROM nginx:alpine AS production

# Copiem fișierele build-uite în nginx
COPY --from=build /app/dist /usr/share/nginx/html

# Copiem configurația nginx (opțional - pentru SPA routing)
RUN echo 'server { \
    listen 80; \
    server_name localhost; \
    root /usr/share/nginx/html; \
    index index.html; \
    location / { \
        try_files $uri $uri/ /index.html; \
    } \
}' > /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]

