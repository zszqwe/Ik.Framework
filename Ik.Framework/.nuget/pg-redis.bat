NuGet.exe pack ..\Ik.Framework.Redis\Ik.Framework.Redis.csproj -Prop Configuration=Release
NuGet.exe push Ik.Framework.Redis.*.nupkg my2015 -s http://172.16.0.200/