using Dominio.Entidades.Embarcador.Canhotos;
using Dominio.Entidades.Embarcador.Pessoas;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using Servicos.Embarcador.Notificacao;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize(new string[] { "BuscarDetalhesEntrega", "ObterMiniaturasCanhotos" }, "Cargas/ControleEntrega", "Logistica/Monitoramento", "Logistica/MonitoramentoNovo")]
    public class ControleEntregaEntregaController : BaseController
    {
        #region Construtores

        public ControleEntregaEntregaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDetalhesEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string paginaOrigem = Request.GetNullableStringParam("Page");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigo);

                if (cargaEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                DateTime dataEntrega = cargaEntrega.DataReprogramada ?? cargaEntrega.DataPrevista ?? DateTime.Now;
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> janelasDescarga = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ObterJanelaDescarregamento(cargaEntrega, dataEntrega, unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal repCargaEntregaFotoNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel repCargaEntregaAssinaturaResponsavel = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
                Repositorio.Embarcador.Pessoas.ClienteAreaRedex repClienteAreaRedex = new Repositorio.Embarcador.Pessoas.ClienteAreaRedex(unitOfWork);
                Repositorio.Embarcador.Pedidos.RetiradaContainer repositorioRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(unitOfWork);
                Repositorio.Embarcador.Pedidos.ColetaContainer repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoGTA repCargaEntregaFotoGTA = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoGTA(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Servicos.Embarcador.Chamado.ConfiguracaoTempoChamado servicoConfiguracaoTempoChamado = new Servicos.Embarcador.Chamado.ConfiguracaoTempoChamado(unitOfWork);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos = repCargaEntregaProduto.BuscarPorCargaEntrega(codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> ocorrenciaColetaEntregas = repOcorrenciaColetaEntrega.BuscarPorControleEntrega(codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal> cargaEntregaFotoNotasFiscais = repCargaEntregaFotoNotaFiscal.BuscarPorCargaEntrega(codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCargaEntrega(codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotaFiscals = repCargaEntregaNotaFiscal.BuscarPorCargaEntrega(codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotaFiscaisComLimiteDeRegistros = repCargaEntregaNotaFiscal.BuscarPorCargaEntregaComLimiteDeRegistros(codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoGTA> cargaEntregaFotosGTA = repCargaEntregaFotoGTA.BuscarPorCargaEntrega(codigo);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaEntrega.Carga;
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel cargaEntregaAssinatura = repCargaEntregaAssinaturaResponsavel.BuscarPorCargaEntrega(codigo);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> listaCargaPedidoProdutoCarga = repCargaPedidoProduto.BuscarPorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Pessoas.ClienteAreaRedex> listaAreaRedex = repClienteAreaRedex.BuscarPorCNPJCPFCliente(cargaEntrega.Cliente?.CPF_CNPJ ?? 0);

                Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer retiradaContainer = repositorioRetiradaContainer.BuscarPorCarga(carga.Codigo);
                Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = repositorioColetaContainer.BuscarPorCargaAtual(carga.Codigo);
                Dominio.Entidades.Embarcador.Pedidos.ContainerTipo containerTipo = retiradaContainer?.ContainerTipo ?? carga.ModeloVeicularCarga?.ContainerTipo ?? carga.Carregamento?.ModeloVeicularCarga?.ContainerTipo;
                Dominio.Entidades.Embarcador.Pedidos.Container container = retiradaContainer?.Container;
                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repositorioChamado.BuscarPorCargaEntrega(cargaEntrega.Codigo);
                List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamadosCargaEntrega = repositorioChamado.BuscarListaPorCargaEntrega(cargaEntrega.Codigo);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoBase = cargaEntregaPedidos.FirstOrDefault()?.CargaPedido?.Pedido;
                Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado configuracaoTempoChamado = chamado != null ? servicoConfiguracaoTempoChamado.ObterConfiguracaoPorCriterios(cargaEntrega.Cliente, carga.TipoOperacao, carga.Filial) : null;
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repositorioConfiguracaoControleEntrega.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracoTMS.BuscarConfiguracaoPadrao();

                bool possuiCargaEmDevolucaoPallet = repositorioGestaoDevolucao.PossuiCargaDevolucaoPallet(carga.Codigo);

                decimal peso = (from o in cargaEntregaPedidos select o.CargaPedido.Peso).Sum();
                TimeSpan? tempoProgramadaColeta = (pedidoBase?.DataPrevisaoSaida - pedidoBase?.DataInicialColeta);

                string dataSugerida = (!cargaEntrega.DataConfirmacao.HasValue) ? DateTime.Now.ToString("dd/MM/yyyy HH:mm") : string.Empty;

                decimal distanciaOrigemXEntrega = (carga?.TipoOperacao?.ConfiguracaoCarga?.ConsiderarKMRecibidoDoEmbarcador ?? false) && carga.DadosSumarizados?.Distancia > 0 ? carga.DadosSumarizados.Distancia : 0;

                decimal latitude = 0;
                decimal longitude = 0;

                decimal.TryParse((cargaEntrega.Cliente?.Latitude ?? "").Replace(".", ","), out latitude);
                decimal.TryParse((cargaEntrega.Cliente?.Longitude ?? "").Replace(".", ","), out longitude);

                string enderecoCompleto = cargaEntrega.ClienteOutroEndereco?.Descricao ?? cargaEntrega.Cliente?.EnderecoCompleto ?? cargaEntrega.Localidade?.DescricaoCidadeEstado ?? string.Empty;
                string descricaoCidadeEstado = cargaEntrega.ClienteOutroEndereco?.Localidade?.DescricaoCidadeEstado ?? cargaEntrega.Cliente?.Localidade?.DescricaoCidadeEstado ?? cargaEntrega.Localidade?.DescricaoCidadeEstado ?? string.Empty;
                string descricaoLocalidadeEntrega = cargaEntrega.ClienteOutroEndereco?.Descricao ?? cargaEntrega.Cliente?.Localidade?.DescricaoCidadeEstado ?? cargaEntrega.Localidade?.DescricaoCidadeEstado ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(cargaEntrega.ClienteOutroEndereco?.Longitude))
                {
                    decimal.TryParse(cargaEntrega.ClienteOutroEndereco.Latitude.Replace(".", ","), out latitude);
                    decimal.TryParse(cargaEntrega.ClienteOutroEndereco.Longitude.Replace(".", ","), out longitude);
                }

                string linkOcorrencia = "";
                if (pedidoBase != null)
                {
                    string codigoRastreamento = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterCodigoRastreamentoPedido(pedidoBase, unitOfWork);
                    string urlBase = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLBase(Cliente.Codigo, TipoServicoMultisoftware, adminUnitOfWork, _conexao.AdminStringConexao, unitOfWork);
                    linkOcorrencia = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLRastreamentoPedido(codigoRastreamento, urlBase);
                }

                TimeSpan? diferencaCarregamentoOuDescarregamento = cargaEntrega.DataFim - cargaEntrega.DataInicio;
                string dataEntregaNota = cargaEntrega.DataReentregaEmMesmaCarga?.ToString("dd/MM/yyyy HH:mm") ?? cargaEntrega.DataReentregaEmMesmaCarga?.ToString("dd/MM/yyyy HH:mm") ?? cargaEntrega.DataAgendamento?.ToString("dd/MM/yyyy HH:mm") ?? cargaEntrega.DataPrevista?.ToString("dd/MM/yyyy HH:mm");

                List<Dominio.Entidades.Embarcador.Ocorrencias.GestaoOcorrenciaSobras> sobras = ObterSobrasDaCargaEntrega(cargaEntrega, unitOfWork);

                string caminhoArquivos = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo()?.CaminhoArquivos ?? string.Empty;

                var retorno = new
                {
                    cargaEntrega.Codigo,
                    Email = cargaEntrega.Cliente?.Email ?? "",
                    OrdemEntrega = $"{(cargaEntrega.Ordem + 1)}/" + (cargaEntrega.Situacao == SituacaoEntrega.NaoEntregue ? "" : (cargaEntrega.OrdemRealizada + 1).ToString()),
                    EntregaNoPrazo = cargaEntrega.DescricaoEntregaNoPrazo,
                    CodigoChamado = chamado?.Codigo ?? 0,
                    NumeroChamado = chamado?.Numero.ToString() ?? "",
                    ChamadosEntrega = ObterTodosChamadosCargaEntrega(chamadosCargaEntrega, unitOfWork),
                    DataEstimadaResolucaoChamado = (configuracaoTempoChamado?.TempoAtendimento ?? 0) > 0 ? chamado.DataCriacao.AddMinutes(configuracaoTempoChamado.TempoAtendimento).ToDateTimeString(true) : string.Empty,
                    Numero = cargaEntrega.Cliente != null ? $"{cargaEntrega.Cliente?.CPF_CNPJ_Formatado} - {cargaEntrega.Cliente.Nome}" : cargaEntrega.Localidade != null ? cargaEntrega.Localidade.Descricao : string.Empty,
                    ResponsavelFinalizacaoManual = cargaEntrega.ResponsavelFinalizacaoManual?.Descricao ?? string.Empty,
                    NumeroCTe = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && cargaEntregaNotaFiscals.Count > 0 ? RetornarNumerosCTes(cargaEntregaNotaFiscals.Where(p => p.PedidoXMLNotaFiscal != null)?.Select(p => p.PedidoXMLNotaFiscal.Codigo).ToList() ?? null, unitOfWork) : "",
                    Destinatario = cargaEntrega.Cliente?.Nome ?? cargaEntrega.Localidade?.Descricao ?? string.Empty,
                    Localidade = descricaoCidadeEstado,
                    LocalidadeCliente = descricaoCidadeEstado,
                    LocalidadeEntrega = descricaoLocalidadeEntrega,
                    Situacao = cargaEntrega.DescricaoSituacao,
                    EnumSituacao = cargaEntrega.Situacao,
                    cargaEntrega.PermitirEntregarMaisTarde,
                    OnTime = cargaEntrega.SituacaoOnTime,
                    JustificativaOnTime = cargaEntrega.JustificativaOnTime,
                    FilialVenda = string.Join(", ", cargaEntrega.Pedidos.Select(pedido => pedido.CargaPedido?.Pedido?.FilialVenda?.Descricao ?? string.Empty).Distinct().ToList()),
                    DataPrevisaoEntrega = !string.IsNullOrWhiteSpace(carga?.StatusLoger) && carga.StatusLoger == "Ativo" ? string.Empty : cargaEntrega.DataPrevista?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataPrevisaoSaida = cargaEntrega.DataFimPrevista?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataAgendada = (cargaEntrega.Pedidos?.FirstOrDefault()?.CargaPedido?.Pedido?.DataAgendamento.HasValue ?? false) ? $"{cargaEntrega.Pedidos?.FirstOrDefault()?.CargaPedido?.Pedido?.DataAgendamento.Value.ToString("dd/MM/yyyy HH:mm")} (Agendado por {cargaEntrega.Pedidos?.FirstOrDefault()?.CargaPedido?.Pedido?.Usuario?.Nome})" : string.Empty,
                    ObservacoesAgendamento = (cargaEntrega.Pedidos?.FirstOrDefault()?.CargaPedido?.Pedido?.DataAgendamento.HasValue ?? false) ? cargaEntrega.Pedidos?.FirstOrDefault()?.CargaPedido?.Pedido?.ObservacaoAdicional : "",
                    DataEntregaReprogramada = !string.IsNullOrWhiteSpace(carga?.StatusLoger) && carga.StatusLoger == "Ativo" ? string.Empty : cargaEntrega.DataReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    InicioViagemRealizada = carga?.DataInicioViagem?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataProgramadaColeta = pedidoBase?.DataInicialColeta?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    TempoProgramadaColeta = tempoProgramadaColeta.HasValue ? $"{(int)tempoProgramadaColeta.Value.TotalHours}:{tempoProgramadaColeta.Value.Minutes}" : string.Empty,
                    DataProgramadaDescarga = pedidoBase?.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataInicio = cargaEntrega.DataInicio?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataFim = cargaEntrega.DataFim?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataConfirmacao = cargaEntrega.DataConfirmacao?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataConfirmacaoApp = cargaEntrega.DataConfirmacaoApp?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataRejeitado = cargaEntrega.DataRejeitado?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataEntradaRaio = cargaEntrega.DataEntradaRaio?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataSaidaRaio = cargaEntrega.DataSaidaRaio?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataReentregaMesmaCarga = (chamado?.NaoAssumirDataEntregaNota ?? false) ? dataEntregaNota : cargaEntrega.DataReentregaEmMesmaCarga?.ToString("dd/MM/yyyy HH:mm"),
                    DataEntregaNota = dataEntregaNota,
                    StatusEntregaNota = dataEntregaNota.ToDateTime() > DateTime.Now ? "No prazo" : "Nota em Atraso",
                    Observacao = (cargaEntrega.Observacao ?? "").Replace("\n", "<br />"),
                    NomeRecebedor = (cargaEntrega.Cliente?.CodigoIntegracao ?? string.Empty) + " - " + cargaEntrega.Cliente?.Nome ?? cargaEntrega.Localidade?.Descricao ?? string.Empty,
                    //MotivoDevolucaoEntrega = new { Codigo = cargaEntrega.MotivoDevolucaoEntrega?.Codigo ?? 0, Descricao = cargaEntrega.MotivoDevolucaoEntrega?.Descricao ?? "" },
                    DocumentoRecebedor = cargaEntrega.Cliente?.CPF_CNPJ_Formatado ?? cargaEntrega.Localidade?.CEP ?? string.Empty,
                    EnderecoCliente = enderecoCompleto,
                    JanelaDescarga = janelasDescarga != null ? string.Join(", ", (from janelaDescarga in janelasDescarga select janelaDescarga?.HoraInicio.ToString(@"hh\:mm") + " - " + janelaDescarga?.HoraTermino.ToString(@"hh\:mm"))) : ObterDatasJanelaDaEntrega(cargaEntrega),
                    InicioViagemPrevista = !string.IsNullOrWhiteSpace(carga?.StatusLoger) && carga.StatusLoger == "Ativo" ? string.Empty : cargaEntrega.Carga?.DataInicioViagemPrevista?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    NotasFiscais = String.Join(", ", (from notas in cargaEntregaNotaFiscaisComLimiteDeRegistros select notas?.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Numero)),
                    cargaEntrega.Coleta,
                    //MotivoRejeicaoColeta = cargaEntrega.MotivoRejeicaoColeta?.Descricao ?? "",
                    InfoMotivoRejeicao = cargaEntrega.MotivoRejeicao?.Descricao ?? "",
                    InfoMotivoRetificacao = cargaEntrega.MotivoRetificacaoColeta?.Descricao ?? "",
                    InfoMotivoFalhaGTA = cargaEntrega.MotivoFalhaGTA?.Descricao ?? "",
                    ExigirFotoGTA = cargaEntrega.MotivoFalhaGTA?.ExigirFotoGTA ?? false,
                    MostrarAbaNFTransferenciaDevolucaoPallet = possuiCargaEmDevolucaoPallet,
                    InfoMotivoFalhaNotaFiscal = cargaEntrega.MotivoFalhaNotaFiscal?.Descricao ?? "",
                    cargaEntrega.PossuiNotaCobertura,
                    PermitirTransportadorConfirmarRejeitarEntrega = carga?.TipoOperacao?.PermitirTransportadorConfirmarRejeitarEntrega ?? false,
                    ClientePossuiAreaRedex = !cargaEntrega.Coleta && (retiradaContainer != null || listaAreaRedex.Count > 0) && !cargaEntrega.Cliente.Armador,
                    ColetaDeContainer = cargaEntrega.Coleta && cargaEntrega.ColetaEquipamento && retiradaContainer != null,
                    GerenteArea = ObterGerenteArea(pedidoBase),
                    TipoContainerCarga = containerTipo?.Codigo ?? 0,
                    DescricaoTipoContainerCarga = containerTipo?.Descricao ?? "",
                    CodigoContainer = container?.Codigo ?? 0,
                    DescricaoContainer = container?.Descricao ?? "",
                    LocalRetiradaContainer = retiradaContainer?.Local?.CPF_CNPJ.ToString() ?? "",
                    ObrigarFotoCanhoto = carga?.TipoOperacao?.ConfiguracaoMobile?.ObrigarFotoCanhoto ?? false,
                    Armador = cargaEntrega.Cliente?.Armador ?? false,
                    DistanciaOrigemXEntrega = distanciaOrigemXEntrega > 0 ? $"{distanciaOrigemXEntrega.ToString("n3")} Km" : "0 Km",
                    TelefoneCliente = cargaEntrega.Cliente?.Telefone1.ObterTelefoneFormatado() ?? string.Empty,
                    LeadTimeTransportador = !cargaEntrega.Coleta ? cargaEntrega.Pedidos?.FirstOrDefault()?.CargaPedido?.Pedido?.DiasUteisPrazoTransportador : 0,
                    Vendedor = new
                    {
                        Nome = pedidoBase?.FuncionarioVendedor?.Nome,
                        Email = pedidoBase?.FuncionarioVendedor?.Email,
                        Telefone = pedidoBase?.FuncionarioVendedor?.Telefone,
                    },
                    Supervisor = new
                    {
                        Nome = pedidoBase?.FuncionarioSupervisor?.Nome,
                        Email = pedidoBase?.FuncionarioSupervisor?.Email,
                        Telefone = pedidoBase?.FuncionarioSupervisor?.Telefone,
                    },
                    GerenteRegional = ObterGerenteRegional(pedidoBase),
                    GerenteNacional = ObterGerenteNacional(pedidoBase),
                    CodigoSap = pedidoBase?.Destinatario?.CodigoSap ?? "",
                    Localizacao = !string.IsNullOrEmpty(cargaEntrega.ClienteOutroEndereco?.Latitude) && !string.IsNullOrEmpty(cargaEntrega.ClienteOutroEndereco?.Longitude) ?
                                  cargaEntrega.ClienteOutroEndereco?.Latitude.Replace(",", ".") + "," + cargaEntrega.ClienteOutroEndereco?.Longitude.Replace(",", ".") :

                                  !string.IsNullOrEmpty(cargaEntrega.Cliente?.Latitude) && !string.IsNullOrEmpty(cargaEntrega.Cliente?.Longitude) ?
                                  cargaEntrega.Cliente?.Latitude.Replace(",", ".") + "," + cargaEntrega.Cliente?.Longitude.Replace(",", ".") :

                                  !string.IsNullOrEmpty(cargaEntrega.Localidade?.Latitude.ToString()) && !string.IsNullOrEmpty(cargaEntrega.Localidade?.Longitude.ToString()) ?
                                  cargaEntrega.Localidade?.Latitude.ToString().Replace(",", ".") + "," + cargaEntrega.Localidade?.Longitude.ToString().Replace(",", ".") :

                                    "",

                    Pedidos = string.Join(", ", (from o in cargaEntregaPedidos select o.CargaPedido.Pedido.NumeroPedidoEmbarcador)),
                    NumeroPedidoCliente = string.Join(", ", (from o in cargaEntregaPedidos select o.CargaPedido.Pedido?.CodigoPedidoCliente).Take(100)),
                    Peso = peso.ToString("n2"),
                    JustificativaEntregaForaRaio = !string.IsNullOrWhiteSpace(cargaEntrega.JustificativaEntregaForaRaio) ? cargaEntrega.JustificativaEntregaForaRaio : "",
                    LocalEntrega = new
                    {
                        Latitude = cargaEntrega.LatitudeFinalizada ?? (paginaOrigem != null && paginaOrigem == "ControleEntrega" ? 0 : carga.LatitudeInicioViagem ?? 0),
                        Longitude = cargaEntrega.LongitudeFinalizada ?? (paginaOrigem != null && paginaOrigem == "ControleEntrega" ? 0 : carga.LongitudeInicioViagem ?? 0),
                    },
                    LocalDescarga = new
                    {
                        Latitude = cargaEntrega.LatitudeFinalizada ?? 0,
                        Longitude = cargaEntrega.LongitudeFinalizada ?? 0,
                    },
                    LocalCliente = new
                    {
                        Latitude = latitude,
                        Longitude = longitude,
                        Raio = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ObterRaioEntrega(cargaEntrega.Cliente, unitOfWork),
                    },
                    Assinatura = cargaEntregaAssinatura == null ? null : new
                    {
                        cargaEntregaAssinatura.Codigo,
                        Miniatura = Base64ImagemAssinatura(cargaEntregaAssinatura, unitOfWork)
                    },
                    Imagens = (
                        from o in cargaEntrega.Fotos
                        select new
                        {
                            o.Codigo,
                            o.Latitude,
                            o.Longitude,
                            DataRecebimento = o.DataEnvioImagem.ToString("dd/MM/yyyy HH:mm"),
                            Miniatura = Base64ImagemAnexo(o, unitOfWork),
                            ArquivosPDF = o.NomeArquivo.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ? true : false
                        }
                    ).ToList(),
                    ImagensNotasFiscais = (
                        from o in cargaEntregaFotoNotasFiscais
                        select new
                        {
                            o.Codigo,
                            Miniatura = Base64ImagemNotaFiscal(o, unitOfWork)
                        }
                    ).ToList(),
                    ImagensFotoGTA = (
                        from o in cargaEntregaFotosGTA
                        select new
                        {
                            o.Codigo,
                            Miniatura = Base64ImagemFotoGTA(o, unitOfWork)
                        }
                    ).ToList(),
                    Ocorrencias = (
                       from o in ocorrenciaColetaEntregas
                       select new
                       {
                           o.Codigo,
                           CodigoOcorrencia = o.TipoDeOcorrencia?.Codigo ?? 0,
                           CodigoPedido = pedidoBase?.Codigo ?? 0,
                           Descricao = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterDescricaoPortalOcorrencia(o.TipoDeOcorrencia, carga, o.CargaEntrega.Cliente, pedidoBase?.Remetente, true),
                           DataOcorrencia = o.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                           LinkOcorrencia = linkOcorrencia,
                           DataPosicao = o.DataPosicao.HasValue ? o.DataPosicao.Value.ToString("dd/MM/yyyy HH:mm") : "",
                           DataReprogramada = o.DataPrevisaoRecalculada.HasValue ? o.DataPrevisaoRecalculada.Value.ToString("dd/MM/yyyy HH:mm") : "",
                           Latitude = o.Latitude ?? 0,
                           Longitude = o.Longitude ?? 0,
                           TempoPercurso = o.TempoPercurso ?? "",
                           Distancia = (o.DistanciaAteDestino > 0 ? (o.DistanciaAteDestino / 1000).ToString("n3") : "0") + " KM",
                           o.Pacote,
                           o.Volumes,
                           ObservarcaoOcorrencia = o.ObservacaoOcorrencia,
                           Origem = o.OrigemOcorrencia.HasValue ? OrigemCriacaoOcorrenciaHelper.ObterDescricao(o.OrigemOcorrencia.Value) : "",
                       }
                    ).ToList(),
                    AreasRedex = ObterAreasRedex(listaAreaRedex, cargaEntrega),
                    CheckList = ObterCheckListCargaEntrega(cargaEntrega, cargaEntrega.Coleta ? TipoCheckList.Coleta : TipoCheckList.Entrega, unitOfWork),
                    CheckListDesembarque = ObterCheckListCargaEntrega(cargaEntrega, TipoCheckList.Desembarque, unitOfWork),
                    Produtos = ObterListaProduto(cargaEntrega, cargaEntregaProdutos, cargaEntregaPedidos, listaCargaPedidoProdutoCarga, unitOfWork),
                    QuantidadePlanejada = servicoControleEntrega.ObterQuantidadePlanejada(cargaEntrega, cargaEntregaPedidos, listaCargaPedidoProdutoCarga)?.ToString("n2") ?? null,
                    QuantidadeTotal = servicoControleEntrega.ObterQuantidadeTotal(cargaEntrega, cargaEntregaPedidos, listaCargaPedidoProdutoCarga)?.ToString("n2") ?? null,
                    DataInicioEntregaSugerida = dataSugerida,
                    DataEntregaSugerida = dataSugerida,
                    AvaliacaoEntrega = !cargaEntrega.DataAvaliacao.HasValue ? null : new
                    {
                        DataAvaliacao = cargaEntrega.DataAvaliacao.Value.ToDateTimeString(),
                        Avaliacao = cargaEntrega.AvaliacaoGeral,
                        TipoAvaliacaoGeral = cargaEntrega.AvaliacaoGeral.HasValue,
                        Questionario = cargaEntrega.Avaliacoes.OrderBy(o => o.Ordem).Select(o => new { o.Codigo, o.Titulo, o.Conteudo, o.Resposta }).ToList(),
                        ObservacaoAvaliacao = (cargaEntrega?.ObservacaoAvaliacao ?? string.Empty).Replace("\n", "<br />"),
                        MotivoAvaliacao = cargaEntrega.MotivoAvaliacao?.Descricao ?? string.Empty
                    },
                    PermiteRemoverReentrega = (cargaEntrega.Reentrega && !carga.DataInicioViagem.HasValue && !cargaEntrega.Coleta),
                    PermitirRemoverReentregaAdicionadaAEntrega = (cargaEntrega.PossuiPedidosReentrega && !cargaEntrega.Coleta && (cargaEntrega.Situacao == SituacaoEntrega.Reentergue || SituacaoEntregaHelper.ObterSituacaoEntregaEmAberto(cargaEntrega.Situacao))),

                    //Dados recebedor
                    DadosRecebedorNome = cargaEntrega.DadosRecebedor?.Nome ?? "",
                    DadosRecebedorCPF = cargaEntrega.DadosRecebedor?.CPF ?? "",
                    DadosRecebedorDataEntrega = cargaEntrega.DadosRecebedor?.DataEntrega.ToString("dd/MM/yyyy") ?? "",
                    DadosRecebedorPercentualCompatibilidadeFoto = cargaEntrega.DadosRecebedor?.PercentualCompatibilidadeFoto != null ? $"{cargaEntrega.DadosRecebedor?.PercentualCompatibilidadeFoto}% compatível" : "Sem informação de compatibilidade",
                    DadosRecebedorFoto = Base64Imagem(Utilidades.Directory.CriarCaminhoArquivos(new string[] { caminhoArquivos, "Anexos", "CargaColetaEntrega", "FotoRecebedor" }), "foto.jpg", cargaEntrega.DadosRecebedor?.GuidFoto),

                    // Confirmação de chegada
                    LocalConfirmacaoChegada = new
                    {
                        Latitude = cargaEntrega.LatitudeConfirmacaoChegada,
                        Longitude = cargaEntrega.LongitudeConfirmacaoChegada,
                    },
                    TipoOperacao = new
                    {
                        PermiteConfirmarChegadaColeta = carga?.TipoOperacao?.ConfiguracaoMobile?.PermiteConfirmarChegadaColeta ?? false,
                        PermiteConfirmarChegadaEntrega = carga?.TipoOperacao?.ConfiguracaoMobile?.PermiteConfirmarChegadaEntrega ?? false,
                        ControlarTempo = (cargaEntrega.Coleta ? carga?.TipoOperacao?.ConfiguracaoMobile?.ControlarTempoColeta : carga?.TipoOperacao?.ConfiguracaoMobile?.ControlarTempoEntrega) ?? false,
                        PermitirEscanearChavesNfe = carga?.TipoOperacao?.ConfiguracaoMobile?.PermitirEscanearChavesNfe ?? false,
                        PermiteAdicionarNotasControleEntrega = carga?.TipoOperacao?.ConfiguracaoControleEntrega?.PermitirInformarNotasFiscaisNoControleEntrega ?? false,
                        PermitirAtualizarEntregasCargasFinalizadas = carga?.TipoOperacao?.PermitirAtualizarEntregasCargasFinalizadas ?? false,
                        SolicitarReconhecimentoFacialDoRecebedor = carga?.TipoOperacao?.ConfiguracaoMobile?.SolicitarReconhecimentoFacialDoRecebedor ?? false,
                        PermiteBaixarComprovanteColeta = carga?.TipoOperacao?.ConfiguracaoImpressao?.PermiteBaixarComprovanteColeta ?? false
                    },

                    // Datas de carregamento ou descarregamento
                    DataInicioCarregamentoOuDescarregamento = cargaEntrega.DataInicio?.ToString("dd/MM/yyyy HH:mm"),
                    DataTerminoCarregamentoOuDescarregamento = cargaEntrega.DataFim?.ToString("dd/MM/yyyy HH:mm"),
                    TempoCarregamentoOuDescarregamento = diferencaCarregamentoOuDescarregamento != null ? diferencaCarregamentoOuDescarregamento?.Hours + "h" + diferencaCarregamentoOuDescarregamento?.Minutes + "min" : null,

                    // Mostrar abas
                    MostrarAbaGta = (from o in cargaEntregaProdutos select o.Produto.ObrigatorioGuiaTransporteAnimal).Any((o) => o),
                    MostrarAbaNotasFiscais = (from o in cargaEntregaProdutos select o.Produto.ObrigatorioNFProdutor).Any((o) => o),

                    CpfCnpjCliente = cargaEntrega.Cliente?.CPF_CNPJ,
                    DadosDevolucao = ObterDadosDevolucao(cargaEntrega),
                    Fronteira = cargaEntrega.Fronteira,
                    Parqueamento = cargaEntrega.Parqueamento,
                    AtivarConfirmacaoEntrega = carga.TipoOperacao?.ConfiguracaoControleEntrega?.ExigirConferenciaProdutosAoConfirmarEntrega ?? false,
                    ObservacoesPedidos = string.Join(" / ", (from o in cargaEntregaPedidos where !string.IsNullOrEmpty(o.CargaPedido.Pedido.Observacao) select o.CargaPedido.Pedido.Observacao)),
                    EtapaStage = ObterDadosEtapaStage(cargaEntregaPedidos, unitOfWork),
                    ExigeConferenciaProdutos = ExigirConferenciaProdutos(cargaEntrega, unitOfWork),
                    DataAgendamentoDeEntrega = cargaEntrega.DataAgendamento.HasValue ? cargaEntrega.DataAgendamento.Value.ToDateTimeString() : "",
                    DataPrevisaoEntregaAjustada = cargaEntrega.DataPrevisaoEntregaAjustada.HasValue ? cargaEntrega.DataPrevisaoEntregaAjustada.Value.ToDateTimeString() : "",
                    AlterarDataAgendamentoDeEntrega = configuracaoControleEntrega?.PermiteAlterarAgendamentoDaEntregaNoAcompanhamentoDeCargas ?? false,
                    DataAgendamentoEntregaTransportador = cargaEntrega.DataAgendamentoEntregaTransportador?.ToDateTimeString() ?? string.Empty,
                    AlterarDataAgendamentoEntregaTransportador = configuracaoControleEntrega?.PermitirAlterarDataAgendamentoEntregaTransportador ?? false,
                    OrigemSituacaoEntrega = cargaEntrega.OrigemCriacaoDataAgendamentoCargaEntrega.HasValue ? cargaEntrega.OrigemCriacaoDataAgendamentoCargaEntrega.Value.ObterDescricao() : string.Empty,
                    ParametrosDeCalculo = ObterParametrosCalculo(configuracaoTMS, cargaEntrega.Distancia, cargaEntrega, cargaEntrega.Carga.DataInicioViagemPrevista, cargaEntrega.DataReprogramada, unitOfWork),
                    ExibirSobras = sobras.Count > 0,
                    Sobras = (from sobra in sobras
                              select new
                              {
                                  sobra.Codigo,
                                  sobra.CodigoSobra,
                                  sobra.QuantidadeSobra,
                                  Observacao = sobra.ObservacaoConferencia ?? "",
                                  QuantidadeConferencia = sobra?.QuantidadeConferencia ?? 0
                              }).ToList(),
                    ExigirInformarNumeroPacotesNaColetaTrizy = cargaEntrega.Coleta && (carga.TipoOperacao?.ConfiguracaoControleEntrega?.ExigirInformarNumeroPacotesNaColetaTrizy ?? false),
                    cargaEntrega.QuantidadePacotesColetados,
                    ExibirDataPrevisaoEntregaTransportador = cargaEntrega.DataPrevisaoEntregaTransportador.HasValue,
                    DataPrevisaoEntregaTransportador = cargaEntrega.DataPrevisaoEntregaTransportador?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DetalhesProcessamentoFinalizacaoAssincrona = cargaEntrega.Situacao != SituacaoEntrega.Entregue && cargaEntrega.CargaEntregaFinalizacaoAssincrona != null && cargaEntrega.CargaEntregaFinalizacaoAssincrona.SituacaoProcessamento == SituacaoProcessamentoIntegracao.ErroProcessamento ? cargaEntrega.CargaEntregaFinalizacaoAssincrona.DetalhesProcessamento : string.Empty,
                    CanalVenda = pedidoBase?.CanalVenda?.Descricao,
                    EscritorioVenda = pedidoBase?.EscritorioVenda,
                    EquipeVendas = pedidoBase?.EquipeVendas,
                    TipoMercadoria = pedidoBase?.TipoMercadoria,
                    StatusTendenciaEntrega = cargaEntrega.Tendencia.ObterDescricao(),
                    Mesoregiao = cargaEntrega.Cliente?.MesoRegiao?.Descricao ?? string.Empty,
                    Regiao = cargaEntrega.Cliente?.Regiao?.Descricao ?? string.Empty,
                    Parqueada = cargaEntrega.Carga.Parqueada.HasValue ? cargaEntrega.Carga.Parqueada.Value ? "Sim" : "Não" : string.Empty,
                    CodigoIntegracaoCliente = cargaEntrega.Cliente?.CodigoIntegracao ?? "",
                    CodigoIntegracaoFilial = cargaEntrega.Carga.Filial != null ? (string.IsNullOrEmpty(cargaEntrega.Carga.Filial.CodigoFilialEmbarcador)
                                        ? (cargaEntrega.Carga.Filial.OutrosCodigosIntegracao != null && cargaEntrega.Carga.Filial.OutrosCodigosIntegracao.Any()
                                        ? string.Join(", ", cargaEntrega.Carga.Filial.OutrosCodigosIntegracao)
                                        : "") : (cargaEntrega.Carga.Filial.OutrosCodigosIntegracao != null && cargaEntrega.Carga.Filial.OutrosCodigosIntegracao.Any()
                                        ? $"{cargaEntrega.Carga.Filial.CodigoFilialEmbarcador}, {string.Join(", ", cargaEntrega.Carga.Filial.OutrosCodigosIntegracao)}"
                                        : cargaEntrega.Carga.Filial.CodigoFilialEmbarcador)) : "",
                    DataEmissaoNota = String.Join(", ", cargaEntregaNotaFiscaisComLimiteDeRegistros.Select(notas => notas?.PedidoXMLNotaFiscal?.XMLNotaFiscal?.DataEmissao.ToString("dd/MM/yyyy HH:mm"))),
                    DataRejeicaoEntrega = cargaEntrega.DataRejeitado?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataConfirmacaoEntrega = cargaEntrega.DataConfirmacaoEntrega.HasValue ? cargaEntrega.DataConfirmacaoEntrega.Value.ToDateTimeString() : " - "
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
                adminUnitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDestinatario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCargaEntrega = Request.GetIntParam("Entrega");
                double codigoCliente = Request.GetDoubleParam("Destinatario");
                decimal peso = Request.GetDecimalParam("Peso");

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);
                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(codigoCliente);

                if (entrega == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (cliente == null)
                    throw new ControllerException(Localization.Resources.Cargas.ControleEntrega.ClienteNaoLocalizado);

                entrega.Cliente = cliente;
                repCargaEntrega.Atualizar(entrega);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(entrega, repCargaEntrega, unitOfWork);

                if (peso > 0)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaEntregaPedido.BuscarPedidosPorCargaEntrega(entrega.Codigo);
                    decimal pesoRateado = Math.Round(peso / cargaPedidos.Count, 2, MidpointRounding.AwayFromZero);
                    decimal pesoSumarizado = 0;
                    for (int i = 0, s = cargaPedidos.Count; i < s; i++)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedidos[i].Pedido;

                        bool isUltimoPedido = i == (s - 1);

                        pedido.Destinatario = cliente;
                        pedido.PesoTotal = !isUltimoPedido ? pesoRateado : peso - pesoSumarizado;
                        repPedido.Atualizar(pedido);

                        pesoSumarizado += pedido.PesoTotal;
                    }
                }

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.NotificarCargaEntregaAlterada(entrega, Cliente, unitOfWork);

                return new JsonpResult(new
                {
                    NomeRecebedor = cliente.Nome,
                    DocumentoRecebedor = cliente.CPF_CNPJ_Formatado
                });
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(null, true, excecao.Message);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(null, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoAdicionarPedidoNaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> SalvarObservacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigo);
                if (cargaEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.ControleDeEntregaNaoFoiEncontrado);

                cargaEntrega.Observacao = Request.GetStringParam("Observacao");

                repCargaEntrega.Atualizar(cargaEntrega);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoSalvar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> ReenviarEmail()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);

                int codigoOcorrencia = Request.GetIntParam("CodigoOcorrencia");
                int codigoPedido = Request.GetIntParam("CodigoPedido");

                Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega ocorrencia = repPedidoOcorrenciaColetaEntrega.BuscarPorPedidoTipoOcorrencia(codigoOcorrencia, codigoPedido);

                if (ocorrencia == null)
                    return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorrenciaNaoEncontrada);

                ocorrencia.PendenteEnvioEmail = true;
                repPedidoOcorrenciaColetaEntrega.Atualizar(ocorrencia);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoReenviarEmail);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> RemoverReentrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigo = Request.GetIntParam("Codigo");

                if (!ConfiguracaoEmbarcador.PermiteRemoverReentrega)
                    return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.ConfiguracaoAtualNaoPermiteRemoverReentregas);

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> listaPedidos = repositorioCargaEntregaPedido.BuscarPorCargaEntrega(codigo);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                bool permiteRemoverReentrega = (cargaEntrega.Reentrega && !cargaEntrega.Carga.DataInicioViagem.HasValue && !cargaEntrega.Coleta && ConfiguracaoEmbarcador.PermiteRemoverReentrega);

                if (!permiteRemoverReentrega)
                    return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.NaoPossivelRemoverEntregaNoEstadoAtual);

                //TODO: Deve-se excluir apenas cargaPedido que nao estao vinculados a outras entregas.

                //List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = listaPedidos.Select(x => x.CargaPedido).Distinct().ToList();
                //foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargaPedido)
                //{
                //    // verificar se cargaPedido nao esta vinculado a outra entrega, caso sim, remover apenas dados vinculados a esta; senao pode remover.
                //    int CodigosEntrega = repositorioCargaEntregaPedido.VerificarCargaEntregaPedidoPossuiDiferentesEntregas(cargaPedido.Codigo, codigo);
                //    if (CodigosEntrega > 0)
                //    {
                // AQUI REVER A REGRA PARA O QUE DEVE SER NECESSARIO EXCLUIR DESTA CARGAPEDIDO, PARA NAO GERAR NOVAMENTE NA ROTERIZACAO.              
                //    }
                //    else
                //        Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoCarga(cargaEntrega.Carga, cargaPedido, ConfiguracaoEmbarcador, TipoServicoMultisoftware, unitOfWork, true);
                //}

                Servicos.Embarcador.Carga.CargaPedido.RemoverPedidosCarga(cargaEntrega.Carga, listaPedidos.Select(x => x.CargaPedido).Distinct().ToList(), ConfiguracaoEmbarcador, TipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga, true);

                cargaEntrega.Carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Aguardando;

                repositorioCarga.Atualizar(cargaEntrega.Carga);

                int codigoEntrega = cargaEntrega.Codigo;
                int codigoCarga = cargaEntrega.Carga.Codigo;

                string pedidos = string.Join(",", (from obj in listaPedidos select obj.CargaPedido.Pedido.NumeroPedidoEmbarcador).ToString());

                repositorioCargaEntrega.Deletar(cargaEntrega);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntrega.Carga, string.Format(Localization.Resources.Cargas.ControleEntrega.RemoveuEntrega, "(" + pedidos + ")"), unitOfWork);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.NotificarCargaEntregaRemovida(codigoEntrega, codigoCarga, Cliente, unitOfWork);

                return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.ReentregaRemovidaComSucesso);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoRemoverReentrega);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> RemoverPedidoReentrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCargaEntrega = Request.GetIntParam("CodigoCargaEntrega");
                int codigoCargaEntregaPedido = Request.GetIntParam("CodigoCargaEntregaPedido");
                int codigoPedidoReentrega = Request.GetIntParam("CodigoPedidoReentrega");

                if (!ConfiguracaoEmbarcador.PermiteRemoverReentrega)
                    return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.ConfiguracaoAtualNaoPermiteRemoverReentregas);

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido = repositorioCargaEntregaPedido.BuscarPorCodigo(codigoCargaEntregaPedido);

                bool permiteRemoverPedidoReentrega = (cargaEntrega.PossuiPedidosReentrega && (cargaEntrega.Situacao == SituacaoEntrega.Reentergue || SituacaoEntregaHelper.ObterSituacaoEntregaEmAberto(cargaEntrega.Situacao)) && !cargaEntrega.Coleta);

                if (!permiteRemoverPedidoReentrega)
                    return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.NaoFoiPossivelRemoverReentregaNoEstadoAtual);

                unitOfWork.Start();

                Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoCarga(cargaEntrega.Carga, cargaEntregaPedido.CargaPedido, ConfiguracaoEmbarcador, TipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga, true);

                repositorioCarga.Atualizar(cargaEntrega.Carga);

                string numeroPedido = cargaEntregaPedido.CargaPedido.Pedido.NumeroPedidoEmbarcador;

                Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(cargaEntrega.Carga, unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntrega.Carga, string.Format(Localization.Resources.Cargas.ControleEntrega.RemoveuPedidoDeReentrega, numeroPedido, cargaEntrega.Codigo.ToString()), unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.PedidoDeReentregaRemovidoComSucesso);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoRemoverPedidoDeReentrega);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> RemoverEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                bool isNotaCoberturaEPodeAlterarEntrega = cargaEntrega.PossuiNotaCobertura && cargaEntrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue;

                if (!isNotaCoberturaEPodeAlterarEntrega)
                    return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.SituacaoNaoPermiteRemoverEntrega);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> listaPedidos = repositorioCargaEntregaPedido.BuscarPorCargaEntrega(codigo);

                Servicos.Embarcador.Carga.CargaPedido.RemoverPedidosCarga(cargaEntrega.Carga, listaPedidos.Select(x => x.CargaPedido).ToList(), ConfiguracaoEmbarcador, TipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga, true);

                cargaEntrega.Carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Aguardando;

                repositorioCarga.Atualizar(cargaEntrega.Carga);

                int codigoEntrega = cargaEntrega.Codigo;
                int codigoCarga = cargaEntrega.Carga.Codigo;

                repositorioCargaEntrega.Deletar(cargaEntrega);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.NotificarCargaEntregaRemovida(codigoEntrega, codigoCarga, Cliente, unitOfWork);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoRemoverReentrega);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> EnviarImagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntraga = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                Servicos.DTO.CustomFile arquivo = HttpContext.GetFile();
                int codigo = Request.GetIntParam("Codigo");

                bool arquivoPDF = arquivo.ContentType.ToString().Contains("pdf");

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagem(arquivo.InputStream, unitOfWork, out string tokenImagem, arquivoPDF);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagemEntrega(1, codigo, tokenImagem, unitOfWork, DateTime.Now, 0, 0, (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? OrigemSituacaoEntrega.UsuarioPortalTransportador : OrigemSituacaoEntrega.UsuarioMultiEmbarcador), false, Auditado, arquivoPDF ? ".pdf" : ".jpg");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntraga.BuscarPorCodigo(codigo);

                dynamic retorno = (from o in cargaEntrega.Fotos
                                   select new
                                   {
                                       o.Codigo,
                                       DataRecebimento = o.DataEnvioImagem.ToString("dd/MM/yyyy HH:mm"),
                                       Miniatura = Base64ImagemAnexo(o, unitOfWork)
                                   }).ToList();

                return new JsonpResult(retorno);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoConfirmarEntrega);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> EnviarImagemAssinatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntraga = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.UsuarioSemPermissaoParaExecutarEssaAcao);

                int codigo = Request.GetIntParam("Codigo");
                string imagemBase64 = Request.GetStringParam("Imagem");

                Stream imagem = new MemoryStream(Convert.FromBase64String(imagemBase64));

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagemAssinatura(imagem, unitOfWork, out string guid);
                if (!Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarAssinaturaProdutorColetaEntrega(1, codigo, guid, DateTime.Now, unitOfWork, out string mensagemErro))
                {
                    return new JsonpResult(false, mensagemErro);
                }

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel repCargaEntregaAssinaturaResponsavel = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel cargaEntregaAssinatura = repCargaEntregaAssinaturaResponsavel.BuscarPorCargaEntrega(codigo);

                return new JsonpResult(Base64ImagemAssinatura(cargaEntregaAssinatura, unitOfWork));
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoConfirmarEntrega);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }


        public async Task<IActionResult> ConfirmarEntrega()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/ControleEntrega", "Logistica/Monitoramento");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                DateTime dataInicioEntrega = Request.GetDateTimeParam("DataInicioEntregaInformada");
                DateTime dataEntrega = Request.GetDateTimeParam("DataEntregaInformada");
                int.TryParse(Request.Params("MotivoRetificacao"), out int motivoRetificacao);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigo);

                double.TryParse(Request.Params("AreaRedex"), out double clienteRedex);

                int.TryParse(Request.Params("ColetaContainer"), out int container);
                DateTime? dataColetaContainer = Request.GetNullableDateTimeParam("DataColetaContainer");

                bool iniciarViagemAutomaticamente = Request.GetBoolParam("IniciarViagemAutomaticamente");

                // Valida
                if ((!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaEntrega_PermiteIniciarFinalizarViagensEntregasManualmente)
                    && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    || (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && !ValidaPermissaoTransportador(cargaEntrega)
                    && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaEntrega_PermiteIniciarFinalizarViagensEntregasManualmente)))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.VoceNaoPossuiPermissaoParaExecutarEstaAcao);

                if (cargaEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.NaoFoiPossivelEncontrarEntrega);

                if (cargaEntrega?.Carga?.DataInicioViagem == null && !iniciarViagemAutomaticamente)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.NaoPossivelConfirmarEntregaPoisViagemNaoFoiIniciada);

                if (cargaEntrega?.Carga?.DataFimViagem != null && !(cargaEntrega?.Carga?.TipoOperacao?.PermitirAtualizarEntregasCargasFinalizadas ?? false))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.NaoPossivelConfirmarEntregaPoisViagfemJaFoiFinalizada);

                if (dataInicioEntrega == null || dataInicioEntrega == DateTime.MinValue || dataEntrega == null || dataEntrega == DateTime.MinValue)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.DevemSerInformadasAsDatasDeInicioFimDaEntrega);

                if (dataInicioEntrega < cargaEntrega?.Carga?.DataInicioViagem || dataEntrega < cargaEntrega?.Carga?.DataInicioViagem)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.InicioFimDaEntregaDeveSerMaiorQueInicioDaViagem);

                if (dataInicioEntrega > dataEntrega)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.InicioDaEntregaDeveSerMenorQueFimDaEntrega);

                if (dataInicioEntrega > DateTime.Now || dataEntrega > DateTime.Now)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.InicioFimDaEntregaDevemSerMenoresQueDataAtual);

                unitOfWork.Start();

                try
                {
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork).BuscarPrimeiroRegistro();
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarPorCodigoFetch(cargaEntrega.Carga.TipoOperacao?.Codigo ?? 0);

                    //O INICIO VIAGEM ACONTECE DENTRO DA FINALIZACAO DA ENTREGA.

                    //Montar produtos aqui quando for coleta.
                    List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Pedido> pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Pedido>();

                    OrigemSituacaoEntrega origemSituacaoEntrega = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) ?
                                OrigemSituacaoEntrega.UsuarioMultiEmbarcador : OrigemSituacaoEntrega.UsuarioPortalTransportador;

                    Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros parametros = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros
                    {
                        cargaEntrega = cargaEntrega,
                        dataInicioEntrega = dataInicioEntrega,
                        dataTerminoEntrega = dataEntrega,
                        dataConfirmacao = dataEntrega,
                        dataSaidaRaio = null,
                        wayPoint = null,
                        wayPointDescarga = null,
                        pedidos = pedidos,
                        motivoRetificacao = motivoRetificacao,
                        justificativaEntregaForaRaio = "",
                        motivoFalhaGTA = 0,
                        configuracaoEmbarcador = ConfiguracaoEmbarcador,
                        tipoServicoMultisoftware = TipoServicoMultisoftware,
                        sistemaOrigem = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                        dadosRecebedor = null,
                        OrigemSituacaoEntrega = origemSituacaoEntrega,
                        ClienteAreaRedex = clienteRedex,
                        Container = container,
                        DataColetaContainer = dataColetaContainer,
                        auditado = Auditado,
                        configuracaoControleEntrega = configuracaoControleEntrega,
                        tipoOperacaoParametro = tipoOperacaoParametro,
                        TornarFinalizacaoDeEntregasAssincrona = configuracaoControleEntrega.TornarFinalizacaoDeEntregasAssincrona
                    };

                    if (!(cargaEntrega?.Cliente?.NaoExigirDigitalizacaoDoCanhotoParaEsteCliente ?? false))
                        SetarCanhotosComoPendente(cargaEntrega, unitOfWork);

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(parametros, unitOfWork);

                    cargaEntrega.FinalizadaManualmente = true;
                    cargaEntrega.ResponsavelFinalizacaoManual = Usuario;
                    cargaEntrega.Observacao = Request.GetStringParam("Observacao");

                    repCargaEntrega.Atualizar(cargaEntrega);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntrega, string.Format(Localization.Resources.Cargas.ControleEntrega.OperadorConfirmouManualmenteEntrega, Usuario.Descricao), unitOfWork);

                    // Enviar notificacao para o novo app MTrack que a entrega foi atualizada
                    NotificacaoMTrack serNotificacaoMTrack = new NotificacaoMTrack(unitOfWork);
                    serNotificacaoMTrack.NotificarMudancaCargaEntrega(cargaEntrega, cargaEntrega.Carga.Motoristas.ToList(), AdminMultisoftware.Dominio.Enumeradores.MobileHubs.EntregaConfirmadaNoEmbarcador, notificarSignalR: true, codigoClienteMultisoftware: Empresa.Codigo);

                    unitOfWork.CommitChanges();
                }
                catch (ServicoException excecao)
                {
                    throw;
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.NotificarCargaEntregaAlterada(cargaEntrega, Cliente, unitOfWork);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoConfirmarEntrega);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> RejeitarEntrega()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/ControleEntrega", "Logistica/Monitoramento");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega repositorioMotivoDevolucaoEntrega = new Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega(unitOfWork);

                OrigemSituacaoEntrega origemSituacaoEntrega = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) ?
                                OrigemSituacaoEntrega.UsuarioMultiEmbarcador : OrigemSituacaoEntrega.UsuarioPortalTransportador;

                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros parametros = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros
                {
                    codigoCargaEntrega = Request.GetIntParam("Codigo"),
                    codigoMotivo = Request.GetIntParam("MotivoRejeicao"),
                    configuracao = ConfiguracaoEmbarcador,
                    data = DateTime.Now,
                    devolucaoParcial = Request.GetEnumParam("TipoDevolucao", TipoColetaEntregaDevolucao.Total) == TipoColetaEntregaDevolucao.Parcial,
                    motivoRetificacao = Request.GetIntParam("MotivoRetificacao"),
                    notasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal>(),
                    observacao = "",
                    permitirEntregarMaisTarde = Request.GetBoolParam("PermitirEntregarMaisTarde"),
                    produtos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto>(),
                    tipoServicoMultisoftware = TipoServicoMultisoftware,
                    usuario = this.Usuario,
                    OrigemSituacaoEntrega = origemSituacaoEntrega,
                    clienteMultisoftware = this.Cliente
                };

                dynamic itensDevolver = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ItensDevolver"));

                foreach (dynamic notaFiscal in itensDevolver.NotasFiscais)
                {
                    if (!parametros.devolucaoParcial)
                        notaFiscal.DevolucaoTotal = true;

                    if (!((string)notaFiscal.DevolucaoParcial).ToBool() && !((string)notaFiscal.DevolucaoTotal).ToBool())
                        continue;

                    Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal()
                    {
                        Codigo = ((string)notaFiscal.Codigo).ToInt(),
                        DevolucaoParcial = ((string)notaFiscal.DevolucaoParcial).ToBool(),
                        DevolucaoTotal = ((string)notaFiscal.DevolucaoTotal).ToBool(),
                        MotivoDevolucaoEntrega = repositorioMotivoDevolucaoEntrega.BuscarPorCodigo(((string)notaFiscal.MotivoDevolucaoEntrega).ToInt()),
                        Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto>(),
                    };

                    foreach (dynamic produto in notaFiscal.Produtos)
                        cargaEntregaNotaFiscal.Produtos.Add(new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto()
                        {
                            Protocolo = ((string)produto.Codigo).ToInt(),
                            QuantidadeDevolucao = ((string)produto.QuantidadeDevolucao).ToDecimal(),
                            Lote = ((string)produto.Lote),
                            DataCritica = ((string)produto.DataCritica).ToNullableDateTime(),
                            ValorDevolucao = ((decimal)produto.ValorDevolucao),
                        });

                    parametros.notasFiscais.Add(cargaEntregaNotaFiscal);
                }

                foreach (dynamic produto in itensDevolver.Produtos)
                    parametros.produtos.Add(new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto()
                    {
                        Protocolo = ((string)produto.Codigo).ToInt(),
                        QuantidadeDevolucao = ((string)produto.QuantidadeDevolucao).ToDecimal()
                    });

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(parametros.codigoCargaEntrega);

                if ((!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaEntrega_PermiteIniciarFinalizarViagensEntregasManualmente) && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) || (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && !ValidaPermissaoTransportador(cargaEntrega) && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaEntrega_PermiteIniciarFinalizarViagensEntregasManualmente)))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.VoceNaoPossuiPermissaoParaExecutarEstaAcao);

                if (cargaEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.NaoFoiPossivelEncontrarEntrega);

                if (cargaEntrega?.Carga?.DataInicioViagem == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.NaoPossivelDevolverEntregaNaoIniciada);

                if (cargaEntrega?.Carga?.DataFimViagem != null && !(cargaEntrega?.Carga?.TipoOperacao?.PermitirAtualizarEntregasCargasFinalizadas ?? false))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.NaoPossivelDevolverEntregaComViagemFinalizada);

                if (parametros.codigoMotivo <= 0)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.ObrigatorioInformarMotivoDaRejeicao);

                unitOfWork.Start();

                try
                {
                    Dominio.Entidades.Embarcador.Chamados.Chamado chamado;

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.RejeitarEntrega(parametros, Auditado, unitOfWork, out chamado, TipoServicoMultisoftware);

                    cargaEntrega.FinalizadaManualmente = true;
                    cargaEntrega.ResponsavelFinalizacaoManual = Usuario;
                    cargaEntrega.Observacao = Request.GetStringParam("Observacao");

                    repositorioCargaEntrega.Atualizar(cargaEntrega);
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntrega, string.Format(Localization.Resources.Cargas.ControleEntrega.OperadorRejeitouManualmenteEntrega, Usuario.Descricao), unitOfWork);

                    if (cargaEntrega.Situacao == SituacaoEntrega.Rejeitado)
                    {
                        // Enviar notificacao para o novo app MTrack que a entrega foi atualizada
                        NotificacaoMTrack serNotificacaoMTrack = new NotificacaoMTrack(unitOfWork);
                        serNotificacaoMTrack.NotificarMudancaCargaEntrega(cargaEntrega, cargaEntrega.Carga.Motoristas.ToList(), AdminMultisoftware.Dominio.Enumeradores.MobileHubs.EntregaRejeitadaNoEmbarcador, notificarSignalR: true, codigoClienteMultisoftware: Empresa.Codigo);
                    }

                    unitOfWork.CommitChanges();

                    if (chamado != null)
                    {
                        if (chamado.MotivoChamado.GerarCargaDevolucaoSeAprovado)
                        {
                            int codigoChamado = chamado.Codigo;
                            string stringConexao = unitOfWork.StringConexao;
                            Task t = Task.Factory.StartNew(() => { Servicos.Embarcador.Chamado.Chamado.EnviarEmailCargaDevolucao(codigoChamado, stringConexao); });
                        }

                        Servicos.Embarcador.Chamado.Chamado.NotificarChamadoAdicionadoOuAtualizado(chamado, unitOfWork);
                        new Servicos.Embarcador.Chamado.Chamado(unitOfWork).EnviarEmailChamadoAberto(chamado, unitOfWork);
                    }
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.NotificarCargaEntregaAlterada(cargaEntrega, Cliente, unitOfWork);

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                return new JsonpResult(false, true, ex.Message);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoRejeitarEntrega);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> AlterarDatasEntrega(CancellationToken cancellationToken)
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/ControleEntrega");

            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaEntrega_PermiteAlterarDataEntrega) && !this.Usuario.UsuarioAdministrador)
                return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.VoceNaoPossuiPermissaoParaExecutarEstaAcao);

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = await repositorioCargaEntrega.BuscarPorCodigoAsync(codigo, true);

                if (cargaEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.NaoFoiPossivelEncontrarEntrega);

                cargaEntrega.Initialize();

                DateTime? dataInicioEntrega = Request.GetNullableDateTimeParam("DataInicioEntregaInformada");
                DateTime? dataEntrega = Request.GetNullableDateTimeParam("DataEntregaInformada");
                DateTime? dataEntradaRaio = Request.GetNullableDateTimeParam("DataEntradaRaio");
                DateTime? dataSaidaRaio = Request.GetNullableDateTimeParam("DataSaidaRaio");

                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Transbordo repositorioTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = await repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistroAsync();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = await repositorioConfiguracaoControleEntrega.BuscarPrimeiroRegistroAsync();

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (dataInicioEntrega.HasValue && dataEntrega.HasValue && dataInicioEntrega.Value > dataEntrega.Value)
                        return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.DataInicialMaiorQueAFinal);
                    if (dataEntrega.HasValue && dataEntrega.Value > DateTime.Now)
                        return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.DataFinalMaiorQueDataAtual);
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaAnterior = repositorioCargaEntrega.BuscarEntregaAnteriorPorCarga(cargaEntrega.Carga.Codigo, cargaEntrega.Ordem);
                    if (cargaEntregaAnterior != null)
                    {
                        if (dataInicioEntrega.HasValue && cargaEntregaAnterior.DataFim > dataInicioEntrega.Value)
                            return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.DataInferiorAEtapaAnterior);
                        if (dataEntrega.HasValue && cargaEntregaAnterior.DataFim > dataEntrega.Value)
                            return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.DataInferiorAEtapaAnterior);
                        if (dataInicioEntrega.HasValue && cargaEntregaAnterior.DataInicio > dataInicioEntrega.Value)
                            return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.DataInferiorAEtapaAnterior);
                        if (dataEntrega.HasValue && cargaEntregaAnterior.DataInicio > dataEntrega.Value)
                            return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.DataInferiorAEtapaAnterior);
                    }
                    if (dataEntrega.HasValue && dataInicioEntrega.HasValue)
                    {
                        double diferencaDias = (dataEntrega.Value - dataInicioEntrega.Value).TotalDays;
                        if (diferencaDias >= 60)
                            return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.DataComDiferencaDeSessentaDias);
                    }
                }

                if (dataInicioEntrega.HasValue)
                    cargaEntrega.DataInicio = dataInicioEntrega.Value;

                if (dataEntrega.HasValue)
                {
                    cargaEntrega.DataConfirmacao = dataEntrega.Value;
                    cargaEntrega.DataFim = dataEntrega.Value;

                    repositorioPedido.DefinirDataEntregaPorCargaEntrega(cargaEntrega.Codigo, dataEntrega);
                }

                if (dataEntradaRaio.HasValue)
                    cargaEntrega.DataEntradaRaio = dataEntradaRaio;

                if (dataSaidaRaio.HasValue)
                    cargaEntrega.DataSaidaRaio = dataSaidaRaio;

                await repositorioCargaEntrega.AtualizarAsync(cargaEntrega, Auditado);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork);


                int codigoCargaBase;

                if (cargaEntrega.Carga.CargaTransbordo)
                {
                    Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo = repositorioTransbordo.BuscarPorCargaGerada(cargaEntrega.Carga.Codigo);
                    codigoCargaBase = transbordo?.Carga?.Codigo ?? 0;
                }
                else
                    codigoCargaBase = cargaEntrega.Carga.Codigo;

                List<Dominio.Entidades.Embarcador.Cargas.Transbordo> transbordos = await repositorioTransbordo.BuscarTodosTransbordosPorCargaAsync(codigoCargaBase);
                Dominio.Entidades.Embarcador.Cargas.Carga cargaOriginal = await repositorioCarga.BuscarPorCodigoAsync(codigoCargaBase);

                List<(int CodigoCarga, double CpfCnpj)> cargasParaAtualizar = new();

                if (cargaOriginal != null)
                    cargasParaAtualizar.Add((cargaOriginal.Codigo, cargaEntrega.Cliente?.CPF_CNPJ ?? 0));

                if (transbordos != null)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo in transbordos)
                    {
                        if (transbordo.Carga != null)
                            cargasParaAtualizar.Add((transbordo.CargaGerada?.Codigo ?? 0, cargaEntrega.Cliente?.CPF_CNPJ ?? 0));
                    }
                }

                foreach (var (codigoCarga, cpfCnpj) in cargasParaAtualizar)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntregasAtualizar = repositorioCargaEntrega.BuscarCargaeCliente(codigoCarga, cpfCnpj);

                    if (cargasEntregasAtualizar != null)
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaAtualizar in cargasEntregasAtualizar)
                        {
                            cargaEntregaAtualizar.Initialize();

                            if (dataInicioEntrega.HasValue)
                                cargaEntregaAtualizar.DataInicio = dataInicioEntrega.Value;

                            if (dataEntrega.HasValue)
                            {
                                cargaEntregaAtualizar.DataConfirmacao = dataEntrega.Value;
                                cargaEntregaAtualizar.DataFim = dataEntrega.Value;
                            }

                            if (dataEntradaRaio.HasValue)
                                cargaEntregaAtualizar.DataEntradaRaio = dataEntradaRaio;

                            if (dataSaidaRaio.HasValue)
                                cargaEntregaAtualizar.DataSaidaRaio = dataSaidaRaio;

                            await repositorioCargaEntrega.AtualizarAsync(cargaEntregaAtualizar, Auditado);
                            repositorioPedido.DefinirDataEntregaPorCargaEntrega(cargaEntregaAtualizar.Codigo, dataEntrega);
                        }
                    }
                }

                if (!configuracaoGeralCarga?.UtilizaControleDeEntregaManual ?? false)
                    Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega.GerarEventosColetaEntregaAtualizacaoDatasEntrega(cargaEntrega, dataEntrega.Value, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Cliente, Auditado?.OrigemAuditado ?? OrigemAuditado.Sistema, configuracaoControleEntrega, unitOfWork);

                new Servicos.Embarcador.Carga.ControleEntrega.ControleEntregaQualidade(unitOfWork, null).ProcessarRegrasDeQualidadeDeEntrega(cargaEntrega);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoAlterarDataDaEntrega);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }


        public async Task<IActionResult> AdicionarOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repositorioConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.Embarcador.Frete.ComponenteFrete repositorioComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repositorioOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Ocorrencia"), out int ocorrencia);
                int.TryParse(Request.Params("TipoOcorrencia"), out int codigoTipoOcorrencia);
                IEnumerable<Servicos.DTO.CustomFile> anexosOcorrencia = HttpContext.GetFiles("AnexosOcorrencia");

                int codigoChamado = Request.GetIntParam("CodigoChamado");
                int codigoComponenteFrete = Request.GetIntParam("ComponenteFrete");
                decimal valor = Request.GetDecimalParam("ValorOcorrencia");

                DateTime data = Request.GetDateTimeParam("DataOcorrencia");
                string observacaoOcorrencia = Request.GetStringParam("ObservacaoOcorrencia");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigo);

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repositorioChamado.BuscarPorCodigo(codigoChamado);
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repositorioComponenteFrete.BuscarPorCodigo(codigoComponenteFrete);

                // Valida
                if (cargaEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.NaoFoiPossivelEncontrarEntrega);

                if (cargaEntrega?.Carga?.DataFimViagem != null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.NaoPossivelDevolverEntregaComViagemFinalizada);

                if (data <= DateTime.MinValue)
                    data = DateTime.Now;

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoBase = cargaEntrega.Pedidos.FirstOrDefault()?.CargaPedido?.Pedido;

                OrigemSituacaoEntrega origem = OrigemSituacaoEntrega.UsuarioMultiEmbarcador;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    origem = OrigemSituacaoEntrega.UsuarioPortalTransportador;

                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(codigoTipoOcorrencia);

                if (tipoOcorrencia == null)
                    tipoOcorrencia = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(ocorrencia);

                bool existeOcorrenciaEntrega = repositorioOcorrenciaColetaEntrega.ExisteOcorrenciaPorCargaEntregaETipoOcorrencia(cargaEntrega.Codigo, data, tipoOcorrencia.Codigo);

                if (existeOcorrenciaEntrega)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.ExisteOcorrenciaComEntregaParaDataInformada);

                try
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> configuracaoOcorrenciaEntrega = repositorioConfiguracaoOcorrenciaEntrega.BuscarPorTipoOcorrencia(tipoOcorrencia?.Codigo ?? 0);

                    if (configuracaoOcorrenciaEntrega != null && configuracaoOcorrenciaEntrega.Count > 0)
                    {
                        List<EventoColetaEntrega> listaEventoColetaEntrega = configuracaoOcorrenciaEntrega.Select(evento => evento.EventoColetaEntrega).Distinct().ToList();
                        foreach (EventoColetaEntrega eventoColetaEntrega in listaEventoColetaEntrega)
                        {
                            unitOfWork.Start();
                            Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega.GerarOcorrenciaRejeicao(cargaEntrega, data, tipoOcorrencia, null, null, observacaoOcorrencia, valor, ConfiguracaoEmbarcador, TipoServicoMultisoftware, this.Cliente, origem, unitOfWork, Auditado, componenteFrete, chamado, eventoColetaEntrega, null, null, anexosOcorrencia);
                            unitOfWork.CommitChanges();
                        }
                    }
                    else
                    {
                        unitOfWork.Start();
                        Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega.GerarOcorrenciaRejeicao(cargaEntrega, data, tipoOcorrencia, null, null, observacaoOcorrencia, valor, ConfiguracaoEmbarcador, TipoServicoMultisoftware, this.Cliente, origem, unitOfWork, Auditado, componenteFrete, chamado, EventoColetaEntrega.Todos, null, null, anexosOcorrencia);
                        unitOfWork.CommitChanges();
                    }

                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }

                string linkOcorrencia = "";
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> ocorrenciaColetaEntregas = repOcorrenciaColetaEntrega.BuscarPorControleEntrega(codigo);
                if (pedidoBase != null)
                {
                    string codigoRastreamento = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterCodigoRastreamentoPedido(pedidoBase, unitOfWork);
                    string urlBase = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLBase(Cliente.Codigo, TipoServicoMultisoftware, adminUnitOfWork, _conexao.AdminStringConexao, unitOfWork);
                    linkOcorrencia = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLRastreamentoPedido(codigoRastreamento, urlBase);
                }

                var retorno = new
                {
                    Ocorrencias = (
                       from o in ocorrenciaColetaEntregas
                       select new
                       {
                           o.Codigo,
                           CodigoOcorrencia = o.TipoDeOcorrencia?.Codigo ?? 0,
                           CodigoPedido = pedidoBase?.Codigo ?? 0,
                           Descricao = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterDescricaoPortalOcorrencia(o.TipoDeOcorrencia, o.CargaEntrega.Carga, o.CargaEntrega.Cliente, pedidoBase?.Remetente, true),
                           DataOcorrencia = o.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                           LinkOcorrencia = linkOcorrencia,
                           DataPosicao = o.DataPosicao.HasValue ? o.DataPosicao.Value.ToString("dd/MM/yyyy HH:mm") : "",
                           DataReprogramada = o.DataPrevisaoRecalculada.HasValue ? o.DataPrevisaoRecalculada.Value.ToString("dd/MM/yyyy HH:mm") : "",
                           TempoPercurso = o.TempoPercurso,
                           Distancia = (o.DistanciaAteDestino > 0 ? (o.DistanciaAteDestino / 1000).ToString("n3") : "0") + " KM",
                           Latitude = o.Latitude,
                           Longitude = o.Longitude,
                           Pacote = o.Pacote,
                           Volumes = o.Volumes,
                           Origem = o.OrigemOcorrencia.HasValue ? OrigemCriacaoOcorrenciaHelper.ObterDescricao(o.OrigemOcorrencia.Value) : "",
                       }
                    ).ToList()
                };
                Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia repGatilhoGeracaoAutomaticaOcorrencia = new Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilho = repGatilhoGeracaoAutomaticaOcorrencia.BuscarPorTipoOcorrencia(tipoOcorrencia.Codigo);

                if (gatilho != null && gatilho.AtribuirDataOcorrenciaNaDataAgendamentoTransportador)
                {
                    try
                    {
                        unitOfWork.Start();
                        cargaEntrega.DataAgendamentoEntregaTransportador = data;
                        cargaEntrega.OrigemCriacaoDataAgendamentoEntregaTransportador = OrigemCriacao.WebService;
                        repCargaEntrega.Atualizar(cargaEntrega, Auditado, null, "Data de agendamento de entrega do transportador alterada automaticamente, mesma data da ocorrencia");
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);
                        unitOfWork.CommitChanges();
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                }


                return new JsonpResult(retorno);
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
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoAdicionarOcorrencia);
            }
            finally
            {
                unitOfWork.Dispose();
                adminUnitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> AtualizarCoordenadas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.UsuarioSemPermissaoParaExecutarEssaAcao);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigo);

                // Valida
                if (cargaEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.NaoFoiPossivelEncontrarEntrega);


                if ((cargaEntrega.LatitudeFinalizada == null) || (cargaEntrega.LongitudeFinalizada == null))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.CoordenadasDoVeiculoInvalida);


                DateTime data = DateTime.Now;

                unitOfWork.Start();
                try
                {
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarCoordenadaClientePelaEntrega(codigo, Auditado, unitOfWork);

                    unitOfWork.CommitChanges();
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoRejeitarEntrega);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> AtualizarProdutos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigo);

                if (cargaEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.UsuarioSemPermissaoParaExecutarEssaAcao);

                Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade repositorioCargaPedidoProdutoDivisaoCapacidade = new Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoProduto repositorioCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> listaCargaPedidoProduto = repositorioCargaPedidoProduto.BuscarPorCarga(cargaEntrega.Carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade> listaDivisaoCapacidade = cargaEntrega.Carga.ModeloVeicularCarga?.DivisoesCapacidade?.ToList() ?? new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade>();
                dynamic produtos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Produtos"));

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> listaDivisoesCapacidade = repositorioCargaPedidoProdutoDivisaoCapacidade.BuscarPorCargasPedidoProduto(listaCargaPedidoProduto.Select(obj => obj.Codigo).ToList());

                foreach (dynamic produto in produtos)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto = (from o in listaCargaPedidoProduto where o.Codigo == ((string)produto.Codigo).ToInt() select o).FirstOrDefault();

                    if (cargaPedidoProduto == null)
                        continue;

                    cargaPedidoProduto.Initialize();

                    cargaPedidoProduto.Quantidade = ((string)produto.Quantidade).ToDecimal();
                    cargaPedidoProduto.QuantidadeCaixa = ((string)produto.QuantidadeCaixa).ToInt();
                    cargaPedidoProduto.QuantidadePorCaixaRealizada = ((string)produto.QuantidadePorCaixaRealizada).ToInt();
                    cargaPedidoProduto.QuantidadeCaixasVaziasRealizada = ((string)produto.QuantidadeCaixasVaziasRealizada).ToInt();
                    cargaPedidoProduto.Temperatura = ((string)produto.Temperatura).ToDecimal();

                    repositorioCargaPedidoProduto.Atualizar(cargaPedidoProduto);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedidoProduto, cargaPedidoProduto.GetChanges(), "Salvou o produto", unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> divisoesPedidoProduto = (from o in listaDivisoesCapacidade where o.CargaPedidoProduto.Codigo == cargaPedidoProduto.Codigo select o).ToList();

                    List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> valoresAlterados = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

                    foreach (dynamic divisaoCapacidade in produto.DivisoesCapacidade)
                    {
                        int codigoRegistro = ((string)divisaoCapacidade.Codigo).ToInt();

                        Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade modeloVeicularCargaDivisaoCapacidade = (from o in listaDivisaoCapacidade where o.Codigo == codigoRegistro select o).FirstOrDefault();
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade produtoDivisaoCapacidade = (from o in divisoesPedidoProduto where o.ModeloVeicularCargaDivisaoCapacidade.Codigo == codigoRegistro select o).FirstOrDefault();

                        if (modeloVeicularCargaDivisaoCapacidade == null)
                            continue;
                        else if (produtoDivisaoCapacidade != null)
                            divisoesPedidoProduto.Remove(produtoDivisaoCapacidade);
                        else
                            produtoDivisaoCapacidade = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade();

                        produtoDivisaoCapacidade.Initialize();

                        decimal quantidade = ((string)divisaoCapacidade.Quantidade).ToDecimal();
                        decimal quantidadePlanejada = ((string)divisaoCapacidade.QuantidadePlanejada).ToDecimal();

                        produtoDivisaoCapacidade.CargaPedidoProduto = cargaPedidoProduto;
                        produtoDivisaoCapacidade.ModeloVeicularCargaDivisaoCapacidade = modeloVeicularCargaDivisaoCapacidade;
                        produtoDivisaoCapacidade.Quantidade = quantidade;
                        produtoDivisaoCapacidade.QuantidadePlanejada = quantidadePlanejada;

                        if (produtoDivisaoCapacidade.Codigo > 0)
                            repositorioCargaPedidoProdutoDivisaoCapacidade.Atualizar(produtoDivisaoCapacidade);
                        else
                            repositorioCargaPedidoProdutoDivisaoCapacidade.Inserir(produtoDivisaoCapacidade);

                        foreach (Dominio.Entidades.Auditoria.HistoricoPropriedade alteracao in produtoDivisaoCapacidade.GetChanges())
                            valoresAlterados.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                            {
                                De = alteracao.De,
                                Para = alteracao.Para,
                                Propriedade = $"Divisão {produtoDivisaoCapacidade.Codigo} - {alteracao.Propriedade}"
                            });
                    }

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade divisaoExcluir in divisoesPedidoProduto)
                        repositorioCargaPedidoProdutoDivisaoCapacidade.Deletar(divisaoExcluir);

                    cargaPedidoProduto.SetExternalChanges(valoresAlterados);
                    cargaPedidoProduto.SetChanges();

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedidoProduto, cargaPedidoProduto.GetChanges(), "Salvou divisões de capacidade", unitOfWork);
                }

                Servicos.Log.TratarErro($"AtualizarProdutos - CodigoCargaEntrega: {codigo}");

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoAtualizarOsProdutos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> SalvarChecklistProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList repCargaEntregaCheckList = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList(unitOfWork);

            try
            {
                int codigo = Request.GetIntParam("CargaEntrega");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigo);

                if (cargaEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                TipoCheckList tipoCheckList = cargaEntrega.Coleta ? TipoCheckList.Coleta : TipoCheckList.Entrega;

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList checkList = repCargaEntregaCheckList.BuscarPorCargaEntrega(codigo, tipoCheckList);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.UsuarioSemPermissaoParaExecutarEssaAcao);

                if (checkList == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RespostaCheckList> checkListRespostas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RespostaCheckList>>(Request.GetStringParam("CheckList"));

                unitOfWork.Start();

                new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaCheckList(unitOfWork, Auditado).SalvarRespostasCheckList(checkList, checkListRespostas);
                new Servicos.Embarcador.GestaoPatio.EmailCheckList(unitOfWork, Auditado).EnviarEmailCheckList(checkList);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoSalvarPesquisa);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarChecklistDesembarque()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("CargaEntrega");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList repCargaEntregaCheckList = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta repCargaEntregaCheckListPergunta = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa repCargaEntregaCheckListAlternativa = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList checkList = repCargaEntregaCheckList.BuscarPorCargaEntrega(codigo, TipoCheckList.Desembarque);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.UsuarioSemPermissaoParaExecutarEssaAcao);

                if (checkList == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                string jsonRespostas = Request.GetStringParam("CheckList");
                List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RespostaCheckList> checkListRespostas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RespostaCheckList>>(jsonRespostas);

                unitOfWork.Start();

                new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaCheckList(unitOfWork, Auditado).SalvarRespostasCheckList(checkList, checkListRespostas);
                new Servicos.Embarcador.GestaoPatio.EmailCheckList(unitOfWork, Auditado).EnviarEmailCheckList(checkList);

                if (checkList.CargaEntrega.Carga.SituacaoCarga == SituacaoCarga.Encerrada)
                    new Servicos.Embarcador.GestaoPatio.EmailCheckList(unitOfWork, Auditado).EnviarEmailCheckListSetoresTipoOperacao(checkList.CargaEntrega.Carga.Codigo, checkList.CargaEntrega.Carga.TipoOperacao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoSalvarPesquisa);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExibirAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto repCargaEntregaFoto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto anexo = repCargaEntregaFoto.BuscarPorCodigo(codigo);

                // Valida
                if (anexo == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.NaoFoiPossivelEncontrarEntrega);

                string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" });
                string extensao = Path.GetExtension(anexo.NomeArquivo);
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidArquivo + extensao);

                return File(arquivo, "image/jpeg");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadImagemNotaFiscal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal repCargaEntregaFotoNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal cargaEntregaFotoNF = repCargaEntregaFotoNotaFiscal.BuscarPorCodigo(codigo);

                if (cargaEntregaFotoNF == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.NaoFoiPossivelEncontrarImagem);

                string extensao = System.IO.Path.GetExtension(cargaEntregaFotoNF.NomeArquivo).ToLower();
                string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "CargaColetaEntrega", "NotasFiscais" });
                string nomeAbsolutoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{cargaEntregaFotoNF.GuidArquivo}{extensao}");

                if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeAbsolutoArquivo))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.NaoFoiPossivelEncontrarImagem);

                string nomeAbsolutoArquivoOriginal = Utilidades.IO.FileStorageService.Storage.Combine(caminho, cargaEntregaFotoNF.NomeArquivo);
                byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeAbsolutoArquivo);

                if (arquivoBinario == null)
                    return new JsonpResult(false, false, Localization.Resources.Cargas.ControleEntrega.NaoFoiPossivelBaixarImagem);

                return Arquivo(arquivoBinario, "application/jpg", System.IO.Path.GetFileName(nomeAbsolutoArquivoOriginal));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoRealizarDownloadDaImagem);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadImagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto repositorioCargaEntregaFoto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto cargaEntregaFoto = repositorioCargaEntregaFoto.BuscarPorCodigo(codigo);

                if (cargaEntregaFoto == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.NaoFoiPossivelEncontrarImagem);

                string extensao = System.IO.Path.GetExtension(cargaEntregaFoto.NomeArquivo).ToLower();
                string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" });
                string nomeAbsolutoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{cargaEntregaFoto.GuidArquivo}{extensao}");

                if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeAbsolutoArquivo))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.NaoFoiPossivelEncontrarImagem);

                string nomeAbsolutoArquivoOriginal = Utilidades.IO.FileStorageService.Storage.Combine(caminho, cargaEntregaFoto.NomeArquivo);
                byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeAbsolutoArquivo);

                if (arquivoBinario == null)
                    return new JsonpResult(false, false, Localization.Resources.Cargas.ControleEntrega.NaoFoiPossivelBaixarImagem);

                return Arquivo(arquivoBinario, "application/jpg", System.IO.Path.GetFileName(nomeAbsolutoArquivoOriginal));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoRealizarDownloadDaImagem);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterMiniaturasCanhotos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto srvCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigo);
                if (cargaEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.NaoFoiPossivelEncontrarEntrega);

                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosRetorno = servicoControleEntrega.ObterCanhotosDaCargaEntrega(cargaEntrega);
                List<Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo> canhotosEsperandoVinculo = ObterCanhotosEsperandoVinculoDaCargaEntrega(cargaEntrega, unitOfWork);

                List<dynamic> imagens = new List<dynamic>();

                imagens.AddRange((from canhoto in canhotosRetorno
                                  select new
                                  {
                                      canhoto.Codigo,
                                      canhoto.Numero,
                                      Miniatura = !canhoto.IsPDF() ? srvCanhoto.ObterMiniatura(canhoto, unitOfWork) : null,
                                      ArquivoPDF = canhoto.IsPDF(),
                                      EsperandoVinculo = false,
                                      Carga = canhoto.Carga?.Codigo ?? 0,
                                      Status = canhoto.SituacaoDigitalizacaoCanhoto
                                  }).ToList());

                imagens.AddRange((from canhoto in canhotosEsperandoVinculo
                                  select new
                                  {
                                      canhoto.Codigo,
                                      Numero = "Não definido",
                                      Miniatura = srvCanhoto.ObterMiniatura(canhoto, unitOfWork),
                                      ArquivoPDF = false,
                                      EsperandoVinculo = true,
                                      Carga = canhoto.CargaEntrega?.Carga?.Codigo ?? 0,
                                  }).ToList());

                return new JsonpResult(new
                {
                    Imagens = imagens
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoConsultasOsCanhotos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadCanhotosEmMassa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<int> codigosCanhotos = JsonConvert.DeserializeObject<List<int>>(Request.Params("CodigosCanhotos"));

                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> listaCanhotos = repCanhoto.BuscarPorCodigos(codigosCanhotos);

                if (listaCanhotos == null || listaCanhotos.Count == 0)
                    return new JsonpResult(false, true, "Registros não encontrados");

                Dictionary<string, byte[]> conteudoCompactar = new Dictionary<string, byte[]>();
                foreach (Canhoto canhoto in listaCanhotos)
                {
                    string caminho = retornarCaminhoCanhoto(canhoto, unitOfWork);
                    string extensao = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower();
                    string nomeAbsolutoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);
                    string nomeArquivo = canhoto.Numero + "_" + canhoto.Serie + "_" + canhoto.GuidNomeArquivo + extensao;
                    if (Utilidades.IO.FileStorageService.Storage.Exists(nomeAbsolutoArquivo))
                    {
                        string nomeAbsolutoArquivoOriginal = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.NomeArquivo);
                        byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeAbsolutoArquivo);
                        conteudoCompactar.Add(nomeArquivo, arquivoBinario);
                    }
                }

                if (conteudoCompactar?.Count == 0)
                    return new JsonpResult(false, true, "Não foi possível encontrar as imagens para realizar o download.");

                MemoryStream arquivoCompactado = Utilidades.File.GerarArquivoCompactado(conteudoCompactar);
                byte[] arquivoCompactadoBinario = arquivoCompactado.ToArray();

                arquivoCompactado.Dispose();

                if (arquivoCompactadoBinario == null)
                    return new JsonpResult(false, true, "Não foi possí­vel gerar o arquivo.");

                return Arquivo(arquivoCompactadoBinario, "application/zip", $"Canhotos-{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.zip");

            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> AtualizarDadosRecebedor()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string nome = Request.GetStringParam("DadosRecebedorNome");
                string cpf = Request.GetStringParam("DadosRecebedorCPF").Replace(".", "").Replace("-", "");
                DateTime dataEntrega = Request.GetDateTimeParam("DadosRecebedorDataEntrega");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.DadosRecebedor repDadosRecebedor = new Repositorio.Embarcador.Cargas.ControleEntrega.DadosRecebedor(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigo);

                if (cargaEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor dadosRecebedor = null;

                if (cargaEntrega.DadosRecebedor != null)
                {
                    dadosRecebedor = cargaEntrega.DadosRecebedor;
                }
                else
                {
                    dadosRecebedor = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor();
                }

                dadosRecebedor.Nome = nome;
                dadosRecebedor.CPF = cpf;
                dadosRecebedor.DataEntrega = dataEntrega;
                repDadosRecebedor.Inserir(dadosRecebedor);

                cargaEntrega.DadosRecebedor = dadosRecebedor;
                repositorioCargaEntrega.Atualizar(cargaEntrega);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarSituacaoOnTime()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/ControleEntrega");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaEntrega_PermiteAlterarSituacaoOnTime))
                    throw new ControllerException(Localization.Resources.Gerais.Geral.VoceNaoPossuiPermissaoParaExecutarEstaAcao);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repositorioConfiguracaoControleEntrega.BuscarPrimeiroRegistro();

                if (!configuracaoControleEntrega.ExibirOpcaoAjustarEntregaOnTime)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.SemConfiguracao);

                int codigoCargaEntrega = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);

                if (cargaEntrega == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                cargaEntrega.SituacaoOnTime = Request.GetEnumParam<SituacaoOnTime>("SituacaoOnTime");
                cargaEntrega.JustificativaOnTime = Request.GetStringParam("Justificativa");

                repositorioCargaEntrega.Atualizar(cargaEntrega);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork);

                return new JsonpResult(true, true, Localization.Resources.Gerais.Geral.Sucesso);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDataOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/ControleEntrega");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaEntrega_PermiteAlterarDataOcorrencias))
                    throw new ControllerException(Localization.Resources.Gerais.Geral.VoceNaoPossuiPermissaoParaExecutarEstaAcao);

                unitOfWork.Start();

                int codigoOcorrencia = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repositorioOcorrencia = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega ocorrencia = repositorioOcorrencia.BuscarPorCodigo(codigoOcorrencia, false);

                if (ocorrencia == null)
                    throw new ControllerException(Localization.Resources.Cargas.ControleEntrega.OcorrenciaNaoEncontrada);

                DateTime dataOcorrencia = Request.GetDateTimeParam("DataOcorrencia");

                if (dataOcorrencia == DateTime.MinValue)
                    throw new ControllerException(Localization.Resources.Cargas.ControleEntrega.DataInvalida);

                if (dataOcorrencia < ocorrencia.CargaEntrega.Carga.DataInicioViagem)
                    throw new ControllerException(Localization.Resources.Cargas.ControleEntrega.DataNaoPodeSerMenorQueInicioDaViagem);

                DateTime dataAntiga = ocorrencia.DataOcorrencia;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrencia, string.Format(Localization.Resources.Cargas.ControleEntrega.DataDaOcorrenciaAlteradaManualmenteDePara, dataAntiga.ToString("dd/MM/yyyy HH:mm"), dataOcorrencia.ToString("dd/MM/yyyy HH:mm")), unitOfWork);

                ocorrencia.DataOcorrencia = dataOcorrencia;

                Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repositorioConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> configuracoes = repositorioConfiguracaoOcorrenciaEntrega.BuscarPorTipoOcorrencia(ocorrencia.TipoDeOcorrencia.Codigo);

                if (configuracoes.Any(o => o.EventoColetaEntrega == EventoColetaEntrega.Confirma))
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                    string mensagemAuditoriaCargaEntrega = string.Format(Localization.Resources.Cargas.ControleEntrega.DataDeTerminoDaEntregaAlteradaDeParaDevidoAlteracaoDaOcorrencia, ocorrencia.CargaEntrega.DataFim?.ToString("dd/MM/yyyy HH:mm") ?? "", dataOcorrencia.ToString("dd/MM/yyyy HH:mm"), ocorrencia.Codigo);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrencia.CargaEntrega, mensagemAuditoriaCargaEntrega, unitOfWork);

                    ocorrencia.CargaEntrega.DataFim = dataOcorrencia;

                    repositorioCargaEntrega.Atualizar(ocorrencia.CargaEntrega);
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(ocorrencia.CargaEntrega, repositorioCargaEntrega, unitOfWork);
                    repositorioPedido.DefinirDataEntregaPorCargaEntrega(ocorrencia.CargaEntrega.Codigo, dataOcorrencia);
                }

                // Encontrar a ocorrência do pedido que é replicada e trocar também
                Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> listaCargaEntregaPedido = repCargaEntregaPedido.BuscarPorCargaEntrega(ocorrencia.CargaEntrega.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido in listaCargaEntregaPedido)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaEntregaPedido.CargaPedido.Pedido;
                    Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrencia = repPedidoOcorrenciaColetaEntrega.BuscarPorPedidoTipoOcorrenciaData(ocorrencia.TipoDeOcorrencia.Codigo, pedido.Codigo, dataAntiga);

                    if (pedidoOcorrencia != null)
                    {
                        pedidoOcorrencia.DataOcorrencia = dataOcorrencia;
                        repPedidoOcorrenciaColetaEntrega.Atualizar(pedidoOcorrencia);
                    }
                }

                repositorioOcorrencia.Atualizar(ocorrencia);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(null, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoAdicionarPedidoNaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> BaixarComprovanteColeta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigo);

                if (cargaEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Servicos.Embarcador.Carga.Impressao servicoImpressao = new Servicos.Embarcador.Carga.Impressao(unitOfWork);
                byte[] arquivo = servicoImpressao.ObterPdfComprovanteColeta(cargaEntrega);

                return Arquivo(arquivo, "application/pdf", $"Comprovante Coleta {cargaEntrega.Carga.CodigoCargaEmbarcador}_{cargaEntrega.Cliente.CPF_CNPJ}.pdf");
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VincularCanhotoEsperandoVinculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCanhotoEsperandoVinculo = Request.GetIntParam("CanhotoEsperandoVinculo");
                Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo repositorioCanhotoEsperandoVinculo = new Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo canhotoEsperandoVinculo = repositorioCanhotoEsperandoVinculo.BuscarPorCodigo(codigoCanhotoEsperandoVinculo);

                if (canhotoEsperandoVinculo == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                int codigoCanhotoVincular = Request.GetIntParam("CanhotoVincular");
                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhotoVincular = repositorioCanhoto.BuscarPorCodigo(codigoCanhotoVincular);

                if (canhotoVincular == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Servicos.Embarcador.Canhotos.CanhotoEsperandoVinculo servicoCanhotoEsperandoVinculo = new Servicos.Embarcador.Canhotos.CanhotoEsperandoVinculo(unitOfWork);

                servicoCanhotoEsperandoVinculo.VincularCanhotos(canhotoEsperandoVinculo, canhotoVincular, TipoServicoMultisoftware, this.Cliente);

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
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoVincularCanhoto);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDadosRoteirizacaoReodenarEntregas()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unidadeTrabalho);

                Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unidadeTrabalho);

                int.TryParse(Request.Params("CargaControleEntrega"), out int codigoCarga);

                List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();
                List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaEntregaReordenar> rotasEntregas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaEntregaReordenar>();

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega = repCargaEntrega.BuscarPorCarga(codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = repCarregamentoRoteirizacao.BuscarPorCarregamento(carga.Carregamento?.Codigo ?? 0);

                bool pedidosDeColeta = false;
                bool roteirizarPorLocalidade = (configuracaoEmbarcador.RoteirizarPorCidade || (carga.TipoOperacao?.RoteirizarPorLocalidade ?? false));

                pedidosDeColeta = ObterRemetenteDestinatarioCargaPedidoRoteirizacao(remetentes, destinatarios, carga, cargasEntrega, pedidosDeColeta);

                if (remetentes.Count == 0)
                {
                    Dominio.Entidades.Cliente clienteEmpresa = repCliente.BuscarPorCPFCNPJ(double.Parse(carga.Empresa.CNPJ));
                    remetentes.Add(clienteEmpresa);
                }

                foreach (Dominio.Entidades.Cliente remetente in remetentes)
                    ObterRotasInformacaoEntrega(serPessoa, rotasEntregas, remetente, true, roteirizarPorLocalidade, cargasEntrega);

                foreach (Dominio.Entidades.Cliente destinatario in destinatarios)
                    ObterRotasInformacaoEntrega(serPessoa, rotasEntregas, destinatario, false, roteirizarPorLocalidade, cargasEntrega);


                var retorno = new
                {
                    PedidosDeColeta = pedidosDeColeta,
                    rotasEntregas,
                    PontosPassagem = ObterPontosPassagem(carga.Rota),
                    DistanciaKM = carregamentoRoteirizacao != null ? carregamentoRoteirizacao.DistanciaKM : carga.Distancia,
                    TipoRota = carga.Rota?.TipoRota ?? null,
                    TipoUltimoPontoRoteirizacao = carga.TipoOperacao?.TipoUltimoPontoRoteirizacao ?? TipoUltimoPontoRoteirizacao.PontoMaisDistante,
                    TempoDeViagemEmMinutos = carregamentoRoteirizacao != null ? carregamentoRoteirizacao.TempoDeViagemEmMinutos : carga.TempoPrevistoEmHoras * 60
                };

                return new JsonpResult(retorno);
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(true, false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoBuscarOsDadosDaRoteirizacao);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarReordenacaoEntrega()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unidadeTrabalho.Start();
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unidadeTrabalho);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                int codigocarga;
                int.TryParse(Request.Params("Carga"), out codigocarga);

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaEntregaReordenar> Entregas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaEntregaReordenar>>(Request.Params("EntregasReordenadas"));

                Dominio.Entidades.Embarcador.Cargas.Carga Carga = repCarga.BuscarPorCodigo(codigocarga);

                if (Carga == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();
                foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.RotaEntregaReordenar entrega in Entregas)
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(entrega.CodigoEntrega);
                    //salvar ordem dos pedidos da entrega

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoEntrega = repCargaEntregaPedido.BuscarCargaPedidoPorCargaEntrega(cargaEntrega.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidoEntrega)
                    {
                        cargaPedido.OrdemEntrega = entrega.Ordem;

                        if (!cargaEntrega.Coleta)
                            cargaPedido.OrdemColeta = 1;

                        repCargaPedido.Atualizar(cargaPedido);
                    }

                    cargaEntrega.Ordem = entrega.Ordem;
                    repCargaEntrega.Atualizar(cargaEntrega);
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unidadeTrabalho, configControleEntrega);
                }

                Carga.OrdemRoteirizacaoDefinida = true;
                repCarga.Atualizar(Carga);

                Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(Carga, unidadeTrabalho, configuracaoEmbarcador, TipoServicoMultisoftware, false);

                Servicos.Embarcador.Monitoramento.Monitoramento.GerarMonitoramentoEIniciar(Carga, configuracaoEmbarcador, null, "Reordenação de entregas no controle de entrega", unidadeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, Carga, null, "Reordenou entregas pela gestão de entregas", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoAdicionarPedidoNaCarga);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto repositorioCargaEntregaFoto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto cargaEntregaFoto = repositorioCargaEntregaFoto.BuscarPorCodigo(codigo);

                if (cargaEntregaFoto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = cargaEntregaFoto.CargaEntrega;

                repositorioCargaEntregaFoto.Deletar(cargaEntregaFoto);
                unitOfWork.CommitChanges();

                dynamic retorno = (from o in cargaEntrega.Fotos
                                   select new
                                   {
                                       o.Codigo,
                                       DataRecebimento = o.DataEnvioImagem.ToString("dd/MM/yyyy HH:mm"),
                                       Miniatura = Base64ImagemAnexo(o, unitOfWork)
                                   }).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BaixarRecebimentoProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigo);

                if (cargaEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega(unitOfWork);
                byte[] arquivo = servicoControleEntrega.ObterPdfRecebimentoProduto(cargaEntrega, unitOfWork);

                return Arquivo(arquivo, "application/pdf", $"Recebimento Produto {cargaEntrega.Carga.CodigoCargaEmbarcador}_{cargaEntrega.Cliente.CPF_CNPJ}.pdf");
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDataAgendamentoDeTodosPedidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCargaEntrega = Request.GetIntParam("CodigoCargaEntrega");
                DateTime? dataAgendamento = Request.GetNullableDateTimeParam("DataAgendamento");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repConfiguracaoControleEntrega.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);
                cargaEntrega.Initialize();

                if (cargaEntrega == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedido = repositorioCargaEntregaPedido.BuscarPorCargaEntrega(codigoCargaEntrega);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos = cargaEntregaPedido.Select(o => o.CargaPedido.Pedido).ToList();

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in listaPedidos)
                {
                    if (cargaEntrega.Coleta)
                    {
                        pedido.DataCarregamentoPedido = dataAgendamento;
                        pedido.DataInicialColeta = dataAgendamento;
                    }
                    else
                    {
                        pedido.DataAgendamento = dataAgendamento;
                    }

                    pedido.OrigemCriacaoDataAgendamentoPedido = OrigemCriacao.Operador;

                    repositorioPedido.Atualizar(pedido, Auditado);
                }

                cargaEntrega.DataAgendamento = dataAgendamento;
                cargaEntrega.OrigemCriacaoDataAgendamentoCargaEntrega = OrigemCriacao.Operador;

                repositorioCargaEntrega.Atualizar(cargaEntrega, Auditado, null, $"Data de Agendamento alterada através do controle de Entrega pelo Operador {Usuario?.Nome ?? string.Empty}");
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork);

                if (configuracaoControleEntrega?.CalcularDataAgendamentoAutomaticamenteDataFaturamento ?? false)
                {
                    Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega servicoPrevisaoControleEntregaColeta = new Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega(unitOfWork, configuracaoEmbarcador);
                    servicoPrevisaoControleEntregaColeta.CalcularDataAgendamentoColetaAoFecharCargaAutomatico(cargaEntrega.Carga, unitOfWork);
                }

                var dyn = new
                {
                    DataAgendamentoDeEntrega = cargaEntrega.DataAgendamento.HasValue ? cargaEntrega.DataAgendamento.Value.ToDateTimeString() : string.Empty,
                    OrigemSituacao = cargaEntrega.OrigemCriacaoDataAgendamentoCargaEntrega.Value.ObterDescricao(),
                };

                unitOfWork.CommitChanges();

                return new JsonpResult(dyn, true, "Data de Agendamento de Entrega de todos os Pedidos e de Entrega alteradas com Sucesso!");
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoRemoverPedidoDeReentrega);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDataEntregaAjustada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCargaEntrega = Request.GetIntParam("CodigoCargaEntrega");
                DateTime? dataAjustada = Request.GetNullableDateTimeParam("DataPrevisaoEntregaAjustada");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);

                if (cargaEntrega == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                await unitOfWork.StartAsync();

                cargaEntrega.Initialize();
                cargaEntrega.DataPrevisaoEntregaAjustada = dataAjustada;

                await repositorioCargaEntrega.AtualizarAsync(cargaEntrega, Auditado, null, Localization.Resources.Cargas.ControleEntrega.DataPrevisaoEntregaAjustadaAlteradaAtravesDoControleDeEntrega);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork);

                var dyn = new
                {
                    DataPrevisaoEntregaAjustada = cargaEntrega.DataPrevisaoEntregaAjustada.HasValue ? cargaEntrega.DataPrevisaoEntregaAjustada.Value.ToDateTimeString() : string.Empty,
                };

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(dyn, true, Localization.Resources.Cargas.ControleEntrega.DataPrevisaoEntregaAjustadaAlteradaAtravesDoControleDeEntrega);
            }
            catch (ServicoException excecao)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> AlterarDataAgendamentoEntregaTransportador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCargaEntrega = Request.GetIntParam("CodigoCargaEntrega");
                DateTime? dataAgendamento = Request.GetNullableDateTimeParam("DataAgendamentoEntregaTransportador");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega servicoOcorrenciaEntrega = new Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);

                if (cargaEntrega == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                cargaEntrega.Initialize();

                cargaEntrega.DataAgendamentoEntregaTransportador = dataAgendamento;
                cargaEntrega.OrigemCriacaoDataAgendamentoEntregaTransportador = OrigemCriacao.Operador;

                repositorioCargaEntrega.Atualizar(cargaEntrega, Auditado, null, $"Data de Agendamento de Entrega do Transportador alterada pelo controle de Entrega pelo Operador {Usuario?.Nome ?? string.Empty}");
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork);

                servicoOcorrenciaEntrega.GerarOcorrenciaEntregaPorGatilho(cargaEntrega, TipoDataAlteracaoGatilho.DataAgendamentoEntregaTransportador, TipoServicoMultisoftware, Cliente, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, Localization.Resources.Cargas.ControleEntrega.DataAgendamentoEntregaTransportadorAlteradaComSucesso);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoRemoverPedidoDeReentrega);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterParametrosCalculo(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, int distancia, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime? dataBaseCalculo, DateTime? dataReporgramada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCalculoPrevisao repositorioCalculoPrevisao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCalculoPrevisao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCalculoPrevisao configuracaoCalculoPrevisa = repositorioCalculoPrevisao.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(cargaEntrega.Carga.Codigo);

            TimeSpan? resultado = null;
            double velocidadeKmH = configuracaoTMS.PrevisaoEntregaVelocidadeMediaCarregado > 0 ? configuracaoTMS.PrevisaoEntregaVelocidadeMediaCarregado : 50d;

            double tempoDeslocamentoEmMinutos = ((distancia > 0) && (velocidadeKmH > 0)) ? (((distancia / 1000) / velocidadeKmH) * 60).RoundUp(0) : 0d;

            TimeSpan tempodeslocamento = TimeSpan.FromMinutes(tempoDeslocamentoEmMinutos);
            int tempoIntervaloAlmoco = configuracaoCalculoPrevisa.ConsiderarJornadaMotorita ? configuracaoCalculoPrevisa.MinutosIntervalo : 0;

            if (cargaEntrega.Carga.DataInicioViagem.HasValue)
                resultado = (cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada : cargaEntrega.DataPrevista) - cargaEntrega.Carga.DataInicioViagem;
            else
            {
                switch (configuracaoTMS.DataBaseCalculoPrevisaoControleEntrega)
                {
                    case DataBaseCalculoPrevisaoControleEntrega.DataCriacaoCarga:
                        resultado = (cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada : cargaEntrega.DataPrevista) - cargaEntrega.Carga.DataCriacaoCarga;
                        break;
                    case DataBaseCalculoPrevisaoControleEntrega.DataPrevisaoTerminoCarga:
                        resultado = (cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada : cargaEntrega.DataPrevista) - cargaEntrega.Carga.DataPrevisaoTerminoCarga;
                        break;
                    case DataBaseCalculoPrevisaoControleEntrega.DataCarregamentoCarga:
                        resultado = (cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada : cargaEntrega.DataPrevista) - (cargaEntrega.Carga.DataCarregamentoCarga.HasValue ? cargaEntrega.Carga.DataCarregamentoCarga : cargaEntrega.Carga.DataInicioViagemPrevista);
                        break;
                    case DataBaseCalculoPrevisaoControleEntrega.DataInicioViagemPrevista:
                        resultado = (cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada : cargaEntrega.DataPrevista) - (cargaEntrega.Carga.DataInicioViagemPrevista.HasValue ? cargaEntrega.Carga.DataInicioViagemPrevista : cargaEntrega.Carga.DataPrevisaoTerminoCarga ?? cargaEntrega.Carga.DataCriacaoCarga);
                        break;
                    case DataBaseCalculoPrevisaoControleEntrega.DataInicioCarregamentoJanela:
                        resultado = (cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada : cargaEntrega.DataPrevista) - cargaJanelaCarregamento?.InicioCarregamento;
                        break;
                }
            }

            TimeSpan tempoBalsa = TimeSpan.FromMinutes(cargaEntrega?.TempoExtraEntrega ?? 0);

            return new
            {
                DataBaseConsiderada = $"{configuracaoTMS.DataBaseCalculoPrevisaoControleEntrega.ObterDescricao()}",
                PeriodoUtilDeDislocamento = $"{configuracaoTMS.PrevisaoEntregaTempoUtilDiarioMinutos} minutos diarios das {configuracaoTMS.PrevisaoEntregaPeriodoUtilHorarioInicial.ToString("hh\\:mm")} as {configuracaoTMS.PrevisaoEntregaPeriodoUtilHorarioFinal.ToString("hh\\:mm")}",
                VelocidadeVazio = $"{configuracaoTMS.PrevisaoEntregaVelocidadeMediaVazio} km/h ",
                VelocidadeCarregado = $"{configuracaoTMS.PrevisaoEntregaVelocidadeMediaCarregado} km/h",
                TempoColeta = $"{configuracaoTMS.TempoPadraoDeColetaParaCalcularPrevisao} minutos",
                TempoEntrega = $"{configuracaoTMS.TempoPadraoDeEntregaParaCalcularPrevisao} minutos",
                DesconsideraSabado = configuracaoCalculoPrevisa.DesconsiderarSabadosCalculoPrevisao ? "Sim" : "Não",
                DesconsideraDomingo = configuracaoCalculoPrevisa.DesconsiderarDomingosCalculoPrevisao ? "Sim" : "Não",
                DesconsideraFeriados = configuracaoCalculoPrevisa.DesconsiderarFeriadosCalculoPrevisao ? "Sim" : "Não",
                TempodePercurso = $"{tempodeslocamento.Days}D {tempodeslocamento.Hours}Hrs {tempodeslocamento.Minutes}Min",
                ConsideraJornadaMotorista = configuracaoCalculoPrevisa.ConsiderarJornadaMotorita ? "Sim" : "Não",
                TempoIntervaloAlmoco = $"{tempoIntervaloAlmoco}Min",
                DistanciaEntrega = $"{(distancia / 1000).ToString("n2")}Km",
                DistanciaAteDestino = (cargaEntrega?.DistanciaAteDestino != null) ? String.Format("{0:n1} Km", cargaEntrega.DistanciaAteDestino) : "",
                TempoParaChegaNaEntrega = $"{resultado?.Days ?? 0}D {resultado?.Hours ?? 0}Hrs {resultado?.Minutes ?? 0}Min",
                TempoConsideradoBalsa = (cargaEntrega?.TempoExtraEntrega ?? 0) > 0 ? $"{tempoBalsa.Days}D {tempoBalsa.Hours}Hrs {tempoBalsa.Minutes}Min" : ""
            };
        }

        private dynamic ObterPontosPassagem(Dominio.Entidades.RotaFrete rota)
        {
            List<dynamic> listaPontos = new List<dynamic>();

            if (rota?.PontoPassagemPreDefinido == null)
                return listaPontos;

            foreach (Dominio.Entidades.Embarcador.Logistica.PontoPassagemPreDefinido ponto in rota.PontoPassagemPreDefinido)
            {
                listaPontos.Add(new
                {
                    Descricao = ponto.ObterDescricao(),
                    Latitude = ponto.ObterLatitude(),
                    Longitude = ponto.ObterLongitude(),
                    TempoEstimadoPermanencia = ponto.TempoEstimadoPermanenciaFormatado,
                });
            }

            return listaPontos;
        }

        private void ObterRotasInformacaoEntrega(Servicos.WebService.Pessoas.Pessoa serPessoa, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaEntregaReordenar> rotasEntrega, Dominio.Entidades.Cliente pessoa, bool coleta, bool roteirizarPorLocalidade, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> ListaCargaEntregas)
        {
            string latitude = pessoa.Latitude;
            string longitude = pessoa.Longitude;

            if (roteirizarPorLocalidade)
            {
                latitude = pessoa.Localidade.Latitude?.ToString().Replace(",", ".");
                longitude = pessoa.Localidade.Longitude?.ToString().Replace(",", ".");
            }

            //Destinatários...
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregaOrdenadas = ListaCargaEntregas.OrderBy(o => o.Ordem).ToList();
            cargaEntregaOrdenadas = ListaCargaEntregas.Where(o => o.Cliente.CPF_CNPJ == pessoa.CPF_CNPJ).ToList();

            //verificar se as entregas nao tem outros enderecos para o mesmo cliente
            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregaOrdenadas)
            {
                if (cargaEntrega.ClienteOutroEndereco != null)
                {
                    latitude = cargaEntrega.ClienteOutroEndereco.Latitude;
                    longitude = cargaEntrega.ClienteOutroEndereco.Longitude;
                }

                if (!string.IsNullOrWhiteSpace(latitude) && !string.IsNullOrWhiteSpace(longitude))
                {
                    Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoaObj = serPessoa.ConverterObjetoPessoa(pessoa);

                    if (pessoa.Tipo == "E")
                        pessoaObj.CPFCNPJ = pessoa.CPF_CNPJ.ToString();

                    pessoaObj.Codigo = pessoa.CPF_CNPJ.ToString() + "_" + latitude;

                    if (cargaEntrega.ClienteOutroEndereco != null)
                    {
                        pessoaObj.Endereco.Cidade.Descricao = cargaEntrega.ClienteOutroEndereco.Localidade?.Descricao;
                        pessoaObj.Endereco.Cidade.SiglaUF = cargaEntrega.ClienteOutroEndereco.Localidade?.Estado?.Sigla;
                        pessoaObj.Endereco.Logradouro = cargaEntrega.ClienteOutroEndereco.Endereco;
                        pessoaObj.Endereco.Numero = cargaEntrega.ClienteOutroEndereco.Numero;
                        pessoaObj.Endereco.Bairro = cargaEntrega.ClienteOutroEndereco.Bairro;
                    }

                    if (!rotasEntrega.Exists(x => x.pessoa.Codigo == pessoaObj.Codigo))
                    {

                        Dominio.ObjetosDeValor.Embarcador.Logistica.RotaEntregaReordenar rotaInformacao = new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaEntregaReordenar
                        {
                            pessoa = pessoaObj,

                            coordenadas = new Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas
                            {
                                tipoLocalizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.ponto,
                                latitude = latitude,
                                longitude = longitude,
                                RestricoesEntregas = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.RestricaoEntrega>(),
                            },
                            coleta = coleta,
                            Finalizada = !SituacaoEntregaHelper.ObterSituacaoEntregaEmAberto(cargaEntrega.Situacao),
                            CodigoEntrega = cargaEntrega.Codigo
                        };

                        List<Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega> restricaoEntrega = pessoa.ClienteDescargas?.FirstOrDefault()?.RestricoesDescarga.ToList();
                        if (restricaoEntrega != null && restricaoEntrega.Count > 0)
                        {
                            rotaInformacao.coordenadas.RestricoesEntregas = (from restricao in restricaoEntrega
                                                                             select new Dominio.ObjetosDeValor.Embarcador.Pessoas.RestricaoEntrega()
                                                                             {
                                                                                 Codigo = restricao.Codigo,
                                                                                 Descricao = restricao.Descricao,
                                                                                 Observacao = restricao.Observacao,
                                                                                 PrimeiraEntrega = restricao.PrimeiraEntrega,
                                                                                 CorVisualizacao = restricao.CorVisualizacao,
                                                                             }).ToList();
                        }
                        rotaInformacao.pessoa.CodigoIntegracao = pessoa.CodigoIntegracao ?? string.Empty;

                        rotasEntrega.Add(rotaInformacao);
                    }
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.RotaEntregaReordenar rotaInformacao = new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaEntregaReordenar
                    {
                        pessoa = serPessoa.ConverterObjetoPessoa(pessoa),
                        coordenadas = new Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas
                        {
                            tipoLocalizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.naoEncontrado
                        },
                        Finalizada = !SituacaoEntregaHelper.ObterSituacaoEntregaEmAberto(cargaEntrega.Situacao),
                        CodigoEntrega = cargaEntrega.Codigo
                    };
                    rotaInformacao.pessoa.CodigoIntegracao = pessoa.CodigoIntegracao ?? string.Empty;
                    rotasEntrega.Add(rotaInformacao);
                }
            }
        }

        private bool ObterRemetenteDestinatarioCargaPedidoRoteirizacao(List<Dominio.Entidades.Cliente> remetentes, List<Dominio.Entidades.Cliente> destinatarios, Dominio.Entidades.Embarcador.Cargas.Carga Carga, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> CargaEntrega, bool pedidosDeColeta)
        {
            List<Dominio.Entidades.Cliente> remetentesOrdenados = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> destinatariosOrdenados = new List<Dominio.Entidades.Cliente>();

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregaOrdenada = CargaEntrega.OrderBy(o => o.Ordem).ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregaOrdenada)
            {
                if (cargaEntrega.ColetaEquipamento || cargaEntrega.Coleta)
                {
                    if (cargaEntrega.ColetaEquipamento)
                        pedidosDeColeta = true;

                    remetentesOrdenados.Add(cargaEntrega.Cliente);
                }
                else
                    destinatariosOrdenados.Add(cargaEntrega.Cliente);
            }

            remetentes.Clear();
            destinatarios.Clear();

            remetentes.AddRange(remetentesOrdenados.Distinct().ToList());
            destinatarios.AddRange(destinatariosOrdenados.Distinct().ToList());

            return pedidosDeColeta;
        }

        private string ObterDatasJanelaDaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            string retorno = "";

            if (cargaEntrega.InicioJanela.HasValue && cargaEntrega.FimJanela.HasValue)
            {
                if (cargaEntrega.InicioJanela.Value.Date == cargaEntrega.FimJanela.Value.Date)
                    retorno = cargaEntrega.InicioJanela.Value.Date.ToString("dd/MM/yyyy") + " (" + cargaEntrega.InicioJanela.Value.ToString("HH:mm") + " até " + cargaEntrega.FimJanela.Value.ToString("HH:mm") + ")";
                else
                    retorno = cargaEntrega.InicioJanela.Value.ToString("dd/MM/yyyy HH:mm ") + " até " + cargaEntrega.FimJanela.Value.ToString("dd/MM/yyyy HH:mm");
            }

            return retorno;
        }

        private dynamic ObterDadosDevolucao(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            return new
            {
                ExibirDadosDevolucao = cargaEntrega.Situacao == SituacaoEntrega.AgAtendimento || cargaEntrega.Situacao == SituacaoEntrega.Rejeitado || (cargaEntrega.Situacao == SituacaoEntrega.Entregue && cargaEntrega.DevolucaoParcial),
                MotivoRejeicao = cargaEntrega.MotivoRejeicao?.Codigo ?? 0,
                cargaEntrega.PermitirEntregarMaisTarde,
                TipoDevolucao = cargaEntrega.TipoDevolucao
            };
        }

        private string ObterDadosEtapaStage(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Stage repStage = new Repositorio.Embarcador.Pedidos.Stage(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoStage repPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> stagesPedido = repPedidoStage.BuscarPorListaCodigoPedidos(cargaEntregaPedidos.Select(x => x.CargaPedido.Pedido.Codigo).ToList());
            Dominio.Entidades.Embarcador.Pedidos.PedidoStage pedidostagePreliminar = stagesPedido?.Where(x => x.Stage.TipoPercurso == Vazio.PercursoDireto).FirstOrDefault() ?? null;

            if (stagesPedido.Count() > 1 && stagesPedido.Any(x => x.Stage.TipoPercurso == Vazio.PercursoPreliminar))
                pedidostagePreliminar = stagesPedido.Where(x => x.Stage.TipoPercurso == Vazio.PercursoPreliminar).FirstOrDefault() ?? null;

            return pedidostagePreliminar?.Stage?.NumeroStage ?? "";
        }

        private dynamic ObterGerenteArea(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (pedido?.FuncionarioGerente != null)
            {
                return new
                {
                    Nome = pedido?.FuncionarioGerente?.Nome,
                    Email = pedido?.FuncionarioGerente?.Email,
                    Telefone = pedido?.FuncionarioGerente?.Telefone,
                };
            }

            if (pedido?.FuncionarioSupervisor != null)
            {
                return new
                {
                    Nome = pedido?.FuncionarioSupervisor?.Gerente?.Nome,
                    Email = pedido?.FuncionarioSupervisor?.Gerente?.Email,
                    Telefone = pedido?.FuncionarioSupervisor?.Gerente?.Telefone,
                };
            }

            return new { Nome = string.Empty, Email = string.Empty, Telefone = string.Empty };
        }

        private dynamic ObterGerenteRegional(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (pedido?.FuncionarioGerente != null)
            {
                return new
                {
                    Nome = pedido?.FuncionarioGerente?.Gerente?.Nome,
                    Email = pedido?.FuncionarioGerente?.Gerente?.Email,
                    Telefone = pedido?.FuncionarioGerente?.Gerente?.Telefone,
                };
            }

            if (pedido?.FuncionarioSupervisor != null)
            {
                return new
                {
                    Nome = pedido?.FuncionarioSupervisor?.Gerente?.Gerente?.Nome,
                    Email = pedido?.FuncionarioSupervisor?.Gerente?.Gerente?.Email,
                    Telefone = pedido?.FuncionarioSupervisor?.Gerente?.Gerente?.Telefone,
                };
            }

            return new { Nome = string.Empty, Email = string.Empty, Telefone = string.Empty };
        }

        private dynamic ObterGerenteNacional(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {

            if (pedido?.FuncionarioGerente != null)
            {
                return new
                {
                    Nome = pedido?.FuncionarioGerente?.Gerente?.Gerente?.Nome,
                    Email = pedido?.FuncionarioGerente?.Gerente?.Gerente?.Email,
                    Telefone = pedido?.FuncionarioGerente?.Gerente?.Gerente?.Telefone,
                };
            }

            if (pedido?.FuncionarioSupervisor != null)
            {
                return new
                {
                    Nome = pedido?.FuncionarioSupervisor?.Gerente?.Gerente?.Gerente?.Nome,
                    Email = pedido?.FuncionarioSupervisor?.Gerente?.Gerente?.Gerente?.Email,
                    Telefone = pedido?.FuncionarioSupervisor?.Gerente?.Gerente?.Gerente?.Telefone,
                };
            }

            return new { Nome = string.Empty, Email = string.Empty, Telefone = string.Empty };
        }

        private bool ValidaPermissaoTransportador(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            return cargaEntrega?.Carga?.TipoOperacao?.PermitirTransportadorConfirmarRejeitarEntrega ?? false;
        }

        private string Base64ImagemAssinatura(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel assinaturaEntregaColeta, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "CargaColetaEntrega", "Assinatura" });
            return Base64Imagem(caminho, assinaturaEntregaColeta.NomeArquivo, assinaturaEntregaColeta.GuidArquivo);
        }

        private string Base64ImagemNotaFiscal(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal fotoNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "CargaColetaEntrega", "NotasFiscais" });
            return Base64Imagem(caminho, fotoNotaFiscal.NomeArquivo, fotoNotaFiscal.GuidArquivo);
        }

        private string Base64ImagemAnexo(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto foto, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" });
            return Base64Imagem(caminho, foto.NomeArquivo, foto.GuidArquivo);
        }

        private string Base64Imagem(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto foto, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" });
            string extensao = Path.GetExtension(foto.NomeArquivo);
            string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, foto.GuidArquivo + "-miniatura" + extensao);

            if (Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
            {
                byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                return base64ImageRepresentation;
            }
            else
            {
                return "";
            }
        }

        private string Base64Imagem(string caminho, string nomeArquivo, string guidArquivo)
        {
            string extensao = Path.GetExtension(nomeArquivo);
            string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + "-miniatura" + extensao);

            if (Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
            {
                byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                return base64ImageRepresentation;
            }
            else
            {
                return "";
            }
        }

        private string RetornarNumerosCTes(List<int> codigosPedidoXMLNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            if (codigosPedidoXMLNotaFiscal == null || codigosPedidoXMLNotaFiscal.Count == 0)
                return "";

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            List<int> numerosCTes = new List<int>();

            if (codigosPedidoXMLNotaFiscal == null || codigosPedidoXMLNotaFiscal.Count == 0)
                return "";

            if (codigosPedidoXMLNotaFiscal.Count < 2000)
                numerosCTes = repCargaPedidoXMLNotaFiscalCTe.NumerosCTesPorNotasFiscais(codigosPedidoXMLNotaFiscal);
            else
            {
                try
                {
                    decimal decimalBlocos = Math.Ceiling(((decimal)codigosPedidoXMLNotaFiscal.Count) / 1000);
                    int blocos = (int)Math.Truncate(decimalBlocos);

                    for (int i = 0; i < blocos; i++)
                    {
                        Servicos.Log.TratarErro($"Controle Entrega - blocos {codigosPedidoXMLNotaFiscal.Count} indice {i}");
                        numerosCTes.AddRange(repCargaPedidoXMLNotaFiscalCTe.NumerosCTesPorNotasFiscais((codigosPedidoXMLNotaFiscal.Skip(i * 1000).Take(1000).ToList())));
                    }

                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                }
            }

            if (numerosCTes != null && numerosCTes.Count > 0)
                return string.Join(", ", numerosCTes);
            else
                return "";
        }

        private List<dynamic> ObterListaProduto(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> listaCargaPedidoProdutoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaEntregaProdutos.Count == 0)
                return new List<dynamic>();

            Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade repositorioCargaPedidoProdutoDivisaoCapacidade = new Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade repositorioModeloVeicularCargaDivisaoCapacidade = new Repositorio.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> listaDivisaoCapacidadeEntrega = repositorioCargaPedidoProdutoDivisaoCapacidade.BuscarPorCarga(cargaEntrega.Carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade> listaDivisaoCapacidade = repositorioModeloVeicularCargaDivisaoCapacidade.BuscarPorModeloVeicularCarga(cargaEntrega.Carga.ModeloVeicularCarga?.Codigo ?? 0) ?? new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade>();

            List<dynamic> listaProdutos = new List<dynamic>();

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido in cargaEntregaPedidos)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> listaCargaPedidoProduto = (from obj in listaCargaPedidoProdutoCarga where obj.CargaPedido.Codigo == cargaEntregaPedido.CargaPedido.Codigo select obj).ToList();

                if (!listaCargaPedidoProduto.Any(o => o.Produto.PossuiIntegracaoColetaMobile))
                    continue;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in listaCargaPedidoProduto)
                {
                    List<dynamic> listaDivisaoCapacidadeProduto = new List<dynamic>();

                    foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade divisaoCapacidade in listaDivisaoCapacidade)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade divisaoCapacidadeProduto = (
                            from o in listaDivisaoCapacidadeEntrega
                            where o.CargaPedidoProduto.Codigo == cargaPedidoProduto.Codigo && o.ModeloVeicularCargaDivisaoCapacidade.Codigo == divisaoCapacidade.Codigo
                            select o
                        ).FirstOrDefault();

                        listaDivisaoCapacidadeProduto.Add(new
                        {
                            divisaoCapacidade.Codigo,
                            Descricao = $"{divisaoCapacidade.Descricao} ({Localization.Resources.Cargas.ControleEntrega.Capacidade} {divisaoCapacidade.Quantidade.ToString("n2")} {divisaoCapacidade.UnidadeMedida.Sigla})",
                            Quantidade = divisaoCapacidadeProduto?.Quantidade > 0 ? divisaoCapacidadeProduto.Quantidade.ToString("n2") : "",
                            QuantidadePlanejada = divisaoCapacidadeProduto?.QuantidadePlanejada > 0 ? divisaoCapacidadeProduto.QuantidadePlanejada.ToString("n2") : "",
                            Piso = divisaoCapacidade.Piso,
                            Coluna = divisaoCapacidade.Coluna
                        });
                    }

                    listaProdutos.Add(new
                    {
                        Produto = new
                        {
                            cargaPedidoProduto.Codigo,
                            CodigoProduto = cargaPedidoProduto.Produto.Codigo,
                            ProdutoDescricao = cargaPedidoProduto.Produto.Descricao,
                            NumeroPedido = cargaPedidoProduto.CargaPedido.Pedido.NumeroPedidoEmbarcador,
                            Quantidade = cargaPedidoProduto.Quantidade.ToString("n2"),
                            QuantidadePlanejada = cargaPedidoProduto.QuantidadePlanejada.ToString("n2"),
                            Temperatura = cargaPedidoProduto.Temperatura.ToString("n2"),
                            cargaPedidoProduto.Produto.ObrigatorioInformarTemperatura,
                            JustificativaTemperatura = cargaPedidoProduto.JustificativaTemperatura?.Descricao ?? "",
                            ImunoPlanejado = cargaPedidoProduto.ImunoPlanejado,
                            ImunoRealizado = cargaPedidoProduto.ImunoRealizado,

                            // Aves
                            QuantidadeCaixa = cargaPedidoProduto.QuantidadeCaixa,
                            QuantidadePorCaixaRealizada = cargaPedidoProduto.QuantidadePorCaixaRealizada,
                            QuantidadeCaixasVazias = cargaPedidoProduto.QuantidadeCaixasVazias,
                            QuantidadeCaixasVaziasRealizada = cargaPedidoProduto.QuantidadeCaixasVaziasRealizada,
                        },
                        DivisoesCapacidade = listaDivisaoCapacidadeProduto
                    });
                }
            }

            return listaProdutos;
        }

        private dynamic ObterCheckListCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, TipoCheckList tipoCheckList, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta repCargaEntregaCheckListPergunta = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> perguntas = repCargaEntregaCheckListPergunta.BuscarPerguntasOrdenadasPorCargaEntrega(cargaEntrega.Codigo, tipoCheckList);

            if (perguntas.Count() == 0)
                return null;

            Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaCheckList servicoCheckList = new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaCheckList(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckList> checkList = servicoCheckList.ObterObjetoMobileCheckList(perguntas);

            return new
            {
                Codigo = perguntas.FirstOrDefault()?.CargaEntregaCheckList?.Codigo ?? 0,
                Perguntas = checkList
            };
        }

        private dynamic ObterTodosChamadosCargaEntrega(List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamadosCargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = new Dominio.Entidades.Embarcador.Chamados.MotivoChamado();
            dynamic retorno = null;
            foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamadosCargaEntrega)
            {
                Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
                motivoChamado = repMotivoChamado.BuscarPorChamado(chamado.Codigo);
            }

            retorno = new
            {
                Chamados = from obj in chamadosCargaEntrega
                           select new
                           {
                               obj.Codigo,
                               obj.Numero,
                               MotivoChamado = obj.MotivoChamado.Descricao,
                               DataCriacao = obj.DataCriacao.ToString("dd/MM/yyyy HH:mm") ?? "",
                               Situacao = obj.Situacao.ObterDescricao()
                           }
            };

            return retorno;
        }

        private void SetarCanhotosComoPendente(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            List<Canhoto> canhotos = servicoControleEntrega.ObterCanhotosDaCargaEntrega(cargaEntrega);

            foreach (Canhoto canhoto in canhotos)
            {
                if (canhoto.SituacaoCanhoto == SituacaoCanhoto.Cancelado)
                {
                    canhoto.SituacaoCanhoto = SituacaoCanhoto.Pendente;
                    canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao;
                    repCanhoto.Atualizar(canhoto);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, string.Format(Localization.Resources.Cargas.ControleEntrega.CanhotoEstavaNaSituacaoCanceladoVoltouAutomaticamenteParaSituacaoPendentePorqueEntregaColetaDaCargaFoiConfirmadaManualmente, cargaEntrega.Ordem, cargaEntrega.Carga.CodigoCargaEmbarcador), unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntrega, string.Format(Localization.Resources.Cargas.ControleEntrega.CanhotoEstavaNaSituacaoCanceladoVoltouAutomaticamenteParaSituacaoPendentePorqueEntregaColetaFoiConfirmadaManualmente, canhoto.Numero), unitOfWork);
                }
            }

        }

        private List<Dominio.Entidades.Embarcador.Ocorrencias.GestaoOcorrenciaSobras> ObterSobrasDaCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Ocorrencias.GestaoOcorrenciaSobras> sobrasRetorno = new List<Dominio.Entidades.Embarcador.Ocorrencias.GestaoOcorrenciaSobras>();

            Repositorio.Embarcador.Ocorrencias.GestaoOcorrenciaSobras repSobras = new Repositorio.Embarcador.Ocorrencias.GestaoOcorrenciaSobras(unitOfWork);

            // Pega todas as sobras da cargaEntrega
            sobrasRetorno = repSobras.BuscarPorCargaEntrega(cargaEntrega.Codigo);

            return sobrasRetorno;
        }

        private List<CanhotoEsperandoVinculo> ObterCanhotosEsperandoVinculoDaCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo repCanhotoEsperandoVinculo = new Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo(unitOfWork);

            return repCanhotoEsperandoVinculo.BuscarAguardandoVinculoPorCargaEntrega(cargaEntrega.Codigo);
        }

        private dynamic ObterAreasRedex(List<ClienteAreaRedex> listaAreaRedex, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {

            var areas = (from area in listaAreaRedex
                         select new
                         {
                             text = area.AreaRedex.NomeCNPJ,
                             value = area.AreaRedex.Codigo
                         }).ToList();

            if (areas != null && cargaEntrega.Cliente != null)
                areas.Insert(0, new { text = cargaEntrega.Cliente.NomeCNPJ, value = cargaEntrega.Cliente.Codigo });

            return areas;
        }

        private string Base64ImagemFotoGTA(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoGTA fotoNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" });
            return Base64Imagem(caminho, fotoNotaFiscal.NomeArquivo, fotoNotaFiscal.GuidArquivo);
        }

        private bool ExigirConferenciaProdutos(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            if (!(cargaEntrega.Carga?.TipoOperacao?.ConfiguracaoControleEntrega?.ExigirConferenciaProdutosAoConfirmarEntrega ?? false))
                return false;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioControleEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioControleEntrega.BuscarPedidosPorCargaEntrega(cargaEntrega.Codigo);

            if ((cargaPedidos.Count == 0) || (cargaPedidos.Count > 0 && !cargaPedidos.Any(p => p.Pedido.PedidoDeDevolucao && p.Pedido.PedidoOrigemDevolucao != null)))
                return false;

            return true;
        }

        private string retornarCaminhoCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork)
        {
            return Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, unitOfWork);
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RenderizarImagemEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string caminhoArquivo = string.Empty;

                if (codigo == 0)
                    return new JsonpResult(false, "Necessário selecionar um registro!");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto repositorioCargaEntregaFoto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto cargaEntregaFoto = repositorioCargaEntregaFoto.BuscarPorCodigo(codigo);

                string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" });
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, cargaEntregaFoto.GuidArquivo + ".pdf");

                byte[] pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (pdf == null)
                    return new JsonpResult(true, false, "Não foi possível gerar o arquivo de entrega!");

                MemoryStream stream = new MemoryStream();
                stream.Write(pdf, 0, pdf.Length);
                stream.Position = 0;

                return File(stream, "application/pdf", $"Arquivo entrega N° {cargaEntregaFoto.Codigo}");
            }
            catch (ControllerException ex)
            {
                return new JsonpResult(false, true, ex.Message);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar o arquivo de entrega");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion
    }
}
