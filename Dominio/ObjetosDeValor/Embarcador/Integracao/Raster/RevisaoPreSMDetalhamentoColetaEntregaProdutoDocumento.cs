using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Raster
{
    public class RevisaoSMDetalhamentoColetaEntregaProdutoDocumento
    {
        /// <summary>
        /// BIP, CHASSI, LOAD NUMBER, CTE, CTRC, LACRE, MANIFESTO, MIC, NOTA FISCAL, PEDIDO
        /// </summary>
        public string Tipo { get; set; }
        /// <summary>
        /// Número ou chave do documento 
        /// </summary>
        public string Numero { get; set; }
        public decimal? Valor { get; set; }
        public decimal? Peso { get; set; }
        public decimal? PesoCubado { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Cubagem { get; set; }
        public string CentroCusto { get; set; }
        public DateTime? DataEmissao { get; set; }
        public DateTime? DataAgendamento { get; set; }
        public decimal? ValorServico { get; set; }
        public string Observacao1 { get; set; }
        public string Observacao2 { get; set; }
        public string Observacao3 { get; set; }
    }
}
