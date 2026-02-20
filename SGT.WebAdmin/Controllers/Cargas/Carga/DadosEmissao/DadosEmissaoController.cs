using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosEmissao
{
    [CustomAuthorize(new string[] { "ObterInformacoesGeraisCarga" }, "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class DadosEmissaoController : BaseController
    {
        #region Construtores

        public DadosEmissaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> ObterInformacoesGeraisCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");

                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete repCargaPedidoRotaFrete = new Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao repCargaPedidoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaIsca repositorioCargaIsca = new Repositorio.Embarcador.Cargas.CargaIsca(unitOfWork);
                Repositorio.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga repApoliceSeguraAutorizacaoCarga = new Repositorio.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.CTe.ObservacaoContribuinte repositorioObservacaoContribuinte = new Repositorio.Embarcador.CTe.ObservacaoContribuinte(unitOfWork);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaFronteira repositorioCargaFronteira = new Repositorio.Embarcador.Cargas.CargaFronteira(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigoFetch(codigoCarga);
                Dominio.Entidades.Embarcador.Cargas.Carga cargaTrocaNota = repCarga.BuscarCargaPorCargaTrocaNota(carga.Codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo);

                if (cargaPedido == null)
                    return new JsonpResult(true, false, "A carga não possui nenhum pedido.");

                Servicos.Embarcador.Seguro.Seguro serSeguro = new Servicos.Embarcador.Seguro.Seguro(unitOfWork);

                bool precisaAutorizacao = false;
                string mensagemAutorizarSeguro = serSeguro.VeririficarSeEnecessarioAutorizacaoApolice(carga, unitOfWork, out precisaAutorizacao);
                bool necessarioAutorizacaoPeso = Servicos.Embarcador.Veiculo.Veiculo.VerificarSeEnecessarioAutorizacaoPeso(carga, unitOfWork, out string mensagemAutorizarPeso);
                bool necessarioAutorizacaoManutencaoVeiculo = Servicos.Embarcador.Frota.OrdemServicoManutencao.VerificarSeEnecessarioAutorizacaoServicoVeiculo(carga, unitOfWork, out string mensagemAutorizarManutencaoVeiculo);
                bool necessarioAutorizacaoValorMaximoPendentePagamento = Servicos.Embarcador.Financeiro.Pagamento.VerificarSeEnecessarioAutorizacaoValorMaximoPendentePagamento(carga, unitOfWork, out string mensagemAutorizarValorMaximoPendentePagamento);

                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apoliceSeguroAverbacoes = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedido.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete> rotasFrete = repCargaPedidoRotaFrete.BuscarPorCargaPedido(cargaPedido.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);
                List<Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte> observacoesFiscoContribuinte = repositorioObservacaoContribuinte.BuscarPorCarga(codigoCarga);
                List<Dominio.Entidades.Embarcador.Cargas.CargaIsca> iscas = repositorioCargaIsca.BuscarPorCarga(codigoCarga);
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao contaContabil = repCargaPedidoContaContabilContabilizacao.BuscarFirstOrDefaultPorCargaPedido(cargaPedido.Codigo);
                List<Dominio.Entidades.Embarcador.Filiais.Filial> filiaisCargasAgrupadas = repCarga.BuscarFiliaisCargasOriginais(codigoCarga);

                bool possuiFronteira = false;
                if (cargaPedido.Origem?.Estado.Sigla == "EX" || cargaPedido.Destino?.Estado.Sigla == "EX")
                    possuiFronteira = true;

                Dominio.Entidades.Cliente expedidor = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? cargaPedido.Pedido.Remetente : null;
                if (cargaPedido.Expedidor != null)
                    expedidor = cargaPedido.Expedidor;

                Dominio.Entidades.Cliente recebedor = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? cargaPedido.Pedido.Destinatario : null;
                if (cargaPedido.Recebedor != null)
                    recebedor = cargaPedido.Recebedor;

                List<Dominio.Entidades.Cliente> fronteiras = repositorioCargaFronteira.BuscarFronteirasPorCarga(codigoCarga);

                bool exibirOpcaoNaoComprarValePedagio = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador &&
                   (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermitirNaoComprarValePedagio) || Usuario.UsuarioAdministrador);

                var retorno = new
                {
                    Carga = carga.Codigo,
                    carga.NaoGerarMDFe,
                    carga.NaoComprarValePedagio,
                    ExibirOpcaoNaoComprarValePedagio = exibirOpcaoNaoComprarValePedagio,
                    ApolicesAgurandoAutorizacao = precisaAutorizacao,
                    MensagemAutorizarSeguro = mensagemAutorizarSeguro,
                    InserirDadosContabeisXCampoXTextCTe = (carga.TipoOperacao?.InserirDadosContabeisXCampoXTextCTe ?? false),
                    NecessarioAutorizacaoPeso = necessarioAutorizacaoPeso,
                    MensagemAutorizarPeso = mensagemAutorizarPeso,
                    NecessarioAutorizacaoManutencao = necessarioAutorizacaoManutencaoVeiculo,
                    MensagemAutorizarManutencao = mensagemAutorizarManutencaoVeiculo,
                    NecessarioAutorizacaoValorMaximoPendentePagamento = necessarioAutorizacaoValorMaximoPendentePagamento,
                    MensagemAutorizacaoValorMaximoPendentePagamento = mensagemAutorizarValorMaximoPendentePagamento,
                    TipoOperacaoPermiteTrocaNota = carga.TipoOperacao?.PermitirTrocaNota ?? false,
                    TipoOperacaoUtilizaCentroDeCustoOuPEP = carga.TipoOperacao?.TipoOperacaoUtilizaCentroDeCustoPEP ?? false,
                    TipoOperacaoUtilizaContaRazao = carga.TipoOperacao?.TipoOperacaoUtilizaContaRazao ?? false,
                    CargaTrocaNota = new
                    {
                        Codigo = cargaTrocaNota?.Codigo ?? 0,
                        Descricao = cargaTrocaNota?.Descricao ?? ""
                    },
                    ApolicesSeguro = (from obj in apoliceSeguroAverbacoes
                                      select new
                                      {
                                          obj.ApoliceSeguro.Codigo,
                                          Seguradora = obj.ApoliceSeguro.Seguradora.Nome,
                                          Responsavel = obj.ApoliceSeguro.DescricaoResponsavel,
                                          obj.ApoliceSeguro.NumeroApolice,
                                          obj.ApoliceSeguro.NumeroAverbacao,
                                          InicioVigencia = obj.ApoliceSeguro.InicioVigencia.ToString("dd/MM/yyyy"),
                                          FimVigencia = obj.ApoliceSeguro.FimVigencia.ToString("dd/MM/yyyy"),
                                          Vigencia = "até " + obj.ApoliceSeguro.FimVigencia.ToString("dd/MM/yyyy")
                                      }).ToList(),
                    RotasFrete = (from obj in rotasFrete
                                  select new
                                  {
                                      Codigo = obj.RotaFrete.Codigo,
                                      Descricao = obj.RotaFrete.Descricao
                                  }).ToList(),
                    FormulaRateio = new
                    {
                        Codigo = cargaPedido.FormulaRateio?.Codigo ?? 0,
                        Descricao = cargaPedido.FormulaRateio?.Descricao ?? string.Empty
                    },
                    TipoRateio = cargaPedido.TipoRateio,
                    TipoEmissaoCTeParticipantes = cargaPedido.TipoEmissaoCTeParticipantes,
                    IncluirICMSBC = cargaPedido.IncluirICMSBaseCalculo,
                    ViagemJaOcorreu = cargaPedido.Pedido.SituacaoPedido == SituacaoPedido.Finalizado ? true : false,
                    Expedidor = new
                    {
                        Codigo = expedidor?.CPF_CNPJ_SemFormato ?? "0",
                        Descricao = expedidor?.Descricao ?? string.Empty
                    },
                    Recebedor = new
                    {
                        Codigo = recebedor?.CPF_CNPJ_SemFormato ?? "0",
                        Descricao = recebedor?.Descricao ?? string.Empty
                    },
                    Destinatario = cargaPedido.Pedido.Destinatario != null ? (cargaPedido.Pedido.Destinatario.Descricao + " - " + cargaPedido.Pedido.Destinatario.Localidade.DescricaoCidadeEstado + "") : "",
                    NotasFiscais = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ? string.Join(", ", (from obj in pedidoXMLNotaFiscals select obj.XMLNotaFiscal.Numero)) : "",
                    Observacao = cargaPedido.Pedido.ObservacaoCTe,
                    ObservacaoTerceiro = cargaPedido.Pedido.ObservacaoCTeTerceiro,
                    ModeloDocumentoFiscal = (cargaPedido.ModeloDocumentoFiscal != null && cargaPedido.ModeloDocumentoFiscal.Numero != "57") ? new { cargaPedido.ModeloDocumentoFiscal.Codigo, cargaPedido.ModeloDocumentoFiscal.Descricao } : new { Codigo = 0, Descricao = "" },
                    ModeloDocumentoFiscalEmissaoMunicipal = new
                    {
                        Descricao = cargaPedido.ModeloDocumentoFiscalIntramunicipal?.Descricao ?? string.Empty,
                        Codigo = cargaPedido.ModeloDocumentoFiscalIntramunicipal?.Codigo ?? 0
                    },
                    UtilizarOutroModeloDocumentoEmissaoMunicipal = (cargaPedido.ModeloDocumentoFiscalIntramunicipal != null && cargaPedido.PossuiNFSManual),
                    CobrarOutroDocumento = (cargaPedido.ModeloDocumentoFiscal != null && cargaPedido.ModeloDocumentoFiscal.Numero != "57") ? true : false,
                    cargaPedido.Pedido.UsarTipoPagamentoNF,
                    cargaPedido.Pedido.TipoPagamento,
                    Tomador = new
                    {
                        Codigo = cargaPedido.Tomador?.CPF_CNPJ_SemFormato,
                        Descricao = cargaPedido.Tomador?.Descricao
                    },
                    Fronteiras = (from o in fronteiras
                                  select new
                                  {
                                      o.Codigo,
                                      o.Nome,
                                      o.Descricao,
                                      o.Latitude,
                                      o.Longitude,
                                  }).ToList(),
                    cargaPedido.TipoCobrancaMultimodal,
                    cargaPedido.ModalPropostaMultimodal,
                    cargaPedido.TipoServicoMultimodal,
                    cargaPedido.TipoPropostaMultimodal,
                    FilialCargaAgrupada = carga.FilialCargaAgrupadaValePedagio?.Codigo ?? 0,
                    FiliaisCargasAgrupadas = (from o in filiaisCargasAgrupadas
                                              select new
                                              {
                                                  FilialCodigo = o.Codigo,
                                                  Filial = o.Descricao
                                              }
                    ),
                    cargaPedido.BloquearEmissaoDosDestinatario,
                    cargaPedido.BloquearEmissaoDeEntidadeSemCadastro,
                    cargaPedido.Carga.CargaDePreCarga,
                    cargaPedido.Carga.NumeroCargaVincularPreCarga,
                    cargaPedido.Carga.NumeroOrdem,
                    cargaPedido.TipoTomador,
                    ElementoPEP = cargaPedido.ElementoPEP ?? "",
                    CentroResultado = new
                    {
                        Codigo = cargaPedido.CentroResultado?.Codigo ?? 0,
                        Descricao = cargaPedido.CentroResultado?.Descricao ?? ""
                    },
                    ContaContabil = new
                    {
                        Codigo = contaContabil?.PlanoConta?.Codigo ?? 0,
                        Descricao = contaContabil?.PlanoConta?.BuscarDescricao ?? ""
                    },
                    ObservacoesFiscoContribuinte = observacoesFiscoContribuinte.Select(o => new
                    {
                        o.Codigo,
                        CodigoCarga = o.Carga.Codigo,
                        o.Identificador,
                        TipoCodigo = o.Tipo,
                        Descricao = o.Texto,
                        Tipo = o.Tipo.ObterDescricao()
                    }).ToList(),
                    Iscas = iscas.Select(o => new
                    {
                        o.Codigo,
                        CodigoIsca = o.Isca.Codigo,
                        Isca = o.Isca.Descricao,
                        CodigoIntegracao = o.Isca.CodigoIntegracao
                    }).ToList(),
                    PossuiFronteira = possuiFronteira,
                    QuantidadePaletes = repCargaPedido.BuscarNumeroPaletesPorCarga(carga.Codigo)
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados de emissão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarDadosEmissao(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                bool permiteAlterarInclusaoICMS = false;

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AlterarDadosPedido) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                        return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                    if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermiteAlterarInclusaoICMS))
                        permiteAlterarInclusaoICMS = true;
                }

                await unitOfWork.StartAsync();

                int codigoCarga = 0, codigoPedido = 0, codigoModeloDocumento = 0;
                bool aplicarGeralEmTodosPedidos = false, usarTipoPagamentoNF = false, incluirICMSBC = false, viagemJaOcorreu = false;
                double tomador = 0;

                int.TryParse(Request.Params("Carga"), out codigoCarga);
                int.TryParse(Request.Params("Pedido"), out codigoPedido);
                int.TryParse(Request.Params("ModeloDocumentoFiscal"), out codigoModeloDocumento);
                int.TryParse(Request.Params("Fronteira"), out int codigoFronteira);
                double.TryParse(Request.Params("Tomador"), out tomador);
                bool.TryParse(Request.Params("AplicarGeralEmTodosPedidos"), out aplicarGeralEmTodosPedidos);
                bool.TryParse(Request.Params("UsarTipoPagamentoNF"), out usarTipoPagamentoNF);
                bool.TryParse(Request.Params("IncluirICMSBC"), out incluirICMSBC);
                bool.TryParse(Request.Params("ViagemJaOcorreu"), out viagemJaOcorreu);
                string numeroCargaVincularPreCarga = Request.GetStringParam("NumeroCargaVincularPreCarga");
                string numeroOrdem = Request.GetStringParam("NumeroOrdem");
                string elementoPEP = Request.GetStringParam("ElementoPEP");
                int codigoCentroResultado = Request.GetIntParam("CentroResultado");
                int codigotContaContabil = Request.GetIntParam("ContaContabil");
                Dominio.Enumeradores.TipoPagamento tipoPagamento = (Dominio.Enumeradores.TipoPagamento)int.Parse(Request.Params("TipoPagamento"));
                string numeroPedido = Request.Params("NumeroPedido");
                int codigoFilialCargaAgrupada = Request.GetIntParam("FilialCargaAgrupada");

                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = codigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(codigoCentroResultado) : null;
                Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoConta = codigotContaContabil > 0 ? repPlanoConta.BuscarPorCodigo(codigotContaContabil) : null;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal tipoPropostaMultimodal = Request.GetEnumParam("TipoPropostaMultimodal", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.Nenhum);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal tipoServicoMultimodal = Request.GetEnumParam("TipoServicoMultimodal", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.Nenhum);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal modalPropostaMultimodal = Request.GetEnumParam("ModalPropostaMultimodal", Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.Nenhum);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal tipoCobrancaMultimodal = Request.GetEnumParam("TipoCobrancaMultimodal", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal.Nenhum);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga, true);

                if ((carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete && carga.ExigeNotaFiscalParaCalcularFrete) || (!carga.ExigeNotaFiscalParaCalcularFrete && carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador && carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete))
                    throw new ControllerException("A atual situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite que esta ação seja executada.");

                if (carga.CalculandoFrete && !carga.PendenteGerarCargaDistribuidor && carga.SituacaoRoteirizacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Erro)
                    throw new ControllerException("Não é possível alterar os dados da emissão enquanto a carga estiver em processo de cálculo de valores do frete.");

                if (!serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                    throw new ControllerException("Não é possível alterar os dados da emissão na atual situação da carga (" + carga.DescricaoSituacaoCarga + ")");

                serCarga.ValidarPermissaoAlterarDadosEtapaFrete(carga, unitOfWork);

                if (carga.TipoOperacao?.ConfiguracaoCalculoFrete?.BloquearAjusteConfiguracoesFreteCarga ?? false)
                    throw new ControllerException("A operação não permite que estas informações sejam alteradas manualmente.");

                bool outRecalcular = false;
                bool recalcularFrete = false;

                dynamic dynCargaPedido = null;

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.Entidades.Auditoria.HistoricoPropriedade>();

                Dominio.Entidades.Cliente clienteTomador = tomador > 0d ? repCliente.BuscarPorCPFCNPJ(tomador) : null;
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = null;

                if (aplicarGeralEmTodosPedidos)
                {
                    cargaPedidos = repCargaPedido.BuscarPorCarga(codigoCarga);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        if (!cargaPedido.IsInitialized())
                            cargaPedido.Initialize();

                        AtualizarPEPCentroCusto(cargaPedido, centroResultado, elementoPEP, planoConta, unitOfWork);

                        alteracoes.AddRange(cargaPedido.GetChanges());
                    }
                }

                if (carga.CargaAgrupada)
                {
                    carga.AgIntegracaoAgrupamentoCarga = true;

                    Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork, cancellationToken);
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = codigoFilialCargaAgrupada > 0 ? repositorioFilial.BuscarPorCodigo(codigoFilialCargaAgrupada) : null;
                    if (codigoFilialCargaAgrupada > 0 && filial == null)
                        throw new ControllerException("Filial selecionada para ser a filial da carga agrupada não foi encontrada.");

                    carga.FilialCargaAgrupadaValePedagio = filial;
                }

                bool atualizarResumoCarga = false;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    carga.DadosPagamentoInformadosManualmente = true;

                    if (aplicarGeralEmTodosPedidos)
                    {
                        cargaPedidos = repCargaPedido.BuscarPorCarga(codigoCarga);

                        if (clienteTomador == null)
                            clienteTomador = cargaPedidos.FirstOrDefault()?.ObterTomador();

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                        {
                            cargaPedido.Initialize();
                            cargaPedido.Pedido.Initialize();

                            string mensagemRetorno = AtualizarDadosGeralEmissaoPedido(numeroPedido, tipoPropostaMultimodal, tipoServicoMultimodal, modalPropostaMultimodal, tipoCobrancaMultimodal, cargaPedido, codigoModeloDocumento, usarTipoPagamentoNF, incluirICMSBC, tomador, tipoPagamento, viagemJaOcorreu, unitOfWork, out outRecalcular, permiteAlterarInclusaoICMS);

                            alteracoes.AddRange(cargaPedido.GetChanges());
                            alteracoes.AddRange(cargaPedido.Pedido.GetChanges());

                            if (string.IsNullOrWhiteSpace(mensagemRetorno))
                            {
                                if (!recalcularFrete)
                                    recalcularFrete = outRecalcular;
                            }
                            else
                            {
                                throw new ControllerException(mensagemRetorno);
                            }
                        }

                        serNFe.VerificarSeNecessariaAutorizacaoModalidadePagamento(carga, cargaPedidos, unitOfWork);
                        dynCargaPedido = serCarga.ObterDadosCargaPedido(cargaPedidos.FirstOrDefault(), TipoServicoMultisoftware, unitOfWork);
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(codigoCarga, codigoPedido);

                        cargaPedido.Initialize();
                        cargaPedido.Pedido.Initialize();

                        if (clienteTomador == null)
                            clienteTomador = cargaPedido.ObterTomador();

                        string mensagemRetorno = AtualizarDadosGeralEmissaoPedido(numeroPedido, tipoPropostaMultimodal, tipoServicoMultimodal, modalPropostaMultimodal, tipoCobrancaMultimodal, cargaPedido, codigoModeloDocumento, usarTipoPagamentoNF, incluirICMSBC, tomador, tipoPagamento, viagemJaOcorreu, unitOfWork, out outRecalcular, permiteAlterarInclusaoICMS);

                        if (string.IsNullOrWhiteSpace(mensagemRetorno))
                        {
                            if (!recalcularFrete)
                                recalcularFrete = outRecalcular;

                            alteracoes.AddRange(cargaPedido.GetChanges());
                            alteracoes.AddRange(cargaPedido.Pedido.GetChanges());
                        }
                        else
                            throw new ControllerException(mensagemRetorno);

                        if (cargaPedidos == null)
                            cargaPedidos = repCargaPedido.BuscarPorCarga(codigoCarga);

                        serNFe.VerificarSeNecessariaAutorizacaoModalidadePagamento(carga, cargaPedidos, unitOfWork);
                        dynCargaPedido = serCarga.ObterDadosCargaPedido(cargaPedido, TipoServicoMultisoftware, unitOfWork);
                    }

                    if (!Servicos.Embarcador.Carga.CargaPedido.ValidarNumeroPedidoEmbarcador(out string erro, numeroPedido, clienteTomador, carga.TipoOperacao, ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal))
                        throw new ControllerException(erro);

                    carga.NaoGerarMDFe = Request.GetBoolParam("GerarMDFeManualmente");
                    if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermitirNaoComprarValePedagio))
                        carga.NaoComprarValePedagio = Request.GetBoolParam("NaoComprarValePedagio");

                    repCarga.Atualizar(carga);
                }
                else
                {
                    if (clienteTomador != null) AtualizarTomadorClientePedido(aplicarGeralEmTodosPedidos, codigoCarga, clienteTomador, codigoPedido);

                    cargaPedidos = repCargaPedido.BuscarPorCarga(codigoCarga);

                    atualizarResumoCarga = SalvarFronteiras(carga, cargaPedidos, unitOfWork, configuracaoPedido);

                    carga.NaoGerarMDFe = Request.GetBoolParam("GerarMDFeManualmente");
                    if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermitirNaoComprarValePedagio))
                        carga.NaoComprarValePedagio = Request.GetBoolParam("NaoComprarValePedagio");

                    if (numeroCargaVincularPreCarga != carga.NumeroCargaVincularPreCarga && carga.CargaDePreCarga)
                        carga.NumeroCargaVincularPreCarga = numeroCargaVincularPreCarga;

                    carga.NumeroOrdem = numeroOrdem;

                    repCarga.Atualizar(carga);

                    dynCargaPedido = serCarga.ObterDadosCargaPedido(cargaPedidos.FirstOrDefault(), TipoServicoMultisoftware, unitOfWork);
                }

                alteracoes.AddRange(carga.GetChanges());

                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoFrete = null;

                if (recalcularFrete && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador || (carga.TipoFreteEscolhido == TipoFreteEscolhido.Operador && carga.ValorFreteOperador <= 0m))
                    {
                        carga.TipoFreteEscolhido = TipoFreteEscolhido.Tabela;
                        carga.DataInicioCalculoFrete = DateTime.Now;
                        carga.CalculandoFrete = true;

                        repCarga.Atualizar(carga);

                        retornoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.CalculandoFrete };

                        string retornoMontagem = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork).CalcularFreteTodoCarregamento(carga);

                        if (!string.IsNullOrWhiteSpace(retornoMontagem))
                            throw new ControllerException(retornoMontagem);
                    }
                    else
                    {
                        if (cargaPedidos == null)
                            cargaPedidos = repCargaPedido.BuscarPorCarga(codigoCarga);

                        Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);
                        Servicos.Embarcador.Carga.Frete serFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, TipoServicoMultisoftware);

                        carga.ValorFrete = carga.ValorFreteOperador;

                        serRateioFrete.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, ConfiguracaoEmbarcador, false, unitOfWork, TipoServicoMultisoftware);
                        serFrete.ProcessarRegraInclusaoICMSComponenteFrete(carga, cargaPedidos, ConfiguracaoEmbarcador, unitOfWork);
                        serFrete.CalcularValorFreteComICMSIncluso(carga, unitOfWork);
                    }
                }



                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, alteracoes, "Atualizou os dados de emissão da carga.", unitOfWork);

                var retorno = new
                {
                    CargaPedido = dynCargaPedido,
                    RetornoFrete = retornoFrete
                };

                unitOfWork.CommitChanges();

                if (atualizarResumoCarga)
                    servicoHubCarga.InformarCargaAtualizada(carga.Codigo, TipoAcaoCarga.Alterada, _conexao.StringConexao);

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
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os dados para emissão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AutorizarModalidadePagamentoNota()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AutorizarModalidadePagamentoNota))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                int codigoCarga = 0, codigoPedido = 0;
                bool aplicarGeralEmTodosPedidos = false;
                bool.TryParse(Request.Params("AplicarGeralEmTodosPedidos"), out aplicarGeralEmTodosPedidos);

                int.TryParse(Request.Params("Carga"), out codigoCarga);
                int.TryParse(Request.Params("Pedido"), out codigoPedido);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    return new JsonpResult(false, true, "A atual situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite que esta ação seja executada.");

                unitOfWork.Start();

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = null;

                if (aplicarGeralEmTodosPedidos)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(codigoCarga);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        if (alteracoes == null)
                            cargaPedido.Initialize();

                        LiberarModalidadePagamento(cargaPedido, unitOfWork);

                        if (alteracoes == null)
                            alteracoes = cargaPedido.GetChanges();
                    }
                    carga = cargaPedidos.FirstOrDefault().Carga;
                }
                else
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(codigoCarga, codigoPedido);
                    cargaPedido.Initialize();
                    LiberarModalidadePagamento(cargaPedido, unitOfWork);
                    carga = cargaPedido.Carga;

                    alteracoes = cargaPedido.GetChanges();
                }

                if (carga != null)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, alteracoes, "Autorizou Modalidade de Pagamento Nota", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar as informações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AutorizarPesoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AutorizarPesoCarga))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga repVeiculoToleranciaPesoAutorizacaoCarga = new Repositorio.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga(unitOfWork);

                int codigoCarga = Request.GetIntParam("Carga");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    return new JsonpResult(false, true, "A atual situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite que esta ação seja executada.");

                List<Dominio.Entidades.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga> autorizacoes = repVeiculoToleranciaPesoAutorizacaoCarga.BuscarNaoExtornadasPorCarga(codigoCarga);

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga autorizacao in autorizacoes)
                {
                    if (autorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPesoCarga.AgLiberacao)
                    {
                        autorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPesoCarga.Liberada;
                        autorizacao.Usuario = Usuario;
                        autorizacao.Data = DateTime.Now;
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Autorizou as quantidades da carga.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar as informações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AutorizarManutencaoPendenteVeiculoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AutorizarManutencaoPendenteVeiculo))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Frota.VeiculoServicoAutorizacaoCarga repVeiculoServicoAutorizacaoCarga = new Repositorio.Embarcador.Frota.VeiculoServicoAutorizacaoCarga(unitOfWork);

                int codigoCarga = Request.GetIntParam("Carga");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    return new JsonpResult(false, true, "A atual situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite que esta ação seja executada.");

                List<Dominio.Entidades.Embarcador.Frota.VeiculoServicoAutorizacaoCarga> autorizacoes = repVeiculoServicoAutorizacaoCarga.BuscarNaoExtornadasPorCarga(codigoCarga);

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Frota.VeiculoServicoAutorizacaoCarga autorizacao in autorizacoes)
                {
                    if (autorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoServicoVeiculoCarga.AgLiberacao)
                    {
                        autorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoServicoVeiculoCarga.Liberada;
                        autorizacao.Usuario = Usuario;
                        autorizacao.Data = DateTime.Now;
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Autorizou as manutenções pendentes dos veículos da carga.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar as informações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AutorizarValorMaximoPendentePagamentoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AutorizarValorMaximoPendentePagamento))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga repValorMaximoPendenteAutorizacaoCarga = new Repositorio.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga(unitOfWork);

                int codigoCarga = Request.GetIntParam("Carga");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    return new JsonpResult(false, true, "A atual situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite que esta ação seja executada.");

                List<Dominio.Entidades.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga> autorizacoes = repValorMaximoPendenteAutorizacaoCarga.BuscarNaoExtornadasPorCarga(codigoCarga);

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga autorizacao in autorizacoes)
                {
                    if (autorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValorMaximoPendenteAutorizacaoCarga.AgLiberacao)
                    {
                        autorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValorMaximoPendenteAutorizacaoCarga.Liberada;
                        autorizacao.Usuario = Usuario;
                        autorizacao.Data = DateTime.Now;

                        repValorMaximoPendenteAutorizacaoCarga.Atualizar(autorizacao);
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Autorizou o valor máximo pendente de pagamento para o tomador a carga.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar as informações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void LiberarModalidadePagamento(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoModalidadePagamentoNFAprovacao repCargaPedidoModalidadePagamentoNFAprovacao = new Repositorio.Embarcador.Cargas.CargaPedidoModalidadePagamentoNFAprovacao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedidoModalidadePagamentoNFAprovacao cargaPedidoModalidade = repCargaPedidoModalidadePagamentoNFAprovacao.BuscarPorCargaPedido(cargaPedido.Codigo);
            if (cargaPedidoModalidade != null && cargaPedidoModalidade.SituacaoAutorizacaoModalidadePagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoModalidadePagamento.AgLiberacao)
            {
                cargaPedidoModalidade.SituacaoAutorizacaoModalidadePagamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoModalidadePagamento.Liberada;
                cargaPedidoModalidade.MotivoExtornoAutorizacao = "";
                cargaPedidoModalidade.DataHora = DateTime.Now;
                cargaPedidoModalidade.Usuario = this.Usuario;
                repCargaPedidoModalidadePagamentoNFAprovacao.Atualizar(cargaPedidoModalidade);
            }
            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido, null, "Autorizou Modalidade de Pagamento Nota", unitOfWork);
        }

        private void AtualizarPEPCentroCusto(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado, string elementoPEP, Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoConta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao repCargaPedidoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao contaContabil = repCargaPedidoContaContabilContabilizacao.BuscarFirstOrDefaultPorCargaPedido(cargaPedido.Codigo);


            if (cargaPedido.Carga.TipoOperacao?.TipoOperacaoUtilizaContaRazao ?? false)
            {
                if (planoConta != null)
                {
                    if (contaContabil == null)
                        contaContabil = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao();

                    contaContabil.CargaPedido = cargaPedido;
                    contaContabil.PlanoConta = planoConta;
                    contaContabil.TipoContabilizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Debito;
                    contaContabil.TipoContaContabil = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.TotalReceber;

                    if (contaContabil.Codigo > 0)
                        repCargaPedidoContaContabilContabilizacao.Atualizar(contaContabil);
                    else
                        repCargaPedidoContaContabilContabilizacao.Inserir(contaContabil);
                }
            }

            if (cargaPedido.Carga.TipoOperacao?.TipoOperacaoUtilizaCentroDeCustoPEP ?? false)
            {
                cargaPedido.ElementoPEP = elementoPEP;
                cargaPedido.CentroResultado = centroResultado;

                cargaPedido.Pedido.ElementoPEP = elementoPEP;
                cargaPedido.Pedido.CentroResultado = centroResultado;
                cargaPedido.Pedido.ContaContabil = planoConta;
            }
            repCargaPedido.Atualizar(cargaPedido);
            repPedido.Atualizar(cargaPedido.Pedido);
        }

        private string AtualizarDadosGeralEmissaoPedido(string numeroPedidoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal tipoPropostaMultimodal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal tipoServicoMultimodal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal modalPropostaMultimodal,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal tipoCobrancaMultimodal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, int codigoModeloDocumento, bool usarTipoPagamentoNF, bool incluirICMSBC, double tomador, Dominio.Enumeradores.TipoPagamento tipoPagamento, bool viagemJaOcorreu, Repositorio.UnitOfWork unitOfWork, out bool recalcular, bool permiteAlterarInclusaoICMS)
        {
            string retorno = "";
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
            Dominio.Enumeradores.TipoPagamento tipoPagamentoOrigem = cargaPedido.Pedido.TipoPagamento;
            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoOrigem = cargaPedido.ModeloDocumentoFiscal;
            bool incluirOrigem = cargaPedido.IncluirICMSBaseCalculo;

            if ((integracaoIntercab?.BuscarTipoServicoModeloDocumentoVinculadoCarga ?? false))
            {
                if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.Normal)
                    cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.Normal;
                else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.NormalESubContratada)
                    cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.Normal;
                else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.Redespacho)
                    cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.Redespacho;
                else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.RedespachoIntermediario)
                    cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.RedespachoIntermediario;
                else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.SubContratada)
                    cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.Subcontratacao;
                else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.SVMProprio)
                    cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.VinculadoMultimodalProprio;
                else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.SVMTerceiro)
                    cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.VinculadoMultimodalTerceiro;
                else
                    cargaPedido.TipoServicoMultimodal = tipoServicoMultimodal;
            }
            else
                cargaPedido.TipoServicoMultimodal = tipoServicoMultimodal;

            cargaPedido.TipoPropostaMultimodal = tipoPropostaMultimodal;
            cargaPedido.ModalPropostaMultimodal = modalPropostaMultimodal;
            cargaPedido.TipoCobrancaMultimodal = tipoCobrancaMultimodal;

            cargaPedido.Pedido.UsarTipoPagamentoNF = usarTipoPagamentoNF;
            cargaPedido.Pedido.NumeroPedidoEmbarcador = numeroPedidoEmbarcador;

            if (permiteAlterarInclusaoICMS)
                cargaPedido.IncluirICMSBaseCalculo = incluirICMSBC;

            if (cargaPedido.Pedido.UsarTipoPagamentoNF)
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete? modalidadePagamentoFrete = repPedidoXMLNotaFiscal.BuscarModalidadeDeFretePadraoPorCargaPedido(cargaPedido.Codigo);
                if (modalidadePagamentoFrete.HasValue && modalidadePagamentoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido)
                {
                    retorno = "As notas fiscais não contém a informação do tipo do pagamento, por isso não é possível utilizar essa opção.";
                }
                else
                    cargaPedido.Pedido.TipoPagamento = modalidadePagamentoFrete.HasValue ? (Dominio.Enumeradores.TipoPagamento)modalidadePagamentoFrete : tipoPagamento;

            }
            else
                cargaPedido.Pedido.TipoPagamento = tipoPagamento;

            if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Outros)
            {
                if (tomador > 0)
                {
                    cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                    cargaPedido.Tomador = repCliente.BuscarPorCPFCNPJ(tomador);
                }
                else
                    cargaPedido.Tomador = null;
            }
            else
            {
                cargaPedido.Tomador = null;
                if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
                    cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                else
                    cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
            {
                if (tomador > 0)
                {
                    Dominio.Entidades.Cliente clienteTomador = repCliente.BuscarPorCPFCNPJ(tomador);

                    cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                    cargaPedido.Tomador = clienteTomador;
                }
                else
                    cargaPedido.Tomador = null;
            }

            bool modouModelo = false;
            if (codigoModeloDocumento > 0)
            {
                cargaPedido.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId(codigoModeloDocumento);
                if (modeloDocumentoOrigem == null || modeloDocumentoOrigem.Codigo != codigoModeloDocumento)
                    modouModelo = true;
            }
            else
            {
                cargaPedido.ModeloDocumentoFiscal = null;
                if (modeloDocumentoOrigem != null)
                    modouModelo = true;
            }


            if (viagemJaOcorreu)
                cargaPedido.Pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado;
            else
                cargaPedido.Pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;

            repCargaPedido.Atualizar(cargaPedido, Auditado);
            repPedido.Atualizar(cargaPedido.Pedido, Auditado);

            recalcular = false;

            if (tipoPagamentoOrigem != cargaPedido.Pedido.TipoPagamento || cargaPedido.IncluirICMSBaseCalculo != incluirOrigem || modouModelo)
            {
                recalcular = true;
            }

            return retorno;
        }

        private bool SalvarFronteiras(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaFronteira repositorioCargaFronteira = new Repositorio.Embarcador.Cargas.CargaFronteira(unitOfWork);

            List<double> codigos = new List<double>();
            dynamic listaCodigosFronteiras = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Fronteiras"));
            foreach (dynamic codigo in listaCodigosFronteiras)
                codigos.Add((double)codigo);

            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> fronteiras = repositorioCargaFronteira.BuscarPorCarga(carga.Codigo);
            if (fronteiras.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> fronteirasDeletar = (from obj in fronteiras where !codigos.Contains(obj.Fronteira.CPF_CNPJ) select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaFronteira fronteiraDeletar in fronteirasDeletar)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Fronteiras",
                        De = fronteiraDeletar.Fronteira.Nome,
                        Para = ""
                    });

                    repositorioCargaFronteira.Deletar(fronteiraDeletar);
                }
            }

            if (codigos.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> listaCargaFronteira = new List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira>();
                List<Dominio.Entidades.Cliente> clientesFronteira = repositorioCliente.BuscarPorVariosCPFCNPJ(codigos);

                foreach (Dominio.Entidades.Cliente clienteFronteira in clientesFronteira)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaFronteira cargaFronteira = fronteiras.Where(o => o.Fronteira.CPF_CNPJ == clienteFronteira.CPF_CNPJ).FirstOrDefault();

                    if (cargaFronteira == null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaFronteira fronteira = new Dominio.Entidades.Embarcador.Cargas.CargaFronteira
                        {
                            Carga = carga,
                            Fronteira = clienteFronteira
                        };

                        repositorioCargaFronteira.Inserir(fronteira);

                        listaCargaFronteira.Add(fronteira);

                        alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                        {
                            Propriedade = "Fronteiras",
                            De = "",
                            Para = clienteFronteira.Nome
                        });
                    }
                    else
                    {
                        listaCargaFronteira.Add(cargaFronteira);
                    }
                }

                if (clientesFronteira != null && clientesFronteira.Count > 0)
                    AlterarPassagensDaCargaSeNaoTiverOuForFronteira(carga, cargaPedidos, listaCargaFronteira, unitOfWork, configuracaoPedido);
            }

            carga.SetExternalChanges(alteracoes);


            if (alteracoes.Count > 0)
            {
                Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);
            }

            return alteracoes.Count > 0;
        }

        private void AtualizarTomadorClientePedido(bool aplicarGeralEmTodosPedidos, int codigoCarga, Dominio.Entidades.Cliente clienteTomador, int codigoPedido)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos;
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            if (aplicarGeralEmTodosPedidos)
            {
                cargaPedidos = repCargaPedido.BuscarPorCarga(codigoCarga);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    cargaPedido.Initialize();
                    cargaPedido.Tomador = clienteTomador;
                    cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;

                    repCargaPedido.Atualizar(cargaPedido, Auditado);

                }
            }
            else
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(codigoCarga, codigoPedido);
                if (codigoPedido != 0)
                {
                    cargaPedido.Initialize();
                    cargaPedido.Tomador = clienteTomador;
                    cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                    repCargaPedido.Atualizar(cargaPedido, Auditado);
                }
            }
        }

        private void AlterarPassagensDaCargaSeNaoTiverOuForFronteira(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> listaCargaFronteira, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido)
        {
            if ((carga == null) || ((cargaPedidos?.Count() ?? 0) == 0) || TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            Repositorio.PercursoEstado repositorioPercursoEstado = new Repositorio.PercursoEstado(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaFronteira repositorioCargaFronteira = new Repositorio.Embarcador.Cargas.CargaFronteira(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repositorioCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens repositorioCargaLocaisPrestacaoPassagens = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens(unitOfWork);

            Servicos.Embarcador.Carga.CargaLocaisPrestacao servicoCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalPrestacao = repositorioCargaLocaisPrestacao.BuscarPorCarga(carga.Codigo).FirstOrDefault();

            if (cargaLocalPrestacao == null)
                return;

            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens> listaCargaLocaisPrestacaoPassagens = repositorioCargaLocaisPrestacaoPassagens.BuscarPorLocalPrestacao(cargaLocalPrestacao.Codigo);

            if ((listaCargaLocaisPrestacaoPassagens?.Count() ?? 0) > 0)
                return;

            if (string.IsNullOrWhiteSpace(cargaLocalPrestacao.LocalidadeInicioPrestacao?.Estado?.Sigla ?? string.Empty) && string.IsNullOrWhiteSpace(cargaLocalPrestacao.LocalidadeFronteira?.Estado?.Sigla ?? string.Empty))
                return;

            servicoCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacao(carga, cargaPedidos, unitOfWork, TipoServicoMultisoftware, configuracaoPedido);

            cargaLocalPrestacao = repositorioCargaLocaisPrestacao.BuscarPorCarga(carga.Codigo).Count > 0 ? repositorioCargaLocaisPrestacao.BuscarPorCarga(carga.Codigo).FirstOrDefault() : cargaLocalPrestacao;

            string siglaTerminoPrestacao = string.IsNullOrWhiteSpace(cargaLocalPrestacao.LocalidadeFronteira?.Estado?.Sigla ?? string.Empty) ? listaCargaFronteira.FirstOrDefault().Fronteira.Localidade.Estado.Sigla : cargaLocalPrestacao.LocalidadeFronteira.Estado.Sigla;

            Dominio.Entidades.PercursoEstado percursoEstado = repositorioPercursoEstado.BuscarPorOrigemEDestino(0, cargaLocalPrestacao.LocalidadeInicioPrestacao.Estado.Sigla, siglaTerminoPrestacao);

            Dominio.Entidades.Embarcador.Cargas.CargaFronteira fronteira = repositorioCargaFronteira.BuscarPorCarga(carga.Codigo).FirstOrDefault();

            servicoCargaLocaisPrestacao.AdicaoPercursoCargaSemPassagem(cargaLocalPrestacao, percursoEstado, fronteira.Fronteira, unitOfWork);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Sistema Atualizou as Passagens da Carga pois não haviam Passagens informadas.", unitOfWork);
        }

        #endregion
    }
}
