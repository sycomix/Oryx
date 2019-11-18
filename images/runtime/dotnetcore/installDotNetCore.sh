#!/bin/bash
# --------------------------------------------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license.
# --------------------------------------------------------------------------------------------

set -ex

installDotNetCoreRuntime() {
    local VERSION="$1"
    local SHA="$2"
    
    curl -SL --output dotnet.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/Runtime/$VERSION/dotnet-runtime-$VERSION-linux-x64.tar.gz
    echo "$SHA dotnet.tar.gz" | sha512sum -c -
    mkdir -p /usr/share/dotnet
    tar -zxf dotnet.tar.gz -C /usr/share/dotnet
    rm dotnet.tar.gz
    ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
}

installAspNetCoreRuntime() {
    local VERSION="$1"
    local SHA="$2"
    
    curl -SL --output aspnetcore.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/aspnetcore/Runtime/$VERSION/aspnetcore-runtime-$VERSION-linux-x64.tar.gz
    echo "$SHA aspnetcore.tar.gz" | sha512sum -c -
    mkdir -p /usr/share/dotnet
    tar -zxf aspnetcore.tar.gz -C /usr/share/dotnet ./shared/Microsoft.AspNetCore.App
    rm aspnetcore.tar.gz
}