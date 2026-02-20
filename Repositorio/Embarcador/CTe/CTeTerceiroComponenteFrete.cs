using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.CTe
{
    public class CTeTerceiroComponenteFrete : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete>
    {
        public CTeTerceiroComponenteFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete> BuscarPorCTeParaSubContratacao(int codigoCTeParaSubcontratacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete>();

            query = query.Where(obj => obj.CTeTerceiro.Codigo == codigoCTeParaSubcontratacao);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete> BuscarPorCodigosCTeParaSubContratacao(List<int> codigosCTesParaSubcontratacao)
        {
            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete> result = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete>();

            int take = 1000;
            int start = 0;

            while (start < codigosCTesParaSubcontratacao.Count)
            {
                List<int> tmp = codigosCTesParaSubcontratacao.Skip(start).Take(take).ToList();

                var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete>();

                var filter = from obj in query
                             where tmp.Contains(obj.CTeTerceiro.Codigo)
                             select obj;

                result.AddRange(filter.ToList());

                start += take;
            }

            return result;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.ComponenteFrete> BuscarComponentesAgrupadosPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> queryPedidoCTeParaSubcontratacao = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            IQueryable<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete> queryCTeTerceiroComponenteFrete = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete>();

            queryPedidoCTeParaSubcontratacao = queryPedidoCTeParaSubcontratacao.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            queryCTeTerceiroComponenteFrete = queryCTeTerceiroComponenteFrete.Where(o => queryPedidoCTeParaSubcontratacao.Select(p => p.CTeTerceiro.Codigo).Contains(o.CTeTerceiro.Codigo));

            return queryCTeTerceiroComponenteFrete.GroupBy(o => o.Descricao).Select(o => new Dominio.ObjetosDeValor.Embarcador.CTe.ComponenteFrete()
            {
                Descricao = o.Key,
                Valor = o.Sum(v => v.Valor)
            }).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.ComponenteFrete> BuscarComponentesAgrupadosPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> queryPedidoCTeParaSubcontratacao = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            IQueryable<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete> queryCTeTerceiroComponenteFrete = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete>();

            queryPedidoCTeParaSubcontratacao = queryPedidoCTeParaSubcontratacao.Where(o => o.CargaPedido.Codigo == codigoCargaPedido);

            queryCTeTerceiroComponenteFrete = queryCTeTerceiroComponenteFrete.Where(o => queryPedidoCTeParaSubcontratacao.Select(p => p.CTeTerceiro.Codigo).Contains(o.CTeTerceiro.Codigo));

            return queryCTeTerceiroComponenteFrete.GroupBy(o => o.Descricao).Select(o => new Dominio.ObjetosDeValor.Embarcador.CTe.ComponenteFrete()
            {
                Descricao = o.Key,
                Valor = o.Sum(v => v.Valor)
            }).ToList();
        }

        public void DeletarPorCTeTerceiro(int codigoCTeTerceiro, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente)
        {
            try
            {
                if (codigoCTeTerceiro <= 0)
                    return;
                if (objetoValorPersistente != null)
                {
                    objetoValorPersistente.lstDelete.Add($" DELETE FROM T_CTE_TERCEIRO_COMPONENTE_FRETE WHERE  CPS_CODIGO = {codigoCTeTerceiro}"); // SQL-INJECTION-SAFE
                    return;
                }
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CTeTerceiroComponenteFrete obj WHERE obj.CTeTerceiro.Codigo = :codigoCTeTerceiro")
                                     .SetInt32("codigoCTeTerceiro", codigoCTeTerceiro)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CTeTerceiroComponenteFrete obj WHERE obj.CTeTerceiro.Codigo = :codigoCTeTerceiro")
                                    .SetInt32("codigoCTeTerceiro", codigoCTeTerceiro)
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
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }
    }
}
