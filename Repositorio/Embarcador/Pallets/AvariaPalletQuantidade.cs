using System.Linq;

namespace Repositorio.Embarcador.Pallets
{
    public class AvariaPalletQuantidade : RepositorioBase<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPalletQuantidade>
    {
        #region Construtores

        public AvariaPalletQuantidade(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPalletQuantidade BuscarPorCodigo(int codigo)
        {
            var avariaPalletQuantidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPalletQuantidade>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return avariaPalletQuantidade;
        }

        #endregion
    }
}
