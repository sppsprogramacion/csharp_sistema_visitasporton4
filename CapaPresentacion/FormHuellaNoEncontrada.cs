using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class FormHuellaNoEncontrada : Form
    {
        public FormHuellaNoEncontrada()
        {
            InitializeComponent();
        }

        private void FormHuellaNoEncontrada_Load(object sender, EventArgs e)
        {
            lblMensaje.Text = "NO SE ENCONTRO UN CIUDADANO REGISTRADO CON ESTA HUELLA.\n\n"
                +"Controle nuevamente o siga las directivas especificadas para este caso.";
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
