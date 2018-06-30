#!/usr/bin/env bash

set -e

sudo ./prepare-environment.sh

cd ../tests/KNXLibTests/

dotnet build 
dotnet test
