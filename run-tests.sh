#!/usr/bin/env bash

set -e

apt-get update
apt-get install -y --no-install-recommends \
        ca-certificates \
        libc6 \
        libgcc1 \
        libgssapi-krb5-2 \
        libicu52 \
        liblttng-ust0 \
        libssl1.0.0 \
        libstdc++6 \
        wget \
        zlib1g

export DOTNET_SDK_VERSION=2.1.301

wget -O dotnet.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/Sdk/$DOTNET_SDK_VERSION/dotnet-sdk-$DOTNET_SDK_VERSION-linux-x64.tar.gz
mkdir -p /usr/share/dotnet
tar -zxf dotnet.tar.gz -C /usr/share/dotnet
rm dotnet.tar.gz
ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

./install-eibd.sh

cd ./tests/KNXLibTests/

dotnet build 
dotnet test
