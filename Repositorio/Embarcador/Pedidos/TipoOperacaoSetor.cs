using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoOperacaoControleEntregaSetor : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoControleEntregaSetor>
    {
        #region Construtores

        public TipoOperacaoControleEntregaSetor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoControleEntregaSetor> BuscarPorConfiguracaoTipoOperacaoControleEntrega(int codigoConfigTipoOperacaoControleEntrega)
        {
            var consultaOperacaoControleEntrega = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoControleEntregaSetor>()
                .Where(o => o.ConfiguracaoTipoOperacaoControleEntrega.Codigo == codigoConfigTipoOperacaoControleEntrega);

            return consultaOperacaoControleEntrega
                .Fetch(o => o.Setor)
                .ToList();
        }

        public void DeletarPorConfiguracaoTipoOperacaoControleEntrega(int codigoConfiguracaoTipoOperacaoControleEntrega)
        {
            UnitOfWork.Sessao
                .CreateQuery($"delete TipoOperacaoControleEntregaSetor where ConfiguracaoTipoOperacaoControleEntrega.Codigo = :codigoConfiguracaoTipoOperacaoControleEntrega")
                .SetInt32("codigoConfiguracaoTipoOperacaoControleEntrega", codigoConfiguracaoTipoOperacaoControleEntrega)
                .ExecuteUpdate();
        }

        #endregion Métodos Públicos
    }
}