#!/bin/bash

echo "Testing with Autofac version 4.2.0"
dotnet test -c Debug_AF42

echo "Testing with Autofac version 4.9.4"
dotnet test -c Debug_AF49

echo "Testing with Autofac version 5.2.0"
dotnet test -c Debug_AF52

echo "Testing with Castle.Core version 4.0.0"
dotnet test -c Debug_CC40

echo "Testing with Castle.Core version 4.4.1"
dotnet test -c Debug_CC44
