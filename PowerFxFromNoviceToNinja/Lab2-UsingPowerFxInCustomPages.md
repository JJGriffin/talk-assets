# Lab 2 - Using Power Fx in Custom Pages

In this lab, you will create a basic custom page that connects to Microsoft Dataverse, to allow users to view, edit and create Contact records. You will then extend the page to include a simple Power Fx formula to calculate the age of each Contact record and to display weather information relating to the Contact's location, via a Power Automate cloud flow.

## Scenario

Wingtip Toys require a simple application that allows sales people to view, edit and create Contact records. The app should also display the age of each Contact record and the current weather information for the Contact's location, to assist the sellers in conducting more personalised conversations with their customers.

Having spent some time familiarising with the fundamental capabilities of Power Fx and Power Apps, you have been tasked with creating a simple [custom page](https://learn.microsoft.com/en-us/power-apps/maker/model-driven-apps/model-app-page-overview) that allows the sellers to work with the Contact records, and support the additional requirements relating to the age of the Contacts and the weather information for each individual.

## Instructions

In this lab, you will do the following:

- Create a simple two screen custom page, with navigation between the screens.
- Customise each page screen to display a list of all Contacts and a form level view for a single Contact.
- Add several Power Fx formulas for navigation and to calculate the age of each Contact record.
- Add a Power Automate cloud flow to retrieve the weather information for the Contact's location.
- Review and implement recommendations from the Power Apps app checker.
- Extend the existing custom page to allow the sellers to update existing Contact records in Dataverse, using the `Patch()` function.
- Add a new field to the Contact table in Dataverse to support the filtering of external Contacts.
- Implement a Power Fx formula to filter the existing Contact screen to only display Contacts that are external.
- Import additional data and experiment with the app to understand the impact of delegation and query limits.

This lab will take approximately 60 minutes to complete.

> [!IMPORTANT]
> Ensure that all steps have been completed in Lab 0 before proceeding with this lab.

## Exercise 1: Create a Custom Page

1. Navigate to the [Power Apps Maker Portal](https://make.powerapps.com) and, if not already selected, navigate to the developer environment you created in Lab 0:
   
    ![](Images/Lab2-UsingPowerFxInCustomPages/E1_1.png)

2. Click on **Solutions** from the left-hand navigation menu and then click on the **Wingtip Toys PP Solution** solution you created in Lab 0:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E1_2.png)

3. In the solution view, click on **+ New**, select **App** and then click **Page**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E1_3.png)

4. After a few moments, the canvas designer will open:
   
    ![](Images/Lab2-UsingPowerFxInCustomPages/E1_4.png)

5. By default, a custom page will only support a single screen. Click on the **Settings** icon in the top right corner of the screen:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E1_5.png)

6. Click on **Display** in the dialog and then toggle the option for **Enable multiple screens** to **On**. Click on **Close** to apply the change:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E1_6.png)

7. In the canvas designer, click on the **Screen1** label in the left-hand tree view, and rename the screen to **Contact Screen**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E1_7.png)

> [!IMPORTANT]
> You can also double click any control or screen property to rename it.

8. Add a new screen to the page by clicking on the **+ New Screen** button and selecting the **Blank** screen template:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E1_8.png)

9. Rename the new screen to **Contact Form**. Your page navigation view should resemble the below if done correctly:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E1_9.png)

10. We will now add in the **Contact** table from Microsoft Dataverse as a data source. Click on the **Data** tab in the left-hand menu, and then click on **+ Add data**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E1_10.png)

11. A list of tables from the current environment will display, with the **Contacts** table option visible. Select it if so; otherwise, search for and then select the table from the list:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E1_11.png)

12. After a few moments, the **Data** pane will refresh and display the **Contacts** table:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E1_12.png)

13. On the top right of the designer view, click on the **Save** icon:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E1_13.png)

14. In the **Save as** dialog, enter a name value of `Lab 2`, followed by your initials, and then click on **Save**:
   
    ![](Images/Lab2-UsingPowerFxInCustomPages/E1_14.png)

15. Leave the custom page open, as you will continue working with it in the next exercise.

## Exercise 2: Design the Custom Page

1. In the `Lab 2` custom page, ensure that the **Tree view** icon is selected:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_1.png)

2. With the **Contact Screen** screen selected, on the ribbon, click on the **Insert** tab, and then click on **Vertical gallery**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_2.png)

3. Rename the newly inserted **Gallery1** control to **Contact Gallery**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_3.png)

> [!IMPORTANT]
> Although the process of renaming screens and controls may seem tedious, having useful and descriptive names will help when writing formulas later in the lab.

4. Select the **Contact Gallery** control, and with the **Items** property selected in the dropdown, change the value to `Contacts`:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_4.png)

5. Repeat the same steps in 4, but this time, select and configure the following properties as indicated in the table below. Once configured correctly, the page screen should resemble the screenshot below:

    | Property | Formula |
    | --- | --- |
    | **Height** | `680` |
    | **Width** | `1366` |
    | **X** | `0` |
    | **Y** | `88` |

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_5.png)

> [!IMPORTANT]
> If you see no sample data in the gallery, then you may need to install this into your environment. Select the gear icon in the top right corner of the screen, and then click on **Advanced settings**. Next, click the chevron next to **Settings** and then select **Data Management**. On the **Data Management** screen, click on **Sample data** and then click on the **Install sample data** button. Installation may take a few minutes to complete. You can return to the canvas app and refresh the **Contacts** data source to see the sample data in the gallery. For further details on installing sample data, refer to the [Microsoft Learn article](https://learn.microsoft.com/en-us/power-platform/admin/add-remove-sample-data).

![](Images/Lab2-UsingPowerFxInCustomPages/E2_6.png)

![](Images/Lab2-UsingPowerFxInCustomPages/E2_7.png)

![](Images/Lab2-UsingPowerFxInCustomPages/E2_8.png)

![](Images/Lab2-UsingPowerFxInCustomPages/E2_9.png)

6. On the ribbon, click on the **Insert** tab, and then click on **Rectangle**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_10.png)

