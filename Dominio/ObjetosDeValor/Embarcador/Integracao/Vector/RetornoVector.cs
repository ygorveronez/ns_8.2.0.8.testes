using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vector
{
    public class RetornoVector
    {
        /// <summary>
        /// Código de erro da resposta
        /// </summary>
        [JsonProperty("ErrorCode")]
        public int CodigoErro { get; set; }

        /// <summary>
        /// Número identificador do lote a que pertenece a resposta
        /// </summary>
        [JsonProperty("errorMessage")]
        public string Mensagem { get; set; }

        /// <summary>
        /// Indica se a inserção teve êxito
        /// </summary>
        [JsonProperty("Ok")]
        public bool Sucesso { get; set; }
    }
}