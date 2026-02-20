using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RecebimentoCargaStatus
{
    public class UrlXml
    {
        /// <summary>
        /// Tipo de documento *
        /// OC                  - Ordem de carregamento
        /// NFE                 – nota Fiscal Eletrônica
        /// CTE                 – Conhecimento de Transporte Eletrônico
        /// MDFE                – Manifesto Eletrônico de documentos Fiscais
        /// contratoTransporte  - Contrato de Transporte deve ser retido
        /// </summary>
        [JsonProperty("TipoDocumento")]
        public string TipoDocumento { get; set; }

        /// <summary>
        /// Url de descarga do documento
        /// </summary>
        [JsonProperty("Archivo")]
        public string Arquivo { get; set; }
    }
}
