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
source $REPO_DIR/build/__phpVersions.sh

dockerFiles=$(find $CURRENT_DIR -type f \( -name "base.Dockerfile" \) )

labels=" --label com.microsoft.oryx.git-commit=$GIT_COMMIT"
labels+=" --label com.microsoft.oryx.build-number=$BUILD_NUMBER"

for dockerFile in $dockerFiles; do
    dockerFileDir=$(dirname "${dockerFile}")
    version=$(basename $dockerFileDir)

    buildArgs=""
    # Prefix the tag name with '-' if there is actually a value for the tag, otherwise we want it to be empty
    buildArgs+=" --build-arg DOT_NET_CORE_RUNTIME_BASE_TAG=${DOT_NET_CORE_RUNTIME_BASE_TAG:+-$DOT_NET_CORE_RUNTIME_BASE_TAG}"


done