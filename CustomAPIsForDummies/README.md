# Schedule API Demos 

This folder contains a C# Class Assembly project and Microsoft Dataverse Managed/Unmanaged Solutions, designed to demonstrate the functionality on offer as part of [Custom APIs](https://docs.microsoft.com/en-us/power-apps/developer/data-platform/custom-api?WT.mc_id=BA-MVP-5003861).

The assets within this folder were first presented as part of a talk I gave during the European Power Platform Conference in Berlin, 2022.

## Outline Scenario

Duff Corporation is a beer company based in Springfield, USA, and Berlin, Germany. The organisation uses the Power Platform and Dynamics 365, particularly Microsoft Dataverse and Dynamics 365 Sales.

As part of some changes required to these systems, the organisation wants the ability to:
- Implement and call some custom order qualification logic within our corresponding Dynamics 365 Sales environment. Salespeople must have the ability to execute automation that will then create a Power Automate approval to the internal accounts team. If the quote is approved, it should be converted to an Order; otherwise, a new Quote revision should be generated. Approval / Rejection comments should also be saved back to the Quote row.
- Execute basic math formulas against our Dataverse environment

All changes must consider any relevant localization concerns to ensure that colleagues speaking both English and German can work with the solution.

## What's Included

- **D365.CustomAPIDemos**: This solution contains a C# class assembly project, with the core login for both Custom APIs. These are built and deployed to Microsoft Dataverse in the same manner as a standard plug-in; the only difference is that no plug-in steps need to be registered.
- **CustomAPIsDemo_1_0_0_1.zip / CustomAPIsDemo_1_0_0_1_managed**: These Dataverse solution files contain the complete definitions for the above Custom APIs, the corresponding plug-in assemblies, a Power Automate cloud flow that triggers the **duff_AccountsApproval** from a selected Quote row and other miscellaneous configuration for the demo.