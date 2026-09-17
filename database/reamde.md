docker run -d --name purchase-bill-sqlserver -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=manoj123*" -p 1433:1433 -v purchase_bill_sql_data:/var/opt/mssql mcr.microsoft.com/mssql/server:2022-latest

docker exec -it purchase-bill-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "manoj123*" -C