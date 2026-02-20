using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.WebServiceCarrefour.CTe
{
    [DataContract]
    public sealed class CTe
    {
        public int CodigoCargaCTe { get; set; }

        [DataMember]
        public int Protocolo { get; set; }

        [DataMember]
        public string Chave { get; set; }

        [DataMember]
        public string NumeroControle { get; set; }

        [DataMember]
        public int Numero { get; set; }

        [DataMember]
        public int Serie { get; set; }

        [DataMember]
        public string Modelo { get; set; }

        [DataMember]
        public string DataEmissao { get; set; }

        [DataMember]
        public int CFOP { get; set; }

        [DataMember]
        public Enumeradores.TipoCTE TipoCTE { get; set; }

        [DataMember]
        public Enumeradores.TipoServico TipoServico { get; set; }

        [DataMember]
        public Enumeradores.TipoTomador TipoTomador { get; set; }

        [DataMember]
        public Pessoas.Empresa TransportadoraEmitente { get; set; }

        [DataMember]
        public Pessoas.Pessoa Destinatario { get; set; }

        [DataMember]
        public Pessoas.Pessoa Remetente { get; set; }

        [DataMember]
        public Pessoas.Pessoa Expedidor { get; set; }

        [DataMember]
        public Pessoas.Pessoa Recebedor { get; set; }

        [DataMember]
        public Pessoas.Pessoa Tomador { get; set; }

        [DataMember]
        public Localidade.Localidade LocalidadeInicioPrestacao { get; set; }

        [DataMember]
        public Localidade.Localidade LocalidadeFimPrestacao { get; set; }

        [DataMember]
        public Frete.FreteValor ValorFrete { get; set; }

        [DataMember]
        public List<DocumentosCTe> Documentos { get; set; }

        [DataMember]
        public decimal ValorTotalMercadoria { get; set; }

        [DataMember]
        public string VersaoCTE { get; set; }

        [DataMember]
        public bool Lotacao { get; set; }

        [DataMember]
        public Embarcador.Enumeradores.SituacaoCTeSefaz SituacaoCTeSefaz { get; set; }

        [DataMember]
        public string XML { get; set; }

        [DataMember]
        public string XMLAutorizacao { get; set; }

        [DataMember]
        public string XMLCancelamento { get; set; }

        [DataMember]
        public string PDF { get; set; }

        [DataMember]
        public string MotivoCancelamento { get; set; }

        [DataMember]
        public List<int> ProtocolosDePedidos { get; set; }

        [DataMember]
        public List<Carga.Motorista> Motoristas { get; set; }

        [DataMember]
        public Frota.Veiculo Veiculo { get; set; }

        [DataMember]
        public string NumeroNFSePrefeitura { get; set; }

        [DataMember]
        public string NumeroBoleto { get; set; }

        [DataMember]
        public int NumeroTitulo { get; set; }

        [DataMember]
        public string PDFBoleto { get; set; }

        [DataMember]
        public string TipoDocumentoFiscal { get; set; }

        [DataMember]
        public string MunicipioColeta { get; set; }

        [DataMember]
        public string MunicipioRemetente { get; set; }

        [DataMember]
        public string MunicipioEntrega { get; set; }

        [DataMember]
        public string TipoIcms { get; set; }

        [DataMember]
        public decimal AliquotaCofins { get; set; }

        [DataMember]
        public bool FlagIsentoIcms { get; set; }

        [DataMember]
        public decimal AliquotaPis { get; set; }

        [DataMember]
        public decimal ValorReducao { get; set; }

        [DataMember]
        public decimal ValorIcmsReduzido { get; set; }

        [DataMember]
        public decimal ValorBaseIcmsRemetente { get; set; }

        [DataMember]
        public decimal ValorIcmsRemetente { get; set; }

        [DataMember]
        public decimal ValorBaseIcmsDestinatario { get; set; }

        [DataMember]
        public decimal ValorIcmsDestinatario { get; set; }

        [DataMember]
        public decimal ValorBaseIcmsPobreza { get; set; }

        [DataMember]
        public bool FlagDebitoPisCofins { get; set; }

        [DataMember]
        public bool FlagDebitoIcms { get; set; }

        [DataMember]
        public bool FlagIsentoPisCofins { get; set; }

        [DataMember]
        public decimal ValorIss { get; set; }

        [DataMember]
        public bool InticativoRetencaoIss { get; set; }

        [DataMember]
        public string DataEmbarque { get; set; }

        [DataMember]
        public bool FlagIsendoContribuicoes { get; set; }

        [DataMember]
        public decimal PercentualIcmsRemetente { get; set; }

        [DataMember]
        public decimal AliquotaIcmsRemetente { get; set; }

        [DataMember]
        public decimal PercentualIcmsDestinatario { get; set; }

        [DataMember]
        public decimal AliquotaIcmsDestinatario { get; set; }

        [DataMember]
        public decimal AliquotaIcmsPobreza { get; set; }

        [DataMember]
        public decimal ValorIcmsPobreza { get; set; }

        [DataMember]
        public string SFCSacado { get; set; }

        [DataMember]
        public string PrazoPgtoSacado { get; set; }

        [DataMember]
        public string PrazoPgtoCliente { get; set; }

        [DataMember]
        public decimal ValorBaseIcms { get; set; }

        [DataMember]
        public decimal AliquotaISS { get; set; }

        [DataMember]
        public List<CTeAnterior> DocumentosAnterior { get; set; }

        [DataMember]
        public bool OcorreuSinistroAvaria { get; set; }

        [DataMember]
        public string ProtocoloAutorizacao { get; set; }
    }
}
