using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Compras
{
    public class PontuacaoComprador
    {
        public string Comprador { get; set; }
        public string Produto { get; set; }
        public DateTime DataRequisicao { get; set; }
        public DateTime DataOrdemCompra { get; set; }
        public int QtdDias { get; set; }
    }
}
