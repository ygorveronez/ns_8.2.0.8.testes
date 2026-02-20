using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class NotaFiscalProdutos : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos>
    {
        public NotaFiscalProdutos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos> BuscarPorNota(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos>();
            var result = from obj in query where obj.NotaFiscal.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos> BuscarPorNotaOrdemDescricao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos>();
            var result = from obj in query where obj.NotaFiscal.Codigo == codigo select obj;
            return result.OrderBy("Sequencial").ToList();
        }

        public void DeletarPorNFe(int codigoNFe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE NotaFiscalProdutos obj WHERE obj.NotaFiscal.Codigo = :codigoNFe")
                                     .SetInt32("codigoNFe", codigoNFe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE NotaFiscalProdutos obj WHERE obj.NotaFiscal.Codigo = :codigoNFe")
                                .SetInt32("codigoNFe", codigoNFe)
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
