using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class ItemNaoConformidadeParticipante : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeParticipantes>
    {
        public ItemNaoConformidadeParticipante(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidadeParticipantes> BuscarPorItensNaoConformidadeAtivos()
        {
            var consultaItemNaoConformidadeParticipantes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeParticipantes>()
                .Where(item => item.CodigoItemNaoConformidade.IrrelevanteParaNC == false);

            return consultaItemNaoConformidadeParticipantes
                .Select(item => new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidadeParticipantes()
                {
                    Codigo = item.Codigo,
                    CodigoItemNaoConformidade = item.CodigoItemNaoConformidade.Codigo,
                    Participante = item.Participante
                })
                .ToList();
        }

        public void DeletarPorItemNaoConformidade(int codigoINC)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE from ItemNaoConformidadeParticipantes WHERE CodigoItemNaoConformidade = :INC_CODIGO")
                                     .SetInt32("INC_CODIGO", codigoINC)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE from ItemNaoConformidadeParticipantes WHERE CodigoItemNaoConformidade = :INC_CODIGO")
                                .SetInt32("INC_CODIGO", codigoINC)
                                .ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch (Exception ex)
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
