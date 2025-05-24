@echo off
echo Setup WeeSe Configuratore Vetrate...

rem Crea le cartelle necessarie
mkdir Models\ViewModels
mkdir Views\Account
mkdir Views\Dashboard
mkdir Views\GestioneUtenze
mkdir Views\GestioneListini
mkdir Views\Preventivi
mkdir Views\Ordini
mkdir Views\Commesse

echo Cartelle create!

rem Installa pacchetti necessari
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore

echo Pacchetti installati!

rem Compila il progetto
dotnet build

echo Setup completato!
echo Prossimi passi:
echo 1. Copia tutti i file forniti nelle rispettive cartelle
echo 2. Esegui: dotnet ef migrations add InitialCreate
echo 3. Esegui: dotnet ef database update
echo 4. Esegui: dotnet run

pause