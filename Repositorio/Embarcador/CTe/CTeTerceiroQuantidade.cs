using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CTe
{
    public class CTeTerceiroQuantidade : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade>
    {
        public CTeTerceiroQuantidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> BuscarPorCTeParaSubContratacao(int cteParaSubContratacao, List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> lstCTeTerceiroQuantidade = null)
        {
            if (lstCTeTerceiroQuantidade != null)
                return lstCTeTerceiroQuantidade.Where(obj => obj.CTeTerceiro.Codigo == cteParaSubContratacao).ToList();

            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade>();
            var result = query.Where(obj => obj.CTeTerceiro.Codigo == cteParaSubContratacao);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> BuscarPorCTeParaSubContratacaoSemPesoTotal(int cteParaSubContratacao)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade>();
            var result = query.Where(obj => obj.CTeTerceiro.Codigo == cteParaSubContratacao && obj.TipoMedida != "PESO TOTAL" && obj.TipoMedida != "PESO CUBADO");
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> BuscarPorCTesParaSubContratacao(List<int> cteParaSubContratacao)
        {
            if (cteParaSubContratacao.Count < 2000)
            {
                var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade>();
                var result = query.Where(obj => cteParaSubContratacao.Contains(obj.CTeTerceiro.Codigo));
                return result.ToList();
            }

            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> listaRetornar = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade>();
            List<int> listaOriginal = cteParaSubContratacao;
            int tamanhoLote = 2000;
            int indiceInicial = 0;

            while (indiceInicial < listaOriginal.Count)
            {
                List<int> lote = listaOriginal.GetRange(indiceInicial, Math.Min(tamanhoLote, listaOriginal.Count - indiceInicial));

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade>()
                      .Where(obj => lote.Contains(obj.CTeTerceiro.Codigo));

                listaRetornar.AddRange(
                    query
                    .ToList()
                    );

                indiceInicial += tamanhoLote;
            }

            return listaRetornar;
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> BuscarPorCTeParaSubContratacao(List<int> cteParaSubContratacao)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade>();
            var result = query.Where(obj => cteParaSubContratacao.Contains(obj.CTeTerceiro.Codigo));
            return result.ToList();
        }

        public decimal BuscarPesoPorCTeParaSubContratacao(int cteParaSubContratacao)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade>();
            var result = query.Where(obj => obj.CTeTerceiro.Codigo == cteParaSubContratacao && obj.Unidade == Dominio.Enumeradores.UnidadeMedida.KG);
            return result.Sum(o => (decimal?)o.Quantidade) ?? 0m;
        }

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade BuscarPorCTeTerceiroEUnidadeMedida(int codigoCTeTerceiro, Dominio.Enumeradores.UnidadeMedida unidadeMedida)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade>();

            query = query.Where(obj => obj.CTeTerceiro.Codigo == codigoCTeTerceiro && obj.Unidade == unidadeMedida);

            return query.FirstOrDefault();
        }

        public void DeletarPorCTeTerceiro(int codigoCTeTerceiro, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente)
        {
            try
            {
                if (codigoCTeTerceiro <= 0)
                    return;

                if (objetoValorPersistente != null)
                {
                    objetoValorPersistente.lstDelete.Add($" DELETE FROM T_CTE_TERCEIRO_QUANTIDADE WHERE  CPS_CODIGO = {codigoCTeTerceiro}"); // SQL-INJECTION-SAFE
                    return;
                }
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CTeTerceiroQuantidade obj WHERE obj.CTeTerceiro.Codigo = :codigoCTeTerceiro")
                                     .SetInt32("codigoCTeTerceiro", codigoCTeTerceiro)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CTeTerceiroQuantidade obj WHERE obj.CTeTerceiro.Codigo = :codigoCTeTerceiro")
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

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroQuantidade> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<int> queryPedidoCTeParaSubContratacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>()
                .Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.CTeTerceiro.Peso <= 0)
                .Select(a => a.CTeTerceiro.Codigo);

            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade>()
                .Where(o => queryPedidoCTeParaSubContratacao.Contains(o.CTeTerceiro.Codigo))
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiroQuantidade()
                {
                    Unidade = o.Unidade,
                    TipoMedida = o.TipoMedida,
                    Quantidade = o.Quantidade,
                    CTeTerceiro = new Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiro()
                    {
                        Codigo = o.CTeTerceiro.Codigo
                    }
                }).ToList();
        }
    }
}
