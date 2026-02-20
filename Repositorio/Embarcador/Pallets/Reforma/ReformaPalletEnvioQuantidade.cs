using System.Linq;

namespace Repositorio.Embarcador.Pallets.Reforma
{
    public class ReformaPalletEnvioQuantidade : RepositorioBase<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletEnvioQuantidade>
    {
        #region Construtores

        public ReformaPalletEnvioQuantidade(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletEnvioQuantidade BuscarPorCodigo(int codigo)
        {
            var reformaPalletQuantidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletEnvioQuantidade>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return reformaPalletQuantidade;
        }

        #endregion
    }
}
