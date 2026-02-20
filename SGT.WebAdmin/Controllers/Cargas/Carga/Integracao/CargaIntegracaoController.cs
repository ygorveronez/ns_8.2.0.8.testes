using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
namespace SGT.WebAdmin.Controllers.Cargas.Carga.Integracao
{
    [CustomAuthorize(new string[] { "ObterDadosIntegracoes" }, "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaIntegracaoController : BaseController
    {
        #region Construtores

        public CargaIntegracaoController(Conexao conexao) : base(conexao) { }

        #endregion

        public async Task<IActionResult> ObterDadosIntegracoes(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                bool integracaoFilialEmissora = false;
                bool.TryParse(Request.Params("FilialEmissora"), out integracaoFilialEmissora);

                Repositorio.Embarcador.Cargas.CargaEDIIntegracao repositorioCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaCTeIntegracao repositorioCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork, cancellationToken);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesCTe = await repositorioCargaCTeIntegracao.BuscarTipoIntegracaoPorCargaAsync(codigoCarga, integracaoFilialEmissora);
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesEDI = await repositorioCargaEDIIntegracao.BuscarTipoIntegracaoPorCargaAsync(codigoCarga, integracaoFilialEmissora);
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesCarga = await repositorioCargaCargaIntegracao.BuscarTipoIntegracaoPorCargaAsync(codigoCarga, integracaoFilialEmissora);

                return new JsonpResult(new
                {
                    TiposIntegracoesCTe = tiposIntegracoesCTe,
                    TiposIntegracoesEDI = tiposIntegracoesEDI,
                    TiposIntegracoesCarga = tiposIntegracoesCarga
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados das integrações.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ConfirmarIntegracao))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unidadeDeTrabalho);
                Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaLog repLog = new Repositorio.Embarcador.Cargas.CargaLog(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                if (carga == null)
                    return new JsonpResult(true, false, "Carga não encontrada.");

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao)
                    return new JsonpResult(true, false, "A situação da carga não permite a finalização da etapa.");

                if (carga.CargaEmitidaParcialmente)
                    return new JsonpResult(false, false, "Não é possível a finalização da etapa com a carga emitida parcialmente.");

                if (carga.GerandoIntegracoes)
                    return new JsonpResult(false, false, "O sistema ainda está gerando as integrações, não sendo possível finalizar a etapa. Aguarde alguns minutos e tente novamente.");

                if (repCargaCancelamento.ExistePendentePorCarga(carga.Codigo))
                    return new JsonpResult(false, false, "Existe um cancelamento pendente ou finalizado para esta carga, não sendo possível prosseguir com esta ação.");

                unidadeDeTrabalho.Start();

                Dominio.Entidades.Embarcador.Cargas.CargaLog log = new Dominio.Entidades.Embarcador.Cargas.CargaLog();

                log.Acao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogCarga.FinalizarEtapaIntegracao;
                log.Carga = carga;
                log.Data = DateTime.Now;
                log.Usuario = Usuario;

                repLog.Inserir(log);

                bool integracaoFilialEmissora = false;
                if (carga.EmpresaFilialEmissora != null && !carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                    integracaoFilialEmissora = true;

                if (!integracaoFilialEmissora)
                {
                    if (carga.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GPAMDFe))
                    {
                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte;
                        carga.DataMudouSituacaoParaEmTransporte = DateTime.Now;
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, $"Alterou carga para situação {Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte.ObterDescricao()}", unidadeDeTrabalho);
                    }
                    else
                    {
                        carga.SituacaoCarga = ConfiguracaoEmbarcador.SituacaoCargaAposIntegracao;
                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos && (carga.TipoOperacao?.NaoNecessarioConfirmarImpressaoDocumentos ?? false))
                        {
                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte;
                            carga.DataMudouSituacaoParaEmTransporte = DateTime.Now;
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, $"Alterou carga para situação {Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte.ObterDescricao()}", unidadeDeTrabalho);
                        }
                    }

