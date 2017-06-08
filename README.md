# FMDKiller
Auto closes Visual Studio 2017 "File Modification Detected" dialog and reloads solution.

For debugging:
- Open the project Properties
- Open the Debug tab
- In the section Start action select Start external program and select the path to devenv.exe
- In the section Start options enter the Command line arguments: /rootsuffix Exp
