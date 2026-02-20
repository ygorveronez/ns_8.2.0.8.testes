using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Transportadores
{
    public class TransportadorCodigoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.TransportadorCodigoIntegracao>
    {
        #region Construtores

        public TransportadorCodigoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Transportadores.TransportadorCodigoIntegracao> BuscarPorTransportador(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorCodigoIntegracao>()
                .Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return query
                .ToList();
        }

        #endregion
    }
}
