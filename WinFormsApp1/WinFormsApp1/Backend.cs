using System;
using System.Collections.Generic;
using System.Linq;

namespace Minimization_Console
{
    // Nakaglobal mga to sa buong code
    // Stores the simplex iterations
    public class SimplexTableau
    {
        public double[,] Tableau { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

        // for testing nlng to para makita laman ng array if want itest using CLI
        public virtual void PrintTableau()
        {
            for (int i = 0; i < Rows; i++)
            {
                // Prints the rightmost column first
                Console.Write($"{Tableau[i, Columns - 1],12:F2} ");

                // Prints the rest of the columns
                for (int j = 0; j < Columns - 1; j++)
                {
                    Console.Write($"{Tableau[i, j],12:F2} ");
                }
                Console.WriteLine();
            }
        }
    }

    // Stores and handles each iteration’s tableau operations
    public class SimplexTableauIteration : SimplexTableau
    {
        public int IterationNumber { get; set; }
        public string[] Ratios { get; set; }
        public double[] Cj { get; set; }
        public double[] Zj { get; set; }

        // ito ung nagpprint ng mga array (1st tableu, 2nd ratios, 3rd cj & zj))
        public override void PrintTableau()
        {
            Console.WriteLine($"\n--- Iteration {IterationNumber} ---");
            base.PrintTableau();

            if (Ratios != null)
            {
                Console.WriteLine("\n--- Ratios ---");
                for (int i = 0; i < Ratios.Length; i++)
                {
                    Console.Write($"{Ratios[i],12} ");
                }
                Console.WriteLine();
            }

            if (Cj != null && Zj != null)
            {
                Console.WriteLine("\n--- Cj and Zj ---");
                Console.Write($"{"",12}"); //spacer
                for (int j = 0; j < Cj.Length; j++)
                {
                    Console.Write($"{Cj[j],12:F2} ");
                }
                Console.WriteLine();

                Console.Write($"Zj:{"",12}"); //label
                for (int j = 0; j < Zj.Length; j++)
                {
                    Console.Write($"{Zj[j],12:F2} ");
                }
                Console.WriteLine();
            }
        }
    }

    // ito ung part ng program na nagaask ng input sa user at nagssolve ng simplex
    internal class Program
    {
        // Global list to store each iteration’s table
        static List<SimplexTableauIteration> simplexIterations = new List<SimplexTableauIteration>();
        static int currentIteration = 0; // Keeps track of the current iteration being viewed
        const double M = 1000; // Define M as a constant (large number)
        static double[] objectiveFunctionCoefficients; // Store the original objective function

        static void Main(string[] args)
        {

            // ito ung textbox na nag aask ng input sa user (number of variables and constraints))
            Console.Write("Enter the number of variables (max 10): ");
            int numVars = int.Parse(Console.ReadLine());
            Console.Write("Enter the number of constraints (max 10): ");
            int numConstraints = int.Parse(Console.ReadLine());

            // This is for the textbox where the user inputs the coefficients of the objective function
            Console.WriteLine("\nEnter coefficients of the objective function:");
            double[] objective = new double[numVars];
            objectiveFunctionCoefficients = new double[numVars]; // Store in the global variable
            for (int i = 0; i < numVars; i++)
            {
                Console.Write($"Coefficient of x{i + 1}: ");
                objective[i] = double.Parse(Console.ReadLine());
                objectiveFunctionCoefficients[i] = objective[i]; // Store
            }

            // this is for the textbox where the user inputs the coefficients of the constraints and the RHS values
            // mapapalitan na ito kasi may GUI na tayo to handle that. ung need nlng is ipalit ung GUI dito
            double[,] constraints = new double[numConstraints, numVars + 1];
            string[] constraintTypes = new string[numConstraints];

            for (int i = 0; i < numConstraints; i++)
            {
                Console.WriteLine($"\nConstraint {i + 1}:");
                for (int j = 0; j < numVars; j++)
                {
                    Console.Write($"Coefficient of x{j + 1}: ");
                    constraints[i, j] = double.Parse(Console.ReadLine());
                }

                Console.Write("Enter the type of constraint (<=, =, >=): ");
                constraintTypes[i] = Console.ReadLine();

                Console.Write("Enter the RHS value: ");
                constraints[i, numVars] = double.Parse(Console.ReadLine());
            }

            // Builds the initial tableau
            SimplexTableauIteration initialTableau = BuildInitialTableau(objective, constraints, constraintTypes, numVars, numConstraints);
            initialTableau.IterationNumber = 0;
            simplexIterations.Add(initialTableau);
            currentIteration = 0; // Initializes the current iteration

            // Simplex iterations
            RunSimplexAlgorithm(initialTableau.Tableau, numVars, numConstraints, constraintTypes);

            //Determine and Print the result
            double[,] finalTableau = simplexIterations.Last().Tableau;
            int rows = finalTableau.GetLength(0);
            int cols = finalTableau.GetLength(1);
            bool optimal = true;
            for (int j = 0; j < cols - 1; j++)
            {
                if (finalTableau[rows - 1, j] < 0)
                {
                    optimal = false;
                    break;
                }
            }

            // need ishow sa program if optimal or not ung solution
            if (optimal)
            {
                Console.WriteLine("Optimal solution found.");
            }
            else
            {
                Console.WriteLine("Unbounded solution or no feasible solution.");
            }

            ShowNextPreviousMenu(); // This now happens *after* the optimality check

            Console.ReadKey();
        }

