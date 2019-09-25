#!/bin/bash
# --------------------------------------------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license.
# --------------------------------------------------------------------------------------------

set -ex

declare -r REPO_DIR=$( cd $( dirname "$0" ) && cd .. && pwd )

source $REPO_DIR/build/__variables.sh

mkdir -p "$BASE_IMAGES_ARTIFACTS_FILE_PREFIX"
artifactFileName="$BASE_IMAGES_ARTIFACTS_FILE_PREFIX/buildPackDeps-images.txt"
repoName="$ACR_STAGING_NAME/public/oryx/base"
buildNumber="${BUILD_NUMBER:-latest}"

sourceImageName="buildpack-deps:stretch"
targetImageName="$repoName:build-$buildNumber"
docker pull "$sourceImageName" 
docker tag "$sourceImageName" "$targetImageName"
echo "$targetImageName" >> "$artifactFileName"

sourceImageName="buildpack-deps:stretch-curl"
targetImageName="$repoName:runtime-$buildNumber"
docker pull "$sourceImageName" 
docker tag "$sourceImageName" "$targetImageName"
echo "$targetImageName" >> "$artifactFileName"