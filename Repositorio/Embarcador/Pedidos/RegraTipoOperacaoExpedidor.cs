using Dominio.ObjetosDeValor.Embarcador.Pedido;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class RegraTipoOperacaoExpedidor : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoExpedidor>
    {
        public RegraTipoOperacaoExpedidor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoExpedidor> BuscarPorRegraTipoOperacao(int codigoRegraTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoExpedidor>();

            query = query.Where(o => o.RegraTipoOperacao.Codigo == codigoRegraTipoOperacao);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoExpedidor BuscarPorRegraTipoOperacaoEExpedidor(int codigoRegraTipoOperacao, long codigoExpedidor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoExpedidor>();

            query = query.Where(o => o.RegraTipoOperacao.Codigo == codigoRegraTipoOperacao && o.Expedidor.CPF_CNPJ == codigoExpedidor);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoExpedidor BuscarPorExpedidor(double codigoExpedidor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoExpedidor>();

            query = query.Where(o => o.Expedidor.CPF_CNPJ == codigoExpedidor);

            return query.FirstOrDefault();
        }

        public List<RetornoRegraTipoOperacao> BuscarCodigosExpedidorPorRegra(List<int> codigoRegraTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoExpedidor>();
            query = query.Where(o => codigoRegraTipoOperacao.Contains(o.RegraTipoOperacao.Codigo));
            return query.Select(r => new RetornoRegraTipoOperacao()
            {
                CodigoRegra = r.RegraTipoOperacao.Codigo,
                CodigoCampoD = r.Expedidor.CPF_CNPJ
            }).ToList();
        }
        #endregion
    }
}