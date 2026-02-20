using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Globalization;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoPedido
{
    public class PedidoGestaoPedido
    {
        #region Propriedades

        public bool DT_Row_Selecionado { get; set; }

        public string NumeroCarregamento { get; set; }

        public string CodigoAgrupamentoCarregamento { get; set; }

        public string NumeroPedido { get; set; }

        public int Codigo { get; set; }

        public int Protocolo { get; set; }

        public string SessaoRoteirizador { get; set; }

        public string SituacaoComercialPedido { get; set; }

        public string SituacaoEstoque { get; set; }

        public SituacaoRoteirizadorIntegracao SituacaoRoteirizadorIntegracao { get; set; }

        public DateTime? DataCarregamentoPedido { get; set; }

        public DateTime? DataPrevisaoEntrega { get; set; }

        public string Filial { get; set; }

        public string Destino { get; set; }

        public string AbreviacaoPais { get; set; }

        public string NomePais { get; set; }

        public string SiglaEstado { get; set; }

        public int CodigoIBGE { get; set; }

        public string NomeCliente { get; set; }

        public string NomeFantasiaCliente { get; set; }

        public bool PontoTransbordoCliente { get; set; }

        public string CodigoIntegracaoCliente { get; set; }

        public double CPF_CNPJ_Cliente { get; set; }

        public string TipoCliente { get; set; }

        public string DestinoCep { get; set; }

        public decimal PesoTotalPaletes { get; set; }

        public decimal PesoTotalPedido { get; set; }

        public decimal ValorTotalPedido { get; set; }

        public decimal NumeroPaletesFracionado { get; set; }

        public int NumeroPaletes { get; set; }

        public int QuantidadeMaiorQueEstoque { get; set; }

        public string CanalEntrega { get; set; }

        public string TipoOperacao { get; set; }

        public SituacaoPedido SituacaoPedido { get; set; }

        public string NumeroCargas { get; set; }

        public bool HabilitarCadastroArmazem { get; set; }

        public bool ExisteIntegracaoRouteasy { get; set; }

        public bool ReentregaSolicitada { get; set; }

        #endregion Propriedades

        #region Propriedades Com Regras

        public decimal TotalPallets
        {
            get { return NumeroPaletes + NumeroPaletesFracionado; }
        }

        public string DataCarregamentoPedidoFormatada
        {
            get { return this.DataCarregamentoPedido.HasValue ? this.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataPrevisaoEntregaFormatada
        {
            get { return this.DataPrevisaoEntrega.HasValue ? this.DataPrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DescricaoSituacaoRoteirizadorIntegracao
        {
            get
            {
                return SituacaoRoteirizadorIntegracao.ObterDescricao();
            }
        }

        public string ValorTotalPedidoFormatada
        {
            get { return this.ValorTotalPedido.ToString("N", new CultureInfo("pt-BR")); }
        }

        public string DestinoFormatada
        {
            get
            {
                if (this.CodigoIBGE != 9999999 || this.NomePais == null)
                    return this.Destino + " - " + this.SiglaEstado ?? "";
                else
                {
                    if (this.AbreviacaoPais != null)
                        return this.Destino + " - " + this.AbreviacaoPais;
                    else
                        return this.Destino + " - " + this.NomePais;
                }
            }
        }

        public string Destinatario
        {
            get
            {
                string descricao = string.Empty;

                string nome = this.NomeCliente;
                if (this.PontoTransbordoCliente)
                    nome = this.NomeFantasiaCliente;

                if (!string.IsNullOrWhiteSpace(this.CodigoIntegracaoCliente))
                    descricao += this.CodigoIntegracaoCliente + " - ";
                if (!string.IsNullOrWhiteSpace(nome))
                    descricao += nome;
                if (!string.IsNullOrWhiteSpace(this.TipoCliente))
                    descricao += " (" + this.CPF_CNPJ_Formatado + ")";

                return descricao;
            }
        }

        public string CPF_CNPJ_Formatado
        {
            get
            {
                if (this.TipoCliente?.Equals("E") ?? false)
                    return "00.000.000/0000-00";
                else
                    return (this.TipoCliente?.Equals("J") ?? false) ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CPF_CNPJ_Cliente) : String.Format(@"{0:000\.000\.000\-00}", this.CPF_CNPJ_Cliente);
            }
        }

        public string DT_RowColor
        {
            get
            {
                if (this.HabilitarCadastroArmazem && this.QuantidadeMaiorQueEstoque > 0)
                    return "#f9bfbc";

                if (this.ExisteIntegracaoRouteasy)
                    return SituacaoRoteirizadorIntegracao.ObterCorLinha();

                return string.Empty;
            }
        }

        public string DT_FontColor
        {
            get
            {
                if (this.HabilitarCadastroArmazem && this.QuantidadeMaiorQueEstoque > 0 || this.ExisteIntegracaoRouteasy)
                    return "#474747";

                return string.Empty;
            }
        }

        public string ReentregaSolicitadaFormatada
        {
            get
            {
                return this.ReentregaSolicitada.ObterDescricao();
            }
        }

        #endregion Propriedades Com Regras
    }
}