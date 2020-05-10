# BindingGenerator
Generates assembly binding redirects for .net projects from their bin directories

## Purpose
Trying to work out what assemblies need binding redirects is a huge pain, and working out what those versions should be is an even bigger one. 
NuGet versions don't match assembly versions and then you start hating life!

This application enumerates the dlls in a folder and generates the xml to configure binding redirects for everything it finds.

## Usage
From the command line:
BindingGenerator.Console <input path> <output filename> [string to exclude]

- <input path> is fairly obvious - where should it look for dlls
- <output path> is where you want the xml written. This will overwrite any file that's there without asking so be careful.
- [string to exclude] is an optional param that will exclude dlls that match a string. This isn't clever and doesn't take wildcards, it just removes the value from the output if it contains the string.

### Example
In a folder containing the following
```
MyApp.Core.dll
MyApp.Main.dll
MyApp.exe
Microsoft.ApplicationInsights.dll
System.Memory.dll
```
```BindingGenerator.Console c:\path\to\my\app\bin\Debug\net472 c:\binding-redirects.txt MyApp```

would generate the follwing in c:\binding-redirects.txt

```xml
<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
  <dependentAssembly>
    <assemblyIdentity name="Microsoft.ApplicationInsights" publicKeyToken="31bf3856ad364e35" culture="neutral" />
    <bindingRedirect oldVersion="0.0.0.0-2.11.0.0" newVersion="2.11.0.0" />
  </dependentAssembly>
  <dependentAssembly>
    <assemblyIdentity name="System.Memory" publicKeyToken="cc7b13ffcd2ddd51" culture="neutral" />
    <bindingRedirect oldVersion="0.0.0.0-4.0.1.1" newVersion="4.0.1.1" />
  </dependentAssembly>
</assemblyBinding>
```
