using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.FaturamentoMensal
{
    public class FaturamentoMensalClienteServico : RepositorioBase<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico>
    {
        public FaturamentoMensalClienteServico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico> BuscarPorFaturamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico>();
            var result = from obj in query where obj.FaturamentoMensal.Codigo == codigo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico BuscarUltimoFaturamentoCliente(int codigo, int codigoFaturamentoDesconsiderara = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico>();
            var result = from obj in query where obj.Codigo != codigoFaturamentoDesconsiderara && obj.FaturamentoMensalCliente.Codigo == codigo && obj.FaturamentoMensal.StatusFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.Finalizado select obj;

            if (result.Count() > 0)
                return result.OrderBy("DataVencimento desc").FirstOrDefault();
            else
                return null;
        }

        public Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico BuscarPorNFSe(int codigoNFSe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico>();
            var result = from obj in query where obj.NotaFiscalServico.Codigo == codigoNFSe select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico BuscarPorNFe(int codigoNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico>();
            var result = from obj in query where obj.NotaFiscal.Codigo == codigoNFe select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico BuscarPorTitulo(int codigoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico>();
            var result = from obj in query where obj.Titulo.Codigo == codigoTitulo select obj;
            return result.FirstOrDefault();
        }

        public void DeletarPorFaturamento(int codigoFaturamento)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FaturamentoMensalClienteServico obj WHERE obj.FaturamentoMensal.Codigo = :codigoFaturamento")
                                     .SetInt32("codigoFaturamento", codigoFaturamento)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FaturamentoMensalClienteServico obj WHERE obj.FaturamentoMensal.Codigo = :codigoFaturamento")
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
