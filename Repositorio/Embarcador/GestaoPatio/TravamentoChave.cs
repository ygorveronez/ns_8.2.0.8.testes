using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class TravamentoChave : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave>
    {
        #region Construtores

        public TravamentoChave(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave> Consultar(SituacaoTravamentoChave? situacao, DateTime dataInicial, DateTime dataFinal, string carga)
        {
            var consultaTravamentoChave = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave>()
                .Where(o =>
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado &&
                    (o.Carga != null && o.Carga.OcultarNoPatio == false && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada) ||
                    (o.Carga == null && o.PreCarga != null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada)
                );

            if (situacao.HasValue)
                consultaTravamentoChave = consultaTravamentoChave.Where(o => o.Situacao == situacao);

            if (dataInicial != DateTime.MinValue)
                consultaTravamentoChave = consultaTravamentoChave.Where(o => o.Carga.DataCarregamentoCarga.Value.Date >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                consultaTravamentoChave = consultaTravamentoChave.Where(o => o.Carga.DataCarregamentoCarga.Value.Date <= dataFinal);

            if (!string.IsNullOrWhiteSpace(carga))
                consultaTravamentoChave = consultaTravamentoChave.Where(o => o.Carga.CodigoCargaEmbarcador == carga || o.Carga.CodigosAgrupados.Contains(carga));

            return consultaTravamentoChave;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave BuscarPorCodigo(int codigo)
        {
            var consultaTravamentoChave = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave>()
                .Where(o => o.Codigo == codigo);

            return consultaTravamentoChave.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave BuscarPorFluxoGestaoPatio(int codigoFluxoGestaoPatio)
        {
            var consultaTravamentoChave = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave>()
                .Where(o => o.FluxoGestaoPatio.Codigo == codigoFluxoGestaoPatio);

            return consultaTravamentoChave.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave BuscarTravamentoChaveMotoristaPorFluxoGestaoPatio(int codigoFluxoGestaoPatio)
        {
            var consultaTravamentoChave = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave>()
                .Where(o => o.FluxoGestaoPatio.Codigo == codigoFluxoGestaoPatio);

            return consultaTravamentoChave.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave> Consultar(SituacaoTravamentoChave? situacao, DateTime dataInicial, DateTime dataFinal, string carga, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaTravamentoChave = Consultar(situacao, dataInicial, dataFinal, carga);

            return ObterLista(consultaTravamentoChave, parametrosConsulta);
        }

        public int ContarConsulta(SituacaoTravamentoChave? situacao, DateTime dataInicial, DateTime dataFinal, string carga)
        {
            var result = Consultar(situacao, dataInicial, dataFinal, carga);

            return result.Count();
        }

        #endregion
    }
}
