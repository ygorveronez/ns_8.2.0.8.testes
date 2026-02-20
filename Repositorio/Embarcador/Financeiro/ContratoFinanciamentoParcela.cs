using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class ContratoFinanciamentoParcela : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela>
    {
        public ContratoFinanciamentoParcela(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }


        public Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela BuscarPorParcela(int codigoContratoFinanciamento, int sequencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela>();
            var result = from obj in query where obj.Sequencia == sequencia && obj.ContratoFinanciamento.Codigo == codigoContratoFinanciamento select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela> BuscarPorContratoFinanciamento(int codigoContratoFinanciamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela>();

            query = from obj in query where obj.ContratoFinanciamento.Codigo == codigoContratoFinanciamento select obj;

            return query.ToList();
        }

        public decimal BuscarValorParcelasAnterior(int codigoContratoFinanciamento, int sequencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela>();

            query = from obj in query where obj.ContratoFinanciamento.Codigo == codigoContratoFinanciamento && obj.Sequencia <= sequencia select obj;

            return query.Sum(o => o.Valor);
        }

        public decimal BuscarAcrescimoParcelasAnterior(int codigoContratoFinanciamento, int sequencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela>();

            query = from obj in query where obj.ContratoFinanciamento.Codigo == codigoContratoFinanciamento && obj.Sequencia <= sequencia select obj;

            return query.Sum(o => o.ValorAcrescimo);
        }

        public int ContarPorContratoFinanciamento(int codigoContratoFinanciamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa>();
            var queryParcela = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela>();

            queryParcela = queryParcela.Where(o => o.ContratoFinanciamento.Codigo == codigoContratoFinanciamento);
            query = query.Where(o => queryParcela.Select(p => p.Titulo).Contains(o.Titulo));

            return query.Count();
        }

        public int ContarPorStatusEContratoFinanciamento(int codigoContratoFinanciamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var queryParcela = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela>();

            queryParcela = queryParcela.Where(o => o.ContratoFinanciamento.Codigo == codigoContratoFinanciamento);

            query = query.Where(o => o.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada);
            query = query.Where(o => queryParcela.Select(p => p.Titulo.Codigo).Contains(o.Codigo));

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarPorContratoFinanciamentoParcela(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.Select(o => o.Titulo).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela> Consultar(int codigoContratoFinanciamento, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela>();

            var result = from obj in query where obj.ContratoFinanciamento.Codigo == codigoContratoFinanciamento select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoContratoFinanciamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela>();

            var result = from obj in query where obj.ContratoFinanciamento.Codigo == codigoContratoFinanciamento select obj;

            return result.Count();
        }

        public void DeletarPorContratoFinanceiro(int codigo)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM ContratoFinanciamentoParcela c WHERE c.ContratoFinanciamento.Codigo = :codigo").SetInt32("codigo", codigo).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM ContratoFinanciamentoParcela c WHERE c.ContratoFinanciamento.Codigo = :codigo").SetInt32("codigo", codigo).ExecuteUpdate();

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
