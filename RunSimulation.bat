ComSim\bin\Release\netcoreapp2.2\publish\comsim.exe -d ComSim\bin\Debug\netcoreapp2.2\Data -e -i "Examples\%1.xml" -s ComSim\bin\Debug\netcoreapp2.2\Sniffers -st Examples\SnifferXmlToHtml.xslt -mr "C:\ProgramData\IoT Gateway\Root\Reports\%1.md" -xr "C:\ProgramData\IoT Gateway\Root\Reports\%1.xml" -master /Master.md -l "C:\ProgramData\IoT Gateway\Root\Reports\%1.Log.xml" -lt "C:\ProgramData\IoT Gateway\Transforms\EventXmlToHtml.xslt" -af ComSim\bin\Release\netcoreapp2.2\publish