using System;
using System.Collections.Generic;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca
{
    public class Pedido
    {
        public int Codigo { get; set; }

        public string Sequencia { get; set; }

        public string Operacao { get; set; }

        public string VendaNFe { get; set; }

        public string Cliente { get; set; }

        public string Fantasia { get; set; }

        public string CNPJCPF { get; set; }

        public string Fone { get; set; }

        public string Endereco { get; set; }

        public string Bairro { get; set; }

        public string Numero { get; set; }

        public string Cidade { get; set; }

        public string UF { get; set; }

        public string CEP { get; set; }

        public string Referencia { get; set; }

        public string Restricoes { get; set; }

        public DateTime DataEntrega { get; set; }

        public string Observacao { get; set; }

        public string Blocos { get; set; }

        public decimal QuantidadeTotal { get; set;}

        public decimal TotalizadorQuantidade{ get; set; }

        //public List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Produto> Produtos { get; set; }

        //public List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Pagamento> Pagamentos { get; set; }

        //public List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Recolhimento> Recolhimentos { get; set; }
    }

    public class PedidoAgrupado
    {
        public string CNPJCPF { get; set; }

        public string Cliente { get; set; }

        public string Fantasia { get; set; }

        public string Fone { get; set; }

        public string Endereco { get; set; }

        public string Bairro { get; set; }

        public string Numero { get; set; }

        public string Cidade { get; set; }

        public string UF { get; set; }
        public string CEP { get; set; }
        public string Complemento { get; set; }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> CargasPedidos { get; set; }
    }
}
