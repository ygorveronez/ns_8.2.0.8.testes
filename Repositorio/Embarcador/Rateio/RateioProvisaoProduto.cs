using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Rateio
{
    public class RateioProvisaoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Rateio.RateioProvisaoProduto>
    {
        public RateioProvisaoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public bool ExistePorProvisao(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Rateio.RateioProvisaoProduto>();
            var result = query.Any(obj => obj.Provisao == provisao);
            return result;
        }

        public List<Dominio.Entidades.Embarcador.Rateio.RateioProvisaoProduto> BuscarPorProvisao(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Rateio.RateioProvisaoProduto>();
            var result = query.Where(obj => obj.Provisao == provisao);
            return result.ToList();
        }
    }
}