7. Rename the newly inserted **Rectangle1** control to **Gallery Header**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_11.png)

8. Select the **Gallery Header** control, and with the **Fill** property selected in the dropdown, change the value to `RGBA(250, 155, 112, 1)`:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_12.png)

9. Repeat the same steps in 8, but this time, select and configure the following properties as indicated in the table below. Once configured correctly, the app screen should resemble the screenshot below:

    | Property | Formula |
    | --- | --- |
    | **Height** | `88` |
    | **Width** | `1366` |
    | **X** | `0` |
    | **Y** | `0` |

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_13.png)

10. On the **Gallery Header** control, click on the **Insert** tab, and then click on **Label**. You may need to expand the **Display** heading:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_14.png)

11. Rename the newly inserted **Label1** control to **Header Label**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_15.png)

12. Select the **Header Label** control, and with the **Text** property selected in the dropdown, change the value to `Wingtip Toys Contacts`:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_16.png)

13. Repeat the same steps in 12, but this time, select and configure the following properties as indicated in the table below. Once configured correctly, the app screen should resemble the screenshot below:

    | Property | Formula |
    | --- | --- |
    | **Alignment** | `Align.Center` |
    | **Color** | `RGBA(255, 255, 255, 1)` |
    | **FontWeight** | `FontWeight.Bold` |
    | **Height** | `60` |
    | **FontSize** | `24` |
    | **Width** | `380` |
    | **X** | `493` |
    | **Y** | `14` |

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_17.png)

14. With the **Tree view** icon selected, click on the **Contact Form** screen in the tree view:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_18.png)

15. On the **Contact Form** screen, click on the **Insert** tab, and then click on **Edit form**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_19.png)

> [!IMPORTANT]
> You may need to scroll down to see the **Edit form** option.

16. Rename the newly inserted **Form1** control to **ContactForm**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_20.png)

17. Select the **ContactForm** control, and with the **DataSource** property selected in the dropdown, change the value to `Contacts`:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_21.png)

18. Repeat the same steps in 17, but this time, select and configure the following properties as indicated in the table below. Once configured correctly, the app screen should resemble the screenshot below:

    | Property | Formula |
    | --- | --- |
    | **BorderThickness** | `2` |
    | **Height** | `679` |
    | **Item** | `'Contact Gallery'.Selected` |
    | **Width** | `950` |
    | **X** | `124` |
    | **Y** | `89` |

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_22.png)

19. With the **ContactForm** control selected, select the **Edit fields** button in the properties pane:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_23.png)

> [!IMPORTANT]
> In all of the previous lab steps, we could have used the properties pane to modify font size, color, height, width and a range of other properties. Use whichever experience you prefer when building your own apps.

20. In the **Edit fields** pane, click on the **Add field** button:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_24.png)

21. In the **Add a field** dialog, search for and tick the box next to the **Full Name** field:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_25.png)

22. Repeat step 21, but this time, search and tick the box for the following fields. If any other fields are ticked, make sure they are unticked. Once all required fields are ticked, click on the **Add** button:
    
    - **Last Name**
    - **Email**
    - **Birthday**
    - **Business Phone**
    - **Address 1**

23. After a few moments, the form control should refresh itself and resemble the below:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_26.png)

24. Navigate back to the **Contact Screen** screen in the tree view.

25. With the `CTRL` key held down, select the following controls in the tree view:
    - **Header Label**
    - **Gallery Header**

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_27.png)

26. In the ribbon, select the **Copy** option:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_28.png)

27. Navigate to the **Contact Form** screen in the tree view and then click on the **Paste** icon. The shape and label will be copied into the correct position, as indicated below:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_29.png)

28. Rename the newly pasted **Header Label_1** control to **Form Label** and **Gallery Header_1** control to **Form Header**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_30.png)

29. Adjust the **Text** property of the **Form Label** control to `Contact Details`:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_31.png)

30. On the **Contact Form** screen, click on the **Insert** tab, and then search for and select the **Back arrow** control:

31. Rename the newly inserted **Arrow1** control to **Back Arrow**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_33.png)

32. Select the **Back Arrow** control, and with the **Height** property selected in the dropdown, change the value to `150`:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_34.png)

33. Repeat the same steps in 32, but this time, select and configure the following properties as indicated in the table below. Once configured correctly, the app screen should resemble the screenshot below:

    | Property | Formula |
    | --- | --- |
    | **Width** | `124` |
    | **X** | `0` |
    | **Y** | `89` |

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_35.png)

34. On the **Contact Form** screen, click on the **Insert** tab, and then search for and select the **Button** control:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_36.png)

35. Rename the newly inserted **Button1** control to **Get Weather**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_37.png)

36. Select the **Get Weather** control, and with the **Text** property selected in the dropdown, change the value to `"Get Weather"`:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_38.png)

37. Repeat the same steps in 36, but this time, select and configure the following properties as indicated in the table below. Once configured correctly, the app screen should resemble the screenshot below:

    | Property | Formula |
    | --- | --- |
    | **X** | `1144` |
    | **Y** | `408` |

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_39.png)

38. On the **Contact Form** screen, click on the **Insert** tab, and then search for and select the **Label** control:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_40.png)

39. Rename the newly inserted **Label1** control to **Weather Label**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_41.png)

