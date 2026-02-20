using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio.Embarcador.PagamentoMotorista;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class CheckListCarga : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga>
    {
        #region Construtores

        public CheckListCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CheckListCarga(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga> Consultar(DateTime dataInicial, DateTime dataFinal, string carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCheckList? situacao)
        {
            var consultaChecklist = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga>()
                .Where(o =>
                    (o.Carga != null && o.Carga.OcultarNoPatio == false && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada) ||
                    (o.Carga == null && o.PreCarga != null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada)
                );

            if (dataInicial != DateTime.MinValue)
                consultaChecklist = consultaChecklist.Where(o => o.Carga.DataCarregamentoCarga.Value.Date >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                consultaChecklist = consultaChecklist.Where(o => o.Carga.DataCarregamentoCarga.Value.Date < dataFinal.AddDays(1));

            if (!string.IsNullOrWhiteSpace(carga))
                consultaChecklist = consultaChecklist.Where(o => o.Carga.CodigoCargaEmbarcador == carga || o.Carga.CodigosAgrupados.Contains(carga));

            if (situacao.HasValue)
                consultaChecklist = consultaChecklist.Where(o => o.Situacao == situacao);

            return consultaChecklist;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga BuscarPorFluxoGestaoPatio(int codigoFluxoGestaoPatio)
        {
            var consultaChecklist = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga>()
                .Where(o => o.FluxoGestaoPatio.Codigo == codigoFluxoGestaoPatio);

            return consultaChecklist.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga BuscarAnteriorPorFilialVeiculoETIpoOperacao(int codigoFilial, int codigoVeiculo, int codigoTipoOperacao)
        {
            var consultaChecklist = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga>()
                .Where(o =>
                    o.Situacao == SituacaoCheckList.Finalizado &&
                    o.CheckListCargaVigencia != null &&
                    o.CheckListCargaVigencia.Ativo &&
                    (o.CheckListCargaVigencia.TipoOperacao == null || o.CheckListCargaVigencia.TipoOperacao.Codigo == codigoTipoOperacao) &&
                    o.FluxoGestaoPatio.Filial.Codigo == codigoFilial &&
                    o.FluxoGestaoPatio.Veiculo.Codigo == codigoVeiculo
                );

            return consultaChecklist
                .OrderBy(checkList => checkList.CheckListCargaVigencia.TipoOperacao == null)
                .ThenByDescending(checkList => checkList.DataLiberacao)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga> BuscarPorFluxosGestaoPatio(List<int> listaCodigos)
        {
            var consultaChecklist = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga>()
                .Where(o => listaCodigos.Contains(o.FluxoGestaoPatio.Codigo));

            return consultaChecklist
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga> BuscarChecklistsPendentesPorMotorista(int codigoMotorista)
        {
            var consultaChecklist = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga>()
                .Where(o =>
                    o.Carga.Motoristas.Any(x => x.Codigo == codigoMotorista) &&
                    o.LicencaInvalida == true &&
                    o.Situacao == SituacaoCheckList.Aberto &&
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado
                );

            return consultaChecklist.ToList();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga BuscarPorCodigo(int codigo)
        {
            var consultaChecklist = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga>()
                .Where(o => o.Codigo == codigo);

            return consultaChecklist.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga> Consultar(DateTime dataInicial, DateTime dataFinal, string carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCheckList? situacao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaChecklist = Consultar(dataInicial, dataFinal, carga, situacao);

            return ObterLista(consultaChecklist, parametrosConsulta);
        }

        public int ContarConsulta(DateTime dataInicial, DateTime dataFinal, string carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCheckList? situacao)
        {
            var consultaChecklist = Consultar(dataInicial, dataFinal, carga, situacao);

            return consultaChecklist.Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.RelatorioCheckListCarga> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioCheckList filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCheckList = new ConsultaCheckList().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCheckList.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.RelatorioCheckListCarga)));

            return consultaCheckList.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.RelatorioCheckListCarga>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioCheckList filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaCheckList = new ConsultaCheckList().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaCheckList.SetTimeout(600).UniqueResult<int>();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga BuscarPorFluxoGestaoPatioEEtapaCheckList(int codigoFluxoGestaoPatio, EtapaCheckList etapaCheckList)
        {
            var consultaChecklist = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga>()
                .Where(o => o.FluxoGestaoPatio.Codigo == codigoFluxoGestaoPatio && ((EtapaCheckList?)o.EtapaCheckList ?? EtapaCheckList.Checklist) == etapaCheckList);

            return consultaChecklist.FirstOrDefault();
        }

        #endregion
    }
}
