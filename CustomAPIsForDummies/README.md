# Schedule API Demos 

This folder contains a C# Console Application and Microsoft Dataverse Solution with a Power Automate Flow, both of which are designed to demonstrate the functionality on offer as part of the [Dynamics 365 Project Operations Schedule API](https://docs.microsoft.com/en-us/dynamics365/project-operations/project-management/schedule-api-preview?WT.mc_id=BA-MVP-5003861).

## How to Use

### C# Console App

1. Clone the repository
2. Open the **D365.ScheduleAPIDemos** Solution using Visual Studio.
3. Within the **ScheduleAPIConsoleDemo** project, add a new XML file to the project named **local.config**. Populate the file with an adapted version of the following XML, with your environment URL entered. For example, if your environmeht has the URL **https://mycrm.crm11.dynamics.com**:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<appSettings>
	<add key="d365URL" value="https://mycrm.crm11.dynamics.com/" />
</appSettings>
```

4. Build and run the application from Visual Studio. Observe the output window and the press return, where appropriate, to execute each of the demos.

### Solution

1. Import the **ScheduleAPIDemos_1_0_0_1.zip** into your Dataverse environment which contains Dynamics 365 Project Operations.
2. Ensure that you have at least 1 Contact row present in the system.
3. Navigate to the Solution and to the **Schedule API Demo** cloud flow.
4. Select **Run**. You will be prompted to provide a name for the Project that will be created.
5. Observe the Flow executing. It will take approximately 2 minutes and the end result will be a Project containing a list of sample Team Members and Project Tasks.