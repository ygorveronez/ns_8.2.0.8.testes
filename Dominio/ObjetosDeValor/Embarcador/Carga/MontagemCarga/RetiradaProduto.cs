using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga
{
    public class RetiradaProduto
    {
        public int Codigo { get; set; }
        public string NumeroCarregamento { get; set; }
        public string NumeroCarregamentoFormatado { get { return (this.SituacaoCarregamento == SituacaoCarregamento.Fechado || this.SituacaoCarregamento == SituacaoCarregamento.Bloqueado) ? this.NumeroCarregamento : string.Empty; } }
        public SituacaoCarregamento SituacaoCarregamento { get; set; }
        public string SituacaoCarregamentoDescricao { get { return this.SituacaoCarregamento.ObterDescricaoRetirada(); } }
        public DateTime DataCarregamentoCarga { get; set; }
        public string NomeEmpresa { get; set; }
        public string ObservacaoCarregamento { get; set; }
        public string Filial { get; set; }
        public string Pedido { get; set; }
        public string Cliente { get; set; }
        public string CodigoPedidoCliente { get; set; }
        public string OrdemCompraCliente { get; set; }
        public string PlacaVeiculo { get; set; }
        public string Motorista { get; set; }

        public string ObterDescricaoSituacaoAgendamento
        {
            get
            {
                return this.SituacaoCarregamento == SituacaoCarregamento.Fechado && (this.DataCarregamentoCarga == null || this.DataCarregamentoCarga == DateTime.MinValue) ? Localization.Resources.Pedidos.RetiradaProdutoLista.PendenteAgendamento : this.SituacaoCarregamentoDescricao;

            }


        }
    }
}
