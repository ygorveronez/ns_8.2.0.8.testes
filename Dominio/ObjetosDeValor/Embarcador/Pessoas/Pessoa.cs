using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dominio.ObjetosDeValor.Embarcador.Pessoas
{
    [DataContract]
    public class Pessoa
    {
        [DataMember]
        public bool ClienteExterior { get; set; }

        [DataMember]
        public string CPFCNPJ { get; set; }

        [DataMember]
        public string CodigoIntegracao { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Dominio.Enumeradores.TipoPessoa TipoPessoa { get; set; }

        [DataMember]
        public int CodigoAtividade { get; set; }

        [DataMember]
        public string RGIE { get; set; }

        [DataMember]
        public string IM { get; set; }

        public string InscricaoSuframa { get; set; }

        [DataMember]
        public string Observacao { get; set; }

        [DataMember]
        public string RazaoSocial { get; set; }

        [DataMember]
        public string NomeFantasia { get; set; }

        [DataMember]
        public string CNAE { get; set; }

        [DataMember]
        public Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco Endereco { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public bool AtualizarEnderecoPessoa { get; set; }

        [DataMember]
        public string Latitude { get; set; }

        [DataMember]
        public string Longitude { get; set; }

        [DataMember]
        public string EmailFatura { get; set; }

        [DataMember]
        public string TempoAgendamento { get; set; }

        [DataMember]
        public string FormaAgendamento { get; set; }

        [DataMember]
        public string LinkAgendamento { get; set; }

        [DataMember]
        public bool? ExigeAgendamentoDescarga { get; set; }

        [DataMember]
        public bool? AgendamentoExigeNotaFiscal { get; set; }

        [DataMember]
        public string RestricaoEntrega { get; set; }

        public string EmailContador { get; set; }

        public string EmailContato { get; set; }

        public bool EnviarEmialContador { get; set; }

        public bool EnviarEmailContato { get; set; }

        public bool EnviarEmail { get; set; }

        public string CPFCNPJSemFormato { get; set; }

        [DataMember]
        public string Codigo { get; set; }

        [DataMember]
        public string CodigoCategoria { get; set; }

        [DataMember]
        public string CodigoDocumento { get; set; }

        [DataMember]
        public string CodigoAlternativo { get; set; }

        [DataMember]
        public string RNTRC { get; set; }

        [DataMember]
        public string NumeroCartaoCIOT { get; set; }

        [DataMember]
        public bool? ExigeAgendamento { get; set; }

        [DataMember]
        public bool? GerarCIOT { get; set; }

        [DataMember]
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT? TipoFavorecidoCIOT { get; set; }

        [DataMember]
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? TipoPagamentoCIOT { get; set; }

        [DataMember]
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa GrupoPessoa { get; set; }

        [DataMember]
        public bool ExigirNumeroControleCliente { get; set; }

        [DataMember]
        public bool ExigirNumeroNumeroReferenciaCliente { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos TipoEmissaoCTeDocumentosExclusivo { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula ParametroRateioFormulaExclusivo { get; set; }

        [DataMember]
        public bool AdicionarComoOutroEndereco { get; set; }

        [DataMember]
        public bool InativarCliente { get; set; }

        public bool ValidarCPFPrimeiro { get; set; }

        [DataMember]
        public string CodigoDocumentoFornecedor { get; set; }

        [DataMember]
        public bool Cliente { get; set; }

        [DataMember]
        public bool NaoEnviarParaDocsys { get; set; }

        [DataMember]
        public bool Fornecedor { get; set; }

        [DataMember]
        public int RaioEmMetros { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario RegimeTributario { get; set; }

        public string SenhaConfirmacaoColetaEntrega { get; set; }

        [DataMember]
        public string NumeroCUITRUIT { get; set; }

        [DataMember]
        public virtual IndicadorIE? IndicadorIE { get; set; }

        [DataMember]
        public DadosBancarios DadosBancarios { get; set; }

        [DataMember]
        public bool? NaoEObrigatorioInformarNfeNaColeta { get; set; }

        [DataMember]
        public bool? NaoExigePreenchimentoDeChecklistEntrega { get; set; }


        [DataMember]
        public MesoRegiao MesoRegiao { get; set; }

        [DataMember]
        public long Protocolo { get; set; }

        [DataMember]
        public bool? ValidarValorMinimoMercadoriaPorEntrega { get; set; }

        [DataMember]
        public string ValorMinimoMercadoriaPorEntrega { get; set; }

        [DataMember]
        public string RKST {  get; set; }

        [DataMember]
        public string MDGCode { get; set; }

        [DataMember]
        public string CMDID { get; set; }

    }
}
