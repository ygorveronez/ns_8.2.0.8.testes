using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.AlcadasCarga
{
    public class RegraAutorizacaoCarga : RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga>
    {
        #region Construtores

        public RegraAutorizacaoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga> BuscarAtivas(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoCarga tipoRegra)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga> consultaRegraAutorizacaoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga>()
                .Where(o => (
                    o.Ativo &&
                    (o.Vigencia == null || o.Vigencia.Value.Date >= DateTime.Now.Date) &&
                    (o.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoCarga.Ambos || o.TipoRegra == tipoRegra) &&
                    (
                        o.RegraPorComponenteFrete ||
                        o.RegraPorFilial ||
                        o.RegraPorModeloVeicularCarga ||
                        o.RegraPorTipoCarga ||
                        o.RegraPorTipoOperacao ||
                        o.RegraPorValorFrete ||
                        o.RegraPorPesoContainer ||
                        o.RegraPorTomador ||
                        o.RegraPorPortoDestino ||
                        o.RegraPorPortoOrigem ||
                        o.RegraPorMotivoSolicitacaoFrete ||
                        o.RegraPorPercentualAcrescimoValorTabelaFrete ||
                        o.RegraPorPercentualDiferencaFreteLiquidoFreteTerceiro ||
                        o.RegraPorPercentualDiferencaFreteLiquidoTotalFreteTerceiro ||
                        o.RegraPorPercentualDescontoValorTabelaFrete ||
                        o.RegraPorValorAcrescimoValorTabelaFrete ||
                        o.RegraPorPercentualFreteSobreNota ||
                        o.RegraPorDiferencaValorFrete ||
                        o.RegraPorAutorizacaoTipoTerceiro
                    )
                )
            );

            return consultaRegraAutorizacaoCarga.ToList();
        }

        #endregion
    }
}
