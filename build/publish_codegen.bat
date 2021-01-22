echo build CodeGen...
dotnet publish ..\CoreApp.DefensiveCache.CodeGen\CoreApp.DefensiveCache.CodeGen.csproj -c Release -r linux-x64 --self-contained
dotnet publish ..\CoreApp.DefensiveCache.CodeGen\CoreApp.DefensiveCache.CodeGen.csproj -c Release -r linux-arm --self-contained
dotnet publish ..\CoreApp.DefensiveCache.CodeGen\CoreApp.DefensiveCache.CodeGen.csproj -c Release -o ..\CoreApp.DefensiveCache.CodeGen\bin\Release\netcoreapp3.1\portable\publish
pause