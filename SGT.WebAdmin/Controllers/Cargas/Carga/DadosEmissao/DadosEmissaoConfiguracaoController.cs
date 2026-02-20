using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosEmissao
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento", "GestaoPatio/FluxoPatio")]
    public class DadosEmissaoConfiguracaoController : BaseController
    {
        #region Construtores

        public DadosEmissaoConfiguracaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargasPedidoConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº do Pedido", "CodigoPedidoEmbarcador", 10, Models.Grid.Align.center, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    grid.AdicionarCabecalho("Canal de Entrega", "CanalEntrega", 10, Models.Grid.Align.center, true);

                grid.AdicionarCabecalho("Notas Fiscais", "NotasFiscais", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Participantes", "TipoEmissaoCTeParticipantes", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Recebedor", "Recebedor", 20, Models.Grid.Align.left, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                int totalRegistros = 0;
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaGrid = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                var retorno = ExecutaPesquisaDocumento(ref listaGrid, ref totalRegistros, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarConfiguracoesPedidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AlterarConfiguracao) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoCarga, codigoFormulaRateio;
                int.TryParse(Request.Params("Carga"), out codigoCarga);
                int.TryParse(Request.Params("FormulaRateio"), out codigoFormulaRateio);

                double cpfCnpjRecebedor;
                double.TryParse(Request.Params("Recebedor"), out cpfCnpjRecebedor);

                double cpfCnpjExpedidor;
                double.TryParse(Request.Params("Expedidor"), out cpfCnpjExpedidor);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoRateio;
                Enum.TryParse(Request.Params("TipoRateio"), out tipoRateio);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoParticipanteDocumentos;
                Enum.TryParse(Request.Params("TipoEmissaoCTeParticipantes"), out tipoParticipanteDocumentos);
                int codigoCargaTrocaNota = 0;

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);


                Repositorio.Embarcador.Rateio.RateioFormula repFormulaRateio = new Repositorio.Embarcador.Rateio.RateioFormula(unitOfWork);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);
                Servicos.Embarcador.Carga.Frete serCargaFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, TipoServicoMultisoftware);
                Servicos.Embarcador.Carga.Rota serRota = new Servicos.Embarcador.Carga.Rota(unitOfWork);
                Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                Servicos.Embarcador.Carga.RateioCTeParaSubcontratacao serRateioCTeParaSubcontratacao = new Servicos.Embarcador.Carga.RateioCTeParaSubcontratacao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga, true);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete && carga.ExigeNotaFiscalParaCalcularFrete)
                    return new JsonpResult(false, true, "A atual situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite que esta ação seja executada.");

                if (!serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                    return new JsonpResult(false, true, "Não é possível alterar os dados da emissão na atual situação da carga (" + carga.DescricaoSituacaoCarga + ")");

                if (carga.CalculandoFrete && !carga.PendenteGerarCargaDistribuidor && carga.SituacaoRoteirizacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Erro)
                    return new JsonpResult(false, true, "Não é possível alterar os dados da emissão enquanto a carga estiver em processo de cálculo de valores do frete.");

                if (carga.CargaAgrupamento != null || carga.CargaVinculada != null)
                    return new JsonpResult(false, true, "A carga foi agrupada, sendo assim não é possível alterá-la.");

                if (carga.TipoOperacao?.ConfiguracaoCalculoFrete?.BloquearAjusteConfiguracoesFreteCarga ?? false)
                    return new JsonpResult(false, true, "A operação não permite que estas informações sejam alteradas manualmente.");

                Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio = repFormulaRateio.BuscarPorCodigo(codigoFormulaRateio);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = null;

                if (formulaRateio != null && formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.peso && repPedidoXMLNotaFiscal.VerificarSeExistemNotaSemPeso(carga.Codigo))
                    return new JsonpResult(false, true, "Não é possível alterar a fórmula de rateio, pois existem notas fiscais sem peso. Retorne para a etapa de notas fiscais e informe o peso.");

                if (formulaRateio != null && formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PorPesoUtilizandoFatorCubagem && repPedidoXMLNotaFiscal.VerificarSeAlgumaNaoPossuiMetrosCubicos(carga.Codigo))
                    return new JsonpResult(false, true, "Não é possível alterar a fórmula de rateio, pois existem notas fiscais sem metros cúbicos. Retorne para a etapa de notas fiscais e informe o metro cúbico.");

                unitOfWork.Start();

                string erro = "";
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = BuscarDocumentosSelecionados(unitOfWork, out erro);
                if (string.IsNullOrWhiteSpace(erro))
                {
                    bool outAlterarFrete = false;
                    bool outAlterarRota = false;
                    bool outReprocessarValePedagio = false;
                    bool alterarFrete = false;
                    bool alterarRota = false;
                    bool reprocessarValePedagio = false;

                    Dominio.Entidades.Cliente expedidor = cpfCnpjExpedidor > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjExpedidor) : null;
                    Dominio.Entidades.Cliente recebedor = cpfCnpjRecebedor > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjRecebedor) : null;

                    bool proximoTrechoComplemento = false;
                    if (recebedor != null && ConfiguracaoEmbarcador.EmitirComplementarRedespachoFilialEmissoraDiferenteUFOrigem)
                    {
                        if (repEmpresa.BuscarEmpresaFilialEmissoraPadraoPorEstadoOrigemRedespacho(recebedor.Localidade.Estado) == null)
                            proximoTrechoComplemento = true;
                    }

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        if (alteracoes == null)
                            cargaPedido.Initialize();

                        AtualizarConfiguracaoDadosEmissao(cargaPedido, tipoParticipanteDocumentos, tipoRateio, recebedor, expedidor, codigoCargaTrocaNota, formulaRateio, proximoTrechoComplemento, unitOfWork, out outAlterarRota, out outAlterarFrete, out outReprocessarValePedagio);

                        if (!alterarFrete)
                            alterarFrete = outAlterarFrete;

                        if (!alterarRota)
                            alterarRota = outAlterarRota;

                        if (!reprocessarValePedagio)
                            reprocessarValePedagio = outReprocessarValePedagio;

                        if (alteracoes == null)
                            alteracoes = cargaPedido.GetChanges();
                    }

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCarga = null;

                    if (alterarRota || alterarFrete)
                        Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(carga, cargaPedidos, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);

                    if (alterarRota || reprocessarValePedagio)
                        cargaPedidosCarga = repCargaPedido.BuscarPorCarga(carga.Codigo);

                    if (alterarRota)
                    {
                        serRota.DeletarPercursoDestinosCarga(carga, unitOfWork);
                        serCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacao(carga, cargaPedidosCarga, unitOfWork, TipoServicoMultisoftware, configuracaoPedido);
                        serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidosCarga, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);
                    }

                    if (reprocessarValePedagio)
                        Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, cargaPedidosCarga, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);

                    serCarga.SetarTipoContratacaoCarga(carga, unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoFrete = null;
                    if (alterarFrete)
                    {
                        if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador && carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                        {
                            carga.DataInicioCalculoFrete = DateTime.Now;
                            carga.CalculandoFrete = true;
                            repCarga.Atualizar(carga);
                            retornoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.CalculandoFrete };
                            string retornoMontagem = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork).CalcularFreteTodoCarregamento(carga);
                            if (!string.IsNullOrWhiteSpace(retornoMontagem))
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, retornoMontagem);
                            }
                        }
                        else
                        {
                            Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);

                            if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Operador)
                                carga.ValorFrete = carga.ValorFreteOperador;

                            if (cargaPedidosCarga == null)
                                cargaPedidosCarga = repCargaPedido.BuscarPorCarga(carga.Codigo);

                            serRateioFrete.RatearValorDoFrenteEntrePedidos(carga, cargaPedidosCarga, ConfiguracaoEmbarcador, false, unitOfWork, TipoServicoMultisoftware);
                            serRateioCTeParaSubcontratacao.RatearFreteCargaCTesParaSubcontratacao(carga, TipoServicoMultisoftware, unitOfWork);


                            if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Embarcador)
                            {
                                carga.DataInicioCalculoFrete = DateTime.Now;
                                carga.CalculandoFrete = true;
                                repCarga.Atualizar(carga);
                                retornoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.CalculandoFrete };
                                string retornoMontagem = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork).CalcularFreteTodoCarregamento(carga);
                                if (!string.IsNullOrWhiteSpace(retornoMontagem))
                                {
                                    unitOfWork.Rollback();
                                    return new JsonpResult(false, true, retornoMontagem);
                                }
                            }
                        }
                    }

                    if (alteracoes != null)
                        alteracoes.AddRange(carga.GetChanges());
                    else
                        alteracoes = carga.GetChanges();

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, alteracoes, "Atualizou as configurações da Carga", unitOfWork);

                    unitOfWork.CommitChanges();
                    svcHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                    return new JsonpResult(retornoFrete);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a configuração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AlterarConfiguracao) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                bool aplicarConfiguracaoEmTodosPedidos = Request.GetBoolParam("AplicarConfiguracaoEmTodosPedidos");

                int codigoCarga, codigoFormulaRateio, codigoPedido;
                int.TryParse(Request.Params("Carga"), out codigoCarga);
                int.TryParse(Request.Params("FormulaRateio"), out codigoFormulaRateio);
                int.TryParse(Request.Params("Pedido"), out codigoPedido);
                int codigoCargaTrocaNota = aplicarConfiguracaoEmTodosPedidos ? Request.GetIntParam("CargaTrocaNota") : 0;
                int quantidadePaletes = Request.GetIntParam("QuantidadePaletes");

                decimal? fatorCubagemRateioFormula = Request.GetNullableDecimalParam("FatorCubagemRateioFormula");
                TipoUsoFatorCubagem? tipoUsoFatorCubagemRateioFormula = Request.GetNullableEnumParam<TipoUsoFatorCubagem>("TipoUsoFatorCubagemRateioFormula");

                double cpfCnpjRecebedor = Request.GetDoubleParam("Recebedor");
                double cpfCnpjExpedidor = Request.GetDoubleParam("Expedidor");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoRateio = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos>("TipoRateio");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoParticipanteDocumentos = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes>("TipoEmissaoCTeParticipantes");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Rateio.RateioFormula repFormulaRateio = new Repositorio.Embarcador.Rateio.RateioFormula(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);


                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);
                Servicos.Embarcador.Carga.Frete serCargaFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, TipoServicoMultisoftware);
                Servicos.Embarcador.Carga.Rota serRota = new Servicos.Embarcador.Carga.Rota(unitOfWork);
                Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                Servicos.Embarcador.Carga.RateioCTeParaSubcontratacao serRateioCTeParaSubcontratacao = new Servicos.Embarcador.Carga.RateioCTeParaSubcontratacao(unitOfWork);
                Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
                Servicos.Embarcador.Carga.CargaPallets svcCargaPaletes = new Servicos.Embarcador.Carga.CargaPallets(unitOfWork, ConfiguracaoEmbarcador);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = null;

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga, auditavel: true);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

                if (!serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                    return new JsonpResult(false, true, "Não é possível alterar os dados da emissão na atual situação da carga (" + carga.DescricaoSituacaoCarga + ")");

                if (carga.CalculandoFrete && !carga.PendenteGerarCargaDistribuidor && carga.SituacaoRoteirizacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Erro)
                    return new JsonpResult(false, true, "Não é possível alterar os dados da emissão enquanto a carga estiver em processo de cálculo de valores do frete.");

                Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio = repFormulaRateio.BuscarPorCodigo(codigoFormulaRateio);

                if (formulaRateio != null && formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.peso && repPedidoXMLNotaFiscal.VerificarSeExistemNotaSemPeso(carga.Codigo))
                    return new JsonpResult(false, true, "Não é possível alterar a fórmula de rateio, pois existem notas fiscais sem peso. Retorne para a etapa de notas fiscais e informe o peso.");

                if (formulaRateio != null && formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PorPesoUtilizandoFatorCubagem && repPedidoXMLNotaFiscal.VerificarSeAlgumaNaoPossuiMetrosCubicos(carga.Codigo))
                    return new JsonpResult(false, true, "Não é possível alterar a fórmula de rateio, pois existem notas fiscais sem metros cúbicos. Retorne para a etapa de notas fiscais e informe o metro cúbico.");

                if (carga.TipoOperacao?.ConfiguracaoCalculoFrete?.BloquearAjusteConfiguracoesFreteCarga ?? false)
                    return new JsonpResult(false, true, "A operação não permite que estas informações sejam alteradas manualmente.");

                serCarga.ValidarPermissaoAlterarDadosEtapaFrete(carga, unitOfWork);

                unitOfWork.Start();

                bool outAlterarFrete = false;
                bool outAlterarRota = false;
                bool outReprocessarValePedagio = false;
                bool alterarFrete = false;
                bool alterarRota = false;
                bool ratearPaletes = carga.TipoOperacao?.ConfiguracaoCalculoFrete?.PermiteInformarQuantidadePaletes ?? false;
                bool reprocessarValePedagio = false;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = null;

                Dominio.Entidades.Cliente expedidor = cpfCnpjExpedidor > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjExpedidor) : null;
                Dominio.Entidades.Cliente recebedor = cpfCnpjRecebedor > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjRecebedor) : null;

                if (carga.CargaAgrupada)
                    carga.AgIntegracaoAgrupamentoCarga = true;

                bool proximoTrechoComplemento = false;
                if (recebedor != null && ConfiguracaoEmbarcador.EmitirComplementarRedespachoFilialEmissoraDiferenteUFOrigem)
                {
                    if (repEmpresa.BuscarEmpresaFilialEmissoraPadraoPorEstadoOrigemRedespacho(recebedor.Localidade.Estado) == null)
                        proximoTrechoComplemento = true;
                }

                if (aplicarConfiguracaoEmTodosPedidos)
                {
                    cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                    if (ratearPaletes)
                        svcCargaPaletes.RatearPaletesEntreOsPedidos(quantidadePaletes, cargaPedidos);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        if (alteracoes == null)
                            cargaPedido.Initialize();

                        AtualizarConfiguracaoDadosEmissao(cargaPedido, tipoParticipanteDocumentos, tipoRateio, recebedor, expedidor, codigoCargaTrocaNota, formulaRateio, proximoTrechoComplemento, unitOfWork, out outAlterarRota, out outAlterarFrete, out outReprocessarValePedagio, fatorCubagemRateioFormula, tipoUsoFatorCubagemRateioFormula);

                        if (!alterarFrete)
                            alterarFrete = outAlterarFrete;

                        if (!alterarRota)
                            alterarRota = outAlterarRota;

                        if (!reprocessarValePedagio)
                            reprocessarValePedagio = outReprocessarValePedagio;

                        if (alteracoes == null)
                            alteracoes = cargaPedido.GetChanges();
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(codigoCarga, codigoPedido);
                    cargaPedido.Initialize();

                    if (ratearPaletes)
                        svcCargaPaletes.RatearPaletesEntreOsPedidos(quantidadePaletes, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido });

                    AtualizarConfiguracaoDadosEmissao(cargaPedido, tipoParticipanteDocumentos, tipoRateio, recebedor, expedidor, codigoCargaTrocaNota, formulaRateio, proximoTrechoComplemento, unitOfWork, out outAlterarRota, out outAlterarFrete, out outReprocessarValePedagio, fatorCubagemRateioFormula, tipoUsoFatorCubagemRateioFormula);

                    if (!alterarFrete)
                        alterarFrete = outAlterarFrete;

                    if (!alterarRota)
                        alterarRota = outAlterarRota;

                    if (!reprocessarValePedagio)
                        reprocessarValePedagio = outReprocessarValePedagio;

                    alteracoes = cargaPedido.GetChanges();

                    cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                }

                if (alterarRota || alterarFrete)
                {
                    if (alterarRota)
                        carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.SemDefinicao;
                    Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(carga, cargaPedidos, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);
                }

                if (alterarRota || (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && alterarFrete))
                {
                    cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                    serRota.DeletarPercursoDestinosCarga(carga, unitOfWork);
                    serCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacao(carga, cargaPedidos, unitOfWork, TipoServicoMultisoftware, configuracaoPedido);
                    serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);
                }

                if (reprocessarValePedagio)
                    Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, cargaPedidos, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);

                serCarga.SetarTipoContratacaoCarga(carga, unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoFrete = null;

                if (alterarFrete)
                {
                    if (carga.CargaTransbordo)
                    {
                        retornoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { situacao = SituacaoRetornoDadosFrete.FreteValido };

                        Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro svcFreteSubcontratacaoTerceiro = new Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro(unitOfWork);
                        Servicos.Embarcador.Carga.Frete svcFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, TipoServicoMultisoftware);

                        svcFreteSubcontratacaoTerceiro.CalcularFreteSubcontratacao(carga, carga.TipoFreteEscolhido, unitOfWork, false, TipoServicoMultisoftware, _conexao.StringConexao);
                        svcFrete.SetarDadosGeraisRetornoFrete(ref retornoFrete, carga, unitOfWork, true, false, carga.TipoFreteEscolhido);
                    }
                    else
                    {
                        if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador && carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                        {
                            Servicos.Embarcador.Carga.CargaAprovacaoFrete servicoCargaAprovacaoFrete = new Servicos.Embarcador.Carga.CargaAprovacaoFrete(unitOfWork);
                            servicoCargaAprovacaoFrete.RemoverAprovacao(carga);
                            carga.DataInicioCalculoFrete = DateTime.Now;
                            carga.CalculandoFrete = true;
                            repCarga.Atualizar(carga);
                            retornoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.CalculandoFrete };
                            string retornoMontagem = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork).CalcularFreteTodoCarregamento(carga);
                            if (!string.IsNullOrWhiteSpace(retornoMontagem))
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, retornoMontagem);
                            }
                        }
                        else
                        {
                            Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);

                            if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Operador)
                                carga.ValorFrete = carga.ValorFreteOperador;

                            if (cargaPedidos == null)
                                cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                            serRateioFrete.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, ConfiguracaoEmbarcador, false, unitOfWork, TipoServicoMultisoftware);
                            serRateioCTeParaSubcontratacao.RatearFreteCargaCTesParaSubcontratacao(carga, TipoServicoMultisoftware, unitOfWork);

                            if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Embarcador)
                            {
                                carga.DataInicioCalculoFrete = DateTime.Now;
                                carga.CalculandoFrete = true;
                                repCarga.Atualizar(carga);
                                retornoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.CalculandoFrete };
                                string retornoMontagem = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork).CalcularFreteTodoCarregamento(carga);
                                if (!string.IsNullOrWhiteSpace(retornoMontagem))
                                {
                                    unitOfWork.Rollback();
                                    return new JsonpResult(false, true, retornoMontagem);
                                }
                            }
                        }
                    }
                }

                if (alteracoes != null)
                    alteracoes.AddRange(carga.GetChanges());
                else
                    alteracoes = carga.GetChanges();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, alteracoes, "Atualizou as configurações da Carga", unitOfWork);

                unitOfWork.CommitChanges();
                svcHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                return new JsonpResult(retornoFrete);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a configuração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void AtualizarConfiguracaoDadosEmissao(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoParticipanteDocumentos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoRateio, Dominio.Entidades.Cliente recebedor, Dominio.Entidades.Cliente expedidor, int codigoCargaTrocaNota, Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio, bool proximoTrechoComplemento, Repositorio.UnitOfWork unitOfWork, out bool alterarRota, out bool alterarFrete, out bool reprocessarValePedagio, decimal? fatorCubagemRateioFormula = null, TipoUsoFatorCubagem? tipoUsoFatorCubagemRateioFormula = null)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

            cargaPedido.Initialize();

            int codigoOrigemOriginal = cargaPedido.Origem.Codigo;
            int codigoDestinoOriginal = cargaPedido.Destino?.Codigo ?? 0;
            int formulaRateioOriginal = cargaPedido.FormulaRateio != null ? cargaPedido.FormulaRateio.Codigo : 0;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoParticipanteDocumentosOriginal = cargaPedido.TipoEmissaoCTeParticipantes;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoRateioOriginal = cargaPedido.TipoRateio;
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

            cargaPedido.TipoRateio = tipoRateio;
            cargaPedido.FormulaRateio = formulaRateio;
            cargaPedido.TipoEmissaoCTeParticipantes = tipoParticipanteDocumentos;

            if (cargaPedido.FormulaRateio?.ParametroRateioFormula == ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
            {
                cargaPedido.TipoUsoFatorCubagemRateioFormula = tipoUsoFatorCubagemRateioFormula ?? null;
                cargaPedido.FatorCubagemRateioFormula = fatorCubagemRateioFormula ?? null;
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {             
                List<Dominio.Entidades.Cliente> clientesBloquearEmissaoDosDestinatarios = cargaPedido.Pedido.TipoOperacao?.ClientesBloquearEmissaoDosDestinatario.ToList();
                serCargaPedido.BuscarConfiguracoesMultimodal(cargaPedido.Carga, unitOfWork, ref cargaPedido, integracaoIntercab, clientesBloquearEmissaoDosDestinatarios);
            }              


            if (cargaPedido.CargaPedidoProximoTrecho != null)
                throw new ControllerException("Não possível alterar a configuração pois a carga já possui uma carga de próximo trecho.");

            //if (cargaPedido.CargaPedidoTrechoAnterior != null)
            //    throw new ControllerException("Não possível alterar a configuração pois a carga possui uma carga de trecho anterior.");

            if (!cargaPedido.AgInformarRecebedor && cargaPedido.PendenteGerarCargaDistribuidor && cargaPedido.Recebedor != null)
                throw new ControllerException("Não possível alterar a configuração pois a carga de segundo trecho está sendo gerada.");

            if (tipoParticipanteDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || tipoParticipanteDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
            {
                cargaPedido.Recebedor = recebedor;
                if (cargaPedido.Recebedor != null)
                {
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || cargaPedido.Recebedor.CPF_CNPJ != cargaPedido.Pedido.Destinatario?.CPF_CNPJ)
                    {
                        cargaPedido.ProximoTrechoComplementaFilialEmissora = (proximoTrechoComplemento && recebedor.Localidade.Estado.Sigla != cargaPedido.Origem.Estado.Sigla && !cargaPedido.EmitirComplementarFilialEmissora && cargaPedido.CargaPedidoFilialEmissora);

                        if (!(cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false))
                            cargaPedido.Destino = cargaPedido.Recebedor.Localidade;

                        cargaPedido.AgInformarRecebedor = false;
                    }
                    else
                    {
                        if (cargaPedido.AgInformarRecebedor)
                            throw new ControllerException("O recebedor deve ser diferente do destinatário do pedido.");

                        cargaPedido.Recebedor = null;
                        if (cargaPedido.Expedidor != null)
                            cargaPedido.TipoEmissaoCTeParticipantes = TipoEmissaoCTeParticipantes.ComExpedidor;
                        else
                            cargaPedido.TipoEmissaoCTeParticipantes = TipoEmissaoCTeParticipantes.Normal;
                    }
                }
                else
                    throw new ControllerException("É obrigatório informar o recebedor");
            }
            else
            {
                if (cargaPedido.Recebedor != null)
                    cargaPedido.Destino = cargaPedido.Pedido.EnderecoDestino?.Localidade ?? cargaPedido.Pedido.Destinatario?.Localidade;

                cargaPedido.Recebedor = null;
            }

            if (tipoParticipanteDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor || tipoParticipanteDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
            {
                cargaPedido.Expedidor = expedidor;

                if (cargaPedido.Expedidor != null)
                {
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || expedidor.CPF_CNPJ != cargaPedido.Pedido.Remetente?.CPF_CNPJ)
                        cargaPedido.Origem = cargaPedido.Expedidor.Localidade;
                    else
                    {
                        cargaPedido.Expedidor = null;
                        if (cargaPedido.Recebedor != null)
                            cargaPedido.TipoEmissaoCTeParticipantes = TipoEmissaoCTeParticipantes.ComRecebedor;
                        else
                            cargaPedido.TipoEmissaoCTeParticipantes = TipoEmissaoCTeParticipantes.Normal;
                    }
                }
                else
                    throw new ControllerException("É obrigatório informar o recebedor");
            }
            else
            {
                if (cargaPedido.Expedidor != null)
                    cargaPedido.Origem = cargaPedido.Pedido.EnderecoOrigem?.Localidade ?? cargaPedido.Pedido.Remetente?.Localidade;

                cargaPedido.Expedidor = null;
            }

            alterarFrete = false;
            alterarRota = false;
            reprocessarValePedagio = false;

            bool verificarPendentesDistribuidor = false;
            if (cargaPedido.AgInformarRecebedor && tipoParticipanteDocumentos != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor && tipoParticipanteDocumentos != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
            {
                cargaPedido.Destino = cargaPedido.Pedido.Destinatario.Localidade;
                cargaPedido.AgInformarRecebedor = false;
                cargaPedido.PendenteGerarCargaDistribuidor = false;
                verificarPendentesDistribuidor = true;
            }

            if (codigoCargaTrocaNota > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga cargaTrocaNota = repCarga.BuscarPorCodigo(codigoCargaTrocaNota, auditavel: false);

                if ((cargaTrocaNota.CargaTrocaNota?.Codigo != cargaPedido.Carga.Codigo))
                {
                    if (!(cargaPedido.Carga.TipoOperacao?.PermitirTrocaNota ?? false))
                        throw new ControllerException($"O tipo de operação da carga não permite a troca de nota.");

                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoTrocaNota = cargaTrocaNota.Pedidos.FirstOrDefault();

                    if (
                        (cargaTrocaNota.CargaTrocaNota != null) &&
                        (cargaTrocaNota.CargaTrocaNota.SituacaoCarga != SituacaoCarga.Cancelada) &&
                        (cargaTrocaNota.CargaTrocaNota.SituacaoCarga != SituacaoCarga.Anulada)
                    )
                        throw new ControllerException($"A carga selecionada já foi utilizada para troca de nota da carga {servicoCarga.ObterNumeroCarga(cargaTrocaNota.CargaTrocaNota, configuracaoEmbarcador)}.");

                    if (cargaPedido.Origem.Codigo != cargaPedidoTrocaNota.Destino.Codigo)
                        throw new ControllerException("O destino da carga selecionada para troca de nota deve ser igual a origem da carga atual.");

                    cargaTrocaNota.CargaTrocaNota = cargaPedido.Carga;

                    repCarga.Atualizar(cargaTrocaNota);

                    Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                    cargaPedido.Carga.OrigemTrocaNota = cargaPedidoTrocaNota.Origem;
                    cargaPedido.Carga.TipoOperacao = repositorioTipoOperacao.BuscarTipoOperacaoTrocaNota() ?? throw new ControllerException("Tipo de operação para troca de nota não encontrada.");

                    repCarga.Atualizar(cargaPedido.Carga);

                    alterarFrete = true;
                }
            }
            else if (cargaPedido.Carga.OrigemTrocaNota != null)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga cargaTrocaNota = repCarga.BuscarCargaPorCargaTrocaNota(cargaPedido.Carga.Codigo);

                if (cargaTrocaNota != null)
                {
                    cargaTrocaNota.CargaTrocaNota = null;

                    repCarga.Atualizar(cargaTrocaNota);
                }

                cargaPedido.Carga.OrigemTrocaNota = null;

                repCarga.Atualizar(cargaPedido.Carga);

                alterarFrete = true;
            }

            if (cargaPedido.Origem.Codigo != codigoOrigemOriginal || cargaPedido.Destino.Codigo != codigoDestinoOriginal)
            {
                alterarFrete = true;
                alterarRota = true;
                //mesmo que o imposto foi mandando pelo embarcador, ao mudar a rota o imposto pode mudar também se a localidade da prestação for modificada.
                cargaPedido.ImpostoInformadoPeloEmbarcador = false;

                if ((codigoOrigemOriginal == codigoDestinoOriginal && cargaPedido.Destino.Codigo != cargaPedido.Origem.Codigo) || (cargaPedido.Destino.Codigo == cargaPedido.Origem.Codigo && codigoOrigemOriginal != codigoDestinoOriginal))
                {
                    bool possuiCTe = false;
                    bool possuiNFS = false;
                    bool possuiNFSManual = false;
                    Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoIntramunicipal = null;

                    serCargaPedido.VerificarQuaisDocumentosDeveEmitir(cargaPedido.Carga, cargaPedido, cargaPedido.Origem, cargaPedido.Destino, TipoServicoMultisoftware, unitOfWork, out possuiCTe, out possuiNFS, out possuiNFSManual, out modeloDocumentoIntramunicipal, configuracaoEmbarcador, out bool sempreDisponibilizarDocumentoNFSManual);

                    cargaPedido.PossuiCTe = possuiCTe;
                    cargaPedido.PossuiNFS = possuiNFS;
                    cargaPedido.PossuiNFSManual = possuiNFSManual;
                    cargaPedido.DisponibilizarDocumentoNFSManual = sempreDisponibilizarDocumentoNFSManual;
                }
            }

            if (cargaPedido.TipoEmissaoCTeParticipantes != tipoParticipanteDocumentosOriginal || (cargaPedido.FormulaRateio != null && cargaPedido.FormulaRateio.Codigo != formulaRateioOriginal) || cargaPedido.TipoRateio != tipoRateioOriginal)
                alterarFrete = true;

            if (cargaPedido.TipoEmissaoCTeParticipantes != tipoParticipanteDocumentosOriginal || cargaPedido.TipoRateio != tipoRateioOriginal)
                reprocessarValePedagio = true;

            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido, cargaPedido.GetChanges(), "Atualizou as configurações da Carga", unitOfWork);

            if ((cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMTerceiro ||
                cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada ||
                                cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho ||
                                cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMProprio ||
                                cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                                && !cargaPedido.Carga.EmitirCTeComplementar && !(integracaoIntercab?.BuscarTipoServicoModeloDocumentoVinculadoCarga ?? false))
            {
                if (cargaPedido.Recebedor != null && cargaPedido.Expedidor != null)
                {
                    if (cargaPedido.Carga.GrupoPessoaPrincipal != null && cargaPedido.Carga.GrupoPessoaPrincipal.EmitirSempreComoRedespacho)
                        cargaPedido.TipoContratacaoCarga = TipoContratacaoCarga.Redespacho;
                    else
                        cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario;
                }
                else if (cargaPedido.Expedidor != null || cargaPedido.Recebedor != null)
                    cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                else
                    cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;

                if ((ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal && cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.Subcontratacao) || (cargaPedido.Carga.TipoOperacao?.SempreEmitirSubcontratacao ?? false))
                    cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;
            }

            if (verificarPendentesDistribuidor)
            {
                if (!repCargaPedido.VerificarCargasPendentesDistribuidor(cargaPedido.Carga.Codigo))
                {
                    cargaPedido.Carga.PendenteGerarCargaDistribuidor = false;
                    repCarga.Atualizar(cargaPedido.Carga);
                }
            }
            serCargaPedido.VerificarFilialEmissaoCargaPedido(cargaPedido, configuracaoGeralCarga);

            repCargaPedido.Atualizar(cargaPedido);
            repPedido.Atualizar(cargaPedido.Pedido);
        }

        private List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarDocumentosSelecionados(Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaBusca = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();


            bool.TryParse(Request.Params("SelecionarTodos"), out bool todosSelecionados);
            if (todosSelecionados)
            {
                // Reconsulta com os mesmos dados e remove apenas os desselecionados
                try
                {
                    int totalRegistros = 0;
                    var retorno = ExecutaPesquisaDocumento(ref listaBusca, ref totalRegistros, "Codigo", "", 0, 0, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                // Iterar ocorrencias desselecionados e remove da lista
                dynamic listaNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosNaoSelecionadas"));
                foreach (var dynNaoSelecionada in listaNaoSelecionadas)
                    listaBusca.Remove(new Dominio.Entidades.Embarcador.Cargas.CargaPedido() { Codigo = (int)dynNaoSelecionada.Codigo });
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosSelecionadas"));
                foreach (var dynSelecionada in listaSelecionadas)
                    listaBusca.Add(repCargaPedido.BuscarPorCodigo((int)dynSelecionada.Codigo, false));
            }

            // Retorna lista
            return listaBusca;
        }

        private dynamic ExecutaPesquisaDocumento(ref List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaGrid, ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            int empresa;
            int.TryParse(Request.Params("Empresa"), out empresa);

            int carga;
            int.TryParse(Request.Params("Carga"), out carga);

            int codigo;
            int.TryParse(Request.Params("Codigo"), out codigo);

            bool somentePendentes = false;
            bool.TryParse(Request.Params("SomentePendentes"), out somentePendentes);

            string CodigoCargaEmbarcador = Request.Params("CodigoCargaEmbarcador");
            string CodigoPedidoEmbarcador = Request.Params("CodigoPedidoEmbarcador");
            int operador;
            int.TryParse(Request.Params("Operador"), out operador);
            int veiculo;
            int.TryParse(Request.Params("Veiculo"), out veiculo);

            int numeroNF;
            int.TryParse(Request.Params("NumeroNF"), out numeroNF);

            int numeroCTe;
            int.TryParse(Request.Params("NumeroCTe"), out numeroCTe);

            int modeloVeicularCarga;
            int.TryParse(Request.Params("ModeloVeicularCarga"), out modeloVeicularCarga);
            int tipoCarga;
            int.TryParse(Request.Params("TipoCarga"), out tipoCarga);
            double destinatario;
            double.TryParse(Request.Params("Destinatario"), out destinatario);
            double remetente;
            double.TryParse(Request.Params("Remetente"), out remetente);
            int filial, origem, destino;
            int.TryParse(Request.Params("Filial"), out filial);
            int.TryParse(Request.Params("Origem"), out origem);
            int.TryParse(Request.Params("Destino"), out destino);

            int tipoOperacao = 0;
            int.TryParse(Request.Params("TipoOperacao"), out tipoOperacao);

            string estadoDestino = "";

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesLiberadas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>();
            List<int> codigosFiliais = new List<int>(); //ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork);
            listaGrid = repCargaPedido.Consultar(situacoesLiberadas, carga, somentePendentes, numeroNF, numeroCTe, CodigoPedidoEmbarcador, CodigoCargaEmbarcador, origem, destino, filial, remetente, destinatario, 0, 0, false, false, estadoDestino, false, codigosFiliais, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repCargaPedido.ContarConsulta(situacoesLiberadas, carga, somentePendentes, numeroNF, numeroCTe, CodigoPedidoEmbarcador, CodigoCargaEmbarcador, origem, destino, filial, remetente, destinatario, 0, 0, false, false, estadoDestino, false, codigosFiliais);


            var dynListaCarga = (from obj in listaGrid
                                 select new
                                 {
                                     obj.Codigo,
                                     CodigoPedidoEmbarcador = obj.Pedido.NumeroPedidoEmbarcador,
                                     //Destino = obj.Destino.DescricaoCidadeEstado,
                                     CanalEntrega = obj.Pedido.CanalEntrega?.Descricao ?? "",
                                     Destinatario = obj.Pedido.Destinatario != null ? (obj.Pedido.Destinatario.Descricao + " - " + obj.Pedido.Destinatario.Localidade.DescricaoCidadeEstado) : "",
                                     TipoEmissaoCTeParticipantes = obj.TipoEmissaoCTeParticipantes.ObterDescricao(),
                                     Recebedor = obj.Recebedor?.Descricao ?? "",
                                     NotasFiscais = string.Join(",", (from nf in obj.NotasFiscais select nf.XMLNotaFiscal.Numero))
                                 }).ToList();

            return dynListaCarga;
        }

        #endregion
    }
}
