using Dominio.ObjetosDeValor.Embarcador.Pedido;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class RegraTipoOperacaoFilial : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoFilial>
    {
        public RegraTipoOperacaoFilial(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoFilial> BuscarPorRegraTipoOperacao(int codigoRegraTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoFilial>();

            query = query.Where(o => o.RegraTipoOperacao.Codigo == codigoRegraTipoOperacao);

            return query.ToList();
        }

        

        public Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoFilial BuscarPorRegraTipoOperacaoEFilial(int codigoTipoOperacao, int codigoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoFilial>();

            query = query.Where(o => o.RegraTipoOperacao.Codigo == codigoTipoOperacao && o.Filial.Codigo == codigoFilial);

            return query.FirstOrDefault();
        }



        public Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoFilial BuscarPorFilial(int codigoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoFilial>();

            query = query.Where(o => o.Filial.Codigo == codigoFilial);

            return query.FirstOrDefault();
        }

        public List<RetornoRegraTipoOperacao> BuscarCodigoFiliaisPorRegra(List<int> codigoRegras)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoFilial>();
            query = query.Where(o =>  codigoRegras.Contains(o.RegraTipoOperacao.Codigo));
            return query.Select(r => new RetornoRegraTipoOperacao()
            {
                CodigoCampo = r.Filial.Codigo,
                CodigoRegra = r.RegraTipoOperacao.Codigo
            }).ToList();
        }
        #endregion
    }
}