using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class XMLNotaFiscalComponente : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente>
    {
        public XMLNotaFiscalComponente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public bool ExistePorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> queryPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            queryPedidoXMLNotaFiscal = queryPedidoXMLNotaFiscal.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            query = query.Where(o => queryPedidoXMLNotaFiscal.Select(p => p.XMLNotaFiscal.Codigo).Contains(o.XMLNotaFiscal.Codigo));

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente> BuscarPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> queryPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            queryPedidoXMLNotaFiscal = queryPedidoXMLNotaFiscal.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            query = query.Where(o => queryPedidoXMLNotaFiscal.Select(p => p.XMLNotaFiscal.Codigo).Contains(o.XMLNotaFiscal.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente> BuscarPorXMLNotaFiscal(int codigoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente>();

            query = query.Where(o => o.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente BuscarPorXMLNotaFiscalEComponenteFrete(int codigoXMLNotaFiscal, int codigoComponenteFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente>();

            query = query.Where(o => o.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal && o.ComponenteFrete.Codigo == codigoComponenteFrete);

            return query.FirstOrDefault();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Pedido.XMLNotaFiscalComponente> BuscarSumarizadoPorCargaPedido(int carga)
        {
            string query = $"select CargaPedido.CPE_CODIGO CodigoCargaPedido, Componente.CFR_CODIGO CodigoComponenteFrete, " +
                             $"sum(Componente.XNC_VALOR) Valor from T_XML_NOTA_FISCAL_COMPONENTE as Componente " +
                             $"inner join T_PEDIDO_XML_NOTA_FISCAL as PedidoXMLNotaFiscal on Componente.NFX_CODIGO = PedidoXMLNotaFiscal.NFX_CODIGO " +
                             $"inner join T_CARGA_PEDIDO as CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO " +
                             $"where CargaPedido.CAR_CODIGO = {carga} Group by CargaPedido.CPE_CODIGO, Componente.CFR_CODIGO ";


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Pedido.XMLNotaFiscalComponente)));

            return nhQuery.List<Dominio.ObjetosDeValor.Embarcador.Pedido.XMLNotaFiscalComponente>();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Pedido.XMLNotaFiscalComponente> BuscarSumarizadoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente>();
            var queryPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            queryPedidoXMLNotaFiscal = queryPedidoXMLNotaFiscal.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            query = query.Where(o => o.XMLNotaFiscal.nfAtiva && queryPedidoXMLNotaFiscal.Select(pnf => pnf.XMLNotaFiscal.Codigo).Contains(o.XMLNotaFiscal.Codigo));

            return query.GroupBy(o => new { CodigoComponenteFrete = o.ComponenteFrete.Codigo, IncluirICMS = o.IncluirICMS, IncluirIntegralmenteContratoFreteTerceiro = o.IncluirIntegralmenteContratoFreteTerceiro }).Select(o => new Dominio.ObjetosDeValor.Embarcador.Pedido.XMLNotaFiscalComponente()
            {
                CodigoComponenteFrete = o.Key.CodigoComponenteFrete,
                Valor = o.Sum(c => c.Valor),
                IncluirICMS = o.Key.IncluirICMS,
                IncluirIntegralmenteContratoFreteTerceiro = o.Key.IncluirIntegralmenteContratoFreteTerceiro
            }).ToList();
        }

        public void DeletarPorXMLNotaFiscal(int codigoXMLNotaFiscal)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao.CreateQuery("DELETE XMLNotaFiscalComponente obj WHERE obj.XMLNotaFiscal.Codigo = :codigoXMLNotaFiscal")
                                 .SetInt32("codigoXMLNotaFiscal", codigoXMLNotaFiscal)
                                 .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao.CreateQuery("DELETE XMLNotaFiscalComponente obj WHERE obj.XMLNotaFiscal.Codigo = :codigoXMLNotaFiscal")
                                .SetInt32("codigoXMLNotaFiscal", codigoXMLNotaFiscal)
                                .ExecuteUpdate();

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }
    }
}
