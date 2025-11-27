# Docker Setup pentru OurPlan

Acest proiect include configurații Docker pentru a rula aplicația într-un mediu containerizat.

## Structură

- `Dockerfile` - Backend .NET 9.0
- `ui/OurPlanUI/Dockerfile` - Frontend Vue.js (Production)
- `ui/OurPlanUI/Dockerfile.dev` - Frontend Vue.js (Development)
- `docker-compose.yml` - Configurație pentru producție
- `docker-compose.dev.yml` - Configurație pentru development

## Utilizare

### Producție

Pentru a rula aplicația în modul producție:

```bash
# Construiește și pornește toate serviciile
docker-compose up -d

# Sau cu build explicit
docker-compose up -d --build
```

Serviciile vor fi disponibile la:
- Frontend: http://localhost
- Backend API: http://localhost:5230
- Swagger: http://localhost:5230/swagger
- PostgreSQL: localhost:5432

### Development

Pentru development cu hot-reload:

```bash
# Folosește docker-compose.dev.yml
docker-compose -f docker-compose.dev.yml up -d
```

Serviciile vor fi disponibile la:
- Frontend: http://localhost:5173
- Backend API: http://localhost:5230

### Variabile de mediu

Creează un fișier `.env` în rădăcina proiectului:

```env
DB_CONNECTION_STRING=User Id=postgres;Password=postgres;Server=db;Port=5432;Database=ourplan
JWT_KEY=your-secret-key-here
JWT_ISSUER=OurPlanApi
JWT_AUDIENCE=OurPlanApiUsers
JWT_EXPIRE_MINUTES=100000
```

### Comenzi utile

```bash
# Oprește toate containerele
docker-compose down

# Oprește și șterge volume-urile (atenție: șterge datele din DB!)
docker-compose down -v

# Vezi logurile
docker-compose logs -f

# Vezi logurile pentru un serviciu specific
docker-compose logs -f backend
docker-compose logs -f frontend

# Reconstruiește un serviciu specific
docker-compose build backend
docker-compose up -d backend

# Intră în container
docker exec -it ourplan-backend bash
docker exec -it ourplan-frontend sh
```

## Migrații Database

Migrațiile se aplică automat la pornirea backend-ului (vezi `Program.cs`).

Dacă vrei să rulezi migrații manual:

```bash
# Intră în container-ul backend
docker exec -it ourplan-backend bash

# Rulează migrațiile
dotnet ef database update
```

## Note

- Backend-ul rulează pe portul 80 în container, dar este mapat la 5230 pe host
- Frontend-ul în producție folosește nginx pentru a servi fișierele statice
- PostgreSQL datele sunt persistate într-un Docker volume (`postgres_data`)
- Pentru producție reală, folosește variabile de mediu sigure și nu hardcodeaza credențialele

