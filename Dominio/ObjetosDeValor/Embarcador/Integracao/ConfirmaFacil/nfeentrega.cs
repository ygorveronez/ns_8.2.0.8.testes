using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil
{
    public class nfeentrega
    {
        public string embarcador { get; set; }

        public List<nfeentreganota> notas { get; set; }
    }

    public class nfeentreganota
    {
        public string data_entrega { get; set; }

        public string recebedor_nome { get; set; }

        public string recebedor_rg { get; set; }

        public string chave { get; set; }

        public string transportadora { get; set; }

        public decimal? latitude { get; set; }

        public decimal? longitude { get; set; }
    }
}
