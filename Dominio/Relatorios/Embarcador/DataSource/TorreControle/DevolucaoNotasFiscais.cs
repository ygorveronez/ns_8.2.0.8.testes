using System;

namespace Dominio.Relatorios.Embarcador.DataSource.TorreControle
{
    public class DevolucaoNotasFiscais
    {
        #region Propriedades

        public int Codigo { get; set; }
        public DateTime DataEmissaoNFOrigem { get; set; }
        public DateTime DataEmissaoNFD { get; set; }
        public int NotaFiscalOrigem { get; set; }
        public string Carga { get; set; }
        public string GrupoTipoOperacao { get; set; }
        public string TipoOperacao { get; set; }
        public int Chamado { get; set; }
        public string UFOrigemNota { get; set; }
        public string CidadeOrigemNota { get; set; }
        public string Filial { get; set; }
        public DateTime DataFinalizacaoOcorrencia { get; set; }
        public string PedidoEmbarcador { get; set; }
        public string PedidoCliente { get; set; }
        public string MotivoAtendimento { get; set; }
        public string CNPJTransportadora { get; set; }
        public string Transportadora { get; set; }
        public int NumeroNotaDevolucao { get; set; }
        public string CodigoIntegracaoCliente { get; set; }
        public string Cliente { get; set; }
        public decimal ValorNotaFiscalOrigem { get; set; }
        public decimal PesoNotaFiscalOrigem { get; set; }
        public string CodigoProduto { get; set; }
        public string DescricaoProduto { get; set; }
        public decimal QuantidadeDevolvida { get; set; }
        public decimal PesoProdutoNFD { get; set; }
        public decimal ValorProdutoNFD { get; set; }
        public bool TipoDevolucao { get; set; }

        #endregion

        #region Propriedades Formatadas

        public virtual string DataEmissaoNFOrigemFormatada
        {
            get { return DataEmissaoNFOrigem != DateTime.MinValue ? DataEmissaoNFOrigem.ToString("dd/MM/yyyy HH:mm") : ""; }
        }
        public virtual string DataEmissaoNFDFormatada
        {
            get { return DataEmissaoNFD != DateTime.MinValue ? DataEmissaoNFD.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public virtual string DataFinalizacaoOcorrenciaFormatada
        {
            get { return DataFinalizacaoOcorrencia != DateTime.MinValue ? DataFinalizacaoOcorrencia.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public virtual string TipoDevolucaoFormatada
        {
            get
            {
                if (TipoDevolucao)
                    return "Parcial";
                else if (!TipoDevolucao)
                    return "Total";
                else
                    return "";
            }
        }

        #endregion
    }
}
