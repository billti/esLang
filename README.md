# esLang: VS 2017 language service skeleton

A simple project for implementing a basic VSPackage and Language Service for VS 2017.

The .es classifier uses the Managed Package Framework and 'legacy' interfaces currently, not
the newer MEF object model (or the Roslyn APIs).

The .mjs classifier uses the new MEF based model.

## Notes

After cloning this repo, debugging may not work. Go into project properties and
ensure the following are set in the 'Debug' tab:

 - Start External Program: C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe
 - Command line arguments: /rootsuffix Exp

On the 'VSIX' tab 'Create VSIX container' and 'Deploy VSIX content' should be 
checked also.