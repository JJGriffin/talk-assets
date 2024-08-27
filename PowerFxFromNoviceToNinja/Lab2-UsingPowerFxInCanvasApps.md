# Lab 2 - Using Power Fx in Canvas Power Apps

In this lab, you will create a basic canvas app that connects to Microsoft Dataverse, to allow users to view, edit and create Contact records. You will then extend the app to include a simple Power Fx formula to calculate the age of each Contact record and to display weather information relating to the Contact's location, via a Power Automate cloud flow.

## Scenario

Wingtip Toys require a simple application that allows sales people to view, edit and create Contact records. The app should also display the age of each Contact record and the current weather information for the Contact's location, to assist the sellers in conducting more personalised conversations with their customers.

Having spent some time familiarising with the fundamental capabilities of Power Fx and Power Apps, you have been tasked with creating a simple canvas app that allows the sellers to work with the Contact records, and support the additional requirements relating to the age of the Contacts and the weather information for each individual.

## Instructions

In this lab, you will do the following:

- Create a simple two screen application, with navigation between the screens.
- Customise each app screen to display a list of all Contacts and a form level view for a single Contact.
- Add several Power Fx formulas for navigation and to calculate the age of each Contact record.
- Add a Power Automate cloud flow to retrieve the weather information for the Contact's location.
- Review and implement recommendations from the Power Apps app checker.

This lab will take approximately 30 minutes to complete.

> [!IMPORTANT]
> Ensure that all steps have been completed in Lab 0 before proceeding with this lab.

## Exercise 1: Create a Canvas App

> [!IMPORTANT]
> When creating a canvas app, it is generally preferred to [create a solution](https://learn.microsoft.com/en-us/power-platform/alm/solution-concepts-alm) first, alongside a corresponding [solution publisher](https://learn.microsoft.com/en-us/power-platform/alm/solution-concepts-alm#solution-publisher), and to create the app from there. This will help to keep your apps organized and make it easier to deploy them out. For the purposes of this lab, we will skip these steps.

1. Navigate to the [Power Apps Maker Portal](https://make.powerapps.com) and, if not already selected, navigate to the developer environment you created in Lab 0:
   
    ![](Images/Lab2-UsingPowerFxInCanvasApps/E1_1.png)

2. Click on **Apps** from the left-hand navigation menu, and then click on **+ New app**. In the sub-menu, select **Start with a page design**:
   
    ![](Images/Lab2-UsingPowerFxInCanvasApps/E1_2.png)

3. On the **Start with a page design** screen, select **Blank canvas**:
   
    ![](Images/Lab2-UsingPowerFxInCanvasApps/E1_3.png)

4. After a few moments, the Power Apps studio will open. If you see a **Welcome to Power Apps Studio** window, click on **Skip** to proceed to designer view:
   
    ![](Images/Lab2-UsingPowerFxInCanvasApps/E1_4.png)

5. In the Power Apps studio, click on the **Screen1** label in the left-hand tree view, and rename the screen to **Contact Screen**:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E1_5.png)

> [!IMPORTANT]
> You can also double click any control or screen property to rename it.

6. Add a new screen to the app by clicking on the **+ New Screen** button and selecting the **Blank** screen template:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E1_6.png)

7. Rename the new screen to **Contact Form**. Your app navigation view should resemble the below if done correctly:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E1_7.png)

8. We will now add in the **Contact** table from Microsoft Dataverse as a data source. Click on the **Data** tab in the left-hand menu, and then click on **+ Add data**:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E1_7.png)

9. A list of tables from the current environment will display, with the **Contacts** table option visible. Select it if so; otherwise, search for and then select the table from the list:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E1_9.png)

10. After a few moments, the **Data** pane will refresh and display the **Contacts** table:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E1_10.png)

11. On the top right of the designer view, click on the **Save** icon:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E1_11.png)

12. In the **Save as** dialog, enter a name value of `Lab 2`, followed by your initials, and then click on **Save**:
   
    ![](Images/Lab2-UsingPowerFxInCanvasApps/E1_12.png)

13. Leave the canvas app open, as you will continue working with it in the next exercise.

## Exercise 2: Design the Canvas App

