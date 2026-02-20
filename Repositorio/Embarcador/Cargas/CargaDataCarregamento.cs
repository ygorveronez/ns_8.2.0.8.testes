using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaDataCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaDataCarregamento>
    {
        #region Construtores

        public CargaDataCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public int BuscarProximoNumero()
        {
            var consultaCargaDataCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDataCarregamento>();
            int? ultimoNumero = consultaCargaDataCarregamento.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDataCarregamento BuscarDataCarregamentoCargaFilial(string numeroCarga, Dominio.Entidades.Embarcador.Filiais.Filial filial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDataCarregamento>();
            query = query.Where(o => o.CodigoCargaEmbarcador == numeroCarga && (o.CodigoFilialEmbarcador == filial.CodigoFilialEmbarcador || filial.OutrosCodigosIntegracao.Contains(o.CodigoFilialEmbarcador)));
            return query.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        #endregion
    }
}
