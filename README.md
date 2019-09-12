# Communication Wcf and Windows service

Implemented WCF and Windows services. WCF service stores data in one of two repositories (this is specified in the configuration file). 
The first stores the data in the database, the second is just in the folder (the path to the folder is configured in the configuration file). 
Implemented Windows service that monitors the Monitor folder (the path to the folder is configured in the configuration file). When added 
to the "correct" folder check in the form of *.txt file with JSON record of this format {"Id":1,"CheckNumber":"No. 123456","Summ":100500.0,
"Discount":105.5,"Articles":"data1;data2;data3"}, windows service calls the wcf service and the method 
saves data to a database or folder. The "correct file" is moved to the "Complite" folder. "Wrong" file (deserialization errors, not *.txt 
extension, etc.) is transferred to the Garbage folder. Folder paths are configured in the configuration file.

There are five projects in the solution. Two main - wcf (ConsoleAppServiceHost) and windows service (WindowsServiceFolderMonitor) and three 
support - dll assemblies whose objects are used in two main projects.The Wcf service is hosted in a console application for simplicity. 
Logging is conducted. 

Order of use:
1. Clone/download from git hub https://github.com/proskurin-vp/WcfService solution. Rebuild the solution (using Visual Studio 2017) - 
the necessary libraries will be tightened.
2. Run wcf service (as administrator).
3. Install windows service. You can use the InstallUtil utility to do this.exe.
4. Start the windows service. It will create a folder Monitor, Complite, Garbage. The paths to them are taken from the configuration file.
5. When you move the" correct " check file to the Monitor folder, its data will be written to the database or file (depending on the settings 
in the file configuration) and it will be moved by the Windows service to the Complite folder. If an "incorrect" file is moved to the 
Monitor folder, it will be moved to the Garbage folder. If the file name is the same as those already in the Complite or Garbage folders, 
a timestamp will be appended to the file name. 

The database is local, in the bin folder, connected via connectionString. Writing to a table is performed by a stored procedure through dapper.
