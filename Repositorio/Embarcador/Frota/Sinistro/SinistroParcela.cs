using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frota
{
    public class SinistroParcela : RepositorioBase<Dominio.Entidades.Embarcador.Frota.SinistroParcela>
    {
        public SinistroParcela(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frota.SinistroParcela> BuscarPorFluxoSinistro(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.SinistroParcela>()
                .Where(obj => obj.Sinistro.Codigo == codigo);

            return query.ToList();
        }
    }
}
