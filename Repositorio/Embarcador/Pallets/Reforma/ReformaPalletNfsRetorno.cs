using System.Linq;

namespace Repositorio.Embarcador.Pallets.Reforma
{
    public class ReformaPalletNfsRetorno : RepositorioBase<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfsRetorno>
    {
        #region Construtores

        public ReformaPalletNfsRetorno(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfsRetorno BuscarPorCodigo(int codigo)
        {
            var reformaPalletNfsRetorno = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfsRetorno>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return reformaPalletNfsRetorno;
        }

        public Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfsRetorno BuscarPorReformaNumeroSerie(int codigoReforma, int numero, int numeroSerie)
        {
            var reformaPalletNfsRetorno = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfsRetorno>()
                .Where(o => (o.ReformaPallet.Codigo == codigoReforma) && (o.Numero == numero) && (o.Serie == numeroSerie))
                .FirstOrDefault();

            return reformaPalletNfsRetorno;
        }

        #endregion
    }
}
