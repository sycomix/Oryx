#!/bin/bash

installNodeVersion() {
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
