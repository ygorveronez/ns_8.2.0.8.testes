using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus
{
    public class retornoWebServiceToken
    {
        public bool success { get; set; }
        public Data data { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public class Data
        {
            public string token { get; set; }
        }

    }
}