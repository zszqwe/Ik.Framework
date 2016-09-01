NuGet.exe pack ..\Ik.Framework\Ik.Framework.csproj -Prop Configuration=Release
NuGet.exe push Ik.Framework.*.nupkg my2015 -s http://172.16.0.200/