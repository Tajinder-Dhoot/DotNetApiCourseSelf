# About Project

This is a WEB API develiopment project implemented using Dapper instead of Entity Framework Database Context. Some concepts of DB Context are also covered though.

## Setup Project
To Run this project, follow the steps:
- Fork the repo.
- Clone the repo on your local.
- Change working directory to 'DotNetAPI'.
- Add packages if not installed.
- Run Command `dotnet build` to check any errors/ warnings from directory.
- Run Command `dotnet run` or `dotnet watch run`.

## Packages Required
Add the packages to project by running following commands in root folder.<br>
<br>
`dotnet add package Microsoft.AspNetCore.OpenApi`<br>
<br>
`dotnet add package Microsoft.data.sqlclient` <br>
<br>
`dotnet add package Microsoft.EntityFrameworkCore.SqlServer` <br>
<br>
`dotnet add package Dapper`<br>
<br>
`dotnet add package Microsoft.extensions.configuration` <br>
<br>
`dotnet add package Swashbuckle.AspNetCore` <br>

## Authentication

Add following package for token authentication.<br>
<br>
`dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer`

## Get Docker Image for SQL Server
`docker pull mcr.microsoft.com/mssql/server`

## Create new project in VS Code
If you want to create new project in VS Code, run the following command and add the packages. <br>
`dotnet new console --framework net8.0 --use-program-main`
