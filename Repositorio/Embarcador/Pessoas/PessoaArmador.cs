using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class PessoaArmador : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.PessoaArmador>
    {
        public PessoaArmador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pessoas.PessoaArmador> BuscarPorPessoa(double cpfCnpjPessoa)
        {
            var PessoaArmador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaArmador>()
               .Where(o =>
                   o.Pessoa.CPF_CNPJ == cpfCnpjPessoa
               );

            return PessoaArmador.ToList();
        }

        public Dominio.Entidades.Embarcador.Pessoas.PessoaArmador BuscarPorPessoaETipoContainer(double cpfCnpjPessoa, int tipoContainer)
        {
            var PessoaArmador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaArmador>()
               .Where(o =>
                   o.Pessoa.CPF_CNPJ == cpfCnpjPessoa && o.ContainerTipo.Codigo == tipoContainer
               );

            return PessoaArmador.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.PessoaArmador BuscarPorPessoaETipoContainer(double cpfCnpjPessoa, int tipoContainer, DateTime data)
        {
            var PessoaArmador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaArmador>()
               .Where(o =>
                   o.Pessoa.CPF_CNPJ == cpfCnpjPessoa && o.ContainerTipo.Codigo == tipoContainer
               );
            PessoaArmador = PessoaArmador.Where(obj => 
                (obj.DataVigenciaInicial < data && data < obj.DataVigenciaFinal) ||
                (!obj.DataVigenciaInicial.HasValue && data <= obj.DataVigenciaFinal) ||
                (!obj.DataVigenciaFinal.HasValue && data >= obj.DataVigenciaInicial)
                );

            return PessoaArmador.FirstOrDefault();
        }

        public void DeletarPorPessoa(double codigoPessoa)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE PessoaArmador obj WHERE obj.Pessoa.CPF_CNPJ = :codigoPessoa")
                                     .SetDouble("codigoPessoa", codigoPessoa)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE PessoaArmador obj WHERE obj.Pessoa.CPF_CNPJ = :codigoPessoa")
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

        #endregion
    }
}
