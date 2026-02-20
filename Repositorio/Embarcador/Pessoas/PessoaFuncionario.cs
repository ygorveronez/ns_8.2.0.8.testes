using NHibernate.Linq;
using System;
using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class PessoaFuncionario : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.PessoaFuncionario>
    {
        public PessoaFuncionario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pessoas.PessoaFuncionario BuscarPorFuncionarioEPessoa(int codigoFuncionario, double cpfCnpjPessoa)
        {
            var consultaPessoaFuncionario = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaFuncionario>()
               .Where(o =>
                   o.Pessoa.CPF_CNPJ == cpfCnpjPessoa &&
                   o.Funcionario.Codigo == codigoFuncionario &&
                   o.DataInicioVigencia <= DateTime.Now &&
                   o.DataFimVigencia >= DateTime.Now
               );

            return consultaPessoaFuncionario.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.PessoaFuncionario BuscarPorPessoaECodigoIntegracao(string codigoIntegracao, double cpfCnpjPessoa)
        {
            var consultaPessoaFuncionario = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaFuncionario>()
               .Where(o =>
                   o.Pessoa.CPF_CNPJ == cpfCnpjPessoa &&
                   o.Funcionario.CodigoIntegracao == codigoIntegracao
               );

            return consultaPessoaFuncionario.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarVendedorPorPessoaETipoCarga(double cpfCnpjPessoa, int codigoTipoCarga)
        {
            var consultaPessoaFuncionario = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaFuncionario>()
                .Where(o =>
                    o.Pessoa.CPF_CNPJ == cpfCnpjPessoa &&
                    o.Funcionario != null
                );

            if (codigoTipoCarga > 0)
                consultaPessoaFuncionario = consultaPessoaFuncionario.Where(o => o.TipoDeCarga.Codigo == codigoTipoCarga);

            return consultaPessoaFuncionario
                .Select(o => o.Funcionario)
                .Fetch(o => o.Supervisor)
                .Fetch(o => o.Gerente)
                .FirstOrDefault();
        }

        public void DeletarPorPessoaEmpresa(double codigoPessoa, int codigoEmpresa)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    if (codigoEmpresa > 0)
                        UnitOfWork.Sessao.CreateQuery("DELETE PessoaFuncionario obj WHERE obj.Pessoa.CPF_CNPJ = :codigoPessoa and obj.Funcionario.Codigo IN (SELECT f.Codigo FROM Usuario f WHERE f.Empresa.Codigo =:codigoEmpresa)")
                                     .SetDouble("codigoPessoa", codigoPessoa)
                                     .SetInt32("codigoEmpresa", codigoEmpresa)
                                     .ExecuteUpdate();
                    else
                        UnitOfWork.Sessao.CreateQuery("DELETE PessoaFuncionario obj WHERE obj.Pessoa.CPF_CNPJ = :codigoPessoa")
                                         .SetDouble("codigoPessoa", codigoPessoa)
                                         .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        if (codigoEmpresa > 0)
                            UnitOfWork.Sessao.CreateQuery("DELETE PessoaFuncionario obj WHERE obj.Pessoa.CPF_CNPJ = :codigoPessoa and obj.Funcionario.Codigo IN (SELECT f.Codigo FROM Usuario f WHERE f.Empresa.Codigo =:codigoEmpresa)")
                                            .SetDouble("codigoPessoa", codigoPessoa)
                                            .SetInt32("codigoEmpresa", codigoEmpresa)
                                            .ExecuteUpdate();
                        else
                            UnitOfWork.Sessao.CreateQuery("DELETE PessoaFuncionario obj WHERE obj.Pessoa.CPF_CNPJ = :codigoPessoa")
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