40. Select the **Weather Label** control, and with the **Text** property selected in the dropdown, change the value to `"REPLACEME"`:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_42.png)

41. Repeat the same steps in 40, but this time, select and configure the following properties as indicated in the table below. Once configured correctly, the app screen should resemble the screenshot below:

    | Property | Formula |
    | --- | --- |
    | **Alignment** | `Align.Center` |
    | **FontWeight** | `FontWeight.Bold` |
    | **Height** | `260` |
    | **Width** | `250` |
    | **X** | `1099` |
    | **Y** | `118` |

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_43.png)

42. Save all changes to the custom page by clicking on the **Save** icon in the top right corner of the screen.

    ![](Images/Lab2-UsingPowerFxInCustomPages/E2_44.png)

We now have a basic custom page that allows us to view Contact record data and drill into a single Contact record. In the next exercises, we will add the necessary Power Fx formulas to navigate between screens, calculate and display the age of a Contact and allow the user to dynamically display weather information for the Contact's location.

## Exercise 3: Implement Power Fx Formulas

> [!IMPORTANT]
> This exercise assumes that you have completed the previous exercises and have the `Lab 2` custom page open. If you are not there currently, navigate to the app now.

To ensure the page is functional, we need to implement the following using Power Fx:

- When a user selects a Contact from the **Contact Gallery** gallery, they should be taken to the **Contact Form** screen, with the details of the selected Contact displayed.
- When a user selects the **Back Arrow** control, they should be taken back to the **Contact Screen** screen.
- The **Contact Form** should display a calculated field for the age of the Contact, derived from the **Birthday** field.

Let's proceed to implement the functionality in the order described above, and then test the page to ensure it works as expected.

1. Navigate to the **Contact Screen** screen in the tree view.
2. Select the **Contact Gallery** control and then click on the pencil icon to edit the gallery template:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E3_1.png)

3. The first row in the gallery will now be selected, allowing you to add additional controls. Click on **Insert** and add the **Right** icon:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E3_2.png)

4. Rename the newly inserted control to **NextArrow1**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E3_3.png)

5. With the **NextArrow1** control selected, ensure that the **OnSelect** property is selected in the formula dropdown and adjust the formula value as follows:

    ```
    Select(Parent); Navigate('Contact Form',ScreenTransition.Fade)
    ```
    ![](Images/Lab2-UsingPowerFxInCustomPages/E3_4.png)

> [!IMPORTANT]
> It's also possible to use `Navigate('Contact Form')` as the formula here, omitting the screen transition argument. This argument is used to provide a visual effect when transitioning between screens, with a few options available. For further details, [review the Microsoft Learn article on the topic.](https://docs.microsoft.com/en-us/powerapps/maker/canvas-apps/functions/function-navigate).

6. Repeat the same steps in 5, but this time, select and configure the following properties as indicated in the table below. Once configured correctly, the app screen should resemble the screenshot below:

    | Property | Formula |
    | --- | --- |
    | **X** | `1326` |
    | **Y** | `8` |

    ![](Images/Lab2-UsingPowerFxInCustomPages/E3_5.png)

7. Navigate to the **Contact Form** screen in the tree view.
8. Select the **Back Arrow** control, and with the **OnSelect** property selected in the formula dropdown, adjust the formula value as follows:

    ```
    Back()
    ```
    ![](Images/Lab2-UsingPowerFxInCustomPages/E3_6.png)

> [!IMPORTANT]
> `Back()` is a shorthand function that can be used to return the user to the previous screen they were on. In this context, it would also be valid to use `Navigate('Contact Screen')` to achieve the same result.

9. With the **ContactForm** control selected, click on the **6 selected** button in the properties pane:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E3_7.png)

10. On the **Fields** pane, click on the elipses (`...`) and then select **Add a custom card**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E3_8.png)

11. A new custom data card called **DataCard1** will be added to the form:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E3_9.png)

12. Click on the text **Add an item from the insert pane** and then select **Label** from the **Insert** pane:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E3_10.png)

13. A text label control will be added to the custom data card. Rename this control to **Age Label**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E3_11.png)

14. Select the **Age Label** control, and with the **Text** property selected in the formula dropdown, adjust the formula value as follows:

    ```
    "Contact Age: " & RoundDown((Today() - ThisItem.Birthday) / 365.25, 0)
    ```

    ![](Images/Lab2-UsingPowerFxInCustomPages/E3_12.png)

15. Repeat the same steps in 14, but this time, select and configure the following properties as indicated in the table below. Once configured correctly, the app screen should resemble the screenshot below:

    | Property | Formula |
    | --- | --- |
    | **FontWeight** | `FontWeight.Bold` |
    | **Height** | `130` |
    | **FontSize** | `30` |
    | **Width** | `255` |
    | **X** | `28` |
    | **Y** | `32` |

    ![](Images/Lab2-UsingPowerFxInCustomPages/E3_13.png)

16. Navigate back to the **Contact Screen** screen in the tree view.

17. Save all changes to the custom page by clicking on the **Save** icon in the top right corner of the screen.

    ![](Images/Lab2-UsingPowerFxInCustomPages/E3_14.png)

18. We will now proceed to test the page. Click on the **Play** icon in the top right corner of the screen to launch the page in preview mode:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E3_15.png)

19. Validate that the page functions as follows. You can press the cross icon in the top right of the player to return back to the designer view at any time:
    - When you select a Contact from the **Contact Gallery** gallery, you are taken to the **Contact Form** screen, with the details of the selected Contact displayed.
    - When you select the **Back Arrow** control, you are taken back to the **Contact Screen** screen.
    - The **Contact Form** displays a calculated field for the age of the Contact, derived from the **Birthday** field. Verify that the age is correct compared to the birthday.

