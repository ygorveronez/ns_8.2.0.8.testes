using Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170
{
    public class envOcorrencia
    {
        public string cpf { get; set; }

        public string placa { get; set; }

        public string status { get; set; }

        public string data_inicial { get; set; }

        public string data_final { get; set; }
    }
}