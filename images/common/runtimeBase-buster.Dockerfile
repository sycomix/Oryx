FROM oryxmcr.azurecr.io/public/oryx/base:buildpack-deps-buster-curl

RUN apt-get update \
	&& apt-get upgrade -y \
	&& apt-get install -y --no-install-recommends \
		xz-utils \
	&& rm -rf /var/lib/apt/lists/*

COPY images/receiveGpgKeys.sh /tmp/scripts/receiveGpgKeys.sh
RUN chmod +x /tmp/scripts/receiveGpgKeys.sh
