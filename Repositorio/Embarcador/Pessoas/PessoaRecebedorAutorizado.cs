using System;
using System.Linq;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Pessoas
{
    public class PessoaRecebedorAutorizado : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.PessoaRecebedorAutorizado>
    {
        public PessoaRecebedorAutorizado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.PessoaRecebedorAutorizado> BuscarPorPessoa(double cpfCnpjPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaRecebedorAutorizado>()
               .Where(o =>
                   o.Pessoa.CPF_CNPJ == cpfCnpjPessoa
               );

            return query.ToList();
        }

        public void DeletarPorPessoaEmpresa(double codigoPessoa)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE PessoaRecebedorAutorizado obj WHERE obj.Pessoa.CPF_CNPJ = :codigoPessoa")
                                        .SetDouble("codigoPessoa", codigoPessoa)
                                        .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE PessoaRecebedorAutorizado obj WHERE obj.Pessoa.CPF_CNPJ = :codigoPessoa")
                                            .SetDouble("codigoPessoa", codigoPessoa)
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
