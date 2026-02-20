using System;

namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public sealed class FiltroPesquisaRelatorioOcorrenciaCentroCusto
    {
        public int CodigoCarga { get; set; }

        public int CodigoTransportador { get; set; }

        public DateTime? DataOcorrenciaInicial { get; set; }

        public DateTime? DataOcorrenciaLimite { get; set; }

        public int NumeroOcorrencia { get; set; }
    }
}
