using Dominio.ObjetosDeValor.Embarcador.Pedido;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class RegraTipoOperacaoDestinatario : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoDestinatario>
    {
        public RegraTipoOperacaoDestinatario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoDestinatario> BuscarPorRegraTipoOperacao(int codigoRegraTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoDestinatario>();

            query = query.Where(o => o.RegraTipoOperacao.Codigo == codigoRegraTipoOperacao);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoDestinatario BuscarPorRegraTipoOperacaoEDestinatario(int codigoRegraTipoOperacao, double codigoExpedidor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoDestinatario>();

            query = query.Where(o => o.RegraTipoOperacao.Codigo == codigoRegraTipoOperacao && o.Destinatario.CPF_CNPJ == codigoExpedidor);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoDestinatario BuscarPorExpedidor(double codigoExpedidor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoDestinatario>();

            query = query.Where(o => o.Destinatario.CPF_CNPJ == codigoExpedidor);

            return query.FirstOrDefault();
        }

        public List<RetornoRegraTipoOperacao> BuscarCodigosDestinatarioPorRegra(List<int> codigoRegraTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoDestinatario>();
            query = query.Where(o => codigoRegraTipoOperacao.Contains(o.RegraTipoOperacao.Codigo));
            return query.Select(r => new RetornoRegraTipoOperacao()
            {
                CodigoRegra = r.RegraTipoOperacao.Codigo,
                CodigoCampoD = r.Destinatario.CPF_CNPJ
            }).ToList();
        }
        #endregion
    }
}