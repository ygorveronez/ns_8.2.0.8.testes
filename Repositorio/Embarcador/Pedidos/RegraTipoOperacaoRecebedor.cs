using Dominio.ObjetosDeValor.Embarcador.Pedido;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class RegraTipoOperacaoRecebedor : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoRecebedor>
    {
        public RegraTipoOperacaoRecebedor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoRecebedor> BuscarPorRegraTipoOperacao(int codigoRegraTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoRecebedor>();

            query = query.Where(o => o.RegraTipoOperacao.Codigo == codigoRegraTipoOperacao);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoRecebedor BuscarPorRegraTipoOperacaoERecebedor(int codigoRegra, long codigoRecebedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoRecebedor>();

            query = query.Where(o => o.RegraTipoOperacao.Codigo == codigoRegra && o.Recebedor.CPF_CNPJ == codigoRecebedor);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoRecebedor BuscarPorRecebedor(double codigoRecebedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoRecebedor>();

            query = query.Where(o => o.Recebedor.CPF_CNPJ == codigoRecebedor);

            return query.FirstOrDefault();
        }

        public List<RetornoRegraTipoOperacao> BuscarCodigosRecebedorPorRegra(List<int> codigoRegraTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoRecebedor>();
            query = query.Where(o => codigoRegraTipoOperacao.Contains(o.RegraTipoOperacao.Codigo));
            return query.Select(r => new RetornoRegraTipoOperacao()
            {
                CodigoCampoD = r.Recebedor.CPF_CNPJ,
                CodigoRegra = r.RegraTipoOperacao.Codigo
            }).ToList();
        }
        #endregion
    }
}