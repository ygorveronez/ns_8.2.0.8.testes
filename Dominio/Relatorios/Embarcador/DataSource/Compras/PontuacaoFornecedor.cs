using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Compras
{
    public class PontuacaoFornecedor
    {
        public string Comprador { get; set; }
        public string Fornecedor { get; set; }
        public int OrdemCompra { get; set; }
        public DateTime DataOrdemCompra { get; set; }
        public string NotasEntradas { get; set; }
        public int CriterioPrazoEntregaPontualidade { get; set; }                
        public int CriterioCaracteristicaEspecificacoes { get; set; }        
        public int CriterioQuantidadeRecebida { get; set; }                
        public int CriterioIntegridadeFisica { get; set; }        
        public int CriterioAtendimento { get; set; }
    }
}
