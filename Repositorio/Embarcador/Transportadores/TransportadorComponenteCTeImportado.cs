using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Transportadores
{
    public class TransportadorComponenteCTeImportado : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado>
    {
        #region Construtores

        public TransportadorComponenteCTeImportado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado> BuscarPorTransportador(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado>()
                .Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return query
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado> BuscarPorTransportadorComFetchComponenteFrete(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado>()
                .Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return query
                .Fetch(obj => obj.ComponenteFrete)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado> BuscarComponentesPorOutraDescricaoDePara()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado>();

            return query
                .Fetch(obj => obj.ComponenteFrete)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> BuscarComponentesDePara()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado>();

            return query
                .Select(obj => obj.ComponenteFrete)
                .Distinct()
                .ToList();
        }

        #endregion
    }
}
