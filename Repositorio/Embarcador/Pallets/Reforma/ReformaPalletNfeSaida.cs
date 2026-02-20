using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pallets.Reforma
{
    public class ReformaPalletNfeSaida : RepositorioBase<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeSaida>
    {
        #region Construtores

        public ReformaPalletNfeSaida(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeSaida BuscarPorCodigo(int codigo)
        {
            var reformaPalletNfeSaida = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeSaida>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return reformaPalletNfeSaida;
        }

        public List<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeSaida> BuscarPorReforma(int codigo)
        {
            var listaReformaPalletNfeSaida = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeSaida>()
                .Where(o => o.ReformaPallet.Codigo == codigo)
                .ToList();

            return listaReformaPalletNfeSaida;
        }

        public Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeSaida BuscarPorReformaChaveNfe(int codigoReforma, string chave)
        {
            var reformaPalletNfeSaida = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeSaida>()
                .Where(o => (o.ReformaPallet.Codigo == codigoReforma) && (o.XmlNotaFiscal.Chave == chave))
                .FirstOrDefault();

            return reformaPalletNfeSaida;
        }

        public Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeSaida BuscarPorReformaNumeroSerie(int codigoReforma, int numero, int numeroSerie)
        {
            var reformaPalletNfeSaida = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeSaida>()
                .Where(o => (o.ReformaPallet.Codigo == codigoReforma) && (o.XmlNotaFiscal.Numero == numero) && (o.XmlNotaFiscal.Serie == numeroSerie.ToString()))
                .FirstOrDefault();

            return reformaPalletNfeSaida;
        }

        #endregion
    }
}
