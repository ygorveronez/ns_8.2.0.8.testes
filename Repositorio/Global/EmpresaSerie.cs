using NHibernate.Linq;
using Repositorio.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;

namespace Repositorio
{
    public class EmpresaSerie : RepositorioBase<Dominio.Entidades.EmpresaSerie>, Dominio.Interfaces.Repositorios.EmpresaSerie
    {
        public EmpresaSerie(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public EmpresaSerie(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.EmpresaSerie BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int BuscarProximoNumeroDocumentoPorSerie(int codigoEmpresa, int serie, Dominio.Enumeradores.TipoSerie tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Numero == serie && obj.Tipo == tipo select obj;
            if (result.Count() > 0)
            {
                if (result.FirstOrDefault().ProximoNumeroDocumento > 0)
                    return result.FirstOrDefault().ProximoNumeroDocumento;
                else
                    return 1;
            }
            else
                return 1;
        }

        public Dominio.Entidades.EmpresaSerie BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.EmpresaSerie> BuscarTodosPorEmpresa(int codigoEmpresa, Dominio.Enumeradores.TipoSerie tipo, string status = "")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Tipo == tipo select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.ToList();
        }

        public List<Dominio.Entidades.EmpresaSerie> BuscarTodosPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.ToList();
        }

        public Dominio.Entidades.EmpresaSerie BuscarPorSerie(int codigoEmpresa, int serie, Dominio.Enumeradores.TipoSerie tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Numero == serie && obj.Tipo == tipo select obj;
            return result.FirstOrDefault();
        }

        public System.Threading.Tasks.Task<Dominio.Entidades.EmpresaSerie> BuscarPorSerieAsync(int codigoEmpresa, int serie, Dominio.Enumeradores.TipoSerie tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Numero == serie && obj.Tipo == tipo select obj;
            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.EmpresaSerie BuscarPorEmpresaTipo(int codigoEmpresa, Dominio.Enumeradores.TipoSerie tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Tipo == tipo && obj.Status == "A" select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.EmpresaSerie BuscarPorEmpresaNumeroTipo(int codigoEmpresa, int numero, Dominio.Enumeradores.TipoSerie tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Numero == numero && obj.Tipo == tipo && obj.Status == "A" select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.EmpresaSerie> BuscarSeriesPorEmpresaTipo(int codigoEmpresa, Dominio.Enumeradores.TipoSerie tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Tipo == tipo && obj.Status == "A" select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.EmpresaSerie> BuscarTodos(int codigoEmpresa, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();
            var result = (from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == "A" select obj).OrderBy(o => o.Numero).Take(maximoRegistros).Skip(inicioRegistros);
            return result.ToList();
        }

        public int ContarTodos(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == "A" select obj.Codigo;
            return result.Count();
        }

        public Dominio.Entidades.EmpresaSerie BuscarUltimoRegistroNFSe(int codigoEmpresa, int[] series = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == "A" && obj.Tipo == Dominio.Enumeradores.TipoSerie.NFSe select obj;
            if (series != null && series.Count() > 0)
                result = result.Where(o => series.Contains(o.Codigo));
            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public Dominio.Entidades.EmpresaSerie BuscarUltimoRegistro(int codigoEmpresa, int[] series = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == "A" select obj;
            if (series != null && series.Count() > 0)
                result = result.Where(o => series.Contains(o.Codigo));
            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public List<Dominio.Entidades.EmpresaSerie> Consultar(int codigoEmpresa, int serie, string status, int inicioRegistros, int maximoRegistros, string propriedadeOrdenacao = "Codigo", string direcaoOrdenacao = "desc")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (serie > 0)
                result = result.Where(o => o.Numero == serie);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.OrderBy(propriedadeOrdenacao + (direcaoOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int serie, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (serie > 0)
                result = result.Where(o => o.Numero == serie);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.Count();
        }

        public List<Dominio.Entidades.EmpresaSerie> ConsultarPorTipo(int codigoEmpresa, int serie, string status, Dominio.Enumeradores.TipoSerie tipo, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();

            var result = from obj in query where obj.Tipo == tipo select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (serie > 0)
                result = result.Where(o => o.Numero == serie);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.OrderBy(o => o.Numero).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaPorTipo(int codigoEmpresa, int serie, string status, Dominio.Enumeradores.TipoSerie tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();

            var result = from obj in query where obj.Tipo == tipo select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (serie > 0)
                result = result.Where(o => o.Numero == serie);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.Count();
        }

        public List<Dominio.Entidades.EmpresaSerie> BuscarTodosAtivosInativos(int codigoEmpresa, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();
            var result = (from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj).OrderBy(o => o.Numero).Take(maximoRegistros).Skip(inicioRegistros);
            return result.ToList();
        }

        public int ContarTodosAtivosInativos(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj.Codigo;
            return result.Count();
        }

        #region Relatorios
        public IList<Dominio.Relatorios.Embarcador.DataSource.Documentos.SerieDocumentos> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaSerieDocumentos filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = new ConsultaSerieDocumentos().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Documentos.SerieDocumentos)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Documentos.SerieDocumentos>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaSerieDocumentos filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consulta = new ConsultaSerieDocumentos().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }
        #endregion
    }
}
