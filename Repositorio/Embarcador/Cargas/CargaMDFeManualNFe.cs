using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaMDFeManualNFe : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualNFe>
    {
        public CargaMDFeManualNFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualNFe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualNFe>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualNFe> BuscarPorCargaMDFeManual(int codigoCargaMDFeManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualNFe>();

            query = query.Where(o => o.CargaMDFeManual.Codigo == codigoCargaMDFeManual);

            return query.ToList();
        }

        public bool ContemPorCargaMDFeManual(int codigoCargaMDFeManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualNFe>();

            query = query.Where(o => o.CargaMDFeManual.Codigo == codigoCargaMDFeManual);

            return query.Any();
        }
    }
}
