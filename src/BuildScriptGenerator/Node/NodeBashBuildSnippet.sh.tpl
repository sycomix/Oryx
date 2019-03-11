pkgsDir="/tmp/pkgs"
mkdir -p "$pkgsDir"

{{ if InstallProductionOnlyDependencies }}
if [ -f "$SOURCE_DIR/package.json" ]; then
	cp -f "$SOURCE_DIR/package.json" "$pkgsDir"
fi

if [ -f "$SOURCE_DIR/package-lock.json" ]; then
	cp -f "$SOURCE_DIR/package-lock.json" "$pkgsDir"
fi

if [ -f "$SOURCE_DIR/yarn.lock" ]; then
	cp -f "$SOURCE_DIR/yarn.lock" "$pkgsDir"
fi

echo Installing production-only packages ...
echo
echo "Running '{{ ProductionOnlyPackageInstallCommand }}' ..."
echo
cd "$pkgsDir"
{{ ProductionOnlyPackageInstallCommand }}
cp -rf node_modules "$SOURCE_DIR"
{{ end }}

echo Installing packages ...
echo
echo "Running '{{ PackageInstallCommand }}' ..."
echo
cd "$SOURCE_DIR"
# Yarn config is per user, and since the build might run with a non-root account, we make sure
# the yarn cache is set on every build.
YARN_CACHE_DIR=/usr/local/share/yarn-cache
if [ -d $YARN_CACHE_DIR ]
then
    echo "Configuring Yarn cache folder"
    yarn config set cache-folder $YARN_CACHE_DIR
fi

{{ PackageInstallCommand }}

{{ if NpmRunBuildCommand | IsNotBlank }}
echo
echo "Running '{{ NpmRunBuildCommand }}' ..."
echo
cd "$SOURCE_DIR"
{{ NpmRunBuildCommand }}
{{ end }}

{{ if NpmRunBuildAzureCommand | IsNotBlank }}
echo
echo "Running '{{ NpmRunBuildAzureCommand }}' ..."
echo
{{ NpmRunBuildAzureCommand }}
{{ end }}

{{ if InstallProductionOnlyDependencies }}
echo
echo Copying production only packages ...
echo
mkdir -p "$DESTINATION_DIR"
cp -rf $pkgsDir/node_modules "$DESTINATION_DIR"

echo Copying source files ...
cd "$SOURCE_DIR"
cp -rf `ls -A | grep -v "node_modules"` "$DESTINATION_DIR"

{{ else }}
cd "$SOURCE_DIR"
cp -rf . "$DESTINATION_DIR"
{{ end }}

CopyFilesToDestination=false