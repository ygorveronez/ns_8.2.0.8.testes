using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.GerenciamentoIrregularidades
{
    public class Irregularidade : RepositorioBase<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade>
    {
        #region Construtores

        public Irregularidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade> Consultar(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaIrregularidade filtrosPesquisa)
        {
            var consultaIrregularidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade>();

            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
                consultaIrregularidade = consultaIrregularidade.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Sequencia > 0)
                consultaIrregularidade = consultaIrregularidade.Where(obj => obj.Sequencia == filtrosPesquisa.Sequencia);

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoIntegracao))
                consultaIrregularidade = consultaIrregularidade.Where(obj => obj.CodigoIntegracao == filtrosPesquisa.CodigoIntegracao);

            if (filtrosPesquisa.CodigoPortfolioModuloControle > 0)
                consultaIrregularidade = consultaIrregularidade.Where(obj => obj.PortfolioModuloControle.Codigo == filtrosPesquisa.CodigoPortfolioModuloControle);

            if (filtrosPesquisa.SeguirAprovacaoTranspPrimeiro.HasValue)
                consultaIrregularidade = consultaIrregularidade.Where(obj => obj.SeguirAprovacaoTranspPrimeiro == filtrosPesquisa.SeguirAprovacaoTranspPrimeiro.Value);

            if (filtrosPesquisa.Situacao != SituacaoAtivaPesquisa.Todos)
            {
                if (filtrosPesquisa.Situacao == SituacaoAtivaPesquisa.Ativa)
                    consultaIrregularidade = consultaIrregularidade.Where(obj => obj.Ativa == true);
                else
                    consultaIrregularidade = consultaIrregularidade.Where(obj => obj.Ativa == false);
            }

            return consultaIrregularidade;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade BuscarPorCodigo(int codigo)
        {
            var consultaIrregularidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade>()
                .Where(obj => obj.Codigo == codigo);

            return consultaIrregularidade.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade> BuscarPorCodigos(List<int> codigos)
        {
            var consultaIrregularidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade>()
                .Where(obj => codigos.Contains(obj.Codigo));

            return consultaIrregularidade.ToList();
        }

        public List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade> Consultar(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaIrregularidade filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaIrregularidade = Consultar(filtrosPesquisa);

            return ObterLista(consultaIrregularidade, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaIrregularidade filtrosPesquisa)
        {
            var consultaIrregularidade = Consultar(filtrosPesquisa);

            return consultaIrregularidade.Count();
        }

        public bool ExisteDuplicidade(Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade irregularidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade>();
            query = query.Where(i => i.Codigo != irregularidade.Codigo);
            query = query.Where(i => i.Ativa == irregularidade.Ativa &&
                             i.CodigoIntegracao == irregularidade.CodigoIntegracao &&
                             i.Descricao == irregularidade.Descricao &&
                             i.PortfolioModuloControle.Codigo == irregularidade.PortfolioModuloControle.Codigo &&
                             i.SeguirAprovacaoTranspPrimeiro == irregularidade.SeguirAprovacaoTranspPrimeiro &&
                             i.Sequencia == irregularidade.Sequencia);

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade> BuscarIrregularidadesAtivas()
        {
            var consultaIrregularidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade>()
                .Where(obj =>obj.Ativa);

            return consultaIrregularidade.OrderBy(x => x.Sequencia).ToList();
        }

        public bool ExisteRegistroComMesmoGatilho(Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade irregularidade)
        {
            if (!irregularidade.Ativa)
                return false;

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade>();
            query = query.Where(x => x.Ativa == irregularidade.Ativa);
            query = query.Where(x => x.Codigo != irregularidade.Codigo);
            query = query.Where(x => x.GatilhoIrregularidade == irregularidade.GatilhoIrregularidade);

            return query.Any();
        }

        //public IList<Dominio.Relatorios.Embarcador.DataSource.GerenciamentoIrregularidades.RelatorioProcessamentoModuloControle> ConsultarRelatorioProcessamentoModuloControle(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaRelatorioProcessamentoModuloControle filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        //{
        //    var consultaProcessamentoModuloControle = this.SessionNHiBernate.CreateSQLQuery(new Repositorio.Embarcador.GerenciamentoIrregularidades.ConsultaProcessamentoModuloControle().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).StringQuery);

        //    consultaProcessamentoModuloControle.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.GerenciamentoIrregularidades.RelatorioProcessamentoModuloControle)));

        //    return consultaProcessamentoModuloControle.SetTimeout(1200).List<Dominio.Relatorios.Embarcador.DataSource.GerenciamentoIrregularidades.RelatorioProcessamentoModuloControle>();
        //}

        //public int ContarConsultaRelatorioPacotes(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaRelatorioProcessamentoModuloControle filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        //{
        //    var consultaProcessamentoModuloControle = this.SessionNHiBernate.CreateSQLQuery(new Repositorio.Embarcador.GerenciamentoIrregularidades.ConsultaProcessamentoModuloControle().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).StringQuery);

        //    return consultaProcessamentoModuloControle.SetTimeout(1200).UniqueResult<int>();
        //}


        //public IList<Dominio.Relatorios.Embarcador.DataSource.GerenciamentoIrregularidades.RelatorioModuloControle> ConsultarRelatorioProcessamentoModuloControle(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaRelatorioModuloControle filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        //{
        //    var consultaModuloControle = this.SessionNHiBernate.CreateSQLQuery(new Repositorio.Embarcador.GerenciamentoIrregularidades.ConsultaModuloControle().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).StringQuery);

        //    consultaModuloControle.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.GerenciamentoIrregularidades.RelatorioModuloControle)));

        //    return consultaModuloControle.SetTimeout(1200).List<Dominio.Relatorios.Embarcador.DataSource.GerenciamentoIrregularidades.RelatorioModuloControle>();
        //}

        //public int ContarConsultaRelatorioPacotes(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaRelatorioModuloControle filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        //{
        //    var consultaModuloControle = this.SessionNHiBernate.CreateSQLQuery(new Repositorio.Embarcador.GerenciamentoIrregularidades.ConsultaModuloControle().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).StringQuery);

        //    return consultaModuloControle.SetTimeout(1200).UniqueResult<int>();
        //}

        #endregion Métodos Públicos
    }
}
