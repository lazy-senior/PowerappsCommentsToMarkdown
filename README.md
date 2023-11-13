# PowerappsCommentsToMarkdown 
Generates a markdown documentation from the comments in a powerapps app.

Based on https://github.com/sebastian-muthwill/powerapps-docstring, which somehow didnt work for me.

# usage
-i: Input Folder (Path to unzipped app)

-o: Output Folder

-c: Config File (see config.yaml)

# config 
﻿propertiesToScan: these properties will be searched for comments on every object in objectsToScan. (Only use common properties here, which are present in (almost) every object)
 
objectsToScan: objects and their properties which will be searched for comments (For object-specific properties)
