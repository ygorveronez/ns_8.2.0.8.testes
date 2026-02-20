using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Fatura
{
    public class FaturaDocumentoAcrescimoDesconto : RepositorioBase<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto>
    {
        public FaturaDocumentoAcrescimoDesconto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto> BuscarPorFaturaDocumento(int codigoFaturaDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.FaturaDocumento.Codigo == codigoFaturaDocumento);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto> BuscarPorFatura(int codigoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.FaturaDocumento.Fatura.Codigo == codigoFatura);

            return query.Fetch(o => o.FaturaDocumento)
                        .Fetch(o => o.Justificativa)
                        .Fetch(o => o.TipoMovimentoReversao)
                        .Fetch(o => o.TipoMovimentoUso)
                        .ToList();
        }

        public decimal BuscarDescontoPorFaturaDocumento(int codigoFatura, int codigoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.FaturaDocumento.Documento.CTe.Codigo == codigoDocumento && o.FaturaDocumento.Fatura.Codigo == codigoFatura && o.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto);

            return query.Sum(o => (decimal?)o.Valor) ?? 0m;
        }

        public Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto BuscarPorCodigo(int codigoAcrescimoDesconto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.Codigo == codigoAcrescimoDesconto);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto> Consultar(int codigoDocumento, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.FaturaDocumento.Codigo == codigoDocumento);

            return query.OrderBy(propOrdena + " " + dirOrdena)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();
        }

        public int ContarConsulta(int codigoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.FaturaDocumento.Codigo == codigoDocumento);

            return query.Count();
        }

        public void DeletarPorFaturaDocumento(int codigoFaturaDocumento)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FaturaDocumentoAcrescimoDesconto obj WHERE obj.FaturaDocumento.Codigo = :codigoFaturaDocumento")
                                     .SetInt32("codigoFaturaDocumento", codigoFaturaDocumento)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FaturaDocumentoAcrescimoDesconto obj WHERE obj.FaturaDocumento.Codigo = :codigoFaturaDocumento")
                                    .SetInt32("codigoFaturaDocumento", codigoFaturaDocumento)
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
