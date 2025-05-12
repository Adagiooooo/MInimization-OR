using System;
using System.Collections.Generic;
using System.Linq;

namespace Minimization_Console
{
    // Base class for storing simplex table data
    public class SimplexTableau
    {
        public double[,] Tableau { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

        public virtual void PrintTableau()
        {
            for (int i = 0; i < Rows; i++)
            {
                Console.Write($"{Tableau[i, Columns - 1],8:F2} "); // RHS
                for (int j = 0; j < Columns - 1; j++)
                {
                    Console.Write($"{Tableau[i, j],8:F2} ");
                }
                Console.WriteLine();
            }
        }
    }

    // Derived class for storing each iteration’s tableau and ratios
    public class SimplexTableauIteration : SimplexTableau
    {
        public int IterationNumber { get; set; }
        public string[] Ratios { get; set; } // Store ratios as strings to handle "-"

        public override void PrintTableau()
        {
            Console.WriteLine($"\n--- Iteration {IterationNumber} ---");
            for (int i = 0; i < Rows; i++)
            {
                Console.Write($"{Tableau[i, Columns - 1],8:F2} "); // RHS
                for (int j = 0; j < Columns - 1; j++)
                {
                    Console.Write($"{Tableau[i, j],8:F2} ");
                }
                if (Ratios != null && i < Ratios.Length)
                {
                    Console.Write($"   {Ratios[i],8} "); // Print ratio alongside
                }
                Console.WriteLine();
            }
        }
    }
    internal class Program
    {
        // Global list to store each iteration’s table
        static List<SimplexTableauIteration> simplexIterations = new List<SimplexTableauIteration>();
        static int currentIteration = 0; // Keep track of the current iteration being viewed

        static void Main(string[] args)
        {
            Console.WriteLine("Simplex Method Minimization (up to 10x10)");

            Console.Write("Enter the number of variables (max 10): ");
            int numVars = int.Parse(Console.ReadLine());
            Console.Write("Enter the number of constraints (max 10): ");
            int numConstraints = int.Parse(Console.ReadLine());

            // Get objective function coefficients
            Console.WriteLine("\nEnter coefficients of the objective function:");
            double[] objective = new double[numVars];
            for (int i = 0; i < numVars; i++)
            {
                Console.Write($"Coefficient of x{i + 1}: ");
                objective[i] = double.Parse(Console.ReadLine());
            }

            // Get constraints
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

            // Build the initial tableau
            SimplexTableauIteration initialTableau = BuildInitialTableau(objective, constraints, constraintTypes, numVars, numConstraints);
            initialTableau.IterationNumber = 0;
            simplexIterations.Add(initialTableau);
            currentIteration = 0; // Initialize current iteration
            initialTableau.PrintTableau();

            // Start simplex iterations and display each one
            RunSimplexAlgorithm(initialTableau.Tableau, numVars, numConstraints, constraintTypes);

            Console.WriteLine("\n--- All Iterations ---");
            foreach (var iterationTableau in simplexIterations)
            {
                iterationTableau.PrintTableau();
            }
            Console.WriteLine("\nSimplex process completed.");
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
                    tableau[i, slackArtificialIndex++] = 1;     // Artificial variable
                }
                else if (constraintTypes[i] == "=")
                {
                    tableau[i, slackArtificialIndex++] = 1;     // Artificial variable
                }
                tableau[i, totalCols - 1] = constraints[i, numVars]; // RHS
            }

            // Fill objective row
            for (int j = 0; j < numVars; j++)
            {
                tableau[totalRows - 1, j] = -objective[j]; // Minimization: negate coefficients
            }

            return new SimplexTableauIteration
            {
                Tableau = tableau,
                Rows = totalRows,
                Columns = totalCols,
                Ratios = null // Initialize ratios to null
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

                // Calculate Ratios
                string[] ratios = new string[rows - 1];
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

                if (pivotCol != -1)
                {
                    for (int i = 0; i < rows - 1; i++)
                    {
                        if (tableau[i, pivotCol] > 0)
                        {
                            ratios[i] = (tableau[i, cols - 1] / tableau[i, pivotCol]).ToString("F2");
                        }
                        else
                        {
                            ratios[i] = "-";
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
    }
}
