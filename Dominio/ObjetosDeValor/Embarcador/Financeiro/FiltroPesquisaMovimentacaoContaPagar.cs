using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaMovimentacaoContaPagar
    {
        public DateTime? DataDocInicial { get; set; }
        public DateTime? DataDocFinal { get; set; }
        public DateTime? DataCompensacaoInicial { get; set; }
        public DateTime? DataCompensacaoFinal { get; set; }
        public int Transportador { get; set; }
        public TipoRegistro TipoArquivo { get; set; }
        public SituacaoProcessamento? SituacaoProcessamento { get; set; }
        public SituacaoDocumentoMovimentacao? SituacaoDocumentoMovimentacao { get; set; }
        public int CodigoNumeroDocumento { get; set; }
        public int NumeroTermoQuitacao { get; set; }
        public string DocumentoCompensacao { get; set; }
        public bool TodasFiliaisTransportador { get; set; }
        public List<int> CodigosFiliaisTransportador { get; set; }
    }
}
