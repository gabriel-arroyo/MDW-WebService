using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDW_WebService.Models
{
    public class ActivoLocalizado
    {
        public string nombre { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public string serie { get; set; }
        public string epc { get; set; }
        public string ubicacion { get; set; }
        public string ultimaLectura { get; set; }
        public ActivoLocalizado()
        {

        }
        public ActivoLocalizado(string _nombre, string _marca, string _modelo, string _serie, string _epc, string _ubicacion, string _ultimaLectura)
        {
            nombre = _nombre;
            marca = _marca;
            modelo = _modelo;
            serie = _serie;
            epc = _epc;
            ubicacion = _ubicacion;
            ultimaLectura = _ultimaLectura;
        }
    }
}