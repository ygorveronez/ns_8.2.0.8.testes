using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil
{
    public sealed class FiltroPesquisaFreteContabil
    {
        public int CodigoTransportador { get; set; }

        public double CpfCnpjCompanhia { get; set; }

        public double CpfCnpjEmitente { get; set; }

        public DateTime? DataEmissaoInicio { get; set; }

        public DateTime? DataEmissaoLimite { get; set; }

        public TipoContabilizacao TipoContabilizacao { get; set; }
    }
}
