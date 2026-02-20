/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

// #region Objetos Globais do Arquivo

var _configuracaoGestaoPatio;
var $boxesConfiguracaoGestaoPatio = $("#knockoutConfiguracaoGestaoPatio .etapas-container");
var $templateAlturaBoxConfiguracaoGestaoPatio = $("#style-altura-template-box-configuracao-gestao-patio");
var $styleAlturaBoxConfiguracaoGestaoPatio = $("#style-altura-box-configuracao-gestao-patio");

// #endregion Objetos Globais do Arquivo

// #region Classes

var ConfiguracaoGestaoPatio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.InformarDocaCarregamentoDescricao = PropertyEntity({ text: "Descrição:" });
    this.InformarDocaCarregamentoPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.InformarDocaCarregamentoPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.InformarDocaCarregamentoUtilizarLocalCarregamento = PropertyEntity({ text: "Utilizar local de carregamento", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.NaoPermitirInformarMaisDeUmVeiculoPorVezNaDoca = PropertyEntity({ text: "Não permitir informar mais que um veículo na mesma doca por vez (somente após emissão dos documentos a doca será liberada)", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.InformarDocaCarregamentoNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.GerarOcorrenciaPedidoEtapaDocaCarregamento = PropertyEntity({ text: "Gerar uma ocorrência para os pedidos ao confirmar essa etapa", getType: typesKnockout.bool, val: ko.observable(false) });
    this.InformarDocaPermiteAntecipar = PropertyEntity({ text: "Permite antecipar etapa no fluxo de pátio", getType: typesKnockout.bool, val: ko.observable(false) });
    this.InformarDocaCarregamentoTipoIntegracao = PropertyEntity({ text: "Integração", val: ko.observable(0), visible: ko.observable(true), options: ko.observable(new Array()), def: 0 });

    this.MontagemCargaDescricao = PropertyEntity({ text: "Descrição:" });
    this.MontagemCargaCodigoControle = PropertyEntity({ text: "Código de Controle:" });
    this.MontagemCargaPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.MontagemCargaPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.MontagemCargaNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.MontagemCargaPermiteGerarAtendimento = PropertyEntity({ text: "Permite gerar atendimento", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.MontagemCargaPermiteAntecipar = PropertyEntity({ text: "Permite antecipar etapa no fluxo de pátio", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.ChegadaVeiculoDescricao = PropertyEntity({ text: "Descrição:" });
    this.ChegadaVeiculoPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ChegadaVeiculoPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ChegadaVeiculoPermiteAvancarAutomaticamenteAposInformarDadosTransporteCarga = PropertyEntity({ text: "Permite avançar automaticamente após informar os dados de transporte da carga", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ChegadaVeiculoNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ChegadaVeiculoPermiteInformarComEtapaBloqueada = PropertyEntity({ text: "Permite informar a chegada com a etapa bloqueada", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ChegadaVeiculoTipoIntegracao = PropertyEntity({ text: "Integração", val: ko.observable(0), visible: ko.observable(true), options: ko.observable(new Array()), def: 0 });
    this.PermiteGerarAtendimento = PropertyEntity({ text: "Permite gerar atendimentos", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ChegadaVeiculoPermiteAntecipar = PropertyEntity({ text: "Permite antecipar etapa no fluxo de pátio", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.ChegadaVeiculoTipoIntegracao.val.subscribe(function (valor) {
        if (valor == EnumTipoIntegracao.Deca) {
            _configuracaoGestaoPatio.ChegadaVeiculoAction.visible(true);
            _configuracaoGestaoPatio.ChegadaVeiculoAction.required(true);
            _configuracaoGestaoPatio.ChegadaVeiculoAction.text("*Action:");
        } else {
            _configuracaoGestaoPatio.ChegadaVeiculoAction.visible(false);
        }
    });

    this.ChegadaVeiculoAction = PropertyEntity({ text: ko.observable("Action:"), required: ko.observable(false), visible: ko.observable(false), });

    this.GuaritaEntradaDescricao = PropertyEntity({ text: "Descrição:" });
    this.GuaritaEntradaPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.GuaritaEntradaPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.GuaritaEntradaNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.GuaritaEntradaTipoIntegracao = PropertyEntity({ text: "Integração", val: ko.observable(0), visible: ko.observable(true), options: ko.observable(new Array()), def: 0 });
    this.GuaritaEntradaPermiteAntecipar = PropertyEntity({ text: "Permite antecipar etapa no fluxo de pátio", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.GuaritaEntradaTipoIntegracao.val.subscribe(function (valor) {
        if (valor == EnumTipoIntegracao.Deca) {
            _configuracaoGestaoPatio.GuaritaEntradaAction.visible(true);
            _configuracaoGestaoPatio.GuaritaEntradaAction.required(true);
            _configuracaoGestaoPatio.GuaritaEntradaAction.text("*Action:");
        } else {
            _configuracaoGestaoPatio.GuaritaEntradaAction.visible(false);
        }
    });

    this.GuaritaEntradaAction = PropertyEntity({ text: ko.observable("Action:"), required: ko.observable(false), visible: ko.observable(false), });

    this.CheckListDescricao = PropertyEntity({ text: "Descrição:" });
    this.CheckListPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.CheckListPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.UtilizarCategoriaDeReboqueConformeModeloVeicularCarga = PropertyEntity({ text: "Utilizar categoria de Reboque conforme Modelo Veicular da carga", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.CheckListPermiteSalvarSemPreencher = PropertyEntity({ text: "Permite salvar sem preencher", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.CheckListNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ChecklistTipoIntegracao = PropertyEntity({ text: "Integração", val: ko.observable(0), visible: ko.observable(true), options: ko.observable(new Array()), def: 0 });
    this.ChecklistPermiteAntecipar = PropertyEntity({ text: "Permite antecipar etapa no fluxo de pátio", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.TravaChaveDescricao = PropertyEntity({ text: "Descrição:" });
    this.TravaChavePermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.TravaChavePermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.TravaChaveNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.TravaChavePermiteGerarAtendimento = PropertyEntity({ text: "Permite gerar atendimento", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.ExpedicaoDescricao = PropertyEntity({ text: "Descrição:" });
    this.ExpedicaoPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ExpedicaoPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ExpedicaoNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.LiberaChaveDescricao = PropertyEntity({ text: "Descrição:" });
    this.LiberaChavePermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.LiberaChavePermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.LiberaChaveNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.FaturamentoDescricao = PropertyEntity({ text: "Descrição:" });
    this.FaturamentoPermiteAvancarAutomaticamenteAposGerarDocumentos = PropertyEntity({ text: "Permite avançar automaticamente após gerar os documentos", getType: typesKnockout.bool, val: ko.observable(true), visible: ko.observable(true) });
    this.FaturamentoPermiteSolicitarNotasFiscaisEtapaBloqueada = PropertyEntity({ text: "Permite solicitar NF-e com a etapa bloqueada", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.FaturamentoPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.FaturamentoPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.FaturamentoNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.FaturamentoPermiteImprimirCapaViagem = PropertyEntity({ text: "Permite imprimir capa viagem", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.FaturamentoMensagemCapaViagem = PropertyEntity({ text: "Mensagem capa viagem: ", maxlength: 300, visible: this.FaturamentoPermiteImprimirCapaViagem.val });

    this.GuaritaSaidaDescricao = PropertyEntity({ text: "Descrição:" });
    this.GuaritaSaidaPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.GuaritaSaidaPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.GuaritaSaidaNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.GuaritaSaidaIniciarViagemControleEntregaAoFinalizarEtapa = PropertyEntity({ text: "Iniciar viagem no controle de entrega ao finalizar a etapa", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.GuaritaSaidaTipoIntegracao = PropertyEntity({ text: "Integração", val: ko.observable(0), visible: ko.observable(true), options: ko.observable(new Array()), def: 0 });

    this.PosicaoDescricao = PropertyEntity({ text: "Descrição:" });
    this.PosicaoPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.PosicaoPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false) });
    this.PosicaoNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.ChegadaLojaDescricao = PropertyEntity({ text: "Descrição:" });
    this.ChegadaLojaPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ChegadaLojaPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ChegadaLojaNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.DeslocamentoPatioDescricao = PropertyEntity({ text: "Descrição:" });
    this.DeslocamentoPatioPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.DeslocamentoPatioPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.DeslocamentoPatioNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.SaidaLojaDescricao = PropertyEntity({ text: "Descrição:" });
    this.SaidaLojaPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.SaidaLojaPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.SaidaLojaNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.FimViagemDescricao = PropertyEntity({ text: "Descrição:" });
    this.FimViagemPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.FimViagemPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.FimViagemNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.InicioHigienizacaoDescricao = PropertyEntity({ text: "Descrição:" });
    this.InicioHigienizacaoPermiteAvancarAutomaticamenteComVeiculosHigienizados = PropertyEntity({ text: "Permite avançar automaticamente com veículos higienizados", getType: typesKnockout.bool, val: ko.observable(false) });
    this.InicioHigienizacaoPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.InicioHigienizacaoPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false) });
    this.InicioHigienizacaoNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.FimHigienizacaoDescricao = PropertyEntity({ text: "Descrição:" });
    this.FimHigienizacaoPermiteAvancarAutomaticamenteComVeiculosHigienizados = PropertyEntity({ text: "Permite avançar automaticamente com veículos higienizados", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FimHigienizacaoPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.FimHigienizacaoPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false) });
    this.FimHigienizacaoNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.InicioCarregamentoDescricao = PropertyEntity({ text: "Descrição:" });
    this.InicioCarregamentoPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.InicioCarregamentoPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.InicioCarregamentoNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.InicioCarregamentoTipoIntegracao = PropertyEntity({ text: "Integração", val: ko.observable(0), visible: ko.observable(true), options: ko.observable(new Array()), def: 0 });

    this.FimCarregamentoDescricao = PropertyEntity({ text: "Descrição:" });
    this.FimCarregamentoPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.FimCarregamentoPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.FimCarregamentoPermiteAvancarSomenteDadosTransporteInformados = PropertyEntity({ text: "Permite avançar somente com dados de transporte informados", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.FimCarregamentoNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.FimCarregamentoTipoIntegracao = PropertyEntity({ text: "Integração", val: ko.observable(0), visible: ko.observable(true), options: ko.observable(new Array()), def: 0 });

    this.SeparacaoMercadoriaDescricao = PropertyEntity({ text: "Descrição:" });
    this.SeparacaoMercadoriaPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.SeparacaoMercadoriaPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.SeparacaoMercadoriaNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.AvaliacaoDescargaDescricao = PropertyEntity({ text: "Descrição:" });
    this.AvaliacaoDescargaPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.SolicitacaoVeiculoDescricao = PropertyEntity({ text: "Descrição:" });
    this.SolicitacaoVeiculoPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.SolicitacaoVeiculoPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.SolicitacaoVeiculoPermiteEnvioSMSMotorista = PropertyEntity({ text: "Permite enviar SMS para o Motorista", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.SolicitacaoVeiculoNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.SolicitacaoVeiculoTipoIntegracao = PropertyEntity({ text: "Integração", val: ko.observable(0), visible: ko.observable(true), options: ko.observable(new Array()), def: 0 });

    this.InicioDescarregamentoDescricao = PropertyEntity({ text: "Descrição:" });
    this.InicioDescarregamentoPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.InicioDescarregamentoPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.InicioDescarregamentoNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.FimDescarregamentoDescricao = PropertyEntity({ text: "Descrição:" });
    this.FimDescarregamentoPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.FimDescarregamentoPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.FimDescarregamentoNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.DocumentoFiscalDescricao = PropertyEntity({ text: "Descrição:" });
    this.DocumentoFiscalPermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.DocumentoFiscalPermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.DocumentoFiscalNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.DocumentoFiscalPermiteAvancarAutomaticamenteAposNotasFiscaisInseridas = PropertyEntity({ text: "Permite avançar automaticamente após Notas Fiscais Inseridas", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.DocumentosTransporteDescricao = PropertyEntity({ text: "Descrição:" });
    this.DocumentosTransportePermiteQRCode = PropertyEntity({ text: "Permite QR Code", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.DocumentosTransportePermiteVoltar = PropertyEntity({ text: "Permite voltar", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.DocumentosTransporteNotificarMotoristaApp = PropertyEntity({ text: "Notificar motorista no app", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.DocumentosTransportePermiteAvancarAutomaticamenteAposGerarDocumentos = PropertyEntity({ text: "Permite avançar automaticamente após gerar os documentos", getType: typesKnockout.bool, val: ko.observable(true), visible: ko.observable(true) });
    this.DocumentosTransporteTipoIntegracao = PropertyEntity({ text: "Integração", val: ko.observable(0), visible: ko.observable(true), options: ko.observable(new Array()), def: 0 });

    this.MacroInicioViagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Macro Início da Viagem:", idBtnSearch: guid() });
    this.MacroChegadaDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Macro Chegada no Destinatário:", idBtnSearch: guid() });
    this.MacroSaidaDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Macro Saída do Destinatário:", idBtnSearch: guid() });
    this.MacroFimViagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Macro Fim da Viagem:", idBtnSearch: guid() });

    this.TipoDeOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Ocorrencia:", idBtnSearch: guid() });

    this.ViewFluxoPatioTabelado = PropertyEntity({ text: "Layout tabelado" });
    this.ExibirComprovanteSaida = PropertyEntity({ text: "Exibir comprovante de saída" });
    this.IniciarViagemSemGuarita = PropertyEntity({ text: "Iniciar viagem sem liberar na guarita", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarObservacaoEtapa = PropertyEntity({ text: "Habilitar observação nas etapas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirDetalhesIdentificacaoFluxo = PropertyEntity({ text: "Exibir detalhes na identificação do fluxo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.OcultarFluxoCarga = PropertyEntity({ text: "Não exibir o fluxo da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.DocaDetalhada = PropertyEntity({ text: "Detalhar informações na etapa da doca", getType: typesKnockout.bool, val: ko.observable(false) });
    this.OcultarTransportador = PropertyEntity({ text: "Ocultar transportador nas etapas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarPreCarga = PropertyEntity({ text: "Habilitar pré carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirTempoPrevistoERealizado = PropertyEntity({ text: "Exibir o tempo previsto e tempo realizado nas etapas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirRejeicaoFluxo = PropertyEntity({ text: "Permitir rejeição do fluxo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ListarCargasCanceladas = PropertyEntity({ text: "Listar cargas canceladas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IdentificacaoFluxoExibirTipoOperacao = PropertyEntity({ text: "Id Fluxo - Tipo Operação", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IdentificacaoFluxoExibirCodigoIntegracaoFilial = PropertyEntity({ text: "Id Fluxo - Código de Integração da Filial", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirSiglaFilial = PropertyEntity({ text: "Exibir Sigla da Filial", getType: typesKnockout.bool, val: ko.observable(false) });

    this.IdentificacaoFluxoExibirModeloVeicularCargaVeiculo = PropertyEntity({ text: "Id Fluxo - Modelo Veicular de Carga do Veículo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IdentificacaoFluxoExibirOrigemXDestinos = PropertyEntity({ text: "Id Fluxo - Origem x Destino", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SempreExibirPrevistoXRealizadoEDiferenca = PropertyEntity({ text: "Sempre exibir previsto x realizado + diferença", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SempreAtualizarDataPrevistaAoAlterarHorarioCarregamento = PropertyEntity({ text: "Sempre atualizar a data prevista ao alterar o horário de carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.VisualizarPlacaReboque = PropertyEntity({ text: "Visualizar Placa Reboque", getType: typesKnockout.bool, val: ko.observable(false) });
    this.VisualizarPlacaTracao = PropertyEntity({ text: "Visualizar Placa Tração", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteCancelarFluxoPatioAtual = PropertyEntity({ text: "Permite cancelar o fluxo de pátio atual (será gerado um novo fluxo)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AvancarCargaAgrupadaApenasComAsCargasFilhasAvancadas = PropertyEntity({ text: "Avançar fluxo de cargas agrupadas somente quando as cargas filhas estiverem avançadas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IniciarFluxoPatioSomenteComCarregamentoAgendado = PropertyEntity({ text: "Iniciar o fluxo de pátio somente com o carregamento agendado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigatorioInformarDataInicial = PropertyEntity({ text: "Obrigatório informar a data inicial na pesquisa do fluxo de pátio.", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IntegrarFluxoPatioWMS = PropertyEntity({ text: "Integrar fluxo de pátio ao WMS", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarFluxoPatioCargaCanceladaAoReenviarCarga = PropertyEntity({ text: "Utilizar fluxo de pátio de carga cancelada ao reenviar a carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarDataPrevistaEtapaAtualAtivarAlerta = PropertyEntity({ text: "Utilizar data prevista da etapa atual para ativar alerta", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarFluxoDestinoAntesFinalizarOrigem = PropertyEntity({ text: "Gerar fluxo de destino antes de finalizar fluxo de origem", getType: typesKnockout.bool, val: ko.observable(false) });

    this.RelatorioFluxoHorarioQuantidadeBaixa = PropertyEntity({ text: "Quantidade Verde Relatório Fluxo:", getType: typesKnockout.int, val: ko.observable(0) });
    this.RelatorioFluxoHorarioQuantidadeBaixa.configInt.allowZero = true;
    this.RelatorioFluxoHorarioQuantidadeNormal = PropertyEntity({ text: "Quantidade Amarelo Relatório Fluxo:", getType: typesKnockout.int, val: ko.observable(0) });
    this.RelatorioFluxoHorarioQuantidadeNormal.configInt.allowZero = true;
    this.RelatorioFluxoHorarioQuantidadeAlta = PropertyEntity({ text: "Quantidade Vermelho Relatório Fluxo:", getType: typesKnockout.int, val: ko.observable(0) });
    this.RelatorioFluxoHorarioQuantidadeAlta.configInt.allowZero = true;

    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadAlturarBoxesConfiguracaoGestaoPatio() {
    $styleAlturaBoxConfiguracaoGestaoPatio.html("");

    var maiorAltura = 0;

    $boxesConfiguracaoGestaoPatio.find(".box fieldset").each(function () {
        var $el = $(this);
        var height = $el.height();

        maiorAltura = height > maiorAltura ? height : maiorAltura;
    });

    var style = $templateAlturaBoxConfiguracaoGestaoPatio.html();

    style = style.replace(/{{altura}}/, maiorAltura);

    $styleAlturaBoxConfiguracaoGestaoPatio.html(style);
}

function loadConfiguracaoGestaoPatio() {
    _configuracaoGestaoPatio = new ConfiguracaoGestaoPatio();
    KoBindings(_configuracaoGestaoPatio, "knockoutConfiguracaoGestaoPatio");

    HeaderAuditoria("ConfiguracaoGestaoPatio", _configuracaoGestaoPatio);

    BuscarMacro(_configuracaoGestaoPatio.MacroInicioViagem);
    BuscarMacro(_configuracaoGestaoPatio.MacroChegadaDestinatario);
    BuscarMacro(_configuracaoGestaoPatio.MacroSaidaDestinatario);
    BuscarMacro(_configuracaoGestaoPatio.MacroFimViagem);
    new BuscarTipoOcorrencia(_configuracaoGestaoPatio.TipoDeOcorrencia);

    EditarConfiguracaoGestaoPatioPadrao();

    $("#liTabConfiguracaoEtapas").one('click', function () {
        setTimeout(loadAlturarBoxesConfiguracaoGestaoPatio, 100);
    });
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function atualizarClick(e, sender) {
    Salvar(_configuracaoGestaoPatio, "ConfiguracaoGestaoPatio/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function EditarConfiguracaoGestaoPatioPadrao() {
    BuscarPorCodigo(_configuracaoGestaoPatio, "ConfiguracaoGestaoPatio/ConfiguracaoGestaoPatioPadrao", function (arg) {
        if (arg.Data) {
            const integracoes = [
                { entity: _configuracaoGestaoPatio.ChegadaVeiculoTipoIntegracao, property: "ChegadaVeiculoTipoIntegracao" },
                { entity: _configuracaoGestaoPatio.GuaritaEntradaTipoIntegracao, property: "GuaritaEntradaTipoIntegracao" },
                { entity: _configuracaoGestaoPatio.GuaritaSaidaTipoIntegracao, property: "GuaritaSaidaTipoIntegracao" },
                { entity: _configuracaoGestaoPatio.ChecklistTipoIntegracao, property: "ChecklistTipoIntegracao" },
                { entity: _configuracaoGestaoPatio.DocumentosTransporteTipoIntegracao, property: "DocumentosTransporteTipoIntegracao" },
                { entity: _configuracaoGestaoPatio.SolicitacaoVeiculoTipoIntegracao, property: "SolicitacaoVeiculoTipoIntegracao" },
                { entity: _configuracaoGestaoPatio.InformarDocaCarregamentoTipoIntegracao, property: "InformarDocaCarregamentoTipoIntegracao" },
                { entity: _configuracaoGestaoPatio.InicioCarregamentoTipoIntegracao, property: "InicioCarregamentoTipoIntegracao" },
                { entity: _configuracaoGestaoPatio.FimCarregamentoTipoIntegracao, property: "FimCarregamentoTipoIntegracao" }
            ];

            integracoes.forEach(function (integracao) {
                const options = [{ text: arg.Data[integracao.property].text, value: arg.Data[integracao.property].value }];
                integracao.entity.options(options);
            });
        }

        ObterTipoIntegracao();
    }, null);
}

function ObterTipoIntegracao() {
    let integracaoOption = new Array();
    let integracaoOptionBind = new Array();

    executarReST("ConfiguracaoGestaoPatio/BuscarTiposIntegracao", null, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                const integracoes = arg.Data;

                if (integracoes.length > 0) {
                    for (let i = 0; i < integracoes.length; i++) {
                        integracaoOption.push({ text: integracoes[i].text, value: integracoes[i].value });
                    }

                    // Doca Carregamento // Início Carregamento // Fim Carregamento
                    for (let i = 0; i < integracoes.length; i++) {
                        if (integracoes[i].value == EnumTipoIntegracao.Bind || integracoes[i].value == EnumTipoIntegracao.NaoInformada || integracoes[i].value == EnumTipoIntegracao.NaoPossuiIntegracao)
                            integracaoOptionBind.push({ text: integracoes[i].text, value: integracoes[i].value });
                    }

                    _configuracaoGestaoPatio.InformarDocaCarregamentoTipoIntegracao.options(integracaoOptionBind);
                    _configuracaoGestaoPatio.InicioCarregamentoTipoIntegracao.options(integracaoOptionBind);
                    _configuracaoGestaoPatio.FimCarregamentoTipoIntegracao.options(integracaoOptionBind);

                    _configuracaoGestaoPatio.ChegadaVeiculoTipoIntegracao.options(integracaoOption);
                    _configuracaoGestaoPatio.GuaritaEntradaTipoIntegracao.options(integracaoOption);
                    _configuracaoGestaoPatio.GuaritaSaidaTipoIntegracao.options(integracaoOption);
                    _configuracaoGestaoPatio.ChecklistTipoIntegracao.options(integracaoOption);
                    _configuracaoGestaoPatio.DocumentosTransporteTipoIntegracao.options(integracaoOption);
                    _configuracaoGestaoPatio.SolicitacaoVeiculoTipoIntegracao.options(integracaoOption);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}

// #endregion Funções Privadas
