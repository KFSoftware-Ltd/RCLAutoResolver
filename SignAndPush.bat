@ECHO OFF
IF "%NUGET_API_KEY%"=="" (
  ECHO Nuget API Key is NOT defined.
  goto end
)

for /D %%i in (.\src\*) do (
  echo Entering %%i...
  cd %%i
  echo.  Removing build output...
  rd /s /q .\bin > NUL
  rd /s /q .\obj > NUL

  echo.  Building project...
  dotnet restore > NUL
  dotnet build --configuration Release --no-restore > NUL
  dotnet pack --configuration Release --no-restore --no-build > NUL

  cd bin
  cd release

  echo.  Signing NuGet Packages...
  for %%f in (*.nupkg) do (
    echo.    Signing %%f
    nuget sign %%f -OutputDirectory .\Signed -Timestamper http://timestamp.sectigo.com -CertificateStoreName My -CertificateFingerprint 76301C8984CA0BCB84F259A0083CC6BCABCE608F > NUL && del %%f
  )
  for %%f in (*.snupkg) do (
    echo.    Signing %%f
    nuget sign %%f -OutputDirectory .\Signed -Timestamper http://timestamp.sectigo.com -CertificateStoreName My -CertificateFingerprint 76301C8984CA0BCB84F259A0083CC6BCABCE608F > NUL && del %%f
  )
  echo.  Pushing NuGet Packages...
  for %%f in (.\Signed\*.nupkg) do (
    echo.    Pushing %%f
    nuget push -ApiKey %NUGET_API_KEY% -source https://api.nuget.org/v3/index.json %%f > NUL && del %%f
  )
  for %%f in (.\Signed\*.snupkg) do (
    echo.    Pushing %%f
    nuget push -ApiKey %NUGET_API_KEY% -source https://api.nuget.org/v3/index.json %%f > NUL && del %%f
  )
  cd ..
)

:end
pause