1. In the `Lab 2` canvas app, ensure that the **Tree view** icon is selected:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_1.png)

2. On the ribbon, click on the **Insert** tab, and then click on **Vertical gallery**:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_2.png)

3. Rename the newly inserted **Gallery1** control to **Contact Gallery**:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_3.png)

> [!IMPORTANT]
> Although the process of renaming screens and controls may seem tedious, having useful and descriptive names will help when writing formulas later in the lab.

4. Select the **Contact Gallery** control, and with the **Items** property selected in the dropdown, change the value to `Contacts`:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_4.png)

5. Repeat the same steps in 4, but this time, select and configure the following properties as indicated in the table below. Once configured correctly, the app screen should resemble the screenshot below:

    | Property | Formula |
    | --- | --- |
    | **Height** | `680` |
    | **Size** | `36` |
    | **Width** | `1366` |
    | **X** | `0` |
    | **Y** | `88` |

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_5.png)

> [!IMPORTANT]
> If you see no sample data in the gallery, then you may need to install this into your environment. Select the gear icon in the top right corner of the screen, and then click on **Advanced settings**. Next, click the chevron next to **Settings** and then select **Data Management**. On the **Data Management** screen, click on **Sample data** and then click on the **Install sample data** button. Installation may take a few minutes to complete. You can return to the canvas app and refresh the **Contacts** data source to see the sample data in the gallery. For further details on installing sample data, refer to the [Microsoft Learn article](https://learn.microsoft.com/en-us/power-platform/admin/add-remove-sample-data).

![](Images/Lab2-UsingPowerFxInCanvasApps/E2_6.png)

![](Images/Lab2-UsingPowerFxInCanvasApps/E2_7.png)

![](Images/Lab2-UsingPowerFxInCanvasApps/E2_8.png)

![](Images/Lab2-UsingPowerFxInCanvasApps/E2_9.png)

6. On the ribbon, click on the **Insert** tab, and then click on **Rectangle**:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_10.png)

7. Rename the newly inserted **Rectangle1** control to **Gallery Header**:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_11.png)

8. Select the **Gallery Header** control, and with the **Fill** property selected in the dropdown, change the value to `RGBA(250, 155, 112, 1)`:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_12.png)

9. Repeat the same steps in 8, but this time, select and configure the following properties as indicated in the table below. Once configured correctly, the app screen should resemble the screenshot below:

    | Property | Formula |
    | --- | --- |
    | **Height** | `88` |
    | **Width** | `1366` |
    | **X** | `0` |
    | **Y** | `0` |

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_13.png)

10. On the **Gallery Header** control, click on the **Insert** tab, and then click on **Text label**:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_14.png)

11. Rename the newly inserted **Label1** control to **Header Label**:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_15.png)

12. Select the **Header Label** control, and with the **Text** property selected in the dropdown, change the value to `Wingtip Toys Contacts`:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_16.png)

13. Repeat the same steps in 12, but this time, select and configure the following properties as indicated in the table below. Once configured correctly, the app screen should resemble the screenshot below:

    | Property | Formula |
    | --- | --- |
    | **Align** | `Align.Center` |
    | **Color** | `RGBA(255, 255, 255, 1)` |
    | **FontWeight** | `FontWeight.Bold` |
    | **Height** | `60` |
    | **Size** | `24` |
    | **Width** | `380` |
    | **X** | `493` |
    | **Y** | `14` |

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_17.png)

14. With the **Tree view** icon selected, click on the **Contact Screen** screen in the tree view:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_18.png)

15. On the **Contact Screen** screen, click on the **Insert** tab, and then click on **Edit form**:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_19.png)

> [!IMPORTANT]
> You may need to scroll down to see the **Edit form** option.

16. Rename the newly inserted **Form1** control to **ContactForm**:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_20.png)

17. Select the **ContactForm** control, and with the **DataSource** property selected in the dropdown, change the value to `Contacts`:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_21.png)

18. Repeat the same steps in 17, but this time, select and configure the following properties as indicated in the table below. Once configured correctly, the app screen should resemble the screenshot below:

    | Property | Formula |
    | --- | --- |
    | **BorderThickness** | `2` |
    | **Height** | `679` |
    | **Item** | `'Contact Gallery'.Selected` |
    | **Width** | `950` |
    | **X** | `124` |
    | **Y** | `89` |

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_22.png)

