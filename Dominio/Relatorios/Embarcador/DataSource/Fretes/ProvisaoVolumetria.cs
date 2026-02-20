using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Linq;

namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public class ProvisaoVolumetria
    {
        #region Propriedades

        public int Codigo { get; set; }
        private string CNPJEmpresa { get; set; }
        public string Empresa { get; set; }
        private string Placa { get; set; }
        public string TipoOperacao { get; set; }
        public string ModeloVeicular { get; set; }
        public string NumeroCarga { get; set; }
        private DateTime DataEmissao { get; set; }
        public DateTime DataMigracaoNF { get; set; }
        public int NotaFiscal { get; set; }
        public string SerieNotaFiscal { get; set; }
        public string ChaveNFe { get; set; }
        public string CFOP { get; set; }
        public int Volumes { get; set; }
        public string Origem { get; set; }
        public string UFOrigem { get; set; }
        public string Destino { get; set; }
        public string UFDestino { get; set; }
        private double CNPJRemetente { get; set; }
        private string TipoPessoaRemetente { get; set; }
        public string Remetente { get; set; }
        private double CNPJDestinatario { get; set; }
        private string TipoPessoaDestinatario { get; set; }
        private Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }
        private DateTime DataEmissaoCTE { get; set; }
        public string CTe { get; set; }
        public string SerieCTe { get; set; }
        private string TipoCTe { get; set; }
        public string ChaveCTe { get; set; }
        private DateTime DataDevolucaoCTRC { get; set; }
        public string DataImportacao { get; set; }
        private DateTime DataAprovacao { get; set; }
        public string NomeAprovador { get; set; }
        public string CodUC { get; set; }
        public string DescricaoUC { get; set; }
        public string CodContaContabil { get; set; }
        public string DescricaoContaContabil { get; set; }
        public string CIA { get; set; }
        public string Filial { get; set; }
        public string CodFilial { get; set; }
        public string Mercado { get; set; }
        public string Diretoria { get; set; }
        public string ContaGerencial { get; set; }
        public string VA { get; set; }
        public string DocEnviadoRI { get; set; }
        private DateTime DataAprovacaoPagamento { get; set; }
        private DateTime DataIntegracaoPagamento { get; set; }
        public string NumeroPagamento { get; set; }
        private SituacaoPagamento SituacaoPagamento { get; set; }
        public string ErroIntegracaoPagamento { get; set; }
        private DateTime DataPagamento { get; set; }
        private string SituacaoCarga { get; set; }
        public string Pago { get; set; }

        private decimal ValorComponentes { get; set; }
        private decimal Frete { get; set; }
        private string CST { get; set; }
        private decimal _ICMS { get; set; }
        private decimal _ValorISS { get; set; }
        private decimal ValorISSRetido { get; set; }

        #endregion

        #region Propriedades com Regras

        public string CNPJEmpresaFormatada
        {
            get
            {
                return String.Format(@"{0:00\.000\.000\/0000\-00}", CNPJEmpresa);
            }
        }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataMigracaoNFFormatada
        {
            get { return DataMigracaoNF != DateTime.MinValue ? DataMigracaoNF.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string CNPJRemetenteFormatado
        {
            get
            {
                if (TipoPessoaRemetente == "F")
                    return String.Format(@"{0:000\.000\.000\-00}", CNPJRemetente);
                else
                    return String.Format(@"{0:00\.000\.000\/0000\-00}", CNPJRemetente);
            }
        }

        public string CNPJDestinatarioFormatado
        {
            get
            {
                if (TipoPessoaDestinatario == "F")
                    return String.Format(@"{0:000\.000\.000\-00}", CNPJDestinatario);
                else
                    return String.Format(@"{0:00\.000\.000\/0000\-00}", CNPJDestinatario);
            }
        }

        public string DescricaoTipoTomador
        {
            get
            {
                switch (TipoTomador)
                {
                    case Dominio.Enumeradores.TipoTomador.Remetente:
                        return "Remetente";
                    case Dominio.Enumeradores.TipoTomador.Expedidor:
                        return "Expedidor";
                    case Dominio.Enumeradores.TipoTomador.Recebedor:
                        return "Recebedor";
                    case Dominio.Enumeradores.TipoTomador.Destinatario:
                        return "Destinatário";
                    case Dominio.Enumeradores.TipoTomador.Outros:
                        return "Outro";
                    default:
                        return "";
                }
            }
        }

        public string DataEmissaoCTEFormatada
        {
            get { return DataEmissaoCTE != DateTime.MinValue ? DataEmissaoCTE.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TipoCTeFormatado
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TipoCTe))
                    return TipoCTe;

                string[] listatipos = TipoCTe.Split(',');

                return string.Join(", ", (from tipo in listatipos select TipoCTeHelper.ObterDescricao((TipoCTE)tipo.ToInt())));
            }
        }

        public string DataDevolucaoCTRCFormatada
        {
            get { return DataDevolucaoCTRC != DateTime.MinValue ? DataDevolucaoCTRC.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public decimal TotalReceber
        {
            get
            {
                return ValorComponentes + Frete + ICMS + ValorISS;
            }
        }

        public decimal ValorISS
        {
            get
            {
                return _ValorISS - ValorISSRetido;
            }
        }

        public decimal ICMS
        {
            get
            {
                return CST != "60" ? _ICMS : 0;
            }
        }

        public decimal FreteTotalSemImposto
        {
            get
            {
                return ValorComponentes + Frete;
            }
        }

        public string DataAprovacaoFormatada
        {
            get { return DataAprovacao != DateTime.MinValue ? DataAprovacao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataAprovacaoPagamentoFormatada
        {
            get { return DataAprovacaoPagamento != DateTime.MinValue ? DataAprovacaoPagamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataIntegracaoPagamentoFormatada
        {
            get { return DataIntegracaoPagamento != DateTime.MinValue ? DataIntegracaoPagamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string SituacaoPagamentoFormatada
        {
            get { return SituacaoPagamento.ObterDescricao(); }
        }

        public string DataPagamentoFormatada
        {
            get { return DataPagamento != DateTime.MinValue ? DataPagamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string SituacaoCargaFormatada
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SituacaoCarga))
                    return SituacaoCarga;

                string[] listaSituacoes = SituacaoCarga.Split(',');

                return string.Join(", ", (from situacao in listaSituacoes select SituacaoCargaHelper.ObterDescricao((SituacaoCarga)situacao.ToInt())));
            }
        }

        public string PlacaFormatada { get => Placa.ObterPlacaFormatada(); }

        #endregion
    }
}
