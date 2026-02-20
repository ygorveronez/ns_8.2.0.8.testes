using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frota
{
    public class SinistroServico : RepositorioBase<Dominio.Entidades.Embarcador.Frota.SinistroServico>
    {
        public SinistroServico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frota.SinistroServico> BuscarPorSinistro(int codigoSinistro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.SinistroServico>();

            query = query.Where(o => o.Sinistro.Codigo == codigoSinistro);

            return query.ToList();
        }
    }
}
