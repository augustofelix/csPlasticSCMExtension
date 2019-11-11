=== How to compile the solution ===

- You need to replace the next RedmineExtensionVS2008 references:

   - Codice.I3.dll
   - commontypes.dll
   - utils.dll
   - extensioncommon.dll

You have to replace this references by those found in your Plastic client folder.

Then, you will be able to build the solution, and just need to copy the generated dlls (RedmineApi.dll, RedmineExtension.dll ) in your Plastic client folder.


The last step is to create a "customextensions.conf" file in your Plastic client folder with the next text:
Redmine=RedmineExtension.dll


After following the previous steps, you will found a new "Redmine" field in Plastic Issue tracker menu.


If you have any issue, we will be happy to assist you at:
support@codicesoftware.com

