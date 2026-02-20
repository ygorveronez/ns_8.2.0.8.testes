using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Threading;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao>
    {
        public CargaIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTiposPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao>();
            var resut = from obj in query where obj.Carga.Codigo == carga select obj.TipoIntegracao.Tipo;
            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao> BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao>();
            var resut = from obj in query where obj.Carga.Codigo == carga select obj;
            return resut.Fetch(o => o.TipoIntegracao).ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao>> BuscarPorCargaAsync(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao>();
            var result = from obj in query where obj.Carga.Codigo == carga select obj;
            return await result.Fetch(o => o.TipoIntegracao).ToListAsync();
        }

        public bool PossuiIntegracaoBloqueadaParaCancelamento(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao>();
            var resut = from obj in query
                        where
                            obj.Carga.Codigo == carga
                            && obj.BloquearCancelamentoCarga
                        select obj;
            return resut.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao> BuscarPorCarga(int carga, bool gerarIntegracaoDadosTransporteCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao>();

            query = query.Where(o => o.Carga.Codigo == carga && o.TipoIntegracao.GerarIntegracaoDadosTransporteCarga);

            return query.Fetch(o => o.TipoIntegracao).ToList();
        }

        public int ContarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao>();

            var resut = from obj in query where obj.Carga.Codigo == carga select obj;

            return resut.Count();
        }

        public int ContarPorCargaETipoIntegracao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao>();

            var resut = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Tipo == tipoIntegracao select obj.Codigo;

            return resut.Count();
        }

        public bool ExistePorTipoIntegracao(int codigoCarga, int codigoTipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.TipoIntegracao.Codigo == codigoTipoIntegracao);

            return query.Any();
        }

        public bool ExistePorCargaETipo(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.TipoIntegracao.Tipo == tipoIntegracao);

            return query.Select(o => o.Codigo).Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracao BuscarPorCargaETipo(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegraca)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao>();
            var resut = from obj in query where obj.Carga.Codigo == carga && obj.TipoIntegracao.Tipo == tipoIntegraca select obj;
            return resut.Fetch(o => o.TipoIntegracao).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracao BuscarPorCargaETipoIntegracao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao>();

            var resut = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Tipo == tipoIntegracao select obj;

            return resut.FirstOrDefault();
        }

        public void DeletarPorCargaETipoIntegracao(int codigoCarga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoes)
        {
            string sqlQuery = "DELETE FROM CargaIntegracao WHERE Codigo IN (SELECT c.Codigo FROM CargaIntegracao c WHERE c.Carga.Codigo = :codigoCarga AND c.TipoIntegracao.Tipo in (:tipoIntegracao))";

            UnitOfWork.Sessao.CreateQuery(sqlQuery).SetInt32("codigoCarga", codigoCarga).SetParameterList("tipoIntegracao", tiposIntegracoes).ExecuteUpdate();
        }

        #endregion

        #region Relatório Integrações

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao.CargaIntegracao> ConsultarRelatorioCargaIntegracao(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaCargaIntegracao().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao.CargaIntegracao)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao.CargaIntegracao>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao.CargaIntegracao>> ConsultarRelatorioCargaIntegracaoAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaCargaIntegracao().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao.CargaIntegracao)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao.CargaIntegracao>();
        }


        public int ContarConsultaRelatorioCargaIntegracao(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaCargaIntegracao().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
