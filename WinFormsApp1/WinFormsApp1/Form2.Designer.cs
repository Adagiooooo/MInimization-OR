namespace WinFormsApp1
{
    partial class Form2
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox textBox3;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            textBox3 = new TextBox();
            SuspendLayout();
            // 
            // textBox3
            // 
            textBox3.Font = new Font("Consolas", 10F);
            textBox3.Location = new Point(10, 9);
            textBox3.Margin = new Padding(3, 2, 3, 2);
            textBox3.Multiline = true;
            textBox3.Name = "textBox3";
            textBox3.ReadOnly = true;
            textBox3.ScrollBars = ScrollBars.Both;
            textBox3.Size = new Size(666, 404);
            textBox3.TabIndex = 0;
            textBox3.WordWrap = false;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(686, 421);
            Controls.Add(textBox3);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form2";
            Text = "Solution";
            Load += Form2_Load;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
