NuGet.exe pack ..\Ik.Service.Auth\Ik.Service.Auth.csproj -Prop Configuration=Release
NuGet.exe push Ik.Service.Auth.*.nupkg my2015 -s http://172.16.0.200/