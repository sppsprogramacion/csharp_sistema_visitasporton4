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

        }


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
                                MessageBox.Show($"COINCIDENCIA ENCONCTRADA CON ESTA HUELLA", "Sistema Visitas", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                FormHuellasEncontrado formHuellasEncontrado = new FormHuellasEncontrado(huella.ciudadano_id);
                                formHuellasEncontrado.ShowDialog();
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            // registrar error si quieres
                            continue;
                        }
                    }

                    MessageBox.Show("NO SE ENCONTRO COINCIDENCIA DE ESTA HUELLA", "Sistema Visitas", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
