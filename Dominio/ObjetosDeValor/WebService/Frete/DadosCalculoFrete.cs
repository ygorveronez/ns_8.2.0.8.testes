using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Frete
{
    public class DadosCalculoFrete
    {
        /// <summary>
        /// Código IBGE das localidades de origem
        /// </summary>
        public List<int> IBGEOrigem { get; set; }

        /// <summary>
        /// Código IBGE das localidades de destino
        /// </summary>
        public List<int> IBGEDestino { get; set; }
        
        /// <summary>
        /// CEP de origem
        /// </summary>
        public List<int> CEPOrigem { get; set; }

        /// <summary>
        /// CEP de destino
        /// </summary>
        public List<int> CEPDestino { get; set; }

        /// <summary>
        /// Valor da mercadoria ou notas fiscais
        /// </summary>
        public decimal ValorMercadoria { get; set; }

        /// <summary>
        /// Quantidade de volumes transportadas
        /// </summary>
        public decimal Volumes { get; set; }

        /// <summary>
        /// Peso da mercadoria transportada
        /// </summary>
        public decimal Peso { get; set; }

        /// <summary>
        /// Código de Integração do Tipo de Operação
        /// </summary>
        public string CodigoIntegracaoTipoOperacao { get; set; }

        /// <summary>
        /// Código de Integração do Tipo de Carga
        /// </summary>
        public string CodigoIntegracaoTipoCarga { get; set; }
    }
}
