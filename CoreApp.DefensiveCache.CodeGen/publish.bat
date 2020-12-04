::dotnet publish CoreApp.DefensiveCache.CodeGen.csproj -c Release -r win-x86 --self-contained
::dotnet publish CoreApp.DefensiveCache.CodeGen.csproj -c Release -r win-x64 --self-contained
::dotnet publish CoreApp.DefensiveCache.CodeGen.csproj -c Release -r win-arm --self-contained
::dotnet publish CoreApp.DefensiveCache.CodeGen.csproj -c Release -r osx-x64 --self-contained
dotnet publish CoreApp.DefensiveCache.CodeGen.csproj -c Release -r linux-x64 --self-contained
dotnet publish CoreApp.DefensiveCache.CodeGen.csproj -c Release -r linux-arm --self-contained
dotnet publish CoreApp.DefensiveCache.CodeGen.csproj -c Release -o bin\Release\netcoreapp3.1\portable\publish
pause