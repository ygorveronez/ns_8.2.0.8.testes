using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class NaturezaCargaANTT : RepositorioBase<Dominio.Entidades.NaturezaCargaANTT>, Dominio.Interfaces.Repositorios.NaturezaCargaANTT
    {
        public NaturezaCargaANTT(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.NaturezaCargaANTT BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaCargaANTT>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.NaturezaCargaANTT> BuscarTodos(bool buscarSemOpcaoZero = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaCargaANTT>();

            if (buscarSemOpcaoZero)
                query = query.Where(x => x.CodigoNatureza != "0");

            var result = from obj in query orderby obj.CodigoNatureza ascending select obj;

            return result.ToList();
        }

        public Dominio.Entidades.NaturezaCargaANTT BuscarPorNatureza(string codigoNatureza)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaCargaANTT>();

            var result = from obj in query where obj.CodigoNatureza == codigoNatureza select obj;

            return result.FirstOrDefault();
        }
    }
}
