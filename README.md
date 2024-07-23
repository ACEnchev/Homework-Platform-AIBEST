# Homework-Platform-AIBEST

To start the project you need Visula Studio Code or Visual Studio
You need the following extensions:
-.NET Extension Pack
-.NET Install Tool
-C#
-C# Dev Kit
-C# Extensions
-NuGet Gallery
-NuGet Package Manager

This project uses MySql and the dabase is hosted on your computer.
So you have to go to the appsettings.json file and change the password
in the connectionString to yours password

In order to create the database you have to go to the terminal then 
type
//
cd WebApplication14
//
//
dotnet tool install --global dotnet-ef
//
dotnet ef database update
//
after that you should have a database in MySql
After that use the code written in database.txt
This will add the dafault Admins, Teachers and Classes
