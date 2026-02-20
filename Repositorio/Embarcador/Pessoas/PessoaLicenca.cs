using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas
{
    public class PessoaLicenca : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.PessoaLicenca>
    {
        public PessoaLicenca(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pessoas.PessoaLicenca> BuscarLicencasParaAlerta()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaLicenca>();
            var result = from obj in query where obj.Pessoa.Ativo select obj;

            var queryAlerta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Alerta>();
            var resultAlerta = from obj in queryAlerta where obj.TelaAlerta == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ControleAlertaTela.Pessoa select obj;

            result = result.Where(o => !resultAlerta.Any(c => c.CodigoEntidade == o.Codigo));

            return result.ToList();
        }

        public bool ContemLicencaValida(int codigoTipoLicenca, DateTime dataAtual, double cnpjPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaLicenca>();
            var result = from obj in query where obj.Pessoa.CPF_CNPJ == cnpjPessoa && obj.Licenca.Codigo == codigoTipoLicenca && (!obj.DataVencimento.HasValue || obj.DataVencimento.Value >= dataAtual) select obj;

            return result.Any();
        }

        public void DeletarPorPessoa(double codigoPessoa)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE PessoaLicenca obj WHERE obj.Pessoa.CPF_CNPJ = :codigoPessoa")
                                     .SetDouble("codigoPessoa", codigoPessoa)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE PessoaLicenca obj WHERE obj.Pessoa.CPF_CNPJ = :codigoPessoa")
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

        public List<Dominio.Entidades.Embarcador.Pessoas.PessoaLicenca> BuscarPorPessoa(double cpfcnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaLicenca>();

            var result = query.Where(x => x.Pessoa.CPF_CNPJ == cpfcnpj);
            return result.ToList();
        }
        #endregion
    }
}
