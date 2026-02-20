using Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170
{
    public class envMacro
    {
        public int? idIdent { get; set; }

        public int? idTecnologia { get; set; }

        public string mct { get; set; }

        public string dataHora { get; set; }

        public decimal latitude { get; set; }

        public decimal longitude { get; set; }

        public int? idMacro { get; set; }

        public string textoMacro { get; set; }
    }
}