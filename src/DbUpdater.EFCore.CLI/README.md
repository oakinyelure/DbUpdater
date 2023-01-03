## Introduction

The DbUpdater.EFCore.CLI is a command line tool that can be used to persist database migrations to the database. The project stems from the idea to include both code-first entities and SQL-based scripts in the same code repository and CI/CD process. while code-first migrations can be persisted by calling the `Update-Database` command, this is not feasible when the code have been published. We never want to point to production using visual studio in an enterprise environment. DbUpdater.EFCore.CLI enables all your code to live within the same code base and also allows all deployments (code and data) to be implemented through the same CI / CD pipeline. The DbUpdater.EFCore.CLI make provisions for migrations, script execution and custom data seeding instead of using the `builder.HasData` method from the Migration builder.

## Installation

## Installing DbUpdater.EFCore.CLI via Package Manager Console

`Install-Package DbUpdater.EFCore.CLI`

## Install via dotnet cli

`dotnet add package DbUpdater.EFCore.CLI`

## Usage

See https://github.com/oakinyelure/DbUpdater/wiki/DbUpdater.EFCore.CLI