        static SimplexTableauIteration BuildInitialTableau(double[] objective, double[,] constraints, string[] constraintTypes, int numVars, int numConstraints)
        {
            int slackArtificialCount = 0;

            // Count how many extra columns we need (slack + artificial)
            // ginagamit ito para maging same ung dami ng columns ng cj at zj sa tableau
            // bali "0" lng nmn mga ian. not needed narin naman sa gui
            // not needed nmn ito tlga sa calculator. kasi iba way q para isolve ung pivot column
            // pero need ito para tama ung dami ng columns pag nagttest sa cli
            foreach (var type in constraintTypes)
            {
                if (type == "<=")
                    slackArtificialCount++;
                else if (type == ">=" || type == "=")
                    slackArtificialCount += 2; // surplus + artificial
            }

            int totalCols = numVars + slackArtificialCount + 1; // +1 for RHS
            int totalRows = numConstraints + 1; // constraints + objective

            double[,] tableau = new double[totalRows, totalCols];
            int slackArtificialIndex = numVars;
            List<int> artificialVarColumns = new List<int>(); // List to store artificial variable column indices

            // Fill constraint rows
            // pambuild at panglaman ng values s tableau
            for (int i = 0; i < numConstraints; i++)
            {
                for (int j = 0; j < numVars; j++)
                {
                    tableau[i, j] = constraints[i, j];
                }

                if (constraintTypes[i] == "<=")
                {
                    tableau[i, slackArtificialIndex++] = 1; // Slack variable
                }
                else if (constraintTypes[i] == ">=")
                {
                    tableau[i, slackArtificialIndex++] = -1; // Surplus variable
                    tableau[i, slackArtificialIndex] = 1;      // Artificial variable
                    artificialVarColumns.Add(slackArtificialIndex); // Store artificial variable column
                    slackArtificialIndex++;
                }
                else if (constraintTypes[i] == "=")
                {
                    tableau[i, slackArtificialIndex] = 1;      // Artificial variable
                    artificialVarColumns.Add(slackArtificialIndex); // Store artificial variable column
                    slackArtificialIndex++;
                }

                tableau[i, totalCols - 1] = constraints[i, numVars]; // RHS
            }

            // Fill objective row
            for (int j = 0; j < numVars; j++)
            {
                tableau[totalRows - 1, j] = -objective[j]; // Minimization: negate coefficients
            }

            // Add Big M for artificial variables in the objective function
            for (int i = 0; i < numConstraints; i++)
            {
                if (constraintTypes[i] == "=" || constraintTypes[i] == ">=")
                {
                    int artificialCol = numVars + (constraintTypes[i] == ">=" ? i * 2 + 1 : i * 2);
                    tableau[totalRows - 1, artificialCol] = M;
                }
            }

            // Adjust the objective row for the initial basic feasible solution
            for (int i = 0; i < numConstraints; i++)
            {
                if (constraintTypes[i] == "=" || constraintTypes[i] == ">=")
                {
                    int artificialCol = numVars + (constraintTypes[i] == ">=" ? i * 2 + 1 : i * 2);
                    for (int j = 0; j < totalCols; j++)
                    {
                        tableau[totalRows - 1, j] -= M * tableau[i, j];
                    }
                }
            }

            return new SimplexTableauIteration
            {
                Tableau = tableau,
                Rows = totalRows,
                Columns = totalCols,
                Ratios = null, // Initialize ratios to null for the initial tableau
                Cj = GetCjArray(objective, numVars, constraintTypes), // Store Cj, now correctly sized
                Zj = null
            };
        }

