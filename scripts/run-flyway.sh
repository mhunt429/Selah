#!/bin/bash

flyway -url=jdbc:postgresql://localhost:55432/postgres -user=postgres -password=postgres -locations=filesystem:database/Migrations migrate

flyway -url=jdbc:postgresql://localhost:65432/postgres -user=postgres -password=postgres -locations=filesystem:database/Migrations migrate