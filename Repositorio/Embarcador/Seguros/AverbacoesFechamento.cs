using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Seguros
{
    public class AverbacoesFechamento : RepositorioBase<Dominio.Entidades.Embarcador.Seguros.AverbacoesFechamento>
    {
        public AverbacoesFechamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Seguros.AverbacoesFechamento> Consultar(int fechamento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.AverbacoesFechamento>();

            var result = from obj in query where obj.FechamentoAverbacao.Codigo == fechamento select obj;

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int fechamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.AverbacoesFechamento>();

            var result = from obj in query where obj.FechamentoAverbacao.Codigo == fechamento select obj;

            return result.Count();
        }
    }
}
