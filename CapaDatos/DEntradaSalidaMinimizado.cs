using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class DEntradaSalidaMinimizado
    {
        public int id_entrada_salida { get; set; }
        public string numero_ficha { get; set; }
        public int ciudadano_id { get; set; }
        public DateTime fecha_registro { get; set; }
        public string hora_registro { get; set; }
        public string hora_egreso { get; set; }
        public string organismo { get; set; }
    }
}
