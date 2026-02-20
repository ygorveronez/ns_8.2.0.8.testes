using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using AdminMultisoftware.Dominio.Enumeradores;

namespace AdminMultisoftware.Repositorio
{
    public class RepositorioBase<T> where T : AdminMultisoftware.Dominio.Entidades.EntidadeBase
    {

        #region Atributos Privados

        private readonly UnitOfWork _unitOfWork;

        #endregion

        #region Atributos Protegidos

        protected string StringConexao
        {
            get { return _unitOfWork.StringConexao; }
        }

        protected ISession SessionNHiBernate
        {
            get { return _unitOfWork.Sessao; }
        }

        protected UnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
        }

        protected List<TEntidade> ObterLista<TEntidade>(IQueryable<TEntidade> consulta, Dominio.ObjetosDeValor.Auditoria.ParametroConsulta parametroConsulta)
        {
            if (parametroConsulta != null)
            {
                //if (!string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeOrdenar))
                //    consulta = consulta.OrderBy(parametroConsulta.PropriedadeOrdenar + (parametroConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"));

                if (parametroConsulta.InicioRegistros > 0)
                    consulta = consulta.Skip(parametroConsulta.InicioRegistros);

                if (parametroConsulta.LimiteRegistros > 0)
                    consulta = consulta.Take(parametroConsulta.LimiteRegistros);
            }


            return consulta.ToList();
        }

        #endregion

        #region Construtores

        public RepositorioBase()
        {
            throw new NotImplementedException("Não utilizar, utilizar com string de conexão ou unidade de trabalho");
        }

        [Obsolete("Pode ocorrer memory leak, utilizar o construtor passando a unit of work.")]
        public RepositorioBase(string stringConexao) : this(new UnitOfWork(stringConexao)) { }

        public RepositorioBase(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados Auditoria

        private Dominio.Entidades.Auditoria.HistoricoObjeto Auditar(Dominio.ObjetosDeValor.Auditoria.Auditado Auditado, dynamic entidade, List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes, Dominio.Enumeradores.AcaoBancoDados acao, Dominio.Entidades.Auditoria.HistoricoObjeto historioPai, string descricaoAcao = "")
        {
            Auditoria.HistoricoObjeto repositorioHistoricoObjeto = new Auditoria.HistoricoObjeto(UnitOfWork);
            string nomeEntidade = entidade.GetType().Name.Replace("ProxyForFieldInterceptor", "").Replace("Proxy", "");

            if (historioPai == null)
            {
                Dominio.Entidades.Auditoria.HistoricoObjeto historico = new Dominio.Entidades.Auditoria.HistoricoObjeto
                {
                    CodigoObjeto = (long)entidade.Codigo,
                    Data = DateTime.Now,
                    Objeto = nomeEntidade,
                    Acao = acao,
                    Descricao = ((string)entidade.Descricao).Length > 299 ? ((string)entidade.Descricao).Substring(0, 299) : (string)entidade.Descricao,
                    DescricaoAcao = !string.IsNullOrWhiteSpace(descricaoAcao) ? descricaoAcao : acao.ObterDescricao(),
                    Usuario = Auditado.Usuario,
                    UsuarioMultisoftware = Auditado.Usuario?.Nome ?? "",
                    IP = Auditado.IP,
                    TipoAuditado = Auditado.TipoAuditado,
                    OrigemAuditado = Auditado.OrigemAuditado
                };

                if (alteracoes != null)
                    historico.Propriedades = alteracoes;

                repositorioHistoricoObjeto.Inserir(historico);

                return historico;
            }
            else
            {
                switch (acao)
                {
                    case Dominio.Enumeradores.AcaoBancoDados.Insert:
                        InserirHistoricoBag(historioPai, entidade, nomeEntidade);
                        repositorioHistoricoObjeto.Atualizar(historioPai);
                        return historioPai;

                    case Dominio.Enumeradores.AcaoBancoDados.Delete:
                        ExcluirHistoricoBag(historioPai, entidade, nomeEntidade);
                        repositorioHistoricoObjeto.Atualizar(historioPai);
                        return historioPai;

                    case Dominio.Enumeradores.AcaoBancoDados.Update:
                        if ((alteracoes?.Count ?? 0) > 0)
                        {
                            Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjetoFilho = Auditar(Auditado, entidade, alteracoes, acao, null);
                            AtualizarHistoricoBag(historioPai, entidade, nomeEntidade, historicoObjetoFilho);
                            repositorioHistoricoObjeto.Atualizar(historioPai);
                            return historicoObjetoFilho;
                        }
                        else
                            return historioPai;

                    default:
                        return null;
                }
            }
        }

        private void InserirHistoricoBag(Dominio.Entidades.Auditoria.HistoricoObjeto historicoPai, dynamic obj, string propriedade)
        {
            Dominio.Entidades.Auditoria.HistoricoPropriedade historicoPropriedade = new Dominio.Entidades.Auditoria.HistoricoPropriedade(propriedade, "", obj.Descricao);
            historicoPai.Propriedades.Add(historicoPropriedade);
        }
        private void AtualizarHistoricoBag(Dominio.Entidades.Auditoria.HistoricoObjeto historicoPai, dynamic obj, string propriedade, Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjetoGerado)
        {
            Dominio.Entidades.Auditoria.HistoricoPropriedade historicoPropriedade = new Dominio.Entidades.Auditoria.HistoricoPropriedade(propriedade, obj.Descricao, obj.Descricao, historicoObjetoGerado);
            historicoPai.Propriedades.Add(historicoPropriedade);
        }
        private void ExcluirHistoricoBag(Dominio.Entidades.Auditoria.HistoricoObjeto historicoPai, dynamic obj, string propriedade)
        {
            Dominio.Entidades.Auditoria.HistoricoPropriedade historicoPropriedade = new Dominio.Entidades.Auditoria.HistoricoPropriedade(propriedade, obj.Descricao, "");
            historicoPai.Propriedades.Add(historicoPropriedade);
        }

        #endregion

        #region Métodos Públicos

        public void Atualizar(T objeto, Dominio.ObjetosDeValor.Auditoria.Auditado Auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historioPai = null, string descricaoAcao = "")
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                if (Auditado != null)
                    Auditar(Auditado, objeto, objeto.GetChanges(), Dominio.Enumeradores.AcaoBancoDados.Update, historioPai, descricaoAcao);

                SessionNHiBernate.Update(objeto);
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    if (Auditado != null)
                        Auditar(Auditado, objeto, objeto.GetChanges(), Dominio.Enumeradores.AcaoBancoDados.Update, historioPai, descricaoAcao);

                    UnitOfWork.Sessao.Update(objeto);

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }

