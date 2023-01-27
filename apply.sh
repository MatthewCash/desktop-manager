#! /usr/bin/env nix-shell
#! nix-shell -i bash -p openssh coreutils

# Stop current DesktopManager task (so files can be copied)
ssh host "schtasks /End /TN DesktopManager"
sleep 1

# Patch the binary to make it WinExe instead of Console
printf '\x67\x64\x02\x00\x02' | dd of=./bin/win-x64/publish/DesktopManager.exe bs=1 seek=320 count=5 conv=notrunc

# Copy changed files
rsync -a ./bin/win-x64/publish/* /mnt/win/Users/Matthew/bin/DesktopManager/

# Start task
ssh host "schtasks /Run /TN DesktopManager"