                    if (svcCarga.PermitirFinalizarCargaAutomaticamente(carga, configuracao, TipoServicoMultisoftware, repCargaMDFe))
                        svcCarga.ValidarCargasFinalizadas(ref carga, TipoServicoMultisoftware, Auditado, unidadeDeTrabalho);
                }
                else
                    Servicos.Embarcador.Carga.Documentos.LiberarEmissaoFilialEmissora(carga, unidadeDeTrabalho);

                carga.PossuiPendencia = false;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Liberou carga sem efetuar as Integrações", unidadeDeTrabalho);

                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte)
                {
                    new Servicos.Embarcador.SuperApp.IntegracaoNotificacaoApp(unidadeDeTrabalho).GerarIntegracaoNotificacao(carga, TipoNotificacaoApp.MotoristaPodeSeguirViagem);
                    if (configuracao.QuandoIniciarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.AoInformarVeiculoNaCargaECargaEmTransporte)
                        Servicos.Embarcador.Monitoramento.Monitoramento.GerarMonitoramentoEIniciar(carga, configuracao, null, "Carga em transporte", unidadeDeTrabalho);
                }

                repCarga.Atualizar(carga);

                unidadeDeTrabalho.CommitChanges();

                svcHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao finalizar a etapa.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarNFesEmillenium()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            Servicos.Embarcador.Integracao.Emillenium.IntegracaoEmillenium svcIntegracao = new Servicos.Embarcador.Integracao.Emillenium.IntegracaoEmillenium(unitOfWork, TipoServicoMultisoftware, unitOfWork.StringConexao);
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoProcessamento> listaProcessamento = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoProcessamento>();

            try
            {
                int codigo = Request.GetIntParam("Carga");

                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.BuscarPedidosPorCaraSemNotasFiscais(codigo);

                unitOfWork.Start();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoProcessamento retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoProcessamento() { status = true, mensagem = "Sucesso no processamento" };

                if (pedidos?.Count > 0)
                    retorno = svcIntegracao.BuscarNotasPorPedidosPendentes(pedidos);
                else
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosSemNF = repPedido.BuscarCargaPedidoSemXMLNotasFiscaisPorCarga(codigo);

                    if (!(cargaPedidosSemNF?.Count > 0))
                        throw new ControllerException("Não existem pedidos sem notas fiscais nessa carga");

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosSemNF)
                        listaProcessamento.Add(VincularCargaPedidoXMLNotaFiscal(cargaPedido, unitOfWork));
                }

                retorno = (listaProcessamento.Count > 0 && listaProcessamento.Exists(obj => !obj.status)) ? listaProcessamento.Find(obj => !obj.status) : retorno;

                unitOfWork.CommitChanges();

                return new JsonpResult(retorno.status, true, retorno.mensagem);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, ex.Message);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados das integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoProcessamento VincularCargaPedidoXMLNotaFiscal(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoProcessamento retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoProcessamento() { status = true, mensagem = "Sucesso no processamento" };

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlsNotasFiscaisPedido = cargaPedido.Pedido.NotasFiscais?.ToList();

            if (xmlsNotasFiscaisPedido == null || xmlsNotasFiscaisPedido.Count == 0)
                throw new ServicoException("Não existem pedidos sem notas fiscais nessa carga");

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in xmlsNotasFiscaisPedido)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosExistente = repPedidoXMLNotaFiscal.BuscarPorNotaFiscal(xmlNotaFiscal.Codigo);
                if (pedidosExistente.Exists(obj => obj.CargaPedido.Carga.Codigo == cargaPedido.Carga.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal();
                pedidoXMLNotaFiscal.CargaPedido = cargaPedido;
                pedidoXMLNotaFiscal.XMLNotaFiscal = xmlNotaFiscal;
                pedidoXMLNotaFiscal.TipoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda;
                repPedidoXMLNotaFiscal.Inserir(pedidoXMLNotaFiscal);

                string retornoFinalizacao = Servicos.WebService.NFe.NotaFiscal.FinalizarEnvioDasNotas(ref cargaPedido, xmlNotaFiscal.Peso, xmlNotaFiscal.Volumes, null, null, null, null, null, ConfiguracaoEmbarcador, TipoServicoMultisoftware, null, null, unitOfWork);
                if (!string.IsNullOrWhiteSpace(retornoFinalizacao))
                    throw new ServicoException(retornoFinalizacao);
            }

            return retorno;
        }
    }
}
