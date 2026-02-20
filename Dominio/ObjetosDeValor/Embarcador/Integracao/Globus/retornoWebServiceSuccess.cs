using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus
{
    public class retornoWebServiceSuccess
    {
        public bool success { get; set; }
        public DataResponse data { get; set; }
        public int? CodigoDocumento { get; set; } 

    }
    public class DataResponse
    {
        public string idExterno { get; set; }
        public string idInterno { get; set; }
    }
}