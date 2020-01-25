#!/bin/bash

# ----------------------
# KUDU Deployment Script
# Version: 0.2.2
# ----------------------

# Helpers
# -------

exitWithMessageOnError () {
  if [ ! $? -eq 0 ]; then
    echo "An error has occurred during web site deployment."
    echo $1
    exit 1
  fi
}

# Prerequisites
# -------------

# Verify node.js installed
hash node 2>/dev/null
exitWithMessageOnError "Missing node.js executable, please install node.js, if already installed make sure it can be reached from current environment."

# Setup
# -----

SCRIPT_DIR="${BASH_SOURCE[0]%\\*}"
SCRIPT_DIR="${SCRIPT_DIR%/*}"
ARTIFACTS=$SCRIPT_DIR/../artifacts
KUDU_SYNC_CMD=${KUDU_SYNC_CMD//\"}

if [[ ! -n "$DEPLOYMENT_SOURCE" ]]; then
  DEPLOYMENT_SOURCE=$SCRIPT_DIR
fi

if [[ ! -n "$NEXT_MANIFEST_PATH" ]]; then
  NEXT_MANIFEST_PATH=$ARTIFACTS/manifest

  if [[ ! -n "$PREVIOUS_MANIFEST_PATH" ]]; then
    PREVIOUS_MANIFEST_PATH=$NEXT_MANIFEST_PATH
  fi
fi

if [[ ! -n "$DEPLOYMENT_TARGET" ]]; then
  DEPLOYMENT_TARGET=$ARTIFACTS/wwwroot
else
  KUDU_SERVICE=true
fi

if [[ ! -n "$KUDU_SYNC_CMD" ]]; then
  # Install kudu sync
  echo Installing Kudu Sync
  npm install kudusync -g --silent
  exitWithMessageOnError "npm failed"

  if [[ ! -n "$KUDU_SERVICE" ]]; then
    # In case we are running locally this is the correct location of kuduSync
    KUDU_SYNC_CMD=kuduSync
  else
    # In case we are running on kudu service this is the correct location of kuduSync
    KUDU_SYNC_CMD=$APPDATA/npm/node_modules/kuduSync/bin/kuduSync
  fi
fi

# Node Helpers
# ------------

selectNodeVersion () {
  if [[ -n "$KUDU_SELECT_NODE_VERSION_CMD" ]]; then
    SELECT_NODE_VERSION="$KUDU_SELECT_NODE_VERSION_CMD \"$DEPLOYMENT_SOURCE\" \"$DEPLOYMENT_TARGET\" \"$DEPLOYMENT_TEMP\""
    eval $SELECT_NODE_VERSION
    exitWithMessageOnError "select node version failed"

    if [[ -e "$DEPLOYMENT_TEMP/__nodeVersion.tmp" ]]; then
      NODE_EXE=`cat "$DEPLOYMENT_TEMP/__nodeVersion.tmp"`
      exitWithMessageOnError "getting node version failed"
    fi

    if [[ -e "$DEPLOYMENT_TEMP/.tmp" ]]; then
      NPM_JS_PATH=`cat "$DEPLOYMENT_TEMP/__npmVersion.tmp"`
      exitWithMessageOnError "getting npm version failed"
    fi

    if [[ ! -n "$NODE_EXE" ]]; then
      NODE_EXE=node
    fi

    NPM_CMD="\"$NODE_EXE\" \"$NPM_JS_PATH\""
  else
    NPM_CMD=npm
    NODE_EXE=node
  fi
}

##################################################################################################################################
# Deployment
# ----------

echo Handling TailwindTrader app deployment.

# 1. Install npm packages
if [ -e "$DEPLOYMENT_SOURCE/package.json" ]; then
  printf "BuildId\tWebAppName\tDate\tWithCDNInSeconds\tWithoutCDNInSeconds\tDurationInSeconds\tWithCDN\n" >> /home/site/wwwroot/log_$APPSETTING_WEBSITE_SITE_NAME.csv
  for i in {1..5}
  do  
    cd "$DEPLOYMENT_SOURCE"
    rm -rf node_modules
    rm package-lock.json
    ls -l
    echo "Running npm cache clean"
    eval npm cache clean --force
    eval npm cache verify
    exitWithMessageOnError "npm cache clean failed"
    echo "Running npm install with npm endpoint started: "$SECONDS
    eval npm config set registry https://registry.npmjs.org 
    start1=$SECONDS
    eval npm install 
    exitWithMessageOnError "npm install from npm endpoint failed"
    end1=$SECONDS
    duration1=$(( $end1 - $start1 ))
    echo "************************************************************"
    echo "time taken for installation from npm: "$(( $end1 - $start1 ))
    echo "************************************************************"
    a="https\:\/\/st\-verizon\-arroyc\.azureedge\.net"
    b="https\:\/\/registry\.npmjs\.org"
    rm -rf node_modules
    sed -i "s/$b/$a/g" package-lock.json
    #rm package-lock.json
    ls -l
    echo "Running npm cache clean"
    eval npm cache clean --force
    exitWithMessageOnError "npm cache clean failed"
    echo "Running npm install with cdn started: "$SECONDS
    eval npm config set registry https://st-verizon-arroyc.azureedge.net/
    start2=$SECONDS
    eval npm install
    exitWithMessageOnError "npm install from cdn failed"
    end2=$SECONDS
    duration2=$(( $end2 - $start2 ))
    printf "$i\t$APPSETTING_WEBSITE_SITE_NAME\t$(date)\t$duration2\t$duration1\n" >> /home/site/wwwroot/log_$APPSETTING_WEBSITE_SITE_NAME.csv
    echo "************************************************************"
    echo "time taken for installation from verizon cdn: "$(( $end2 - $start2 ))
    echo "************************************************************"
  done
  
 cd - > /dev/null
fi

# 2. KuduSync
if [[ "$IN_PLACE_DEPLOYMENT" -ne "1" ]]; then
  "$KUDU_SYNC_CMD" -v 50 -f "$DEPLOYMENT_SOURCE/dist" -t "$DEPLOYMENT_TARGET" -n "$NEXT_MANIFEST_PATH" -p "$PREVIOUS_MANIFEST_PATH" -i ".git;.hg;.deployment;deploy.sh"
  exitWithMessageOnError "Kudu Sync failed"
fi

##################################################################################################################################
echo "Finished successfully."
