using System;

namespace Repositorio.Embarcador.Pessoas
{
    public class GrupoPessoasFuncionario : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFuncionario>
    {
        public GrupoPessoasFuncionario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public void DeletarPorGrupoPessoaEmpresa(int codigoGrupoPessoa, int codigoEmpresa)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    if (codigoEmpresa > 0)
                        UnitOfWork.Sessao.CreateQuery("DELETE GrupoPessoasFuncionario obj WHERE obj.GrupoPessoas.Codigo = :codigoGrupoPessoa and obj.Funcionario.Codigo IN (SELECT f.Codigo FROM Usuario f WHERE f.Empresa.Codigo =:codigoEmpresa)")
                                     .SetInt32("codigoGrupoPessoa", codigoGrupoPessoa)
                                     .SetInt32("codigoEmpresa", codigoEmpresa)
                                     .ExecuteUpdate();
                    else
                        UnitOfWork.Sessao.CreateQuery("DELETE GrupoPessoasFuncionario obj WHERE obj.GrupoPessoas.Codigo = :codigoGrupoPessoa")
                                         .SetInt32("codigoGrupoPessoa", codigoGrupoPessoa)
                                         .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        if (codigoEmpresa > 0)
                            UnitOfWork.Sessao.CreateQuery("DELETE GrupoPessoasFuncionario obj WHERE obj.GrupoPessoas.Codigo = :codigoGrupoPessoa and obj.Funcionario.Codigo IN (SELECT f.Codigo FROM Usuario f WHERE f.Empresa.Codigo =:codigoEmpresa)")
                                            .SetInt32("codigoGrupoPessoa", codigoGrupoPessoa)
                                            .SetInt32("codigoEmpresa", codigoEmpresa)
                                            .ExecuteUpdate();
                        else
                            UnitOfWork.Sessao.CreateQuery("DELETE GrupoPessoasFuncionario obj WHERE obj.GrupoPessoas.Codigo = :codigoGrupoPessoa")
                                                .SetInt32("codigoGrupoPessoa", codigoGrupoPessoa)
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
