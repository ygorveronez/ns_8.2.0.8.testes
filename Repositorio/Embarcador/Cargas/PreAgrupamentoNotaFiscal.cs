using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public sealed class PreAgrupamentoNotaFiscal : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal>
    {
        #region Construtores

        public PreAgrupamentoNotaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal> BuscarPorAgrupador(int codigoAgrupador)
        {
            var consultaPreAgrupamentoNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal>()
                .Where(o => o.PreAgrupamentoCarga.Agrupador.Codigo == codigoAgrupador);

            return consultaPreAgrupamentoNotaFiscal.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal> BuscarPorCargaAgrupadores(List<int> codigosAgrupadorCarga)
        {
            var consultaPreAgrupamentoNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal>()
                .Where(o => codigosAgrupadorCarga.Contains(o.PreAgrupamentoCarga.Codigo));

            return consultaPreAgrupamentoNotaFiscal.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal> BuscarPorPreAgrupamentoCarga(int codigoPreAgrupamentoCarga)
        {
            var consultaPreAgrupamentoNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal>()
                .Where(o => o.PreAgrupamentoCarga.Codigo == codigoPreAgrupamentoCarga);

            return consultaPreAgrupamentoNotaFiscal.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal> BuscarPorNumeroNotaEmitente(string numero, string emitente)
        {
            var consultaPreAgrupamentoNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal>()
                .Where(o => 
                    (o.PreAgrupamentoCarga.Agrupador.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreAgrupamentoCarga.SemCarga ||
                    o.PreAgrupamentoCarga.Agrupador.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreAgrupamentoCarga.AguardandoCargasPararEncaixe)
                    && o.NumeroNota == numero &&
                    o.CnpjEmitente == emitente &&
                    o.PreAgrupamentoCarga.PedidoEncaixe
                );

            return consultaPreAgrupamentoNotaFiscal.ToList();
        }

        public void DeletarPorAgrupamento(int codigoAgrupamento)
        {
            try
            {

                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM PreAgrupamentoNotaFiscal preAgrupamentoNotaFiscal WHERE preAgrupamentoNotaFiscal.PreAgrupamentoCarga.Codigo IN (SELECT preAgrupamentoCarga.Codigo FROM PreAgrupamentoCarga preAgrupamentoCarga WHERE preAgrupamentoCarga.Agrupador.Codigo = :CodigoAgrupamento)").SetInt32("CodigoAgrupamento", codigoAgrupamento).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM PreAgrupamentoNotaFiscal preAgrupamentoNotaFiscal WHERE preAgrupamentoNotaFiscal.PreAgrupamentoCarga.Codigo IN (SELECT preAgrupamentoCarga.Codigo FROM PreAgrupamentoCarga preAgrupamentoCarga WHERE preAgrupamentoCarga.Agrupador.Codigo = :CodigoAgrupamento)").SetInt32("CodigoAgrupamento", codigoAgrupamento).ExecuteUpdate();
                            
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
