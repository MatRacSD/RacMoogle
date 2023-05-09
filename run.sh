#!/bin/bash


cd "$(dirname "$0")"


cd MoogleServer/


dotnet restore


dotnet build


dotnet run 