20. Leave the page open, as you will continue working with it in the next exercise.

## Exercise 4: Add a Power Automate cloud flow to retrieve weather information

> [!IMPORTANT]
> This exercise assumes that you have completed the previous exercises and have the `Lab 2` custom page open in the Power Apps studio. If you are not there currently, navigate to the app now.

To display weather information for the Contact's location, we will create a Power Automate cloud flow that will retrieve the weather information using [the MSN Weather connector](https://learn.microsoft.com/en-us/connectors/msnweather/). We will then call this flow from the custom page using the button we added to the page in Exercise 3 to display the weather information.

Note that it would be possible to add the MSN Weather connector directly into the app, and then use this to achieve the same outcome. We will use Power Automate in this instance to demonstrate the integration capabilities between different areas of the Power Platform, and so you can familliarize yourself with the approaches to follow when calling a cloud flow from a canvas app.

1. In the `Lab 2` custom page, navigate to the **Contact Screen** screen in the tree view.
2. On the right hand-side of the screen, select the **Power Automate** icon

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_1.png)

3. In the **Power Automate** pane, click on the **Create new flow** button:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_2.png)

4. In the **Create your flow** dialog, click on the **+ Create from blank** button:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_3.png)

5. The flow designer screen will open. Click on the **Power Apps V2** trigger to expand it:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_4.png)

6. Click on the **+ Add an input** button, and then select the **Text** input type:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_5.png)

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_6.png)

7. Rename the new input to **Location**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_7.png)

8. Click on **+ New step** and then search for the **MSN Weather** connector. Under **Actions**, select the **Get forecast for today** action:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_8.png)

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_9.png)

9. In the **Get forecast for today** action, select the **Location** input and set the value to the **Location** input from the Power Apps trigger:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_10.png)

10. Repeat the same steps in 8, but this time, search for **Power Apps** and select the **Respond to Power Apps or Flow** action:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_11.png)

11. In the **Respond to Power Apps or Flow** action, select **+ Add an Output** and then select **Text**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_12.png)

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_13.png)

12. In the **Enter title** input, enter `Forecast` and in the **Enter a value to respond** input, set the value to the **Day Summary** output from the **Get forecast for today** action:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_14.png)

13. You have now configured the flow. If setup correctly, it should resemble the screenshot below:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_15.png)

14. Rename the new cloud flow to `GetWeatherForLocation`, by clicking on **Untitled flow** and updating the input accordingly:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_16.png)

15. Click on the **Save** icon in the top right corner of the screen to save the flow:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_17.png)

16. Close the flow designer by clicking on the **Close** icon in the top right corner of the screen. The flow will then be automatically added to the page and made visible in the **Power Automate** pane:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_18.png)

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_19.png)

17. Click on the **Get Weather** button in the **Contact Form** screen, and with the **OnSelect** property selected in the formula dropdown, adjust the formula value as follows:

    ```
    Set(varWeatherForecast, GetWeatherForLocation.Run(Concatenate('Contact Gallery'.Selected.'Address 1: Street 1', ",", 'Contact Gallery'.Selected.'Address 1: City', ",", 'Contact Gallery'.Selected.'Address 1: ZIP/Postal Code', ",", 'Contact Gallery'.Selected.'Address 1: Country/Region')).forecast)
    ```

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_20.png)

18. Select the **Weather Label** control, and with the **Text** property selected in the formula dropdown, adjust the formula value as follows:

    ```
    varWeatherForecast
    ```

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_21.png)

19. Save all changes to the custom page by clicking on the **Save** icon in the top right corner of the screen.
20. Click on the **Play** icon in the top right corner of the screen to launch the page in preview mode
21. Click on the **Get Weather** button to trigger the cloud flow. After a few moments, the weather information for the Contact's location should display in the **Weather Label** control:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E4_22.png)

22. You can (optionally) experiment further by going back to the gallery screen, selecting different Contacts and then generating the unique forecast for each Contact's location.
23. Leave the page open, as you will continue working with it in the next exercise.

## Exercise 5: Review and implement recommendations from the Power Apps App Checker

> [!IMPORTANT]
> This exercise assumes that you have completed the previous exercises and have the `Lab 2` canvas app open in the Power Apps studio. If you are not there currently, navigate to the app now.

Similar to a canvas app, custom pages provides an [App Checker tool](https://learn.microsoft.com/en-us/power-apps/maker/canvas-apps/accessibility-checker) that can help you identify and resolve potential issues. These issues can range from errors, through to accessibility concerns to performance improvements. Ensuring that you address accesibility issues is important, as it will ensure that all users can interact with your apps and pages effectively.

In this exercise, you will run the App Checker tool against the `Lab 2` custom page, review the recommendations and then implement any necessary changes. We will also simulate an issue in the application, to see how the App Checker tool can be used to identify and resolve errors.

1. In the `Lab 2` custom page, navigate to the **Contact Form** screen in the tree view, select the **Get Weather** button and adjust the **OnSelect** property formula as follows:

    ```
    Set(varWeatherForecasts, GetWeatherForLocation.Run(Concatenate('Contact Gallery'.Selected.'Error Simulation', ",", 'Contact Gallery'.Selected.'Address 1: City', ",", 'Contact Gallery'.Selected.'Address 1: ZIP/Postal Code', ",", 'Contact Gallery'.Selected.'Address 1: Country/Region')))
    ```

    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_1.png)

2. Notice that Intellisense will highlight the formula as having an error, and the App Checker icon will also display a red dot in the top right corner of the screen. The individual control(s) will also display a red cross icon to indicate an error:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_2.png)

