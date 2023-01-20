#! /usr/bin/env nix-shell
#! nix-shell -i bash -p dotnet-sdk

dotnet publish -c Release
