#!/bin/bash
echo "Creating keycloak user"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL

    --  DROP ROLE IF EXISTS keycloak;

   CREATE ROLE keycloak LOGIN PASSWORD 'password';

   ALTER ROLE keycloak WITH LOGIN;
   ALTER ROLE keycloak WITH SUPERUSER;
   ALTER ROLE keycloak with CREATEDB;
   ALTER ROLE keycloak WITH INHERIT;
   ALTER ROLE keycloak WITH CREATEROLE;
   ALTER ROLE keycloak WITH REPLICATION;

    CREATE DATABASE keycloak;
    GRANT ALL PRIVILEGES ON DATABASE keycloak TO keycloak;
EOSQL