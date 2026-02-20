using System.Linq;

namespace Repositorio.Embarcador.Documentos
{
    public class CIOTDadosPamcard : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.CIOTDadosPamcard>
    {
        public CIOTDadosPamcard(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Documentos.CIOTDadosPamcard BuscarPorCIOT (Dominio.Entidades.Embarcador.Documentos.CIOT ciot)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.CIOTDadosPamcard> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOTDadosPamcard>();

            query = query.Where(o => o.CIOT == ciot);

            return query.FirstOrDefault();
        }
    }
}
