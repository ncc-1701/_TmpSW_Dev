#! /bin/sh

git add '*.c'
git add '*.cpp'
git add '*.cs'
git add '*.h'
git add '*.resx'

find . -name 'README' | xargs git add

git add '*.csproj'
git add '*.config'
git add '*.settings'
git add '*.md'
git add '*.txt'
git add '*.url'
git add '*.sh'
