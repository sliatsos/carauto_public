#!/bin/bash
set -e

file="/docker-entrypoint-initdb.d/demo.dump"
dbname=IDigital365 

echo "Restoring DB using $file"

pg_restore -U postgres -C --dbname=$dbname --verbose  < "$file" || exit $?

echo "Restoring DB $dbname complete!"