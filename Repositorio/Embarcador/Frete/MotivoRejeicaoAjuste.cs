using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class MotivoRejeicaoAjuste : RepositorioBase<Dominio.Entidades.Embarcador.Frete.MotivoRejeicaoAjuste>
    {

        public MotivoRejeicaoAjuste(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.MotivoRejeicaoAjuste BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.MotivoRejeicaoAjuste>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frete.MotivoRejeicaoAjuste> _Consultar()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.MotivoRejeicaoAjuste>();

            var result = from obj in query select obj;

            // Filtros

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Frete.MotivoRejeicaoAjuste> Consultar(string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar();

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta()
        {
            var result = _Consultar();

            return result.Count();
        }
    }
}
