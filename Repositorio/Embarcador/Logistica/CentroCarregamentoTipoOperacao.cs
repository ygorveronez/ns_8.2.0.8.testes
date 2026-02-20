using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class CentroCarregamentoTipoOperacao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao>
    {
        #region Construtores

        public CentroCarregamentoTipoOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao> BuscarPorCentro(int codigoCentroCarregamento)
        {
            var consultaCentroCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao>()
                .Where(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento);

            return consultaCentroCarregamentoTransportador
                .Fetch(x => x.TipoOperacao)
                .ThenFetch(x => x.TipoDeCargaPadraoOperacao)
                .Fetch(x => x.CentroCarregamento)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao BuscarPorFilialTipoOperacao(int codigoFilial, int codigoTipoOperacao)
        {
            var consultaCentroCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao>()
                .Where(o => o.CentroCarregamento.Filial.Codigo == codigoFilial && o.TipoOperacao.Codigo == codigoTipoOperacao);

            return consultaCentroCarregamentoTransportador
                .Fetch(x => x.TipoOperacao)
                .ThenFetch(x => x.TipoDeCargaPadraoOperacao)
                .FirstOrDefault();
        }

        #endregion
    }
}