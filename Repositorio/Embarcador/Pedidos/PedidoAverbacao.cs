using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoAverbacao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoAverbacao>
    {
        public PedidoAverbacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoAverbacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAverbacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoAverbacao BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAverbacao>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;
            return result.FirstOrDefault();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao> BuscarPorCarga(int codigoCarga)
        {
            var sqlQuery = @"SELECT DISTINCT PEA_CNPJ_RESPONSAVEL CNPJResponsavel,
                                             PEA_CNPJ_SEGURADORA CNPJSeguradora,
	                                         PEA_NOME_SEGURADORA NomeSeguradora,
	                                         PEA_NUMERO_APOLICE NumeroApolice,
	                                         PEA_NUMERO_AVERBACAO NumeroAverbacao
                             FROM T_PEDIDO_AVERBACAO PA
                              JOIN T_CARGA_PEDIDO CP ON PA.PED_CODIGO = CP.PED_CODIGO
                              JOIN T_CARGA CA ON CP.CAR_CODIGO = CA.CAR_CODIGO
                             WHERE CA.RED_CODIGO IS NULL AND CA.CAR_CODIGO = " + codigoCarga.ToString();

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao)));

            return query.List<Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao>();
        }
    }
}
