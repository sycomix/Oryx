# Yarn config is per user, and since the build might run with a non-root account,
# we make sure the yarn cache is set on every build.
YARN_CACHE_DIR=/usr/local/share/yarn-cache
if [ -d $YARN_CACHE_DIR ]
then
    echo "Configuring Yarn cache folder"
    yarn config set cache-folder $YARN_CACHE_DIR
fi

# Since we do not know if the source or destination folder is a shared volume,
# create a folder within the container to restore the packages for better perf.
pkgsDir="/tmp/pkgs"
mkdir -p "$pkgsDir"

if [ -f "$SOURCE_DIR/package.json" ]; then
	cp -f "$SOURCE_DIR/package.json" "$pkgsDir"
fi

if [ -f "$SOURCE_DIR/package-lock.json" ]; then
	cp -f "$SOURCE_DIR/package-lock.json" "$pkgsDir"
fi

if [ -f "$SOURCE_DIR/yarn.lock" ]; then
	cp -f "$SOURCE_DIR/yarn.lock" "$pkgsDir"
fi

if [ "$SOURCE_DIR" == "$DESTINATION_DIR" ]
then
	cd "$pkgsDir"
	{{ PackageInstallCommand }}
	cp -rf "$pkgsDir/node_modules" "$SOURCE_DIR"
else
	{{ if InstallProductionOnlyDependencies }}
		echo Installing production-only packages ...
		echo
		echo "Running '{{ ProductionOnlyPackageInstallCommand }}' ..."
		echo
		cd "$pkgsDir"
		{{ ProductionOnlyPackageInstallCommand }}
		cp -rf "$pkgsDir/node_modules" "$SOURCE_DIR"
		cd "$SOURCE_DIR"
		{{ PackageInstallCommand }}
	{{ else }}
		# Restore all packages
		cd "$pkgsDir"
		{{ PackageInstallCommand }}
		cp -rf "$pkgsDir/node_modules" "$SOURCE_DIR"
	{{ end }}
fi

cd "$SOURCE_DIR"

{{ if NpmRunBuildCommand | IsNotBlank }}
echo
echo "Running '{{ NpmRunBuildCommand }}' ..."
echo
{{ NpmRunBuildCommand }}
{{ end }}

{{ if NpmRunBuildAzureCommand | IsNotBlank }}
echo
echo "Running '{{ NpmRunBuildAzureCommand }}' ..."
echo
{{ NpmRunBuildAzureCommand }}
{{ end }}

if [ "$SOURCE_DIR" != "$DESTINATION_DIR" ]
then
	{{ if InstallProductionOnlyDependencies }}
	echo
	echo Copying production only packages ...
	echo
	mkdir -p "$DESTINATION_DIR"
	cd "$pkgsDir"
	{{ if ZipNodeModulesDir }}
		rm -f "node_modules.tar"
		tar -cf node_modules.tar node_modules
		cp -f node_modules.tar "$DESTINATION_DIR"
	{{ else }}
		cp -rf node_modules "$DESTINATION_DIR"
	{{ end }}

	echo Copying source files ...
	cd "$SOURCE_DIR"
	cp -rf `ls -A | grep -v "node_modules"` "$DESTINATION_DIR"
	{{ else }}
	cd "$SOURCE_DIR"
	cp -rf . "$DESTINATION_DIR"
	{{ end }}
fi

CopyFilesToDestination=false