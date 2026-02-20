using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.WMS
{
    public class FiltroPesquisaRelatorioSaldoArmazenamento
    {
        public int CodigoProdutoEmbarcador { get; set; }
        public int CodigoDeposito { get; set; }
        public int CodigoBloco { get; set; }
        public int CodigoPosicao { get; set; }
        public int CodigoRua { get; set; }
        public bool SaldoDisponivel { get; set; }
        public string CodigoBarras { get; set; }
        public string Descricao { get; set; }
        public DateTime DataVencimentoInicial { get; set; }
        public DateTime DataVencimentoFinal { get; set; }
        public TipoRecebimentoMercadoria? TipoRecebimento { get; set; }

    }
}
