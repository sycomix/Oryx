ARG DOT_NET_CORE_RUNTIME_BASE_TAG
ARG DOT_NET_CORE_30_SDK_VERSION

# dotnet tools are currently available as part of SDK so we need to create them in an sdk image
# and copy them to our final runtime image
FROM mcr.microsoft.com/dotnet/core/sdk:${DOT_NET_CORE_30_SDK_VERSION} AS tools-install
RUN dotnet tool install --tool-path /dotnetcore-tools dotnet-sos
RUN dotnet tool install --tool-path /dotnetcore-tools dotnet-trace
RUN dotnet tool install --tool-path /dotnetcore-tools dotnet-dump
RUN dotnet tool install --tool-path /dotnetcore-tools dotnet-counters

FROM mcr.microsoft.com/oryx/base:dotnetcore-runtime-buster${DOT_NET_CORE_RUNTIME_BASE_TAG}

RUN mkdir -p /tmp/scripts
COPY images/runtime/dotnetcore/installDotNetCore.sh /tmp/scripts

# Configure web servers to bind to port 80 when present
ENV ASPNETCORE_URLS=http://+:80 \
    # Enable detection of running in a container
    DOTNET_RUNNING_IN_CONTAINER=true

COPY --from=tools-install /dotnetcore-tools /opt/dotnetcore-tools
ENV PATH="/opt/dotnetcore-tools:${PATH}"

ARG NET_CORE_APP_30
ARG NET_CORE_APP_30_SHA
ARG ASP_NET_CORE_RUN_TIME_VERSION_30
ARG ASP_NET_CORE_RUN_TIME_VERSION_30_SHA

# Install .NET Core & ASP.NET Core runtimes
RUN set -ex \
    && . /tmp/scripts/installDotNetCore.sh \
    && installDotNetCoreRuntime $NET_CORE_APP_30 $NET_CORE_APP_30_SHA \
    && installAspNetCoreRuntime $ASP_NET_CORE_RUN_TIME_VERSION_30 $ASP_NET_CORE_RUN_TIME_VERSION_30_SHA

RUN dotnet-sos install
RUN rm -rf /tmp/scripts