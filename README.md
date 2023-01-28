# desktop-manager

A program to change various aspects of the Windows shell/desktop.

Refer to the [config file](resources/config.toml) for available options.

## Building

This program is meant to be built on Linux, specifically NixOS using the `build.sh` script

On Linux, dotnet does not support customizing the executable, so to change the Windows subsystem, the binary is patched in the same build script