> [!IMPORTANT]
> The App Checker tool will always display a red dot if there are any formula errors present. However, just because a red dot isn't visible, doesn't mean that there are no issues. It's always a good idea to run the App Checker tool after creating the first version of your app or custom page, and then periodically throughout the development process.

3. Click on the **App Checker** icon in the top right corner of the screen. The App Checker pane will open on the right-hand side of the screen:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_3.png)

4. Expand the **Formulas** heading. Notice that the list of errors are grouped by screen, and then by the individual control property that contains the error:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_4.png)

5. Click the arrow next to each of the errors to inspect the details further. Notice that you are provided with an outline description of the issue, a precise location of where the error is occurring, suggestions on how to fix and a link to any relevant documentation. Click on the back arrow next to the **Details** to return back to the previous pane:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_5.png)

    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_6.png)

6. Once you have finished inspecting all errors, return back to the App Checker pane by clicking the back arrow next to the **Formulas** heading and then expand the **Performance** heading:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_7.png)

    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_8.png)

7. The **Performance** pane should display a single warning relating to an unusued variable. Click on the arrow next to the warning to inspect the details further. When you are finished, click on the back arrow next to the **Details** to return back to the previous pane, and then the back arrow next to the **Performance** heading to return back to the main App Checker pane.

> [!IMPORTANT]
> Variables that are referenced or initialised, but not used anywhere in the app, can lead to performance issues and confusion to other developers working on the app. You should regularly review and remove any unnecessary variables from your app.

![](Images/Lab2-UsingPowerFxInCustomPages/E5_9.png)

8. Fix the issue with the **Get Weather** button by reverting the **OnSelect** property formula back to it's original value:

    ```
    Set(varWeatherForecast, GetWeatherForLocation.Run(Concatenate('Contact Gallery'.Selected.'Address 1: Street 1', ",", 'Contact Gallery'.Selected.'Address 1: City', ",", 'Contact Gallery'.Selected.'Address 1: ZIP/Postal Code', ",", 'Contact Gallery'.Selected.'Address 1: Country/Region')).forecast)
    ```

9. Notice that the red icon in the top right corner of the screen will disappear, and the individual control(s) will no longer display a red cross icon:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_10.png)

10. In the **App Checker** pane, expand the **Accessibility** heading:
    
    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_11.png)

11. Notice that there are several accessibility errors displayed, relating to missing accessibility labels and missing tab stops:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_12.png)

> [!IMPORTANT]
> Accessibility labels are relied upon for screen readers, to help clearly explain the purpose for a control to users who may have visual impairments. Tab stops are used to help users navigate through the app using the keyboard, and are important for users who may have mobility impairments. Tab order can also be useful for users who prefer to use keyboard shortcuts to navigate through their apps.

12. Click on the arrow next to the accessible label error for the **Contact Gallery** to inspect the details further. Notice that you are provided with an outline description of the issue, a precise location of where the error is occurring, suggestions on how to fix and a link to any relevant documentation. In addition, the relevant property is selected in the tree view and formula bar:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_13.png)

    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_14.png)

13. Correct the issue with the accessible label by providing a suitable description in the formula bar. For example, `"Gallery for displaying a list of Contacts from Dataverse"`:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_15.png)

14. Repeat the same steps in 12 and 13 for the remaining accessibility label errors. For each label, type in an appropriate description value that accurately describe what the control is doing. Once all errors are resolved and the **Recheck** option is selected on the **Accessibility** pane, all errors should be resolved:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_16.png)

15. In the **Accessibility** pane, select the missing tab stop error for the **Contact Gallery** control. Notice that the **TabIndex** property of the control is selected in the formula bar, and that it's value is set to `-1`:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_17.png)

16. Adjust the **TabIndex** property value to `1` and then click on **Recheck**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_18.png)

17. Notice that although the error for the **Control Gallery** missing tab stop has disappeared, a new tip is displayed regarding the order of screen items:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_19.png)

> [!IMPORTANT]
> The precise order you indicate via the **TabIndex** property will have an important impact on how users interact with the app, and their general experience. In a real-world scenario, you would need to pay special attention to the order you would like to set for each control, and ensure that it makes sense for the user. For the purposes of this exercise, we will ignore the tip.

18. Repeat steps 15 and 16 for the remaining missing tab stop errors, providing an appropriate value for each control (e.g. `1` or `2`). Once all errors are resolved and the **Recheck** option is selected on the **Accessibility** pane, all errors should be resolved, with only the screen order tips remaining:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_20.png)

19. Save all changes to the custom page by clicking on the **Save** icon in the top right corner of the screen.
20. Click on the **Publish** icon in the top right corner of the screen to publish the app. In the **Publish** dialog, click on the **Publish this version** button:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E5_21.png)

21. The app will automatically publish itself in the background. You may now close the app by clicking on the **Back** icon in the top left of the screen.
22. Leave the custom page designer open if you plan to continue to the next exercise.

## Exercise 6: Extend the Custom Page

1. You should still have the `Lab 2` custom page open in the canvas designer. If not, navigate there now and ensure the **Contact Form** screen is selected in the tree view.

    ![](Images/Lab2-UsingPowerFxInCustomPages/E6_1.png)

2. Click on the **Insert** tab in the top menu, and then search for and select the **Save** icon to add the control to the screen:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E6_2.png)

3. Rename the **Save** button to **Save Contact** by double clicking it in the **Tree view** menu:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E6_3.png)

4. With the **Save Contact** icon selected and using the dropdown menu, configure the properties for the control as indicated below:

    | Property | Formula |
    | --- | --- |
    | **Height** | `140` |
    | **Width** | `124` |
    | **X** | `0` |
    | **Y** | `239` |

