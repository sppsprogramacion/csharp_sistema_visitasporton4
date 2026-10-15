using CapaDatos;
using CapaNegocio;
using CapaPresentacion.FuncionesGenerales;
using CommonCache;
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
    public partial class FormPrincipal : Form
    {
        public FormPrincipal()
        {
            InitializeComponent();
        }

        private async void FormPrincipal_Load(object sender, EventArgs e)
        {
            FormularioAyudas.AjustarFormulario(this);
            this.ControlBox = false;

            lblEncabezado.Text = lblEncabezado.Text + " - " + CurrentUser.Instance.organismo;
            lblUsuario.Text = CurrentUser.Instance.nombre.ToUpper() + " " + CurrentUser.Instance.apellido.ToUpper();

            //INICIAR SQLLITE
            try
            {
                DSQLite sqlite = new DSQLite();

                sqlite.Inicializar();

                //SINCRONIZACION
                NHuella nHuella = new NHuella();

                this.Enabled = false;
                (bool estadoResponse, string errorResponse) = await nHuella.Sincronizar();
                this.Enabled = true;

                if (estadoResponse == false)
                {
                    MessageBox.Show(errorResponse, "Sistema Visitas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //FIN SINCRONIZACION 
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message
                );
            }

            //OBTENER COMPUTO
            this.ObtenerComputo();
        }


        private void btnCerrarSistema_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        

        private void btnIngresoVisita_Click(object sender, EventArgs e)
        {
            FormIngresoVisitas formIngresoVisitas = new FormIngresoVisitas();

            formIngresoVisitas.ShowDialog();
        }

        private void btnEgresoVisitas_Click(object sender, EventArgs e)
        {
            FormEgresoVisita formEgresoVisitas = new FormEgresoVisita();

            formEgresoVisitas.ShowDialog();
        }
                

        private void btnControlAcceso_Click(object sender, EventArgs e)
        {
            FormControlPorton4 formControl = new FormControlPorton4();

            formControl.ShowDialog();
        }

        private void btnVerIngresos_Click(object sender, EventArgs e)
        {
            FormReimprimirFicha formReimprimirFicha = new FormReimprimirFicha();

            formReimprimirFicha.ShowDialog();
        }

        private void btnActualizarComputo_Click(object sender, EventArgs e)
        {
            this.ObtenerComputo();
        }

        //CONTAR INGRESOS
        private async void ObtenerComputo()
        {
            NEntradaSalida nEntradaSalida = new NEntradaSalida();

            (List<DEntradaSalidaComputo> listaComputo, string errorResponse) = await nEntradaSalida.ListaComputoXOrganismo();


            if (listaComputo == null)
            {
                if (errorResponse != null)
                {
                    MessageBox.Show(errorResponse, "Sistema Visitas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                MessageBox.Show("No se pudo actualizar el computo", "Judiciales", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            int totalGeneral = listaComputo.Sum(x => x.cantidad);

            lblTotalTotales.Text = totalGeneral.ToString();

            

            lblAdultosMasculinos.Text = Convert.ToString(listaComputo
                .FirstOrDefault(x =>
                    x.categoria == "ADULTO" &&
                    x.sexo_id == 2)
                    ?.cantidad ?? 0);

            lblAdultosFemeninas.Text = Convert.ToString(listaComputo
                .FirstOrDefault(x =>
                    x.categoria == "ADULTO" &&
                    x.sexo_id == 1)
                    ?.cantidad ?? 0);
            lblAdultosNoBinarios.Text = Convert.ToString(listaComputo
                .FirstOrDefault(x =>
                    x.categoria == "ADULTO" &&
                    x.sexo_id == 3)
                    ?.cantidad ?? 0);

            
            lblAdultosTotales.Text = Convert.ToString(listaComputo
                .Where(x => x.categoria == "ADULTO")
                .Sum(x => x.cantidad));

            
            lblMenoresMasculinos.Text = Convert.ToString(listaComputo
                .FirstOrDefault(x =>
                    x.categoria == "MENOR" &&
                    x.sexo_id == 2)
                    ?.cantidad ?? 0);

            lblMenoresFemeninas.Text = Convert.ToString(listaComputo
                .FirstOrDefault(x =>
                    x.categoria == "MENOR" &&
                    x.sexo_id == 1)
                    ?.cantidad ?? 0);

            lblMenoresNoBinario.Text = Convert.ToString(listaComputo
                .FirstOrDefault(x =>
                    x.categoria == "MENOR" &&
                    x.sexo_id == 3)
                    ?.cantidad ?? 0);

            lblMenoresTotales.Text = Convert.ToString(listaComputo
                .Where(x => x.categoria == "MENOR")
                .Sum(x => x.cantidad));


            lblTotalesMasculinos.Text = Convert.ToString(Convert.ToInt32(lblAdultosMasculinos.Text) + Convert.ToInt32(lblMenoresMasculinos.Text));
            lblTotalesFemeninas.Text = Convert.ToString(Convert.ToInt32(lblAdultosFemeninas.Text) + Convert.ToInt32(lblMenoresFemeninas.Text));
            lblTotalesNoBinario.Text = Convert.ToString(Convert.ToInt32(lblAdultosNoBinarios.Text) + Convert.ToInt32(lblMenoresNoBinario.Text));



        }

        private void btnImprimirComputo_Click(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void lblTotalTotales_Click(object sender, EventArgs e)
        {

        }

        private void lblTotalesNoBinario_Click(object sender, EventArgs e)
        {

        }

        private void lblTotalesFemeninas_Click(object sender, EventArgs e)
        {

        }

        private void lblTotalesMasculinos_Click(object sender, EventArgs e)
        {

        }

        private void lblMenoresTotales_Click(object sender, EventArgs e)
        {

        }

        private void lblMenoresNoBinario_Click(object sender, EventArgs e)
        {

        }

        private void lblMenoresFemeninas_Click(object sender, EventArgs e)
        {

        }

        private void lblMenoresMasculinos_Click(object sender, EventArgs e)
        {

        }

        private void lblAdultosTotales_Click(object sender, EventArgs e)
        {

        }

        private void lblAdultosNoBinarios_Click(object sender, EventArgs e)
        {

        }

        private void lblAdultosFemeninas_Click(object sender, EventArgs e)
        {

        }

        private void lblAdultosMasculinos_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        //FIN CONTAR INGRESOS........................................................
    }
}
