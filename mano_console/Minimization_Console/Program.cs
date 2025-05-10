using System;
using System.Collections.Generic;
using System.Linq;

namespace Minimization_Console
{
    // Stores the simplex iterations
    public class SimplexTableau
    {
        public double[,] Tableau { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

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
        }
    }

    internal class Program
    {
        // Global list to store each iteration’s table
        static List<SimplexTableauIteration> simplexIterations = new List<SimplexTableauIteration>();
        static int currentIteration = 0; // Keeps track of the current iteration being viewed
        const double M = 1000; // Define M as a constant (large number)

        static void Main(string[] args)
        {
            Console.WriteLine("Simplex Method Minimization (up to 10x10)");

            Console.Write("Enter the number of variables (max 10): ");
            int numVars = int.Parse(Console.ReadLine());
            Console.Write("Enter the number of constraints (max 10): ");
            int numConstraints = int.Parse(Console.ReadLine());

            // Get the objective function coefficients
            Console.WriteLine("\nEnter coefficients of the objective function:");
            double[] objective = new double[numVars];
            for (int i = 0; i < numVars; i++)
            {
                Console.Write($"Coefficient of x{i + 1}: ");
                objective[i] = double.Parse(Console.ReadLine());
            }

            // Get the constraints
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
            initialTableau.PrintTableau();

            // Simplex iterations
            RunSimplexAlgorithm(initialTableau.Tableau, numVars, numConstraints, constraintTypes);
            Console.WriteLine("\nSimplex process completed. You can navigate through iterations using next/previous.");

            ShowNextPreviousMenu();

            Console.ReadKey();
        }

        static SimplexTableauIteration BuildInitialTableau(double[] objective, double[,] constraints, string[] constraintTypes, int numVars, int numConstraints)
        {
            int slackArtificialCount = 0;

            // Count how many extra columns we need (slack + artificial)
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
                    tableau[i, slackArtificialIndex] = 1;    // Artificial variable
                    artificialVarColumns.Add(slackArtificialIndex); // Store artificial variable column
                    slackArtificialIndex++;
                }
                else if (constraintTypes[i] == "=")
                {
                    tableau[i, slackArtificialIndex] = 1;    // Artificial variable
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
                Ratios = null // Initialize ratios to null for the initial tableau
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
                            ratios[i] = "-"; // Use "-" for invalid ratios
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < rows - 1; i++)
                        ratios[i] = "-";
                }
                // Store the current tableau and ratios
                SimplexTableauIteration tableauIteration = new SimplexTableauIteration
                {
                    IterationNumber = iteration,
                    Tableau = currentTableau,
                    Rows = rows,
                    Columns = cols,
                    Ratios = ratios
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
                    Console.WriteLine("Optimal solution found.");
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
                    Console.WriteLine("Unbounded solution.");
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

        static void ShowNextPreviousMenu()
        {
            int currentIteration = 0;
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
    }
}
