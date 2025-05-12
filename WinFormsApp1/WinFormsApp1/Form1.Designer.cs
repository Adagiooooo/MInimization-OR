namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            textBox1 = new TextBox();
            label2 = new Label();
            textBox2 = new TextBox();
            button1 = new Button();
            label3 = new Label();
            FindButton = new Button();
            flowZ = new FlowLayoutPanel();
            flowConst = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(362, 37);
            label1.Name = "label1";
            label1.Size = new Size(123, 23);
            label1.TabIndex = 0;
            label1.Text = "Total Variables:";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(491, 33);
            textBox1.Margin = new Padding(3, 4, 3, 4);
            textBox1.MaxLength = 1;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(91, 27);
            textBox1.TabIndex = 1;
            textBox1.TextAlign = HorizontalAlignment.Center;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(753, 37);
            label2.Name = "label2";
            label2.Size = new Size(141, 23);
            label2.TabIndex = 2;
            label2.Text = "Total Constraints:";
            // 
            // textBox2
            // 
            textBox2.ImeMode = ImeMode.NoControl;
            textBox2.Location = new Point(900, 37);
            textBox2.Margin = new Padding(3, 4, 3, 4);
            textBox2.MaxLength = 1;
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(91, 27);
            textBox2.TabIndex = 3;
            textBox2.TextAlign = HorizontalAlignment.Center;
            textBox2.KeyPress += textBox2_KeyPress;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.Location = new Point(1067, 37);
            button1.Margin = new Padding(3, 4, 3, 4);
            button1.Name = "button1";
            button1.Size = new Size(110, 31);
            button1.TabIndex = 4;
            button1.Text = "Generate";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(362, 195);
            label3.Name = "label3";
            label3.Size = new Size(273, 32);
            label3.TabIndex = 0;
            label3.Text = "Subject to Constraints:";
            // 
            // FindButton
            // 
            FindButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            FindButton.Location = new Point(1067, 605);
            FindButton.Margin = new Padding(3, 4, 3, 4);
            FindButton.Name = "FindButton";
            FindButton.Size = new Size(110, 31);
            FindButton.TabIndex = 7;
            FindButton.Text = "Find";
            FindButton.UseVisualStyleBackColor = true;
            FindButton.Click += FindButton_Click;
            // 
            // flowZ
            // 
            flowZ.AutoSize = true;
            flowZ.Location = new Point(362, 82);
            flowZ.Name = "flowZ";
            flowZ.Size = new Size(815, 94);
            flowZ.TabIndex = 8;
            // 
            // flowConst
            // 
            flowConst.AutoSize = true;
            flowConst.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowConst.Location = new Point(362, 253);
            flowConst.MinimumSize = new Size(100, 100);
            flowConst.Name = "flowConst";
            flowConst.Size = new Size(100, 100);
            flowConst.TabIndex = 9;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1579, 1033);
            Controls.Add(flowConst);
            Controls.Add(flowZ);
            Controls.Add(FindButton);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(textBox2);
            Controls.Add(label2);
            Controls.Add(textBox1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Operations Research Minimization";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private Label label2;
        private TextBox textBox2;
        private Button button1;
        private Label label3;
        private Button FindButton;
        private FlowLayoutPanel flowZ;
        private FlowLayoutPanel flowConst;
    }
}
