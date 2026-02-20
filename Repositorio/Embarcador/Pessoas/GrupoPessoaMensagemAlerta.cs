using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;

namespace Repositorio.Embarcador.Pessoas
{
    public class GrupoPessoaMensagemAlerta : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaMensagemAlerta>
    {
        public GrupoPessoaMensagemAlerta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaMensagemAlerta> BuscarPorGrupoPessoas(int codigoGrupoPessoas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaMensagemAlerta>()
                .Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaMensagemAlerta> BuscarAlertasPorGrupoPessoas(int codigoGrupoPessoas, string observacaoNota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaMensagemAlerta>()
                .Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas && observacaoNota.Contains(o.Tag));

            return query.Distinct().ToList();
        }

        public void DeletarPorGrupoPessoa(int codigoGrupoPessoa)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE GrupoPessoaMensagemAlerta obj WHERE obj.GrupoPessoas.Codigo = :codigoGrupoPessoa")
                                          .SetInt32("codigoGrupoPessoa", codigoGrupoPessoa)
                                          .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE GrupoPessoaMensagemAlerta obj WHERE obj.GrupoPessoas.Codigo = :codigoGrupoPessoa")
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
