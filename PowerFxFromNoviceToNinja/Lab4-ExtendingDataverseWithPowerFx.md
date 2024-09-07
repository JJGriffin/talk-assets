# Lab 4 - Extending Microsoft Dataverse with Power Fx

In this lab, you will implement customizations in Dataverse using Power Fx in modern commanding, formula columns and low-code plug-ins. Finally, you will take the canvas app created in Lab 2 and 3 and embed this into a model-driven Power App.

## Scenario

It has been three months since WingTip Toys have deployed out the canvas app developed for the sellers in Lab 2 and 3. Due to the success of the initial rollout, additional budget has been allocated to extend out the organisations Power Platform environment, and to introduce a new application for the account management team, which will enable them to better manage all of WingTip Toys customers. After a meeting with the account management team, the following requirements have been identified:

- The account management team would like to have a desktop based application that allows them to manage Contacts from their web browsers.
- When the Address details of an Account record is updated, the account management team should have the option to update all the related Contact records with the same address details, by pressing a button on the Account form.
- The account management team were impressed with the age calculation based on a Contacts birthday in the canvas app, and would like the same replicated and persisted in their new application.
- The account management team frequently encounter issues with the telephone numbers provided by the sellers in the field, which causes issues with their click-to-dial solution. They would like a mechanism automatically validate and format the telephone numbers when they are entered into the system to align to the US standard format of **(NPA) NXX-XXXX**.
- To assist the account management team in working with all Contacts for an Account, they would like a mechanism to list and then update related Contact records from the Account form. The account management team are expecting a similar user interface to the canvas app.

