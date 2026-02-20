using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class MontagemCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamento>
    {
        public MontagemCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamento BuscarPrimerioCarregamento(int codigoSessaoRoteirizador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamento>();
            if (codigoSessaoRoteirizador > 0)
                query = query.Where(x => x.Carregamento.SessaoRoteirizador.Codigo == codigoSessaoRoteirizador);
            var result = from obj in query select obj;
            return result.FirstOrDefault();
        }

        public List<int> SessoesRoteirizador()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamento>();
            return query.Where(o => o.Carregamento.SessaoRoteirizador != null).Select(o => o.Carregamento.SessaoRoteirizador.Codigo).Distinct().ToList();
        }

        public List<int> BusarCodigosCarregamento(int cod_sessao_roteirizador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento situacaoCarregamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.EmMontagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamento>();
            if (cod_sessao_roteirizador > 0)
                query = query.Where(x => x.Carregamento.SessaoRoteirizador != null && x.Carregamento.SessaoRoteirizador.Codigo == cod_sessao_roteirizador && x.Carregamento.SituacaoCarregamento == situacaoCarregamento);
            var result = from obj in query select obj.Carregamento.Codigo;
            return result.ToList();
        }

        public void DeletarTodos(List<int> codigosCarregamentos) //int codigoSessaoRoteirizacao)
        {
            try
            {
                if (codigosCarregamentos.Count == 0) return;
                //                if (UnitOfWork.IsActiveTransaction())
                //                {
                //                    if (codigoSessaoRoteirizacao == 0)
                //                        UnitOfWork.Sessao.CreateQuery("DELETE FROM MontagemCarregamento").ExecuteUpdate();
                //                    else
                //                        UnitOfWork.Sessao.CreateQuery(@"
                //DELETE FROM MontagemCarregamento 
                // WHERE Codigo in (SELECT c.Codigo 
                //                    FROM MontagemCarregamento c
                //                   WHERE c.Carregamento.SessaoRoteirizador.Codigo = :codigoSessaoRoteirizador)").SetInt32("codigoSessaoRoteirizador", codigoSessaoRoteirizacao).ExecuteUpdate();
                //                }
                //                else
                //                {
                //                    using (UnitOfWork.Start())
                //                    {
                //                        try
                //                        {
                //                            if (codigoSessaoRoteirizacao == 0)
                //                                UnitOfWork.Sessao.CreateQuery("DELETE FROM MontagemCarregamento").ExecuteUpdate();
                //                            else
                //                                UnitOfWork.Sessao.CreateQuery(@"
                //DELETE FROM MontagemCarregamento 
                // WHERE Codigo in (SELECT c.Codigo 
                //                    FROM MontagemCarregamento c
                //                   WHERE c.Carregamento.SessaoRoteirizador.Codigo = :codigoSessaoRoteirizador)").SetInt32("codigoSessaoRoteirizador", codigoSessaoRoteirizacao).ExecuteUpdate();
                //                            UnitOfWork.Sessao.Transaction.Commit();
                //                        }
                //                        catch
                //                        {
                //                            UnitOfWork.Rollback();
                //                            throw;
                //                        }
                //                    }
                //                }

                if (UnitOfWork.IsActiveTransaction())
                    UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_MONTAGEM_CARREGAMENTO WHERE crg_codigo IN (:codigos)").SetParameterList("codigos", codigosCarregamentos).ExecuteUpdate();
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_MONTAGEM_CARREGAMENTO WHERE crg_codigo IN (:codigos)").SetParameterList("codigos", codigosCarregamentos).ExecuteUpdate();

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
