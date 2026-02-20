using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public sealed class FiltroPesquisaIntegracaoLotePagamento
    {
        public int CodigoCarga { get; set; }
        public int CodigoCTe { get; set; }
        public int NumeroPagamento { get; set; }
        public List<int> CodigosFilial { get; set; }
        public List<int> CodigosTransportador { get; set; }
        public DateTime DataInicialEmissaoDocumento { get; set; }
        public DateTime DataFinalEmissaoDocumento { get; set; }
        public SituacaoPagamento? SituacaoPagamento { get; set; }
        public SituacaoIntegracao? SituacaoIntegracao { get; set; }
        public SituacaoCarga? SituacaoCarga { get; set; }
        public bool ExibirUltimoRegistroQuandoExistirProtocoloCTeDuplicado { get; set; }
    }
}
