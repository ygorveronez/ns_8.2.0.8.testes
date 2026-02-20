using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CTe
{
    public class CTeTerceiroNotaFiscal : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal>
    {
        public CTeTerceiroNotaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal> BuscarPorCTeParaSubContratacao(int cteParaSubContratacao)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal>();
            var result = query.Where(obj => obj.CTeTerceiro.Codigo == cteParaSubContratacao);
            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroNotaFiscal> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<int> queryPedidoCTeParaSubContratacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>()
                .Where(o => o.CargaPedido.Codigo == codigoCargaPedido)
                .Select(a => a.CTeTerceiro.Codigo);

            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal>()
                .Where(o => queryPedidoCTeParaSubContratacao.Contains(o.CTeTerceiro.Codigo))
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroNotaFiscal()
                {
                    Numero = o.Numero,
                    Serie = o.Serie,
                    DataEmissao = o.DataEmissao,
                    CFOP = o.CFOP,
                    Peso = o.Peso,
                    ValorTotal = o.ValorTotal,
                    CTeTerceiro = new Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiro()
                    {
                        Codigo = o.CTeTerceiro.Codigo
                    }
                }).ToList();
        }

        public bool ExistePorCTeTerceiro(int codigoCTeTerceiro)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal>();

            query = query.Where(obj => obj.CTeTerceiro.Codigo == codigoCTeTerceiro);

            return query.Select(o => o.Codigo).Any();
        }

        public void DeletarPorCTeTerceiro(int codigoCTeTerceiro, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente)
        {
            try
            {
                if (codigoCTeTerceiro <= 0)
                    return;
                if (objetoValorPersistente != null)
                {
                    objetoValorPersistente.lstDelete.Add($" DELETE FROM T_CTE_TERCEIRO_NOTA_FISCAL WHERE  CPS_CODIGO = {codigoCTeTerceiro}"); // SQL-INJECTION-SAFE
                    return;
                }
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CTeTerceiroNotaFiscal obj WHERE obj.CTeTerceiro.Codigo = :codigoCTeTerceiro")
                                     .SetInt32("codigoCTeTerceiro", codigoCTeTerceiro)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CTeTerceiroNotaFiscal obj WHERE obj.CTeTerceiro.Codigo = :codigoCTeTerceiro")
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
