using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte
{
    public class ReferenciaEnvio
    {
        /// <summary>
        /// Número de referência da entrega com até 10 caracteres.
        /// Edit: Número do endereço de entrega, com até 10 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "no", Order = 0)]
        public string NumeroReferenciaEntrega { get; set; }

        /// <summary>
        /// Cidade de entrega, com até 45 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "cidade", Order = 1)]
        public string Cidade { get; set; }

        /// <summary>
        /// Estado de entrega, com até 28 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "estado", Order = 2)]
        public string Estado { get; set; }

        /// <summary>
        /// Endereço de entrega com até 45 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "endereco", Order = 3)]
        public string Endereco { get; set; }

        /// <summary>
        /// Inscrição municipal, com até 15 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "im", Order = 4)]
        public string InscricaoMunicipal { get; set; }

        /// <summary>
        /// Bairro de entrega com até 45 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "bairro", Order = 5)]
        public string Bairro { get; set; }

        /// <summary>
        /// Código IBGE da cidade, até 7 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "ibge", Order = 6)]
        public string IBGE { get; set; }

        /// <summary>
        /// Linha de entrega, com até 15 caracteres. Código identificador do cadastro de entrega.
        /// </summary>
        [JsonProperty(PropertyName = "linhaEntrega", Order = 7)]
        public decimal LinhaEntrega { get; set; }

        /// <summary>
        /// Nome da empresa ou pessoa destinatária, com até 42 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "nome", Order = 8)]
        public string Nome { get; set; }

        /// <summary>
        /// CNPJ do destinatário, com até 14 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "cnpj", Order = 9)]
        public string CNPJ { get; set; }

        /// <summary>
        /// Código de Endereçamento Postal, com até 8 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "cep", Order = 10)]
        public string CEP { get; set; }

        /// <summary>
        /// Código de cadastro do destinatário, com até 15 caracteres. Código identificador do cadastro do cliente.
        /// </summary>
        [JsonProperty(PropertyName = "codCadastro", Order = 11)]
        public string CodigoCadastro { get; set; }

        /// <summary>
        /// Código de cadastro do destinatário, com até 15 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "uf", Order = 12)]
        public string UF { get; set; }

        /// <summary>
        /// Unidade da federação, com até 2 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "complemento", Order = 13)]
        public string Complemento { get; set; }

        /// <summary>
        /// Complemento do endereço, com até 45 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "ie", Order = 14)]
        public string InscricaoEstadual { get; set; }
    }
}
