using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Operacional
{
    public class OperadorFilial : RepositorioBase<Dominio.Entidades.Embarcador.Operacional.OperadorFilial>
    {
         public OperadorFilial(UnitOfWork unitOfWork) : base(unitOfWork) { }

         public Dominio.Entidades.Embarcador.Operacional.OperadorFilial BuscarPorCodigo(int codigo)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.OperadorFilial>();
             var result = from obj in query where obj.Codigo == codigo select obj;
             return result.FirstOrDefault();
         }

         public List<Dominio.Entidades.Embarcador.Operacional.OperadorFilial> BuscarPorOperador(int codigoOperador)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.OperadorFilial>();
             var result = from obj in query where obj.OperadorLogistica.Codigo == codigoOperador select obj;
             return result.ToList();
         }

        public Dominio.Entidades.Embarcador.Operacional.OperadorFilial BuscarPorOperadorEFilial(int codigo, int codigoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.OperadorFilial>()
                .Where(obj => obj.OperadorLogistica.Codigo == codigo && obj.Filial.Codigo == codigoFilial);

            return query.FirstOrDefault();
        }

        public List<int> BuscarCodigosFiliaisPorOperador(int codigoOperador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.OperadorFilial>();

            var result = from obj in query where obj.OperadorLogistica.Codigo == codigoOperador select obj;

            return result.Select(o => o.Filial.Codigo).ToList();
        }
    }
}