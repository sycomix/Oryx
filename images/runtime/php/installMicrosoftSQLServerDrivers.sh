#!/bin/bash
# --------------------------------------------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license.
# --------------------------------------------------------------------------------------------

set -ex

# Install the Microsoft SQL Server PDO driver on supported versions only.
#  - https://docs.microsoft.com/en-us/sql/connect/php/installation-tutorial-linux-mac
#  - https://docs.microsoft.com/en-us/sql/connect/odbc/linux-mac/installing-the-microsoft-odbc-driver-for-sql-server
if [[ $PHP_VERSION == 7.1.* || $PHP_VERSION == 7.2.* || $PHP_VERSION == 7.3.* ]]; then
    apt-get update && apt-get install -y gnupg2 apt-transport-https
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add -

    if [[ $PHP_VERSION == 7.1.* ]]; then
        curl https://packages.microsoft.com/config/debian/9/prod.list > /etc/apt/sources.list.d/mssql-release.list
    else
        curl https://packages.microsoft.com/config/debian/10/prod.list > /etc/apt/sources.list.d/mssql-release.list
    fi
    
    apt-get update && ACCEPT_EULA=Y apt-get install -y msodbcsql17
    
    pecl install sqlsrv pdo_sqlsrv
    echo extension=pdo_sqlsrv.so >> `php --ini | grep "Scan for additional .ini files" | sed -e "s|.*:\s*||"`/30-pdo_sqlsrv.ini
    echo extension=sqlsrv.so >> `php --ini | grep "Scan for additional .ini files" | sed -e "s|.*:\s*||"`/20-sqlsrv.ini
fi