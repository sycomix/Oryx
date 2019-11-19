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
source $REPO_DIR/build/__dotNetCoreRunTimeVersions.sh
source $REPO_DIR/build/__aspNetCoreRunTimeVersions.sh
source $REPO_DIR/build/__dotNetCoreSdkVersions.sh

dockerFiles=$(find $CURRENT_DIR -type f \( -name "base.Dockerfile" \) )

labels=" --label com.microsoft.oryx.git-commit=$GIT_COMMIT"
labels+=" --label com.microsoft.oryx.build-number=$BUILD_NUMBER"

for dockerFile in $dockerFiles; do
    dockerFileDir=$(dirname "${dockerFile}")
    version=$(basename $dockerFileDir)

    buildArgs=""
    # Prefix the tag name with '-' if there is actually a value for the tag, otherwise we want it to be empty
    buildArgs+=" --build-arg DOT_NET_CORE_RUNTIME_BASE_TAG=${DOT_NET_CORE_RUNTIME_BASE_TAG:+-$DOT_NET_CORE_RUNTIME_BASE_TAG}"

    case $version in
        '1.0')
            buildArgs+=" --build-arg NET_CORE_APP_10=$NET_CORE_APP_10"
            ;;
        '1.1')
            buildArgs+=" --build-arg NET_CORE_APP_11=$NET_CORE_APP_11"
            ;;
        '2.0')
            buildArgs+=" --build-arg NET_CORE_APP_20=$NET_CORE_APP_20"
            ;;
        '2.1')
            buildArgs+=" --build-arg DOT_NET_CORE_21_SDK_VERSION=$DOT_NET_CORE_21_SDK_VERSION"
            buildArgs+=" --build-arg NET_CORE_APP_21=$NET_CORE_APP_21"
            buildArgs+=" --build-arg NET_CORE_APP_21_SHA=$NET_CORE_APP_21_SHA"
            ;;
        '2.2')
            buildArgs+=" --build-arg DOT_NET_CORE_22_SDK_VERSION=$DOT_NET_CORE_22_SDK_VERSION"
            buildArgs+=" --build-arg NET_CORE_APP_22=$NET_CORE_APP_22"
            buildArgs+=" --build-arg NET_CORE_APP_22_SHA=$NET_CORE_APP_22_SHA"
            ;;
        '3.0')
            buildArgs+=" --build-arg DOT_NET_CORE_30_SDK_VERSION=$DOT_NET_CORE_30_SDK_VERSION"
            buildArgs+=" --build-arg DOT_NET_CORE_30_SDK_SHA512=$DOT_NET_CORE_30_SDK_SHA512"
            buildArgs+=" --build-arg NET_CORE_APP_30=$NET_CORE_APP_30"
            buildArgs+=" --build-arg NET_CORE_APP_30_SHA=$NET_CORE_APP_30_SHA"
            buildArgs+=" --build-arg ASP_NET_CORE_RUN_TIME_VERSION_30=$ASP_NET_CORE_RUN_TIME_VERSION_30"
            buildArgs+=" --build-arg ASP_NET_CORE_RUN_TIME_VERSION_30_SHA=$ASP_NET_CORE_RUN_TIME_VERSION_30_SHA"
            ;;
        *)
            echo "Unknown version directory"
            ;;
    esac

    uniqueTagSuffix=${RELEASE_TAG_NAME:+-$RELEASE_TAG_NAME}

    echo
    echo "Building base image for .NET Core version '$version'..."
    docker build \
        -t $BASE_IMAGES_REPO:dotnetcore-$version$uniqueTagSuffix \
        -f $dockerFile \
        $buildArgs \
        $labels \
        $REPO_DIR
done