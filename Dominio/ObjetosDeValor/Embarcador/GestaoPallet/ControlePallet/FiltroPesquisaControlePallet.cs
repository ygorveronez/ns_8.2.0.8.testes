using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet
{
    public sealed class FiltroPesquisaControlePallet
    {
        public int NotaFiscal { get; set; }

        public string Carga { get; set; }

        public int Filial { get; set; }

        public int Transportador { get; set; }

        public long Cliente { get; set; }

        public List<string> UFOrigem { get; set; }

        public List<string> UFDestino { get; set; }

        public string EscritorioVendas { get; set; }

        public DateTime DataInicialCriacaoCarga { get; set; }

        public DateTime DataFinalCriacaoCarga { get; set; }

        public DateTime DataInicialNotaFiscal { get; set; }

        public DateTime DataFinalNotaFiscal { get; set; }

        public SituacaoGestaoPallet? Situacao { get; set; }

        public ResponsavelPallet? ResponsavelPallet { get; set; }

        public int DiasLimiteParaDevolucao { get; set; }

        public DateTime DataLimiteGeracaoDevolucao { get; set; }

        public RegraPallet? RegraPallet { get; set; }
    }
}
