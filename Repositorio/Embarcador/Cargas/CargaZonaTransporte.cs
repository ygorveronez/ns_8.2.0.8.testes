using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaZonaTransporte : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaZonaTransporte>
    {
        public CargaZonaTransporte(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaZonaTransporte> BuscarCargaZonaTransportePorCarga(int codigoCarga)
        {
            string sqlQuery = @"select
                                PedidoAdicional.TDE_CODIGO_ZONA_TRANSPORTE CodigoZonaTransporte,
                                Pedido.PED_PESO_TOTAL_CARGA PesoTotalPedido,
                                Pedido.PED_CUBAGEM_TOTAL CubagemTotalPedido,
                                Pedido.PED_VALOR_TOTAL_NOTAS_FISCAIS ValorMercadoriaPedido,
                                CargaPedido.PED_ORDEM_ENTREGA Sequencia
                                FROM T_PEDIDO_ADICIONAL PedidoAdicional
                                JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = PedidoAdicional.PED_CODIGO
                                JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.PED_CODIGO = Pedido.PED_CODIGO
                                WHERE PedidoAdicional.TDE_CODIGO_ZONA_TRANSPORTE is not null
                                AND CargaPedido.CAR_CODIGO = :codigoCarga; ";
            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetParameter("codigoCarga", codigoCarga);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaZonaTransporte)));
            return query.List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaZonaTransporte>();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaZonaTransporte> BuscarListaCargaZonaTransportePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaZonaTransporte>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaZonaTransporte> Consultar(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaZonaTransporte>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaZonaTransporte>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            return result.Count();
        }

        public void AtualizarPrioridadesInferiores(int codigoCarga, int sequenciaAtual, int sequenciaAnterior)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery(
                        @"update CargaZonaTransporte
                             set Sequencia = (Sequencia - 1)
                           where Carga.Codigo = :codigoCarga
                             and Sequencia <= :sequenciaAtual
                             and Sequencia > :sequenciaAnterior"
                    )
                    .SetParameter("codigoCarga", codigoCarga)
                    .SetParameter("sequenciaAnterior", sequenciaAnterior)
                    .SetParameter("sequenciaAtual", sequenciaAtual)
                    .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao
                    .CreateQuery(
                        @"update CargaZonaTransporte
                             set Sequencia = (Sequencia - 1)
                           where Carga.Codigo = :codigoCarga
                             and Sequencia <= :sequenciaAtual
                             and Sequencia > :sequenciaAnterior"
                    )
                    .SetParameter("codigoCarga", codigoCarga)
                    .SetParameter("sequenciaAnterior", sequenciaAnterior)
                    .SetParameter("sequenciaAtual", sequenciaAtual)
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

        public void AtualizarPrioridadesSuperiores(int codigoCarga, int sequenciaAtual, int sequenciaAnterior)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery(
                        @"update CargaZonaTransporte
                             set Sequencia = (Sequencia + 1)
                           where Carga.Codigo = :codigoCarga
                             and Sequencia >= :sequenciaAtual
                             and Sequencia < :sequenciaAnterior"
                    )
                    .SetParameter("codigoCarga", codigoCarga)
                    .SetParameter("sequenciaAnterior", sequenciaAnterior)
                    .SetParameter("sequenciaAtual", sequenciaAtual)
                    .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao
                    .CreateQuery(
                        @"update CargaZonaTransporte
                             set Sequencia = (Sequencia + 1)
                           where Carga.Codigo = :codigoCarga
                             and Sequencia >= :sequenciaAtual
                             and Sequencia < :sequenciaAnterior"
                    )
                    .SetParameter("codigoCarga", codigoCarga)
                    .SetParameter("sequenciaAnterior", sequenciaAnterior)
                    .SetParameter("sequenciaAtual", sequenciaAtual)
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
