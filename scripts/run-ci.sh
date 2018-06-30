#!/usr/bin/env bash

set -e

export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true
export DOTNET_CLI_TELEMETRY_OPTOUT=true

sudo ./scripts/prepare-environment.sh

cd ./tests/KNXLibTests/

dotnet build 
dotnet test
