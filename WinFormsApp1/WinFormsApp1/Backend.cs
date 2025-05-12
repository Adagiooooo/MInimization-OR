using System;
using System.Collections.Generic;

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
                for (int j = 0; j < Columns; j++)
                {
                    Console.Write($"{Tableau[i, j],8:F2} ");
                }
                Console.WriteLine();
            }
        }
    }

    // Derived class for storing each iteration’s tableau
    public class SimplexTableauIteration : SimplexTableau
    {
        public int IterationNumber { get; set; }

        public override void PrintTableau()
        {
            Console.WriteLine($"\n--- Iteration {IterationNumber} ---");
            base.PrintTableau();
        }
    }

    internal class Program
    {
        // Global list to store each iteration’s table
        static List<SimplexTableau> simplexIterations = new List<SimplexTableau>();

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

            initialTableau.PrintTableau();

            // Start simplex iterations
            RunSimplexAlgorithm(numVars, numConstraints);

            Console.WriteLine("\nSimplex process completed. You can access stored iterations from the global list.");
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
                    tableau[i, slackArtificialIndex++] = 1;  // Artificial variable
                }
                else if (constraintTypes[i] == "=")
                {
                    tableau[i, slackArtificialIndex++] = 1;  // Artificial variable
                }

                tableau[i, totalCols - 1] = constraints[i, numVars]; // RHS
            }

            // Fill objective row
            for (int j = 0; j < numVars; j++)
            {
                tableau[totalRows - 1, j] = -objective[j]; // Minimization: negate coefficients
            }

            // Big M adjustment can be added here if needed...

            return new SimplexTableauIteration
            {
                Tableau = tableau,
                Rows = totalRows,
                Columns = totalCols
            };
        }

        static void RunSimplexAlgorithm(int numVars, int numConstraints)
        {
            int iteration = 1;
            bool optimal = false;

            while (!optimal)
            {
                // Clone the last tableau
                SimplexTableauIteration lastTableau = simplexIterations[simplexIterations.Count - 1] as SimplexTableauIteration;
                double[,] tableau = lastTableau.Tableau;
                int rows = lastTableau.Rows;
                int cols = lastTableau.Columns;

                // Check if optimal (no negative values in objective row)
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

                if (pivotCol == -1)
                {
                    Console.WriteLine("Optimal solution found.");
                    optimal = true;
                    break;
                }

                // Find pivot row
                double minRatio = double.MaxValue;
                int pivotRow = -1;
                for (int i = 0; i < rows - 1; i++)
                {
                    if (tableau[i, pivotCol] > 0)
                    {
                        double ratio = tableau[i, cols - 1] / tableau[i, pivotCol];
                        if (ratio < minRatio)
                        {
                            minRatio = ratio;
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

                // Store the new tableau
                double[,] newTable = (double[,])tableau.Clone();
                simplexIterations.Add(new SimplexTableauIteration
                {
                    IterationNumber = iteration,
                    Tableau = newTable,
                    Rows = rows,
                    Columns = cols
                });

                simplexIterations[iteration].PrintTableau();
                iteration++;
            }
        }
    }
}
