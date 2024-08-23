# Lab 1 - Working with Basic Functions

In this lab, you will experiment working with the most common functions in Power Fx, using the interactive Power Fx Read, Evaluation, Print, Loop (REPL) within the PAC CLI.

## Scenario

Having just finished configuring your developer environment for Wingtip Toys, you need to start familiarising yourself with how Power Fx works, and the types of functions you can execute. You plan to use the following experiences to compare and contrast the capabilities on offer:

- The PAC CLI, within a local PowerShell terminal environment.
- A canvas app in the developer environment you provisioned in Lab 0.

Through this experimentation, you plan to gain an understanding of the core functions you will need to use every day when working with Power Fx, and how the different authoring experiences compare.

## Instructions

In this lab, you will do the following:

- Connect to your Dataverse environment using the Power Fx REPL.
- Execute some basic calculations within the PAC CLI.
- Execute some basic functions within the PAC CLI.
- Create a canvas app within the Power Apps Maker portal.
- Implement and execute basic functions within a canvas app, and see how this compares to the PAC CLI.

This lab will take approximately 30 minutes.

> [!IMPORTANT]
> Ensure that all steps have been completed in Lab 0 before proceeding with this lab.

## Exercise 1: Connect to Dataverse from the PAC CLI to execute Power Fx

1. If Visual Studio Code is not open from Lab 0, open it now.
2. Open a new terminal window by selecting **Terminal** from the top menu, and then **New Terminal**:
   
    ![](Images/Lab1-WorkingWithBasicFunctions/E1_1.png)

3. In the terminal window, type the following command and then press **Enter**:

    ```
    pac power-fx repl
    ```
4. The Power Fx REPL will start. Once connected, you will see a prompt that resembles the below. This indicates that the REPL is ready to receive new commands:

    ![](Images/Lab1-WorkingWithBasicFunctions/E1_2.png)

## Exercise 2: Execute basic calculations using Power Fx

1. In the Power Fx REPL, type the following command and then press **Enter**:

    ```
    73 + 156
    ```
2. The Power Fx REPL will return the result of the calculation, which should equal `229`:
    
    ![](Images/Lab1-WorkingWithBasicFunctions/E2_1.png)

3. In the Power Fx REPL, type the following command and then press **Enter**:

    ```
    156 - 123
    ```
4. The Power Fx REPL will return the result of the calculation, which should equal `33`:
    
    ![](Images/Lab1-WorkingWithBasicFunctions/E2_2.png)

5. In the Power Fx REPL, type the following command and then press **Enter**:

    ```
    (325 + 123) / (12 - 3) * 2
    ```

6. The Power Fx REPL will return the result of the calculation, which should equal `99.55555555555555555555555556`. Notice that Power Fx is able to handle the more complex calculation and return the correct result:
    
    ![](Images/Lab1-WorkingWithBasicFunctions/E2_3.png)

7. In the Power Fx REPL, type the following command and then press **Enter**:

    ```
    123 / 0
    ```
8. The Power Fx REPL will return an error message, indicating that division by zero is not allowed:

    ![](Images/Lab1-WorkingWithBasicFunctions/E2_4.png)

9. Rewrite the formula in step 7 by using the **IfError()** function to return a default value of zero if an error occurs and then press **Enter**:

    ```
    IfError(123 / 0, 0)
    ```
10. This time, the Power Fx REPL will return the value of zero, instead of an error:

    ![](Images/Lab1-WorkingWithBasicFunctions/E2_5.png)

> [!IMPORTANT]
> Division by zero is a common scenario that can cause errors, not just in Power Fx, but in other progamming languages as well. It's always a good idea to handle these scenarios in your any formulas where divisions take place, to prevent any unexpected errors from occurring.

11. Using the previous examples as a guide, experiment with other basic calculations in the Power Fx REPL. For example, attempt to calculate the following values:

 - Sixty five (65) multiplied by twenty three (23).
 - One hundred and twenty three (123) divided by three (3).
 - Combine the previous two calculations into a single formula, that is then divided together.

12. Leave the Power Fx REPL open, as you will use it again in the next exercise.

## Exercise 3: Execute basic functions using Power Fx

1. In the Power Fx REPL, run the following functions to intialize some variables; these will be used throughout the rest of this exercise:

    ```
    Set(varString, "Sample string value."); Set(varNumber, 456); Set(varDate, Date(2024, 10, 15)); Set(varNow, Now())
    ```
    
    ![](Images/Lab1-WorkingWithBasicFunctions/E3_1.png)

2. In the Power Fx REPL, type the following command and then press **Enter**:

    ```
    If(varString = "Sample string value.", true, false)  
    ```
3. The Power Fx REPL will return the value `true`, indicating that the variable `varString` is equal to the string value:

    ![](Images/Lab1-WorkingWithBasicFunctions/E3_2.png)

4. In the Power Fx REPL, type the following command and then press **Enter**:

    ```
    If(varString = "Another sample string value.", true, false)
    ```

