using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class AverbacaoMDFe : RepositorioBase<Dominio.Entidades.AverbacaoMDFe>, Dominio.Interfaces.Repositorios.AverbacaoMDFe
    {
        public AverbacaoMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.AverbacaoMDFe> BuscarPorMDFe(int codigoEmpresa, int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoMDFe>();

            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe && obj.MDFe.Empresa.Codigo == codigoEmpresa select obj;

            return result.ToList();
        }

        public List<int> BuscarAverbacoes(Dominio.Enumeradores.StatusAverbacaoMDFe situacao, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoMDFe>();

            query = query.Where(o => o.Status == situacao);

            return query.Select(o => o.Codigo).Take(maximoRegistros).ToList();
        }

        public int ContarPorMDFeTipoEStatus(int codigoMDFe, Dominio.Enumeradores.TipoAverbacaoMDFe tipo, Dominio.Enumeradores.StatusAverbacaoMDFe[] status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoMDFe>();

            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe && obj.Tipo == tipo && status.Contains(obj.Status) select obj;

            return result.Count();
        }

        public int ContarPorMDFeEStatus(int codigoMDFe, Dominio.Enumeradores.StatusAverbacaoMDFe status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoMDFe>();

            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe && obj.Status == status select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.AverbacaoMDFe> BuscarPorCodigoMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoMDFe>();

            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.AverbacaoMDFe> BuscarPorMDFeESituacao(int codigoMDFe, Dominio.Enumeradores.StatusAverbacaoMDFe situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoMDFe>();

            query = query.Where(o => o.MDFe.Codigo == codigoMDFe && o.Status == situacao);

            return query.ToList();
        }

        public bool ExistePorMDFeEApoliceSeguro(int codigoMDFe, int codigoApoliceSeguro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoMDFe>();

            query = query.Where(o => o.MDFe.Codigo == codigoMDFe && o.ApoliceSeguroAverbacao.Codigo == codigoApoliceSeguro);

            return query.Any();
        }

        public bool ExistePorCargaTipoEStatus(int codigoCarga, Dominio.Enumeradores.TipoAverbacaoMDFe tipoAverbacao, Dominio.Enumeradores.StatusAverbacaoMDFe[] statusAverbacaoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoMDFe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.Tipo == tipoAverbacao && statusAverbacaoMDFe.Contains(o.Status));

            return query.Any();
        }

        public List<Dominio.Entidades.AverbacaoMDFe> BuscarPorCargaTipoEStatus(int codigoCarga, Dominio.Enumeradores.TipoAverbacaoMDFe tipoAverbacao, Dominio.Enumeradores.StatusAverbacaoMDFe[] statusAverbacaoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoMDFe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.Tipo == tipoAverbacao && statusAverbacaoMDFe.Contains(o.Status));

            return query.ToList();
        }

        public bool ExistePorCargaTipoEStatus(int codigoCarga, Dominio.Enumeradores.TipoAverbacaoMDFe tipoAverbacao, Dominio.Enumeradores.StatusAverbacaoMDFe statusAverbacaoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoMDFe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.Tipo == tipoAverbacao && o.Status == statusAverbacaoMDFe);

            return query.Any();
        }

        public bool ExistePorCargaEStatus(int codigoCarga, Dominio.Enumeradores.StatusAverbacaoMDFe statusAverbacaoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoMDFe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.Status == statusAverbacaoMDFe);

            return query.Any();
        }

        public List<Dominio.Entidades.AverbacaoMDFe> BuscarPorCargaEStatus(int codigoCarga, Dominio.Enumeradores.StatusAverbacaoMDFe[] statusAverbacaoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoMDFe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && statusAverbacaoMDFe.Contains(o.Status));

            return query.ToList();
        }

        public Dominio.Entidades.AverbacaoMDFe BuscarPorCodigoECarga(int codigoAverbacaoMDFe, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoMDFe>();

            query = query.Where(o => o.Codigo == codigoAverbacaoMDFe && o.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.AverbacaoMDFe BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoMDFe>();

            var result = from obj in query where obj.ArquivosIntegracao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.AverbacaoMDFe> ConsultaPorCarga(int carga, int codigoCancelamentoCarga, int numeroMDFe, string apolice, Dominio.Enumeradores.StatusAverbacaoMDFe? situacao, string propOrdenacao, string dirOrdena, int inicio, int limite)
        {
            var query = _ConsultaPorCarga(carga, codigoCancelamentoCarga, numeroMDFe, apolice, situacao);

            return query.Fetch(o => o.ApoliceSeguroAverbacao).ThenFetch(o => o.ApoliceSeguro).ThenFetch(o => o.Seguradora)
                        .OrderBy(propOrdenacao + " " + dirOrdena)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();
        }

        public int ContarConsultaPorCarga(int carga, int codigoCancelamentoCarga, int numeroMDFe, string apolice, Dominio.Enumeradores.StatusAverbacaoMDFe? situacao)
        {
            var query = _ConsultaPorCarga(carga, codigoCancelamentoCarga, numeroMDFe, apolice, situacao);

            return query.Count();
        }

        public List<Dominio.Entidades.AverbacaoMDFe> BuscarPorApoliceSeguroAverbacao(int codigoApoliceSeguroAverbacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoMDFe>();

            query = query.Where(o => o.ApoliceSeguroAverbacao.Codigo == codigoApoliceSeguroAverbacao);

            return query.ToList();
        }

        public int ContarConsultaRelatorioMDFes(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMDFesAverbadosRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaMDFesAverbacao().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);
            return query.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.MDFe.MDFesAverbados> ConsultarRelatorioMDFes(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMDFesAverbadosRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaMDFesAverbacao().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.MDFe.MDFesAverbados)));
            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.MDFe.MDFesAverbados>();
        }

        public List<Dominio.Entidades.AverbacaoMDFe> BuscarPorCargaCancelamento(int codigoCarga, int codigoCargaCancelamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoMDFe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CargaCancelamento == null select obj;

            var queryCargaMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            var resultQueryCargaMDFe = from obj in queryCargaMDFe where obj.CargaCancelamento.Codigo == codigoCargaCancelamento select obj;
            result = result.Where(o => resultQueryCargaMDFe.Where(a => a.MDFe.Codigo == o.MDFe.Codigo).Any());

            return result.ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.AverbacaoMDFe> _ConsultaPorCarga(int carga, int codigoCancelamentoCarga, int numeroMDFe, string apolice, Dominio.Enumeradores.StatusAverbacaoMDFe? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoMDFe>();

            query = query.Where(o => o.Carga.Codigo == carga);

            if (codigoCancelamentoCarga > 0)
                query = query.Where(o => o.CargaCancelamento.Codigo == codigoCancelamentoCarga || o.CargaCancelamento == null);
            else
                query = query.Where(o => o.CargaCancelamento == null || o.CargaCancelamento.TipoCancelamentoCargaDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCargaDocumento.Carga);

            if (numeroMDFe > 0)
                query = query.Where(o => o.MDFe.Numero == numeroMDFe);

            if (!string.IsNullOrWhiteSpace(apolice))
                query = query.Where(o => o.ApoliceSeguroAverbacao.ApoliceSeguro.NumeroApolice.Contains(apolice));

            if (situacao.HasValue)
                query = query.Where(o => o.Status == situacao.Value);

            return query;
        }

        #endregion
    }
}
