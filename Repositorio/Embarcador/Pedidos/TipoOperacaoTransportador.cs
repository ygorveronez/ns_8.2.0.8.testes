using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoOperacaoTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador>
    {
        #region Construtores

        public TipoOperacaoTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador> BuscarPorTipoOperacao(int codigoTipoOperacao)
        {
            var consultaTipoOperacaoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador>()
                .Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao);

            return consultaTipoOperacaoTransportador
                .Fetch(o => o.Transportador)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador BuscarPorTipoOperacaoETransportador(int codigoTipoOperacao, int codigoTransportador)
        {
            var consultaTipoOperacaoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador>()
                .Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao && o.Transportador.Codigo == codigoTransportador);

            return consultaTipoOperacaoTransportador
                .FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador> BuscarPorTipoOperacaoETransportadorAsync(int codigoTipoOperacao, int codigoTransportador)
        {
            var consultaTipoOperacaoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador>()
                .Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao && o.Transportador.Codigo == codigoTransportador);

            return await consultaTipoOperacaoTransportador
                .FirstOrDefaultAsync();
        }

        #endregion
    }
}
