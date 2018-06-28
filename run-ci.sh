#!/usr/bin/env bash

set -e

./run-tests.sh

cd ./src/KNXLib
dotnet pack -o ../../output/ -c Release
