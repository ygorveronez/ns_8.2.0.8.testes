using LinqKit;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class PercursoEstado : RepositorioBase<Dominio.Entidades.PercursoEstado>, Dominio.Interfaces.Repositorios.PercursoEstado
    {
        public PercursoEstado(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }
        public PercursoEstado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.PercursoEstado BuscarPorCodigo(int codigoPercurso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PercursoEstado>();

            var result = from obj in query where obj.Codigo == codigoPercurso select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.PercursoEstado Buscar(int codigoEmpresa, int codigoPercurso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PercursoEstado>();

            var result = from obj in query where obj.Codigo == codigoPercurso && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.PercursoEstado Buscar(int codigoEmpresa, string ufOrigem, string ufDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PercursoEstado>();

            var result = from obj in query where obj.EstadoOrigem.Sigla.Equals(ufOrigem) && obj.EstadoDestino.Sigla.Equals(ufDestino) && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.PercursoEstado> BuscarAsync(int codigoEmpresa, string ufOrigem, string ufDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PercursoEstado>();

            var result = from obj in query where obj.EstadoOrigem.Sigla.Equals(ufOrigem) && obj.EstadoDestino.Sigla.Equals(ufDestino) && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.PercursoEstado Buscar(int codigoEmpresa, string ufOrigem, List<string> ufDestinos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PercursoEstado>();

            var result = from obj in query where obj.EstadoOrigem.Sigla.Equals(ufOrigem) && obj.Empresa.Codigo == codigoEmpresa select obj;

            result = result.Where(EstadosDestinoExatos(ufDestinos));

            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.PercursoEstado> Consultar(int codigoEmpresa, string ufOrigem, string ufDestino, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PercursoEstado>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(ufOrigem))
                result = result.Where(o => o.EstadoOrigem.Nome.Contains(ufOrigem) || o.EstadoOrigem.Sigla.Contains(ufOrigem));

            if (!string.IsNullOrWhiteSpace(ufDestino))
                result = result.Where(o => o.EstadoDestino.Nome.Contains(ufDestino) || o.EstadoDestino.Sigla.Contains(ufDestino));

            return result.Fetch(o => o.EstadoDestino)
                         .Fetch(o => o.EstadoOrigem)
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros)
                         .ToList();

        }

        public List<Dominio.Entidades.PercursoEstado> Consultar(int codigoEmpresa, string ufOrigem, string ufDestino, bool pesquisarPorListaDestinos, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PercursoEstado>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(ufOrigem))
            {
                if (pesquisarPorListaDestinos)
                    result = result.Where(o => o.EstadoOrigem.Sigla.Equals(ufOrigem));
                else
                    result = result.Where(o => o.EstadoOrigem.Nome.Contains(ufOrigem) || o.EstadoOrigem.Sigla.Contains(ufOrigem));
            }


            if (!string.IsNullOrWhiteSpace(ufDestino))
            {
                if (pesquisarPorListaDestinos)
                    result = result.Where(o => o.EstadosDestino.Any(est => est.Sigla == ufDestino));
                else
                    result = result.Where(o => o.EstadoDestino.Nome.Contains(ufDestino) || o.EstadoDestino.Sigla.Contains(ufDestino));
            }


            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Fetch(obj => obj.Empresa).ThenFetch(obj => obj.Localidade).Fetch(o => o.EstadoDestino).Fetch(o => o.EstadoOrigem).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(int codigoEmpresa, string ufOrigem, string ufDestino, bool pesquisarPorListaDestinos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PercursoEstado>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(ufOrigem))
            {
                if (pesquisarPorListaDestinos)
                    result = result.Where(o => o.EstadoOrigem.Sigla.Equals(ufOrigem));
                else
                    result = result.Where(o => o.EstadoOrigem.Nome.Contains(ufOrigem) || o.EstadoOrigem.Sigla.Contains(ufOrigem));
            }

            if (!string.IsNullOrWhiteSpace(ufDestino))
            {
                if (pesquisarPorListaDestinos)
                    result = result.Where(o => o.EstadosDestino.Any(est => est.Sigla == ufDestino));
                else
                    result = result.Where(o => o.EstadoDestino.Nome.Contains(ufDestino) || o.EstadoDestino.Sigla.Contains(ufDestino));
            }

            return result.Count();
        }

        public Dominio.Entidades.PercursoEstado BuscarPorOrigemEDestino(int codigoEmpresa, string ufOrigem, string ufDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PercursoEstado>();

            var result = from obj in query where obj.EstadoOrigem.Sigla.Equals(ufOrigem) && obj.EstadoDestino.Sigla.Equals(ufDestino) select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.PercursoEstado BuscarPorOrigemEDestino(int codigoEmpresa, string ufOrigem, List<string> ufDestinos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PercursoEstado>();

            var result = from obj in query where obj.EstadoOrigem.Sigla.Equals(ufOrigem) select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            result = result.Where(EstadosDestinoExatos(ufDestinos));

            return result.FirstOrDefault();
        }


        private Expression<Func<Dominio.Entidades.PercursoEstado, bool>> EstadosDestinoExatos(List<string> estadosDestino)
        {
            var estadosExatos = PredicateBuilder.True<Dominio.Entidades.PercursoEstado>();

            estadosExatos = estadosExatos.And(o => o.EstadosDestino.Count == estadosDestino.Count);

            var estadosExatosEstados = PredicateBuilder.True<Dominio.Entidades.PercursoEstado>();

            foreach (string estadoDestino in estadosDestino)
                estadosExatosEstados = estadosExatosEstados.And(o => o.EstadosDestino.Any(l => l.Sigla == estadoDestino));

            estadosExatos = estadosExatos.And(estadosExatosEstados);

            return estadosExatos;
        }

    }
}
