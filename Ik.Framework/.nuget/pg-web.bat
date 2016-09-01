NuGet.exe pack ..\Ik.WebFramework\Ik.WebFramework.csproj -Prop Configuration=Release
NuGet.exe push Ik.WebFramework.*.nupkg my2015 -s http://172.16.0.200/