5. After configuring the **Save Contact** icon, the screen should resemble the below screenshot:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E6_4.png)

6. Configure the **DisplayMode** property of the **Save Contact** icon by using the following formula. This will ensure the icon can only be selected if a change has been made to the form:

    ```
    If(ContactForm.Unsaved = true, DisplayMode.Edit, DisplayMode.Disabled)
    ```

    ![](Images/Lab2-UsingPowerFxInCustomPages/E6_5.png)

7. Configure the **OnSelect** property of the **Save Contact** icon by using the following formula. This formula will update the contact with the latest changes from the form, and add a custom description value:

    ```
    Patch(Contacts, 'Contact Gallery'.Selected, ContactForm.Updates, {Description: Concatenate("Last updated by ", User().FullName)})
    ```

    ![](Images/Lab2-UsingPowerFxInCustomPages/E6_6.png)

> [!IMPORTANT]
> The `Patch()` function can be used interchangeably to either create or update records in a data source. It would also be possible to use the `SubmitForm()` function to achieve a similar resullt, but in this scenario, we want to update an additional field that is not currently part of the form. We will add this field in a read only state shortly.

8. Click on the **Contact Form** screen in the left-hand **Tree view** menu to open the screen, and then click on the **Edit fields** option in the properties pane:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E6_7.png)

9. Click on the **Add field** button, select the **Description** field from the list of fields and then click on **Add** to add it to the form. You can use the search box to find the field more easily:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E6_8.png)

10. On the list of fields, expand the **Description** field if not already expanded and change the **Control type** to **View text**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E6_9.png)

11. Close the fields pane by selecting the cross icon in the top right corner of the pane:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E6_10.png)

12. The screen should now resemble the below screenshot - the **Save icon** should be disabled and the **Description** data card should no longer have a text input field displayed:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E6_11.png)

13. Test the application by clicking on the **Play** icon in the top right corner of the screen. Make a change to any of the Contact fields and confirm that that the **Save Contact** icon is enabled. Then, click on the **Save Contact** icon. The changes should be saved and the description field should be updated with the text `Last updated by [Your Name]`. The **Save Contact** icon should also be disabled again:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E6_12.png)

    ![](Images/Lab2-UsingPowerFxInCustomPages/E6_13.png)

> [!IMPORTANT]
> The `Patch()` function can provide additional versaility, particularly when there is a need to populate additional hidden fields based on a more complex calculation. However, for most scenarios, remember that the `SubmitForm()` function is the recommended approach for saving data in a form.
>
> In this scenario, we wrote information regarding who last modified the record to the **Description** field. However, keep in mind that Dataverse does have a built in **Modified By** field that can be used to track this information. This field is automatically updated by Dataverse when a record is modified, and may be a more appropriate choice for a production application.

14. Exit the app player by clicking on the **Close** icon in the top right corner of the screen.
15. Click on the **Save** icon to save all changes to the custom page.
16. Click on the **Back** button to exit the canvas designer. We will return to the app later in the lab.

## Exercise 7: Customize the Contact Table

> [!IMPORTANT]
> This exercise assumes that you have completed the previous exercises. Make sure you have completed all steps described above, including closing the canvas app designer.

1. Open a new browser tab and navigate to the [Power Apps Maker Portal](https://make.powerapps.com).
2. In the **Power Apps Maker Portal**, click on **Solutions** from the left-hand navigation menu:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E7_1.png)

3. On the **Solutions** page, click on the **Wingtip Toys PP Solution** solution created in Lab 0:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E7_2.png)

4. Verify that the **Lab 2** custom page, the **GetWeatherForLocation** cloud flow and a corresponding connection reference you created earlier is present in the solution; this is because we configured a default solution for our environment in Lab 0:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E7_3.png)

> [!IMPORTANT]
> Connection references are used to ensure different connection profiles can be defined for our apps and automations as we move them between different environments. For example, if our app was using a SQL Server database and we have different servers/databases for our live and testing environments, the connection reference would enable us to define these seperately. For more information on connection references, [consult the Microsoft Learn site](https://learn.microsoft.com/en-us/power-apps/maker/data-platform/create-connection-reference).

5. We will now customize the Contact table to add the new field. Click on **Add existing** again and then select **Table**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E7_4.png)

6. On the **Add existing tables** screen, scroll down to select the **Contact** table and then click on **Next**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E7_5.png)

7. On the **Selected tables** screen, click on **Add**. Do **NOT** tick the boxes for **Include all objects** or **Include table metadata**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E7_6.png)

> [!IMPORTANT]
> The **Include all objects** and **Include table metadata** options are used to include all fields and metadata for the table. In this scenario, to avoid solution "bloat" and because we are customizing a table that forms part of the Common Data Model, adding in the entire table could cause issues with deploying it out in future.

8. The **Contact** table should now be added and visible in the solution:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E7_7.png)

9. On the **Objects** list, expand **Tables**, then **Contact** and then click on **Columns**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E7_8.png)

10. On the **Columns** view, click on **+ New column**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E7_9.png)

11. In the **New column** pane, enter the following details and then click on **Save**:

- **Display name**: `External Contact?`
- **Description**: `Indicates the type of Contact`
- **Data type**: Select `Choice` -> `Choice`
- **Sync with global choice?**: Select `No`
    - **Choices**: Add the following choices:
        - First Choice:
            - **Label**: `Internal`
            - **Value**: `962950000`
        - Second Choice:
            - **Label**: `External`
            - **Value**: `962950001`
- **Default choice**: Select `None`
- **Schema name**: `wtt_contacttype`

    ![](Images/Lab2-UsingPowerFxInCustomPages/E7_10.png)

    ![](Images/Lab2-UsingPowerFxInCustomPages/E7_11.png)

