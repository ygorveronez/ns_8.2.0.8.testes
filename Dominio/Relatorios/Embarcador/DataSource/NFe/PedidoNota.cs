using System;

namespace Dominio.Relatorios.Embarcador.DataSource.NFe
{
    public class PedidoNota
    {
        public int NumeroNota { get; set; }
        public int Serie { get; set; }
        public string DescricaoStatus { get; set; }
        public DateTime DataNota { get; set; }
        public string Pessoa { get; set; }
        public double CNPJPessoa { get; set; }
        public string NumerosPedido { get; set; }
        public string DatasPedido { get; set; }
        public string TiposPedido { get; set; }
        public string FuncionariosPedido { get; set; }
        public decimal ValorNota { get; set; }
        public decimal ValorPedidos { get; set; }
    }
}
