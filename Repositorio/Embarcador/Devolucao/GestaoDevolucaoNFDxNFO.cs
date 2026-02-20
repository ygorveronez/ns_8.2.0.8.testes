using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Repositorio.Embarcador.Devolucao
{
    public class GestaoDevolucaoNFDxNFO : RepositorioBase<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO>
    {
        public GestaoDevolucaoNFDxNFO(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO> BuscarPorNFD(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO>();
            query = query.Where(o => codigos.Contains(o.NFD.Codigo));
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO> BuscarPorNFO(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO>();
            query = query.Where(o => codigos.Contains(o.NFO.Codigo));
            return query.ToList();
        }
    }
}
