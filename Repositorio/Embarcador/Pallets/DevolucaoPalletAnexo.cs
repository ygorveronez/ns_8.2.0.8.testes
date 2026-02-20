using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pallets
{
    public class DevolucaoPalletAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Pallets.DevolucaoPalletAnexo>
    {
        public DevolucaoPalletAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pallets.DevolucaoPalletAnexo> BuscarPorDevolucao(int codigoDevolucao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPalletAnexo>()
                .Where(o => o.EntidadeAnexo.Codigo == codigoDevolucao);
            
            return query
                .ToList();
        }
    }
}
