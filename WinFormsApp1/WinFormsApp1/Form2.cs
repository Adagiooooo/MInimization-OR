using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form2 : Form
    {
        private const double BigM = 1000000;

        public Form2()
        {
            InitializeComponent();
            RunSimplexFromForm1();
        }

        private void RunSimplexFromForm1()
        {
            Form1 form1 = Application.OpenForms["Form1"] as Form1;
            if (form1 == null) return;

            int totalVariables = int.Parse(form1.Controls.Find("textBox1", true)[0].Text);
            int totalConstraints = int.Parse(form1.Controls.Find("textBox2", true)[0].Text);

            double[] objective = new double[totalVariables];
            for (int i = 0; i < totalVariables; i++)
            {
                string name = $"VarZ{i + 1}";
                var ctrl = form1.Controls.Find("panel2", true)[0].Controls.Find(name, true).FirstOrDefault();
                objective[i] = double.Parse((ctrl as TextBox)?.Text ?? "0");
            }

            double[,] constraints = new double[totalConstraints, totalVariables + 1];
            string[] types = new string[totalConstraints];
            for (int i = 0; i < totalConstraints; i++)
            {
                for (int j = 0; j < totalVariables; j++)
                {
                    string name = $"EquVar{i + 1}_{j + 1}";
                    var ctrl = form1.Controls.Find("panel1", true)[0].Controls.Find(name, true).FirstOrDefault();
                    constraints[i, j] = double.Parse((ctrl as TextBox)?.Text ?? "0");
                }

                string signName = $"signValue{i + 1}";
                string rhsName = $"EquValue{i + 1}";

                var signCtrl = form1.Controls.Find("panel1", true)[0].Controls.Find(signName, true).FirstOrDefault();
                types[i] = (signCtrl as ComboBox)?.SelectedItem.ToString() ?? "=";

                var rhsCtrl = form1.Controls.Find("panel1", true)[0].Controls.Find(rhsName, true).FirstOrDefault();
                constraints[i, totalVariables] = double.Parse((rhsCtrl as TextBox)?.Text ?? "0");
            }

            string result = RunSimplexWithBigM(objective, constraints, types, totalVariables, totalConstraints);
            this.Controls.Find("textBox3", true)[0].Text = result; // Assuming textBox3 is the output box
        }

        private string RunSimplexWithBigM(double[] obj, double[,] cons, string[] types, int vars, int consCount)
        {
            var output = new StringBuilder();
            int extraCols = 0;
            foreach (string t in types)
                extraCols += (t == "<=" ? 1 : (t == "=" ? 1 : 2));

            int totalCols = vars + extraCols + 1;
            int totalRows = consCount + 1;

            double[,] tableau = new double[totalRows, totalCols];
            int currCol = vars;
            List<int> artificialCols = new List<int>();

            for (int i = 0; i < consCount; i++)
            {
                for (int j = 0; j < vars; j++)
                    tableau[i, j] = cons[i, j];

                switch (types[i])
                {
                    case "<=":
                        tableau[i, currCol++] = 1;
                        break;
                    case ">=":
                        tableau[i, currCol++] = -1;
                        tableau[i, currCol] = 1;
                        artificialCols.Add(currCol++);
                        break;
                    case "=":
                        tableau[i, currCol] = 1;
                        artificialCols.Add(currCol++);
                        break;
                }

                tableau[i, totalCols - 1] = cons[i, vars];
            }

            for (int j = 0; j < vars; j++)
                tableau[totalRows - 1, j] = -obj[j];

            foreach (int col in artificialCols)
                tableau[totalRows - 1, col] = -BigM;

            output.AppendLine("--- Initial Tableau ---");
            output.Append(PrintTableau(tableau));

            int iteration = 0;
            while (true)
            {
                int pivotCol = -1;
                double min = 0;
                for (int j = 0; j < totalCols - 1; j++)
                {
                    if (tableau[totalRows - 1, j] < min)
                    {
                        min = tableau[totalRows - 1, j];
                        pivotCol = j;
                    }
                }

                if (pivotCol == -1)
                {
                    output.AppendLine("\r\nOptimal solution found.");
                    break;
                }

                double minRatio = double.MaxValue;
                int pivotRow = -1;
                for (int i = 0; i < totalRows - 1; i++)
                {
                    if (tableau[i, pivotCol] > 0)
                    {
                        double ratio = tableau[i, totalCols - 1] / tableau[i, pivotCol];
                        if (ratio < minRatio)
                        {
                            minRatio = ratio;
                            pivotRow = i;
                        }
                    }
                }

                if (pivotRow == -1)
                {
                    output.AppendLine("\r\nUnbounded solution.");
                    break;
                }

                double pivot = tableau[pivotRow, pivotCol];
                for (int j = 0; j < totalCols; j++)
                    tableau[pivotRow, j] /= pivot;

                for (int i = 0; i < totalRows; i++)
                {
                    if (i != pivotRow)
                    {
                        double factor = tableau[i, pivotCol];
                        for (int j = 0; j < totalCols; j++)
                            tableau[i, j] -= factor * tableau[pivotRow, j];
                    }
                }

                output.AppendLine($"\r\n--- Iteration {++iteration} ---");
                output.Append(PrintTableau(tableau));
            }

            return output.ToString();
        }

        private string PrintTableau(double[,] table)
        {
            int rows = table.GetLength(0);
            int cols = table.GetLength(1);
            var result = new StringBuilder();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    result.Append($"{table[i, j],10:F2}");
                result.AppendLine();
            }
            return result.ToString();
        }
    }
}
