using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public class ExtracaoMassivaTabelaFrete
    {
        #region Parâmetros da auditoria
        public DateTime DataAlteracao { get; set; }
        public string DescricaoDataAlteracao
        {
            get
            {
                return DataAlteracao == DateTime.MinValue ? "" : DataAlteracao.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }

        public string UsuarioAlteracao { get; set; }

        public int CodigoAlteracaoOrigem { get; set; }
        public int CodigoAlteracaoAtual { get; set; }
        public bool Exclusao { get; set; }
        public string DescricaoAcaoAuditoria { get; set; }

        #endregion

        #region Parâmetros da tabela de frete

        public int CodigoTabelaFrete { get; set; }

        public int CodigoTabelaFreteCliente { get; set; }

        public int CodigoTabelaFreteClienteOriginal { get; set; }

        public string DescricaoTabelaFrete { get; set; }

        public virtual bool SituacaoTabelaFrete { get; set; }
        public string DescricaoSituacaoTabelaFrete
        {
            get
            {
                if (this.SituacaoTabelaFrete)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public string DescricaoOrigemAntes { get; set; }
        public string DescricaoOrigemDepois { get; set; }

        public string DescricaoDestinoAntes { get; set; }
        public string DescricaoDestinoDepois { get; set; }

        public string DataInicialVigenciaAntes { get; set; }
        public DateTime DataHoraInicialVigenciaDepois { get; set; }
        public string DataInicialVigenciaDepois
        {
            get
            {
                return DataHoraInicialVigenciaDepois == DateTime.MinValue ? "" : DataHoraInicialVigenciaDepois.ToString("dd/MM/yyyy") ?? "";
            }
        }

        public string DataFinalVigenciaAntes { get; set; }
        public DateTime DataHoraFinalVigenciaDepois { get; set; }
        public string DataFinalVigenciaDepois
        {
            get
            {
                return DataHoraFinalVigenciaDepois == DateTime.MinValue ? "" : DataHoraFinalVigenciaDepois.ToString("dd/MM/yyyy");
            }
        }
        public string AprovadorTabelaFreteCliente { get; set; }

        #endregion

        #region Parâmetros da base de cálculo

        public TipoParametroBaseTabelaFrete TipoParametroBaseTabelaFrete { get; set; }

        public string DescricaoTipoParametroBaseTabelaFrete
        {
            get
            {
                return TipoParametroBaseTabelaFreteHelper.ObterDescricao(TipoParametroBaseTabelaFrete);
            }
        }

        public virtual int CodigoObjetoParametroBaseCalculo { get; set; }

        public string DescricaoObjetoParametroBaseCalculo { get; set; }

        public string DescricaoValorMinimoGarantidoParametroBaseCalculoAntes { get; set; }
        public decimal ValorMinimoGarantidoParametroBaseCalculoDepois { get; set; }
        public string DescricaoValorMinimoGarantidoParametroBaseCalculoDepois
        {
            get
            {
                return ValorMinimoGarantidoParametroBaseCalculoDepois.ToString("N4");
            }
        }

        public string DescricaoValorEntregaExcedenteParametroBaseCalculoAntes { get; set; }
        public decimal ValorEntregaExcedenteParametroBaseCalculoDepois { get; set; }
        public string DescricaoValorEntregaExcedenteParametroBaseCalculoDepois
        {
            get
            {
                return ValorEntregaExcedenteParametroBaseCalculoDepois.ToString("N4");
            }
        }

        public string DescricaoValorPalletExcedenteParametroBaseCalculoAntes { get; set; }
        public decimal ValorPalletExcedenteParametroBaseCalculoDepois { get; set; }
        public string DescricaoValorPalletExcedenteParametroBaseCalculoDepois
        {
            get
            {
                return ValorPalletExcedenteParametroBaseCalculoDepois.ToString("N4");
            }
        }

        public string DescricaoValorQuilometragemExcedenteParametroBaseCalculoAntes { get; set; }
        public decimal ValorQuilometragemParametroBaseCalculoDepois { get; set; }
        public string DescricaoValorQuilometragemExcedenteParametroBaseCalculoDepois
        {
            get
            {
                return ValorQuilometragemParametroBaseCalculoDepois.ToString("N4");
            }
        }

        public string DescricaoValorPesoExcedenteParametroBaseCalculoAntes { get; set; }
        public decimal ValorPesoExcedenteParametroBaseCalculoDepois { get; set; }
        public string DescricaoValorPesoExcedenteParametroBaseCalculoDepois
        {
            get
            {
                return ValorPesoExcedenteParametroBaseCalculoDepois.ToString("N4");
            }
        }

        public string DescricaoValorAjudanteExcedenteParametroBaseCalculoAntes { get; set; }
        public decimal ValorAjudanteExcedenteParametroBaseCalculoDepois { get; set; }
        public string DescricaoValorAjudanteExcedenteParametroBaseCalculoDepois
        {
            get
            {
                return ValorAjudanteExcedenteParametroBaseCalculoDepois.ToString("N4");
            }
        }

        public string DescricaoValorMaximoParametroBaseCalculoAntes { get; set; }
        public decimal ValorMaximoParametroBaseCalculoDepois { get; set; }
        public string DescricaoValorMaximoParametroBaseCalculoDepois
        {
            get
            {
                return ValorMaximoParametroBaseCalculoDepois.ToString("N4");
            }
        }

        public string DescricaoPercentualPagamentoAgregadoParametroBaseCalculoAntes { get; set; }
        public decimal PercentualPagamentoAgregadoParametroBaseCalculoDepois { get; set; }
        public string DescricaoPercentualPagamentoAgregadoParametroBaseCalculoDepois
        {
            get
            {
                return PercentualPagamentoAgregadoParametroBaseCalculoDepois.ToString("N2");
            }
        }

        public string DescricaoValorBaseParametroBaseCalculoAntes { get; set; }
        public decimal ValorBaseParametroBaseCalculoDepois { get; set; }
        public string DescricaoValorBaseParametroBaseCalculoDepois
        {
            get
            {
                return ValorBaseParametroBaseCalculoDepois.ToString("N4");
            }
        }

        public string DescricaoValorHoraExcedenteParametroBaseCalculoAntes { get; set; }
        public decimal ValorHoraExcedenteParametroBaseCalculoDepois { get; set; }
        public string DescricaoValorHoraExcedenteParametroBaseCalculoDepois
        {
            get
            {
                return ValorHoraExcedenteParametroBaseCalculoDepois.ToString("N4");
            }
        }

        public string DescricaoValorPacoteExcedenteParametroBaseCalculoAntes { get; set; }
        public decimal ValorPacoteExcedenteParametroBaseCalculoDepois { get; set; }
        public string DescricaoValorPacoteExcedenteParametroBaseCalculoDepois
        {
            get
            {
                return ValorPacoteExcedenteParametroBaseCalculoDepois.ToString("N4");
            }
        }

        #endregion

        #region Parâmetros do item

        public string TipoAcao { get; set; }

        public TipoParametroBaseTabelaFrete TipoParametroObjetoItem { get; set; }
        public string DescricaoTipoParametroObjetoItem
        {
            get
            {
                return TipoParametroBaseTabelaFreteHelper.ObterDescricao(TipoParametroObjetoItem);
            }
        }

        public virtual int CodigoObjetoItem { get; set; }

        public string DescricaoObjetoItem { get; set; }

        public virtual TipoCampoValorTabelaFrete TipoValorItem { get; set; }
        public string DescricaoTipoValorItem
        {
            get
            {
                return TipoCampoValorTabelaFreteHelper.ObterDescricao(TipoValorItem);
            }
        }

        public string DescricaoValorItemAntes { get; set; }
        public decimal ValorItemDepois { get; set; }
        public string DescricaoValorItemDepois
        {
            get
            {
                return ValorItemDepois.ToString("N4");
            }
        }

        public long CodigoItem { get; set; }

        #endregion
    }
}
