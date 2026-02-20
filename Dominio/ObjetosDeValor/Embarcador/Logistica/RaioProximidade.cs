using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class RaioProximidade
    {

        public int Codigo { get; set; }
        public int Raio { get; set; }
        public string Identificacao { get; set; }
        public string Cor { get; set; }
        public bool GerarAlertaAutomaticoPorPermanencia { get; set; }
        public int Tempo { get; set; }
        public TipoAlerta TipoAlerta { get; set; }
    }
}
