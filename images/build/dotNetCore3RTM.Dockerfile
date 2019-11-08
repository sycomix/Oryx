FROM mcr.microsoft.com/oryx/build:20191105.2

ARG SDK_VERSION="3.1.100-preview2-014569"
ARG RUNTIME_VERSION="3.1.0"
ARG SDK_URL="https://dotnetcli.blob.core.windows.net/dotnet/Sdk/${SDK_VERSION}/dotnet-sdk-${SDK_VERSION}-linux-x64.tar.gz"
ARG SDK_SHA="8c7b68efcf67cb365d79bf4481c7639305e158c1a7feb7fc88fd601e2200619fb43897603f4c4eb06b30f97b4cdb992e86bc85d03369a3bef55445ba1d200ba1"
ARG SDKS_DIR="/opt/dotnet/sdks"
ARG RUNTIMES_DIR="/opt/dotnet/runtimes"
ARG DOTNET_DIR="${SDKS_DIR}/${SDK_VERSION}"
ARG DOWNLOAD_ROOT_DIR="/tmp/dotnet-3.1"

RUN mkdir -p ${DOWNLOAD_ROOT_DIR} \
    && curl -SL ${SDK_URL} --output ${DOWNLOAD_ROOT_DIR}/dotnet.tar.gz \
    && echo "${SDK_SHA} ${DOWNLOAD_ROOT_DIR}/dotnet.tar.gz" | sha512sum -c - \
    && mkdir -p ${DOTNET_DIR} \
    && tar -xzf ${DOWNLOAD_ROOT_DIR}/dotnet.tar.gz -C ${DOTNET_DIR} \
    && ln -s ${SDK_VERSION} ${SDKS_DIR}/3.1 \
    && ln -fs ${SDK_VERSION} ${SDKS_DIR}/3 \
    # Remove downloaded files
    && rm -rf ${DOWNLOAD_ROOT_DIR}

RUN set -ex \
    && cd ${RUNTIMES_DIR} \
    # Remove existing lists which point to 3.0.0 version
    && rm -f 3 \
    && rm -f lts \
    && mkdir -p ${RUNTIME_VERSION} \
    && ln -s ${RUNTIME_VERSION} 3.1 \
    && ln -s 3.1 3 \
    && ln -s 3 lts \
    && ln -s ${SDKS_DIR}/${SDK_VERSION} ${RUNTIME_VERSION}/sdk
