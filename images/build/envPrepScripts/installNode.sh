#!/bin/bash

versionToBeInstalled="$1"
curl -sL https://git.io/n-install | bash -s -- -ny -
~/n/bin/n -d $versionToBeInstalled
mv /usr/local/n/versions/node /opt/nodejs
rm -rf /usr/local/n ~/n