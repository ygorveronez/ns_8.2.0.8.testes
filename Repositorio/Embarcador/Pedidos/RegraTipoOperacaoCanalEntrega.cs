using Dominio.ObjetosDeValor.Embarcador.Pedido;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class RegraTipoOperacaoCanalEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega>
    {
        public RegraTipoOperacaoCanalEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega> BuscarPorRegraTipoOperacao(int codigoRegraTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega>();

            query = query.Where(o => o.RegraTipoOperacao.Codigo == codigoRegraTipoOperacao);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega BuscarPorRegraTipoOperacaoECanalEntrega(int codigoRecebedor, int codigoCanaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega>();

            query = query.Where(o => o.RegraTipoOperacao.Codigo == codigoRecebedor && o.CanalEntrega.Codigo == codigoCanaEntrega);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega BuscarPorCanalEntrega(int codigoCanalEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega>();

            query = query.Where(o => o.CanalEntrega.Codigo == codigoCanalEntrega);

            return query.FirstOrDefault();
        }

        public List<RetornoRegraTipoOperacao> BuscarCodigoCanalEntregaPorRegra(List<int> codigoTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega>();
            query = query.Where(o => codigoTipoOperacao.Contains(o.RegraTipoOperacao.Codigo) );
            return query.Select(r =>new RetornoRegraTipoOperacao()
            {
                CodigoRegra = r.RegraTipoOperacao.Codigo,
                CodigoCampoS = r.CanalEntrega.CodigoIntegracao
            }).ToList();
        }
        #endregion
    }
}