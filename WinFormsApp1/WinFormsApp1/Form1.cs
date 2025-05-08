using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        /*
        textBox1= Total Variables
        textBox2 = Total Constraints
        VarZ(j+1)= variable ni Z
        EquVar{i + 1}_{j + 1} = Variables nung equation
        signValue(i+1) = value kung <= >= or = 
        EquValue(i+1) = contraints ni equ
        */

        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "3";
            textBox2.Text = "3";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            int totalVariable = int.Parse(textBox1.Text);
            int totalConstraints = int.Parse(textBox2.Text);
            mainSolution(totalVariable);
            equPanel(totalVariable, totalConstraints);

        }

        //para number lang input
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        //para number lang input
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void mainSolution(int totalVar)
        {
            panel2.Controls.Clear();
            int horizontalSpacing = 80;
            int verticalSpacing = 40;

            Label z = new Label();
            z.Text = "Z =";
            z.Location = new Point(10, 35);
            z.AutoSize = true;
            panel2.Controls.Add(z);

            for (int j = 0; j < totalVar; j++)
            {
                TextBox txtBox = new TextBox();
                txtBox.Name = $"VarZ{j + 1}";
                txtBox.Width = 45;
                txtBox.Location = new Point(10 + (j * horizontalSpacing) + z.Width, 30);
                panel2.Controls.Add(txtBox);
                Label varLabel = new Label();
                if (j == totalVar - 1)
                {
                    varLabel.Text = $"x{j + 1}";
                }
                else
                {
                    varLabel.Text = $"x{j + 1} + ";
                }
                varLabel.Location = new Point(txtBox.Location.X + txtBox.Width + 5, txtBox.Location.Y + 8);
                varLabel.AutoSize = true;
                panel2.Controls.Add(varLabel);
            }


            int totalWidth = (totalVar * horizontalSpacing) + 120 + z.Width;
            int totalHeight = verticalSpacing + 60;
            panel2.Size = new Size(totalWidth, totalHeight);
            panel2.AutoScroll = true;
        }




        private void equPanel(int totalVar, int totalConst)
        {
            panel1.Controls.Clear();
            int maxWidth = 0;
            int horizontalSpacing = 80;
            int verticalSpacing = 40;

            for (int i = 0; i < totalConst; i++)
            {
                for (int j = 0; j < totalVar; j++)
                {
                    TextBox txtBox = new TextBox();
                    txtBox.Name = $"EquVar{i + 1}_{j + 1}";
                    txtBox.Location = new Point(10 + (j * horizontalSpacing), (i * verticalSpacing) + 30);
                    txtBox.Width = 45;
                    panel1.Controls.Add(txtBox);
                    Label varLabel = new Label();
                    if (j == totalVar - 1)
                    {
                        varLabel.Text = $"x{j + 1}";
                    }
                    else
                    {
                        varLabel.Text = $"x{j + 1} + ";
                    }
                    varLabel.Location = new Point(txtBox.Location.X + txtBox.Width + 5, txtBox.Location.Y + 8);
                    varLabel.AutoSize = true;
                    panel1.Controls.Add(varLabel);
                }
                ComboBox comboBox = new ComboBox();
                comboBox.Name = $"signValue{i + 1}";
                comboBox.Location = new Point((totalVar * horizontalSpacing) + 10, (i * verticalSpacing) + 30);
                comboBox.Width = 40;
                comboBox.Items.Add("<=");
                comboBox.Items.Add(">=");
                comboBox.Items.Add("=");
                comboBox.SelectedIndex = 0;
                panel1.Controls.Add(comboBox);

                TextBox txtBoxValue = new TextBox();
                txtBoxValue.Name = $"EquValue{i + 1}";
                txtBoxValue.Location = new Point(comboBox.Location.X + comboBox.Width + 10, (i * verticalSpacing) + 30);
                txtBoxValue.Width = 45;
                panel1.Controls.Add(txtBoxValue);
            }

            int totalHeight = (verticalSpacing * totalConst) + 30;
            int totalWidth = maxWidth + (totalVar * horizontalSpacing) + 200;
            panel1.Size = new Size(totalWidth, totalHeight);
            panel1.AutoScroll = true;
        }




        private void button1_Click(object sender, EventArgs e)
        {
            int totalVariable = int.Parse(textBox1.Text);
            int totalConstraints = int.Parse(textBox2.Text);
            mainSolution(totalVariable);
            equPanel(totalVariable, totalConstraints);
            System.Diagnostics.Debug.WriteLine("Hello");
        }

        private void FindButton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out int totalVariable) || totalVariable <= 0)
            {
                MessageBox.Show("Total Variables must be a number greater than 0.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textBox2.Text, out int totalConstraints) || totalConstraints <= 0)
            {
                MessageBox.Show("Total Constraints must be a number greater than 0.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            for (int i = 0; i < totalVariable; i++)
            {
                string varName = $"VarZ{i + 1}";
                var control = panel2.Controls.Find(varName, true).FirstOrDefault();
                if (control is TextBox txtBox)
                {
                    if (!double.TryParse(txtBox.Text, out double value) || value <= 0)
                    {
                        MessageBox.Show($"Z Variable x{i + 1} must be a number greater than 0.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            for (int i = 0; i < totalConstraints; i++)
            {
                for (int j = 0; j < totalVariable; j++)
                {
                    string varName = $"EquVar{i + 1}_{j + 1}";
                    var control = panel1.Controls.Find(varName, true).FirstOrDefault();
                    if (control is TextBox txtBox)
                    {
                        if (!double.TryParse(txtBox.Text, out double value) || value <= 0)
                        {
                            MessageBox.Show($"Coefficient for Constraint {i + 1}, Variable x{j + 1} must be a number greater than 0.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }

                string constraintName = $"EquValue{i + 1}";
                var constraintControl = panel1.Controls.Find(constraintName, true).FirstOrDefault();
                if (constraintControl is TextBox constraintBox)
                {
                    if (!double.TryParse(constraintBox.Text, out double rhs) || rhs <= 0)
                    {
                        MessageBox.Show($"Constraint {i + 1} right-hand value must be a number greater than 0.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
            //palitan nung actual code for next page
            MessageBox.Show("OksGus", "Next", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
}
}
