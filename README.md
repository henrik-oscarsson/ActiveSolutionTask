# Active Solution Car Rental

This task implements a frontend and backend to provide the business logic for car rental and return.

## Prerequisites
For this to work you need to run MS SQL Server in a Docker container.

### Install a database
1. Install via Docker
```
docker pull mcr.microsoft.com/mssql/server
```
2. Configure and run in Docker:
```
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Qwerty123!" -p 1433:1433 --name sql_server_container -d mcr.microsoft.com/mssql/server
```

## Backend
The backend is written in .NET 9, and is a classic yet modern API with endpoints, business logic and data storage.
As part of the startup procedure the API initializes the database and adds some test data. For ease of testing it also resets data in between sessions.

### Running the backend
Just start it in the IDE of choice. I run it in Jetbrains Rider IDE. You can start the `Aspire AppHost` project, or start the `ApiService` directly.

## Frontend
The frontend is a vanilla React application with Material UI as the UI component framework. React Query is used for state management
as well as api calls.

### Running the frontend
Run the frontend by first building it by issuing the following command in the `Frontend` folder:
```
npm install
```
Then start the frontend app by issuing this command, also in the `Frontend` folder:
```
npm run start
```


