using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class AuditoriaOrdemServicoProduto
    {
        public int Codigo { get; set; }
        public int CodigoOrdemServico { get; set; }
        public string CodigoProduto { get; set; }
        public string NomeInsumo { get; set; }
        public decimal QuantidadePrevia { get; set; }
        public int Unidade { get; set; }
        public decimal CustoPrevio { get; set; }
        public decimal QuantidadeReal { get; set; }
        public decimal CustoReal { get; set; }
        public decimal Diferenca { get; set; }
        public string UnidadeDescricao
        {
            get
            {
                return ((UnidadeDeMedida)this.Unidade).ObterDescricao();
            }
        }
    }
}
