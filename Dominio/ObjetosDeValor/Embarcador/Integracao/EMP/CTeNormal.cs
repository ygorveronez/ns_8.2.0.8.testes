using System.Collections.Generic;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.WebService.CTe;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.EMP
{
    public class CTeNormal
    {
        public long ProtocoloCTe { get; set; }
        public string ChaveCTe { get; set; }
        public string SituacaoCTe { get; set; }
        public string NumeroCarga { get; set; }
        public decimal ValorCTe { get; set; }
        public string MensagemSefaz { get; set; }
        public string DataEmissao { get; set; }
        public string DataEvento { get; set; }
        public string CNPJEmissor { get; set; }
        public string Evento { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroContainer { get; set; }
        public string NumeroLacre { get; set; }
        public int NumeroCTe { get; set; }
        public int Serie { get; set; }
        public decimal Peso { get; set; }
        public string CNPJEmitente { get; set; }

        public int Protocolo { get; set; }

        public string Chave { get; set; }

        public string NumeroControle { get; set; }

        public int Numero { get; set; }

        public string Modelo { get; set; }

        public int CFOP { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Dominio.Enumeradores.TipoCTE TipoCTE { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Dominio.Enumeradores.TipoServico TipoServico { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa TransportadoraEmitente { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Destinatario { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Remetente { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Expedidor { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Recebedor { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Tomador { get; set; }

        public Dominio.ObjetosDeValor.Localidade LocalidadeInicioPrestacao { get; set; }

        public Dominio.ObjetosDeValor.Localidade LocalidadeFimPrestacao { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor ValorFrete { get; set; }

        public List<Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe> Documentos { get; set; }

        public decimal ValorTotalMercadoria { get; set; }

        public string VersaoCTE { get; set; }

        public bool Lotacao { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz SituacaoCTeSefaz { get; set; }

        public string MensagemRetornoSefaz { get; set; }

        public string XML { get; set; }

        public string XMLAutorizacao { get; set; }

        public string XMLCancelamento { get; set; }

        public string PDF { get; set; }

        public string MotivoCancelamento { get; set; }

        public List<int> ProtocolosDePedidos { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista> Motoristas { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo Veiculo { get; set; }

        public string NumeroNFSePrefeitura { get; set; }

        public string NumeroBoleto { get; set; }
        public int NumeroTitulo { get; set; }
        public string PDFBoleto { get; set; }

        public string TipoDocumentoFiscal { get; set; }

        public string MunicipioColeta { get; set; }

        public string MunicipioRemetente { get; set; }

        public string MunicipioEntrega { get; set; }

        public string TipoIcms { get; set; }

        public decimal AliquotaCofins { get; set; }

        public bool FlagIsentoIcms { get; set; }

        public decimal AliquotaPis { get; set; }

        public decimal ValorReducao { get; set; }

        public decimal ValorIcmsReduzido { get; set; }

        public decimal ValorBaseIcmsRemetente { get; set; }

        public decimal ValorIcmsRemetente { get; set; }

        public decimal ValorBaseIcmsDestinatario { get; set; }

        public decimal ValorIcmsDestinatario { get; set; }

        public decimal ValorBaseIcmsPobreza { get; set; }

        public bool FlagDebitoPisCofins { get; set; }

        public bool FlagDebitoIcms { get; set; }

        public bool FlagIsentoPisCofins { get; set; }

        public decimal ValorIss { get; set; }

        public bool InticativoRetencaoIss { get; set; }

        public string DataEmbarque { get; set; }

        public bool FlagIsendoContribuicoes { get; set; }

        public decimal PercentualIcmsRemetente { get; set; }

        public decimal AliquotaIcmsRemetente { get; set; }

        public decimal PercentualIcmsDestinatario { get; set; }

        public decimal AliquotaIcmsDestinatario { get; set; }

        public decimal AliquotaIcmsPobreza { get; set; }

        public decimal ValorIcmsPobreza { get; set; }

        public string SFCSacado { get; set; }

        public string PrazoPgtoSacado { get; set; }

        public string PrazoPgtoCliente { get; set; }

        public decimal ValorBaseIcms { get; set; }

        public decimal AliquotaISS { get; set; }

        public List<CTeAnterior> DocumentosAnterior { get; set; }

        public bool OcorreuSinistroAvaria { get; set; }

        public string ProtocoloAutorizacao { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal TipoModal { get; set; }

        public string NaturezaServico { get; set; }

        public string NaturezaOperacao { get; set; }

        public string DataPreviaVencimento { get; set; }

        public string DataETA { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao TipoOperacao { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.Container> Containeres { get; set; }

        public decimal PesoCubado { get; set; }
        public decimal PesoFaturado { get; set; }

        public int ProtocoloCarga { get; set; }

        public string ChaveCTeVinculado { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil> ContasContabeis { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Financeiro.CentroResultado CentroResultadoFaturamento;

        public Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado CentroResultado;

        public Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado CentroResultadoEscrituracao;

        public Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado CentroResultadoDestinatario;

        public string ItemServico { get; set; }

        public bool DocumentoGlobalizado { get; set; }

        public bool CTeFaturadoExclusivamente { get; set; }

        public List<ObjetosDeValor.CTe.Observacao> ObservacoesContribuinte { get; set; }

        public decimal ValorTaxaFeeder { get; set; }
        public string DataAutorizacao { get; set; }
        public string DataEntrega { get; set; }
        public string DataColeta { get; set; }
        public string DataPrevisaoEntrega { get; set; }
        public string NumeroManifesto { get; set; }
        public string NumeroManifestoTransbordo { get; set; }
        public string NumeroCEMercante { get; set; }
        public string ProdutoPredominante { get; set; }
        public bool Afretamento { get; set; }
        public string NumeroProtocoloANTAQ { get; set; }
        public string NumeroManifestoFEEDER { get; set; }
        public string NumeroCEFEEDER { get; set; }
        public string NumeroCTeSubstituto { get; set; }
        public string NumeroControleCTeSubstituto { get; set; }
        public string NumeroCTeAnulacao { get; set; }
        public string NumeroControleCTeAnulacao { get; set; }
        public string NumeroControleCTeComplementar { get; set; }
        public string NumeroCTeComplementar { get; set; }
        public string NumeroCTeDuplicado { get; set; }
        public string NumeroControleCTeDuplicado { get; set; }
        public string NumeroCTeOriginal { get; set; }
        public string NumeroControleCTeOriginal { get; set; }
        public string DataCancelamento { get; set; }
        public string DataAnulacao { get; set; }
        public string ProtocoloCancelamentoInutilizacao { get; set; }
        public string DataEmissaoFatura { get; set; }
        public string DataEmissaoBoleto { get; set; }
        public bool PossuiCTeComplementar { get; set; }
        public string NumeroMDFeVinculado { get; set; }
        public string DataEmissaoMDFeVinculado { get; set; }
        public StatusMDFe StatusMDFeVinculado { get; set; }
        public CTeTitulo CTeTitulo { get; set; }
        public List<string> ChavesCTeSVM { get; set; }
        public List<string> ChavesCTeCTM { get; set; }

        public bool GerarFaturamentoAVista { get; set; }
        public bool RolagemCarga { get; set; }
    }
}
