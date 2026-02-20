using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class Carregamento : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>
    {
        #region Construtores

        public Carregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Carregamento(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public void ExcluirCarregamentos(List<int> carregamentos, bool excluirMontagensSimulacoesFrete)
        {
            {
                try
                {
                    if (UnitOfWork.IsActiveTransaction())
                    {
                        List<int> codigosMontagemCarregamentoBloco = new List<int>();

                        if (excluirMontagensSimulacoesFrete)
                            codigosMontagemCarregamentoBloco = this.ObterCodigosMontagemCarregamentoBloco(carregamentos);

                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_PEDIDO_CARREGAMENTO_NOTAS_FISCAIS WHERE CPN_CODIGO IN (SELECT CPN_CODIGO FROM T_CARREGAMENTO_PEDIDO_NOTA_FISCAL CarregamentoPedidoNotaFiscal JOIN T_CARREGAMENTO_PEDIDO CarregamentoPedido ON CarregamentoPedido.CRP_CODIGO = CarregamentoPedidoNotaFiscal.CRP_CODIGO WHERE CarregamentoPedido.CRG_CODIGO IN ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_PEDIDO_NOTA_FISCAL WHERE CRP_CODIGO IN(SELECT CarregamentoPedido.CRP_CODIGO FROM T_CARREGAMENTO_PEDIDO CarregamentoPedido WHERE CarregamentoPedido.CRG_CODIGO IN ( :codigoCarregamento )); ").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoPedidoProduto WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoPedidoProduto c WHERE c.CarregamentoPedido.Carregamento.Codigo in ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoPedido WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoPedido c WHERE c.Carregamento.Codigo in ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoRoteirizacaoClientesRota WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoRoteirizacaoClientesRota c WHERE c.CarregamentoRoteirizacao.Carregamento.Codigo in ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoRoteirizacaoPontosPassagem WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoRoteirizacaoPontosPassagem c WHERE c.CarregamentoRoteirizacao.Carregamento.Codigo in ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoRoteirizacao WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoRoteirizacao c WHERE c.Carregamento.Codigo in ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM BlocoCarregamento WHERE Codigo IN (SELECT c.Codigo FROM BlocoCarregamento c WHERE c.Carregamento.Codigo in ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM SimulacaoFrete WHERE Codigo IN (SELECT c.Codigo FROM SimulacaoFrete c WHERE c.Carregamento.Codigo in ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM AprovacaoAlcadaCarregamento WHERE Codigo IN (SELECT a.Codigo FROM AprovacaoAlcadaCarregamento a WHERE a.OrigemAprovacao.Carregamento.Codigo in ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoSolicitacao WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoSolicitacao c WHERE c.Carregamento.Codigo in ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_MOTORISTAS WHERE CRG_CODIGO in ( :codigos )").SetParameterList("codigos", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_REBOQUES WHERE CRG_CODIGO in ( :codigos )").SetParameterList("codigos", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_INTEGRACAO_ARQUIVO WHERE CIN_CODIGO in (SELECT CIN_CODIGO FROM T_CARREGAMENTO_INTEGRACAO WHERE CRG_CODIGO in ( :codigos ))").SetParameterList("codigos", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_INTEGRACAO WHERE CRG_CODIGO in ( :codigos )").SetParameterList("codigos", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_LOTE_INTEGRACAO_CARREGAMENTO_CARREGAMENTOS WHERE CRG_CODIGO in ( :codigos )").SetParameterList("codigos", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_FRONTEIRA WHERE CRG_CODIGO in ( :codigos )").SetParameterList("codigos", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_FILIAL WHERE CRG_CODIGO in ( :codigos )").SetParameterList("codigos", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_APOLICE WHERE CRG_CODIGO in ( :codigos )").SetParameterList("codigos", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("UPDATE T_MONTAGEM_CARREGAMENTO_BLOCO SET CRG_CODIGO = NULL WHERE CRG_CODIGO in ( :codigos )").SetParameterList("codigos", carregamentos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM Carregamento WHERE Codigo in ( :codigoCarregamento )").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();

                        // Simulações de frete Mattel..
                        this.ExcluirDadosSimuladorFreteCarregamento(codigosMontagemCarregamentoBloco);
                    }
                    else
                    {
                        try
                        {
                            UnitOfWork.Start();

                            List<int> codigosMontagemCarregamentoBloco = new List<int>();
                            if (excluirMontagensSimulacoesFrete)
                                codigosMontagemCarregamentoBloco = this.ObterCodigosMontagemCarregamentoBloco(carregamentos);

                            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_PEDIDO_CARREGAMENTO_NOTAS_FISCAIS WHERE CPN_CODIGO IN (SELECT CPN_CODIGO FROM T_CARREGAMENTO_PEDIDO_NOTA_FISCAL CarregamentoPedidoNotaFiscal JOIN T_CARREGAMENTO_PEDIDO CarregamentoPedido ON CarregamentoPedido.CRP_CODIGO = CarregamentoPedidoNotaFiscal.CRP_CODIGO WHERE CarregamentoPedido.CRG_CODIGO IN ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_PEDIDO_NOTA_FISCAL WHERE CRP_CODIGO IN(SELECT CarregamentoPedido.CRP_CODIGO FROM T_CARREGAMENTO_PEDIDO CarregamentoPedido WHERE CarregamentoPedido.CRG_CODIGO IN ( :codigoCarregamento )); ").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoPedidoProduto WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoPedidoProduto c WHERE c.CarregamentoPedido.Carregamento.Codigo in ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoPedido WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoPedido c WHERE c.Carregamento.Codigo in ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoRoteirizacaoClientesRota WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoRoteirizacaoClientesRota c WHERE c.CarregamentoRoteirizacao.Carregamento.Codigo in ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoRoteirizacaoPontosPassagem WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoRoteirizacaoPontosPassagem c WHERE c.CarregamentoRoteirizacao.Carregamento.Codigo in ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoRoteirizacao WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoRoteirizacao c WHERE c.Carregamento.Codigo in ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM BlocoCarregamento WHERE Codigo IN (SELECT c.Codigo FROM BlocoCarregamento c WHERE c.Carregamento.Codigo in ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM SimulacaoFrete WHERE Codigo IN (SELECT c.Codigo FROM SimulacaoFrete c WHERE c.Carregamento.Codigo in ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM AprovacaoAlcadaCarregamento WHERE Codigo IN (SELECT a.Codigo FROM AprovacaoAlcadaCarregamento a WHERE a.OrigemAprovacao.Carregamento.Codigo in ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoSolicitacao WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoSolicitacao c WHERE c.Carregamento.Codigo in ( :codigoCarregamento ))").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_MOTORISTAS WHERE CRG_CODIGO in ( :codigos )").SetParameterList("codigos", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_REBOQUES WHERE CRG_CODIGO in ( :codigos )").SetParameterList("codigos", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_INTEGRACAO_ARQUIVO WHERE CIN_CODIGO in (SELECT CIN_CODIGO FROM T_CARREGAMENTO_INTEGRACAO WHERE CRG_CODIGO in ( :codigos ))").SetParameterList("codigos", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_INTEGRACAO WHERE CRG_CODIGO in ( :codigos )").SetParameterList("codigos", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_LOTE_INTEGRACAO_CARREGAMENTO_CARREGAMENTOS WHERE CRG_CODIGO in ( :codigos )").SetParameterList("codigos", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_FRONTEIRA WHERE CRG_CODIGO in ( :codigos )").SetParameterList("codigos", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_FILIAL WHERE CRG_CODIGO in ( :codigos )").SetParameterList("codigos", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_APOLICE WHERE CRG_CODIGO in ( :codigos )").SetParameterList("codigos", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateSQLQuery("UPDATE T_MONTAGEM_CARREGAMENTO_BLOCO SET CRG_CODIGO = NULL WHERE CRG_CODIGO in ( :codigos )").SetParameterList("codigos", carregamentos).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM Carregamento WHERE Codigo in ( :codigoCarregamento )").SetParameterList("codigoCarregamento", carregamentos).ExecuteUpdate();
                            // Simulações de frete Mattel..
                            this.ExcluirDadosSimuladorFreteCarregamento(codigosMontagemCarregamentoBloco);

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

        public void ExcluirCarregamento(int codigoCarregamento, bool excluirMontagensSimulacoesFrete)
        {
            {
                try
                {
                    if (UnitOfWork.IsActiveTransaction())
                    {
                        List<int> codigosMontagemCarregamentoBloco = new List<int>();
                        if (excluirMontagensSimulacoesFrete)
                            codigosMontagemCarregamentoBloco = this.ObterCodigosMontagemCarregamentoBloco(new List<int>() { codigoCarregamento });

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoPedidoProduto WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoPedidoProduto c WHERE c.CarregamentoPedido.Carregamento.Codigo = :codigoCarregamento )").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoPedido WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoPedido c WHERE c.Carregamento.Codigo = :codigoCarregamento)").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoRoteirizacaoClientesRota WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoRoteirizacaoClientesRota c WHERE c.CarregamentoRoteirizacao.Carregamento.Codigo = :codigoCarregamento)").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoRoteirizacaoPontosPassagem WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoRoteirizacaoPontosPassagem c WHERE c.CarregamentoRoteirizacao.Carregamento.Codigo = :codigoCarregamento)").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoRoteirizacao WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoRoteirizacao c WHERE c.Carregamento.Codigo = :codigoCarregamento)").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM BlocoCarregamento WHERE Codigo IN (SELECT c.Codigo FROM BlocoCarregamento c WHERE c.Carregamento.Codigo = :codigoCarregamento)").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM SimulacaoFrete WHERE Codigo IN (SELECT c.Codigo FROM SimulacaoFrete c WHERE c.Carregamento.Codigo = :codigoCarregamento )").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM AprovacaoAlcadaCarregamento WHERE Codigo IN (SELECT a.Codigo FROM AprovacaoAlcadaCarregamento a WHERE a.OrigemAprovacao.Carregamento.Codigo = :codigoCarregamento)").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoSolicitacao WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoSolicitacao c WHERE c.Carregamento.Codigo = :codigoCarregamento)").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_MOTORISTAS WHERE CRG_CODIGO = :codigo ").SetInt32("codigo", codigoCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_REBOQUES WHERE CRG_CODIGO = :codigo ").SetInt32("codigo", codigoCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_FRONTEIRA WHERE CRG_CODIGO = :codigo ").SetInt32("codigo", codigoCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_FILIAL WHERE CRG_CODIGO = :codigo ").SetInt32("codigo", codigoCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_LOTE_INTEGRACAO_CARREGAMENTO_CARREGAMENTOS WHERE CRG_CODIGO = :codigo ").SetInt32("codigo", codigoCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("UPDATE T_MONTAGEM_CARREGAMENTO_BLOCO SET CRG_CODIGO = NULL WHERE CRG_CODIGO = :codigo").SetInt32("codigo", codigoCarregamento).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM Carregamento WHERE Codigo = (:codigoCarregamento)").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();

                        this.ExcluirDadosSimuladorFreteCarregamento(codigosMontagemCarregamentoBloco);
                    }
                    else
                    {
                        try
                        {
                            UnitOfWork.Start();

                            List<int> codigosMontagemCarregamentoBloco = new List<int>();
                            if (excluirMontagensSimulacoesFrete)
                                codigosMontagemCarregamentoBloco = this.ObterCodigosMontagemCarregamentoBloco(new List<int>() { codigoCarregamento });

                            UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoPedidoProduto WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoPedidoProduto c WHERE c.CarregamentoPedido.Carregamento.Codigo = :codigoCarregamento )").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoPedido WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoPedido c WHERE c.Carregamento.Codigo = :codigoCarregamento)").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoRoteirizacaoClientesRota WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoRoteirizacaoClientesRota c WHERE c.CarregamentoRoteirizacao.Carregamento.Codigo = :codigoCarregamento)").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoRoteirizacaoPontosPassagem WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoRoteirizacaoPontosPassagem c WHERE c.CarregamentoRoteirizacao.Carregamento.Codigo = :codigoCarregamento)").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoRoteirizacao WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoRoteirizacao c WHERE c.Carregamento.Codigo = :codigoCarregamento)").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM BlocoCarregamento WHERE Codigo IN (SELECT c.Codigo FROM BlocoCarregamento c WHERE c.Carregamento.Codigo = :codigoCarregamento)").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM SimulacaoFrete WHERE Codigo IN (SELECT c.Codigo FROM SimulacaoFrete c WHERE c.Carregamento.Codigo = :codigoCarregamento )").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM AprovacaoAlcadaCarregamento WHERE Codigo IN (SELECT a.Codigo FROM AprovacaoAlcadaCarregamento a WHERE a.OrigemAprovacao.Carregamento.Codigo = :codigoCarregamento)").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoSolicitacao WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoSolicitacao c WHERE c.Carregamento.Codigo = :codigoCarregamento)").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_MOTORISTAS WHERE CRG_CODIGO = :codigo").SetInt32("codigo", codigoCarregamento).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_REBOQUES WHERE CRG_CODIGO = :codigo").SetInt32("codigo", codigoCarregamento).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_FRONTEIRA WHERE CRG_CODIGO = :codigo").SetInt32("codigo", codigoCarregamento).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_FILIAL WHERE CRG_CODIGO = :codigo").SetInt32("codigo", codigoCarregamento).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_LOTE_INTEGRACAO_CARREGAMENTO_CARREGAMENTOS WHERE CRG_CODIGO = :codigo ").SetInt32("codigo", codigoCarregamento).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateSQLQuery("UPDATE T_MONTAGEM_CARREGAMENTO_BLOCO SET CRG_CODIGO = NULL WHERE CRG_CODIGO = :codigo").SetInt32("codigo", codigoCarregamento).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM Carregamento WHERE Codigo = (:codigoCarregamento)").SetInt32("codigoCarregamento", codigoCarregamento).ExecuteUpdate();

                            this.ExcluirDadosSimuladorFreteCarregamento(codigosMontagemCarregamentoBloco);

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

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> BuscarCarregamentoAguardandoCalculoFrete(int limite)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Carga(UnitOfWork);
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
            var queryCancelamento = repCarga.ObterCargasEmCancelamento();

            //pai nosso, que estais no céu...
            query = query.Where(car =>
                car.TipoMontagemCarga == TipoMontagemCarga.AgruparCargas
                && car.CargasFrete.Count() > 0
                && car.CargasFrete.Count() == car.CargasFrete.Where(o =>
                    (o.SituacaoCarga == SituacaoCarga.CalculoFrete
                        || (
                            !o.ExigeNotaFiscalParaCalcularFrete
                            && o.SituacaoCarga == SituacaoCarga.AgTransportador
                        )
                    )
                    && o.CargaFechada
                    && o.CalculandoFrete
                    && !o.PendenteGerarCargaDistribuidor
                    && !queryCancelamento.Select(can => can.Carga.Codigo).Any(cac => cac == o.Codigo)
                ).Count()
            );

            return query.Take(limite).ToList();
        }

        //public int ContarCarregamentosPendentesIntegracao()
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
        //    var result = from obj in query select obj;
        //    result = result.Where(p => !p.CarregamentoIntegradoERP && p.SituacaoCarregamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Fechado);
        //    return result.Count();
        //}

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento BuscarPorCodigoComFetch(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>()
                .Where(o => o.Codigo == codigo);

            return query
                .Fetch(obj => obj.UsuarioAgendamento)
                .Fetch(obj => obj.Empresa)
                .Fetch(obj => obj.ModeloVeicularCarga)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento BuscarPorCTe(int codigoCTe)
        {
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(c => c.CTe.Codigo == codigoCTe);

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(c => queryCargaCTe.Any(e => e.Carga == c.Carga));

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();
            query = query.Where(c => queryCargaPedido.Any(p => p.Pedido == c.Pedido));

            query = query.Where(c => c.Carregamento.SituacaoCarregamento != SituacaoCarregamento.Cancelado);

            return query.Select(c => c.Carregamento)?.FirstOrDefault() ?? null;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> BuscarPorNumeroCargaEmbarcador(List<string> codigosCargaEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
            var result = from obj in query where obj.Cargas.Any(x => codigosCargaEmbarcador.Contains(x.Carga.CodigoCargaEmbarcador)) select obj;
            return result.ToList();
        }

        public int BuscarQuantidadeCarregamentoDoDia(DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
            var result = from obj in query select obj;
            result = result.Where(p => p.DataCriacao.Date == data.Date);
            return result.Count() + 1;
        }

        public List<int> NaoPermitirCancelarCarregamentos(List<int> codigos, List<SituacaoCarregamento> situacoesPermitidasCliente)
        {
            List<SituacaoCarregamento> situacoesPermitemCancelamento = SituacaoCarregamentoHelper.ObterSituacoesCarregamentoPendente();
            if (situacoesPermitidasCliente.Count > 0)
                situacoesPermitemCancelamento.AddRange(situacoesPermitidasCliente);

            var consultaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>()
                .Where(o =>
                    !situacoesPermitemCancelamento.Contains(o.SituacaoCarregamento) &&
                    codigos.Contains(o.Codigo)
                );

            return consultaCarregamento.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> BuscarPorPedido(int pedido)
        {
            return BuscarPorPedidos(new List<int>() { pedido });
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> BuscarPorPedidos(List<int> pedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Pedidos.Any(o => pedidos.Contains(o.Pedido.Codigo)));
            return result.ToList();
        }

        public List<int> BuscarCodigosCarregamentosPendenteIntegracao(int inicio, int limite, bool retornarApenasComTransportadora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>()
                .Where(a => !a.CarregamentoIntegradoERP && a.SituacaoCarregamento == SituacaoCarregamento.EmMontagem);

            if (retornarApenasComTransportadora)
                query = query.Where(x => x.Empresa != null);
            return
                query.Select(a => a.Codigo)
                .Skip(inicio)
                .Take(limite)
                .ToList();
        }

        public int ContarCarregamentosPendenteIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
            var result = from obj in query where !obj.CarregamentoIntegradoERP && obj.SituacaoCarregamento == SituacaoCarregamento.EmMontagem select obj;
            return
                result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> BuscarPorCodigos(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();

            query = query
                .Where(p => codigos.Contains(p.Codigo))
                .Fetch(obj => obj.TipoOperacao)
                    .ThenFetch(obj => obj.ConfiguracaoCarga);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> BuscarCodigos(List<int> carregamentos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();

            query = query.Where(p => carregamentos.Contains(p.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> CarregamentosSessaoRoteirizador(int codigoSessaoRoteirizador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento situacao = SituacaoCarregamento.EmMontagem)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
            query = query.Where(x => x.SituacaoCarregamento == situacao && x.SessaoRoteirizador.Codigo == codigoSessaoRoteirizador);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> CarregamentosSessaoRoteirizador(int codigoSessaoRoteirizador, bool carregamentoColeta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
            query = query.Where(x => x.SessaoRoteirizador.Codigo == codigoSessaoRoteirizador && x.SituacaoCarregamento != SituacaoCarregamento.Cancelado && x.CarregamentoColeta == carregamentoColeta);
            return query.ToList();
        }

        public List<int> BuscarCodigosPorPedidos(List<int> pedidos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();

            query = query.Where(p => p.SituacaoCarregamento != SituacaoCarregamento.Cancelado && p.Pedidos.Any(o => pedidos.Contains(o.Pedido.Codigo)));

            return query.Select(o => o.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento BuscarPorPedido(int codigoPedido, bool redespacho)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
            var result = from obj in query where obj.CarregamentoRedespacho == redespacho select obj;
            result = result.Where(p => p.Pedidos.Any(obj => obj.Pedido.Codigo == codigoPedido && (obj.Pedido.PedidoTotalmenteCarregado || p.SituacaoCarregamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.EmMontagem)));
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Cargas.Any(obj => obj.Carga.Codigo == codigoCarga));
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento BuscarPorPreCarga(int precarga, int carregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
            var result = from obj in query where obj.PreCarga.Codigo == precarga select obj;

            if (carregamento > 0)
                result = result.Where(o => o.Codigo != carregamento);

            return result.FirstOrDefault();
        }

        public int BuscarQuantidadePorDataFilialeModeloVeicular(DateTime datainicial, DateTime dataFinal, int filial, int modeloVeicular)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
            var result = from obj in query
                         where obj.SituacaoCarregamento != SituacaoCarregamento.Cancelado &&
                               obj.Pedidos.Any(p => p.Pedido.Filial.Codigo == filial) &&
                                                    obj.DataCarregamentoCarga >= datainicial &&
                                                    obj.DataCarregamentoCarga < dataFinal &&
                                                    obj.ModeloVeicularCarga.Codigo == modeloVeicular
                         select obj.Codigo;


            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento BuscarPorMontagemCarregamentoBloco(int codigoMontagemCarregamentoBloco)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
            query = query.Where(x => x.MontagemCarregamentoBloco.Codigo == codigoMontagemCarregamentoBloco);
            var result = from obj in query select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento BuscarPorNumeroCarregamento(string numeroCarregamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>()
                .Where(o => o.NumeroCarregamento == numeroCarregamento);

            return query.FirstOrDefault();
        }

        /// <summary>
        ///  Importante: Utilizar método ObterProximoCodigoCarregamento da montagem de carga.
        /// </summary>
        /// <returns></returns>
        public int BuscarProximoCodigo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();

            int? retorno = query.Max(o => (int?)o.AutoSequenciaNumero);

            return retorno.HasValue ? retorno.Value + 1 : 1;
        }

        public List<int> ConsultarCodigos(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtrosPesquisa)
        {
            var consultaCarregamento = MontarQuery(filtrosPesquisa);

            return consultaCarregamento.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCarregamento = MontarQuery(filtrosPesquisa);

            consultaCarregamento.Fetch(x => x.Empresa)
                                .Fetch(x => x.PedidoViagemNavio)
                                .Fetch(x => x.Veiculo)
                                .Fetch(x => x.TipoOperacao)
                                .Fetch(x => x.Fronteiras)
                                .Fetch(x => x.TipoDeCarga)
                                .Fetch(x => x.ModeloVeicularCarga)
                                .Fetch(x => x.PreCarga)
                                .Fetch(x => x.Recebedor)
                                .Fetch(x => x.Rota)
                                .Fetch(x => x.TipoSeparacao)
                                .Fetch(x => x.TipoOperacao)
                                .ThenFetch(x => x.ConfiguracaoMobile);

            consultaCarregamento = consultaCarregamento.WithOptions(opcoes => opcoes.SetTimeout(120));

            return ObterLista(consultaCarregamento, parametrosConsulta);
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>> ConsultarAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCarregamento = MontarQuery(filtrosPesquisa);

            consultaCarregamento = consultaCarregamento
                                .Fetch(x => x.Empresa)
                                .Fetch(x => x.TipoDeCarga)
                                .Fetch(x => x.ModeloVeicularCarga)
                                .Fetch(x => x.TipoOperacao).ThenFetch(x => x.ConfiguracaoMobile)
                                .Fetch(x => x.TipoOperacao).ThenFetch(x => x.ConfiguracaoMontagemCarga)
                                .Fetch(x => x.TipoOperacao).ThenFetch(x => x.ConfiguracaoCarga)
                                .WithOptions(opcoes => opcoes.SetTimeout(120));

            return ObterListaAsync(consultaCarregamento, parametrosConsulta);
        }

        public Task<int> ContarConsultaAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtrosPesquisa)
        {
            var consultaCarregamento = MontarQuery(filtrosPesquisa);

            return consultaCarregamento.CountAsync(CancellationToken);
        }

        public bool ValidarSeCargasJaPossuemCarregamentos(List<int> codigosCargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
            var result = from obj in query where obj.Cargas.Any(c => codigosCargas.Contains(c.Carga.Codigo)) select obj;

            return result.Count() == 0;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> ConsultarCarregamentosLoteintegracao(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
            var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.DataInicio.HasValue && filtrosPesquisa.DataInicio.Value != DateTime.MinValue)
                result = result.Where(obj => obj.DataCarregamentoCarga >= filtrosPesquisa.DataInicio.Value);

            if (filtrosPesquisa.DataFim.HasValue && filtrosPesquisa.DataFim.Value != DateTime.MinValue)
                result = result.Where(obj => obj.DataCarregamentoCarga <= filtrosPesquisa.DataFim.Value);

            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroCarregamento))
                result = result.Where(obj => obj.NumeroCarregamento == filtrosPesquisa.NumeroCarregamento);

            if (filtrosPesquisa.CpfCnpjRemetente > 0)
                result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Remetente.CPF_CNPJ == filtrosPesquisa.CpfCnpjRemetente));

            if (filtrosPesquisa.CpfCnpjRecebedor > 0)
                result = result.Where(obj => obj.Recebedor.CPF_CNPJ == filtrosPesquisa.CpfCnpjRecebedor);

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoCargaEmbarcador))
                result = result.Where(obj => queryCarga.Where(c => c.SituacaoCarga != SituacaoCarga.Cancelada && c.SituacaoCarga != SituacaoCarga.Anulada && c.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador).Any(p => p.Carregamento.Codigo == obj.Codigo));

            result = result.Where(obj => obj.LoteIntegracaoCarregamento == null && obj.SituacaoCarregamento != SituacaoCarregamento.Cancelado);

            return ObterLista(result, parametrosConsulta);

        }

        public int ContarConsultaCarregamentoLoteIntegracao(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
            var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.DataInicio.HasValue && filtrosPesquisa.DataInicio.Value != DateTime.MinValue)
                result = result.Where(obj => obj.DataCarregamentoCarga >= filtrosPesquisa.DataInicio.Value);

            if (filtrosPesquisa.DataFim.HasValue && filtrosPesquisa.DataFim.Value != DateTime.MinValue)
                result = result.Where(obj => obj.DataCarregamentoCarga <= filtrosPesquisa.DataFim.Value);

            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroCarregamento))
                result = result.Where(obj => obj.NumeroCarregamento == filtrosPesquisa.NumeroCarregamento);

            if (filtrosPesquisa.CpfCnpjRemetente > 0)
                result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Remetente.CPF_CNPJ == filtrosPesquisa.CpfCnpjRemetente));

            if (filtrosPesquisa.CpfCnpjRecebedor > 0)
                result = result.Where(obj => obj.Recebedor.CPF_CNPJ == filtrosPesquisa.CpfCnpjRecebedor);

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoCargaEmbarcador))
                result = result.Where(obj => queryCarga.Where(c => c.SituacaoCarga != SituacaoCarga.Cancelada && c.SituacaoCarga != SituacaoCarga.Anulada && c.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador).Any(p => p.Carregamento.Codigo == obj.Codigo));

            result = result.Where(obj => obj.LoteIntegracaoCarregamento == null && obj.SituacaoCarregamento != SituacaoCarregamento.Cancelado);

            return result.Count();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.RetiradaProduto> ConsultarRetiradaProduto(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = ConsultarProduto(filtrosPesquisa, somenteContarNumeroRegistros: false, parametrosConsulta).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.RetiradaProduto)));

            return consulta.SetTimeout(120).List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.RetiradaProduto>();
        }

        public int ContarConsultaRetiradaProduto(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtrosPesquisa)
        {
            var consultaRetiradaProduto = ConsultarProduto(filtrosPesquisa, somenteContarNumeroRegistros: true).CriarSQLQuery(this.SessionNHiBernate);

            return consultaRetiradaProduto.SetTimeout(600).UniqueResult<int>();
        }

        #endregion

        #region Métodos Privados

        private SQLDinamico ConsultarProduto(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtrosPesquisa, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = null)
        {
            var parametros = new List<ParametroSQL>();

            string sql;

            if (somenteContarNumeroRegistros)
                sql = @"select distinct(count(0)) ";

            else
                sql = @"SELECT carregamento.CRG_CODIGO Codigo,
		                carregamento.CRG_SITUACAO SituacaoCarregamento,
		                carregamento.CRG_NUMERO_CARREGAMENTO NumeroCarregamento,
		                carregamento.CRG_DATA_CARREGAMENTO DataCarregamentoCarga,
		                carregamento.CRG_OBSERVACAO ObservacaoCarregamento,
                        carregamento.CRG_PLACA_VEICULO PlacaVeiculo,

		                SUBSTRING((SELECT DISTINCT ', ' + filial.FIL_DESCRICAO
                                                    FROM T_FILIAL filial    
                                                    join T_CARGA carga on carga.FIL_CODIGO = filial.FIL_CODIGO
                                                    WHERE carga.CRG_CODIGO = carregamento.CRG_CODIGO FOR XML PATH('')), 3, 1000) Filial,

		                SUBSTRING((SELECT DISTINCT ', ' + pedido.PED_NUMERO_PEDIDO_EMBARCADOR
                                                    FROM T_PEDIDO pedido    
                                                    join T_CARREGAMENTO_PEDIDO carregamentoPedido on carregamentoPedido.PED_CODIGO = pedido.PED_CODIGO
                                                    WHERE carregamentoPedido.CRG_CODIGO = carregamento.CRG_CODIGO  FOR XML PATH('')), 3, 1000) Pedido,

		                SUBSTRING((SELECT DISTINCT ', ' + dadosSumarizado.CDS_DESTINATARIOS
                                                    FROM T_CARGA_DADOS_SUMARIZADOS dadosSumarizado    
                                                    join T_CARGA carga on carga.CDS_CODIGO = dadosSumarizado.CDS_CODIGO
                                                    WHERE carga.CRG_CODIGO = carregamento.CRG_CODIGO FOR XML PATH('')), 3, 1000) Cliente,

		                SUBSTRING((SELECT DISTINCT ', ' + pedido.PED_CODIGO_PEDIDO_CLIENTE
                                                    FROM T_PEDIDO pedido    
                                                    join T_CARREGAMENTO_PEDIDO carregamentoPedido on carregamentoPedido.PED_CODIGO = pedido.PED_CODIGO
                                                    WHERE carregamentoPedido.CRG_CODIGO = carregamento.CRG_CODIGO FOR XML PATH('')), 3, 1000) CodigoPedidoCliente,
		
		                SUBSTRING((SELECT DISTINCT ', ' + dadosSumarizado.CDS_NUMERO_ORDEM
                                                    FROM T_CARGA_DADOS_SUMARIZADOS dadosSumarizado    
                                                    join T_CARGA carga on carga.CDS_CODIGO = dadosSumarizado.CDS_CODIGO
                                                    WHERE carga.CRG_CODIGO = carregamento.CRG_CODIGO FOR XML PATH('')), 3, 1000) OrdemCompraCliente,

		                SUBSTRING((SELECT DISTINCT ', ' + (motorista.FUN_NOME + ' (' + motorista.FUN_CPF + ')') 
                                                     FROM T_CARREGAMENTO_MOTORISTAS motoristaCarregamento 
                                                     INNER JOIN T_FUNCIONARIO motorista ON motoristaCarregamento.FUN_CODIGO = motorista.FUN_CODIGO 
                                                     WHERE motoristaCarregamento.CRG_CODIGO = carregamento.CRG_CODIGO FOR XML PATH('')), 3, 1000) Motorista,

	                    ISNULL(carregamento.CRG_NOME_TRANSPORTADORA, SUBSTRING((SELECT DISTINCT ', ' + empresa.EMP_RAZAO
                                                    FROM T_EMPRESA empresa    
                                                    where empresa.EMP_CODIGO = carregamento.EMP_CODIGO
                                                    FOR XML PATH('')), 3, 1000)) NomeEmpresa ";

            sql += @" FROM T_CARREGAMENTO carregamento";

            sql += @" where carregamento.CRG_CARREGAMENTO_COLETA = 0 ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoEmbarcador))
            {
                sql += $"and carregamento.CRG_CODIGO in (select carregamentoPedido.CRG_CODIGO FROM T_PEDIDO pedido join T_CARREGAMENTO_PEDIDO carregamentoPedido on carregamentoPedido.PED_CODIGO = pedido.PED_CODIGO WHERE pedido.PED_NUMERO_PEDIDO_EMBARCADOR = :PEDIDO_PED_NUMERO_PEDIDO_EMBARCADOR) "; 
                parametros.Add(new ParametroSQL("PEDIDO_PED_NUMERO_PEDIDO_EMBARCADOR", filtrosPesquisa.NumeroPedidoEmbarcador));
            }

            if (filtrosPesquisa.CodigoFuncionarioVendedor > 0)
            {
                sql += $"and exists (select top 1 _pedido.FUN_CODIGO_VENDEDOR  FROM T_PEDIDO _pedido join T_CARREGAMENTO_PEDIDO _carregamentoPedido on _carregamentoPedido.PED_CODIGO = _pedido.PED_CODIGO WHERE _pedido.FUN_CODIGO_VENDEDOR = {filtrosPesquisa.CodigoFuncionarioVendedor} and carregamento.CRG_CODIGO = _carregamentoPedido.CRG_CODIGO) "; // SQL-INJECTION-SAFE
            }

            if ((filtrosPesquisa.CodigosFilial?.Count > 0) || (filtrosPesquisa.CodigosTipoOperacao?.Count > 0) || (filtrosPesquisa.CpfCnpjDestinatario > 0))
            {
                sql += $"and exists (select carregamentoPedido.CRG_CODIGO from T_CARREGAMENTO_PEDIDO carregamentoPedido join T_PEDIDO pedido on pedido.PED_CODIGO = carregamentoPedido.PED_CODIGO where carregamentoPedido.CRG_CODIGO = carregamento.CRG_CODIGO ";

                if (filtrosPesquisa.CodigosFilial?.Count > 0)
                    sql += $"and pedido.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}) ";

                if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                    sql += $"and pedido.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}) ";

                if (filtrosPesquisa.CpfCnpjDestinatario > 0)
                    sql += $"and pedido.CLI_CODIGO = {filtrosPesquisa.CpfCnpjDestinatario} ";

                sql += ") ";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarregamento))
                sql += $"and carregamento.CRG_NUMERO_CARREGAMENTO = '{filtrosPesquisa.NumeroCarregamento}' ";

            if (filtrosPesquisa.SituacoesCarregamento.Count > 0)
                sql += $"and carregamento.CRG_SITUACAO in ({string.Join(", ", filtrosPesquisa.SituacoesCarregamento.Select(o => (int)o))}) ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NomeTransportadora))
            {
                sql += $"and (carregamento.CRG_NOME_TRANSPORTADORA like :CRG_NOME_TRANSPORTADORA OR carregamento.CRG_CODIGO in (select carregamento.CRG_CODIGO from T_CARREGAMENTO carregamento left join T_EMPRESA empresa on empresa.EMP_CODIGO = carregamento.EMP_CODIGO where empresa.EMP_RAZAO like :CRG_NOME_TRANSPORTADORA)) "; 
                parametros.Add(new ParametroSQL("CRG_NOME_TRANSPORTADORA", $"%{filtrosPesquisa.NomeTransportadora}%"));
            }

            if (filtrosPesquisa.SituacaoAgendamento == 1)
                sql += $"and carregamento.CRG_DATA_CARREGAMENTO is not null and carregamento.CRG_SITUACAO = 2 ";

            if (filtrosPesquisa.SituacaoAgendamento == 2)
                sql += $"and carregamento.CRG_DATA_CARREGAMENTO is null and carregamento.CRG_SITUACAO = 2 ";

            if (filtrosPesquisa.TransportadoraMatriz > 0)
                sql += $"and carregamento.EMP_CODIGO = {filtrosPesquisa.TransportadoraMatriz}";

            if (filtrosPesquisa.GrupoPessoaDestinatario > 0)
                sql += $@"and carregamento.CRG_CODIGO in 
                       (select carregamentoPedido.CRG_CODIGO FROM T_PEDIDO pedido 
                        join T_CARREGAMENTO_PEDIDO carregamentoPedido on carregamentoPedido.PED_CODIGO = pedido.PED_CODIGO
                        left join T_CLIENTE clienteDestinatario on clienteDestinatario.CLI_CGCCPF = pedido.CLI_CODIGO
                        left join T_CLIENTE clienteRemetente on clienteRemetente.CLI_CGCCPF = pedido.CLI_CODIGO_REMETENTE
                        where (clienteRemetente.GRP_CODIGO = {filtrosPesquisa.GrupoPessoaDestinatario} OR clienteDestinatario.GRP_CODIGO = {filtrosPesquisa.GrupoPessoaDestinatario})) ";

            if (parametrosConsulta != null && !somenteContarNumeroRegistros)
            {
                sql += $" order by {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar}";

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql += $" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;";
            }

            return new SQLDinamico(sql,parametros);
        }

        private void ExcluirDadosSimuladorFreteCarregamento(List<int> codigosMontagemCarregamentoBloco)
        {
            if (codigosMontagemCarregamentoBloco?.Count > 0)
            {
                UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE_PEDIDO WHERE MSF_CODIGO IN (SELECT MSF_CODIGO FROM T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE MSF WHERE MSF.MCB_CODIGO IN ( :codigosMontagemCarregamentoBloco ) AND NOT  EXISTS (SELECT 1 FROM T_CARREGAMENTO CRG WHERE CRG.MCB_CODIGO = MSF.MCB_CODIGO));").SetParameterList("codigosMontagemCarregamentoBloco", codigosMontagemCarregamentoBloco).ExecuteUpdate();
                UnitOfWork.Sessao.CreateSQLQuery("DELETE MSF FROM T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE MSF WHERE MSF.MCB_CODIGO IN ( :codigosMontagemCarregamentoBloco ) AND NOT  EXISTS (SELECT 1 FROM T_CARREGAMENTO CRG WHERE CRG.MCB_CODIGO = MSF.MCB_CODIGO);").SetParameterList("codigosMontagemCarregamentoBloco", codigosMontagemCarregamentoBloco).ExecuteUpdate();
                UnitOfWork.Sessao.CreateSQLQuery("DELETE MBP FROM T_MONTAGEM_CARREGAMENTO_BLOCO_PEDIDO MBP WHERE MBP.MCB_CODIGO IN ( :codigosMontagemCarregamentoBloco ) AND NOT  EXISTS (SELECT 1 FROM T_CARREGAMENTO CRG WHERE CRG.MCB_CODIGO = MBP.MCB_CODIGO);").SetParameterList("codigosMontagemCarregamentoBloco", codigosMontagemCarregamentoBloco).ExecuteUpdate();
                UnitOfWork.Sessao.CreateSQLQuery("DELETE MCB FROM T_MONTAGEM_CARREGAMENTO_BLOCO MCB WHERE MCB.MCB_CODIGO IN ( :codigosMontagemCarregamentoBloco ) AND NOT  EXISTS (SELECT 1 FROM T_CARREGAMENTO CRG WHERE CRG.MCB_CODIGO = MCB.MCB_CODIGO);").SetParameterList("codigosMontagemCarregamentoBloco", codigosMontagemCarregamentoBloco).ExecuteUpdate();
            }
        }

        private List<int> ObterCodigosMontagemCarregamentoBloco(List<int> carregamentos)
        {
            var values = UnitOfWork.Sessao.CreateQuery("SELECT MontagemCarregamentoBloco.Codigo from Carregamento WHERE Codigo in ( :CRG_CODIGO ) AND MontagemCarregamentoBloco IS NOT NULL;").SetParameterList("CRG_CODIGO", carregamentos).List();
            List<int> codigosMontagemCarregamentoBloco = new List<int>();
            foreach (var item in values)
                if (item != null)
                    codigosMontagemCarregamentoBloco.Add(Convert.ToInt32(item));
            values = UnitOfWork.Sessao.CreateQuery("SELECT Codigo from MontagemCarregamentoBloco WHERE Carregamento.Codigo in ( :CRG_CODIGO );").SetParameterList("CRG_CODIGO", carregamentos).List();
            foreach (var item in values)
                if (item != null)
                    codigosMontagemCarregamentoBloco.Add(Convert.ToInt32(item));

            return codigosMontagemCarregamentoBloco;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> MontarQuery(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
            var result = from obj in query select obj;

            if (filtrosPesquisa.ProgramaComSessaoRoteirizador || filtrosPesquisa.CodigoSessaoRoteirizador > 0)
            {
                result = result.Where(x => x.SessaoRoteirizador.Codigo == filtrosPesquisa.CodigoSessaoRoteirizador);

                if (filtrosPesquisa.CpfCnpjDestinatario > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjDestinatario));

                if (filtrosPesquisa.NumeroPedido > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Numero == filtrosPesquisa.NumeroPedido));

                if (filtrosPesquisa.CodigosEmpresa?.Count > 0)
                    result = result.Where(obj => filtrosPesquisa.CodigosEmpresa.Contains(obj.Empresa.Codigo));

                if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                    result = result.Where(obj => filtrosPesquisa.CodigosTipoOperacao.Contains(obj.TipoOperacao.Codigo));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoEmbarcador))
                {
                    if (filtrosPesquisa.FiltrarPorParteDoNumero)
                        result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.NumeroPedidoEmbarcador.Contains(filtrosPesquisa.NumeroPedidoEmbarcador)));
                    else
                        result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.NumeroPedidoEmbarcador == filtrosPesquisa.NumeroPedidoEmbarcador));
                }

                if (filtrosPesquisa.CodigoProdutoEmbarcador > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Produtos.Any(prod => prod.Produto.Codigo == filtrosPesquisa.CodigoProdutoEmbarcador)));

                if (filtrosPesquisa.CodigosLinhaSeparacao?.Count > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Produtos.Any(prod => filtrosPesquisa.CodigosLinhaSeparacao.Contains(prod.Produto.LinhaSeparacao.Codigo))));

                if (filtrosPesquisa.CodigosGrupoProdutos?.Count > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Produtos.Any(prod => filtrosPesquisa.CodigosGrupoProdutos.Contains(prod.Produto.GrupoProduto.Codigo))));

                if (filtrosPesquisa.CodigosCategoriaClientes?.Count > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => filtrosPesquisa.CodigosCategoriaClientes.Contains(ped.Pedido.Destinatario.Categoria.Codigo)));

                if (filtrosPesquisa.CodigosCanalEntrega?.Count > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => filtrosPesquisa.CodigosCanalEntrega.Contains(ped.Pedido.CanalEntrega.Codigo)));

                if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroCarregamento))
                    result = result.Where(obj => obj.NumeroCarregamento == filtrosPesquisa.NumeroCarregamento);
            }
            else
            {
                if (filtrosPesquisa.DataInicio.HasValue)
                    result = result.Where(obj => obj.DataCarregamentoCarga >= filtrosPesquisa.DataInicio.Value);

                if (filtrosPesquisa.DataFim.HasValue)
                    result = result.Where(obj => obj.DataCarregamentoCarga <= filtrosPesquisa.DataFim.Value);

                if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroCarregamento))
                    result = result.Where(obj => obj.NumeroCarregamento == filtrosPesquisa.NumeroCarregamento);

                if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoCargaEmbarcador))
                    result = result.Where(obj => obj.Cargas.Any(car => car.Carga.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador));

                if (filtrosPesquisa.NumeroPedido > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Numero == filtrosPesquisa.NumeroPedido));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoEmbarcador))
                {
                    if (filtrosPesquisa.FiltrarPorParteDoNumero)
                        result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.NumeroPedidoEmbarcador.Contains(filtrosPesquisa.NumeroPedidoEmbarcador)));
                    else
                        result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.NumeroPedidoEmbarcador == filtrosPesquisa.NumeroPedidoEmbarcador));
                }

                if (filtrosPesquisa.CodigoProdutoEmbarcador > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Produtos.Any(prod => prod.Produto.Codigo == filtrosPesquisa.CodigoProdutoEmbarcador)));

                if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroBooking))
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.NumeroBooking.Like(filtrosPesquisa.NumeroBooking)));

                if (!string.IsNullOrEmpty(filtrosPesquisa.Ordem))
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Ordem.Equals(filtrosPesquisa.Ordem)));

                if (!string.IsNullOrEmpty(filtrosPesquisa.PortoSaida))
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.PortoSaida.Equals(filtrosPesquisa.PortoSaida)));

                if (!string.IsNullOrEmpty(filtrosPesquisa.Reserva))
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Reserva.Equals(filtrosPesquisa.Reserva)));

                if (filtrosPesquisa.SomenteComReserva)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Reserva != null && ped.Pedido.Reserva != ""));

                if (!string.IsNullOrEmpty(filtrosPesquisa.TipoEmbarque))
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.TipoEmbarque.Equals(filtrosPesquisa.TipoEmbarque)));

                if (filtrosPesquisa.CodigoPaisDestino > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Destino.Pais.Codigo == filtrosPesquisa.CodigoPaisDestino));

                if (filtrosPesquisa.CodigoPedidoViagemNavio > 0)
                    result = result.Where(obj => obj.PedidoViagemNavio.Codigo == filtrosPesquisa.CodigoPedidoViagemNavio);

                if (filtrosPesquisa.CpfCnpjRemetente > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Remetente.CPF_CNPJ == filtrosPesquisa.CpfCnpjRemetente));

                if (filtrosPesquisa.CpfCnpjDestinatario > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjDestinatario));

                if (filtrosPesquisa.CpfCnpjExpedidor > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Expedidor.CPF_CNPJ == filtrosPesquisa.CpfCnpjExpedidor));

                if (filtrosPesquisa.CpfCnpjRecebedor > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Recebedor.CPF_CNPJ == filtrosPesquisa.CpfCnpjRecebedor));

                if (filtrosPesquisa.NotaFiscal?.Count > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.NotasFiscais.Any(n => filtrosPesquisa.NotaFiscal.Contains(n.Codigo))));

                if (filtrosPesquisa.CodigoOrigem > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Origem.Codigo == filtrosPesquisa.CodigoOrigem));

                if (filtrosPesquisa.CodigoDestino > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Destino.Codigo == filtrosPesquisa.CodigoDestino));

                if (filtrosPesquisa.CodigosEmpresa?.Count > 0)
                    result = result.Where(obj => filtrosPesquisa.CodigosEmpresa.Contains(obj.Empresa.Codigo));

                if (filtrosPesquisa.TransportadoraMatriz > 0)
                    result = result.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.TransportadoraMatriz || obj.Empresa.Matriz.Any(e => e.Codigo == filtrosPesquisa.TransportadoraMatriz));

                if (filtrosPesquisa.CodigosFilial?.Count > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => filtrosPesquisa.CodigosFilial.Contains(ped.Pedido.Filial.Codigo)));

                if (filtrosPesquisa.CodigosFilialVenda?.Count > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => filtrosPesquisa.CodigosFilialVenda.Contains(ped.Pedido.FilialVenda.Codigo)));

                if (filtrosPesquisa.CodigosPedido?.Count > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => filtrosPesquisa.CodigosPedido.Contains(ped.Pedido.Codigo)));

                if (filtrosPesquisa.CodigosRegiaoDestino?.Count > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => filtrosPesquisa.CodigosRegiaoDestino.Contains(ped.Pedido.Destino.Regiao.Codigo)));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.DeliveryTerm))
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.DeliveryTerm == filtrosPesquisa.DeliveryTerm));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.IdAutorizacao))
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.IdAutorizacao == filtrosPesquisa.IdAutorizacao));

                if (filtrosPesquisa.DataInclusaoBookingInicial.HasValue)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.DataInclusaoBooking >= filtrosPesquisa.DataInclusaoBookingInicial.Value.Date));

                if (filtrosPesquisa.DataInclusaoBookingLimite.HasValue)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.DataInclusaoBooking <= filtrosPesquisa.DataInclusaoBookingLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay)));

                if (filtrosPesquisa.DataInclusaoPCPInicial.HasValue)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.DataInclusaoPCP >= filtrosPesquisa.DataInclusaoPCPInicial.Value.Date));

                if (filtrosPesquisa.DataInclusaoPCPLimite.HasValue)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.DataInclusaoPCP <= filtrosPesquisa.DataInclusaoPCPLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay)));

                if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => filtrosPesquisa.CodigosTipoCarga.Contains(ped.Pedido.TipoDeCarga.Codigo) || (filtrosPesquisa.CodigosTipoCarga.Contains(-1) && ped.Pedido.TipoDeCarga == null)));

                if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => filtrosPesquisa.CodigosTipoOperacao.Contains(ped.Pedido.TipoOperacao.Codigo) || (filtrosPesquisa.CodigosTipoOperacao.Contains(-1) && ped.Pedido.TipoOperacao == null)));

                if (filtrosPesquisa.GrupoPessoaDestinatario > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Destinatario.GrupoPessoas.Codigo == filtrosPesquisa.GrupoPessoaDestinatario));

                if (filtrosPesquisa.CodigoFuncionarioVendedor > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.FuncionarioVendedor.Codigo == filtrosPesquisa.CodigoFuncionarioVendedor));

                if (filtrosPesquisa.Transportador > 0)
                    result = result.Where(obj => obj.Pedidos.Any(p => p.Pedido.Empresa.Codigo == filtrosPesquisa.Transportador));

                if (filtrosPesquisa.ExigeAgendamento)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Destinatario.ExigeQueEntregasSejamAgendadas == filtrosPesquisa.ExigeAgendamento));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NomeTransportadora))
                    result = result.Where(obj => !(obj.NomeTransportadora == null || obj.NomeTransportadora == string.Empty) ? obj.NomeTransportadora.Contains(filtrosPesquisa.NomeTransportadora) : obj.Empresa.RazaoSocial.Contains(filtrosPesquisa.NomeTransportadora));
            }

            if (filtrosPesquisa.SituacoesCarregamento?.Count > 0)
                result = result.Where(obj => filtrosPesquisa.SituacoesCarregamento.Contains(obj.SituacaoCarregamento));

            if (filtrosPesquisa.TipoMontagemCarga != TipoMontagemCarga.Todos)
                result = result.Where(obj => obj.TipoMontagemCarga == filtrosPesquisa.TipoMontagemCarga);

            if (filtrosPesquisa.SituacaoAgendamento == 1)
                result = result.Where(obj => obj.DataCarregamentoCarga.HasValue && obj.SituacaoCarregamento == SituacaoCarregamento.Fechado); // Confirmado

            if (filtrosPesquisa.SituacaoAgendamento == 2)
                result = result.Where(obj => !obj.DataCarregamentoCarga.HasValue && obj.SituacaoCarregamento == SituacaoCarregamento.Fechado); // Pendente de agendamento

            result = result.Where(obj => obj.CarregamentoColeta == filtrosPesquisa.CarregamentosDeColeta);

            return result;
        }

        #endregion
    }
}
