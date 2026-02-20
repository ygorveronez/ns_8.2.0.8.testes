using Dominio.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Fatura
{
    public class FiltroDocumentosParaFatura
    {
        public int CodigoFatura { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public int CodigoCarga { get; set; }
        public string NumeroDocumento { get; set; }
        public int Notas { get; set; }
        public int NumeroDocumentoInicial { get; set; }
        public int NumeroDocumentoFinal { get; set; }
        public int Serie { get; set; }
        public int CodigoGrupoPessoas { get; set; }
        public double CPFCNPJTomador { get; set; }
        public string IETomador { get; set; }
        public int TipoOperacao { get; set; }
        public int Empresa { get; set; }
        public int TipoCarga { get; set; }
        public decimal? AliquotaICMS { get; set; }
        public int PedidoViagemNavio { get; set; }
        public int TerminalOrigem { get; set; }
        public int TerminalDestino { get; set; }
        public int Origem { get; set; }
        public int PaisOrigem { get; set; }
        public int Destino { get; set; }
        public string NumeroBooking { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> TipoPropostaMultimodal { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> TiposPropostasMultimodal { get; set; }
        public int CodigoMDFe { get; set; }
        public int CodigoContainer { get; set; }
        public string NumeroControleCliente { get; set; }
        public string NumeroReferenciaEDI { get; set; }
        public int CodigoCTe { get; set; }
        public int CodigoVeiculo { get; set; }
        public TipoCTE TipoCTe { get; set; }
        public bool ApenasFaturaExclusiva { get; set; }
        public List<int> CodigosCTes { get; set; }
        public bool HabilitarOpcaoGerarFaturasApenasCanhotosAprovados { get; set; }
        public int Filial { get; set; }
        public double TomadorFatura { get; set; }
        public string NumeroContainer { get; set; }
        public int CodigoCentroResultado { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOSConvertido> TiposOSConvertidos { get; set; }
        public bool GerarDocumentosApenasCanhotosAprovados { get; set; }
    }
}