19. With the **ContactForm** control selected, select the **Edit fields** button in the properties pane:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_23.png)

> [!IMPORTANT]
> In all of the previous lab steps, we could have used the properties pane to modify font size, color, height, width and a range of other properties. Use whichever experience you prefer when building your own apps.

20. In the **Edit fields** pane, click on the **Add field** button:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_24.png)

21. In the **Add a field** dialog, search for and tick the box next to the **Full Name** field:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_25.png)

22. Repeat step 21, but this time, search and tick the box for the following fields. Once all fields are ticked, click on the **Add** button:
    
    - **Last Name**
    - **Email**
    - **Birthday**
    - **Business Phone**
    - **Address 1**

23. After a few moments, the form control should refresh itself and resemble the below:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_26.png)

24. Navigate back to the **Contact Screen** screen in the tree view.

25. With the `CTRL` key held down, select the following controls in the tree view:
    - **Header Label**
    - **Gallery Header**

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_27.png)

26. In the ribbon, select the **Copy** option:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_28.png)

27. Navigate to the **Contact Form** screen in the tree view and then click on the **Paste** icon. The shape and label will be copied into the correct position, as indicated below:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_29.png)

28. Rename the newly pasted **Header Label_1** control to **Form Label** and **Gallery Header_1** control to **Form Header**:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_30.png)

29. Adjust the **Text** property of the **Form Label** control to `Contact Details`:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_31.png)

30. On the **Contact Screen** screen, click on the **Insert** tab, and then search for and select the **Back arrow** control:

31. Rename the newly inserted **Arrow1** control to **Back Arrow**:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_33.png)

32. Select the **Back Arrow** control, and with the **Height** property selected in the dropdown, change the value to `150`:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_34.png)

33. Repeat the same steps in 32, but this time, select and configure the following properties as indicated in the table below. Once configured correctly, the app screen should resemble the screenshot below:

    | Property | Formula |
    | --- | --- |
    | **Width** | `124` |
    | **X** | `0` |
    | **Y** | `89` |

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_35.png)

34. On the **Contact Screen** screen, click on the **Insert** tab, and then search for and select the **Button** control:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_36.png)

35. Rename the newly inserted **Button1** control to **Get Weather**:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_37.png)

36. Select the **Get Weather** control, and with the **Text** property selected in the dropdown, change the value to `"Get Weather"`:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_38.png)

37. Repeat the same steps in 36, but this time, select and configure the following properties as indicated in the table below. Once configured correctly, the app screen should resemble the screenshot below:

    | Property | Formula |
    | --- | --- |
    | **X** | `1144` |
    | **Y** | `408` |

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_39.png)

38. On the **Contact Screen** screen, click on the **Insert** tab, and then search for and select the **Label** control:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_40.png)

39. Rename the newly inserted **Label1** control to **Weather Label**:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_41.png)

40. Select the **Weather Label** control, and with the **Text** property selected in the dropdown, change the value to `"REPLACEME"`:

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_42.png)

41. Repeat the same steps in 40, but this time, select and configure the following properties as indicated in the table below. Once configured correctly, the app screen should resemble the screenshot below:

    | Property | Formula |
    | --- | --- |
    | **FontWeight** | `FontWeight.Bold` |
    | **Height** | `260` |
    | **Width** | `250` |
    | **X** | `1099` |
    | **Y** | `118` |

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_43.png)

42. Save all changes to the canvas app by clicking on the **Save** icon in the top right corner of the screen.

    ![](Images/Lab2-UsingPowerFxInCanvasApps/E2_44.png)

We now have a basic application that allows us to view Contact record data and drill into a single Contact record. In the next exercises, we will add the necessary Power Fx formulas to navigate between screens, calculate and display the age of a Contact and allow the user to dynamically display weather information for the Contact's location.

## Exercise 3: Implement Power Fx Formulas

## Exercise 4: Add a Power Automate cloud flow to retrieve weather information

## Exercise 5: Review and implement recommendations from the Power Apps app checker