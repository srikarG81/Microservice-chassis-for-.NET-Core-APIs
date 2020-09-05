ECHO OFF

SETLOCAL=ENABLEDELAYEDEXPANSION

    REN NugetVersionNumber.txt text.tmp
    FOR /f %%a in (text.tmp) do (
        SET ver=%%a 			
        ) 
SET /A VersionNumber=!ver!+1
ECHO %VersionNumber% >> NugetVersionNumber.txt
DEL text.tmp

SET output_dir="local-nuget-repo"

SET project_dir="..\BT.SaaS.UKB.API.Core"

SET curr_dir=%cd%"

SET version=1.0.%VersionNumber%

::Delete output_dir if already exists

RMDIR /S /Q  %output_dir%

ECHO "Building the solution.."

FOR /R %project_dir% %%G IN (*.sln) DO (

dotnet build %%G --configuration Release

)

ECHO %errorlevel%
IF %errorlevel% NEQ 0 GOTO ERROR

ECHO "Creating Nuget packages.."

FOR /R %project_dir% %%G IN (*.csproj) DO (

dotnet pack %%G -p:PackageVersion=%version% --no-build --output %output_dir% --configuration Release

)


ECHO %errorlevel%
IF %errorlevel% NEQ 0 GOTO ERROR

SET /P upload_required=Do you want to upload packages to nuget server? Y/N:
IF "%upload_required%" NEQ "y" GOTO OK
ECHO "uploading nuget pack to server https://agile.nat.bt.com/nexus/repository/APP11755_UKB-Agent-Online_nuget_hosted .. "
FOR %%G in ("%output_dir%\*.nupkg") DO (

dotnet nuget push %%G -k 7a78267a-e609-3f50-9c8d-e1c5df69995f -s https://agile.nat.bt.com/nexus/repository/APP11755_UKB-Agent-Online_nuget_hosted
)

ECHO "Package uploaded successfuly."
SET /P delete_required=Do you want to delete local nuget temp folder? Y/N:
IF "%delete_required%" NEQ "y" GOTO OK
RMDIR /S /Q  %output_dir%

ECHO %errorlevel%
IF %errorlevel% NEQ 0 ( GOTO ERROR )
GOTO OK

:ERROR
ECHO ON
ECHO "Program failed, please check this log file for errors ..."
ECHO OFF
GOTO END

:OK
ECHO ON
ECHO "Program successful"
ECHO OFF
GOTO END

:END
CD %curr_dir%

