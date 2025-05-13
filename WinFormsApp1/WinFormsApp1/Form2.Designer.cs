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
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();

            // 
            // textBox3
            // 
            this.textBox3.Multiline = true;
            this.textBox3.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox3.Font = new System.Drawing.Font("Consolas", 10F);
            this.textBox3.Location = new System.Drawing.Point(12, 12);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(760, 537);
            this.textBox3.TabIndex = 0;
            this.textBox3.WordWrap = false;

            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.textBox3);
            this.Name = "Form2";
            this.Text = "Solution";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
