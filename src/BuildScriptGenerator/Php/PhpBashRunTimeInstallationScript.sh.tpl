apt-get install apt-transport-https lsb-release ca-certificates
curl https://packages.sury.org/php/apt.gpg | apt-key add -
echo "deb https://packages.sury.org/php/ stretch main" > /etc/apt/sources.list.d/php.list
apt-get update
apt-get install php{{ VersionMajorAndMinor }}
apt-get install libapache2-mod-php{{ VersionMajorAndMinor }}
apt-get install php{{ VersionMajorAndMinor }}-cli php{{ VersionMajorAndMinor }}-common
apt-get install php{{ VersionMajorAndMinor }}-curl php{{ VersionMajorAndMinor }}-mbstring php{{ VersionMajorAndMinor }}-mysql php{{ VersionMajorAndMinor }}-xml
