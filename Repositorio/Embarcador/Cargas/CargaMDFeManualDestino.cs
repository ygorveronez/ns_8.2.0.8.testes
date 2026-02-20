using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaMDFeManualDestino : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino>
    {
        public CargaMDFeManualDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino> BuscarPorCargaMDFeManual(int codigoCargaMDFeManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino>();

            query = query.Where(o => o.CargaMDFeManual.Codigo == codigoCargaMDFeManual);

            return query.OrderBy(obj => obj.Ordem).ToList();
        }
        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino BuscarPorLocalidadeECargaMDFeManual(int codigoCargaMDFeManual, int localidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino>();

            query = query.Where(o => o.CargaMDFeManual.Codigo == codigoCargaMDFeManual && o.Localidade.Codigo == localidade);

            return query.FirstOrDefault();
        }
    }
}