> [!IMPORTANT]
> You may need to expand the **Advanced options** heading to view all configuration properties.

12. The new column should now be visible in the **Columns** view:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E7_12.png)

13. Click on **All** to return an overview of the solution:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E7_13.png)

14. Leave the solution view and maker portal open, as we will continue from here in the next exercise.

## Exercise 8: Add External Contact and Filter to the Canvas App

> [!IMPORTANT]
> Ensure that all steps in Exercise 7 have been completed before proceeding.

1. You should still be in the solution view for the **Wingtip Toys PP Solution** created in the previous exercise; if not, navigate there now.
2. Click on the `Lab 2` custom page to open it in the designer:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_1.png)

3. In the **Tree view**, expand the **Contact Form** screen and then select the **ContactForm** control:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_2.png)

4. In the properties pane, click on **8 selected** next to the **Fields** label:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_3.png)

5. In the **Fields** pane, click on **Add field** and then select the **External Contact?** field from the list of fields. You can use the search box to more easily locate it. Once selected, click on **Add** to add the field to the form:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_4.png)

6. Click on the cross icon to close the **Fields** pane. The **External Contact?** field should now be visible in the form, as a dropdown control:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_5.png)

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_6.png)

7. Click on the **Contact Screen** screen in the **Tree view** and then play the custom page:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_7.png)

8. Navigate into the first Contact record and then populate the **External Contact?** field with the value `External`. Save the record and then navigate back to the Contact screen:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_8.png)

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_9.png)

9. Repeat step 8 for all remaining Contacts in the gallery. For each Contact, alternate between the `Internal` and `External` values for the **External Contact?** field, ensuring an even split between the two values:
10. Exit the player by clicking on the **Close** icon in the top right corner of the screen.

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_10.png)

11. Select the **Contact Gallery** gallery in the **Tree view**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_11.png)

12. Adjust the properties of the gallery as indicated in the table below. The gallery should resemble the screenshot below if configured correctly:

    | Property | Formula |
    | --- | --- |
    | **Width** | `1150` |
    | **X** | `216` |

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_12.png)

13. Click on the **+ Insert** button in the top menu, and then select **Radio group**. You may need to expand the **Input** heading to locate the control:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_13.png)

14. Rename the **Radio** control to **Contact Filter** by double clicking it in the **Tree view** menu:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_14.png)

15. Adjust the properties of the **Contact Filter** control as indicated in the table below. The screen should resemble the screenshot below if configured correctly:

    | Property | Formula |
    | --- | --- |
    | **Height** | `154` |
    | **Items** | `["All", "Internal", "External"]` |
    | **Width** | `160` |
    | **X** | `40` |
    | **Y** | `185` |

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_15.png)

16. Select the **Contact Gallery** control and adjust the **Items** property to filter the gallery based on the selected value in the **Contact Filter** control. Use the following formula:

    ```
    Switch('Contact Filter'.Selected.Value, 
        "All", Contacts,
        "Internal", Filter(Contacts, 'External Contact?' = 'External Contact? (Contacts)'.Internal),
        "External", Filter(Contacts, 'External Contact?' = 'External Contact? (Contacts)'.External)
    )
    ```

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_16.png)

15. Press the **Play** icon in the top right corner of the screen to test the custom page. Select the different options from the **Contact Filter** control and confirm that the list of Contacts in the gallery updates accordingly. Close the player when you are finished testing:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_17.png)

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_18.png)

16. Currently, the gallery is returning the data in an inconsistent order. Adjust the formula for the **Items** property on the **Contact Gallery** to include the `Sort()` function. Use the following formula:

    ```
    Switch('Contact Filter'.Selected.Value, 
        "All", Sort(Contacts, 'Full Name', SortOrder.Ascending),
        "Internal", Sort(Filter(Contacts, 'External Contact?' = 'External Contact? (Contacts)'.Internal), 'Full Name', SortOrder.Ascending),
        "External", Sort(Filter(Contacts, 'External Contact?' = 'External Contact? (Contacts)'.External), 'Full Name', SortOrder.Ascending)
    )
    ```

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_19.png)

17. Press the **Play** icon in the top right corner of the screen to test the custom page. Select the different options from the **Contact Filter** control and confirm that the list of Contacts is now sorted in ascending order. Close the player when you are finished testing:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E8_20.png)

18. Click on the **Save** icon to save all changes to the custom page.
19. Click on the **Back** button to exit the canvas designer.

## Exercise 9: Diagnosing and Resolving Performance Issues

As your custom page is deployed and used over time, performance issues may arise. For these scenarios, the [Monitor tool](https://learn.microsoft.com/en-us/power-apps/maker/monitor-overview) provides us with the capability to perform a "deep view" into our app, thereby allowing us to understand all the key activities that occur. From there, bottlenecks can be identified and resolved. In order to use Monitor, you first need to integrate your custom page into your model-driven app. From there, the Monitor tool can be launched from the model-driven app, and it will capture all the relevant information from the custom page.

For this exercise, we will skip working with the Monitor; instead, we will simulate a scenario involving [delegation and query limits](https://learn.microsoft.com/en-us/power-apps/maker/canvas-apps/delegation-overview) by importing some additional Contact data into our Dataverse environment. We will see how this behaviour affects our app at runtime.

1. Download the [ContactData.csv](/PowerFxFromNoviceToNinja/Resources/Lab2-UsingPowerFxInCustomPages/Contacts.csv) file to your local machine.
2. Open a new browser tab and navigate to the [Power Apps Maker Portal](https://make.powerapps.com).
3. In the **Power Apps Maker Portal**, click on **Tables** from the left-hand navigation menu and then click on **Import** -> **Import Data**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_1.png)

4. In the **Choose data source** page, drag and drop or browse and select the **ContactData.csv** file you downloaded in step 1. Once selected, click on **Next**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_2.png)

5. On the **Connect to data source** page, click on **Sign in**. If prompted, sign in with your work or school account. Once you've signed in, click on **Next**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_3.png)

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_4.png)

