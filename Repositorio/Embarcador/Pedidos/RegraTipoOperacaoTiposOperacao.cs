using Dominio.ObjetosDeValor.Embarcador.Pedido;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class RegraTipoOperacaoTiposOperacao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoTiposOperacao>
    {
        public RegraTipoOperacaoTiposOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoTiposOperacao> BuscarPorRegraTipoOperacao(int codigoRegraTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoTiposOperacao>();

            query = query.Where(o => o.RegraTipoOperacao.Codigo == codigoRegraTipoOperacao);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoTiposOperacao BuscarPorRegraTipoOperacaoETipoOperacao(int codigoRegraTipoOperacao, int codigoTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoTiposOperacao>();

            query = query.Where(o => o.RegraTipoOperacao.Codigo == codigoRegraTipoOperacao && o.TipoOperacao.Codigo == codigoTipoOperacao);

            return query.FirstOrDefault();
        }

        public List<RetornoRegraTipoOperacao> BuscarCodigosTiposOperacaoPorRegra(List<int> codigoTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoTiposOperacao>();
            query = query.Where(o => codigoTipoOperacao.Contains(o.RegraTipoOperacao.Codigo));
            return query.Select(r => new RetornoRegraTipoOperacao()
            {
                CodigoRegra = r.RegraTipoOperacao.Codigo,
                CodigoCampoS = r.TipoOperacao.CodigoIntegracao
            }).ToList();
        }

        #endregion
    }
}