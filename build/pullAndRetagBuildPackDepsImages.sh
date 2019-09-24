#!/bin/bash
# --------------------------------------------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license.
# --------------------------------------------------------------------------------------------

set -e

declare -r REPO_DIR=$( cd $( dirname "$0" ) && cd .. && pwd )

source $REPO_DIR/build/__variables.sh

mkdir -p "$BASE_IMAGES_ARTIFACTS_FILE_PREFIX"
artifactFileName="$BASE_IMAGES_ARTIFACTS_FILE_PREFIX/buildPackDeps-images.txt"

imageName="buildpack-deps:stretch"
docker pull "$imageName" 
docker tag "$imageName" "oryx-$imageName-$IMAGE_TAG"
echo "$imageName-$IMAGE_TAG" >> "$artifactFileName"

imageName="buildpack-deps:stretch-curl"
docker pull "$imageName" 
docker tag "$imageName" "oryx-$imageName-$IMAGE_TAG"
echo "$imageName-$IMAGE_TAG" >> "$artifactFileName"