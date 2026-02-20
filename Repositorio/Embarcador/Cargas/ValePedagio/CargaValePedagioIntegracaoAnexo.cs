
using Repositorio.Embarcador.Anexo;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ValePedagio
{
    public class CargaValePedagioIntegracaoAnexo : Anexo<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoAnexo, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>
    {
        #region Construtores
        public CargaValePedagioIntegracaoAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos
        public Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoAnexo BuscarPorCargaIntegracao(int codigoCargaIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoAnexo>();
            query = query.Where(c => c.EntidadeAnexo.Codigo == codigoCargaIntegracao);
            return query.FirstOrDefault();
        }
        #endregion

    }
}