        public T BuscarPorCodigo(int codigo)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();

            criteria.Add(NHibernate.Criterion.Expression.Eq("Codigo", codigo));

            T objeto = criteria.UniqueResult<T>();

            return objeto;
        }

        public T BuscarPorCodigo(long codigo)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();

            criteria.Add(NHibernate.Criterion.Expression.Eq("Codigo", codigo));

            T objeto = criteria.UniqueResult<T>();

            return objeto;
        }

        public List<T> BuscarPorEmpresa(int codigoEmpresa, int inicioRegistros, int maximoRegistros, string propriedadeOrdenar, bool decrescente = false)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();
            if (!string.IsNullOrWhiteSpace(propriedadeOrdenar))
            {
                if (decrescente)
                    criteria.AddOrder(NHibernate.Criterion.Order.Desc(propriedadeOrdenar));
                else
                    criteria.AddOrder(NHibernate.Criterion.Order.Asc(propriedadeOrdenar));
            }
            criteria.Add(NHibernate.Criterion.Expression.Eq("Empresa.Codigo", codigoEmpresa));
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<T>().ToList();
        }

        public T BuscarPrimeiroRegistro()
        {
            var query = this.SessionNHiBernate.Query<T>();

            return query
                .FirstOrDefault();
        }

        public List<T> BuscarTodos(int inicioRegistros, int maximoRegistros, string propriedadeOrdenar, bool decrescente = false)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();
            if (!string.IsNullOrWhiteSpace(propriedadeOrdenar))
            {
                if (decrescente)
                    criteria.AddOrder(NHibernate.Criterion.Order.Desc(propriedadeOrdenar));
                else
                    criteria.AddOrder(NHibernate.Criterion.Order.Asc(propriedadeOrdenar));
            }
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<T>().ToList();
        }

        public List<T> BuscarTodos()
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();

            return criteria.List<T>().ToList();
        }

        public int ContarPorEmpresa(int codigoEmpresa)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();
            criteria.Add(NHibernate.Criterion.Expression.Eq("Empresa.Codigo", codigoEmpresa));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public int ContarTodos()
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public int ContarPorFilial(int codigoFilial)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();
            criteria.Add(NHibernate.Criterion.Expression.Eq("Filial.Codigo", codigoFilial));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public List<T> BuscarPorFilial(int codigoFilial, int inicioRegistros, int maximoRegistros, string propriedadeOrdenar, bool decrescente = false)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();
            if (!string.IsNullOrWhiteSpace(propriedadeOrdenar))
            {
                if (decrescente)
                    criteria.AddOrder(NHibernate.Criterion.Order.Desc(propriedadeOrdenar));
                else
                    criteria.AddOrder(NHibernate.Criterion.Order.Asc(propriedadeOrdenar));
            }
            criteria.Add(NHibernate.Criterion.Expression.Eq("Filial.Codigo", codigoFilial));
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<T>().ToList();
        }

        public void Deletar(T objeto, Dominio.ObjetosDeValor.Auditoria.Auditado Auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historioPai = null, string descricaoAcao = "")
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    if (Auditado != null)
                        Auditar(Auditado, objeto, null, Dominio.Enumeradores.AcaoBancoDados.Delete, historioPai);

                    UnitOfWork.Sessao.Delete(objeto);
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        if (Auditado != null)
                            Auditar(Auditado, objeto, null, Dominio.Enumeradores.AcaoBancoDados.Delete, historioPai);

                        UnitOfWork.Sessao.Delete(objeto);

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

        public long Inserir(T objeto, Dominio.ObjetosDeValor.Auditoria.Auditado Auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historioPai = null, string descricaoAcao = "")
        {
            object returning = null;
            if (UnitOfWork.IsActiveTransaction())
            {
                returning = UnitOfWork.Sessao.Save(objeto);

                if (Auditado != null)
                    Auditar(Auditado, objeto, null, Dominio.Enumeradores.AcaoBancoDados.Insert, historioPai);
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    returning = UnitOfWork.Sessao.Save(objeto);

                    if (Auditado != null)
                        Auditar(Auditado, objeto, null, Dominio.Enumeradores.AcaoBancoDados.Insert, historioPai);

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }

            return Convert.ToInt64(returning);
        }

        public void DeletarPorEntidade(Dominio.Entidades.EntidadeBase entidade)
        {
            try
            {
                dynamic entidadeBase = entidade;

                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery($"DELETE FROM {this.GetType().Name} c WHERE c.{entidadeBase.GetType().Name}.Codigo = :codigo").SetInt32("codigo", entidadeBase.Codigo).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery($"DELETE FROM {this.GetType().Name} c WHERE c.{entidade.GetType().Name}.Codigo = :codigo").SetInt32("codigo", entidadeBase.Codigo).ExecuteUpdate();

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

        #endregion
    }
}
