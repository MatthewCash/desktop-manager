#! /usr/bin/env nix-shell
#! nix-shell -i bash -p bash dotnet-sdk gnugrep coreutils

dotnet publish -c Release

bin_path="./bin/win-x64/publish/DesktopManager.exe"

# Patch the binary to make it WinExe instead of Console
offset=$(grep -obaP -m 1 'PE\x00\x00' "$bin_path" | cut -d: -f1)
printf '\x02\x00' | dd of="$bin_path" bs=1 seek=$((offset + 0x5c)) count=2 conv=notrunc
