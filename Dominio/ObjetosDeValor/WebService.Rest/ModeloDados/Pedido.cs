using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class Pedido
    {
        public int Protocolo { get; set; }

        public string Numero { get; set; }

        public DateTime? DataCriacao { get; set; }

        public decimal PesoTotal { get; set; }

        public Cliente Destinatario { get; set; }

        public Filial Filial { get; set; }

        public Filial FilialVenda { get; set; }

        public Endereco OutroEnderecoDestino { get; set; }

        public Usuario FuncionarioVendedor { get; set; }

        public List<PedidoProduto> Produtos { get; set; }

        public IEnumerable<NotaFiscal> NotasFiscais { get; set; }
    }
}
