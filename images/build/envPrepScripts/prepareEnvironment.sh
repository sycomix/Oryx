#!/bin/bash

envPrepScriptsDir="/opt/oryx/envPrepScripts"

installNode() {
    local versionToBeInstalled="$1"
    if [ ! -d "/opt/nodejs/$versionToBeInstalled" ]; then
        echo "Node version '$versionToBeInstalled' not found. Installing it..."
        installationScript="$envPrepScriptsDir/installNode.sh"
        "$installationScript" $versionToBeInstalled
    else
        echo "Node version '$versionToBeInstalled' found. Skipped installation."
    fi
}

installPhp() {
    local versionToBeInstalled="$1"
    if [ ! -d "/opt/php/$versionToBeInstalled" ]; then
        echo "Php version '$versionToBeInstalled' not found. Installing it..."
    else
        echo "Php version '$versionToBeInstalled' found. Skipped installation."
    fi
}

installYarn() {
    local versionToBeInstalled="$1"
    if [ ! -d "/opt/yarn/$versionToBeInstalled" ]; then
        echo "Yarn version '$versionToBeInstalled' not found. Installing it..."
    else
        echo "Yarn version '$versionToBeInstalled' found. Skipped installation."
    fi
}

installDotNet() {
    local versionToBeInstalled="$1"
    if [ ! -d "/opt/dotnet/sdks/$versionToBeInstalled" ]; then
        echo "DotNet version '$versionToBeInstalled' not found. Installing it..."
        installationScript="$envPrepScriptsDir/installDotNetCore.sh"
        "$installationScript" $versionToBeInstalled
    else
        echo "DotNet version '$versionToBeInstalled' found. Skipped installation."
    fi
}

# Perform case-insensitive comparison
matchesName() {
  local expectedName="$1"
  local providedName="$2"
  local result=
  shopt -s nocasematch
  [[ "$expectedName" == "$providedName" ]] && result=0 || result=1
  shopt -u nocasematch
   return $result
}

while [[ $1 = *"="* ]]; do
    name=$(echo $1 | sed 's/=.*$//')
    value=$(echo $1 | sed 's/^.*=//')
    
    if matchesName "dotnet" "$name"; then
        installDotNet $value
    elif matchesName "node" "$name"; then
        installNode $value
    elif matchesName "python" "$name"; then
        installPython $value
    elif matchesName "php" "$name"; then
        installPhp $value
    fi
    shift
done