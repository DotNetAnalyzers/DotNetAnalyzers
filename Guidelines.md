DotNetAnalyzer Guidelines
=========================
The following are the guidelines for the behavior of the analyzers and refactoring extensions.

Behavior
--------
+ Potentially controversial analyzers and refactoring tools will be off by default.
For example, although an analyzer for the existence of #region or var/no-var is acceptable, by default such analyzers will be off by default, and need to be turned on by the user using the ruleset file or other configuration setting.


Coding Standards
-----------------
+ All code must pass the DotNetAnalyzers that are on by default.
+ At a minimum, all "on by default" analyzers must have significant unit testing.
(This is stated as less than all to allow the project to ramp up initially.)
+ Code within a file should be consistent, conforming to other coding standards in said file.
