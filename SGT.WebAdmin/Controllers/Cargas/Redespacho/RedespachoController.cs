using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Redespacho
{
    [CustomAuthorize(new string[] { "ObterExpedidorCarga" }, "Cargas/Redespacho")]
    public class RedespachoController : BaseController
    {
        #region Construtores

        public RedespachoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosCargaPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    Distancia = carga.Distancia > 0m ? carga.Distancia.ToString("n4") : ""
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados da carga.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.Redespacho repRedespacho = new Repositorio.Embarcador.Cargas.Redespacho(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.Redespacho redespacho = await repRedespacho.BuscarPorCodigoAsync(codigo, false);
                return new JsonpResult(ObterRedespacho(redespacho, unitOfWork));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarPedidosRedespacho()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                List<int> cargasUtilizadas = Request.GetListParam<int>("Cargas");

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº do Pedido", "CodigoPedidoEmbarcador", 10, Models.Grid.Align.center, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    grid.AdicionarCabecalho("Canal de Entrega", "CanalEntrega", 10, Models.Grid.Align.center, true);

                grid.AdicionarCabecalho("Notas Fiscais", "NotasFiscais", 15, Models.Grid.Align.left, false);

                var repoConfigCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                var configuracaoCarga = repoConfigCarga.BuscarPrimeiroRegistro();

                if (new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro().PermiteSelecionarMultiplasCargasParaRedespacho)
                    grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.center, true);

                grid.AdicionarCabecalho("Destinatário", "Destinatario", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Participantes", "TipoEmissaoCTeParticipantes", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Recebedor", "Recebedor", 20, Models.Grid.Align.left, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "CodigoPedidoEmbarcador")
                    propOrdenacao = "Pedido.NumeroPedidoEmbarcador";

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

        public async Task<IActionResult> GerarRedespacho(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Redespacho repRedespacho = new Repositorio.Embarcador.Cargas.Redespacho(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await repConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();

                int codigoCarga, codigoTipoOperacao;
                int.TryParse(Request.Params("Carga"), out codigoCarga);
                int.TryParse(Request.Params("TipoOperacao"), out codigoTipoOperacao);

                List<int> cargasUtilizadas = Request.GetListParam<int>("Cargas");

                var repoConfigCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                var configuracaoCarga = await repoConfigCarga.BuscarPrimeiroRegistroAsync();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigoCarga);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasUtilizadasEntidade = await repCarga.BuscarPorCodigosAsync(cargasUtilizadas);
                if (carga == null && cargasUtilizadasEntidade.Count == 0)
                    return new JsonpResult(false, true, "Não foi possivel encontrar a carga");

                if (configuracaoCarga.NaoPermitirGerarRedespachoDeCargasDeRedespacho)
                {
                    if (carga.Redespacho != null)
                        return new JsonpResult(false, true, "Não é permitido gerar Redespacho de cargas de Redespacho.");
                }

                int recebedor = 0;
                if (configuracaoCarga.PermitirInformarRecebedorAoCriarUmRedespachoManual)
                {
                    int.TryParse(Request.Params("Recebedor"), out recebedor);
                }

                double expedidor = 0;
                double.TryParse(Request.Params("Expedidor"), out expedidor);
                bool selecionouTodas = bool.Parse(Request.Params("SelecionarTodos"));

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

                if (!selecionouTodas)
                {
                    dynamic dynPedidosSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosSelecionadas"));
                    foreach (var dynCargaPedidoSelecionada in dynPedidosSelecionados)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = await repCargaPedido.BuscarPorCodigoAsync((int)dynCargaPedidoSelecionada.Codigo);
                        cargaPedidos.Add(cargaPedido);
                    }
                }
                else
                {
                    int totalRegistro = 0;
                    ExecutaPesquisaDocumento(ref cargaPedidos, ref totalRegistro, "", "", 0, 0, unitOfWork);
                    dynamic dynPedidosNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosNaoSelecionadas"));
                    foreach (var dynCargaPedidoNaoSelecionada in dynPedidosNaoSelecionados)
                    {
                        int codigo = (int)dynCargaPedidoNaoSelecionada.Codigo;
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = (from obj in cargaPedidos where obj.Codigo == codigo select obj).FirstOrDefault();

                        if (cargaPedido != null)
                            cargaPedidos.Remove(cargaPedido);
                    }
                }

                List<int> codigosCargasCargaPedido = (from cp in cargaPedidos select cp.Carga.Codigo).Distinct().ToList();

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarPorCodigos(codigosCargasCargaPedido);

                if (cargasUtilizadas.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaUtilizada in cargasUtilizadasEntidade)
                    {
                        if (!codigosCargasCargaPedido.Contains(cargaUtilizada.Codigo))
                            return new JsonpResult(false, true, "Não é possível gerar o redespacho pois nenhum pedido da carga " + cargaUtilizada.CodigoCargaEmbarcador + " foi selecionado. Selecione um pedido desta carga, ou remova-a do campo de cargas");
                    }
                }

                if (cargaPedidos.Count <= 0)
                    return new JsonpResult(false, true, "É obrigatório informar ao menos um pedido");

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                if (!(tipoOperacao?.PermitirGerarRedespacho ?? true))
                    return new JsonpResult(false, true, "O tipo de operação da carga não permite gerar redespacho");

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.Redespacho redespacho = new Dominio.Entidades.Embarcador.Cargas.Redespacho();

                redespacho.NumeroRedespacho = repRedespacho.BuscarProximoCodigo();
                redespacho.Carga = cargas.FirstOrDefault();
                redespacho.CargasUtilizadas = cargas;
                redespacho.TipoOperacao = tipoOperacao;
                redespacho.TipoRedespacho = (tipoOperacao?.Reentrega ?? false) ? Request.GetEnumParam<TipoRedespacho>("TipoRedespacho") : TipoRedespacho.Redespacho;
                redespacho.Expedidor = await repCliente.BuscarPorCPFCNPJAsync(expedidor);
                redespacho.DataRedespacho = DateTime.Now;
                redespacho.Distancia = Request.GetDecimalParam("Distancia");
                redespacho.Recebedor = recebedor > 0 ? await repCliente.BuscarPorCPFCNPJAsync(recebedor) : null;

                await repRedespacho.InserirAsync(redespacho, Auditado);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    if (cargaPedido.PedidoEncaixado)
                        throw new ControllerException($"Não é possível gerar o redespacho do pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} pois ele é um pedido que deve ser encaixado ou ter uma carga dedicada.");

                    if (cargaPedido.Expedidor != null && redespacho.Expedidor != null && redespacho.Expedidor.CPF_CNPJ == cargaPedido.Expedidor.CPF_CNPJ)

                        throw new ControllerException($"Não é possível gerar o redespacho do pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} pois o expedidor informado é o mesmo informado para a viagem.");

                    if (
                        cargaPedido.CargaRedespacho != null &&
                        cargaPedido.CargaRedespacho.Expedidor != null &&
                        cargaPedido.CargaRedespacho.CargaGerada.SituacaoCarga != SituacaoCarga.Cancelada &&
                        cargaPedido.CargaRedespacho.CargaGerada.SituacaoCarga != SituacaoCarga.Anulada &&
                        !cargaPedido.Pedido.ReentregaSolicitada &&
                        !(redespacho.Carga.TipoOperacao?.PermitirGerarRecorrenciaRedespacho ?? false)
                    )
                        throw new ControllerException($"Não é possível gerar o redespacho do pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} pois ele já foi redespachado pelo expedidor {cargaPedido.CargaRedespacho.Expedidor.Descricao} no redespacho de número {cargaPedido.CargaRedespacho.NumeroRedespacho}.");


                    if (cargaPedido.Recebedor != null && redespacho.Recebedor != null && redespacho.Recebedor.CPF_CNPJ == cargaPedido.Recebedor.CPF_CNPJ)

                        throw new ControllerException($"Não é possível gerar o redespacho do pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} pois o recebedor informado é o mesmo informado para a viagem.");

                    if (
                        cargaPedido.CargaRedespacho != null &&
                        cargaPedido.CargaRedespacho.Recebedor != null &&
                        cargaPedido.CargaRedespacho.CargaGerada.SituacaoCarga != SituacaoCarga.Cancelada &&
                        cargaPedido.CargaRedespacho.CargaGerada.SituacaoCarga != SituacaoCarga.Anulada &&
                        !cargaPedido.Pedido.ReentregaSolicitada &&
                        !(redespacho.Carga.TipoOperacao?.PermitirGerarRecorrenciaRedespacho ?? false)
                    )
                        throw new ControllerException($"Não é possível gerar o redespacho do pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} pois ele já foi redespachado pelo recebedor {cargaPedido.CargaRedespacho.Recebedor.Descricao} no redespacho de número {cargaPedido.CargaRedespacho.NumeroRedespacho}.");


                    if (cargaPedido.CargaPedidoProximoTrecho != null)
                    {
                        if (cargaPedido.CargaPedidoProximoTrecho.Carga.SituacaoCarga != SituacaoCarga.Cancelada && cargaPedido.CargaPedidoProximoTrecho.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                            throw new ControllerException($"Não é possível gerar o redespacho do pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} pois ele já foi possui um segundo trecho gerado para o expedidor {(cargaPedido.CargaPedidoProximoTrecho.Expedidor?.Descricao ?? "")}.");

                        if (cargaPedido.PendenteGerarCargaDistribuidor)
                            throw new ControllerException($"Não é possível gerar o redespacho do pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} pois no momento já está sendo gerada uma carga de segundo trecho para ele.");
                    }
                    cargaPedido.CargaRedespacho = redespacho;
                    await repCargaPedido.AtualizarAsync(cargaPedido);
                }

                redespacho.CargaGerada = Servicos.Embarcador.Carga.CargaDistribuidor.GerarCargaProximoTrecho(redespacho.Carga, redespacho.TipoOperacao, redespacho.Distancia, true, redespacho.Expedidor, cargaPedidos, null, configuracaoTMS, false, redespacho, null, TipoServicoMultisoftware, unitOfWork, recebedor: redespacho.Recebedor);

                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork).BuscarPorTipo(TipoIntegracao.TelhaNorte);
                if (redespacho.CargaGerada != null && tipoIntegracao != null)
                {
                    if (redespacho.CargaGerada.Carregamento == null)
                    {
                        Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento
                        {
                            DataCriacao = DateTime.Now,
                            DataCarregamentoCarga = redespacho.CargaGerada.DataCarregamentoCarga,
                            SituacaoCarregamento = SituacaoCarregamento.Fechado,
                            TipoMontagemCarga = TipoMontagemCarga.NovaCarga,
                            AutoSequenciaNumero = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork).ObterProximoCodigoCarregamento(),
                            TipoOperacao = tipoOperacao,
                            TipoDeCarga = redespacho.CargaGerada.TipoDeCarga,
                            ModeloVeicularCarga = redespacho.Carga.ModeloVeicularCarga,
                            Empresa = redespacho.Carga.Empresa,
                            PesoCarregamento = cargaPedidos.Select(obj => obj.Peso).Sum(),
                            ValorFrete = redespacho.CargaGerada.ValorFrete
                        };

                        carregamento.NumeroCarregamento = carregamento.AutoSequenciaNumero.ToString();

                        await repositorioCarregamento.InserirAsync(carregamento);

                        redespacho.CargaGerada.Carregamento = carregamento;

                        await repCarga.AtualizarAsync(redespacho.CargaGerada);
                    }
                    Servicos.Embarcador.Integracao.IntegracaoCarregamento servicoIntegracaoCarregamento = new Servicos.Embarcador.Integracao.IntegracaoCarregamento(unitOfWork);
                    servicoIntegracaoCarregamento.AdicionarIntegracaoCarregamento(redespacho.CargaGerada.Carregamento, StatusCarregamentoIntegracao.Inserir, TipoIntegracao.TelhaNorte);
                }

                await repRedespacho.AtualizarAsync(redespacho);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (ServicoException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Redespacho repRedespacho = new Repositorio.Embarcador.Cargas.Redespacho(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº Redespacho", "NumeroRedespacho", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Carga Redespacho", "CargaRedespacho", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Carga", "CargasUtilizadasFormatada", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data", "DataRedespachoFormatada", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Expedidor", "ExpedidorFormatada", 20, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Carga.Redespacho.FiltroPesquisaRedespacho filtroPesquisa = ObterFiltrosPesquisaRedespacho(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarOuAgrupar);

                IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Redespacho.Redespacho> redespachos = await repRedespacho.ConsultarAsync(filtroPesquisa, parametrosConsulta);

                int quantidadeTotalLinhas = await repRedespacho.ContarConsultaAsync(filtroPesquisa);

                grid.setarQuantidadeTotal(quantidadeTotalLinhas);
                grid.AdicionaRows(redespachos);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar redespacho (Pesquisa).");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ObterExpedidorCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Cliente cliente = null;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    cliente = repositorioCliente.BuscarPorCPFCNPJ(Empresa.CNPJ.ToDouble());

                return new JsonpResult(new
                {
                    CNPJExpedidor = cliente?.CPF_CNPJ ?? 0,
                    Expedidor = cliente?.Descricao ?? string.Empty,
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o Expedidor.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.Redespacho.FiltroPesquisaRedespacho ObterFiltrosPesquisaRedespacho(Repositorio.UnitOfWork unitOfWork)
        {
            double expedidor = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                expedidor = this.Usuario.Empresa.CNPJ.ToDouble();
            else
                double.TryParse(Request.Params("Expedidor"), out expedidor);

            Dominio.ObjetosDeValor.Embarcador.Carga.Redespacho.FiltroPesquisaRedespacho filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.Redespacho.FiltroPesquisaRedespacho()
            {
                CodigoExpedidor = expedidor,
                NumeroRedespacho = Request.GetIntParam("NumeroRedespacho"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataFim = Request.GetNullableDateTimeParam("DataFim"),
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigosFilial = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork),
                CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork)
            };
            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        private dynamic ExecutaPesquisaDocumento(ref List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaGrid, ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            int carga;
            int.TryParse(Request.Params("Carga"), out carga);

            List<int> cargasUtilizadas = Request.GetListParam<int>("Cargas");

            int codigo;
            int.TryParse(Request.Params("Codigo"), out codigo);

            if (codigo > 0)
                carga = 0;

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

            double expedidor;
            double.TryParse(Request.Params("Expedidor"), out expedidor);

            int tipoOperacao = 0;
            int.TryParse(Request.Params("TipoOperacao"), out tipoOperacao);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoEntidade = repTipoOperacao.BuscarPorCodigo(tipoOperacao);

            if (tipoOperacaoEntidade?.Expedidor != null)
                expedidor = 0;

            string estadoDestino = "";

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXmlNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaOrigem(carga);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesLiberadas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>();
            List<int> codigosFiliais = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();
            listaGrid = repCargaPedido.Consultar(situacoesLiberadas, carga, somentePendentes, numeroNF, numeroCTe, CodigoPedidoEmbarcador, CodigoCargaEmbarcador, origem, destino, filial, remetente, destinatario, expedidor, codigo, false, !configuracaoGeralCarga.GerarRedespachoDeCargasAgrupadas, estadoDestino, true, codigosFiliais, propOrdenar, dirOrdena, inicio, limite, cargasUtilizadas);
            totalRegistros = repCargaPedido.ContarConsulta(situacoesLiberadas, carga, somentePendentes, numeroNF, numeroCTe, CodigoPedidoEmbarcador, CodigoCargaEmbarcador, origem, destino, filial, remetente, destinatario, expedidor, codigo, false, !configuracaoGeralCarga.GerarRedespachoDeCargasAgrupadas, estadoDestino, true, codigosFiliais, cargasUtilizadas);

            //listaGrid = listaGrid.Where(obj => obj.Recebedor == null || obj.Recebedor.ClienteDescargas.Any(x => x.FilialResponsavelRedespacho == null || x.FilialResponsavelRedespacho.Codigo == obj.CargaOrigem.Filial.Codigo || x.FilialResponsavelRedespacho.Codigo == obj.Carga.Filial.Codigo)).ToList();

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
                                     NotasFiscais = string.Join(",", (from nf in pedidosXmlNotasFiscais where nf.CargaPedido.Codigo == obj.Codigo select nf.XMLNotaFiscal.Numero)),
                                     Carga = obj.Carga.CodigoCargaEmbarcador
                                 }).ToList();

            return dynListaCarga;
        }

        private dynamic ObterRedespacho(Dominio.Entidades.Embarcador.Cargas.Redespacho redespacho, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TransbordoMDFe repTransbordoMDFe = new Repositorio.Embarcador.Cargas.TransbordoMDFe(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Servicos.Embarcador.Terceiros.ContratoFrete serContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            int numeroMDFe = repTransbordoMDFe.ContarConsulta(redespacho.Codigo);

            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFreteTerceiro.BuscarPorTransbordo(redespacho.Codigo);

            var dynTransbordo = new
            {
                redespacho.Codigo,
                Carga = new
                {
                    redespacho.Carga.Codigo,
                    Descricao = redespacho.Carga.CodigoCargaEmbarcador
                },
                Cargas = (from carga in redespacho.CargasUtilizadas
                          select new
                          {
                              carga.Codigo,
                              Descricao = carga.CodigoCargaEmbarcador
                          }).ToList(),
                DataRedespacho = redespacho.DataRedespacho.ToString("dd/MM/yyyy HH:mm:ss"),
                redespacho.NumeroRedespacho,
                redespacho.TipoRedespacho,
                TipoOperacao = new
                {
                    Codigo = redespacho.TipoOperacao?.Codigo ?? 0,
                    Descricao = redespacho.TipoOperacao?.Descricao ?? "",
                    Reentrega = redespacho.TipoOperacao?.Reentrega ?? false
                },
                Expedidor = new
                {
                    Codigo = redespacho.Expedidor?.CPF_CNPJ ?? 0,
                    Descricao = redespacho.Expedidor?.Descricao ?? ""
                },
                Distancia = redespacho.Distancia > 0m ? redespacho.Distancia.ToString("n4") : "",
                Recebedor = new
                {
                    Codigo = redespacho.Recebedor?.CPF_CNPJ ?? 0,
                    Descricao = redespacho.Recebedor?.Descricao ?? " "
                }
            };

            return dynTransbordo;
        }

        #endregion
    }
}
