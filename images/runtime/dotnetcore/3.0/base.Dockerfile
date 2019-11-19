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

# Install .NET Core
RUN set -ex \
    && curl -SL --output dotnet.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/Runtime/$NET_CORE_APP_30/dotnet-runtime-$NET_CORE_APP_30-linux-x64.tar.gz \
    && echo "$NET_CORE_APP_30_SHA dotnet.tar.gz" | sha512sum -c - \
    && mkdir -p /usr/share/dotnet \
    && tar -zxf dotnet.tar.gz -C /usr/share/dotnet \
    && rm dotnet.tar.gz \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
    
# Install ASP.NET Core
RUN set -ex \
    && curl -SL --output aspnetcore.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/aspnetcore/Runtime/$ASP_NET_CORE_RUN_TIME_VERSION_30/aspnetcore-runtime-$ASP_NET_CORE_RUN_TIME_VERSION_30-linux-x64.tar.gz \
    && echo "$ASP_NET_CORE_RUN_TIME_VERSION_30_SHA  aspnetcore.tar.gz" | sha512sum -c - \
    && mkdir -p /usr/share/dotnet \
    && tar -zxf aspnetcore.tar.gz -C /usr/share/dotnet ./shared/Microsoft.AspNetCore.App \
    && rm aspnetcore.tar.gz

RUN dotnet-sos install
