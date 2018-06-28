#!/usr/bin/env bash

set -e

sudo ./prepare-environment.sh

pushd ./tests/KNXLibTests/

dotnet build 
dotnet test

popd

pushd ./src/KNXLib

dotnet pack -o ../../output/ -c Release

popd
