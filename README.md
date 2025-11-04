Perf Demo - Docker Compose (Windows w/ Docker Desktop + WSL2)

How to run:
1. Ensure Docker Desktop is running and using Linux containers (WSL2 backend).
2. Open a terminal in this folder.
3. Run:
   docker compose up --build

Services:
- SQL Server 2022 (container) : 1433
- Redis : 6379
- API (.NET) : http://localhost:5000
- React Web (Vite) : http://localhost:5173
- Prometheus : http://localhost:9090
- Grafana : http://localhost:3000  (admin/admin)

The `seeder` service will run once during startup and insert rows into the PerfDemo database (default 50,000 rows).
k6 will run the ramp test automatically — remove the k6 service if you prefer to run manually.

IMPORTANT:
- SQL Server needs SA password length/complexity; the compose file uses 'Your_strong@Passw0rd1'. Change if needed.
- For large seeding ensure Docker has enough memory (increase in Docker Desktop settings).
