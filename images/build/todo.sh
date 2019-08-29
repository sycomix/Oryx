#!/bin/bash

installNode() {
    local versionToBeInstalled="$1"
    if [ ! -d "/opt/nodejs/$versionToBeInstalled" ]; then
        echo "Node version '$versionToBeInstalled' not found. Installing it..."
        curl -sL https://git.io/n-install | bash -s -- -ny -
        ~/n/bin/n -d $versionToBeInstalled
        mv /usr/local/n/versions/node /opt/nodejs
        rm -rf /usr/local/n ~/n
    else
        echo "Node version '$versionToBeInstalled' found. Skipped installation."
    fi
}

installPhp() {
    local versionToBeInstalled="$1"
    if [ ! -d "/opt/php/$versionToBeInstalled" ]; then
    else
        echo "Php version '$versionToBeInstalled' found. Skipped installation."
    fi
}

installYarn() {
    local versionToBeInstalled="$1"
    if [ ! -d "/opt/yarn/$versionToBeInstalled" ]; then
    else
        echo "Yarn version '$versionToBeInstalled' found. Skipped installation."
    fi
}

installDotNet() {
    local versionToBeInstalled="$1"
    if [ ! -d "/opt/dotnet/$versionToBeInstalled" ]; then
        if [ "$versionToBeInstalled" == "1."* ]; then
        elif 
    else
        echo "DotNet version '$versionToBeInstalled' found. Skipped installation."
    fi
}