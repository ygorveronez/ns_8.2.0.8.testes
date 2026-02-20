using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Fatura
{
    public class FiltroPesquisaFatura
    {
        public int NumeroFatura { get; set; }
        public int NumeroPreFatura { get; set; }
        public int Empresa { get; set; }
        public int GrupoPessoa { get; set; }
        public int NumeroCTe { get; set; }
        public int Operador { get; set; }
        public int TerminalOrigem { get; set; }
        public int TerminalDestino { get; set; }
        public int PedidoViagemNavio { get; set; }
        public int Origem { get; set; }
        public int Destino { get; set; }
        public int TipoOperacao { get; set; }
        public int NumeroNota { get; set; }
        public int CentroDeResultado { get; set; }
        public int Container { get; set; }
        public double Pessoa { get; set; }
        public double Tomador { get; set; }
        public string NumeroControleCliente { get; set; }
        public string NumeroReferenciaEDI { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroOS { get; set; }
        public string NumeroControle { get; set; }
        public string NumeroCarga { get; set; }
        public DateTime DataFatura { get; set; }
        public DateTime DataFaturaInicial { get; set; }
        public DateTime DataFaturaFinal { get; set; }
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public DateTime DataVencimentoInicial { get; set; }
        public DateTime DataVencimentoFinal { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura Etapa { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura? Situacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa TipoPessoa { get; set; }
        public Dominio.Enumeradores.OpcaoSimNaoPesquisa? FaturadoAR { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> TipoPropostaMultimodal { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> TiposPropostasMultimodal { get; set; }
    }
}
