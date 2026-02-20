using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class Planejamento
    {
        public DateTime DataCarregamento { get; set; }
        public DateTime? DataInicioViagem { get; set; }
        public DateTime? DataInicioViagemPrevista { get; set; }
        public DateTime? DataFimViagemPrevista { get; set; }
        public DateTime? DataFimViagem { get; set; }
        public int CodigoCarga { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public string Origens { get; set; }
        public string Destinos { get; set; }
        public int LocalidadeInicio { get; set; }
        public int LocalidadeFim { get; set; }
        public string cidadeInicio { get; set; }
        public string estadoInicio { get; set; }
        public int paisInicio { get; set; }
        public string cidadeFim { get; set; }
        public string estadoFim { get; set; }
        public int paisFim { get; set; }

    }
}