Based on the requirements, you have determined that a [model-driven Power App](https://learn.microsoft.com/en-us/power-apps/maker/model-driven-apps/model-driven-app-overview) will be the best solution for the account management and, using the additional capabilities available as part of Power Fx in Dataverse, you plan to leverage the following features to meet the requirements:

- [Modern commanding using Power Fx](https://learn.microsoft.com/en-us/power-apps/maker/model-driven-apps/commanding-use-powerfx)
- [Formula columns](https://learn.microsoft.com/en-us/power-apps/maker/data-platform/formula-columns?tabs=type-or-paste)
- [Low-code plug-ins](https://learn.microsoft.com/en-us/power-apps/maker/data-platform/low-code-plug-ins?tabs=instant)
- [Embedding a canvas app into a model-driven app](https://learn.microsoft.com/en-us/power-apps/maker/model-driven-apps/embedded-canvas-app-add-classic-designer)

## Instructions

In this lab, you will do the following:

- Create a simple model-driven app
- Implement a custom command in Dataverse using Power Fx
- Create a formula column for the Contact table
- Implement a low-code plug-in that will validate and format a Contact's telephone number
- Embed the canvas app created in Lab 2 and 3 into the model-driven app

This lab will take approximately 45 minutes to complete.

> [!IMPORTANT]
> Ensure that all steps have been completed in Lab 3 before proceeding with this lab.

## Exercise 1: Create a Model-Driven App

1. Navigate to the [Power Apps Maker Portal](https://make.powerapps.com) and, if not already selected, navigate to the developer environment you created in Lab 0:
   
    ![](Images/Lab4-ExtendingDataverseWithPowerFx/E1_1.png)

2. Click on **Solutions** in the left-hand navigation pane and then click on the **Wingtip Toys PP Solution** solution you created in Lab 3:
   
    ![](Images/Lab4-ExtendingDataverseWithPowerFx/E1_2.png)

3. Click on **+ New** and then select  **App** -> **Model-driven app**:

    ![](Images/Lab4-ExtendingDataverseWithPowerFx/E1_3.png)

4. In the **New model-driven app** dialog, enter the following details and then select **Create**:
    - **Name**: `Wingtip Toys Account Management`
    - **Description**: `A model-driven app for the account management team to manage Accounts and Contacts`

    ![](Images/Lab4-ExtendingDataverseWithPowerFx/E1_4.png)

5. Once created, you will be presented with the model-driven app designer, which should resemble the screenshot below:

    ![](Images/Lab4-ExtendingDataverseWithPowerFx/E1_5.png)

6. We can now proceed to add the required tables to our solution. Click on **+ New** next to the **Pages** heading and select **Dataverse table**:

    ![](Images/Lab4-ExtendingDataverseWithPowerFx/E1_6.png)

7. In the **Select a table** dialog, search for and select the following tables. ensuring that the **Show in navigation** tickbox is checked. Then, press **Add**:
    - **Account**
    - **Contact**

    ![](Images/Lab4-ExtendingDataverseWithPowerFx/E1_7.png)

    ![](Images/Lab4-ExtendingDataverseWithPowerFx/E1_8.png)

8. Once added, you should see the tables appear in the left-hand navigation pane underneath the **Navigation** heading and the designer view should provide a preview of how the app looks:

    ![](Images/Lab4-ExtendingDataverseWithPowerFx/E1_9.png)

> [!IMPORTANT]
> If you see a message in the preview saying `An error has occurred. Please try again later. If the problem persists, contact your system administrator.`, refresh your browser window to rectify it.

9. Click on the **New Group** group in the **Navigation** area and then, on the properties pane to the right side of the screen, change the **Title** to `Customers`:

    ![](Images/Lab4-ExtendingDataverseWithPowerFx/E1_10.png)

10. Save your changes by clicking on the **Save** button in the top right-hand corner of the screen:

    ![](Images/Lab4-ExtendingDataverseWithPowerFx/E1_11.png)

11. Publish the newly created app by clicking on the **Publish** button. The app may take a few minutes to publish:

    ![](Images/Lab4-ExtendingDataverseWithPowerFx/E1_12.png)

12. Once published, click on the **Play** button to open the app in a new tab:

    ![](Images/Lab4-ExtendingDataverseWithPowerFx/E1_13.png)

> [!IMPORTANT]
> You may be prompted to sign-in again.

13. You should now see the model-driven app you created, with the Account and Contact tables available in the navigation pane. By default, the app will display the **Accounts** table and the **My Active Accounts** view:

    ![](Images/Lab4-ExtendingDataverseWithPowerFx/E1_14.png)

14. Click on any Account record in the **My Active Accounts** view to open the Account form:

    ![](Images/Lab4-ExtendingDataverseWithPowerFx/E1_15.png)

    ![](Images/Lab4-ExtendingDataverseWithPowerFx/E1_16.png)

15. The Account form will display for the selected record. Observe the following regarding the form:
    - The form, similar to the view, has a ribbon bar with a series of different options, such as **Save**, **Delete**, **Run Workflow**, etc. Unlike a canvas app, most common data operations are already available for us in the ribbon and we don't have to define this logic ourselves. We can also extend out the ribbon bar for forms and views with our own custom commands, which we will do in the next exercise.

    ![](Images/Lab4-ExtendingDataverseWithPowerFx/E1_17.png)

    - Currently, the form does not display the Birthday for the Contact record in question. In Exercise 3, we will add this on along with our custom formula column. Forms can customized easily and straightforwardly in Microsoft Dataverse. Keep in mind though, that Dataverse forms and canvas app forms are **NOT** the same.
    - On the form, we can see the **Phone** field has already been added. This field will be targeted as part of the low-code plug-in we will implement in Exercise 4.

    ![](Images/Lab4-ExtendingDataverseWithPowerFx/E1_18.png)

    - The form currently has three tabs displayed - **Summary**, **Details** and **Related**. We can add additional tabs and sections to the form to display further information or to group information together. In Exercise 5, we will add a new tab to this form to display the canvas app we created in Lab 2 and 3.

16. Press the **Back** button to return back to the **My Active Accounts** view:

    ![](Images/Lab4-ExtendingDataverseWithPowerFx/E1_19.png)

17. Continue to experiment with the newly created application by opening further records, making changes and also doing the same with the **Contacts** table. Observe how the experience of working with different tables is similar, providing a streamlined experience for our app users. When you are finished exploring, close the browser tab to return to the model-driven app designer window.

18. Leave the model-driven app designer open as we will be starting from here in the next exercise.

## Exercise 2: Implement a Custom Command in Dataverse using Power Fx

## Exercise 3: Create a Formula Column for the Contact Table

## Exercise 4: Implement a Low-Code Plug-In

## Exercise 5: Embed the Canvas App into the Model-Driven App

**Congratulations, you've finished Lab 4** 🥳