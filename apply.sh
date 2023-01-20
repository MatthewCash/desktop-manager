#! /usr/bin/env nix-shell
#! nix-shell -i bash -p openssh

ssh host "schtasks /End /TN DesktopManager && schtasks /Run /TN DesktopManager"
