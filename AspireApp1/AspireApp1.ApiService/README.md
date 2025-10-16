# How to configure and run

## Database
1. Install via Docker
```
docker pull mcr.microsoft.com/mssql/server
```
2. Configure and run in Docker:
```
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Qwerty123!" -p 1433:1433 --name sql_server_container -d mcr.microsoft.com/mssql/server
```