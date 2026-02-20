using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga
{
    public sealed class CargaProdutoTransportador
    {
        #region Propriedades

        public int Codigo { get; set; }

        public string CargaProdutoDescricao { get; set; }

        public string CnpjTransportador { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public string CodigoProdutoEmbarcador { get; set; }

        public string DescricaoProduto { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }

        public decimal PesoUnitario { get; set; }

        public decimal Quantidade { get; set; }

        public string TipoFrete { get; set; }

        public string RazaoSocialTransportador { get; set; }

        public double CPFCNPJDestinatario { get; set; }

        public string TipoDestinatario { get; set; }

        public string Destinatario { get; set; }

        public string LocalidadeDestinatario { get; set; }

        public string EnderecoDestinatario { get; set; }

        public string BairroDestinatario { get; set; }

        public string NumeroEnderecoDestinatario { get; set; }

        public string ComplementoEnderecoDestinatario { get; set; }

        public SituacaoCargaJanelaCarregamento SituacaoJanelaCarregamento { get; set; }

        public DateTime DataCarregamentoCarga { get; set; }

        public decimal TotalPallets { get; set; }

        public string UnidadeMedida { get; set; }

        public int SequenciaRoteirizacao { get; set; }

        public string Bloco { get; set; }

        public int OrdemCarregamento { get; set; }

        public int OrdemEntrega { get; set; }

        public string GrupoProduto { get; set; }

        public decimal CubagemPedido { get; set; }

        private double CPFCNPJRecebedor { get; set; }

        public string RecebedorDescricao { get; set; }

        public string LocalidadeRecebedor { get; set; }

        public string EnderecoRecebedor { get; set; }

        public string BairroRecebedor { get; set; }

        public string NumeroEnderecoRecebedor { get; set; }

        public string ComplementoEnderecoRecebedor { get; set; }

        public string Origem { get; set; }

        public string Destino { get; set; }

        public int NotaFiscal { get; set; }

        #endregion

        #region Propriedades com Regras

        public decimal PesoTotal
        {
            get { return Quantidade * PesoUnitario; }
        }

        public string CPFCNPJDestinatarioFormatado
        {
            get
            {
                if (TipoDestinatario == "E")
                    return "00.000.000/0000-00";
                else
                    return TipoDestinatario == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CPFCNPJDestinatario) : string.Format(@"{0:000\.000\.000\-00}", CPFCNPJDestinatario);
            }
        }

        public string CPFCNPJRecebedorFormatado
        {
            get
            {
                return CPFCNPJRecebedor > 0 ? string.Format(@"{0:00\.000\.000\/0000\-00}", CPFCNPJRecebedor) : "";
            }
        }

        public string DescricaoSituacaoJanelaCarregamento
        {
            get { return SituacaoJanelaCarregamento.ObterDescricao(); }
        }

        public string DataCarregamentoFormatada
        {
            get { return DataCarregamentoCarga != DateTime.MinValue ? DataCarregamentoCarga.ToString("dd/MM/yyyy") : ""; }
        }

        public string Barcode
        {
            get
            {
                return $"*{CodigoCargaEmbarcador}*";
            }
        }

        #endregion
    }
}
