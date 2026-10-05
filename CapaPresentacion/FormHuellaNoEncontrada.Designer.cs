namespace CapaPresentacion
{
    partial class FormHuellaNoEncontrada
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.picAdvertencia = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picAdvertencia)).BeginInit();
            this.SuspendLayout();
            // 
            // picAdvertencia
            // 
            this.picAdvertencia.Location = new System.Drawing.Point(24, 64);
            this.picAdvertencia.Name = "picAdvertencia";
            this.picAdvertencia.Size = new System.Drawing.Size(222, 275);
            this.picAdvertencia.TabIndex = 0;
            this.picAdvertencia.TabStop = false;
            // 
            // FormHuellaNoEncontrada
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 543);
            this.Controls.Add(this.picAdvertencia);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormHuellaNoEncontrada";
            this.Text = "FormHuellaNoEncontrada";
            ((System.ComponentModel.ISupportInitialize)(this.picAdvertencia)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picAdvertencia;
    }
}