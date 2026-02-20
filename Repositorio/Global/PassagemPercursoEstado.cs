using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NHibernate.Linq;

namespace Repositorio
{
    public class PassagemPercursoEstado : RepositorioBase<Dominio.Entidades.PassagemPercursoEstado>, Dominio.Interfaces.Repositorios.PassagemPercursoEstado
    {
        public PassagemPercursoEstado(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public PassagemPercursoEstado(UnitOfWork unitOfWork, CancellationToken cancellation) : base(unitOfWork, cancellation) { }

        public Dominio.Entidades.PassagemPercursoEstado Buscar(int codigoPercurso, int codigoPassagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PassagemPercursoEstado>();

            var result = from obj in query where obj.Codigo == codigoPassagem && obj.Percurso.Codigo == codigoPercurso select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.PassagemPercursoEstado> Buscar(int codigoPercurso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PassagemPercursoEstado>();
            
            var result = from obj in query where obj.Percurso.Codigo == codigoPercurso orderby obj.Ordem ascending select obj;

            return result.Fetch(obj => obj.EstadoDePassagem).ToList();
        }

        public Dominio.Entidades.PassagemPercursoEstado BuscarPorEstado(int codigoPercurso, string siglaEstado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PassagemPercursoEstado>();

            var result = from obj in query where obj.EstadoDePassagem.Sigla == siglaEstado && obj.Percurso.Codigo == codigoPercurso select obj;

            return result.FirstOrDefault();
        }

    }
}
