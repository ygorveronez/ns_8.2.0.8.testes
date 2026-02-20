using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet
{
    public class FiltroPesquisaControleSaldoPallet
    {
        public string NumeroNotaFiscal { get; set; }

        public string NumeroCarga { get; set; }

        public List<int> CodigosFilial { get; set; }

        public DateTime DataInicialCriacaoCarga { get; set; }

        public DateTime DataFinalCriacaoCarga { get; set; }

        public SituacaoGestaoPallet? SituacaoPallet { get; set; }

        public SituacaoPalletGestaoDevolucao? SituacaoPalletGestaoDevolucao { get; set; }

        public ResponsavelPallet? ResponsavelPallet { get; set; }

        public long CodigoCliente { get; set; }

        public int CodigoTransportador { get; set; }

        public int DiasLimiteParaDevolucao { get; set; }

        public DateTime DataLimiteGeracaoDevolucao { get; set; }
    }
}
