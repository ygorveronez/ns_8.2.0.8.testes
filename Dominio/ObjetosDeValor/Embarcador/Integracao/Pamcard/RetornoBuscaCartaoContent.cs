using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard
{
    public class RetornoBuscaCartaoContent
    {
        [JsonProperty(PropertyName = "idCartao", Required = Required.AllowNull)]
        public int IdCartao { get; set; }

        [JsonProperty(PropertyName = "panFormatado", Required = Required.AllowNull)]
        public string PanFormatado { get; set; }

        [JsonProperty(PropertyName = "quatroUltimosDigitos", Required = Required.AllowNull)]
        public string QuatroUltimosDigitos { get; set; }

        [JsonProperty(PropertyName = "dataValidade", Required = Required.AllowNull)]
        public string DataValidade { get; set; }

        [JsonProperty(PropertyName = "dataHoraCadastro", Required = Required.AllowNull)]
        public DateTime DataHoraCadastro { get; set; }

        [JsonProperty(PropertyName = "dataHoraAtualizacao", Required = Required.AllowNull)]
        public DateTime DataHoraAtualizacao { get; set; }

        [JsonProperty(PropertyName = "emissor", Required = Required.AllowNull)]
        public string Emissor { get; set; }

        [JsonProperty(PropertyName = "status", Required = Required.AllowNull)]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "statusSolicitado", Required = Required.AllowNull)]
        public string StatusSolicitado { get; set; }

        [JsonProperty(PropertyName = "statusIntegracao", Required = Required.AllowNull)]
        public string StatusIntegracao { get; set; }

        [JsonProperty(PropertyName = "modalidade", Required = Required.AllowNull)]
        public string Modalidade { get; set; }

        [JsonProperty(PropertyName = "proprietario", Required = Required.AllowNull)]
        public RetornoBuscaCartaoContentProprietario Proprietario { get; set; }

        [JsonProperty(PropertyName = "portador", Required = Required.AllowNull)]
        public RetornoBuscaCartaoContentPortador Portador { get; set; }

        [JsonProperty(PropertyName = "portadorSolicitado", Required = Required.AllowNull)]
        public string PortadorSolicitado { get; set; }

        [JsonProperty(PropertyName = "operacoesDisponiveis", Required = Required.AllowNull)]
        public List<RetornoBuscaCartaoContentOperacaoDisponivel> OperacoesDisponiveis { get; set; }

        [JsonProperty(PropertyName = "emProcessamento", Required = Required.AllowNull)]
        public bool EmProcessamento { get; set; }

        [JsonProperty(PropertyName = "labelEmProcessamento", Required = Required.AllowNull)]
        public string LabelEmProcessamento { get; set; }

        [JsonProperty(PropertyName = "usuarioProcessamento", Required = Required.AllowNull)]
        public string UsuarioProcessamento { get; set; }

        [JsonProperty(PropertyName = "numeroPedidoPlastico", Required = Required.AllowNull)]
        public string NumeroPedidoPlastico { get; set; }

        [JsonProperty(PropertyName = "tipoCartao", Required = Required.AllowNull)]
        public string TipoCartao { get; set; }

        [JsonProperty(PropertyName = "dataHoraAtivacao", Required = Required.AllowNull)]
        public DateTime? DataHoraAtivacao { get; set; }

        [JsonProperty(PropertyName = "dataHoraCancelamento", Required = Required.AllowNull)]
        public DateTime? DataHoraCancelamento { get; set; }

        [JsonProperty(PropertyName = "idImpressoCartao", Required = Required.AllowNull)]
        public int IdImpressoCartao { get; set; }

        [JsonProperty(PropertyName = "idContaCartao", Required = Required.AllowNull)]
        public string IdContaCartao { get; set; }

        [JsonProperty(PropertyName = "idPedidoEmissor", Required = Required.AllowNull)]
        public string IdPedidoEmissor { get; set; }

        [JsonProperty(PropertyName = "operacaoId", Required = Required.AllowNull)]
        public string OperacaoId { get; set; }
    }
}
