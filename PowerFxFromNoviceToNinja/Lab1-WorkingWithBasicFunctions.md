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

7. Using the previous examples as a guide, experiment with other basic calculations in the Power Fx REPL. For example, attempt to calculate the following values:

 - Sixty five (65) multiplied by twenty three (23).
 - One hundred and twenty three (123) divided by three (3).
 - Combine the previous two calculations into a single formula, that is then divided together.

8. Leave the Power Fx REPL open, as you will use it again in the next exercise.

## Exercise 3: Execute basic functions using Power Fx

## Exercise 4: Create a canvas app

1. Navigate to the [Power Apps Maker Portal](https://make.powerapps.com) and, if not already selected, navigate to the developer environment you created in Lab 0:
   
    ![](Images/Lab1-WorkingWithBasicFunctions/E4_1.png)

2. Click on **Apps** from the left-hand navigation menu, and then click on **+ New app**. In the sub-menu, select **Start with a page design**:
   
    ![](Images/Lab1-WorkingWithBasicFunctions/E4_2.png)

## Exercise 5: Execute basic Power Fx functions in a canvas app