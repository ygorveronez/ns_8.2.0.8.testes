using System.Linq;

namespace Repositorio.Embarcador.Pallets.Reforma
{
    public class ReformaPalletNfeRetorno : RepositorioBase<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeRetorno>
    {
        #region Construtores

        public ReformaPalletNfeRetorno(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeRetorno BuscarPorCodigo(int codigo)
        {
            var reformaPalletNfeRetorno = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeRetorno>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return reformaPalletNfeRetorno;
        }

        public Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeRetorno BuscarPorReformaChaveNfe(int codigoReforma, string chave)
        {
            var reformaPalletNfeRetorno = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeRetorno>()
                .Where(o => (o.ReformaPallet.Codigo == codigoReforma) && (o.XmlNotaFiscal.Chave == chave))
                .FirstOrDefault();

            return reformaPalletNfeRetorno;
        }

        public Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeRetorno BuscarPorReformaNumeroSerie(int codigoReforma, int numero, int numeroSerie)
        {
            var reformaPalletNfeRetorno = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeRetorno>()
                .Where(o => (o.ReformaPallet.Codigo == codigoReforma) && (o.XmlNotaFiscal.Numero == numero) && (o.XmlNotaFiscal.Serie == numeroSerie.ToString()))
                .FirstOrDefault();

            return reformaPalletNfeRetorno;
        }

        #endregion
    }
}
