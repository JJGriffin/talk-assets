# Lab 3 - Advanced Development with Power Fx and Canvas Apps

In this lab, you will extend the canvas apps created in Lab 2 with more advanced Power Fx formulas, explore how to more effectively work with data sources in Power Apps and see how to debug a canvas app using the Monitor.

## Scenario

Having successfully created a basic canvas app that allows sales people to view Contact records, and extended the app to include a simple Power Fx formula to calculate the age of each Contact record and to display weather information relating to the Contact's location, Wingtip Toys have asked you to further enhance the app by implementing the following requirements:

- Allow for users to edit Contacts that have been selected from the **Contact Gallery**.
- Write information back to the Contact **Description** field when a Contact is updated. This field should be read-only and not editable by the user.
- Filter the existing Contact screen to only display Contacts that are external. A new field needs to be added to the Contact table, to support this requirement.

You also need to diagnose some issues with the initial version of the app that was launched to the sellers. Sellers have reported that the app is slow to load and that some of the data is not displaying correctly. You plan to use the Monitor to diagnose these issues further.

## Instructions

In this lab, you will do the following:

- Extend the existing canvas app to allow the sellers to update existing Contact records in Dataverse, using the `Patch()` function.
- Add a new field to the Contact table in Dataverse to support the filtering of external Contacts.
- Implement a Power Fx formula to filter the existing Contact screen to only display Contacts that are external.
- Use the Monitor to diagnose and resolve performance issues with the app.

This lab will take approximately 30 minutes to complete.

> [!IMPORTANT]
> Ensure that all steps have been completed in Lab 2 before proceeding with this lab.

## Exercise 1: Extend the Canvas App

1. Navigate to the [Power Apps Maker Portal](https://make.powerapps.com) and, if not already selected, navigate to the developer environment you created in Lab 0:
   
    ![](Images/Lab3-AdvancedPowerFxDevelopmentInCanvasApps/E1_1.png)

2. Click on **Apps** from the left-hand navigation menu, and then click on the edit icon next to the `Lab 2` application created in Lab 2. You may need to hover over the app to see the edit icon:
   
    ![](Images/Lab3-AdvancedPowerFxDevelopmentInCanvasApps/E1_2.png)

3. After a few moments, the canvas app will open in the Power Apps Studio. Click on the **Contact Form** screen in the left-hand **Tree view** menu to open the screen:
   
    ![](Images/Lab3-AdvancedPowerFxDevelopmentInCanvasApps/E1_3.png)

4. Click on the **Insert** tab in the top menu, and then search for and select the **Save** icon to add the control to the screen:
   
    ![](Images/Lab3-AdvancedPowerFxDevelopmentInCanvasApps/E1_4.png)

5. Rename the **Save** button to **Save Contact** by double clicking it in the **Tree view** menu:
   
    ![](Images/Lab3-AdvancedPowerFxDevelopmentInCanvasApps/E1_5.png)

6. With the **Save Contact** icon selected and using the dropdown menu, configure the properties for the control as indicated below:

    | Property | Formula |
    | --- | --- |
    | **Height** | `140` |
    | **Width** | `124` |
    | **X** | `0` |
    | **Y** | `239` |

7. After configuring the **Save Contact** icon, the screen should resemble the below screenshot:

    ![](Images/Lab3-AdvancedPowerFxDevelopmentInCanvasApps/E1_6.png)

8. Configure the **DisplayMode** property of the **Save Contact** icon by using the following formula. This will ensure the icon can only be selected if a change has been made to the form:

    ```
    If(ContactForm.Unsaved = true, DisplayMode.Edit, DisplayMode.Disabled)
    ```

    ![](Images/Lab3-AdvancedPowerFxDevelopmentInCanvasApps/E1_7.png)

9. Configure the **OnSelect** property of the **Save Contact** icon by using the following formula. This formula will update the contact with the latest changes from the form, and add a custom description value:

    ```
    Patch(Contacts, 'Contact Gallery'.Selected, ContactForm.Updates, {Description: Concatenate("Last updated by ", User().FullName)})
    ```

    ![](Images/Lab3-AdvancedPowerFxDevelopmentInCanvasApps/E1_8.png)

> [!IMPORTANT]
> The `Patch()` function can be used interchangeably to either create or update records in a data source. It would also be possible to use the `SubmitForm()` function to achieve a similar resullt, but in this scenario, we want to update an additional field that is not currently part of the form. We will add this field in a read only state shortly.

10. Click on the **Contact Form** screen in the left-hand **Tree view** menu to open the screen, and then click on the **Edit fields** option in the properties pane:
   
    ![](Images/Lab3-AdvancedPowerFxDevelopmentInCanvasApps/E1_9.png)

11. Click on the **Add field** button, select the **Description** field from the list of fields and then click on **Add** to add it to the form. You can use the search box to find the field more easily:
   
    ![](Images/Lab3-AdvancedPowerFxDevelopmentInCanvasApps/E1_10.png)

12. On the list of fields, expand the **Description** field if not already expanded and change the **Control type** to **View text**: 
   
    ![](Images/Lab3-AdvancedPowerFxDevelopmentInCanvasApps/E1_11.png)

13. Close the fields pane by selecting the cross icon in the top right corner of the pane:
   
    ![](Images/Lab3-AdvancedPowerFxDevelopmentInCanvasApps/E1_12.png)

14. The screen should now resemble the below screenshot - the **Save icon** should be disabled and the **Description** data card should no longer have a text input field displayed:

    ![](Images/Lab3-AdvancedPowerFxDevelopmentInCanvasApps/E1_13.png)

15. Test the application by clicking on the **Play** icon in the top right corner of the screen. Make a change to any of the Contact fields and confirm that that the **Save Contact** icon is enabled. Then, click on the **Save Contact** icon. The changes should be saved and the description field should be updated with the text `Last updated by [Your Name]`. The **Save Contact** icon should also be disabled again:

    ![](Images/Lab3-AdvancedPowerFxDevelopmentInCanvasApps/E1_14.png)

    ![](Images/Lab3-AdvancedPowerFxDevelopmentInCanvasApps/E1_15.png)

> [!IMPORTANT]
> The `Patch()` function can provide additional versaility, particularly when there is a need to populate additional hidden fields based on a more complex calculation. However, for most scenarios, remember that the `SubmitForm()` function is the recommended approach for saving data in a form.
>
> In this scenario, we wrote information regarding who last modified the record to the **Description** field. However, keep in mind that Dataverse does have a built in **Modified By** field that can be used to track this information. This field is automatically updated by Dataverse when a record is modified, and may be a more appropriate choice for a production application.

16. Exit the app player by clicking on the **Close** icon in the top right corner of the screen.
17. Click on the **Save** icon to save all changes to the app.
18. Leave the app designer open, as we will be making further changes later in this lab.

## Exercise 2: Customize the Contact Table

## Exercise 3: Implement a Power Fx Formula to Filter Contacts

## Exercise 4: Use the Monitor to Diagnose Performance Issues