using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoValePedagio : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoValePedagio>
    {
        public PedidoValePedagio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoValePedagio BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoValePedagio>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoValePedagio BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoValePedagio>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Pedidos.PedidoValePedagio BuscarPorPedidoEComprovante(int codigoPedido, string comprovante)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoValePedagio>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido  && obj.NumeroComprovante == comprovante select obj;
            return result.FirstOrDefault();
        }

        public IList<Dominio.ObjetosDeValor.MDFe.ValePedagio> BuscarPorCarga(int codigoCarga)
        {
            //Adicionado conforme chamado 5888 para não usar Averbação da carga anterior quando Redespacho
            var sqlQuery = @"SELECT DISTINCT PEV_CNPJ_RESPONSAVEL CNPJResponsavel,
                                             PEV_CNPJ_FORNECEDOR CNPJFornecedor,
	                                         PEV_NUMERO_COMPROVANTE NumeroComprovante,
	                                         PEV_VALOR ValorValePedagio
                             FROM T_PEDIDO_VALE_PEDAGIO PV
                              JOIN T_CARGA_PEDIDO CP ON PV.PED_CODIGO = CP.PED_CODIGO
                              JOIN T_CARGA CA ON CP.CAR_CODIGO = CA.CAR_CODIGO
                             WHERE CA.RED_CODIGO IS NULL AND CA.CAR_CODIGO = " + codigoCarga.ToString();

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.MDFe.ValePedagio)));

            return query.List<Dominio.ObjetosDeValor.MDFe.ValePedagio>();
        }
    }
}
