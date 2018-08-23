This project is used to generate the correct NuGet package.
This is necessary because Fody has a strict way of structuring projects so that they work correctly.

All relevant code is split between 2 other projects:
	- CodeLoom.Fody: This project performs the code-weaving
	- CodeLoom.Runtime: This project contains all code that allows for creating and configuring aspects