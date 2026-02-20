using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/PedidoSimplificado")]
    public class PedidoSimplificadoController : BaseController
    {
        #region Construtores

        public PedidoSimplificadoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("Carga", "NumeroCarga", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Sessão", "SessaoRoteirizador", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Pedido", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Remetente", "Remetente", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Carregamento", "DataCarregamentoPedido", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "SituacaoPedido", 10, Models.Grid.Align.center, true);

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repositorioSessaoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                int totalRegistros = await repositorioPedido.ContarConsultaAsync(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedido = totalRegistros > 0 ? await repositorioPedido.ConsultarAsync(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

                var listaPedidoRetornar = (
                    from pedido in listaPedido
                    select new
                    {
                        pedido.Codigo,
                        Descricao = pedido.NumeroPedidoEmbarcador,
                        NumeroCarga = string.Join(", ", repositorioCargaPedido.BuscarNumeroCargasPorPedido(pedido.Codigo)),
                        SessaoRoteirizador = string.Join(", ", repositorioSessaoPedido.BuscarSessoesPorPedido(pedido.Codigo)),
                        pedido.Numero,
                        pedido.NumeroPedidoEmbarcador,
                        Remetente = pedido.Remetente?.Descricao ?? pedido.GrupoPessoas?.Descricao ?? string.Empty,
                        Destinatario = pedido.Destinatario?.Descricao ?? string.Empty,
                        Destino = pedido.Destino?.DescricaoCidadeEstado ?? string.Empty,
                        Origem = pedido.Origem?.DescricaoCidadeEstado ?? string.Empty,
                        DataCarregamentoPedido = pedido.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        Motorista = string.Join(", ", repositorioPedido.BuscarMotoristas(pedido.Codigo)),
                        Veiculo = ObterPlacasPedido(pedido, repositorioPedido),
                        SituacaoPedido = pedido.DescricaoSituacaoPedido
                    }
                ).ToList();

                grid.AdicionaRows(listaPedidoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Servicos.Embarcador.PreCarga.PreCarga servicoPreCarga = new Servicos.Embarcador.PreCarga.PreCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoBase = ObterPedidoBase(unitOfWork);

                if (ConfiguracaoEmbarcador.BloquearDatasRetroativasPedido)
                {
                    if (pedidoBase.DataCarregamentoPedido.HasValue && pedidoBase.DataCarregamentoPedido.Value > DateTime.MinValue && pedidoBase.DataCarregamentoPedido.Value.Date < DateTime.Now.Date)
                        throw new ControllerException("A data da coleta não pode ser menor que a data atual.");
                }

                unitOfWork.Start();

                PreencherCodigoCargaEmbarcador(unitOfWork, pedidoBase);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = pedidoBase.Clonar();

                pedido.Numero = repositorioPedido.BuscarProximoNumero();

                if (string.IsNullOrEmpty(pedido.NumeroPedidoEmbarcador) && ConfiguracaoEmbarcador.GerarAutomaticamenteNumeroPedidoEmbarcardorNaoInformado)
                {
                    if (ConfiguracaoEmbarcador.UtilizarNumeroPreCargaPorFilial)
                        pedido.NumeroSequenciaPedido = repositorioPedido.ObterProximoCodigo(pedido.Filial);
                    else
                        pedido.NumeroSequenciaPedido = repositorioPedido.ObterProximoCodigo();

                    pedido.NumeroPedidoEmbarcador = pedido.NumeroSequenciaPedido.ToString();
                }

                PreencherDadosDestinatario(unitOfWork, pedido, Request.GetDoubleParam("Destinatario"));
                PreencherDadosRemetente(unitOfWork, pedido, Request.GetDoubleParam("Remetente"));

                string mensagemErro = string.Empty;
                if (!Servicos.Embarcador.Carga.CargaPedido.ValidarNumeroPedidoEmbarcador(out mensagemErro, pedido.NumeroPedidoEmbarcador, pedido.ObterTomador(), pedido.TipoOperacao, ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal))
                    throw new ControllerException(mensagemErro);

                if (Servicos.Embarcador.Pedido.Pedido.TomadorPossuiPendenciaFinanceira(pedido, TipoServicoMultisoftware, out mensagemErro))
                    throw new ControllerException(mensagemErro);

                PreencherDadosRotaFrete(unitOfWork, pedido);
                EnviarEmailRestricoesDescarga(unitOfWork, pedido);
                pedido.SituacaoAcompanhamentoPedido = SituacaoAcompanhamentoPedido.AgColeta;

                if (repositorioPedido.ContemPedidoMesmoNumero(pedido.Numero))
                    pedido.Numero = repositorioPedido.BuscarProximoNumero();

                repositorioPedido.Inserir(pedido, Auditado);

                pedido.Protocolo = pedido.Codigo;
                pedido.SituacaoPedido = SituacaoPedido.Aberto;
                pedido.EtapaPedido = EtapaPedido.Finalizada;

                repositorioPedido.Atualizar(pedido);

                if (pedido.PedidoIntegradoEmbarcador && pedido.ColetaEmProdutorRural)
                {
                    if (!servicoPreCarga.CriarPreCarga(out string erro, pedido, true, TipoServicoMultisoftware, Usuario))
                        throw new ControllerException(erro);

                    repositorioPedido.Atualizar(pedido);
                }

                if (IsGerarCargaAutomaticamente(pedidoBase))
                {
                    string mensagemErroCriarCarga = Servicos.Embarcador.Pedido.Pedido.CriarCarga(pedido, unitOfWork, TipoServicoMultisoftware, Cliente, ConfiguracaoEmbarcador);

                    if (!string.IsNullOrWhiteSpace(mensagemErroCriarCarga))
                        throw new ControllerException(mensagemErroCriarCarga);
                }

                unitOfWork.CommitChanges();

                NotificarJanelaCarregamento(pedido, unitOfWork);

                return new JsonpResult(new
                {
                    pedido.Codigo,
                    Numero = pedido.Numero.ToString(),
                    NumeroCarga = string.Join(", ", repCargaPedido.BuscarNumeroCargasPorPedido(pedido.Codigo))
                });
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Servicos.Embarcador.PreCarga.PreCarga servicoPreCarga = new Servicos.Embarcador.PreCarga.PreCarga(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigo, true);

                if (repCargaPedido.PossuiCargaInaptaAAlteracao(pedido.Codigo))
                    return new JsonpResult(false, true, "O pedido está vinculado à uma carga que não está na etapa 1-Carga ou 2-NF-e, não sendo possível alterar os dados do mesmo.");

                unitOfWork.Start();

                pedido.ObservacaoInterna = Request.GetNullableStringParam("ObservacaoInterna");

                DateTime.TryParseExact(Request.Params("DataColeta"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataColeta);
                pedido.DataCarregamentoPedido = dataColeta;
                pedido.DataInicialColeta = dataColeta;

                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();

                double.TryParse(Request.Params("Remetente"), out double remetente);
                if (remetente > 0)
                {
                    pedido.Remetente = repCliente.BuscarPorCPFCNPJ(remetente);
                    serPedido.PreecherEnderecoPedido(ref pedidoEnderecoOrigem, pedido.Remetente);
                }
                else
                    pedido.Remetente = null;

                double.TryParse(Request.Params("Destinatario"), out double destinatario);
                if (destinatario > 0)
                {
                    pedido.Destinatario = repCliente.BuscarPorCPFCNPJ(destinatario);
                    serPedido.PreecherEnderecoPedido(ref pedidoEnderecoDestino, pedido.Destinatario);
                    pedido.DestinatarioNaoInformado = false;
                }
                else
                {
                    pedido.Destinatario = null;
                    pedido.DestinatarioNaoInformado = true;
                }

                if (pedidoEnderecoOrigem.Localidade != null)
                    repPedidoEndereco.Inserir(pedidoEnderecoOrigem);
                if (pedidoEnderecoDestino.Localidade != null)
                    repPedidoEndereco.Inserir(pedidoEnderecoDestino);

                bool recriarRota = false;
                if (pedido.Origem != null && pedidoEnderecoOrigem.Localidade != null && pedido.Destino != null && pedidoEnderecoDestino.Localidade != null)
                {
                    if (pedido.Origem.Codigo != pedidoEnderecoOrigem.Localidade.Codigo || pedido.Destino.Codigo != pedidoEnderecoDestino.Localidade.Codigo)
                        recriarRota = true;
                }

                if (pedidoEnderecoOrigem.Localidade != null)
                {
                    pedido.Origem = pedidoEnderecoOrigem.Localidade;
                    pedido.EnderecoOrigem = pedidoEnderecoOrigem;
                }

                if (pedidoEnderecoDestino.Localidade != null)
                {
                    pedido.Destino = pedidoEnderecoDestino.Localidade;
                    pedido.EnderecoDestino = pedidoEnderecoDestino;
                }

                if (pedido.Destino != null && pedido.Origem != null)
                    pedido.RotaFrete = repRotaFrete.BuscarPorOrigemDestino(pedido.Origem, pedido.Destino, true);

                //pedido.NumeroPaletesFracionado = Request.GetDecimalParam("PalletsFracionado");
                decimal novaQtdePalletFracionado = Request.GetDecimalParam("PalletsFracionado");

                decimal diferencaPallet = (novaQtdePalletFracionado - pedido.NumeroPaletesFracionado);
                pedido.NumeroPaletesFracionado = novaQtdePalletFracionado;
                pedido.PalletSaldoRestante += diferencaPallet;
                pedido.PedidoIntegradoEmbarcador = false;

                Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repCargaPedido.BuscarPorPedido(pedido.Codigo);
                if (cargasPedido.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedido)
                    {
                        if (!serCarga.VerificarSeCargaEstaNaLogistica(cargaPedido.Carga, TipoServicoMultisoftware))
                        {
                            int numeroNotas = repPedidoXMLNotaFiscal.ContarXMLPorCargaPedido(cargaPedido.Codigo);

                            if (numeroNotas > 0)
                                throw new ControllerException("Não é possível atualizar esse pedido, pois, o mesmo está em uma carga na situação " + cargaPedido.Carga.DescricaoSituacaoCarga + " e já possui Notas Fiscais vinculadas a ele.");
                        }

                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
                            repCargaPedidoProduto.Deletar(cargaPedidoProduto);
                    }
                }

                pedido.SituacaoPedido = SituacaoPedido.Aberto;

                int.TryParse(Request.Params("ModeloVeicularCarga"), out int modeloVeicularCarga);
                if (modeloVeicularCarga > 0)
                    pedido.ModeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(modeloVeicularCarga);
                else
                    pedido.ModeloVeicularCarga = null;

                int.TryParse(Request.Params("TipoOperacao"), out int tipoOperacao);
                if (tipoOperacao > 0)
                {
                    pedido.TipoOperacao = repTipoOperacao.BuscarPorCodigo(tipoOperacao);
                    pedido.TipoDeCarga = pedido.TipoOperacao?.TipoDeCargaPadraoOperacao;
                }
                else
                {
                    pedido.TipoOperacao = null;
                    pedido.TipoDeCarga = null;
                }

                if (pedido.TipoOperacao != null && !pedido.TipoOperacao.GeraCargaAutomaticamente)
                {
                    pedido.GerarAutomaticamenteCargaDoPedido = false;

                    if (cargasPedido.Count == 0)
                    {
                        pedido.PedidoTotalmenteCarregado = false;
                        //TODO: PPC - Adicionado log temporário para identificar problema de retorno de saldo de pedido.
                        Servicos.Log.TratarErro($"Pedido {pedido.NumeroPedidoEmbarcador} - Liberou saldo pedido {pedido.PesoSaldoRestante} - Peso Total.: {pedido.PesoTotal} - Totalmente carregado.: {pedido.PedidoTotalmenteCarregado}. PedidoSimplificadoController.Atualizar", "SaldoPedido");
                    }
                }

                if (pedido.QtdEntregas == 0)
                    pedido.QtdEntregas = 1;

                pedido.UltimaAtualizacao = DateTime.Now;
                pedido.Usuario = this.Usuario;

                if (pedido.Destinatario != null && pedido.Destinatario.ClienteDescargas.Count > 0 && pedido.Destinatario.ClienteDescargas.FirstOrDefault().RestricoesDescarga.Count > 0)
                    pedido.RestricoesDescarga = pedido.Destinatario.ClienteDescargas.FirstOrDefault().RestricoesDescarga.ToList();
                else
                    pedido.RestricoesDescarga.Clear();

                if (!Servicos.Embarcador.Carga.CargaPedido.ValidarNumeroPedidoEmbarcador(out string erro, pedido.NumeroPedidoEmbarcador, pedido.ObterTomador(), pedido.TipoOperacao, ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal))
                    throw new ControllerException(erro);

                repPedido.Atualizar(pedido, Auditado);

                if (!ConfiguracaoEmbarcador.UtilizarIntegracaoPedido)
                {
                    pedido.PedidoIntegradoEmbarcador = true;

                    if (pedido.ColetaEmProdutorRural)
                    {
                        if (!servicoPreCarga.CriarPreCarga(out erro, pedido, true, TipoServicoMultisoftware, Usuario))
                            throw new ControllerException(erro);
                    }
                    else
                    {
                        if (cargasPedido.Count == 0)
                        {
                            PreencherCodigoCargaEmbarcador(unitOfWork, pedido);
                            string retorno = Servicos.Embarcador.Pedido.Pedido.CriarCarga(pedido, unitOfWork, TipoServicoMultisoftware, Cliente, ConfiguracaoEmbarcador);
                            if (string.IsNullOrWhiteSpace(retorno))
                                repPedido.Atualizar(pedido);
                            else
                                throw new ControllerException(retorno);
                        }
                        else
                        {
                            try
                            {
                                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                                servicoCarga.AtualizarCargaPorPedido(pedido, cargasPedido[0].Carga, recriarRota, TipoServicoMultisoftware, ClienteMultisoftware: Cliente, unitOfWork, ConfiguracaoEmbarcador, Auditado, true);

                                repPedido.Atualizar(pedido);
                            }
                            catch (ServicoException excecao)
                            {
                                throw new ControllerException(excecao.Message);
                            }
                        }
                    }
                }

                repPedido.Atualizar(pedido);

                unitOfWork.CommitChanges();

                NotificarJanelaCarregamento(pedido, unitOfWork);

                return new JsonpResult(new { pedido.Codigo });
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
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigo);

                var dynPedido = new
                {
                    pedido.Codigo,
                    DataColeta = pedido.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    PalletsFracionado = pedido.NumeroPaletesFracionado > 0 ? pedido.NumeroPaletesFracionado.ToString("n3") : "",
                    Remetente = pedido.Remetente != null ? new { Codigo = pedido.Remetente.CPF_CNPJ, pedido.Remetente.Descricao } : null,
                    Destinatario = pedido.Destinatario != null ? new { Codigo = pedido.Destinatario.CPF_CNPJ, pedido.Destinatario.Descricao } : null,
                    TipoOperacao = pedido.TipoOperacao != null ? new { pedido.TipoOperacao.Codigo, pedido.TipoOperacao.Descricao } : null,
                    ModeloVeicularCarga = pedido.ModeloVeicularCarga != null ? new { pedido.ModeloVeicularCarga.Codigo, pedido.ModeloVeicularCarga.Descricao } : null,
                    pedido.ObservacaoInterna
                };

                return new JsonpResult(dynPedido);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Servicos.Embarcador.Pedido.OcorrenciaPedido servOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(unitOfWork);


                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigo, true);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (repCargaPedido.ExistePorPedido(pedido.Codigo))
                    return new JsonpResult(false, true, "O pedido já está vinculado à uma carga, não sendo possível realizar a exclusão do mesmo.");

                unitOfWork.Start();
                pedido.SituacaoPedido = SituacaoPedido.Cancelado;
                repPedido.Atualizar(pedido, Auditado);

                servOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoCancelado, pedido, ConfiguracaoEmbarcador, Cliente);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                return new JsonpResult(false, "Ocorreu uma falha ao excluir o pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoTipoCarga = Request.GetIntParam("TipoCarga");
            int codigoCanalEntrega = Request.GetIntParam("CanalEntrega");
            List<int> codigosCanalEntrega = Request.GetListParam<int>("CodigosCanalEntrega");
            if (codigoCanalEntrega > 0 && codigosCanalEntrega.Count == 0)
                codigosCanalEntrega.Add(codigoCanalEntrega);
            int codigoMotorista = Request.GetIntParam("Motorista");
            int codigoVeiculo = Request.GetIntParam("Veiculo");

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido()
            {
                CidadePoloDestino = Request.GetIntParam("CidadePoloDestino"),
                CidadePoloOrigem = Request.GetIntParam("CidadePoloOrigem"),
                CodigosTipoCarga = codigoTipoCarga == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoCarga },
                CodigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork),
                DataColeta = Request.GetNullableDateTimeParam("DataColeta"),
                Destinatario = Request.GetDoubleParam("Destinatario"),
                Expedidor = Request.GetDoubleParam("Expedidor"),
                Recebedor = Request.GetDoubleParam("Recebedor"),
                Destino = Request.GetIntParam("Destino"),
                FiltrarPorParteDoNumero = configuracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                GrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                NumeroCarga = Request.GetStringParam("CodigoCargaEmbarcador"),
                NumeroNotaFiscal = Request.GetIntParam("NotaFiscal"),
                NumeroPedido = Request.GetIntParam("NumeroPedido"),
                NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador"),
                Origem = Request.GetIntParam("Origem"),
                PaisDestino = Request.GetIntParam("PaisDestino"),
                PaisOrigem = Request.GetIntParam("PaisOrigem"),
                Remetente = Request.GetDoubleParam("Remetente"),
                Situacao = Request.GetNullableEnumParam<SituacaoPedido>("Situacao"),
                Tomador = Request.GetDoubleParam("Tomador"),
                CodigosCanalEntrega = codigosCanalEntrega,
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoFuncionarioVendedor = Request.GetIntParam("FuncionarioVendedor"),
                CodigosMotorista = codigoMotorista > 0 ? new List<int>() { codigoMotorista } : null,
                CodigosVeiculo = codigoVeiculo > 0 ? new List<int>() { codigoVeiculo } : null,
                CodigoAutor = Usuario.Codigo
            };

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Remetente")
                return "Remetente.Nome";

            if (propriedadeOrdenar == "Destinatario")
                return "Destinatario.Nome";

            return propriedadeOrdenar;
        }

        private string ObterPlacasPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.Embarcador.Pedidos.Pedido repPedido)
        {
            string placasPadrao = string.Join(", ", repPedido.BuscarPlacas(pedido.Codigo));

            if (ConfiguracaoEmbarcador.PermitirSelecionarReboquePedido)
            {
                string placaTracao = pedido.VeiculoTracao?.Placa;

                if (string.IsNullOrWhiteSpace(placaTracao))
                    return placasPadrao;
                else if (!string.IsNullOrWhiteSpace(placasPadrao))
                    return placaTracao + ", " + placasPadrao;
                else
                    return placaTracao;
            }

            return placasPadrao;
        }

        private void PreencherCodigoCargaEmbarcador(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (IsGerarCargaAutomaticamente(pedido) && pedido.GerarAutomaticamenteCargaDoPedido)
            {
                if (!ConfiguracaoEmbarcador.NumeroCargaSequencialUnico)
                    pedido.CodigoCargaEmbarcador = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork, pedido.Filial?.Codigo ?? 0).ToString();
                else
                    pedido.CodigoCargaEmbarcador = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork).ToString();

                pedido.AdicionadaManualmente = true;
            }
        }

        private void PreencherDadosDestinatario(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, double cpfCnpjDestinatario)
        {
            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
            Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);

            if (cpfCnpjDestinatario > 0d)
            {
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                pedido.Destinatario = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario);
                servicoPedido.PreecherEnderecoPedido(ref pedidoEnderecoDestino, pedido.Destinatario);
            }
            else
                pedido.DestinatarioNaoInformado = true;

            if (pedido.Recebedor != null)
                servicoPedido.PreecherEnderecoPedido(ref pedidoEnderecoDestino, pedido.Recebedor);

            if (pedidoEnderecoDestino.Localidade != null)
            {
                Repositorio.Embarcador.Pedidos.PedidoEndereco repositorioPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
                repositorioPedidoEndereco.Inserir(pedidoEnderecoDestino);

                pedido.Destino = pedidoEnderecoDestino.Localidade;
                pedido.EnderecoDestino = pedidoEnderecoDestino;
            }
        }

        private void PreencherDadosRemetente(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, double cpfCnpjRemetente)
        {
            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
            Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);

            if (cpfCnpjRemetente > 0d)
            {
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                pedido.Remetente = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjRemetente);
                servicoPedido.PreecherEnderecoPedido(ref pedidoEnderecoOrigem, pedido.Remetente);
            }

            if (pedido.Expedidor != null)
                servicoPedido.PreecherEnderecoPedido(ref pedidoEnderecoOrigem, pedido.Expedidor);

            if (pedidoEnderecoOrigem.Localidade != null)
            {
                Repositorio.Embarcador.Pedidos.PedidoEndereco repositorioPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
                repositorioPedidoEndereco.Inserir(pedidoEnderecoOrigem);

                pedido.Origem = pedidoEnderecoOrigem.Localidade;
                pedido.EnderecoOrigem = pedidoEnderecoOrigem;
            }
        }

        private void PreencherDadosRotaFrete(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);

            if (pedido.Destino != null && pedido.Origem != null)
            {
                if (pedido.RotaFrete == null)
                    pedido.RotaFrete = repositorioRotaFrete.BuscarPorOrigemDestino(pedido.Origem, pedido.Destino, true);
            }
        }

        private Dominio.Entidades.Embarcador.Pedidos.Pedido ObterPedidoBase(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido();

            pedido.AdicionadaManualmente = true;
            pedido.GerarAutomaticamenteCargaDoPedido = true;
            pedido.ObservacaoCTe = ConfiguracaoEmbarcador.ObservacaoCTePadraoEmbarcador;
            pedido.PedidoIntegradoEmbarcador = !ConfiguracaoEmbarcador.UtilizarIntegracaoPedido;
            pedido.SituacaoPedido = SituacaoPedido.Aberto;
            pedido.TipoPessoa = TipoPessoa.Pessoa;
            pedido.UltimaAtualizacao = DateTime.Now;
            pedido.DataCadastro = DateTime.Now;
            pedido.Usuario = this.Usuario;
            pedido.Autor = this.Usuario;
            pedido.QtdEntregas = 1;
            pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
            pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;

            DateTime dataColeta = Request.GetDateTimeParam("DataColeta");
            pedido.DataCarregamentoPedido = dataColeta;
            pedido.DataInicialColeta = dataColeta;

            pedido.NumeroPaletesFracionado = Request.GetDecimalParam("PalletsFracionado");
            pedido.PalletSaldoRestante = pedido.NumeroPaletesFracionado;
            pedido.ObservacaoInterna = Request.GetNullableStringParam("ObservacaoInterna");

            int.TryParse(Request.Params("ModeloVeicularCarga"), out int modeloVeicularCarga);
            if (modeloVeicularCarga > 0)
            {
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                pedido.ModeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(modeloVeicularCarga);
            }

            int.TryParse(Request.Params("TipoOperacao"), out int tipoOperacao);
            if (tipoOperacao > 0)
            {
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                pedido.TipoOperacao = repTipoOperacao.BuscarPorCodigo(tipoOperacao);
                pedido.TipoDeCarga = pedido.TipoOperacao?.TipoDeCargaPadraoOperacao;
            }
            if (pedido.TipoOperacao != null)
            {
                if (!pedido.TipoOperacao.GeraCargaAutomaticamente)
                {
                    pedido.GerarAutomaticamenteCargaDoPedido = false;
                    pedido.PedidoTotalmenteCarregado = false;
                    //TODO: PPC - Adicionado log temporário para identificar problema de retorno de saldo de pedido.
                    Servicos.Log.TratarErro($"Pedido {pedido.NumeroPedidoEmbarcador} - Liberou saldo pedido {pedido.PesoSaldoRestante} - Peso Total.: {pedido.PesoTotal} - Totalmente carregado.: {pedido.PedidoTotalmenteCarregado}. PedidoSimplificadoController.ObterPedidoBase", "SaldoPedido");
                }

                pedido.ColetaEmProdutorRural = pedido.TipoOperacao.ColetaEmProdutorRural;
            }

            return pedido;
        }

        private bool IsGerarCargaAutomaticamente(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            return (pedido.PedidoIntegradoEmbarcador && !pedido.ColetaEmProdutorRural && !pedido.Cotacao);
        }

        private void EnviarEmailRestricoesDescarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if ((pedido.Destinatario?.ClienteDescargas?.Count > 0) && (pedido.Destinatario.ClienteDescargas.FirstOrDefault().RestricoesDescarga.Count > 0))
            {
                pedido.RestricoesDescarga = pedido.Destinatario.ClienteDescargas.FirstOrDefault().RestricoesDescarga.ToList();

                foreach (var restricao in pedido.RestricoesDescarga)
                {
                    if (!string.IsNullOrWhiteSpace(restricao.Email))
                    {
                        Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
                        List<string> emails = restricao.Email.Split(';').ToList();

                        if (!servicoPedido.EnviarRelatorioDetalhesPedidoPorEmail(emails, pedido, restricao, unitOfWork, out string mensagem))
                            Servicos.Log.TratarErro(mensagem, "EmailRestricao");
                    }
                }
            }
        }

        private void NotificarJanelaCarregamento(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedido = repCargaPedido.BuscarPorPedido(pedido.Codigo);

            if (cargaPedido.Count > 0)
            {
                int codigoCarga = cargaPedido.First().Carga.Codigo;
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repCargaJanelaCarregamento.BuscarPorCarga(codigoCarga);

                if (cargaJanelaCarregamento != null)
                    new Servicos.Embarcador.Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);
            }
        }

        #endregion
    }
}
