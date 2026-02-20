using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.FaturamentoMensal
{
    public class FaturamentoMensalServico : RepositorioBase<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalServico>
    {
        public FaturamentoMensalServico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalServico BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalServico>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalServico> BuscarServicosExtrasFaturamento(int codigoFaturamentoCliente, DateTime dataVencimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalServico>();

            var primeiroDia = new DateTime(dataVencimento.Year, dataVencimento.Month, 1);
            var ultimoDia = new DateTime(dataVencimento.Year, dataVencimento.Month,
                    DateTime.DaysInMonth(dataVencimento.Year, dataVencimento.Month));

            var result = from obj in query
                         where obj.FaturamentoMensalCliente.Codigo == codigoFaturamentoCliente &&
                               (obj.DataLancamento != null && obj.DataLancamentoAte != null ?
                               (obj.DataLancamento.Value <= primeiroDia && obj.DataLancamentoAte.Value >= ultimoDia) :
                               (obj.DataLancamento == null || (obj.DataLancamento.Value.Month == dataVencimento.AddMonths(-1).Month && obj.DataLancamento.Value.Year == dataVencimento.AddMonths(-1).Year)))
                         select obj;
            return result.ToList();
        }

        public decimal ValorServicoExtra(int codigoFaturamentoCliente, DateTime? dataVencimento)
        {
            if (dataVencimento == null || !dataVencimento.HasValue)
                return 0;

            var primeiroDia = new DateTime(dataVencimento.Value.Year, dataVencimento.Value.Month, 1);
            var ultimoDia = new DateTime(dataVencimento.Value.Year, dataVencimento.Value.Month,
                    DateTime.DaysInMonth(dataVencimento.Value.Year, dataVencimento.Value.Month));

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalServico>();
            var result = from obj in query
                         where obj.FaturamentoMensalCliente.Codigo == codigoFaturamentoCliente &&
                               (obj.DataLancamento != null && obj.DataLancamentoAte != null ?                               
                               (obj.DataLancamento.Value <= primeiroDia && obj.DataLancamentoAte.Value >= ultimoDia) :
                               (obj.DataLancamento == null || (obj.DataLancamento.Value.Month == dataVencimento.Value.AddMonths(-1).Month && obj.DataLancamento.Value.Year == dataVencimento.Value.AddMonths(-1).Year)))
                         select obj;
            if (result.Count() > 0)
                return result.Select(obj => obj.ValorTotal).Sum();
            else
                return 0;
        }

        public void DeletarPorFaturamento(int codigoFaturamento)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FaturamentoMensalServico obj WHERE obj.FaturamentoMensalCliente.Codigo = :codigoFaturamento")
                                     .SetInt32("codigoFaturamento", codigoFaturamento)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE NotaFiscalProdutos obj WHERE obj.FaturamentoMensalCliente.Codigo = :codigoFaturamento")
                                .SetInt32("codigoFaturamento", codigoFaturamento)
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

