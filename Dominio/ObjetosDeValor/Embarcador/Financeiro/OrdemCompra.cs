using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class OrdemCompra
    {
        public int Numero { get; set; }
        public DateTime Data { get; set; }
        public DateTime DataPrevisaoRetorno { get; set; }
        public MotivoCompra MotivoCompra { get; set; }
        public Pessoas.Pessoa Transportador { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo Veiculo { get; set; }
        public Pessoas.Pessoa Fornecedor { get; set; }
        public string CondicaoPagamento { get; set; }
        public string Observacao { get; set; }
        public List<Pedido.Produto> Produtos { get; set; }

        public string StrData { get; set; }
        public string StrDataPrevisaoRetorno { get; set; }
    }
}
