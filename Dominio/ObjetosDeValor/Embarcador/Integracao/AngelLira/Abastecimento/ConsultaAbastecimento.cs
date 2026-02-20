using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Abastecimento
{
    public class ConsultaAbastecimento
    {
        public List<Abastecimento> veiculos { get; set; }
    }
    public class Abastecimento
    {
        public string Placa { get; set; }
        public string Condutor { get; set; }
        public string DataHora { get; set; }
        public string Coordenada { get; set; }
        public string Odometro { get; set; }
    }
}
