using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CTe
{
    public class CTeParcela : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CTeParcela>
    {
        public CTeParcela(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CTe.CTeParcela BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeParcela>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeParcela> BuscarParcelasPendentes()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeParcela>();
            var result = query.Where(obj => obj.ConhecimentoDeTransporteEletronico.Status == "A" && obj.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.Numero == "39");

            var queryTitulo = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var resultTitulo = queryTitulo.Where(obj => obj.CTeParcela != null);

            result = result.Where(obj => !resultTitulo.Select(p => p.CTeParcela.Codigo).Contains(obj.Codigo));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarTitulosParaCancelamento()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeParcela>();
            var result = query.Where(obj => obj.ConhecimentoDeTransporteEletronico.Status == "C" && obj.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.Numero == "39");

            var queryTitulo = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var resultTitulo = queryTitulo.Where(obj => obj.CTeParcela != null && obj.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto);

            resultTitulo = resultTitulo.Where(obj => result.Select(p => p.Codigo).Contains(obj.CTeParcela.Codigo));

            return resultTitulo.ToList();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeParcela> BuscarPorNFSe(int codigoNFSe)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeParcela>();
            var result = query.Where(obj => obj.ConhecimentoDeTransporteEletronico.Codigo == codigoNFSe);
            return result.ToList();
        }

        public void DeletarPorNFSe(int codigoNFSe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CTeParcela obj WHERE obj.ConhecimentoDeTransporteEletronico.Codigo = :codigoNFSe")
                                     .SetInt32("codigoNFSe", codigoNFSe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CTeParcela obj WHERE obj.ConhecimentoDeTransporteEletronico.Codigo = :codigoNFSe")
                                .SetInt32("codigoNFSe", codigoNFSe)
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
