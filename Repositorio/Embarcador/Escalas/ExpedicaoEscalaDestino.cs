using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Escalas
{
    public class ExpedicaoEscalaDestino : RepositorioBase<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscalaDestino>
    {
        public ExpedicaoEscalaDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscalaDestino BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscalaDestino>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscalaDestino> BuscarPorEscalaExpedicao(int codigoEscalaExpedicao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscalaDestino>();
            var result = from obj in query where obj.ExpedicaoEscala.Codigo == codigoEscalaExpedicao select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscalaDestino> BuscarPorGeracaoEscala(int codigoGeracaoEscala)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscalaDestino>();
            var result = from obj in query where obj.ExpedicaoEscala.GeracaoEscala.Codigo == codigoGeracaoEscala select obj;
            return result.ToList();
        }

    }
}
