using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro.Conciliacao
{
    public class ConciliacaoBancaria : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria>
    {
        public ConciliacaoBancaria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool ExisteConciliacaoAberta(int codigoConciliacao, int codigoPlanoSintetico, int codigoPlanoAnalitico, DateTime? dataInicial, DateTime? dataFinal, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria>();

            query = query.Where(c => c.SituacaoConciliacaoBancaria == SituacaoConciliacaoBancaria.Aberto && c.Codigo != codigoConciliacao);

            if (codigoPlanoSintetico > 0)
                query = query.Where(c => c.PlanoContaSintetico.Codigo == codigoPlanoSintetico);

            if (codigoPlanoAnalitico > 0)
                query = query.Where(c => c.PlanoConta.Codigo == codigoPlanoAnalitico);

            if (dataInicial.HasValue && dataInicial.Value > DateTime.MinValue && dataFinal.HasValue && dataFinal.Value > DateTime.MinValue)
                query = query.Where(c => c.DataInicial == null || c.DataInicial <= dataInicial.Value || c.DataFinal == null || c.DataFinal <= dataFinal.Value);

            if (dataInicial.HasValue && dataInicial.Value > DateTime.MinValue)
                query = query.Where(c => c.DataInicial == null || c.DataInicial <= dataInicial.Value);

            if (dataFinal.HasValue && dataFinal.Value > DateTime.MinValue)
                query = query.Where(c => c.DataFinal == null || c.DataFinal >= dataFinal.Value);

            if (codigoEmpresa > 0)
                query = query.Where(c => c.Empresa.Codigo == codigoEmpresa);

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria> BuscarPorExtrato(int codigoExtrato, SituacaoConciliacaoBancaria situacaoConciliacaoBancaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria>();
            var result = from obj in query where obj.Extratos.Any(c => c.Codigo == codigoExtrato) && obj.SituacaoConciliacaoBancaria == situacaoConciliacaoBancaria select obj;
            return result.ToList();
        }

        public void DeletarExtratoBancario(int codigoExtrato, int codigoConciliacal)
        {
            try
            {

                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CONCILIACAO_BANCARIA_EXTRATO_BANCARIO WHERE EXB_CODIGO = { codigoExtrato } AND COB_CODIGO = { codigoConciliacal };").ExecuteUpdate(); // SQL-INJECTION-SAFE
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CONCILIACAO_BANCARIA_EXTRATO_BANCARIO WHERE EXB_CODIGO = { codigoExtrato } AND COB_CODIGO = { codigoConciliacal };").ExecuteUpdate(); // SQL-INJECTION-SAFE

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

        public void DeletarMovimentoBancarioNaoConsolidado(int codigoExtrato)
        {
            try
            {

                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery($@"DELETE Conciliacao from T_CONCILIACAO_BANCARIA_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO Conciliacao
                                                        JOIN T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO Movimento on Movimento.MDC_CODIGO = Conciliacao.MDC_CODIGO
                                                        where Conciliacao.COB_CODIGO =  { codigoExtrato } and (Movimento.MDC_MOVIMENTO_CONCOLIDADO = 0 or Movimento.MDC_MOVIMENTO_CONCOLIDADO IS NULL);").ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery($@"DELETE Conciliacao from T_CONCILIACAO_BANCARIA_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO Conciliacao
                                                    JOIN T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO Movimento on Movimento.MDC_CODIGO = Conciliacao.MDC_CODIGO
                                                    where Conciliacao.COB_CODIGO =  { codigoExtrato } and (Movimento.MDC_MOVIMENTO_CONCOLIDADO = 0 or Movimento.MDC_MOVIMENTO_CONCOLIDADO IS NULL);").ExecuteUpdate();

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

        public void DeletarExtratoBancarioNaoConsolidado(int codigoExtrato)
        {
            try
            {

                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery($@"DELETE Conciliacao from T_CONCILIACAO_BANCARIA_EXTRATO_BANCARIO Conciliacao
                                                        JOIN T_EXTRATO_BANCARIO Extrato on Extrato.EXB_CODIGO = Conciliacao.EXB_CODIGO
                                                        where Conciliacao.COB_CODIGO = { codigoExtrato } and (Extrato.MCO_ATIVO = 0 or Extrato.MCO_ATIVO IS NULL);").ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery($@"DELETE Conciliacao from T_CONCILIACAO_BANCARIA_EXTRATO_BANCARIO Conciliacao
                                                    JOIN T_EXTRATO_BANCARIO Extrato on Extrato.EXB_CODIGO = Conciliacao.EXB_CODIGO
                                                    where Conciliacao.COB_CODIGO = { codigoExtrato } and (Extrato.MCO_ATIVO = 0 or Extrato.MCO_ATIVO IS NULL);").ExecuteUpdate();

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

        public List<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoBancaria filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(filtrosPesquisa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoBancaria filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoBancaria filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.CodigoPlanoConta > 0)
                result = result.Where(obj => (obj.PlanoConta.Codigo == filtrosPesquisa.CodigoPlanoConta || obj.PlanoContaSintetico.Codigo == filtrosPesquisa.CodigoPlanoConta));

            if (filtrosPesquisa.CodigoOperador > 0)
                result = result.Where(obj => obj.Colaborador.Codigo == filtrosPesquisa.CodigoOperador);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.DataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataInicial.Value.Date >= filtrosPesquisa.DataInicial.Date);

            if (filtrosPesquisa.DataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataInicial.Value.Date <= filtrosPesquisa.DataFinal.Date);

            if (filtrosPesquisa.SituacaoConciliacaoBancaria != SituacaoConciliacaoBancaria.Todos)
                result = result.Where(obj => obj.SituacaoConciliacaoBancaria == filtrosPesquisa.SituacaoConciliacaoBancaria);

            if (filtrosPesquisa.ValorExtratoInicial != 0)
                result = result.Where(obj => obj.ValorTotalExtrato >= filtrosPesquisa.ValorExtratoInicial);

            if (filtrosPesquisa.ValorExtratoFinal != 0)
                result = result.Where(obj => obj.ValorTotalExtrato <= filtrosPesquisa.ValorExtratoFinal);

            if (filtrosPesquisa.ValorMovimentoInicial != 0)
                result = result.Where(obj => obj.ValorTotalMovimento >= filtrosPesquisa.ValorMovimentoInicial);

            if (filtrosPesquisa.ValorMovimentoFinal != 0)
                result = result.Where(obj => obj.ValorTotalMovimento <= filtrosPesquisa.ValorMovimentoFinal);

            if (filtrosPesquisa.DataGeracaoMovimentoFinanceiro != DateTime.MinValue)
                result = result.Where(obj => obj.Movimentos.Any(o => o.DataMovimento.Date == filtrosPesquisa.DataGeracaoMovimentoFinanceiro));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroDocumentoMovimentoFinanceiro))
                result = result.Where(obj => obj.Movimentos.Any(o => o.MovimentoFinanceiro.Documento == filtrosPesquisa.NumeroDocumentoMovimentoFinanceiro));

            if (filtrosPesquisa.ValorMovimentoFinanceiroInicial > 0 && filtrosPesquisa.ValorMovimentoFinanceiroFinal > 0)
                result = result.Where(obj => obj.Movimentos.Any(o => o.Valor >= filtrosPesquisa.ValorMovimentoFinanceiroInicial && o.Valor <= filtrosPesquisa.ValorMovimentoFinanceiroFinal));
            else if (filtrosPesquisa.ValorMovimentoFinanceiroInicial > 0)
                result = result.Where(obj => obj.Movimentos.Any(o => o.Valor >= filtrosPesquisa.ValorMovimentoFinanceiroInicial));
            else if (filtrosPesquisa.ValorMovimentoFinanceiroFinal > 0)
                result = result.Where(obj => obj.Movimentos.Any(o => o.Valor <= filtrosPesquisa.ValorMovimentoFinanceiroFinal));

            if (filtrosPesquisa.CodigoTitulo > 0)
                result = result.Where(obj => obj.Movimentos.Any(o => o.MovimentoFinanceiro.Titulo.Codigo == filtrosPesquisa.CodigoTitulo));

            return result;
        }

        #endregion
    }
}