        static void RunSimplexAlgorithm(double[,] initialTableau, int numVars, int numConstraints, string[] constraintTypes)
        {
            int iteration = 1;
            bool optimal = false;
            int rows = numConstraints + 1;
            int cols = initialTableau.GetLength(1);
            double[,] tableau = (double[,])initialTableau.Clone(); // Start with a *copy* of the initial tableau

            while (!optimal)
            {
                // Store the current tableau
                double[,] currentTableau = new double[rows, cols];
                Array.Copy(tableau, currentTableau, tableau.Length);

                // Calculate Ratios before storing the iteration
                string[] ratios = new string[rows - 1]; // Ratios for each constraint row, now strings
                int pivotCol = -1;
                double minVal = 0;

                for (int j = 0; j < cols - 1; j++)
                {
                    if (tableau[rows - 1, j] < minVal)
                    {
                        minVal = tableau[rows - 1, j];
                        pivotCol = j;
                    }
                }

                if (pivotCol != -1) // if a pivot column was found.
                {
                    for (int i = 0; i < rows - 1; i++)
                    {
                        if (tableau[i, pivotCol] > 0)
                        {
                            ratios[i] = (tableau[i, cols - 1] / tableau[i, pivotCol]).ToString("F2"); // Format as string
                        }
                        else
                        {
                            ratios[i] = "-"; // Use "-" for invalid ratios (negative values)
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < rows - 1; i++)
                        ratios[i] = "-";
                }

                // Calculate Zj
                double[] zj = CalculateZj(currentTableau, numVars, constraintTypes);

                // Store the current tableau and ratios
                SimplexTableauIteration tableauIteration = new SimplexTableauIteration
                {
                    IterationNumber = iteration,
                    Tableau = currentTableau,
                    Rows = rows,
                    Columns = cols,
                    Ratios = ratios,
                    Cj = GetCjArray(objectiveFunctionCoefficients, numVars, constraintTypes), // Pass Cj
                    Zj = zj
                };
                simplexIterations.Add(tableauIteration);

                // Check if optimal (no negative values in objective row)
                minVal = 0;
                pivotCol = -1;
                for (int j = 0; j < cols - 1; j++)
                {
                    if (tableau[rows - 1, j] < minVal)
                    {
                        minVal = tableau[rows - 1, j];
                        pivotCol = j;
                    }
                }

                if (pivotCol == -1)
                {
                    optimal = true;
                    break;
                }

                // Find pivot row
                double minRatioVal = double.MaxValue;
                int pivotRow = -1;
                for (int i = 0; i < rows - 1; i++)
                {
                    if (tableau[i, pivotCol] > 0)
                    {
                        double ratio = tableau[i, cols - 1] / tableau[i, pivotCol];
                        if (ratio < minRatioVal)
                        {
                            minRatioVal = ratio;
                            pivotRow = i;
                        }
                    }
                }

                if (pivotRow == -1)
                {
                    optimal = true;
                    break;
                }

                // Pivot operation
                double pivot = tableau[pivotRow, pivotCol];

                for (int j = 0; j < cols; j++)
                    tableau[pivotRow, j] /= pivot;

                for (int i = 0; i < rows; i++)
                {
                    if (i != pivotRow)
                    {
                        double factor = tableau[i, pivotCol];
                        for (int j = 0; j < cols; j++)
                        {
                            tableau[i, j] -= factor * tableau[pivotRow, j];
                        }
                    }
                }

                iteration++;
            }
        }

        // This method handles the next/previous iteration menu (will use a previous and next button for handling this))
        // may error handling ito since CLI lang ung ginamit pero sa GUI, previous and next buttons lng nmn un so no need na
        static void ShowNextPreviousMenu()
        {
            int currentIteration = 0;
            // Print initial tableau *only* when the user starts to navigate
            simplexIterations[currentIteration].PrintTableau();

            while (true)
            {
                Console.WriteLine("\nEnter 'n' for next iteration, 'p' for previous iteration, or 'q' to quit:");
                string input = Console.ReadLine().ToLower();

                if (input == "n")
                {
                    if (currentIteration < simplexIterations.Count - 1)
                    {
                        currentIteration++;
                        simplexIterations[currentIteration].PrintTableau();
                    }
                    // dapat sa GUI wala na mangyayari at last iteration na un
                    else
                    {
                        Console.WriteLine("You are at the last iteration.");
                    }
                }
                else if (input == "p")
                {
                    if (currentIteration > 0)
                    {
                        currentIteration--;
                        simplexIterations[currentIteration].PrintTableau();
                    }
                    // dapat sa GUI wala na mangyayari at first iteration na un
                    else
                    {
                        Console.WriteLine("You are at the first iteration.");
                    }
                }
                else if (input == "q")
                {
                    Console.WriteLine("Exiting iteration viewer.");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 'n', 'p', or 'q'.");
                }
            }
        }

        static double[] CalculateZj(double[,] tableau, int numVars, string[] constraintTypes)
        {
            int rows = tableau.GetLength(0);
            int cols = tableau.GetLength(1);
            double[] zj = new double[cols - 1]; // Zj for each column *except* the RHS

            // Get the coefficients of the variables in the objective function
            double[] cj = GetCjArray(objectiveFunctionCoefficients, numVars, constraintTypes);

            // Iterate through each column (except the RHS)
            for (int j = 0; j < cols - 1; j++)
            {
                zj[j] = 0;
                // Iterate through each row (except the objective row)
                for (int i = 0; i < rows - 1; i++)
                {
                    // For each column, multiply the Cj of the basic variable in that row by the value in that row and column, and sum them up.
                    double cjValue = 0;
                    if (IsBasicVariable(tableau, i, j, numVars, constraintTypes))
                    {
                        cjValue = GetCjForBasicVariable(tableau, i, numVars, constraintTypes);
                    }
                    zj[j] += cjValue * tableau[i, j];
                }
            }
            return zj;
        }

        static bool IsBasicVariable(double[,] tableau, int row, int col, int numVars, string[] constraintTypes)
        {
            int rows = tableau.GetLength(0);
            int cols = tableau.GetLength(1);
            int basicVariableCount = 0;

            for (int j = 0; j < cols - 1; j++)
            {
                basicVariableCount = 0;
                for (int i = 0; i < rows - 1; i++)
                {
                    if (tableau[i, j] == 0)
                    {
                        basicVariableCount++;
                    }
                    else if (tableau[i, j] == 1)
                    {
                        basicVariableCount++;
                    }
                }
                if (basicVariableCount == rows - 1)
                {
                    if (tableau[row, col] == 1)
                        return true;
                }
            }
            return false;
        }

        static double GetCjForBasicVariable(double[,] tableau, int row, int numVars, string[] constraintTypes)
        {
            int cols = tableau.GetLength(1);
            // First, check the original variables
            for (int j = 0; j < numVars; j++)
            {
                if (tableau[row, j] == 1) //check for a 1.
                {
                    bool isBasic = true;
                    for (int i = 0; i < tableau.GetLength(0) - 1; i++)
                    {
                        if (i != row && tableau[i, j] != 0)
                        {
                            isBasic = false;
                            break;
                        }
                    }
                    if (isBasic)
                        return objectiveFunctionCoefficients[j];
                }
            }

            // If it's not one of the original variables, check slack/surplus/artificial
            int slackOrArtificialIndex = numVars;
            for (int i = 0; i < constraintTypes.Length; i++)
            {
                if (constraintTypes[i] == "<=")
                {
                    if (tableau[row, slackOrArtificialIndex] == 1)
                        return 0; // Slack variable
                    slackOrArtificialIndex++;
                }
                else if (constraintTypes[i] == ">=")
                {
                    if (tableau[row, slackOrArtificialIndex] == -1)
                        slackOrArtificialIndex++; // Surplus
                    if (tableau[row, slackOrArtificialIndex] == 1)
                        return M; // Artificial
                    slackOrArtificialIndex += 2;
                }
                else if (constraintTypes[i] == "=")
                {
                    if (tableau[row, slackOrArtificialIndex] == 1)
                        return M; // Artificial
                    slackOrArtificialIndex++;
                }
            }
            return 0;
        }

        static double[] GetCjArray(double[] objectiveFunctionCoefficients, int numVars, string[] constraintTypes)
        {
            int slackArtificialCount = 0;
            foreach (string type in constraintTypes)
            {
                if (type == "<=")
                    slackArtificialCount++;
                else if (type == ">=" || type == "=")
                    slackArtificialCount += 2;
            }
            int totalCols = numVars + slackArtificialCount + 1;
            double[] cj = new double[totalCols - 1]; // -1 because Cj doesn't include the RHS

            // Copy the original objective function coefficients
            for (int i = 0; i < numVars; i++)
                cj[i] = objectiveFunctionCoefficients[i];

            int slackOrArtificialIndex = numVars;
            for (int i = 0; i < constraintTypes.Length; i++)
            {
                if (constraintTypes[i] == "<=")
                {
                    cj[slackOrArtificialIndex++] = 0; // Slack
                }
                else if (constraintTypes[i] == ">=")
                {
                    cj[slackOrArtificialIndex++] = 0; // Surplus
                    cj[slackOrArtificialIndex++] = M; // Artificial
                }
                else if (constraintTypes[i] == "=")
                {
                    cj[slackOrArtificialIndex++] = M; // Artificial
                }
            }
            return cj;
        }
    }
}

