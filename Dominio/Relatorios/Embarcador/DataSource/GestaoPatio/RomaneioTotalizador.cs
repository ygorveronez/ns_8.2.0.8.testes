using System;

namespace Dominio.Relatorios.Embarcador.DataSource.GestaoPatio
{
    public class RomaneioTotalizador
    {
        public string Filial { get; set; }
        public string Transportador { get; set; }
        public string Veiculo { get; set; }
        public string Motorista { get; set; }
        public string NumeroCarga { get; set; }
        public DateTime DataCarregamento { get; set; }
        public string ProdutoCodigo { get; set; }
        public string ProdutoDescricao { get; set; }
        public decimal ProdutoQuantidade { get; set; }   

    }
}
