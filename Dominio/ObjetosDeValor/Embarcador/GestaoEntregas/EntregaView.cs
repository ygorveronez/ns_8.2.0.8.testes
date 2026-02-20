using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoEntregas
{
    public class EntregaView
    {
        public string Numero { get; set; }

        public string Destinatario { get; set; }

        public string Localidade { get; set; }

        public string Situacao { get; set; }

        public string DataAgenda { get; set; }

        public string DataIncioViagem { get; set; }

        public string DataEntrega { get; set; }

        public string DataPrevisaoChegada { get; set; }

        public string NotasFiscais { get; set; }

        public string NumeroTransporte { get; set; }

        public string Transportador { get; set; }

        public string Filial { get; set; }

        public string Observacao { get; set; }

        public int Volumes { get; set; }

        public int Avaliacao { get; set; }

        public string MotivoAvaliacao { get; set; }

        public QuestionarioView Questionario { get; set; }

        public LocalizacaoView Localizacao { get; set; }

        public List<MotivosView> MotivosAvaliacao { get; set; }

        public List<HistoricoView> Historico { get; set; }

        public string EnderecoCompleto { get; set; }

        public bool Entregue { get; set; }

        public string DiaPrevisaoChegada { get; set; }

        public string HorarioPrevisaoChegada { get; set; }

        public string Remetente { get; set; }

        public string NomeMotorista { get; set; }

        public string TelefoneMotorista { get; set; }

        public string CodigoRastreio { get; set; }

        public string PlacaVeiculo { get; set; }
        public List<AnexosView> Anexos { get; set; }

        public bool ClienteExterior { get; set; }

        public bool HabilitarPrevisaoEntrega { get; set; }

        public bool HabilitarObservacao { get; set; }
        public string CodigoRastreioCorreio { get; set; }
		public string CodigoPedidoCliente { get; set; }
		public string NumeroOrdemCompra { get; set; }
		public int QuantidadeVolumesNF { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal PesoLiquido { get; set; }
        public List<string> CaminhoFotosEntrega { get; set; }
        public bool PermitirVisualizarFotosEntrega { get; set; }
        public bool ExibirDetalhesPedido { get; set; }
        public bool ExibirHistoricoPedido { get; set; }
        public bool ExibirDetalhesMotorista { get; set; }
        public bool ExibirDetalhesProduto { get; set; }
        public bool ExibirProduto { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoProduto> Produtos { get; set; }
        public bool HabilitarAcessoPortalMultiCliFor { get; set; }
        public string LinkAcessoPortalMultiCliFor { get; set; }
    }
}
