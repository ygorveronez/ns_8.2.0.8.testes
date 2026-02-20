using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Filiais
{
    public sealed class ValePedagioTransportadorFilial : RepositorioBase<Dominio.Entidades.Embarcador.Filiais.ValePedagioTransportadorFilial>
    {
        #region Construtores

        public ValePedagioTransportadorFilial(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Filiais.ValePedagioTransportadorFilial> BuscarPorFilial(int codigoFilial)
        {
            var consultaValePedagioTransportadorFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.ValePedagioTransportadorFilial>()
                .Where(o => o.Filial.Codigo == codigoFilial);

            return consultaValePedagioTransportadorFilial.ToList();
        }

        public Dominio.Entidades.Embarcador.Filiais.ValePedagioTransportadorFilial BuscarPorFilialETransportador(int codigoFilial, int codigoTransportador)
        {
            var consultaValePedagioTransportadorFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.ValePedagioTransportadorFilial>()
                .Where(o => o.Filial.Codigo == codigoFilial && o.Transportador.Codigo == codigoTransportador);

            return consultaValePedagioTransportadorFilial.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Filiais.ValePedagioTransportadorFilial> BuscarPorFilialETransportadorAsync(int codigoFilial, int codigoTransportador)
        {
            var consultaValePedagioTransportadorFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.ValePedagioTransportadorFilial>()
                .Where(o => o.Filial.Codigo == codigoFilial && o.Transportador.Codigo == codigoTransportador);

            return await consultaValePedagioTransportadorFilial.FirstOrDefaultAsync();
        }

        #endregion
    }
}
