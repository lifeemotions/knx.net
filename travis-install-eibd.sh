#!/bin/sh

sudo apt-get -y install xsltproc libxml2-utils libc6 zlib1g

wget http://ppa.launchpad.net/mkoegler/bcusdk/ubuntu/pool/main/b/bcusdk/bcusdk_0.0.5-1~oneiric2_all.deb
wget http://ppa.launchpad.net/mkoegler/bcusdk/ubuntu/pool/main/b/bcusdk/eibd-server_0.0.5-1~oneiric2_amd64.deb
wget http://ppa.launchpad.net/mkoegler/bcusdk/ubuntu/pool/main/b/bcusdk/eibd-clients_0.0.5-1~oneiric2_amd64.deb
wget http://ppa.launchpad.net/mkoegler/bcusdk/ubuntu/pool/main/b/bcusdk/eibd-client-sources_0.0.5-1~oneiric2_all.deb
wget http://ppa.launchpad.net/mkoegler/bcusdk/ubuntu/pool/main/b/bcusdk/libeibclient-dev_0.0.5-1~oneiric2_amd64.deb
wget http://ppa.launchpad.net/mkoegler/bcusdk/ubuntu/pool/main/b/bcusdk/bcusdk-build_0.0.5-1~oneiric2_amd64.deb
wget http://ppa.launchpad.net/mkoegler/bcusdk/ubuntu/pool/main/p/pthsem/libpthsem20_2.0.8-1~oneiric1_amd64.deb
wget http://ppa.launchpad.net/mkoegler/bcusdk/ubuntu/pool/main/b/bcusdk/libeibclient0_0.0.5-1~oneiric2_amd64.deb
wget http://ppa.launchpad.net/mkoegler/bcusdk/ubuntu/pool/main/m/m68hc05-gnu/m68hc05-gcc_0.0.2-1~oneiric2_amd64.deb
wget http://ppa.launchpad.net/mkoegler/bcusdk/ubuntu/pool/main/m/m68hc05-gnu/m68hc05-runtime_0.0.2-1~oneiric2_all.deb
wget http://ppa.launchpad.net/mkoegler/bcusdk/ubuntu/pool/main/m/m68hc05-gnu/m68hc05-binutils_0.0.2-1~oneiric2_amd64.deb

PACKAGES="bcusdk_0.0.5-1~oneiric2_all.deb eibd-server_0.0.5-1~oneiric2_amd64.deb"
PACKAGES="$PACKAGES eibd-clients_0.0.5-1~oneiric2_amd64.deb eibd-client-sources_0.0.5-1~oneiric2_all.deb"
PACKAGES="$PACKAGES libeibclient-dev_0.0.5-1~oneiric2_amd64.deb bcusdk-build_0.0.5-1~oneiric2_amd64.deb"
PACKAGES="$PACKAGES libpthsem20_2.0.8-1~oneiric1_amd64.deb libeibclient0_0.0.5-1~oneiric2_amd64.deb"
PACKAGES="$PACKAGES m68hc05-gcc_0.0.2-1~oneiric2_amd64.deb m68hc05-runtime_0.0.2-1~oneiric2_all.deb"
PACKAGES="$PACKAGES m68hc05-binutils_0.0.2-1~oneiric2_amd64.deb"

sudo dpkg -i $PACKAGES

rm -f $PACKAGES
