#! /bin/sh

# Example build script for Unity3D project. See the entire example: https://github.com/JonathanPorta/ci-build

# Change this the name of your project. This will be the name of the final executables as well.
project="XposeCraft"

# I used executeMethod workaround, but a not-yet tested -testscenes=TestScene1,TestScene2 that is not in a documentation may possibly work too

echo "Attempting to build $project for Windows"
export location="$(pwd)/Build/windows/$project.exe"
export target="win"
export options=""
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd) \
  -executeMethod "BuildProject.Build" \
  -quit
win=$?

echo "Attempting to build $project for OS X"
export location="$(pwd)/Build/osx/$project.exe"
export target="osx"
export options=""
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd) \
  -executeMethod "BuildProject.Build" \
  -quit
osx=$?

echo "Attempting to build $project for Linux"
export location="$(pwd)/Build/linux/$project.exe"
export target="linux"
export options=""
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd) \
  -executeMethod "BuildProject.Build" \
  -quit
linux=$?

echo 'Logs from build'
cat $(pwd)/unity.log

if [ $win -ne 0 ]
then
  echo "Windows build failed with error code $win."
  exit $win
fi

if [ $osx -ne 0 ]
then
  echo "OS X build failed with error code $win."
  exit $osx
fi

if [ $linux -ne 0 ]
then
  echo "Linux build failed with error code $win."
  exit $linux
fi

echo "All builds have finished successfully."
