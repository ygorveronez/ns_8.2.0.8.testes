using Dominio.ObjetosDeValor.Embarcador.Pedido;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class RegraTipoOperacaoCanalVenda : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda>
    {
        public RegraTipoOperacaoCanalVenda(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda> BuscarPorRegraTipoOperacao(int codigoRegraTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda>();

            query = query.Where(o => o.RegraTipoOperacao.Codigo == codigoRegraTipoOperacao);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda BuscarPorRegraTipoOperacaoECanalVenda(int codigoRecebedor, int codigoTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda>();

            query = query.Where(o => o.RegraTipoOperacao.Codigo == codigoRecebedor && o.CanalVenda.Codigo == codigoTipoCarga);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda BuscarPorCanalVenda(int codigoCanalVenda)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda>();

            query = query.Where(o => o.CanalVenda.Codigo == codigoCanalVenda);

            return query.FirstOrDefault();
        }


        public List<RetornoRegraTipoOperacao> BuscarCodigosCanalVendaPorRegra(List<int> codigoTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda>();
            query = query.Where(o => codigoTipoOperacao.Contains(o.RegraTipoOperacao.Codigo));
            return query.Select(r => new RetornoRegraTipoOperacao()
            {
                CodigoRegra = r.RegraTipoOperacao.Codigo,
                CodigoCampoS = r.CanalVenda.CodigoIntegracao
            }).ToList();
        }
        #endregion
    }
}

