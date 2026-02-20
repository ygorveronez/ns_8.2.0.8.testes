using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaConfirmacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaConfirmacao>
    {
        #region Construtores

        public CargaConfirmacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaConfirmacao BuscarPorCarga(int codigoCarga)
        {
            var consultaCargaConfirmacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaConfirmacao>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaCargaConfirmacao.FirstOrDefault();
        }

        #endregion
    }
}
