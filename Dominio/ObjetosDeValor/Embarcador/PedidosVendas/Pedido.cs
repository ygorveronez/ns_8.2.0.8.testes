using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.PedidosVendas
{
    public class Pedido
    {
        public Cliente Cliente { get; set; }
        public int CodigoIntregacao { get; set; }
        public DateTime DataPedido { get; set; }
        public string FormaPagamento { get; set; }
        public string Observacao { get; set; }
        public decimal Valor { get; set; }
        public List<Produto> Produtos { get; set; }
    }

    public class Cliente
    {
        public string CodigoIntegracao { get; set; }
        public string Email { get; set; }
        public string RazaoSocial { get; set; }
        public string Logradouro { get; set; }
        public string Cidade { get; set; }
        public string SiglaUF { get; set; }
        public string Telefone { get; set; }
        public string CEP { get; set; }
        public string Bairro { get; set; }
        public string CPFCNPJ { get; set; }
        public string NumeroEndereco { get; set; }
    }

    public class Produto
    {
        public string CodigoProduto { get; set; }
        public string CodigoVariacao { get; set; }
        public string DescricaoProduto { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
    }
}
