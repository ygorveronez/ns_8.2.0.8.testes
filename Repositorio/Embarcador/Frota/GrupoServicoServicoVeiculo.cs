using System.Linq;

namespace Repositorio.Embarcador.Frota
{
    public class GrupoServicoServicoVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Frota.GrupoServicoServicoVeiculo>
    {
        public GrupoServicoServicoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.GrupoServicoServicoVeiculo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.GrupoServicoServicoVeiculo>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        #endregion
    }
}
