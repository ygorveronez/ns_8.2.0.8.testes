using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class DuplicataCtes : RepositorioBase<Dominio.Entidades.DuplicataCtes>, Dominio.Interfaces.Repositorios.DuplicataCtes
    {
        public DuplicataCtes(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.DuplicataCtes BuscarPorCodigo(int codigo, int codigoDuplicata)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

            var result = from obj in query where obj.ConhecimentoDeTransporteEletronico.Codigo == codigo && obj.Duplicata.Codigo == codigoDuplicata select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.DuplicataCtes BuscarPorCodigoCTe(int cte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

            var result = from obj in query where obj.ConhecimentoDeTransporteEletronico.Codigo == cte && obj.Duplicata.Status == "A" select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.DuplicataCtes> BuscarPorDuplicata(int codigoDuplicata)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

            var result = from obj in query where obj.Duplicata.Codigo == codigoDuplicata orderby obj.ConhecimentoDeTransporteEletronico.Numero select obj;

            return result.ToList();
        }
    }
}
