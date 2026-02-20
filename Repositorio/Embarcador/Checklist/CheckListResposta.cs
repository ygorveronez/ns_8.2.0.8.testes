using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Repositorio.Embarcador.Checklist.Consultas;

namespace Repositorio.Embarcador.Checklist
{
    public class CheckListResposta : RepositorioBase<Dominio.Entidades.Embarcador.Checklist.CheckListResposta>
    {
        #region Construtores

        public CheckListResposta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Checklist.CheckListResposta> Consultar(DateTime dataInicial, DateTime dataFinal)
        {
            var consultaChecklist = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Checklist.CheckListResposta>();

            if (dataInicial != DateTime.MinValue)
                consultaChecklist = consultaChecklist.Where(o => o.DataResposta.Date >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                consultaChecklist = consultaChecklist.Where(o => o.DataResposta.Date < dataFinal.AddDays(1));

            return consultaChecklist;
        }

        #endregion

        #region Métodos Públicos


        public Dominio.Entidades.Embarcador.Checklist.CheckListResposta BuscarPorUsuarioEData(int Usuario, DateTime data)
        {
            var consultaChecklist = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Checklist.CheckListResposta>()
                .Where(o => o.Usuario.Codigo == Usuario && o.DataResposta == data);

            return consultaChecklist.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Checklist.CheckListResposta BuscarPorCodigo(int codigo)
        {
            var consultaChecklist = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Checklist.CheckListResposta>()
                .Where(o => o.Codigo == codigo);

            return consultaChecklist.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Checklist.CheckListResposta> Consultar(DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaChecklist = Consultar(dataInicial, dataFinal);

            return ObterLista(consultaChecklist, parametrosConsulta);
        }

        public int ContarConsulta(DateTime dataInicial, DateTime dataFinal)
        {
            var consultaChecklist = Consultar(dataInicial, dataFinal);

            return consultaChecklist.Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CheckListsUsuario.CheckListUsuario> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.CheckListsUsuario.FiltroPesquisaCheckListUsuario filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCheque = new ConsultaCheckListUsuario().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCheque.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CheckListsUsuario.CheckListUsuario)));

            return consultaCheque.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.CheckListsUsuario.CheckListUsuario>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.CheckListsUsuario.FiltroPesquisaCheckListUsuario filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaCheque = new ConsultaCheckListUsuario().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaCheque.SetTimeout(600).UniqueResult<int>();
        }
        #endregion
    }
}
