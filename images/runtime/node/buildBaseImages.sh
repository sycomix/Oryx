#!/bin/bash
# --------------------------------------------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license.
# --------------------------------------------------------------------------------------------

set -e

declare -r REPO_DIR=$( cd $( dirname "$0" ) && cd ../../.. && pwd )
declare -r CURRENT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null && pwd )"

source $REPO_DIR/build/__variables.sh
source $REPO_DIR/build/__baseImageTags.sh
source $REPO_DIR/build/__nodeVersions.sh

dockerFiles=$(find $CURRENT_DIR -type f \( -name "base.Dockerfile" \) )

labels=" --label com.microsoft.oryx.git-commit=$GIT_COMMIT"
labels+=" --label com.microsoft.oryx.build-number=$BUILD_NUMBER"

for dockerFile in $dockerFiles; do
    dockerFileDir=$(dirname "${dockerFile}")
    version=$(basename $dockerFileDir)

    buildArgs=""
    # Prefix the tag name with '-' if there is actually a value for the tag, otherwise we want it to be empty
    buildArgs+=" --build-arg NODE_RUNTIME_BASE_TAG=${NODE_RUNTIME_BASE_TAG:+-$NODE_RUNTIME_BASE_TAG}"

    case $version in
    '6')
        buildArgs+=" --build-arg NODE6_VERSION=$NODE6_VERSION"
        ;;
    '8')
        buildArgs+=" --build-arg NODE8_VERSION=$NODE8_VERSION"
        ;;
    '10')
        buildArgs+=" --build-arg NODE10_VERSION=$NODE10_VERSION"
        ;;
    '12')
        buildArgs+=" --build-arg NODE12_VERSION=$NODE12_VERSION"
        ;;
    esac

    uniqueTagSuffix=${RELEASE_TAG_NAME:+-$RELEASE_TAG_NAME}

    echo
    echo "Building base image for .NET Core version '$version'..."
    docker build \
        -t $BASE_IMAGES_REPO:node-$version$uniqueTagSuffix \
        -f $dockerFile \
        $buildArgs \
        $labels \
        $REPO_DIR
done