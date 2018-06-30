#!/usr/bin/env bash

set -e

sudo ./scripts/prepare-environment.sh

cd ./tests/KNXLibTests/

dotnet build 
dotnet test
