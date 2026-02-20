using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaAnexo>
    {
        public CargaAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaAnexo> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaAnexo>()
                .Where(o => o.EntidadeAnexo.Codigo == codigoCarga);

            return query.ToList();
        }

        public int ContarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaAnexo>()
                .Where(o => o.EntidadeAnexo.Codigo == codigoCarga);

            return query.Count();
        }
    }
}
