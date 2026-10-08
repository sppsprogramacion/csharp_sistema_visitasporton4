using CapaDatos;
using CapaNegocio;
using CapaPresentacion.Biometria;
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
    public partial class FormControlPorton4 : Form
    {
        //para huellas
        private FingerprintCapture fingerprintCapture;
        private FingerprintProcessor fingerprintProcessor;
        private FingerprintTemplate fingerprintTemplate;
        private FingerprintVerifier fingerprintVerifier;
        private DPFP.Template templateRegistrado;
        private byte[] templateBytesRegistrado;
        private bool modoVerificacion = false;
        private bool modoIdentificacion = false;
        private string huellaBase64Global = "";

        public FormControlPorton4()
        {
            InitializeComponent();

            fingerprintCapture = new FingerprintCapture();
            fingerprintProcessor = new FingerprintProcessor();
            fingerprintTemplate = new FingerprintTemplate();
            fingerprintVerifier = new FingerprintVerifier();

            fingerprintCapture.FingerDetected += FingerprintCapture_FingerDetected;
            fingerprintCapture.FingerRemoved += FingerprintCapture_FingerRemoved;
            fingerprintCapture.CaptureError += FingerprintCapture_CaptureError;
            fingerprintCapture.SampleCaptured += FingerprintCapture_SampleCaptured;
        }

        private void FormControlPorton4_Load(object sender, EventArgs e)
        {
            //habilitar lector
            fingerprintCapture.Start();
            modoIdentificacion = true;
            picHuella.Visible = true;
            lblLectorEstado.Text = "Coloque el dedo para verificar.";
            lblLectorDedo.Text = "Esperando huella...";

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.InicializarContrles();
        }

        //CONTROL EDAD
        private void ControlEdad(int edad)
        {
            if (edad < 18)
            {
                lblCategoriaEdad.Text = "Edad: " + edad + " años. Es MENOR.";
            }
            else
            {
                lblCategoriaEdad.Text = "Edad: " + edad + " años. Es ADULTO.";
            }
        }
        //FIN CONTROL EDAD
        //-------------------------------------------------------------------------------------

        //CONTROL TIENE DISCAPACIDAD
        private void ControlTieneDiscapacidad(bool tieneDiscapacidad, string detalle)
        {
            if (tieneDiscapacidad)
            {
                lblDiscapacidad.Text = "TIENE DISCAPACIDAD. " + detalle;
            }
            else
            {
                lblDiscapacidad.Text = "NO TIENE DISCAPACIDAD";
            }
        }
        //FIN CONTROL TIENE DISCAPACIDAD
        //------------------------------------------------------------------------------------------

        //BLOQUEAR DEDOS SEGUN HUELLA CARGADA
        private void bloquearChecksHuellasCargadas(List<DHuella> listaHuellas)
        {

            if (listaHuellas.Count == 0)
            {
                MessageBox.Show("El ciudadano no posee huellas registradas.", "Sistema Visistas", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            foreach (DHuella huella in listaHuellas)
            {
                int dedo = Convert.ToInt32(huella.dedo_id);
                //MessageBox.Show (dedo);
                switch (dedo)
                {
                    case 1:
                        opPD.BackColor = Color.Green;
                        break;

                    case 2:

                        opID.BackColor = Color.Green;
                        break;

                    case 3:

                        opMAD.BackColor = Color.Green;
                        break;

                    case 4:

                        opAD.BackColor = Color.Green;
                        break;

                    case 5:

                        opMED.BackColor = Color.Green;
                        break;

                    case 6:

                        opPI.BackColor = Color.Green;
                        break;

                    case 7:

                        opII.BackColor = Color.Green;
                        break;

                    case 8:

                        opMAI.BackColor = Color.Green;
                        break;

                    case 9:

                        opAI.BackColor = Color.Green;
                        break;

                    case 10:

                        opMEI.BackColor = Color.Green;
                        break;

                    default:
                        break;


                }//fin switch
            }//fin foreach
        }
        //FIN PRocedimiento para bloquear dedos segun huella cargada
        //-------------------------------------------------------------------------------

        //BLOQUEAR DEDOS SEGUN HUELLA CARGADA
        //----------------------------------------------------------------------------------------

        //INICIALIZAR CONTROLES
        private void InicializarContrles()
        {
            //DATOS DEL CIUDADANO
            lblApellidoNombre.Text = "Apellido y nombre"; ;
            picFotoVisita.Image = null;
            txtDni.Text = string.Empty; ;
            txtSexo.Text = string.Empty; ;
            txtFechaNacimiento.Text = string.Empty; ;
            txtEdad.Text = string.Empty; ;

            lblCategoriaEdad.Text = "Categoria - edad";
            lblDiscapacidad.Text = "Discapacidad";

            //DATOS DE INGRESO
            txtNumeroFicha.Text = string.Empty;
            txtIdIngreso.Text = string.Empty;

            txtFechaIngreso.Text = string.Empty;
            txtHoraIngreso.Text = string.Empty;

            txtIntrno.Text = string.Empty;
            txtParentesco.Text = string.Empty;

            opPD.BackColor = Color.White;
            opID.BackColor = Color.White;
            opMAD.BackColor = Color.White;
            opAD.BackColor = Color.White;
            opMED.BackColor = Color.White;
            opPI.BackColor = Color.White;
            opII.BackColor = Color.White;
            opMAI.BackColor = Color.White;
            opAI.BackColor = Color.White;
            opMEI.BackColor = Color.White;

            dtgMenores.DataSource = null;
        }
        //FINALIZAR INICIALIZAR CONTROLES
        //----------------------------------------------------------------------------


        //BUSCAR Y CARGAR DATOS DE ENTRADA
        private async void CargarEntrada(int idCiudadano)
        {
             EjecutarEnUI(async () =>
            {
                this.Enabled = false;
                NEntradaSalida nEntradaSalida = new NEntradaSalida();
                (DEntradaSalidaControl dCiudadanoIngresoResponse, string errorResponse) = await nEntradaSalida.BuscarEntradaControlXCiudadano(idCiudadano);
                this.Enabled = true;


                if (dCiudadanoIngresoResponse == null)
                {
                    //MessageBox.Show(errorResponse, "Sistema Visitas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //MessageBox.Show("Buscando identidad de la visita", "Sistema Visitas", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    FormHuellasEncontrado formhuellasencontrado = new FormHuellasEncontrado(idCiudadano);
                    formhuellasencontrado.ShowDialog();
                    return;
                }

                //CARGAR DATOS DEL CIUDADANO
                lblApellidoNombre.Text = dCiudadanoIngresoResponse.nombre_visita;
                picFotoVisita.Load(dCiudadanoIngresoResponse.foto_visita);
                txtDni.Text = dCiudadanoIngresoResponse.dni_visita.ToString();
                txtSexo.Text = dCiudadanoIngresoResponse.sexo_visita;
                txtFechaNacimiento.Text = dCiudadanoIngresoResponse.fecha_nacimiento_visita.ToShortDateString();
                txtEdad.Text = dCiudadanoIngresoResponse.edad_visita.ToString();

                //CARGAR DATOS DE INGRESO
                txtNumeroFicha.Text = dCiudadanoIngresoResponse.numero_ficha.ToString();
                txtIdIngreso.Text = dCiudadanoIngresoResponse.id_entrada_salida.ToString();
                txtNumeroFicha.Text = dCiudadanoIngresoResponse.numero_ficha.ToString();
                txtParentesco.Text = dCiudadanoIngresoResponse.parentesco;
                txtIntrno.Text = dCiudadanoIngresoResponse.nombre_interno;
                txtFechaIngreso.Text = dCiudadanoIngresoResponse.fecha_registro.ToShortDateString();
                txtHoraIngreso.Text = dCiudadanoIngresoResponse.hora_registro;
                txtHoraEgreso.Text = dCiudadanoIngresoResponse.hora_egreso;


                this.ControlTieneDiscapacidad(dCiudadanoIngresoResponse.tiene_discapacidad, dCiudadanoIngresoResponse.discapacidad_detalle);
                this.ControlEdad(dCiudadanoIngresoResponse.edad_visita);

                //Cargar Huellas
                this.bloquearChecksHuellasCargadas(dCiudadanoIngresoResponse.huellasCiudadanoResponse);

                //Cargar menores
                var datosfiltradosMenores = dCiudadanoIngresoResponse.menoresIngresadosResponse
                    .Select(c => new
                    {
                        Id = c.id_ciudadano,
                        ApellidoNombre = c.nombre_menor,
                        Dni = c.dni,
                        Sexo = c.sexo,
                        Edad = c.edad,
                    })
                    .ToList();
                dtgMenores.DataSource = datosfiltradosMenores;


                //dimensionar columnas
                if (dCiudadanoIngresoResponse.menoresIngresadosResponse.Count > 0)
                {                    
                    dtgMenores.Columns["Id"].Width = 40;
                    dtgMenores.Columns["ApellidoNombre"].Width = 300;
                    dtgMenores.Columns["Dni"].Width = 80;
                    dtgMenores.Columns["Sexo"].Width = 80;
                    dtgMenores.Columns["Edad"].Width = 50;
                }
            });
        }
        //FIN BUSCAR Y CARGAR DATOS DE ENTRADA
        //--------------------------------------------------------------------------------


        //------------------------------------------------------
        //METODOS PARA HUELLAS
        //------------------------------------------------------
        #region Metodos para huellas
        private void FingerprintCapture_FingerDetected(
            object sender,
            EventArgs e)
        {
            EjecutarEnUI(() =>
            {
                lblLectorDedo.Text = "Dedo detectado";
            });
        }

        private void FingerprintCapture_FingerRemoved(
            object sender,
            EventArgs e)
        {
            EjecutarEnUI(() =>
            {
                lblLectorDedo.Text = "Dedo retirado";
            });
        }

        private void FingerprintCapture_CaptureError(object sender, string e)
        {
            MessageBox.Show(
                e,
                "Error del lector",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }


        //METODO DE CAPTURA PARA REGISTRO Y/O VERIFICACION
        private async void FingerprintCapture_SampleCaptured(object sender, FingerprintCapture.SampleEventArgs e)
        {
            try
            {
                DPFP.FeatureSet featureSet;

                bool resultado;

                //if (modoVerificacion || modoIdentificacion)
                //{
                resultado = fingerprintProcessor.ExtractFeaturesForVerification(
                        e.Sample,
                        out featureSet
                    );
                //}
                //else
                //{
                //    resultado = fingerprintProcessor.ExtractFeaturesForEnrollment(
                //            e.Sample,
                //            out featureSet
                //        );
                //}

                if (!resultado)
                {
                    EjecutarEnUI(() =>
                    {
                        lblLectorEstado.Text = "La calidad de la huella no es suficiente.";
                    });

                    return;
                }


                // -----------------------------------------
                // MODO VERIFICACIÓN
                // -----------------------------------------

                if (modoVerificacion)
                {
                    NHuella nHuellas = new NHuella();
                    //MessageBox.Show("Verificando huella", "Sistema Visistas", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    (List<DHuella> listaHuellas, string errorResponse) = await nHuellas.RetornarListaXCiudadano(0);
                    if (listaHuellas == null)
                    {
                        MessageBox.Show(errorResponse, "Sistema Visistas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (listaHuellas.Count == 0)
                    {
                        MessageBox.Show("El ciudadano no posee huellas registradas.", "Sistema Visistas", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        return;
                    }

                    foreach (DHuella huella in listaHuellas)
                    {
                        try
                        {
                            byte[] templateBytes = Convert.FromBase64String(huella.huella);

                            DPFP.Template template = fingerprintTemplate.LoadTemplate(templateBytes);

                            if (fingerprintVerifier.Verify(featureSet, template))
                            {
                                EjecutarEnUI(() =>
                                {
                                    gboxDatosParaIngreso.Enabled = true;

                                });

                                MessageBox.Show($"IDENTIDAD VERIFICADA.\nPuede continuar con el ingreso.", "Sistema Visitas", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            // registrar error si quieres
                            continue;
                        }
                    }

                    MessageBox.Show("NO SE VERIFICO LA IDENTIDAD CON ESTA HUELLA. \n\nSE PROCEDE A IDENTIFICACION", "Sistema Visitas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    modoIdentificacion = true;
                    //return;

                }

                // -----------------------------------------
                // MODO IDENTIFICACION
                // -----------------------------------------

                if (modoIdentificacion)
                {
                    //MessageBox.Show("Identificando huella", "Sistema Visitas", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    DSQLite sqlite = new DSQLite();

                    sqlite.Inicializar();
                    //MessageBox.Show("Inicializado", "Sistema Visitas", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //SINCRONIZACION
                    NHuella nHuella = new NHuella();

                    EjecutarEnUI(() =>
                    {
                        this.Enabled = false;

                    });
                    //MessageBox.Show("inicia sincronizacion", "Sistema Visitas", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    (bool estadoResponse, string errorResponse) = await nHuella.Sincronizar();
                    EjecutarEnUI(() =>
                    {
                        this.Enabled = true;

                    });

                    //MessageBox.Show("sincronizado", "Sistema Visitas", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (estadoResponse == false)
                    {
                        MessageBox.Show(errorResponse, "Sistema Visitas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }


                    //FIN SINCRONIZACION 

                    List<DHuellaLocal> listaHuellas = sqlite.ObtenerTodasLasHuellas();

                    if (listaHuellas.Count == 0)
                    {
                        MessageBox.Show("No hay huellas registradas.", "Sistema Visitas", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        return;
                    }

                    //MessageBox.Show("Ya tengo las huellas de sqlite", "Sistema Visitas", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    foreach (DHuellaLocal huella in listaHuellas)
                    {
                        try
                        {

                            //byte[] templateBytes = Convert.FromBase64String(huella.huella);

                            //DPFP.Template template = fingerprintTemplate.LoadTemplate(templateBytes);
                            DPFP.Template template = fingerprintTemplate.LoadTemplate(huella.huella);

                            if (fingerprintVerifier.Verify(featureSet, template))
                            {
                                //MessageBox.Show($"COINCIDENCIA ENCONCTRADA CON ESTA HUELLA", "Sistema Visitas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.CargarEntrada(huella.ciudadano_id);
                                //FormHuellasEncontrado formHuellasEncontrado = new FormHuellasEncontrado(huella.ciudadano_id);
                                //formHuellasEncontrado.ShowDialog();
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            // registrar error si quieres
                            continue;
                        }
                    }

                    //MessageBox.Show("NO SE ENCONTRO COINCIDENCIA DE ESTA HUELLA", "Sistema Visitas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //Cuando no se enocntro la huella
                    EjecutarEnUI(() =>
                    {
                        FormHuellaNoEncontrada formHuellaNoEncontrado = new FormHuellaNoEncontrada();
                        formHuellaNoEncontrado.ShowDialog();
                        

                    });
                    return;

                }


                // -----------------------------------------
                // MODO REGISTRO - formacion del template
                // -----------------------------------------

                bool agregada = fingerprintTemplate.AddFeatures(featureSet);

                uint faltantes = fingerprintTemplate.FeaturesNeeded;


                EjecutarEnUI(() =>
                {
                    if (fingerprintTemplate.IsComplete)
                    {
                        // Obtiene el Template original.
                        templateRegistrado = fingerprintTemplate.GetTemplate();

                        // Lo convierte a bytes.
                        templateBytesRegistrado = fingerprintTemplate.GetTemplateBytes();
                        string huellaBase64 = Convert.ToBase64String(templateBytesRegistrado);
                        this.huellaBase64Global = huellaBase64;

                        EjecutarEnUI(() =>
                        {
                            if (huellaBase64 != "")
                            {
                                lblLectorEstado.Text = "Template generado correctamente.";
                            }
                            else
                            {
                                lblLectorEstado.Text = "Error al construir el Template.";
                            }
                        });

                    }
                    else if (agregada)
                    {
                        lblLectorEstado.Text = "Captura correcta. Faltan " + faltantes + " muestras.";
                    }
                    else
                    {
                        lblLectorEstado.Text = "La muestra no fue aceptada. " + "Coloque nuevamente el dedo.";
                    }
                });
            }
            catch (Exception ex)
            {
                EjecutarEnUI(() =>
                {
                    lblLectorEstado.Text = "Error: " + ex.Message;
                });
            }
        }
        //FIN METODO DE CAPTURA PARA REGISTRO Y/O VERIFICACION
        //-------------------------------------------------------------------------------------------


        protected override void OnFormClosing(
            FormClosingEventArgs e)
        {
            fingerprintCapture?.Dispose();

            base.OnFormClosing(e);
        }

        //PERMITE ACCEDER A CONTROLES DESDE UN METODO QUE NO PODRIA
        private void EjecutarEnUI(Action accion)
        {
            if (InvokeRequired)
            {
                Invoke(accion);
                return;
            }

            accion();
        }

        private void gboxVisita_Enter(object sender, EventArgs e)
        {

        }
        

        #endregion Metodos para huellas
        //----------------------------------------------------------
        //FIN METODOS PARA HUELLAS
        //----------------------------------------------------------

    }
}
