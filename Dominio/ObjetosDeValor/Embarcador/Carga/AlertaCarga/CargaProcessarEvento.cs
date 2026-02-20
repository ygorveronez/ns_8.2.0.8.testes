using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga
{
    public class CargaProcessarEvento
    {
        public int CodigoCarga { get; set; }
        public string CargaEmbarcador { get; set; }
        public DateTime? DataCriacaoCarga { get; set; }
        public DateTime? DataPrevisaoTerminoCarga { get; set; }
        public DateTime? DataInicioViagem { get; set; }
        public DateTime? DataInicioViagemPrevista { get; set; }
        public DateTime? DataCarregamentoCarga { get; set; }
        public DateTime? DataInicioCarregamentoJanela { get; set; }
        public DateTime? DataPrevisaoChegada { get; set; }
        public DateTime? DataPrevisaoChegadaPlanta { get; set; }
        public DateTime? DataLimiteCarregamento { get; set; }

    }

}
