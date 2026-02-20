using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.OrdemServico
{
    public class OrdemServico
    {
        public int Protocolo { get; set; }

        public int Numero { get; set; }

        public string DataEmissao { get; set; }

        public string DataEntrega { get; set; }

        public StatusPedidoVenda Status { get; set; }

        public decimal ValorTotal { get; set; }

        public decimal ValorProdutos { get; set; }

        public decimal ValorServicos { get; set; }

        public string FormaPagamento { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario Funcionario { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario FuncionarioSolicitante { get; set; }

        public string PessoaSolicitante { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Pessoa { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo Veiculo { get; set; }

        public string Observacao { get; set; }

        public List<Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServicoItens> Servicos { get; set; }

    }
}
