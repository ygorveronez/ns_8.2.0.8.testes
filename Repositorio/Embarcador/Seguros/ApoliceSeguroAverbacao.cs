using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Seguros
{
    public class ApoliceSeguroAverbacao : RepositorioBase<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao>
    {
        #region Construtores

        public ApoliceSeguroAverbacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Púlicos

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> BuscarPorCodigo(int[] codigos)
        {
            var consultaApoliceSeguroAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao>()
                .Where(o => codigos.Contains(o.Codigo));

            return consultaApoliceSeguroAverbacao.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> BuscarPorCarga(int codigoCarga)
        {
            var consultaApoliceSeguroAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            return consultaApoliceSeguroAverbacao
                .Fetch(obj => obj.ApoliceSeguro)
                .ThenFetch(obj => obj.Seguradora)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> BuscarPorCargaSemFetch(int codigoCarga)
        {
            var consultaApoliceSeguroAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            return consultaApoliceSeguroAverbacao.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> BuscarPorCargas(List<int> codigoCargas)
        {
            var consultaApoliceSeguroAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao>()
                .Where(o => codigoCargas.Contains(o.CargaPedido.Carga.Codigo));

            return consultaApoliceSeguroAverbacao
                .Fetch(obj => obj.CargaPedido)
                .Fetch(obj => obj.ApoliceSeguro)
                .ThenFetch(obj => obj.Seguradora)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> BuscarPorCargaPedido(List<int> codigosCargaPedidos, bool emitirCteFilialEmissora)
        {
            var consultaApoliceSeguroAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao>()
                .Where(o => codigosCargaPedidos.Contains(o.CargaPedido.Codigo) && o.SeguroFilialEmissora == emitirCteFilialEmissora);

            return consultaApoliceSeguroAverbacao
                .Fetch(obj => obj.CargaPedido)
                .Fetch(obj => obj.ApoliceSeguro)
                .ThenFetch(obj => obj.Seguradora)
                .Distinct()
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> BuscarPorCargaPedido(List<int> codigosCargaPedidos)
        {
            var consultaApoliceSeguroAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao>()
                .Where(o => codigosCargaPedidos.Contains(o.CargaPedido.Codigo));

            return consultaApoliceSeguroAverbacao
                .Fetch(obj => obj.CargaPedido)
                .Fetch(obj => obj.ApoliceSeguro)
                .ThenFetch(obj => obj.Seguradora)
                .Distinct()
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> BuscarPorCargaPedido(int codigoCargaPedido, bool seguroFilialEmissora)
        {
            var consultaApoliceSeguroAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao>()
                .Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.SeguroFilialEmissora == seguroFilialEmissora);

            return consultaApoliceSeguroAverbacao
                .Fetch(obj => obj.CargaPedido)
                .Fetch(obj => obj.ApoliceSeguro)
                .ThenFetch(obj => obj.Seguradora)
                .ThenFetch(obj => obj.ClienteSeguradora)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            var consultaApoliceSeguroAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao>()
                .Where(o => o.CargaPedido.Codigo == codigoCargaPedido);

            return consultaApoliceSeguroAverbacao
                .Fetch(obj => obj.CargaPedido)
                .Fetch(obj => obj.ApoliceSeguro)
                .ThenFetch(obj => obj.Seguradora)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> BuscarApolicesPorCarga(int codigoCarga)
        {
            var consultaApoliceSeguroAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            return consultaApoliceSeguroAverbacao
                .Select(o => o.ApoliceSeguro)
                .Distinct()
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> BuscarApolicesPorCargaPedido(int codigoCargaPedido)
        {
            var consultaApoliceSeguroAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao>()
                .Where(o => o.CargaPedido.Codigo == codigoCargaPedido);

            return consultaApoliceSeguroAverbacao
                .Select(o => o.ApoliceSeguro)
                .ToList();
        }

        public int ContarAverbacoesCarga(int codigoCarga)
        {
            var consultaApoliceSeguroAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            return consultaApoliceSeguroAverbacao.Count();
        }

        public bool ExistePorCarga(int codigoCarga)
        {
            var consultaApoliceSeguroAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            return consultaApoliceSeguroAverbacao.Select(o => o.Codigo).Any();
        }

        public Task<bool> ExistePorCargaAsync(int codigoCarga)
        {
            var consultaApoliceSeguroAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            return consultaApoliceSeguroAverbacao.Select(o => o.Codigo).AnyAsync();
        }

        public bool ExistePorCargaEResponsavelSeguro(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro responsavelSeguro)
        {
            var consultaApoliceSeguroAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.ApoliceSeguro.Responsavel == responsavelSeguro);

            return consultaApoliceSeguroAverbacao.Select(o => o.Codigo).Any();
        }

        public void DeletarPorCarga(int codigoCarga)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_AVERBACAO_INTEGRACAO_ARQUIVO_ARQUIVO WHERE AVE_CODIGO in (select averbacaoCTe.AVE_CODIGO FROM T_CTE_AVERBACAO averbacaoCTe WHERE averbacaoCTe.cpa_codigo in  (SELECT apoliceSeguroAverbacao.CPA_CODIGO FROM T_CARGA_PEDIDO_APOLICE_SEGURO_AVERBACAO apoliceSeguroAverbacao WHERE apoliceSeguroAverbacao.CPE_CODIGO in  (SELECT cargaPedido.CPE_CODIGO FROM T_CARGA_PEDIDO cargaPedido WHERE cargaPedido.CAR_CODIGO  = {codigoCarga})));").ExecuteUpdate(); // SQL-INJECTION-SAFE
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_AVERBACAO_INTEGRACAO_ARQUIVO_CANCELAMENTO_ARQUIVO WHERE AVE_CODIGO in (select averbacaoCTe.AVE_CODIGO FROM T_CTE_AVERBACAO averbacaoCTe WHERE averbacaoCTe.cpa_codigo in  (SELECT apoliceSeguroAverbacao.CPA_CODIGO FROM T_CARGA_PEDIDO_APOLICE_SEGURO_AVERBACAO apoliceSeguroAverbacao WHERE apoliceSeguroAverbacao.CPE_CODIGO in  (SELECT cargaPedido.CPE_CODIGO FROM T_CARGA_PEDIDO cargaPedido WHERE cargaPedido.CAR_CODIGO  = {codigoCarga})));").ExecuteUpdate(); // SQL-INJECTION-SAFE
                    UnitOfWork.Sessao.CreateQuery("DELETE AverbacaoCTe obj WHERE obj.ApoliceSeguroAverbacao.Codigo in (SELECT apoliceSeguroAverbacao.Codigo FROM ApoliceSeguroAverbacao apoliceSeguroAverbacao WHERE apoliceSeguroAverbacao.CargaPedido.Codigo in (SELECT cargaPedido.Codigo FROM CargaPedido cargaPedido WHERE cargaPedido.Carga.Codigo = :codigoCarga))").SetInt32("codigoCarga", codigoCarga).ExecuteUpdate(); // SQL-INJECTION-SAFE
                    UnitOfWork.Sessao.CreateQuery("DELETE ApoliceSeguroAverbacao obj WHERE obj.CargaPedido.Codigo in (SELECT cargaPedido.Codigo FROM CargaPedido cargaPedido WHERE cargaPedido.Carga.Codigo = :codigoCarga)").SetInt32("codigoCarga", codigoCarga).ExecuteUpdate(); // SQL-INJECTION-SAFE
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_AVERBACAO_INTEGRACAO_ARQUIVO_ARQUIVO WHERE AVE_CODIGO in (select averbacaoCTe.AVE_CODIGO FROM T_CTE_AVERBACAO averbacaoCTe WHERE averbacaoCTe.cpa_codigo in  (SELECT apoliceSeguroAverbacao.CPA_CODIGO FROM T_CARGA_PEDIDO_APOLICE_SEGURO_AVERBACAO apoliceSeguroAverbacao WHERE apoliceSeguroAverbacao.CPE_CODIGO in  (SELECT cargaPedido.CPE_CODIGO FROM T_CARGA_PEDIDO cargaPedido WHERE cargaPedido.CAR_CODIGO  = {codigoCarga})));").ExecuteUpdate(); // SQL-INJECTION-SAFE
                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_AVERBACAO_INTEGRACAO_ARQUIVO_CANCELAMENTO_ARQUIVO WHERE AVE_CODIGO in (select averbacaoCTe.AVE_CODIGO FROM T_CTE_AVERBACAO averbacaoCTe WHERE averbacaoCTe.cpa_codigo in  (SELECT apoliceSeguroAverbacao.CPA_CODIGO FROM T_CARGA_PEDIDO_APOLICE_SEGURO_AVERBACAO apoliceSeguroAverbacao WHERE apoliceSeguroAverbacao.CPE_CODIGO in  (SELECT cargaPedido.CPE_CODIGO FROM T_CARGA_PEDIDO cargaPedido WHERE cargaPedido.CAR_CODIGO  = {codigoCarga})));").ExecuteUpdate(); // SQL-INJECTION-SAFE
                        UnitOfWork.Sessao.CreateQuery("DELETE AverbacaoCTe obj WHERE obj.ApoliceSeguroAverbacao.Codigo in (SELECT apoliceSeguroAverbacao.Codigo FROM ApoliceSeguroAverbacao apoliceSeguroAverbacao WHERE apoliceSeguroAverbacao.CargaPedido.Codigo in (SELECT cargaPedido.Codigo FROM CargaPedido cargaPedido WHERE cargaPedido.Carga.Codigo = :codigoCarga))").SetInt32("codigoCarga", codigoCarga).ExecuteUpdate(); // SQL-INJECTION-SAFE
                        UnitOfWork.Sessao.CreateQuery("DELETE ApoliceSeguroAverbacao obj WHERE obj.CargaPedido.Codigo in (SELECT cargaPedido.Codigo FROM CargaPedido cargaPedido WHERE cargaPedido.Carga.Codigo = :codigoCarga)").SetInt32("codigoCarga", codigoCarga).ExecuteUpdate(); // SQL-INJECTION-SAFE

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException excecao)
            {
                if (excecao.InnerException != null && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecao);
                }

                throw;
            }
        }

        public void DeletarPorCargaPedido(int codigoCargaPedido)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_AVERBACAO_INTEGRACAO_ARQUIVO_ARQUIVO WHERE AVE_CODIGO in (select averbacaoCTe.AVE_CODIGO FROM T_CTE_AVERBACAO averbacaoCTe WHERE averbacaoCTe.cpa_codigo in  (SELECT apoliceSeguroAverbacao.CPA_CODIGO FROM T_CARGA_PEDIDO_APOLICE_SEGURO_AVERBACAO apoliceSeguroAverbacao WHERE apoliceSeguroAverbacao.CPE_CODIGO in  (SELECT cargaPedido.CPE_CODIGO FROM T_CARGA_PEDIDO cargaPedido WHERE cargaPedido.CPE_CODIGO  = {codigoCargaPedido})));").ExecuteUpdate(); // SQL-INJECTION-SAFE
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_AVERBACAO_INTEGRACAO_ARQUIVO_CANCELAMENTO_ARQUIVO WHERE AVE_CODIGO in (select averbacaoCTe.AVE_CODIGO FROM T_CTE_AVERBACAO averbacaoCTe WHERE averbacaoCTe.cpa_codigo in  (SELECT apoliceSeguroAverbacao.CPA_CODIGO FROM T_CARGA_PEDIDO_APOLICE_SEGURO_AVERBACAO apoliceSeguroAverbacao WHERE apoliceSeguroAverbacao.CPE_CODIGO in  (SELECT cargaPedido.CPE_CODIGO FROM T_CARGA_PEDIDO cargaPedido WHERE cargaPedido.CPE_CODIGO  = {codigoCargaPedido})));").ExecuteUpdate(); // SQL-INJECTION-SAFE
                    UnitOfWork.Sessao.CreateQuery("DELETE AverbacaoCTe obj WHERE obj.ApoliceSeguroAverbacao.Codigo in (SELECT apoliceSeguroAverbacao.Codigo FROM ApoliceSeguroAverbacao apoliceSeguroAverbacao WHERE apoliceSeguroAverbacao.CargaPedido.Codigo in (SELECT cargaPedido.Codigo FROM CargaPedido cargaPedido WHERE cargaPedido.Codigo = :codigoCargaPedido))").SetInt32("codigoCargaPedido", codigoCargaPedido).ExecuteUpdate(); // SQL-INJECTION-SAFE
                    UnitOfWork.Sessao
                        .CreateQuery("DELETE ApoliceSeguroAverbacao obj WHERE obj.CargaPedido.Codigo = :CodigoCargaPedido")
                        .SetInt32("CodigoCargaPedido", codigoCargaPedido)
                        .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();
                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_AVERBACAO_INTEGRACAO_ARQUIVO_ARQUIVO WHERE AVE_CODIGO in (select averbacaoCTe.AVE_CODIGO FROM T_CTE_AVERBACAO averbacaoCTe WHERE averbacaoCTe.cpa_codigo in  (SELECT apoliceSeguroAverbacao.CPA_CODIGO FROM T_CARGA_PEDIDO_APOLICE_SEGURO_AVERBACAO apoliceSeguroAverbacao WHERE apoliceSeguroAverbacao.CPE_CODIGO in  (SELECT cargaPedido.CPE_CODIGO FROM T_CARGA_PEDIDO cargaPedido WHERE cargaPedido.CPE_CODIGO  = {codigoCargaPedido})));").ExecuteUpdate(); // SQL-INJECTION-SAFE
                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_AVERBACAO_INTEGRACAO_ARQUIVO_CANCELAMENTO_ARQUIVO WHERE AVE_CODIGO in (select averbacaoCTe.AVE_CODIGO FROM T_CTE_AVERBACAO averbacaoCTe WHERE averbacaoCTe.cpa_codigo in  (SELECT apoliceSeguroAverbacao.CPA_CODIGO FROM T_CARGA_PEDIDO_APOLICE_SEGURO_AVERBACAO apoliceSeguroAverbacao WHERE apoliceSeguroAverbacao.CPE_CODIGO in  (SELECT cargaPedido.CPE_CODIGO FROM T_CARGA_PEDIDO cargaPedido WHERE cargaPedido.CPE_CODIGO  = {codigoCargaPedido})));").ExecuteUpdate(); // SQL-INJECTION-SAFE
                        UnitOfWork.Sessao.CreateQuery("DELETE AverbacaoCTe obj WHERE obj.ApoliceSeguroAverbacao.Codigo in (SELECT apoliceSeguroAverbacao.Codigo FROM ApoliceSeguroAverbacao apoliceSeguroAverbacao WHERE apoliceSeguroAverbacao.CargaPedido.Codigo in (SELECT cargaPedido.Codigo FROM CargaPedido cargaPedido WHERE cargaPedido.Codigo = :codigoCargaPedido))").SetInt32("codigoCargaPedido", codigoCargaPedido).ExecuteUpdate(); // SQL-INJECTION-SAFE
                        UnitOfWork.Sessao
                            .CreateQuery("DELETE ApoliceSeguroAverbacao obj WHERE obj.CargaPedido.Codigo = :CodigoCargaPedido")
                            .SetInt32("CodigoCargaPedido", codigoCargaPedido)
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
            catch (NHibernate.Exceptions.GenericADOException excecao)
            {
                if (excecao.InnerException != null && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecao);
                }

                throw;
            }
        }

        #endregion
    }
}
