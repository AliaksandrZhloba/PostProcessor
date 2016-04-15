Project for optimizing g-code  
  
Author: 		Aliaksandr Zhloba, AZhloba@tut.by  
Environment: 		MS Visual Studio 2013  
Platform: 		.Net Framework 4.0, C#, WPF  
Used packages:		NLog.4.1.2  
  
  
Description:  
Programm is designed for optimizing g-code of CAM Software Editor's output file to prevent redundancy operations and also to have a possibility of fast setting's changing without changing settings of editor's internal postprocessor.    
Additional features:  
1. Program can be associated with editor's output files.  
2. Program can be minimized to tray and to rise when new editor's output file appeared in specified directory.  
3. The resulting file can be copied to the specified drive.  
4. Program's actions are logged into the file, when an error occurred, the information about error can be send automatically.  