using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class NotaFiscalParcela : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela>
    {
        public NotaFiscalParcela(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela> BuscarPorNota(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela>();
            var result = from obj in query where obj.NotaFiscal.Codigo == codigo select obj;
            return result.ToList();
        }

        public void DeletarPorNFe(int codigoNFe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE NotaFiscalParcela obj WHERE obj.NotaFiscal.Codigo = :codigoNFe")
                                     .SetInt32("codigoNFe", codigoNFe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE NotaFiscalParcela obj WHERE obj.NotaFiscal.Codigo = :codigoNFe")
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

        public IList<Dominio.Relatorios.Embarcador.DataSource.NFe.DANFEParcela> BuscarParcelasDANFE(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool isRelatorio = false)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectParcelasDANFE(false, propriedades, notaFiscal, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite, isRelatorio));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NFe.DANFEParcela)));

            return query.List<Dominio.Relatorios.Embarcador.DataSource.NFe.DANFEParcela>();
        }

        private string ObterSelectParcelasDANFE(bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool isRelatorio)
        {
            string select = string.Empty;

            select = "SELECT N.NFI_CODIGO CodigoNota, " +
                    " P.NFP_CODIGO CodigoParcela, " +
                    " P.NFP_SEQUENCIA SequenciaParcela, " +
                    " P.NFP_DATA_VENCIMENTO DataVencimentoParcela, " +
                    " REPLICATE('0', 3 - LEN(P.NFP_SEQUENCIA)) + RTrim(P.NFP_SEQUENCIA) NumeroParcela," +//CAST(P.NFP_SEQUENCIA AS VARCHAR(30)) +' / ' + CAST(N.NFI_NUMERO AS VARCHAR(30)) NumeroParcela
                    " P.NFP_VALOR ValorParcela " +
                    " FROM T_NOTA_FISCAL N " +
                    " JOIN T_NOTA_FISCAL_PARCELA P ON P.NFI_CODIGO = N.NFI_CODIGO " +
                    " WHERE N.NFI_CODIGO = " + notaFiscal.Codigo +
                    " ORDER BY P.NFP_SEQUENCIA";
            return select;
        }

    }
}
