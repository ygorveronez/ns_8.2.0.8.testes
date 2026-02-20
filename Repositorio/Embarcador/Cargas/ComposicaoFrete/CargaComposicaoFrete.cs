using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.ComposicaoFrete
{
    public class CargaComposicaoFrete : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete>
    {
        public CargaComposicaoFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaComposicaoFrete(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete BuscarPorCodigoComponente(int componente, int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete>();
            var resut = from obj in query where obj.ComponenteFrete.Codigo == componente && obj.Carga.Codigo == carga select obj;
            return resut.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete> BuscarPorCodigoComponenteAsync(int componente, int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete>();
            var resut = from obj in query where obj.ComponenteFrete.Codigo == componente && obj.Carga.Codigo == carga select obj;
            return await resut.FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete> BuscarPorCarga(int codCarga, bool composicaoFreteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete>();
            var result = from obj in query where obj.Carga.Codigo == codCarga && obj.ComposicaoFreteFilialEmissora == composicaoFreteFilialEmissora select obj;
            return result.OrderBy(obj => obj.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete BuscarPorCargaETipoParametro(int codCarga, bool composicaoFreteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete tipoParametro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete>();
            var result = from obj in query where obj.Carga.Codigo == codCarga && obj.ComposicaoFreteFilialEmissora == composicaoFreteFilialEmissora && obj.TipoParametro == tipoParametro select obj;
            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete> BuscarPorCargaAgrupada(List<int> codCarga, bool composicaoFreteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete>();
            query = query.Where(obj => obj.ComposicaoFreteFilialEmissora == composicaoFreteFilialEmissora && codCarga.Contains(obj.Carga.Codigo));
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete> BuscarPorPreCarga(int codPreCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete>();
            var result = from obj in query where obj.PreCarga.Codigo == codPreCarga select obj;
            return result.OrderBy(obj => obj.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete> BuscarPorPedidoXMLNotaFiscal(int codigoPedidoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete>();
            query = query.Where(obj => obj.PedidoXMLNotasFiscais.Any(o => o.Codigo == codigoPedidoXMLNotaFiscal));
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete> BuscarPorPedidoCTeParaSubcontratacao(int codigoPedidoCTeParaSubcontratacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete>();
            query = query.Where(obj => obj.PedidoCTesParaSubContratacao.Any(o => o.Codigo == codigoPedidoCTeParaSubcontratacao));
            return query.ToList();
        }


        public void DeletarPorCarga(int codigoCarga, bool naoRemoverComponentes, int idCargaPedido = 0)
        {
            try
            {
                string whereComplemento = "";
                if (naoRemoverComponentes)
                    whereComplemento = " and CCF_TIPO_PARAMETRO <> 4 ";
                if (idCargaPedido > 0)
                    whereComplemento += $" or CPE_CODIGO = {idCargaPedido} ";

                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery(
                        $"DELETE FROM T_CARGA_COMPOSICAO_FRETE_PEDIDO_XML_NOTA_FISCAL WHERE CCF_CODIGO IN (SELECT c.CCF_CODIGO FROM T_CARGA_COMPOSICAO_FRETE c WHERE c.CAR_CODIGO = {codigoCarga} {whereComplemento});" + // SQL-INJECTION-SAFE
                        $"DELETE FROM T_CARGA_COMPOSICAO_FRETE_PEDIDO_CTE_PARA_SUBCONTRATACAO WHERE CCF_CODIGO IN (SELECT c.CCF_CODIGO FROM T_CARGA_COMPOSICAO_FRETE c WHERE c.CAR_CODIGO = {codigoCarga} {whereComplemento}); " + // SQL-INJECTION-SAFE
                        $"DELETE FROM T_CARGA_COMPOSICAO_FRETE_CARGA_PEDIDO WHERE CCF_CODIGO IN (SELECT c.CCF_CODIGO FROM T_CARGA_COMPOSICAO_FRETE c WHERE c.CAR_CODIGO = {codigoCarga} {whereComplemento});" + // SQL-INJECTION-SAFE
                        $"DELETE FROM T_CARGA_COMPOSICAO_FRETE_CARGA_PEDIDO WHERE CPE_CODIGO =  {idCargaPedido};" + // SQL-INJECTION-SAFE
                        $"DELETE FROM T_CARGA_COMPOSICAO_FRETE WHERE CAR_CODIGO = {codigoCarga} {whereComplemento};").ExecuteUpdate(); // SQL-INJECTION-SAFE
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery(
                        $"DELETE FROM T_CARGA_COMPOSICAO_FRETE_PEDIDO_XML_NOTA_FISCAL WHERE CCF_CODIGO IN (SELECT c.CCF_CODIGO FROM T_CARGA_COMPOSICAO_FRETE c WHERE c.CAR_CODIGO = {codigoCarga} {whereComplemento});" + // SQL-INJECTION-SAFE
                        $"DELETE FROM T_CARGA_COMPOSICAO_FRETE_PEDIDO_CTE_PARA_SUBCONTRATACAO WHERE CCF_CODIGO IN (SELECT c.CCF_CODIGO FROM T_CARGA_COMPOSICAO_FRETE c WHERE c.CAR_CODIGO = {codigoCarga} {whereComplemento}); " + // SQL-INJECTION-SAFE
                        $"DELETE FROM T_CARGA_COMPOSICAO_FRETE_CARGA_PEDIDO WHERE CCF_CODIGO IN (SELECT c.CCF_CODIGO FROM T_CARGA_COMPOSICAO_FRETE c WHERE c.CAR_CODIGO = {codigoCarga} {whereComplemento});" + // SQL-INJECTION-SAFE
                       $"DELETE FROM T_CARGA_COMPOSICAO_FRETE_CARGA_PEDIDO WHERE CPE_CODIGO =  {idCargaPedido};" + // SQL-INJECTION-SAFE
                        $"DELETE FROM T_CARGA_COMPOSICAO_FRETE WHERE CAR_CODIGO = {codigoCarga} {whereComplemento};").ExecuteUpdate(); // SQL-INJECTION-SAFE

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
