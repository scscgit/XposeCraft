#! /bin/sh

# Example build script for Unity3D project. See the entire example: https://github.com/JonathanPorta/ci-build

# Change this the name of your project. This will be the name of the final executables as well.
project="XposeCraft"

# I used executeMethod workaround, but a not-yet tested -testscenes=TestScene1,TestScene2 that is not in a documentation may possibly work too

echo "Attempting to build $project for Windows"
export location="$(pwd)/Builds/windows/$project.exe"
export target="win"
export options="none"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd) \
  -executeMethod "XposeCraft.UnityWorkarounds.BuildProject.Build"
win=$?

echo "Attempting to build $project for OS X"
export location="$(pwd)/Builds/osx/$project.exe"
export target="osx"
export options="none"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd) \
  -executeMethod "XposeCraft.UnityWorkarounds.BuildProject.Build"
osx=$?

echo "Attempting to build $project for Linux"
export location="$(pwd)/Builds/linux/$project.exe"
export target="linux"
export options="none"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd) \
  -executeMethod "XposeCraft.UnityWorkarounds.BuildProject.Build"
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
  echo "OS X build failed with error code $osx."
  exit $osx
fi

if [ $linux -ne 0 ]
then
  echo "Linux build failed with error code $linux."
  exit $linux
fi

echo "All builds have finished successfully."