5. This time, the Power Fx REPL will return the value `false`, indicating that the variable `varString` is not equal to the new string value:

    ![](Images/Lab1-WorkingWithBasicFunctions/E3_3.png)

6. In the Power Fx REPL, type the following command and then press **Enter**:

    ```
    varString & " This is a new sentence."
    ```
7. The Power Fx REPL will return the value `Sample string value. This is a new sentence.`:

    ![](Images/Lab1-WorkingWithBasicFunctions/E3_4.png)

8. In the Power Fx REPL, type the following command and then press **Enter**:

    ```
    Concatenate(varString, " This is a new sentence.")
    ```
9. The Power Fx REPL will return the value `Sample string value. This is a new sentence.`:

    ![](Images/Lab1-WorkingWithBasicFunctions/E3_5.png)

> [!IMPORTANT]
> Often there will be multiple approaches to achieving the same outcome with Power Fx. In the previous example, both the `&` operator and the `Concatenate()` function were used to concatenate two strings together. Both approaches are valid, but the `Concatenate()` function is more explicit and easier to read. Regardless of which approach you choose, it's important to be consistent in your use of functions and operators throughout your Power Fx formulas.

10. In the Power Fx REPL, type the following command and then press **Enter**:

    ```
    varNumber + 123
    ```

11. The Power Fx REPL will return the result of the addition, `579`:

    ![](Images/Lab1-WorkingWithBasicFunctions/E3_6.png)

12. In the Power Fx REPL, type the following command and then press **Enter**:

    ```
    Sum(varNumber, 123)
    ```

13. The Power Fx REPL will return the same result as before, `579`:

    ![](Images/Lab1-WorkingWithBasicFunctions/E3_7.png)

> [!IMPORTANT]
> Usage of the **Sum()** function or the `+` operator will achieve the same outcome. Again, consistency is always key, and you should avoid mixing different approaches within your formulas or apps.

14. In the Power Fx REPL, type the following command and then press **Enter**:

    ```
    Text(varNumber) * 23
    ```

15. Notice that the expected result of `10488` is returned, despite the fact that `varNumber` has been cast as a string; provided that the underlying value is a valid number, Power Fx will automatically convert the string to a number when required:

    ![](Images/Lab1-WorkingWithBasicFunctions/E3_8.png)

16. In the Power Fx REPL, type the following command and then press **Enter**:

    ```
    Text(DateAdd(varDate, 5), "yyyy-mm-dd")
    ```
17. The Power Fx REPL will return the date `"2024-10-20"`, which is five days after the original date. By default, if the third argument for Units is not specified, **TimeUnit.Days** is used:

    ![](Images/Lab1-WorkingWithBasicFunctions/E3_9.png)

18. Execute the same function again but this time, specify a Units argument of **TimeUnit.Years**. Observe the new result, which is now `"2025-03-15"`:

    ```
    Text(DateAdd(varDate, 5, TimeUnit.Months), "yyyy-mm-dd")
    ```
    ![](Images/Lab1-WorkingWithBasicFunctions/E3_10.png)

> [!IMPORTANT]
> Being explicit with the arguments you pass to functions can help to make your formulas more readable and easier to understand.

19. In the Power Fx REPL, type the following command and then press **Enter**:

    ```
    DateDiff(varNow, varDate)
    ```
20. The Power Fx REPL will return the number of day(s) between the two dates. In the example screenshot below, the result is `53`, based on the indicated **varNow** value; the result may vary depending on the current date and time:
    
    ![](Images/Lab1-WorkingWithBasicFunctions/E3_11.png)

21. In the Power Fx REPL, type the following command and then press **Enter**:

    ```
    DateDiff(varNow + 5, varDate) 
    ```

22. The Power Fx REPL will return the number of days between the two dates. In the example screenshot below, the result is `48`, based on the indicated **varNow** value; the result may vary depending on the current date and time. Notice that it's possible to use arithmetic operators to increase `+` or decrease `-` a date value by days:

    ![](Images/Lab1-WorkingWithBasicFunctions/E3_12.png)

23. Press `CTRL + C` to exit the Power Fx REPL and close the terminal window.

## Exercise 4: Create a canvas app

> [!IMPORTANT]
> When creating a canvas app for the first time, it is generally preferred to [create a solution](https://learn.microsoft.com/en-us/power-platform/alm/solution-concepts-alm) first, alongside a corresponding [solution publisher](https://learn.microsoft.com/en-us/power-platform/alm/solution-concepts-alm#solution-publisher). This will help to keep your apps organized and make it easier to manage them in the future. For the purposes of this lab, we will skip these steps.

1. Navigate to the [Power Apps Maker Portal](https://make.powerapps.com) and, if not already selected, navigate to the developer environment you created in Lab 0:
   
    ![](Images/Lab1-WorkingWithBasicFunctions/E4_1.png)

2. Click on **Apps** from the left-hand navigation menu, and then click on **+ New app**. In the sub-menu, select **Start with a page design**:
   
    ![](Images/Lab1-WorkingWithBasicFunctions/E4_2.png)

## Exercise 5: Execute basic Power Fx functions in a canvas app