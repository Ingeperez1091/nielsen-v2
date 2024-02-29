dotnet test --logger "trx;LogFileName=TestResults.trx" ^
            --logger "nunit;LogFileName=TestResults.xml" ^
            --results-directory ./Coverage ^
            /p:CollectCoverage=true ^
            /p:CoverletOutput=Coverage\ ^
            /p:CoverletOutputFormat=cobertura ^
            /p:Exclude=\"[nunit.*]*,[*]UniversalValidatorApi.*\"

dotnet %userprofile%\.nuget\packages\reportgenerator\5.1.10\tools\net6.0\ReportGenerator.dll ^
	"-reports:Coverage\coverage.cobertura.xml" ^
	"-targetdir:Coverage" ^
	-reporttypes:HTML

start .\Coverage\index.htm\