using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Produtos
{
    [AllowAuthenticate]
    public class RetiradaProdutoController : BaseController
    {
        #region Construtores

        public RetiradaProdutoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repRetiradaProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento retiradaProduto = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento();

                unitOfWork.Start();

                var dados = PreencherObjetoDados(unitOfWork);
                PreencherEntidade(retiradaProduto, dados, unitOfWork);
                repRetiradaProduto.Inserir(retiradaProduto, Auditado);
                SetarPedidos(retiradaProduto, dados, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    retiradaProduto.Codigo
                });
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(true, false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repRetiradaProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Repositorio.Embarcador.Pedidos.NotificacaoRetiradaProduto repNotificacaoRetiradaProduto = new Repositorio.Embarcador.Pedidos.NotificacaoRetiradaProduto(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                int codigoCarregamento = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Pedidos.NotificacaoRetiradaProduto notificacaoRetiradaProduto = repNotificacaoRetiradaProduto.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repRetiradaProduto.BuscarPorCodigo(codigoCarregamento, auditavel: true);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorFilial(carregamento.Filial?.Codigo ?? 0);

                Servicos.Embarcador.Logistica.CentroCarregamento servicoCentroCarregamento = new Servicos.Embarcador.Logistica.CentroCarregamento(unitOfWork);

                if (carregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (carregamento.SituacaoCarregamento == SituacaoCarregamento.Bloqueado || carregamento.SituacaoCarregamento == SituacaoCarregamento.FalhaIntegracao)
                    return new JsonpResult(false, true, "Não foi possível atualizar o registro.");

                if (!carregamento.CarregamentoIntegradoERP)
                    ProcessarGeracaoCarga(carregamento, unitOfWork);
                else
                    ProcessarAtualizacaoCarga(carregamento, unitOfWork, configuracaoGeralCarga);

                EnviarEmailNotificacaoRetiradaProduto(notificacaoRetiradaProduto, unitOfWork);

                if ((centroCarregamento?.EnviarEmailConfirmacaoAgendamentoSomenteQuandoSituacaoAgendamentoForFinalizado ?? false) && (carregamento.SituacaoCarregamento != SituacaoCarregamento.Bloqueado) && (carregamento.Filial != null))
                {
                    return new JsonpResult(new
                    {
                        carregamento.SituacaoCarregamento
                    });
                }

                if (carregamento.SituacaoCarregamento != SituacaoCarregamento.FalhaIntegracao)
                {
                    Task.Factory.StartNew(() =>
                    {
                        using (Repositorio.UnitOfWork unitOfWorkEmail = new Repositorio.UnitOfWork(unitOfWork.StringConexao))
                            servicoCentroCarregamento.EnviarNotificacaoConfirmacaoCarga(carregamento.Codigo, TipoServicoMultisoftware, Cliente.Codigo, _conexao.AdminStringConexao, unitOfWorkEmail);

                    });
                }

                return new JsonpResult(new
                {
                    carregamento.SituacaoCarregamento
                });
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

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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

                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repRetiradaProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento retiradaProduto = repRetiradaProduto.BuscarPorCodigo(codigo);

                if (retiradaProduto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(ObterMontagemCarga(retiradaProduto, unitOfWork, this.ConfiguracaoEmbarcador));
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

        public async Task<IActionResult> BuscarPedidoPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                bool duplicar = Request.GetBoolParam("Duplicar");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoAutorizacao repPedidoAutorizacao = new Repositorio.Embarcador.Pedidos.PedidoAutorizacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoCancelamento repPedidoCancelamento = new Repositorio.Embarcador.Pedidos.PedidoCancelamento(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens repPassagemPercursoEstado = new Repositorio.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoProdutoONU repPedidoProdutoONU = new Repositorio.Embarcador.Pedidos.PedidoProdutoONU(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoFronteira repositorioPedidoFronteira = new Repositorio.Embarcador.Pedidos.PedidoFronteira(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> pedidoOcorrenciasColetaEntrega = repPedidoOcorrenciaColetaEntrega.BuscarPorPedido(codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens> passagensPercursoEstado = repPassagemPercursoEstado.BuscarPorPedido(codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU> pedidoProdutoONU = repPedidoProdutoONU.BuscarPorPedido(codigo);
                List<Dominio.Entidades.Cliente> listaFronteiras = await repositorioPedidoFronteira.BuscarFronteirasPorPedidoAsync(codigo);

                Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao autorizacao = repPedidoAutorizacao.BuscarAutorizacaoPedido(codigo);
                Dominio.Entidades.Embarcador.Pedidos.PedidoCancelamento cancelamento = repPedidoCancelamento.BuscarPorPedido(codigo);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigo);

                Dominio.Entidades.Veiculo veiculo = pedido.VeiculoTracao ?? pedido.Veiculos.FirstOrDefault();

                Dominio.Entidades.Usuario veiculoMotorista = null;
                if (veiculo != null)
                    veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                var dynPedido = new
                {
                    Codigo = duplicar ? 0 : pedido.Codigo,
                    pedido.ColetaEmProdutorRural,
                    CamposSecundariosObrigatoriosPedido = pedido.TipoOperacao?.CamposSecundariosObrigatoriosPedido ?? false,
                    Rota = new { Codigo = pedido.RotaFrete?.Codigo ?? 0, Descricao = pedido.RotaFrete?.Descricao ?? string.Empty },
                    pedido.CodigoCargaEmbarcador,
                    pedido.ObservacaoEmissaRemetente,
                    pedido.ObservacaoEmissaoDestinatario,
                    DataCarregamentoPedido = pedido.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    DataColeta = pedido.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    TipoPessoa = pedido.TipoPessoa,
                    DataFinalColeta = pedido.DataFinalColeta?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    DataInicialColeta = pedido.DataInicialColeta?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    Terceiro = pedido.Terceiro != null ? new { Codigo = pedido.Terceiro.CPF_CNPJ, Descricao = pedido.Terceiro.Descricao } : null,
                    Origem = pedido.Origem != null ? new { pedido.Origem.Codigo, Descricao = pedido.Origem.DescricaoCidadeEstado } : null,
                    Destino = pedido.Destino != null ? new { Codigo = pedido.Destino.Codigo, Descricao = pedido.Destino.DescricaoCidadeEstado } : null,
                    Destinatario = new { Codigo = pedido.Destinatario?.CPF_CNPJ ?? 0d, Descricao = pedido.Destinatario?.Descricao ?? "" },
                    Remetente = new { Codigo = pedido.Remetente?.CPF_CNPJ ?? 0d, Descricao = pedido.Remetente?.Descricao ?? "" },
                    Expedidor = new { Codigo = pedido.Expedidor?.CPF_CNPJ ?? 0d, Descricao = pedido.Expedidor?.Descricao ?? "", codigoIBGE = pedido.Expedidor?.Localidade?.CodigoIBGE.ToString() ?? "" },
                    Recebedor = new { Codigo = pedido.Recebedor?.CPF_CNPJ ?? 0d, Descricao = pedido.Recebedor?.Descricao ?? "", codigoIBGE = pedido.Recebedor?.Localidade?.CodigoIBGE.ToString() ?? "" },
                    Filial = pedido.Filial != null ? new { pedido.Filial.Codigo, pedido.Filial.Descricao } : null,
                    Empresa = pedido.Empresa != null ? new { pedido.Empresa.Codigo, pedido.Empresa.Descricao } : null,
                    Serie = pedido.EmpresaSerie != null ? new { Codigo = pedido.EmpresaSerie != null ? pedido.EmpresaSerie.Codigo : 0, Descricao = pedido.EmpresaSerie != null ? pedido.EmpresaSerie.Numero.ToString() : "" } : null,
                    TotalPallets = (pedido.NumeroPaletes + pedido.NumeroPaletesFracionado).ToString("n3"),
                    PalletsFracionado = pedido.NumeroPaletesFracionado > 0 ? pedido.NumeroPaletesFracionado.ToString("n3") : "",
                    pedido.NumeroPedidoEmbarcador,
                    Observacao = pedido.Observacao != null ? pedido.Observacao : "",
                    pedido.ProdutoPredominante,
                    PesoTotalCarga = pedido.PesoTotal > 0 ? pedido.PesoTotal.ToString("n2") : "",
                    CubagemTotal = pedido.CubagemTotal > 0 ? pedido.CubagemTotal.ToString("n2") : "",
                    PrevisaoEntrega = pedido.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    DataCarregamento = pedido.DataCarregamentoCarga?.ToDateTimeString() ?? string.Empty,
                    GrupoPessoa = pedido.GrupoPessoas != null ? new { Codigo = pedido.GrupoPessoas.Codigo, Descricao = pedido.GrupoPessoas.Descricao } : null,
                    TipoOperacao = new { Codigo = pedido.TipoOperacao?.Codigo ?? 0, Descricao = pedido.TipoOperacao?.Descricao ?? "", UtilizarDeslocamentoPedido = pedido.TipoOperacao?.UtilizarDeslocamentoPedido ?? false, DestinatarioObrigatorio = !pedido.TipoOperacao?.PermiteGerarPedidoSemDestinatario ?? true },
                    OperacaoDeImportacaoExportacao = pedido.TipoOperacao != null ? pedido.TipoOperacao.OperacaoDeImportacaoExportacao : false,
                    NaoExigeVeiculoParaEmissao = pedido.TipoOperacao?.NaoExigeVeiculoParaEmissao ?? false,
                    ListaReboques = ConfiguracaoEmbarcador.PermitirSelecionarReboquePedido ? pedido.Veiculos.Select(o => new { o.Codigo, o.Placa, ModeloVeicular = o.ModeloVeicularCarga?.Descricao ?? string.Empty }).ToList() : null,
                    ProdutoEmbarcador = pedido.Produtos != null && pedido.Produtos.Count > 0 ? new { Codigo = pedido.Produtos.First().Produto.Codigo, Descricao = pedido.Produtos.First().Produto.Descricao } : null,
                    pedido.GerarUmCTEPorNFe,
                    SituacaoPedido = duplicar ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto : pedido.SituacaoPedido,
                    DescricaoEmpresa = pedido.Empresa?.Descricao ?? "",
                    TipoColeta = pedido.TipoColeta != null ? new { Codigo = pedido.TipoColeta != null ? pedido.TipoColeta.Codigo : 0, Descricao = pedido.TipoColeta != null ? pedido.TipoColeta.Descricao : "" } : null,
                    TipoCarga = pedido.TipoDeCarga != null ? new { Codigo = pedido.TipoDeCarga != null ? pedido.TipoDeCarga.Codigo : 0, Descricao = pedido.TipoDeCarga != null ? pedido.TipoDeCarga.Descricao : "" } : null,
                    ModeloVeicularCarga = pedido.ModeloVeicularCarga != null ? new { Codigo = pedido.ModeloVeicularCarga.Codigo, Descricao = pedido.ModeloVeicularCarga.Descricao } : new { Codigo = 0, Descricao = "" },
                    pedido.TipoTomador,
                    pedido.UsarTipoTomadorPedido,
                    UsarOutroEnderecoOrigem = pedido.UsarOutroEnderecoOrigem,
                    LocalidadeClienteOrigem = pedido.EnderecoOrigem != null ? new { Codigo = (pedido.EnderecoOrigem != null && pedido.EnderecoOrigem.ClienteOutroEndereco != null) ? pedido.EnderecoOrigem.ClienteOutroEndereco.Codigo : 0, Descricao = (pedido.EnderecoOrigem != null && pedido.EnderecoOrigem.ClienteOutroEndereco != null) ? pedido.EnderecoOrigem.ClienteOutroEndereco.Localidade.DescricaoCidadeEstado : "" } : null,
                    BairroOrigem = pedido.EnderecoOrigem != null ? pedido.EnderecoOrigem.Bairro : "",
                    CEPOrigem = pedido.EnderecoOrigem != null ? pedido.EnderecoOrigem.CEP : "",
                    NumeroOrigem = pedido.EnderecoOrigem != null ? pedido.EnderecoOrigem.Numero : "",
                    ComplementoOrigem = pedido.EnderecoOrigem != null ? pedido.EnderecoOrigem.Complemento : "",
                    EnderecoOrigem = pedido.EnderecoOrigem != null ? pedido.EnderecoOrigem.Endereco : "",
                    Telefone1Origem = pedido.EnderecoOrigem != null ? pedido.EnderecoOrigem.Telefone : "",
                    RGIE1Origem = pedido.EnderecoDestino != null ? pedido.EnderecoDestino.IE_RG : "",
                    CidadePoloOrigem = pedido.Origem != null ? new { Codigo = pedido.Origem.LocalidadePolo != null ? pedido.Origem.LocalidadePolo.Codigo : 0, Descricao = pedido.Origem.LocalidadePolo != null ? pedido.Origem.LocalidadePolo.DescricaoCidadeEstado : "" } : null,
                    PaisOrigem = pedido.Origem != null ? pedido.Origem.Pais != null ? pedido.Origem.Pais.Nome : "" : "",
                    IBGEOrigem = pedido.Origem != null ? pedido.Origem.CodigoIBGE : 0,
                    UFOrigem = pedido.Origem != null ? pedido.Origem.Estado.Sigla : "",
                    UsarOutroEnderecoDestino = pedido.UsarOutroEnderecoDestino,
                    LocalidadeClienteDestino = pedido.EnderecoDestino != null ? new { Codigo = (pedido.EnderecoDestino != null && pedido.EnderecoDestino.ClienteOutroEndereco != null) ? pedido.EnderecoDestino.ClienteOutroEndereco.Codigo : 0, Descricao = (pedido.EnderecoDestino != null && pedido.EnderecoDestino.ClienteOutroEndereco != null) ? pedido.EnderecoDestino.ClienteOutroEndereco.Localidade.DescricaoCidadeEstado : "" } : null,
                    BairroDestino = pedido.EnderecoDestino != null ? pedido.EnderecoDestino.Bairro : "",
                    CEPDestino = pedido.EnderecoDestino != null ? pedido.EnderecoDestino.CEP : "",
                    NumeroDestino = pedido.EnderecoDestino != null ? pedido.EnderecoDestino.Numero : "",
                    ComplementoDestino = pedido.EnderecoDestino != null ? pedido.EnderecoDestino.Complemento : "",
                    EnderecoDestino = pedido.EnderecoDestino != null ? pedido.EnderecoDestino.Endereco : "",
                    Telefone1Destino = pedido.EnderecoDestino != null ? pedido.EnderecoDestino.Telefone : "",
                    RGIE1Destino = pedido.EnderecoDestino != null ? pedido.EnderecoDestino.IE_RG : "",
                    CidadePoloDestino = pedido.Destino != null ? new { Codigo = pedido.Destino.LocalidadePolo != null ? pedido.Destino.LocalidadePolo.Codigo : 0, Descricao = pedido.Destino.LocalidadePolo != null ? pedido.Destino.LocalidadePolo.DescricaoCidadeEstado : "" } : null,
                    PaisDestino = pedido.Destino != null && pedido.Destino.Pais != null ? pedido.Destino.Pais.Nome : "",
                    IBGEDestino = pedido.Destino != null ? pedido.Destino.CodigoIBGE : 0,
                    UFDestino = pedido.Destino != null ? pedido.Destino.Estado.Sigla : "",
                    ClienteDeslocamento = new { Codigo = pedido.ClienteDeslocamento?.CPF_CNPJ ?? 0d, Descricao = pedido.ClienteDeslocamento?.Descricao ?? "" },
                    pedido.NumeroBL,
                    pedido.NumeroNavio,
                    Porto = pedido.Porto?.Descricao,
                    CodigoPorto = pedido.Porto?.Codigo,
                    TipoTerminalImportacao = pedido.TipoTerminalImportacao?.Descricao,
                    CodigoTipoTerminalImportacao = pedido.TipoTerminalImportacao?.Codigo,
                    pedido.EnderecoEntregaImportacao,
                    pedido.BairroEntregaImportacao,
                    pedido.CEPEntregaImportacao,
                    LocalidadeEntregaImportacao = pedido.LocalidadeEntregaImportacao?.Descricao,
                    CodigoLocalidadeEntregaImportacao = pedido.LocalidadeEntregaImportacao?.Codigo,
                    DataVencimentoArmazenamentoImportacao = pedido.DataVencimentoArmazenamentoImportacao.HasValue ? pedido.DataVencimentoArmazenamentoImportacao.Value.ToString("dd/MM/yyyy") : string.Empty,
                    pedido.ArmadorImportacao,
                    pedido.DescricaoSituacaoPedido,
                    pedido.DescricaoEtapaPedido,
                    NomesMotoristas = pedido.NomeMotoristas,
                    NumeroPedido = duplicar ? "" : TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ? (pedido.Numero > 0 ? pedido.Numero.ToString() : string.Join(", ", pedido.CargasPedido.Select(o => o.CodigoCargaEmbarcador).Distinct())) : pedido.CodigoCargaEmbarcador,

                    Navio = pedido.Navio?.Descricao ?? "",
                    CodigoNavio = pedido.Navio?.Codigo ?? 0,

                    PortoDestino = pedido.PortoDestino?.Descricao ?? "",
                    CodigoPortoDestino = pedido.PortoDestino?.Codigo ?? 0,

                    TerminalOrigem = pedido.TerminalOrigem?.Descricao ?? "",
                    CodigoTerminalOrigem = pedido.TerminalOrigem?.Codigo ?? 0,

                    TerminalDestino = pedido.TerminalDestino?.Descricao ?? "",
                    CodigoTerminalDestino = pedido.TerminalDestino?.Codigo ?? 0,

                    DirecaoViagemMultimodal = pedido.DirecaoViagemMultimodal,

                    Container = pedido.Container?.Descricao ?? "",
                    CodigoContainer = pedido.Container?.Codigo ?? 0,

                    ContainerTipoReserva = pedido.ContainerTipoReserva?.Descricao ?? "",
                    CodigoContainerTipoReserva = pedido.ContainerTipoReserva?.Codigo ?? 0,

                    LacreContainerUmMultimodal = pedido.LacreContainerUm,
                    LacreContainerDoisMultimodal = pedido.LacreContainerDois,
                    LacreContainerTresMultimodal = pedido.LacreContainerTres,
                    TaraContainerMultimodal = pedido.TaraContainer,

                    CodigoMotoristaVeiculo = veiculoMotorista?.Codigo ?? 0,
                    NomeMotoristaVeiculo = veiculoMotorista?.Nome ?? String.Empty,

                    ListaFronteiras = ObterListaFronteiras(pedido, listaFronteiras),
                    ListaClientes = ObterListaClientes(pedido),
                    ProdutosEmbarcador = ObterListaProdutos(pedido),
                    SituacaoCarregamentoPedido = ObterSituacaoCarregamentoPedido(pedido),
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

        public async Task<IActionResult> ObterHorariosDisponiveis()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime dia = Request.GetDateTimeParam("DataCarregamentoDisponibilidade");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(propriedade: "HoraInicio", visivel: false);
                grid.AdicionarCabecalho(propriedade: "HoraTermino", visivel: false);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Pedidos.RetiradaProduto.Data, propriedade: "Data", tamanho: 30, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Pedidos.RetiradaProduto.DiaSemana, propriedade: "Dia", tamanho: 30, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Pedidos.RetiradaProduto.Horario, propriedade: "Horario", tamanho: 30, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

                int codigoFilial = Request.GetIntParam("Filial");
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorFilial(codigoFilial);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

                Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento
                {
                    CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeiculo"),
                    CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                    CodigoTransportador = Request.GetIntParam("Transportador"),
                };

                if (configuracaoGeralCarga?.AssumirSempreTipoOperacaoDoPedido ?? false)
                    configuracaoDisponibilidadeCarregamento.CodigoTipoOperacao = Request.GetIntParam("TipoOperacaoPedido");

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoDisponibilidadeCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork, configuracaoEmbarcador, configuracaoDisponibilidadeCarregamento);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                {
                    Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                    Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarEmpresaPadraoRetirada();

                    configuracaoDisponibilidadeCarregamento.CodigoTransportador = empresa?.Codigo ?? 0;
                    configuracaoDisponibilidadeCarregamento.CpfCnpjCliente = Usuario?.ClienteFornecedor?.CPF_CNPJ ?? 0d;
                }

                List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> horarios = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>();

                if (centroCarregamento != null)
                {
                    if (configuracaoEmbarcador.PermitirAlterarCargaHorarioCarregamentoInferiorAtual || (dia >= DateTime.Today.AddDays(configuracaoJanelaCarregamento.DiasParaPermitirInformarHorarioCarregamento)))
                        horarios = servicoDisponibilidadeCarregamento.ObterPeriodosCarregamentosDisponiveis(centroCarregamento, dia);
                }

                grid.setarQuantidadeTotal(horarios.Count);
                grid.AdicionaRows((
                    from periodo in horarios
                    select new
                    {
                        periodo.Codigo,
                        Horario = $"{periodo.HoraInicio.ToString(@"hh\:mm")} - {periodo.HoraTermino.ToString(@"hh\:mm")}",
                        periodo.HoraInicio,
                        periodo.HoraTermino,
                        Data = dia.ToDateString(),
                        Dia = DiaSemanaHelper.ObterDescricao(dia)
                    }
                ).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter as informações do centro de carregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarFiliais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = repFilial.ConsultarTodas();

                var retornoFiliais = (
                       from obj in filiais
                       select new
                       {
                           obj.Codigo,
                           obj.Descricao,
                       }
                   ).ToList();

                return new JsonpResult(new
                {
                    Filiais = retornoFiliais,
                });
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

        public async Task<IActionResult> BuscarPedidoProdutos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);

                int codigoFilial = Request.GetIntParam("Filial");

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorFilial(codigoFilial);

                List<int> codigos = Request.GetListParam<int>("Codigos");

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorCodigos(codigos);

                if ((centroCarregamento?.NaoPermitirGerarCarregamentosQuandoExistirPedidosAtrasadosAgendamentoPedidos ?? false) && (centroCarregamento?.DiasAtrasoPermitidosPedidosAgendamentoPedidos ?? 0) > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = ObterFiltrosPesquisaBuscarPedidosProdutos(unitOfWork);
                    filtrosPesquisa.NaoExibirPedidosDoDia = configuracaoPedido.NaoExibirPedidosDoDiaAgendamentoPedidos;

                    bool possuiPedidoAtrasado = repositorioPedido.PossuiPedidosAtrasados(filtrosPesquisa, centroCarregamento);

                    if (possuiPedidoAtrasado && pedidos.Any(p => p.DataCriacao.Value >= DateTime.Now.Date.AddDays(-centroCarregamento.DiasAtrasoPermitidosPedidosAgendamentoPedidos)))
                        return new JsonpResult(false, true, "É necessário que todos os pedidos atrasados sejam agendados antes de selecionar outros pedidos.");
                }

                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> produtos = Servicos.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto.ObterSaldoPedidoProdutos(codigos, unitOfWork);

                return new JsonpResult(new
                {
                    Pedidos = (from obj in pedidos select ObterObjetoProdutoPedidoFormatado(obj, produtos)).ToList(),
                });
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

        public async Task<IActionResult> BuscarTipoOperacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tipoOperacaos = repTipoOperacao.BuscarTipoOperacoesRetirada();

                var retornoTipoOperacaos = (
                       from obj in tipoOperacaos
                       select new
                       {
                           obj.Codigo,
                           obj.Descricao,
                       }
                   ).ToList();

                return new JsonpResult(new
                {
                    TipoOperacaos = retornoTipoOperacaos,
                });
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

        public async Task<IActionResult> BuscarModeloVeiculos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);

                int codigoFilial = Request.GetIntParam("Filial");

                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modeloVeiculos = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = null;
                if (codigoFilial > 0)
                    centroCarregamento = repositorioCentroCarregamento.BuscarPorFilial(codigoFilial);

                if (centroCarregamento != null && (centroCarregamento.TemposCarregamento?.Count ?? 0) > 0)
                    modeloVeiculos = centroCarregamento.TemposCarregamento.Select(t => t.ModeloVeicular).ToList();
                else
                    modeloVeiculos = repModeloVeicularCarga.BuscarTodosAtivos();

                var retornoModeloVeiculos = (
                       from obj in modeloVeiculos
                       select new
                       {
                           obj.Codigo,
                           obj.Descricao,
                           obj.CapacidadePesoTransporte,
                       }
                   ).ToList();

                return new JsonpResult(new
                {
                    ModeloVeiculos = retornoModeloVeiculos,
                });
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

        public async Task<IActionResult> ReenviarIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                DateTime dataCarregamentoCarga = Request.GetDateTimeParam("DataRetirada");

                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repCarregamento.BuscarPorCodigo(codigo, true);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorFilial(carregamento.Filial?.Codigo ?? 0);

                Servicos.Embarcador.Logistica.CentroCarregamento servicoCentroCarregamento = new Servicos.Embarcador.Logistica.CentroCarregamento(unitOfWork);

                if (carregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (carregamento.SituacaoCarregamento != SituacaoCarregamento.FalhaIntegracao)
                    return new JsonpResult(false, true, "A situação do agendamento não permite o reenvio da integração.");

                if (dataCarregamentoCarga == DateTime.MinValue)
                    return new JsonpResult(false, true, "É obrigatório escolher a data e hora do agendamento");

                if (IntegrarCriacaoSaintgobain(carregamento, unitOfWork))
                {
                    unitOfWork.Start();

                    carregamento.DataCarregamentoCarga = dataCarregamentoCarga;
                    repCarregamento.Atualizar(carregamento);

                    ControleExibicaoCargaGerada(carregamento, true, unitOfWork);
                    AtualizarDataCarregamento(carregamento, unitOfWork);

                    unitOfWork.CommitChanges();

                    if ((centroCarregamento?.EnviarEmailConfirmacaoAgendamentoSomenteQuandoSituacaoAgendamentoForFinalizado ?? false) && (carregamento.SituacaoCarregamento != SituacaoCarregamento.Bloqueado) && (carregamento.Filial != null))
                    {
                        return new JsonpResult(new
                        {
                            carregamento.SituacaoCarregamento
                        });
                    }

                    Task t = Task.Factory.StartNew(() =>
                    {
                        using (Repositorio.UnitOfWork unitOfWorkEmail = new Repositorio.UnitOfWork(unitOfWork.StringConexao))
                            servicoCentroCarregamento.EnviarNotificacaoConfirmacaoCarga(carregamento.Codigo, TipoServicoMultisoftware, Cliente.Codigo, _conexao.AdminStringConexao, unitOfWorkEmail);
                    });
                }

                return new JsonpResult(new
                {
                    carregamento.SituacaoCarregamento
                });
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao reenviar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirAgendamentosPendentes()
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork, somenteAgendamentosPendentes: true);
                Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagem = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                List<int> codigosCarregamentos = repositorioCarregamento.ConsultarCodigos(filtrosPesquisa);
                int totalAgendamentosPendentes = codigosCarregamentos.Count;
                int totalAgendamentosPendentesExcluidos = 0;

                foreach (int codigoCarregamento in codigosCarregamentos)
                {
                    try
                    {
                        unitOfWork.Start();

                        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repositorioCarregamento.BuscarPorCodigo(codigoCarregamento);

                        if (carregamento.CarregamentoIntegradoERP)
                            IntegrarCancelamentoSaintgobain(carregamento, unitOfWork);

                        bool possuiCargaGerada = CancelarAgendamentoCarregamento(carregamento, unitOfWork);

                        ControleExibicaoCargaGerada(carregamento, visivel: true, unitOfWork);

                        if (!possuiCargaGerada)
                        {
                            bool valida = false;
                            string mensagemErro = "";

                            if (!servicoMontagem.CancelarCarregamentos(new List<int>() { carregamento.Codigo }, ref valida, ref mensagemErro, this.Usuario, unitOfWork))
                                throw new ControllerException(mensagemErro);
                        }

                        unitOfWork.CommitChanges();
                        unitOfWork.FlushAndClear();

                        totalAgendamentosPendentesExcluidos++;
                    }
                    catch (BaseException excecao)
                    {
                        Servicos.Log.TratarErro($"Falha ao remover o agendamento do carregamento {codigoCarregamento}: {excecao.Message}");
                        unitOfWork.Rollback();
                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro(excecao);
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, excecao.Message);
                    }
                }

                return new JsonpResult(new
                {
                    TotalAgendamentosPendentes = totalAgendamentosPendentes,
                    TotalAgendamentosPendentesExcluidos = totalAgendamentosPendentesExcluidos,
                    TotalAgendamentosPendentesRestantes = (totalAgendamentosPendentes - totalAgendamentosPendentesExcluidos)
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover os agendamentos pendentes.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repositorioCarregamento.BuscarPorCodigo(codigo, true);

                if (carregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (carregamento.CarregamentoIntegradoERP)
                    IntegrarCancelamentoSaintgobain(carregamento, unitOfWork);

                Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);

                unitOfWork.Start();

                bool possuiCargaGerada = CancelarAgendamentoCarregamento(carregamento, unitOfWork);

                ControleExibicaoCargaGerada(carregamento, true, unitOfWork);

                if (!possuiCargaGerada)
                {
                    bool valida = false;
                    string mensagemErro = "";

                    if (!servicoMontagemCarga.CancelarCarregamentos(new List<int>() { carregamento.Codigo }, ref valida, ref mensagemErro, true, this.Usuario, unitOfWork))
                        throw new ControllerException(mensagemErro);
                }

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
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> BuscarAgendamentos()
        {
            try
            {
                return new JsonpResult(ObterGridAgendamentos());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTotalAgendamentosPendentes(CancellationToken cancellationToken)
        {
            int totalAgendamentosPendentes = 0;

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                return new JsonpResult(totalAgendamentosPendentes);

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork, somenteAgendamentosPendentes: true);
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork, cancellationToken);

                totalAgendamentosPendentes = await repositorioCarregamento.ContarConsultaAsync(filtrosPesquisa);

                return new JsonpResult(totalAgendamentosPendentes);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter o total de agendamentos pendentes.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ValidarInformacoesPorCentro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilial = Request.GetIntParam("Filial");

                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorFilial(codigoFilial);

                dynamic dyn = new
                {
                    NaoPermitirAgendarCargasNoMesmoDia = (centroCarregamento?.NaoPermitirAgendarCargasNoMesmoDia ?? false),
                    DataCarregamentoDisponibilidade = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy")
                };

                return new JsonpResult(dyn);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter as informações do centro de carregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterObjetoProdutoPedidoFormatado(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> produtos)
        {
            Dominio.Entidades.Cliente clientePedidoReferencia = pedido.Destinatario;

            return new
            {
                pedido.Codigo,
                PesoTotal = produtos.Where(x => x.CodigoPedido == pedido.Codigo && x.Qtde > 0).Sum(x => x.SaldoQtde * (x.Peso / x.Qtde)),
                pedido.NumeroPedidoEmbarcador,
                NomeCliente = clientePedidoReferencia?.Nome ?? string.Empty,
                CnpjCliente = clientePedidoReferencia?.CPF_CNPJ,
                Endereco = clientePedidoReferencia?.Endereco ?? string.Empty,
                CodigoTipoOperacao = pedido.TipoOperacao.Codigo,
                CidadeUf = clientePedidoReferencia?.Localidade?.DescricaoCidadeEstado ?? string.Empty,
                Produtos = (
                    from objProduto in (from prodPedido in produtos where prodPedido.CodigoPedido == pedido.Codigo select prodPedido)
                    where objProduto.SaldoQtde > 0
                    select new
                    {
                        Codigo = objProduto.CodigoProduto,
                        CodigoPedidProduo = objProduto.CodigoPedidoProduto,
                        CodigoIntegracao = objProduto.CodigoProdutoEmbarcador,
                        Descricao = objProduto.Produto,
                        pedido.NumeroPedidoEmbarcador,
                        Quantidade = objProduto.SaldoQtde,
                        QuantidadeRetirada = objProduto.SaldoQtde,
                        PesoUnitario = (objProduto.Peso / objProduto.Qtde),
                        Observacao = objProduto.ObservacaoProduto,
                        TipoEmbalagem = objProduto.TipoEmbalagem,
                    }
                ).ToList(),
            };
        }

        private void EnviarEmailNotificacaoRetiradaProduto(Dominio.Entidades.Embarcador.Pedidos.NotificacaoRetiradaProduto notificacaoRetiradaProduto, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                if (notificacaoRetiradaProduto == null || notificacaoRetiradaProduto.Destinatarios.Count == 0)
                    return;

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                List<Dominio.ObjetosDeValor.Email.Mensagem> mensagens = new List<Dominio.ObjetosDeValor.Email.Mensagem>();
                List<string> listaDestinatarios = new List<string>();

                foreach (var destinatario in notificacaoRetiradaProduto.Destinatarios)
                {
                    string corpoEmail = SubstituirTags(notificacaoRetiradaProduto, destinatario);

                    mensagens.Add(new Dominio.ObjetosDeValor.Email.Mensagem
                    {
                        Destinatarios = new List<string> { destinatario.Email },
                        Assunto = notificacaoRetiradaProduto.Descricao,
                        Corpo = corpoEmail,
                    });
                }

                Servicos.Email.EnviarMensagensAsync(mensagens, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private string SubstituirTags(Dominio.Entidades.Embarcador.Pedidos.NotificacaoRetiradaProduto notificacaoRetiradaProduto, Dominio.Entidades.Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(notificacaoRetiradaProduto.Email))
                return notificacaoRetiradaProduto.Email;

            return notificacaoRetiradaProduto.Email.Replace("#TagRazaoSocialCliente", usuario?.Empresa?.RazaoSocial ?? string.Empty)
                             .Replace("#TagCNPJCliente", usuario?.CPF_CNPJ_Formatado ?? string.Empty)
                             .Replace("#TagEnderecoCliente", usuario?.Endereco ?? string.Empty)
                             .Replace("#TagComplementoCliente", usuario?.Complemento ?? string.Empty)
                             .Replace("#TagBairroCliente", usuario?.Bairro ?? string.Empty);
        }

        private void SetarPedidos(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento retiradaProduto, Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetira dados, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCarregamentoSituacao repositorioPedidoCarregamentoSituacao = new Repositorio.Embarcador.Pedidos.PedidoCarregamentoSituacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repositorioCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarrregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);

            List<int> codigosCarregamentoPedido = new List<int>();
            List<int> codigosPedidoProduto = new List<int>();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            foreach (var dynPedido in dados.Pedidos)
            {
                int codigo = dynPedido.Codigo;

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido = repositorioCarregamentoPedido.BuscarPorCarregamentoEPedido(pedido.Codigo, retiradaProduto.Codigo);

                if (carregamentoPedido == null && dynPedido.Produtos.Count > 0)
                {
                    carregamentoPedido = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido();
                    carregamentoPedido.Pedido = pedido;
                    carregamentoPedido.Carregamento = retiradaProduto;
                    repositorioCarregamentoPedido.Inserir(carregamentoPedido);
                    if (pedido.PedidoCarregamentoSituacao != null)
                    {
                        pedido.PedidoCarregamentoSituacao.SituacaoAtualPedidoRetirada = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtualPedidoRetirada.Agendamento;
                        pedido.PedidoCarregamentoSituacao.DataAgendamento = DateTime.Now;
                        repositorioPedidoCarregamentoSituacao.Atualizar(pedido.PedidoCarregamentoSituacao);
                    }

                    repositorioPedido.Atualizar(pedido);

                    if ((configuracaoGeralCarga?.AssumirSempreTipoOperacaoDoPedido ?? false))
                    {
                        retiradaProduto.TipoOperacao = pedido.TipoOperacao;
                        repositorioCarrregamento.Atualizar(retiradaProduto);
                    }
                }
                else if (carregamentoPedido != null && dynPedido.Produtos.Count == 0)
                {
                    repositorioCarregamentoPedido.ExcluirCarregamentoPedido(carregamentoPedido.Codigo);
                    continue;
                }

                decimal pesoCarregamentoPedido = 0;
                foreach (var prod in dynPedido.Produtos)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = repositorioPedidoProduto.BuscarPorPedidoProduto(pedido.Codigo, prod.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto carregamentoPedidoProduto = repositorioCarregamentoPedidoProduto.BuscaPorCarregamentoPedidoProduto(carregamentoPedido.Codigo, pedidoProduto.Codigo);

                    // Foi identificado que está salvando vários produtos com quantiade "0" sendo estes desnecessárias para posterior integreção.
                    // vamos adicionar na t_carregamento_pedido_produto somente os produtos que possuem quantidade válida...
                    // caso não possuir e existir, vamos excluir o registro.
                    if (carregamentoPedidoProduto == null && prod.Quantidade > 0)
                    {
                        carregamentoPedidoProduto = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto
                        {
                            CarregamentoPedido = carregamentoPedido,
                            PedidoProduto = pedidoProduto
                        };
                        repositorioCarregamentoPedidoProduto.Inserir(carregamentoPedidoProduto);
                    }

                    if (prod.Quantidade > 0)
                    {
                        carregamentoPedidoProduto.Quantidade = prod.Quantidade;
                        decimal pesoEmbalagem = (carregamentoPedidoProduto.PedidoProduto.PesoTotalEmbalagem / carregamentoPedidoProduto.PedidoProduto.Quantidade);
                        decimal peso = (carregamentoPedidoProduto.Quantidade * carregamentoPedidoProduto.PedidoProduto.PesoUnitario) + pesoEmbalagem;
                        carregamentoPedidoProduto.Peso = peso;

                        repositorioCarregamentoPedidoProduto.Atualizar(carregamentoPedidoProduto);

                        codigosPedidoProduto.Add(carregamentoPedidoProduto.Codigo);

                        pesoCarregamentoPedido += peso;
                    }
                    else if (carregamentoPedidoProduto != null)
                        repositorioCarregamentoPedidoProduto.Deletar(carregamentoPedidoProduto);
                }

                codigosCarregamentoPedido.Add(carregamentoPedido.Codigo);

                //Busca os demais produtos do pedido que não foram atualizados para excluir do carregamentoPedidoProduto.
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosExcluir = repositorioCarregamentoPedidoProduto.BuscarPorCodigoDiferenteECarregamentoPedido(codigosPedidoProduto, carregamentoPedido.Codigo);
                foreach (var produtoExcluir in produtosExcluir)
                    repositorioCarregamentoPedidoProduto.Deletar(produtoExcluir);

                carregamentoPedido.Peso = pesoCarregamentoPedido;
                repositorioCarregamentoPedido.Atualizar(carregamentoPedido);
            }

            //Agora, precisa excluir todos os carregamentos pedidos não carregados/atualizados
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repositorioCarregamentoPedido.BuscarPorCarregamento(retiradaProduto.Codigo);
            carregamentoPedidos = (from obj in carregamentoPedidos
                                   where !codigosCarregamentoPedido.Contains(obj.Codigo)
                                   select obj).ToList();

            foreach (var carregamentoPedido in carregamentoPedidos)
                repositorioCarregamentoPedido.ExcluirCarregamentoPedido(carregamentoPedido.Codigo);

        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetira PreencherObjetoDados(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            int codigoMotorista = Request.GetIntParam("Motorista");
            bool motoristaCadastrado = Request.GetBoolParam("MotoristaCadastrado");

            Dominio.Entidades.Usuario motorista = null;

            if (codigoMotorista > 0 && motoristaCadastrado)
                motorista = repositorioUsuario.BuscarMotoristaPorCodigo(codigoMotorista);

            Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetira dados = new Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetira()
            {
                Transportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Empresa : repEmpresa.BuscarPorCodigo(Request.GetIntParam("Transportador")),
                NomeTransportadora = Request.GetStringParam("NomeTransportadora"),
                PlacaVeiculo = Request.GetStringParam("PlacaVeiculo"),
                EmailNotificacao = Request.GetStringParam("EmailNotificacao"),
                ObservacaoTransportador = Request.GetStringParam("ObservacaoTransportador"),
                Motorista = motorista,
                CPFMotorista = Utilidades.String.OnlyNumbers(Request.GetStringParam("CpfMotorista")),
                NomeMotorista = Request.GetStringParam("NomeMotorista"),
                DataCarregamentoCarga = Request.GetNullableDateTimeParam("DataRetirada") ?? throw new ControllerException("A data e hora do agendamento deve ser informada"),
                ModeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(Request.GetIntParam("ModeloVeiculo")),
                TipoOperacao = repTipoOperacao.BuscarPorCodigo(Request.GetIntParam("TipoOperacao")),
                Filial = repFilial.BuscarPorCodigo(Request.GetIntParam("Filial")),
                Pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetiraPedido>()
            };

            if (dados.Motorista == null)
                dados.Motorista = SalvarMotorista(dados, unitOfWork);

            dynamic dynPedidos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Pedidos"));

            foreach (var dynPedido in dynPedidos)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetiraPedido objPedido = new Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetiraPedido()
                {
                    Codigo = dynPedido.Codigo,
                    Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetiraPedidoProduto>()
                };

                foreach (var prod in dynPedido.Produtos)
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetiraPedidoProduto objProduto = new Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetiraPedidoProduto()
                    {
                        Codigo = ((string)prod.Codigo).ToInt(),
                        Quantidade = ((string)prod.QuantidadeRetirada).ToDecimal()
                    };

                    objPedido.Produtos.Add(objProduto);
                }

                dados.Pedidos.Add(objPedido);
            }

            return dados;
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetira dados, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repRetiradaProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);

            carregamento.NomeTransportadora = dados.NomeTransportadora;
            carregamento.Empresa = dados?.Transportador ?? null;
            carregamento.PlacaVeiculo = dados.PlacaVeiculo;
            carregamento.EmailNotificacao = dados.EmailNotificacao;
            carregamento.DataCarregamentoCarga = dados.DataCarregamentoCarga;
            carregamento.ModeloVeicularCarga = dados.ModeloVeicularCarga;
            carregamento.TipoOperacao = dados.TipoOperacao;
            carregamento.Filial = dados.Filial;
            carregamento.UsuarioAgendamento = Usuario;
            carregamento.ObservacaoTransportador = dados.ObservacaoTransportador;

            if (carregamento.Codigo == 0)
            {
                carregamento.DataCriacao = DateTime.Now;
                carregamento.TipoMontagemCarga = TipoMontagemCarga.NovaCarga;
                carregamento.AutoSequenciaNumero = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork).ObterProximoCodigoCarregamento();
                carregamento.NumeroCarregamento = carregamento.AutoSequenciaNumero.ToString();
                carregamento.Motoristas = new List<Dominio.Entidades.Usuario>();
                carregamento.SituacaoCarregamento = SituacaoCarregamento.EmMontagem;
            }

            var veiculo = SalvarVeiculo(dados, unitOfWork);
            carregamento.Veiculo = veiculo;

            carregamento.Motoristas.Clear();
            if (dados.Motorista != null)
                carregamento.Motoristas.Add(dados.Motorista);
        }

        private void AtualizarCarga(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCarregamento(carregamento.Codigo);

            if (carga == null)
                return;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte repositorioConfiguracaoCargaDadosTransporte = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte configuracaoDadosTransporte = repositorioConfiguracaoCargaDadosTransporte.BuscarPrimeiroRegistro();

            carga.Motoristas.Clear();

            if (configuracaoDadosTransporte?.RetornarCargaPendenteConsultaCarregamentoAoSalvarDadosTransporte ?? false)
                carga.CarregamentoIntegradoERP = false;

            repositorioCarga.Atualizar(carga);

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoCarga(cargaPedido, ConfiguracaoEmbarcador, TipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga, permitirRemoverTodos: true, reentrega: false, removerPedidoCarregamento: false);

            Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork, ConfiguracaoEmbarcador);

            servicoMontagemCarga.AtualizarPedidosCargaCarregamento(carregamento, carga.Filial, carga, true, TipoServicoMultisoftware, Cliente);

            AtualizarDataCarregamentoCarga(carga, carregamento.DataCarregamentoCarga.Value, unitOfWork);
        }

        private Dominio.Entidades.Veiculo SalvarVeiculo(Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetira dados, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            var placaVeiculo = dados.PlacaVeiculo;

            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(placaVeiculo);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarEmpresaPadraoRetirada();
            if (dados.Transportador != null)
                empresa = dados.Transportador;

            if (veiculo == null)
                veiculo = Servicos.Veiculo.PreencherVeiculoGenerico(placaVeiculo, empresa, unitOfWork);

            veiculo.Empresa = empresa;
            veiculo.ModeloVeicularCarga = dados.ModeloVeicularCarga;
            repVeiculo.Atualizar(veiculo);

            return veiculo;
        }

        private Models.Grid.Grid ObterGridAgendamentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPedido = Request.GetIntParam("CodigoPedido");

                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork, somenteAgendamentosPendentes: false);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentos = repCarregamento.BuscarPorPedido(codigoPedido);

                Servicos.Embarcador.Logistica.CentroCarregamento servicoCentroCarregamento = new Servicos.Embarcador.Logistica.CentroCarregamento(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaProduto.Agendamento, "NumeroCarregamento", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaProduto.Peso, "PesoCarregamento", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaProduto.DataHoraAgendamento, "DataCarregamentoCarga", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaProduto.NomeTransportadora, "NomeTransportadora", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaProduto.Veiculo, "PlacaVeiculo", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaProduto.Motorista, "Motorista", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaProduto.Modelo, "Modelo", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "Situacao", 20, Models.Grid.Align.left, true);

                int totalRegistros = carregamentos.Count();
                var retorno = carregamentos.Select(carregamento => new
                {
                    carregamento.Codigo,
                    carregamento.NumeroCarregamento,
                    carregamento.PesoCarregamento,
                    carregamento.DataCarregamentoCarga,
                    NomeTransportadora = servicoCentroCarregamento.ObterNomeTransportadora(carregamento),
                    carregamento.PlacaVeiculo,
                    Motorista = carregamento.Motoristas?.FirstOrDefault()?.Nome,
                    Modelo = carregamento.ModeloVeicularCarga?.Descricao,
                    Situacao = carregamento.SituacaoCarregamento.ObterDescricaoRetirada(),
                }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("NomeTransportadora");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Observarcao", false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaProduto.Numero, "NumeroCarregamento", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaProduto.Filial, "Filial", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaProduto.Cliente, "Cliente", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaProduto.Pedidos, "Pedidos", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaProduto.Veiculo, "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaProduto.Motorista, "Motorista", 10, Models.Grid.Align.left, false);

                if (ConfiguracaoEmbarcador.Pais == TipoPais.Exterior)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaProduto.NPedidoCliente, "PedidoCliente", 10, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaProduto.NOCCliente, "OCCliente", 10, Models.Grid.Align.left, false);
                }

                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaProduto.DataHoraAgendamento, "DataCarregamentoCarga", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaProduto.NomeTransportadora, "NomeTransportadora", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "SituacaoCarregamento", 10, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repRetiradaProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork, somenteAgendamentosPendentes: false);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                int totalRegistros = repRetiradaProduto.ContarConsultaRetiradaProduto(filtrosPesquisa);
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.RetiradaProduto> listaRetiradaProduto = repRetiradaProduto.ConsultarRetiradaProduto(filtrosPesquisa, parametrosConsulta);

                var retorno = (from carregamento in listaRetiradaProduto
                               select new
                               {
                                   carregamento.Codigo,
                                   NumeroCarregamento = carregamento.NumeroCarregamentoFormatado,
                                   carregamento.DataCarregamentoCarga,
                                   NomeTransportadora = carregamento.NomeEmpresa,
                                   Situacao = carregamento.SituacaoCarregamentoDescricao ?? "",
                                   SituacaoCarregamento = carregamento.ObterDescricaoSituacaoAgendamento ?? "",
                                   Observarcao = carregamento.ObservacaoCarregamento ?? "",
                                   Cliente = carregamento.Cliente ?? "",
                                   Filial = carregamento.Filial ?? "",
                                   Pedidos = carregamento.Pedido ?? "",
                                   PedidoCliente = carregamento.CodigoPedidoCliente ?? "",
                                   OCCliente = carregamento.OrdemCompraCliente ?? "",
                                   Veiculo = carregamento.PlacaVeiculo,
                                   carregamento.Motorista
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork, bool somenteAgendamentosPendentes)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento()
            {
                NumeroCarregamento = Request.GetStringParam("NumeroCarregamento"),
                NomeTransportadora = Request.GetStringParam("NomeTransportadora"),
                Transportador = Request.GetIntParam("Transportador"),
                SituacaoAgendamento = Request.GetIntParam("SituacaoAgendamento"),
                CpfCnpjDestinatario = Request.GetIntParam("Destinatario"),
            };

            SituacaoCarregamento? situacaoCarregamento = Request.GetNullableEnumParam<SituacaoCarregamento>("SituacaoCarregamento");

            if (somenteAgendamentosPendentes)
                filtrosPesquisa.SituacoesCarregamento = new List<SituacaoCarregamento>() { SituacaoCarregamento.EmMontagem, SituacaoCarregamento.FalhaIntegracao };
            else if (situacaoCarregamento.HasValue)
                filtrosPesquisa.SituacoesCarregamento = new List<SituacaoCarregamento> { situacaoCarregamento.Value };
            else
                filtrosPesquisa.SituacoesCarregamento = new List<SituacaoCarregamento>() { SituacaoCarregamento.EmMontagem, SituacaoCarregamento.FalhaIntegracao, SituacaoCarregamento.Fechado, SituacaoCarregamento.Bloqueado };

            int codigoFilial = Request.GetIntParam("Filial");
            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            if ((codigoFilial > 0) && (codigosFilial.Count == 0))
                codigosFilial.Add(codigoFilial);

            filtrosPesquisa.NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador");
            filtrosPesquisa.CodigosFilial = codigosFilial;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacoes = repositorioTipoOperacao.BuscarTipoOperacoesRetirada();

                filtrosPesquisa.CodigosTipoOperacao = (from obj in tiposOperacoes select obj.Codigo).ToList();

                if (IsCompartilharAcessoEntreGrupoPessoas())
                    filtrosPesquisa.GrupoPessoaDestinatario = Usuario.ClienteFornecedor?.GrupoPessoas?.Codigo ?? 0;
                else
                {
                    if (this.Usuario.TipoComercial != TipoComercial.Gerente)
                        filtrosPesquisa.CpfCnpjDestinatario = this.Usuario.ClienteFornecedor?.CPF_CNPJ ?? 0d;

                    if (this.Usuario.Empresa?.EmpresaPai != null)
                        filtrosPesquisa.Transportador = this.Usuario.Empresa.Codigo;

                    if (this.Usuario.TipoComercial == TipoComercial.Vendedor)
                        filtrosPesquisa.CodigoFuncionarioVendedor = this.Usuario.Codigo;
                }

            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (Empresa.Matriz.Any())
                    filtrosPesquisa.TransportadoraMatriz = Empresa.Matriz.FirstOrDefault().Codigo;
                else
                    filtrosPesquisa.TransportadoraMatriz = Empresa.Codigo;
            }

            return filtrosPesquisa;
        }

        private dynamic ObterMontagemCarga(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool carregarMotoristas = true, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> pedidosCarregamentos = null, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosPedidosCarregamentos = null, bool montagemCargaPorPedidoProduto = false)
        {
            if (carregamento == null) return null;

            Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);

            bool exibirHoraDataCarregamentoEDescarregamento = configuracao.InformaApoliceSeguroMontagemCarga || configuracao.InformaHorarioCarregamentoMontagemCarga;
            Dominio.Entidades.Usuario motorista = (!carregarMotoristas) ? null : carregamento.Motoristas?.FirstOrDefault();

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repositorioCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentosPedidoProduto = repositorioCarregamentoPedidoProduto.BuscarPorCarregamento(carregamento.Codigo);  // .BuscarPorPedidoProduto(pedidoProduto.Codigo);           

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/RetiradaProduto");

            bool naoPermitirEditarAgendamento = false;
            bool naoPermitirExcluirAgendamento = false;

            if (!(Usuario.UsuarioAdministrador))
            {
                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.RetiradaProdutoLista_NaoPermitirEditarAgendamento) && carregamento.SituacaoCarregamento == SituacaoCarregamento.Fechado)
                    naoPermitirEditarAgendamento = true;

                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.RetiradaProdutoLista_NaoPermitirExcluirAgendamento) && carregamento.SituacaoCarregamento == SituacaoCarregamento.Fechado)
                    naoPermitirExcluirAgendamento = true;
            }
            var retorno = new
            {
                carregamento.Codigo,
                NumeroCarregamento = carregamento?.NumeroCarregamento ?? string.Empty,
                Transportador = new
                {
                    Codigo = carregamento.Empresa?.Codigo ?? 0,
                    Descricao = carregamento.Empresa?.RazaoSocial ?? string.Empty,
                },
                NomeTransportadora = carregamento?.NomeTransportadora,
                ObservacaoTransportador = carregamento?.ObservacaoTransportador ?? string.Empty,
                carregamento.PlacaVeiculo,
                Motorista = new { Codigo = motorista?.Codigo ?? 0, Descricao = motorista?.CPF_Formatado ?? string.Empty },
                CpfMotorista = motorista?.CPF_Formatado ?? string.Empty,
                NomeMotorista = motorista?.Nome ?? string.Empty,
                CapacidadeVeiculo = carregamento.Veiculo?.ModeloVeicularCarga?.CapacidadePesoTransporte ?? 0,
                PesoTotal = carregamento.PesoCarregamento,
                Filial = carregamento?.Filial?.Codigo ?? 0,
                FilialDescricao = carregamento?.Filial?.Descricao ?? "",
                ModeloVeiculo = carregamento.ModeloVeicularCarga?.Codigo,
                ModeloVeiculoDescricao = carregamento.ModeloVeicularCarga?.Descricao,
                TipoOperacao = carregamento.TipoOperacao?.Codigo,
                TipoOperacaoDescricao = carregamento.TipoOperacao?.Descricao,
                Data = carregamento.DataCarregamentoCarga?.ToDateString() ?? "",
                Hora = carregamento.DataCarregamentoCarga?.ToTimeString() ?? "",
                Situacao = carregamento.SituacaoCarregamento,
                carregamento.MensagemProblemaCarregamento,
                carregamento.EmailNotificacao,
                Pedidos = (
                        from pedido in carregamento.Pedidos
                        select new
                        {
                            pedido.Pedido.Codigo,
                            pedido.Pedido.NumeroPedidoEmbarcador,
                            PesoTotal = pedido.Peso,
                            NomeCliente = pedido.Pedido.Destinatario?.Nome ?? string.Empty,
                            CnpjCliente = pedido.Pedido.Destinatario?.CPF_CNPJ,
                            Endereco = pedido.Pedido.Destinatario?.Endereco ?? string.Empty,
                            CidadeUf = pedido.Pedido.Destinatario?.Localidade?.DescricaoCidadeEstado ?? string.Empty,
                            Produtos = (from objProduto in carregamentosPedidoProduto
                                        where objProduto.CarregamentoPedido.Codigo == pedido.Codigo
                                        select new
                                        {
                                            objProduto.PedidoProduto.Produto.Codigo,
                                            objProduto.PedidoProduto.Produto.Descricao,
                                            objProduto.PedidoProduto.Pedido.NumeroPedidoEmbarcador,
                                            objProduto.PedidoProduto.Quantidade,
                                            QuantidadeRetirada = objProduto.Quantidade,
                                            //objProduto.PedidoProduto.Produto.PesoUnitario,
                                            PesoUnitario = (objProduto.PedidoProduto.PesoTotal / (objProduto.PedidoProduto.Quantidade > 0 ? objProduto.PedidoProduto.Quantidade : 1)),
                                            objProduto.PedidoProduto.Observacao,
                                            CodigoIntegracao = objProduto.PedidoProduto.Produto?.CodigoProdutoEmbarcador ?? string.Empty,
                                            TipoEmbalagem = objProduto.PedidoProduto.Produto?.TipoEmbalagem?.CodigoIntegracao ?? string.Empty,
                                        }).ToList(),
                        }
                    ).ToList(),
                NaoPermitirEditarAgendamento = naoPermitirEditarAgendamento,
                NaoPermitirExcluirAgendamento = naoPermitirExcluirAgendamento
            };

            return retorno;
        }

        private dynamic ObterListaClientes(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            return pedido?.Clientes.Select(obj => new
            {
                obj.Codigo,
                CPF_CNPJ = obj.CPF_CNPJ_Formatado,
                obj.Nome,
                Localidade = obj?.Localidade?.DescricaoCidadeEstado ?? string.Empty
            }).ToList();
        }

        private dynamic ObterListaFronteiras(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<Dominio.Entidades.Cliente> listaFronteiras)
        {
            if (pedido.Fronteira != null && listaFronteiras.Count == 0)
            {
                listaFronteiras.Add(pedido.Fronteira);
            }

            return listaFronteiras.Select(o => new
            {
                CPF_CNPJ = o.CPF_CNPJ,
                Descricao = o.Descricao ?? "",
            }).ToList();
        }

        private dynamic ObterListaProdutos(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {

            return pedido?.Produtos.Select(obj => new
            {
                Codigo = obj.Codigo,
                AlturaCM = obj.AlturaCM.ToString("n3"),
                ComprimentoCM = obj.ComprimentoCM.ToString("n3"),
                LarguraCM = obj.LarguraCM.ToString("n3"),
                CodigoProdutoEmbarcador = obj.Produto != null ? obj.Produto.Codigo : 0,
                PesoTotal = obj.PesoUnitario * obj.Quantidade,
                CodigoProdutoEmbarcadorIntegracao = obj.Produto != null ? obj.Produto.CodigoProdutoEmbarcador : "",
                Descricao = obj.Produto != null ? obj.Produto.Descricao : string.Empty,
                Quantidade = obj.Quantidade.ToString("n3"),
                QuantidadePlanejada = obj.QuantidadePlanejada.ToString("n3"),
                Peso = obj.PesoUnitario.ToString("n3"),
                QuantidadePalets = obj.QuantidadePalet.ToString("n3"),
                obj.PalletFechado,
                MetrosCubico = obj.MetroCubico.ToString("n3"),
                obj.Observacao,
                obj.QuantidadeUnidadePorCaixa,
                obj.QuantidadeCaixaPorPallet,
                LinhaSeparacao = new
                {
                    Codigo = obj?.LinhaSeparacao?.Codigo ?? 0,
                    Descricao = obj?.LinhaSeparacao?.Descricao ?? ""
                }
            }).ToList();
        }

        private dynamic ObterSituacaoCarregamentoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            dynamic retorno = new
            {
                DataCriacaoPedido = pedido?.PedidoCarregamentoSituacao.DataCriacaoPedido?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataLiberacaoComercial = pedido?.PedidoCarregamentoSituacao?.DataLiberacaoComercial?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataLiberacaoFinanceira = pedido?.PedidoCarregamentoSituacao?.DataLiberacaoFinanceira?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataAgendamento = pedido?.PedidoCarregamentoSituacao?.DataAgendamento?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataRemessaConcluida = pedido?.PedidoCarregamentoSituacao?.DataRemessaConcluida?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataCarregamentoConcluido = pedido?.PedidoCarregamentoSituacao?.DataCarregamentoConcluido?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFaturamentoConcluido = pedido?.PedidoCarregamentoSituacao?.DataFaturamentoConcluido?.ToString("dd/MM/yyyy HH:mm") ?? "",
            };

            return retorno;
        }

        private void AtualizarDataCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasGeradas = repCarga.BuscarCargasPorCarregamento(carregamento.Codigo);

            foreach (var carga in cargasGeradas)
                AtualizarDataCarregamentoCarga(carga, carregamento.DataCarregamentoCarga.Value, unitOfWork);
        }

        private void ControleExibicaoJanelaCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> janelasGeradas = repCargaJanelaCarregamento.BuscarCargasPorCarregamento(carregamento.Codigo);

            foreach (var janela in janelasGeradas)
            {
                janela.CentroCarregamento = null;
                repCargaJanelaCarregamento.Atualizar(janela);
            }
        }

        private void ControleExibicaoCargaGerada(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, bool visivel, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasGeradas = repCarga.BuscarCargasPorCarregamento(carregamento.Codigo);

            foreach (var carga in cargasGeradas)
            {
                carga.CargaFechada = visivel;
                repCarga.Atualizar(carga);
            }
        }

        private bool IntegrarCriacaoSaintgobain(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Servicos.Embarcador.Integracao.IntegracaoCarregamento servicoIntegracaoCarregamento = new Servicos.Embarcador.Integracao.IntegracaoCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracaoSaintGobain = servicoIntegracaoCarregamento.AdicionarIntegracaoCarregamento(carregamento, StatusCarregamentoIntegracao.Inserir, TipoIntegracao.SaintGobain, SituacaoIntegracao.AgRetorno);

                if (carregamentoIntegracaoSaintGobain == null)
                    return true;

                new Servicos.Embarcador.Integracao.SaintGobain.IntegracaoSaintGobain(unitOfWork).IntegrarCarregamento(carregamentoIntegracaoSaintGobain);

                if (carregamentoIntegracaoSaintGobain.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                {
                    carregamento.MensagemProblemaCarregamento = "";
                    carregamento.SituacaoCarregamento = SituacaoCarregamento.Fechado;
                    carregamento.CarregamentoIntegradoERP = true;
                }
                else
                {
                    carregamento.MensagemProblemaCarregamento = carregamentoIntegracaoSaintGobain.ProblemaIntegracao;
                    carregamento.SituacaoCarregamento = SituacaoCarregamento.FalhaIntegracao;
                }

                repositorioCarregamento.Atualizar(carregamento);

                return carregamentoIntegracaoSaintGobain.SituacaoIntegracao == SituacaoIntegracao.Integrado;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return false;
            }
        }

        private void CancelarIntegracaoPententeSaintgobain(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.IntegracaoCarregamento servicoIntegracaoCarregamento = new Servicos.Embarcador.Integracao.IntegracaoCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracaoSaintGobain = servicoIntegracaoCarregamento.ObterIntegracaoCarregamento(carregamento.Codigo, TipoIntegracao.SaintGobain);

            if (carregamentoIntegracaoSaintGobain == null)
                return;

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repositorioCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(unitOfWork);

            carregamentoIntegracaoSaintGobain.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            carregamentoIntegracaoSaintGobain.NumeroTentativas = 99;
            carregamentoIntegracaoSaintGobain.ProblemaIntegracao = "Integração cancelada pelo cancelamento da carga no portal retira";

            repositorioCarregamentoIntegracao.Atualizar(carregamentoIntegracaoSaintGobain);
        }

        private void IntegrarAtualizacaoSaintgobain(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetira dados, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.IntegracaoCarregamento servicoIntegracaoCarregamento = new Servicos.Embarcador.Integracao.IntegracaoCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracaoSaintGobain = servicoIntegracaoCarregamento.ObterIntegracaoCarregamento(carregamento.Codigo, TipoIntegracao.SaintGobain);

            if (carregamentoIntegracaoSaintGobain == null)
                return;

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repositorioCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(unitOfWork);

            carregamentoIntegracaoSaintGobain.Status = StatusCarregamentoIntegracao.Atualizar;

            repositorioCarregamentoIntegracao.Atualizar(carregamentoIntegracaoSaintGobain);

            new Servicos.Embarcador.Integracao.SaintGobain.IntegracaoSaintGobain(unitOfWork).IntegrarCarregamento(carregamentoIntegracaoSaintGobain, dados);

            if (carregamentoIntegracaoSaintGobain.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                throw new ControllerException(carregamentoIntegracaoSaintGobain.ProblemaIntegracao);
        }

        private void IntegrarCancelamentoSaintgobain(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.IntegracaoCarregamento servicoIntegracaoCarregamento = new Servicos.Embarcador.Integracao.IntegracaoCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracaoSaintGobain = servicoIntegracaoCarregamento.ObterIntegracaoCarregamento(carregamento.Codigo, TipoIntegracao.SaintGobain);

            if (carregamentoIntegracaoSaintGobain == null)
                return;

            StatusCarregamentoIntegracao statusAnteriorIntegracao = carregamentoIntegracaoSaintGobain.Status;
            carregamentoIntegracaoSaintGobain.Status = StatusCarregamentoIntegracao.Remover;

            new Servicos.Embarcador.Integracao.SaintGobain.IntegracaoSaintGobain(unitOfWork).IntegrarCarregamento(carregamentoIntegracaoSaintGobain);

            if (carregamentoIntegracaoSaintGobain.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                return;

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repositorioCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(unitOfWork);

            carregamentoIntegracaoSaintGobain.Status = statusAnteriorIntegracao;
            carregamentoIntegracaoSaintGobain.SituacaoIntegracao = SituacaoIntegracao.Integrado;

            repositorioCarregamentoIntegracao.Atualizar(carregamentoIntegracaoSaintGobain);

            throw new ControllerException(carregamentoIntegracaoSaintGobain.ProblemaIntegracao);
        }

        private bool CancelarAgendamentoCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasGeradas = repCarga.BuscarCargasPorCarregamento(carregamento.Codigo);

            foreach (var carga in cargasGeradas)
                CancelarAgendamentoCarga(carga, unitOfWork);

            if (!carregamento.CarregamentoIntegradoERP)
                CancelarIntegracaoPententeSaintgobain(carregamento, unitOfWork);

            if (cargasGeradas.Count > 0)
                return true;
            else
                return false;
        }

        private void AtualizarDataCarregamentoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime dataHora, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);

            if (cargaJanelaCarregamento == null)
                return;

            cargaJanelaCarregamento.Initialize();
            cargaJanelaCarregamento.HorarioEncaixado = false;

            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento
            {
                CodigoModeloVeicularCarga = carga.ModeloVeicularCarga?.Codigo ?? 0,
                CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0,
                CodigoTransportador = carga.Empresa?.Codigo ?? 0,
                NotificarAlteracaoHorarioCarregamento = false
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarEmpresaPadraoRetirada();

                configuracaoDisponibilidadeCarregamento.CodigoTransportador = empresa?.Codigo ?? 0;
                configuracaoDisponibilidadeCarregamento.CpfCnpjCliente = Usuario?.ClienteFornecedor?.CPF_CNPJ ?? 0d;
            }

            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoCargaJanelaCarregamentoDisponibilidade = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork, configuracaoDisponibilidadeCarregamento);

            servicoCargaJanelaCarregamentoDisponibilidade.AlterarHorarioCarregamento(cargaJanelaCarregamento, dataHora, TipoServicoMultisoftware);
            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamento, cargaJanelaCarregamento.GetChanges(), $"Alterado o horário de carregamento para {cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:mm")}.", unitOfWork);
        }

        private void CancelarAgendamentoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
            {
                Carga = carga,
                GerarIntegracoes = false,
                LiberarPedidosParaMontagemCarga = true,
                MotivoCancelamento = Localization.Resources.Pedidos.RetiradaProduto.CancelamentoPorPortalRetira,
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                Usuario = this.Usuario
            };

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, ConfiguracaoEmbarcador, unitOfWork);
            Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware);
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaRetorno GerarCarga(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga cargaExiste = repCarga.BuscarPorCarregamento(carregamento.Codigo);

            if (carregamento.SituacaoCarregamento == SituacaoCarregamento.Bloqueado || cargaExiste != null)
                throw new ControllerException("Já existe uma carga para este carregamento.");

            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = new List<Dominio.Entidades.Embarcador.Filiais.Filial>
            {
                carregamento.Filial
            };

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repCarregamentoPedido.BuscarPorCarregamento(carregamento.Codigo);

            if (carregamentoPedidos.Count == 0)
                throw new ControllerException("Não é permitido Confirmar um agendamento sem nenhum pedido selecionado.");

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork, configuracaoEmbarcador);

            Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga propriedades = new Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga()
            {
                MontagemCargaPedidoProduto = Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaPedidoProduto.Sim,
                Usuario = Usuario
            };

            Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaRetorno montagemCargaRetorno = servicoMontagemCarga.GerarCarga(carregamento, filiais, carregamentoPedidos, TipoServicoMultisoftware, Cliente, Auditado, propriedades, ClienteAcesso.URLAcesso);

            if (carregamento.DataCarregamentoCarga.HasValue)
                AtualizarDataCarregamento(carregamento, unitOfWork);

            return montagemCargaRetorno;
        }

        private void ProcessarGeracaoCarga(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repRetiradaProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);

            unitOfWork.Start();

            var dados = PreencherObjetoDados(unitOfWork);
            PreencherEntidade(carregamento, dados, unitOfWork);
            SetarPedidos(carregamento, dados, unitOfWork);

            repRetiradaProduto.Atualizar(carregamento, Auditado);
            Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaRetorno montagemCargaRetorno = GerarCarga(carregamento, unitOfWork);

            unitOfWork.CommitChanges();

            //#42694
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidosCarga = repositorioCargaPedido.BuscarPorCarregamento(carregamento.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in pedidosCarga)
                Servicos.Embarcador.Integracao.Email.IntegracaoEmail.EnviarEmailDestiantarioTransportadorRetira(cargaPedido.Pedido, cargaPedido, "Agendamento coleta pedido", unitOfWork);

            if (!IntegrarCriacaoSaintgobain(carregamento, unitOfWork))
            {
                ControleExibicaoCargaGerada(carregamento, false, unitOfWork);
                ControleExibicaoJanelaCarregamento(carregamento, unitOfWork);
            }
        }

        private void ProcessarAtualizacaoCarga(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioRetiradaProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetira dados = PreencherObjetoDados(unitOfWork);

            try
            {
                unitOfWork.Start();

                DateTime? dataCarregamento = carregamento.DataCarregamentoCarga;

                PreencherEntidade(carregamento, dados, unitOfWork);
                SetarPedidos(carregamento, dados, unitOfWork);

                repositorioRetiradaProduto.Atualizar(carregamento, Auditado);

                AtualizarCarga(carregamento, unitOfWork, configuracaoGeralCarga);

                if (carregamento.DataCarregamentoCarga.HasValue && !dataCarregamento.Equals(carregamento.DataCarregamentoCarga))
                    AtualizarDataCarregamento(carregamento, unitOfWork);

                dados.Motorista = carregamento.Motoristas?.FirstOrDefault();
                dados.Veiculo = carregamento.Veiculo;

                IntegrarAtualizacaoSaintgobain(carregamento, dados, unitOfWork);
                AvancarEtapaNFe(carregamento, unitOfWork);

                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex, "PORTALRETIRA");
                Servicos.Log.TratarErro($"Usuario: {Usuario.Nome} ({Usuario.Codigo})\nCarregamento: {carregamento.Descricao} ({carregamento.Codigo})\nDados: {dados}", "PORTALRETIRA");
                throw;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido ObterFiltrosPesquisaBuscarPedidosProdutos(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoFilial = Request.GetIntParam("Filial");
            int codigoTipoCarga = Request.GetIntParam("TipoCarga");
            int codigoCargaDestino = Request.GetIntParam("CargaDestino");
            int codigoCanalEntrega = Request.GetIntParam("CanalEntrega");
            List<int> codigosCanalEntrega = Request.GetListParam<int>("CodigosCanalEntrega");
            if (codigoCanalEntrega > 0 && codigosCanalEntrega.Count == 0)
                codigosCanalEntrega.Add(codigoCanalEntrega);
            int codigoMotorista = Request.GetIntParam("Motorista");
            int codigoVeiculo = Request.GetIntParam("Veiculo");

            List<int> codigosTransportador = Request.GetListParam<int>("Transportador");

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();

            if (codigosTransportador.Count > 0)
                codigosTransportador.AddRange(repositorioEmpresa.BuscarCodigosFiliaisVinculadas(codigosTransportador));

            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido()
            {
                CidadePoloDestino = Request.GetIntParam("CidadePoloDestino"),
                CidadePoloOrigem = Request.GetIntParam("CidadePoloOrigem"),
                CodigosFilial = codigoFilial == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : new List<int>() { codigoFilial },
                CodigosTipoCarga = codigoTipoCarga == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoCarga },
                CodigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork),
                DataColeta = Request.GetNullableDateTimeParam("DataColeta"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
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
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                PedidoEmpresaResponsavel = Request.GetIntParam("PedidoEmpresaResponsavel"),
                PedidoCentroCusto = Request.GetIntParam("PedidoCentroCusto"),
                Container = Request.GetIntParam("Container"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoFuncionarioVendedor = Request.GetIntParam("FuncionarioVendedor"),
                NumeroEXP = Request.GetStringParam("NumeroEXP"),
                PedidoParaReentrega = Request.GetBoolParam("PedidosParaReentrega"),
                CodigosMotorista = codigoMotorista > 0 ? new List<int>() { codigoMotorista } : null,
                CodigosVeiculo = codigoVeiculo > 0 ? new List<int>() { codigoVeiculo } : null,
                ProcImportacao = Request.GetStringParam("ProcImportacao"),
                PrevisaoDataInicial = Request.GetNullableDateTimeParam("PrevisaoDataInicial"),
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                NumeroCotacao = Request.GetIntParam("NumeroCotacao"),
                NumeroProtocoloIntegracaoPedido = Request.GetIntParam("NumeroProtocoloIntegracaoPedido"),
                CodigosTransportador = codigosTransportador,
                //PrevisaoDataFinal = Request.GetNullableDateTimeParam("PrevisaoDataFinal"),
                ProgramaComSessaoRoteirizador = Request.GetBoolParam("ProgramaComSessaoRoteirizador"),
                NumeroTransporte = Request.GetStringParam("NumeroTransporte"),
                NumeroCarregamento = Request.GetStringParam("NumeroCarregamento"),
                SomentePedidosComNota = Request.GetBoolParam("PedidoComNotas"),
                NaoExibirPedidosDoDia = Request.GetBoolParam("NaoExibirPedidosDoDia"),
                CodigoPedidoMinimo = 0,
            };

            if (codigoCargaDestino > 0)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCargaDestino = repCargaPedido.BuscarPedidosPorCarga(codigoCargaDestino);
                filtrosPesquisa.NumeroControlesPedido = pedidosCargaDestino.Where(o => !string.IsNullOrWhiteSpace(o.NumeroControle)).Select(o => o.NumeroControle).Distinct().ToList();
            }

            if (operadorLogistica?.TelaPedidosResumido == true)
            {
                filtrosPesquisa.CodigosUsuario = new List<int>() {
                    Usuario.Codigo
                };
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (Empresa.Matriz.Any())
                    filtrosPesquisa.TransportadoraMatriz = Empresa.Matriz.FirstOrDefault().Codigo;
                else
                    filtrosPesquisa.TransportadoraMatriz = Empresa.Codigo;
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                bool compartilharAcessoEntreGrupoPessoas = IsCompartilharAcessoEntreGrupoPessoas();
                int grupoPessoa = Usuario.ClienteFornecedor?.GrupoPessoas?.Codigo ?? 0;

                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacoes = repTipoOperacao.BuscarTipoOperacoesRetirada();

                filtrosPesquisa.ProgramaComSessaoRoteirizador = Request.GetBoolParam("ProgramaComSessaoRoteirizador");

                filtrosPesquisa.CodigosTipoOperacao = (from obj in tiposOperacoes select obj.Codigo).ToList();
                if (filtrosPesquisa.CodigosTipoOperacao.Count > 0)
                    filtrosPesquisa.FiltroRetirada = true;

                if (compartilharAcessoEntreGrupoPessoas)
                {
                    filtrosPesquisa.GrupoPessoaDestinatario = grupoPessoa;
                }
                else
                {

                    if (Usuario.Empresa != null && !TipoComercialGerente())
                    {
                        if (Usuario.Empresa.EmpresaPai != null)
                            filtrosPesquisa.Transportador = Usuario.Empresa.Codigo;
                    }

                    if (this.Usuario.ClienteFornecedor != null && !TipoComercialGerente())
                    {
                        filtrosPesquisa.Destinatario = this.Usuario.ClienteFornecedor.CPF_CNPJ;
                    }

                    if (this.Usuario.TipoComercial == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComercial.Vendedor)
                    {
                        filtrosPesquisa.CodigoFuncionarioVendedor = this.Usuario.Codigo;
                    }
                }
                // #43685 - Pedidos totalmente carregado aparecendo para coleta... sem produtos..
                filtrosPesquisa.PedidoSemCarga = true;
            }
            else
                filtrosPesquisa.OcultarPedidosRetiradaProdutos = true;

            if ((this.Usuario?.LimitarOperacaoPorEmpresa ?? false) && (configuracaoGeral?.AtivarConsultaSegregacaoPorEmpresa ?? false) && this.Usuario?.Empresas != null && this.Usuario?.Empresas.Count > 0)
            {
                filtrosPesquisa.CodigosEmpresa = this.Usuario.Empresas.Select(o => o.Codigo).ToList();
            }

            if (filtrosPesquisa.PedidoParaReentrega)
                filtrosPesquisa.Situacao = null;
            else
            {
                filtrosPesquisa.Situacao = SituacaoPedido.Aberto;
                if (!filtrosPesquisa.ProgramaComSessaoRoteirizador)
                    filtrosPesquisa.PedidoSemCargaPedido = true;
            }

            return filtrosPesquisa;
        }

        private bool TipoComercialGerente()
        {
            return Usuario.TipoComercial == TipoComercial.Gerente;
        }

        private string ObterEnderecoLogoCliente()
        {
            AdminMultisoftware.Repositorio.UnitOfWork adminMultisoftwareUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {

                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(adminMultisoftwareUnitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorClienteETipo(Cliente.Codigo, TipoServicoMultisoftware);

                return clienteURLAcesso.Cliente.Logo;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return "";
            }
            finally
            {
                adminMultisoftwareUnitOfWork.Dispose();
            }
        }

        private string ObterClienteCarga(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCarregamento(carregamento.Codigo);

            return carga?.DadosSumarizados?.Destinatarios ?? string.Empty;
        }

        private string ObterFilialCarga(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCarregamento(carregamento.Codigo);

            return carga?.Filial?.Descricao ?? string.Empty;
        }

        private string ObterPedidosCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento)
        {
            if (carregamento.Pedidos == null || carregamento.Pedidos.Count == 0)
                return string.Empty;

            return string.Join(", ", carregamento.Pedidos.Select(obj => obj.Pedido.NumeroPedidoEmbarcador));
        }

        private Dominio.Entidades.Usuario SalvarMotorista(Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetira dados, Repositorio.UnitOfWork unitOfWork)
        {

            //if (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().UtilizarIntegracaoSaintGobainNova == "SIM")
            //    return null;

            if (string.IsNullOrWhiteSpace(dados.CPFMotorista))
                throw new ControllerException("Necessário informar o motorista.");

            if (string.IsNullOrWhiteSpace(dados.NomeMotorista))
                throw new ControllerException("Necessário informar o nome do motorista.");

            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

            var cpfMotorista = Utilidades.String.OnlyNumbers(dados.CPFMotorista).PadLeft(11, '0');
            var nomeMotorista = dados.NomeMotorista;

            Dominio.Entidades.Usuario motorista = repositorioUsuario.BuscarMotoristaPorCPF(cpfMotorista, string.Empty);

            Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarEmpresaPadraoRetirada();

            if (dados.Transportador != null)
                empresa = dados.Transportador;

            if (Usuario.Empresa != null)
                empresa = Usuario.Empresa;

            if (motorista == null)
            {
                motorista = Servicos.Usuario.PreencherMotoristaGenerico(nomeMotorista, empresa, unitOfWork);
                motorista.CPF = cpfMotorista;
                motorista.Status = "A";
            }
            else if (motorista.Status == "I")
                motorista.Status = "A";

            motorista.Empresa = empresa;

            motorista.Nome = nomeMotorista;

            repositorioUsuario.Atualizar(motorista);

            return motorista;
        }

        private string ObterDescricaoSituacaoAgendamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento agendamento)
        {
            if (agendamento.SituacaoCarregamento == SituacaoCarregamento.Fechado && !agendamento.DataCarregamentoCarga.HasValue)
                return Localization.Resources.Pedidos.RetiradaProdutoLista.PendenteAgendamento;

            return agendamento.SituacaoCarregamento.ObterDescricaoRetirada();
        }

        private string ObterPedidoCliente(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCarregamento(carregamento.Codigo);

            if ((carga?.Pedidos?.Count ?? 0) <= 0)
                return string.Empty;

            return string.Join(", ", carga?.Pedidos.Select(obj => obj.Pedido.CodigoPedidoCliente).ToList());
        }

        private string ObterOrdemCompraCliente(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCarregamento(carregamento.Codigo);

            return carga?.DadosSumarizados?.NumeroOrdem ?? string.Empty;
        }

        private void AvancarEtapaNFe(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCarregamento(carregamento.Codigo);

            if (carga == null)
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(carga.TipoOperacao?.Codigo ?? 0);

            if ((tipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.AvancarEtapaNFeCargaAoConfirmarAgendamentoRetira ?? false) && carga.SituacaoCarga == SituacaoCarga.AgNFe)
            {
                servicoCarga.LiberarCargaSemNFe(carga, cargaPedidos, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware, Auditado);
                carga.ProcessandoDocumentosFiscais = true;
            }

            repositorioCarga.Atualizar(carga);
        }

        #endregion
    }
}
