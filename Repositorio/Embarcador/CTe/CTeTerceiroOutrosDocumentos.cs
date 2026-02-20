using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CTe
{
    public class CTeTerceiroOutrosDocumentos : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos>
    {
        public CTeTerceiroOutrosDocumentos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos> BuscarPorCTeParaSubContratacao(int cteParaSubContratacao)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos>();
            var result = query.Where(obj => obj.CTeTerceiro.Codigo == cteParaSubContratacao);
            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroOutrosDocumentos> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<int> queryPedidoCTeParaSubContratacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>()
                .Where(o => o.CargaPedido.Codigo == codigoCargaPedido)
                .Select(a => a.CTeTerceiro.Codigo);

            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos>()
                .Where(o => queryPedidoCTeParaSubContratacao.Contains(o.CTeTerceiro.Codigo))
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroOutrosDocumentos()
                {
                    Descricao = o.Descricao,
                    NCM = o.NCM,
                    Numero = o.Numero,
                    Tipo = o.Tipo,
                    Valor = o.Valor,
                    CTeTerceiro = new Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiro()
                    {
                        Codigo = o.CTeTerceiro.Codigo,
                        Serie = o.CTeTerceiro.Serie
                    }
                }).ToList();
        }

        public bool ExistePorCTeTerceiro(int codigoCTeTerceiro)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos>();

            query = query.Where(obj => obj.CTeTerceiro.Codigo == codigoCTeTerceiro);

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos> BuscarPorChave(string chaveCTe)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos>();
            var result = query.Where(obj => obj.CTeTerceiro.ChaveAcesso == chaveCTe);
            return result.ToList();
        }

        public void DeletarPorCTeTerceiro(int codigoCTeTerceiro, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente)
        {
            try
            {
                if (codigoCTeTerceiro <= 0)
                    return;

                if (objetoValorPersistente != null)
                {
                    objetoValorPersistente.lstDelete.Add($" DELETE FROM T_CTE_TERCEIRO_OUTROS_DOCUMENTOS WHERE  CPS_CODIGO = {codigoCTeTerceiro}"); // SQL-INJECTION-SAFE
                    return;
                }
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CTeTerceiroOutrosDocumentos obj WHERE obj.CTeTerceiro.Codigo = :codigoCTeTerceiro")
                                     .SetInt32("codigoCTeTerceiro", codigoCTeTerceiro)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CTeTerceiroOutrosDocumentos obj WHERE obj.CTeTerceiro.Codigo = :codigoCTeTerceiro")
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
