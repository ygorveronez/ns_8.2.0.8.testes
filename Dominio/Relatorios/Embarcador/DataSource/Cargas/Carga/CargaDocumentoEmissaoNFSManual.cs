using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga
{
    public class CargaDocumentoEmissaoNFSManual
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string NumeroCarga { get; set; }
        public int NumeroOcorrencia { get; set; }
        public int Numero { get; set; }
        public string Serie { get; set; }
        public string ModeloDocumento { get; set; }
        public string Descricao { get; set; }
        public string Chave { get; set; }
        public decimal Peso { get; set; }
        public string DataEmissao { get; set; }
        public string ModeloDocumentoFiscal { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorPrestacaoServico { get; set; }
        public decimal BaseCalculoISS { get; set; }
        public decimal AliquotaISS { get; set; }
        public decimal ValorISS { get; set; }
        public decimal PercentualRetencaoISS { get; set; }
        public decimal ValorRetencaoISS { get; set; }
        public string IncluirISSBaseCalculo { get; set; }
        public int NumeroNFS { get; set; }
        public int SerieNFS { get; set; }
        public SituacaoLancamentoNFSManual SituacaoNFS { get; set; }
        public string DataEmissaoNFSManual { get; set; }
        public string NomeRemetente { get; set; }
        public string TipoRemetente { get; set; }
        public double CPFCNPJRemetente { get; set; }
        public string NomeDestinatario { get; set; }
        public string TipoDestinatario { get; set; }
        public double CPFCNPJDestinatario { get; set; }
        public string NomeTomador { get; set; }
        public string TipoTomador { get; set; }
        public double CPFCNPJTomador { get; set; }
        public string LocalidadePrestacao { get; set; }
        public string EstadoPrestacao { get; set; }
        public string Empresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string Filial { get; set; }
        public string NFSGerada { get; set; }
        public string NumeroPedidoCliente { get; set; }
        public string Observacao { get; set; }
        public string GrupoPessoasTomador { get; set; }
        public string TipoOperacao { get; set; }
        public decimal ValorNotaFiscal { get; set; }
        public string CEPOrigem { get; set; }
        public string CEPDestino { get; set; }
        public string CpfCnpjEmpresa { get; set; }
        public string VeiculoCarga { get; set; }
        public string ModeloVeicularCarga { get; set; }

        #endregion

        #region Propriedades com Regras

        public string CPFCNPJRemetenteFormatado
        {
            get
            {
                if (CPFCNPJRemetente > 0d)
                {
                    if (TipoRemetente == "J")
                        return string.Format(@"{0:00\.000\.000\/0000\-00}", CPFCNPJRemetente);
                    else if (TipoRemetente == "F")
                        return string.Format(@"{0:000\.000\.000\-00}", CPFCNPJRemetente);
                }

                return "";
            }
        }

        public string CPFCNPJDestinatarioFormatado
        {
            get
            {
                if (CPFCNPJDestinatario > 0d)
                {
                    if (TipoDestinatario == "J")
                        return string.Format(@"{0:00\.000\.000\/0000\-00}", CPFCNPJDestinatario);
                    else if (TipoDestinatario == "F")
                        return string.Format(@"{0:000\.000\.000\-00}", CPFCNPJDestinatario);
                }

                return "";
            }
        }

        public string CPFCNPJTomadorFormatado
        {
            get
            {
                if (CPFCNPJTomador > 0d)
                {
                    if (TipoTomador == "J")
                        return string.Format(@"{0:00\.000\.000\/0000\-00}", CPFCNPJTomador);
                    else if (TipoTomador == "F")
                        return string.Format(@"{0:000\.000\.000\-00}", CPFCNPJTomador);
                }

                return "";
            }
        }

        public string CpfCnpjEmpresaFormatado
        {
            get
            {
                return string.Format(@"{0:00\.000\.000\/0000\-00}", CpfCnpjEmpresa);
            }
        }

        public string SituacaoNFSFormatada
        {
            get { return SituacaoNFS.ObterDescricao(); }
        }

        public string VeiculoCargaFormatada
        {
            get { return string.IsNullOrWhiteSpace(VeiculoCarga) ? string.Empty : $"{VeiculoCarga.Substring(0, 3)}-{VeiculoCarga.Substring(3, 4)}"; }
        }

        #endregion
    }
}
