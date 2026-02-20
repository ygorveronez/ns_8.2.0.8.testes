using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoOperacaoFilialMotoristaGenerico : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFilialMotoristaGenerico>
    {
        #region Construtores

        public TipoOperacaoFilialMotoristaGenerico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFilialMotoristaGenerico> BuscarFiliaisMotoristasPorTipoOperacao(int codigoTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFilialMotoristaGenerico>();

            query = query.Where(tipoOperacaoFilialMotoristaGenerico => tipoOperacaoFilialMotoristaGenerico.TipoOperacao.Codigo == codigoTipoOperacao);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFilialMotoristaGenerico BuscarPorTipoOperacaoEFilial(int codigoTipoOperacao, int codigoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFilialMotoristaGenerico>();

            query = query.Where(tipoOperacaoFilialMotoristaGenerico => tipoOperacaoFilialMotoristaGenerico.TipoOperacao.Codigo == codigoTipoOperacao &&
                                tipoOperacaoFilialMotoristaGenerico.Filial.Codigo == codigoFilial);

            return query.FirstOrDefault();
        }
        #endregion
    }
}
