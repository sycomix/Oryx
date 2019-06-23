#!/bin/bash
# --------------------------------------------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license.
# --------------------------------------------------------------------------------------------

# This script is intended to be copied with the startup command generator to the container to
# build it, thus avoiding having repeat commands in each Dockerfile. It assumes that the source
# is properly mapped in $GOPATH. The platform and target binary as passed as a positional arguments.

set -e

declare -r WORKSPACE_DIR=$( cd $( dirname "$0" ) && cd .. && pwd )
PLATFORM=$1
TARGET_OUTPUT=$2
export GOPATH="$WORKSPACE_DIR"

function buildPlatform() {
    local pkgDir="$1"
    echo
    echo "Building the package for '$pkgDir'..."
    cd "$pkgDir"
    go build \
        -ldflags "-X common.BuildNumber=$BUILD_NUMBER -X common.Commit=$GIT_COMMIT" \
        -v -o "$TARGET_OUTPUT" .
}

if [ ! -z "$PLATFORM_DIR" ]; then
    if [ ! -d "$PLATFORM_DIR" ]; then
        echo "Invalid platform name '$PLATFORM'. Could not find directory '$PLATFORM_DIR'."
        exit 1
    fi
    ./restore-packages.sh
    buildPlatform $PLATFORM
else
    echo "Platform name not provided. Building for all platforms..."
    ./restore-packages.sh
    for pkgDir in $WORKSPACE_DIR/src/* ; do
        if [ -d "$pkgDir" ]; then
            buildPlatform $pkgDir
        fi
    done
fi
