#!/bin/sh

FLAGS="-O2 -Wall -shared"

x86_64-w64-mingw32-gcc $FLAGS ../Plasma.c -o Plasma.dll

strip Plasma.dll

cp -f Plasma.dll ../../Assets/Plugins/Windows/Plasma.dll