6. On the **Preview file data** screen, review the data that will be imported and then click on **Next**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_5.png)

7. On the Power Query editor page, click on the icon next to **birthdate** and change the data type to **Date**. If prompted with a **Change column type** dialog, click on **Add new step**. Once done, click on **Next**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_6.png)

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_7.png)

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_8.png)

8. On the **Map tables** page, populate the details as follows and then select **Next**:
    - **Load settings**: Select **Load to existing table**
    - **Destination table**: Select **Contact**
    - **Column mapping**: Map the **Source column**'s and **Destination column**'s as indicated below:
        - birthdate -> BirthDate
        - emailaddress1 -> EMailAddress1
        - firstname -> FirstName
        - lastname -> LastName

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_9.png)

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_10.png)

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_11.png)

9. On the **Refresh settings** page, ensure the **Refresh manually** option is selected and then click on **Publish**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_12.png)

10. You will be taken back to the Maker portal. Click on **More** and then select **Dataflows**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_13.png)

11. On the **Dataflows** page, you will see a single dataflow, which will display **In progress** under the **Next refresh** heading. Wait a few minutes, refresh the page and confirm that the refresh has completed successfully:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_14.png)

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_15.png)

12. Navigate to the **Wingtip Toys PP Solution** solution and then click on the `Lab 2` custom page to open it in the designer:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_16.png)

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_17.png)

13. With the **Contact Screen** selected, click on the **Play** icon in the top right corner of the screen to test the application. With the **All** option selected, you should see the new Contact records in the gallery:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_18.png)

Now that we have imported additional data into our Dataverse environment, we can simulate a scenario involving delegation. By default, the `Filter()` and `Sort()` function support delegation when connecting to Dataverse, which means that all records will be returned from the data source. This is because the connector can apply the appropriate filtering and sorting rules at the API level.

However, there are scenarios where delegation is not supported, even with the Dataverse connector. In these scenarios, by default, only the first 500 records will be returned from the data source. Let's adjust the app to reduce the number of records returned from the data source when queries can't be delegated, and then adjust the gallery formula so that delegation is no longer supported.

1. In the canvas designer view, click on the **Settings** icon in the bottom left of the screen:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_19.png)

2. On the **General** tab, scroll down, change the **Data row limit** value to `5` and click on **Close**:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_20.png)

3. Back on the **Contact Screen**, observe that the gallery is still displaying all records from the Contact table. Click on the **Play** icon in the top right corner of the screen to test the custom page. With the **Internal** option selected, you should see at least 7 records:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_21.png)

4. Adjust the formula of the **Items** property on the **Contact Gallery** control to introduce a scenario where delegation will no longer be supported:
    
    ```
    Switch('Contact Filter'.SelectedText.Value, 
        "All", Sort(Contacts, 'Full Name', SortOrder.Ascending),
        "Internal", Sort(Filter(Contacts, 'External Contact?' = 'External Contact? (Contacts)'.Internal), 'Full Name', SortOrder.Ascending),
        "External", Sort(Filter(Contacts, 'External Contact?' = 'External Contact? (Contacts)'.External), 'Full Name', SortOrder.Ascending)
    )
    ```

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_22.png)

5. Notice now that the gallery control has a warning icon displayed and when you press **Play** to test the custom page, only 2 internal Contact records are now returned. Because we adjusted the delegation settings of the custom page, we can more clearly see how delegation issues can impact the data that is returned:

    ![](Images/Lab2-UsingPowerFxInCustomPages/E9_23.png)

> [!IMPORTANT]
> For other Dataverse data types, `IsBlank()` is usually delegable. Choice columns are the only type that are not supported. For more information on what is and isn't supported for delegation with the Dataverse connector, [consult the Microsoft Learn site](https://learn.microsoft.com/en-us/power-apps/maker/canvas-apps/connections/connection-common-data-service#power-apps-delegable-functions-and-operations-for-dataverse).

6. Fix the delegation issue by reverting the formula in the gallery **Items** property:

    ```
    Switch('Contact Filter'.SelectedText.Value, 
        "All", Sort(Contacts, 'Full Name', SortOrder.Ascending),
        "Internal", Sort(Filter(Contacts, 'External Contact?' = 'External Contact? (Contacts)'.Internal), 'Full Name', SortOrder.Ascending),
        "External", Sort(Filter(Contacts, 'External Contact?' = 'External Contact? (Contacts)'.External), 'Full Name', SortOrder.Ascending)
    )
    ```
7. Notice that the delegation warnings disappear and the expected number of Contact records are returned again.

This part of the exercise is designed to make you aware of the practical implications of delegation and how it can impact your app, especially in relation to what data is returned for an end user. The Dataverse connector, along with the [SQL Server](https://learn.microsoft.com/en-us/connectors/sql/#power-apps-functions-and-operations-delegable-to-sql-server), [SharePoint](https://learn.microsoft.com/en-us/connectors/sharepointonline/#power-apps-delegable-functions-and-operations-for-sharepoint) and [Salesforce](https://learn.microsoft.com/en-us/connectors/salesforce/#power-apps-delegable-functions-and-operations-for-salesforce) connectors support the widest range of delegation options, but other data sources may vary. You should always ensure you check the documentation for any connector you use in your app, to understand what is and isn't supported with delegation.

**Congratulations, you've finished Lab 2** 🥳