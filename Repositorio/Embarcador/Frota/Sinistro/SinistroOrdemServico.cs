using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frota
{
    public class SinistroOrdemServico : RepositorioBase<Dominio.Entidades.Embarcador.Frota.SinistroOrdemServico>
    {
        public SinistroOrdemServico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frota.SinistroOrdemServico> BuscarPorFluxoSinistro(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.SinistroOrdemServico>()
                .Where(obj => obj.Sinistro.Codigo == codigo);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Frota.SinistroOrdemServico BuscarPorFluxoSinistroEOrdemServico(int codigo, int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.SinistroOrdemServico>()
                .Where(obj => obj.Sinistro.Codigo == codigo && obj.OrdemServico.Codigo == codigoOrdemServico);

            return query.FirstOrDefault();
        }
    }
}
