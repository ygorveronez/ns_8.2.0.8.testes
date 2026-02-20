using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frota
{
    public class SinistroHistorico : RepositorioBase<Dominio.Entidades.Embarcador.Frota.SinistroHistorico>
    {
        public SinistroHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frota.SinistroHistorico> BuscarPorFluxoSinistro(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.SinistroHistorico>()
                .Where(obj => obj.Sinistro.Codigo == codigo);
            
            return query
                .ToList();
        }
    }
}
