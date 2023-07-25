#!/bin/sh

FLAGS="-O2 -Wall -shared"

x86_64-w64-mingw32-g++ $FLAGS ./PluginBinder.cpp -o PluginBinder.dll

strip PluginBinder.dll

cp -f PluginBinder.dll ../../Assets/Plugins/Windows/PluginBinder.dll