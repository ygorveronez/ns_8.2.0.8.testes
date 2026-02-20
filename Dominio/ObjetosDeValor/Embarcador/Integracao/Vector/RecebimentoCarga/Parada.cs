using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RecebimentoCarga
{
    public class Parada
    {
        /// <summary>
        /// Código único identificador da parada
        /// </summary>
        [JsonProperty("StopIdentifier")]
        public string IdentificadorParada { get; set; }

        /// <summary>
        /// Nome da entidade
        /// </summary>
        [JsonProperty("Name")]
        public string Nome { get; set; }

        /// <summary>
        /// Número do CPF/CNPJ
        /// </summary>
        [JsonProperty("TaxId")]
        public string CPFCNPJ { get; set; }

        /// <summary>
        /// Número da Inscrição Estadual
        /// </summary>
        [JsonProperty("StateRegistration")]
        public string InscricaoEstadual { get; set; }

        /// <summary>
        /// Descrição do endereço da entidade
        /// *Obrigatório quando não existir Nota
        /// eletônica(NF Produtor)
        /// </summary>
        [JsonProperty("Address")]
        public string Endereco { get; set; }

        /// <summary>
        /// Número do endereço da entidade
        /// </summary>
        [JsonProperty("AddressNumber")]
        public string Numero { get; set; }

        /// <summary>
        /// Complemento do endereço da entidade
        /// </summary>
        [JsonProperty("Complement")]
        public string Complemento { get; set; }

        /// <summary>
        /// Número do CEP do endereço da entidade
        /// * Obrigatório quando não existir Nota
        /// eletrônica(NF Produtor)
        /// </summary>
        [JsonProperty("Cep")]
        public string CEP { get; set; }

        /// <summary>
        /// Nome do Bairro do endereço da entidade
        /// </summary>
        [JsonProperty("Neighboorhood")]
        public string Bairro { get; set; }

        /// <summary>
        /// Número do IBGE completo da Cidade do Endereço da entidade
        /// * Obrigatório quando não existir Nota
        /// eletrônica(NF Produtor)
        /// </summary>
        [JsonProperty("CityIBGE")]
        public int CodigoIBGE { get; set; }

        /// <summary>
        /// Nome da Cidade. Usar EXTERIOR quando país != Brasil MUNICIPIO?
        /// </summary>
        [JsonProperty("City")]
        public string Cidade { get; set; }

        /// <summary>
        /// Sigla do estado. Usar EX quando país != Brasil
        /// Ex: Siglas das UF do Brasil (AM, AP ...)
        /// </summary>
        [JsonProperty("StateInitials")]
        public string Estado { get; set; }

        /// <summary>
        /// Nome do País
        /// </summary>
        [JsonProperty("Country")]
        public string Pais { get; set; }

        /// <summary>
        /// Número do telefone da entidade
        /// </summary>
        [JsonProperty("Phone")]
        public string Telefone { get; set; }

        /// <summary>
        /// E-mail da entidade
        /// </summary>
        [JsonProperty("Email")]
        public string Email { get; set; }

        /// <summary>
        /// 1 - Remetente
        /// 2 - Destinatário
        /// 3 - Consignatário Expedidor
        /// 4 - Consignatário Recebedor
        /// 5 - Tomador Serviço
        /// </summary>
        [JsonProperty("LocationType")]
        public TipoEntrega TipoEntrega { get; set; }

        /// <summary>
        /// Número de ordem da sequência de entrega.
        /// Para destinatário e Consignatário Recebedor colocar o mesmo número de ordem da
        ///  sequência de forma de poder relacionar entregas com clientes.
        /// </summary>
        [JsonProperty("LocationSequence")]
        public int SequenciaEntrega { get; set; }

        /// <summary>
        /// Detalhes adicionais do ponto de carga
        /// </summary>
        [JsonProperty("TransportDescription")]
        public string DescricaoTransporte { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        [JsonProperty("Lat")]
        public string Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        [JsonProperty("Lng")]
        public string Longitude { get; set; }

        /// <summary>
        /// Listado de entregas de produto
        /// </summary>
        [JsonProperty("DeliveryUnits")]
        public List<Entrega> Entregas { get; set; }

        /// <summary>
        /// Local de Coleta?
        ///   false - Não(default)
        ///   true - Sim
        /// </summary>
        [JsonProperty("IsLoad")]
        public bool LocalColeta { get; set; }

        /// <summary>
        /// Incluir a referência de StopIdentifier do
        /// local da coleta relacionada com cada entrega
        /// </summary>
        [JsonProperty("StopIdentifierLoad")]
        public string IdentificadorCarregamentoParada { get; set; }

        /// <summary>
        /// # de Notas Fiscais para cada Entrega
        /// </summary>
        [JsonProperty("NrNfs")]
        public int NumeroNotasFiscais { get; set; }

        /// <summary>
        /// # de ajudantes necessários
        /// </summary>
        [JsonProperty("Assistants")]
        public int NumeroAjudantes { get; set; }

        /// <summary>
        /// 1 - CIF
        /// 2 - FOB
        /// 3 - FOB Dirigido
        /// </summary>
        [JsonProperty("FreightType")]
        public int TipoFrete { get; set; }

        /// <summary>
        /// Prazo para pagamento do frete
        /// (sendo 0 pago A Vista)
        /// </summary>
        [JsonProperty("PaymentCondition")]
        public int CondicaoPagamento { get; set; }
    }

    public enum TipoEntrega
    {
        Remetente = 1,
        Destinatario = 2,
        ConsignatarioExpedidor = 3,
        ConsignatarioRecebedor = 4,
        TomadorServico = 5
    }
}
