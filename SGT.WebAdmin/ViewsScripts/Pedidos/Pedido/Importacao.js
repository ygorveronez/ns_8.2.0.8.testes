/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoTerminalImportacao.js" />
/// <reference path="Pedido.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _importacao;
var _gridDI;
var _gridTransbordo;
//var _gridComponente;
var _gridDestinatarioBloqueado;

var DIMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.NumeroDI = PropertyEntity({ type: types.map, val: "" });
    this.CodigoImportacao = PropertyEntity({ type: types.map, val: "" });
    this.CodigoReferencia = PropertyEntity({ type: types.map, val: "" });
    this.ValorCarga = PropertyEntity({ type: types.map, val: "" });
    this.Volume = PropertyEntity({ type: types.map, val: "" });
    this.Peso = PropertyEntity({ type: types.map, val: "" });
};

var TransbordoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.SequenciaTransbordo = PropertyEntity({ type: types.map, val: "" });
    this.PortoTransbordo = PropertyEntity({ type: types.map, val: "" });
    this.CodigoPortoTransbordo = PropertyEntity({ type: types.map, val: "" });
    this.NavioTransbordo = PropertyEntity({ type: types.map, val: "" });
    this.CodigoNavioTransbordo = PropertyEntity({ type: types.map, val: "" });
    this.TerminalTransbordo = PropertyEntity({ type: types.map, val: "" });
    this.CodigoTerminalTransbordo = PropertyEntity({ type: types.map, val: "" });
    this.PedidoViagemNavioTransbordo = PropertyEntity({ type: types.map, val: "" });
    this.CodigoPedidoViagemNavioTransbordo = PropertyEntity({ type: types.map, val: "" });
};

var ComponenteMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Valor = PropertyEntity({ type: types.map, val: "" });
    this.CodigoComponente = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoComponente = PropertyEntity({ type: types.map, val: "" });
};

var Importacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroContainer = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.NumeroContainer.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 1000 });
    this.NumeroBL = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.BL.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 1000 });
    this.NumeroNavio = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Navio.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 1000 });
    this.Porto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.PortoOrigem.getFieldDescription()), idBtnSearch: guid() });
    this.TipoTerminalImportacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.TerminalVazio.getFieldDescription()), idBtnSearch: guid() });

    this.EnderecoEntregaImportacao = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Endereco.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 1000 });
    this.BairroEntregaImportacao = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Bairro.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 1000 });
    this.CEPEntregaImportacao = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.CEP.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 1000 });
    this.LocalidadeEntregaImportacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.Municipio.getFieldDescription()), idBtnSearch: guid() });

    this.DataVencimentoArmazenamentoImportacao = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.Pedido.DataArmazenamentoVencimento.getFieldDescription()), getType: typesKnockout.date, required: false });
    this.ArmadorImportacao = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Armador.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 1000 });

    this.NumeroDI = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.NumeroDI.getRequiredFieldDescription(), required: false, visible: ko.observable(true), maxlength: 1000 });
    this.CodigoImportacao = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.CodImportacao.getRequiredFieldDescription(), required: false, visible: ko.observable(true), maxlength: 1000 });
    this.CodigoReferencia = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.CodReferencia.getRequiredFieldDescription(), required: false, visible: ko.observable(true), maxlength: 1000 });
    this.ValorCarga = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.ValorCarga.getRequiredFieldDescription(), required: false, visible: ko.observable(true) });
    this.Volume = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.Volume.getRequiredFieldDescription(), required: false, visible: ko.observable(true) });
    this.Peso = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.Peso.getRequiredFieldDescription(), required: false, visible: ko.observable(true) });

    this.GridDI = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    this.AdicionarDI = PropertyEntity({ eventClick: adicionarDIClick, type: types.event, text: ko.observable(Localization.Resources.Pedidos.Pedido.AdicionarDI.getFieldDescription()), visible: ko.observable(true) });

    //Multimodal
    this.Navio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.Navio.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false) });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.PortoDestino.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false) });
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.TerminalOrigem.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false) });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.TerminalDestino.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false) });
    this.DirecaoViagemMultimodal = PropertyEntity({ val: ko.observable(EnumDirecaoViagemMultimodal.Norte), options: EnumDirecaoViagemMultimodal.obterOpcoes(), def: EnumDirecaoViagemMultimodal.Norte, text: Localization.Resources.Pedidos.Pedido.Direcao.getFieldDescription(), required: false, visible: ko.observable(false) });

    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.Container.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false) });
    this.LacreContainerUmMultimodal = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.LacreContainerum.getFieldDescription(), required: false, visible: ko.observable(false), maxlength: 100 });
    this.LacreContainerDoisMultimodal = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.LacreContainerdois.getFieldDescription(), required: false, visible: ko.observable(false), maxlength: 100 });
    this.LacreContainerTresMultimodal = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.LacreContainertres.getFieldDescription(), required: false, visible: ko.observable(false), maxlength: 100 });
    this.TaraContainerMultimodal = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.TaraContainer.getFieldDescription(), required: false, visible: ko.observable(false), maxlength: 100 });
    this.ContainerTipoReserva = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.TipoContainerReserva.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false) });

    //Transbordo
    this.SequenciaTransbordo = PropertyEntity({ getType: typesKnockout.int, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.Sequencia.getFieldDescription(), configInt: { precision: 0, allowZero: true }, required: false, visible: ko.observable(false) });
    this.NavioTransbordo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.NivelTransbordo.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false) });
    this.TerminalTransbordo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.TerminalTransbordo.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false) });
    this.PortoTransbordo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.PortoTransbordo.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false) });
    this.PedidoViagemNavioTransbordo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.NavioViajemDirecaoTransbordo.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false) });

    this.GridTransbordo = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    this.AdicionarTransbordo = PropertyEntity({ eventClick: AdicionarTransbordoClick, type: types.event, text: ko.observable(Localization.Resources.Pedidos.Pedido.AdicionarTransbordo), visible: ko.observable(true) });

    //Componente
    //this.Valor = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false, allowNegative: false }, maxlength: 15, text: "*Valor:", required: false, visible: ko.observable(true), val: ko.observable("") });
    //this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Componente:"), idBtnSearch: guid(), visible: ko.observable(true) });

    //this.GridComponente = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    //this.AdicionarComponente = PropertyEntity({ eventClick: AdicionarcomponenteClick, type: types.event, text: ko.observable("Adicionar Componente"), visible: ko.observable(true) });

    //Destinatário Bloqueado
    this.CNPJCPFDestinatarioBloqueado = PropertyEntity({ getType: typesKnockout.cpfCnpj, text: Localization.Resources.Pedidos.Pedido.CNPJCPF.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 80 });

    this.GridDestinatarioBloqueado = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    this.AdicionarDestinatarioBloqueado = PropertyEntity({ eventClick: AdicionarDestinatarioBloqueadoClick, type: types.event, text: ko.observable(Localization.Resources.Pedidos.Pedido.AdicionarDestinarioBloqueado), visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadImportacao() {
    _importacao = new Importacao();
    KoBindings(_importacao, "knockoutImportacao");
    $("#" + _importacao.CEPEntregaImportacao.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });
    $("#tabImportacao").hide();
    $("#divTransbordo").hide();
    $("#divComponente").hide();

    new BuscarPorto(_importacao.Porto);
    new BuscarLocalidades(_importacao.LocalidadeEntregaImportacao);
    new BuscarTipoTerminalImportacao(_importacao.TipoTerminalImportacao);

    new BuscarPorto(_importacao.PortoDestino);
    new BuscarTipoTerminalImportacao(_importacao.TerminalOrigem);
    new BuscarTipoTerminalImportacao(_importacao.TerminalDestino);
    new BuscarTipoTerminalImportacao(_importacao.TerminalTransbordo);
    new BuscarPedidoViagemNavio(_importacao.PedidoViagemNavioTransbordo);
    new BuscarPorto(_importacao.PortoTransbordo);

    new BuscarContainers(_importacao.Container, RetornoContainer);
    new BuscarNavios(_importacao.Navio);
    new BuscarNavios(_importacao.NavioTransbordo);
    new BuscarTiposContainer(_importacao.ContainerTipoReserva);

    _importacao.Container.visible(true);  
    _importacao.LacreContainerUmMultimodal.visible(true);
    _importacao.LacreContainerDoisMultimodal.visible(true);
    _importacao.LacreContainerTresMultimodal.visible(true);
    _importacao.TaraContainerMultimodal.visible(true);
    _importacao.ContainerTipoReserva.visible(true);
    //new BuscarComponentesDeFrete(_importacao.ComponenteFrete);

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        _importacao.Navio.visible(false);

        _importacao.DirecaoViagemMultimodal.visible(false);

        _importacao.Container.required = true;
        _importacao.Container.text("Container:");

        _importacao.SequenciaTransbordo.visible(true);
        _importacao.NavioTransbordo.visible(false);
        _importacao.TerminalTransbordo.visible(true);
        _importacao.PortoTransbordo.visible(true);
        _importacao.PedidoViagemNavioTransbordo.visible(true);

        $("#tabImportacao").show();
        $("#divComponente").show();

        _importacao.NumeroNavio.visible(false);
        _importacao.NumeroContainer.visible(false);
        SetarCampoAbaImportacao(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal);
    }

    var excluir = {
        descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: function (data) {
            excluirDI(data);
        }, tamanho: "10", icone: ""
    };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "NumeroDI", title: Localization.Resources.Pedidos.Pedido.NumeroDI, width: "15%" },
        { data: "CodigoImportacao", title: Localization.Resources.Pedidos.Pedido.CodImportacao, width: "15%" },
        { data: "CodigoReferencia", title: Localization.Resources.Pedidos.Pedido.CodReferencia, width: "15%" },
        { data: "ValorCarga", title: Localization.Resources.Pedidos.Pedido.ValorCarga, width: "8%" },
        { data: "Volume", title: Localization.Resources.Pedidos.Pedido.Volume, width: "8%" },
        { data: "Peso", title: Localization.Resources.Pedidos.Pedido.Peso, width: "8%" }
    ];

    _gridDI = new BasicDataTable(_importacao.GridDI.idGrid, header, menuOpcoes);
    recarregarGridDI();


    var excluirTransbordo = {
        descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: function (data) {
            ExcluirTransbordo(data);
        }, tamanho: "10", icone: ""
    };
    var menuOpcoesTransbordo = new Object();
    menuOpcoesTransbordo.tipo = TypeOptionMenu.link;
    menuOpcoesTransbordo.opcoes = new Array();
    menuOpcoesTransbordo.opcoes.push(excluirTransbordo);

    var headerTransbordo = [
        { data: "Codigo", visible: false },
        { data: "CodigoPortoTransbordo", visible: false },
        { data: "CodigoTerminalTransbordo", visible: false },
        { data: "CodigoNavioTransbordo", visible: false },
        { data: "CodigoPedidoViagemNavioTransbordo", visible: false },
        { data: "SequenciaTransbordo", title: Localization.Resources.Pedidos.Pedido.Sequencia, width: "10%" },
        { data: "PortoTransbordo", title: Localization.Resources.Pedidos.Pedido.Porto, width: "20%" },
        { data: "TerminalTransbordo", title: Localization.Resources.Pedidos.Pedido.Terminal, width: "20%" },
        { data: "PedidoViagemNavioTransbordo", title: Localization.Resources.Pedidos.Pedido.ViajemNavioDirecao, width: "20%" },
        { data: "NavioTransbordo", visible: false }
    ];

    _gridTransbordo = new BasicDataTable(_importacao.GridTransbordo.idGrid, headerTransbordo, menuOpcoesTransbordo);
    recarregarGridTransbordo();


    //var excluirComponente = {
    //    descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
    //        excluirComponenteFrete(data);
    //    }, tamanho: "10", icone: ""
    //};
    //var menuOpcoesComponente = new Object();
    //menuOpcoesComponente.tipo = TypeOptionMenu.link;
    //menuOpcoesComponente.opcoes = new Array();
    //menuOpcoesComponente.opcoes.push(excluirComponente);

    //var headerComponente = [
    //    { data: "Codigo", visible: false },
    //    { data: "CodigoComponente", visible: false },
    //    { data: "DescricaoComponente", title: "Componente", width: "60%" },
    //    { data: "Valor", title: "Valor", width: "10%" }
    //];

    //_gridComponente = new BasicDataTable(_importacao.GridComponente.idGrid, headerComponente, menuOpcoesComponente);
    //recarregarGridComponente();

    var excluirDestinatario = {
        descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: function (data) {
            excluirDestinatarioBloqueado(data);
        }, tamanho: "10", icone: ""
    };
    var menuOpcoesDestinatarioBloqueado = new Object();
    menuOpcoesDestinatarioBloqueado.tipo = TypeOptionMenu.link;
    menuOpcoesDestinatarioBloqueado.opcoes = new Array();
    menuOpcoesDestinatarioBloqueado.opcoes.push(excluirDestinatario);

    var headerDestinatarioBloqueado = [
        { data: "CNPJCPFDestinatarioBloqueado", title: Localization.Resources.Pedidos.Pedido.CNPJCPF, width: "70%" }
    ];

    _gridDestinatarioBloqueado = new BasicDataTable(_importacao.GridDestinatarioBloqueado.idGrid, headerDestinatarioBloqueado, menuOpcoesDestinatarioBloqueado);
    recarregarGridDestinatarioBloqueado();
}

function RetornoContainer(data) {
    _importacao.Container.codEntity(data.Codigo);
    _importacao.Container.val(data.Descricao);
    if (data.Tara !== null && data.Tara !== undefined && data.Tara !== "")
        _importacao.TaraContainerMultimodal.val(data.Tara);
}

//function AdicionarcomponenteClick(e, sender) {
//    var tudoCerto = true;
//    if (_importacao.Valor.val() === "" || _importacao.Valor.val() === "0,00")
//        tudoCerto = false;
//    if (_importacao.ComponenteFrete.codEntity() === 0 || _importacao.ComponenteFrete.codEntity() === "")
//        tudoCerto = false;

//    if (tudoCerto) {
//        var map = new Object();

//        map.Codigo = guid();
//        map.Valor = _importacao.Valor.val();
//        map.DescricaoComponente = _importacao.ComponenteFrete.val();
//        map.CodigoComponente = _importacao.ComponenteFrete.codEntity();

//        _importacao.GridComponente.list.push(map);

//        recarregarGridComponente();
//        limparDadosComponente();
//        $("#" + _importacao.ComponenteFrete.id).focus();
//    } else {

//        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios no laçamento do componente!");
//    }
//}

function AdicionarDestinatarioBloqueadoClick(e, sender) {
    var tudoCerto = true;
    if (_importacao.CNPJCPFDestinatarioBloqueado.val() === "")
        tudoCerto = false;

    $.each(_importacao.GridDestinatarioBloqueado.list, function (i, dest) {
        if (dest.CNPJCPFDestinatarioBloqueado === _importacao.CNPJCPFDestinatarioBloqueado.val())
            tudoCerto = false;
    });

    if (tudoCerto) {
        var map = new Object();

        map.CNPJCPFDestinatarioBloqueado = _importacao.CNPJCPFDestinatarioBloqueado.val();

        _importacao.GridDestinatarioBloqueado.list.push(map);

        recarregarGridDestinatarioBloqueado();
        limparDadosDestinatarioBloqueado();
        $("#" + _importacao.CNPJCPFDestinatarioBloqueado.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pedidos.Pedido.InformeOsCamposObrigatoriosDoDestinatario);
    }
}

function AdicionarTransbordoClick(e, sender) {
    var tudoCerto = true;
    if (_importacao.PedidoViagemNavioTransbordo.val() == "")
        tudoCerto = false;
    if (_importacao.TerminalTransbordo.val() == "")
        tudoCerto = false;
    if (_importacao.SequenciaTransbordo.val() == "")
        tudoCerto = false;
    if (_importacao.PortoTransbordo.val() == "")
        tudoCerto = false;

    if (tudoCerto) {
        var map = new Object();

        map.Codigo = guid();
        map.SequenciaTransbordo = _importacao.SequenciaTransbordo.val();

        map.CodigoPortoTransbordo = _importacao.PortoTransbordo.codEntity();
        map.PortoTransbordo = _importacao.PortoTransbordo.val();

        map.CodigoTerminalTransbordo = _importacao.TerminalTransbordo.codEntity();
        map.TerminalTransbordo = _importacao.TerminalTransbordo.val();

        map.CodigoNavioTransbordo = _importacao.NavioTransbordo.codEntity();
        map.NavioTransbordo = _importacao.NavioTransbordo.val();

        map.PedidoViagemNavioTransbordo = _importacao.PedidoViagemNavioTransbordo.val();
        map.CodigoPedidoViagemNavioTransbordo = _importacao.PedidoViagemNavioTransbordo.codEntity();

        _importacao.GridTransbordo.list.push(map);

        recarregarGridTransbordo();
        limparDadosTransbordo();
        $("#" + _importacao.SequenciaTransbordo.id).focus();
    } else {

        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pedidos.Pedido.InformeOsCamposObrigatoriosNoLancamentoDoTransbordo);
    }
}

function adicionarDIClick(e, sender) {

    var tudoCerto = true;
    if (_importacao.NumeroDI.val() == "")
        tudoCerto = false;
    if (_importacao.CodigoImportacao.val() == "")
        tudoCerto = false;
    if (_importacao.CodigoReferencia.val() == "")
        tudoCerto = false;
    if (_importacao.ValorCarga.val() == "")
        tudoCerto = false;
    if (_importacao.Volume.val() == "")
        tudoCerto = false;
    if (_importacao.Peso.val() == "")
        tudoCerto = false;

    if (tudoCerto) {
        var map = new Object();

        map.Codigo = guid();
        map.NumeroDI = _importacao.NumeroDI.val();
        map.CodigoImportacao = _importacao.CodigoImportacao.val();
        map.CodigoReferencia = _importacao.CodigoReferencia.val();
        map.ValorCarga = _importacao.ValorCarga.val();
        map.Volume = _importacao.Volume.val();
        map.Peso = _importacao.Peso.val();

        _importacao.GridDI.list.push(map);

        recarregarGridDI();
        limparDadosDI();
        $("#" + _importacao.NumeroDI.id).focus();
    } else {

        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pedidos.Pedido.InformeOsCamposObrigatoriosNoLancamentoDaDI);
    }
}

//function excluirComponenteFrete(e) {
//    exibirConfirmacao("Confirmação", "Realmente deseja remover o componente selecionado?", function () {
//        $.each(_importacao.GridComponente.list, function (i, di) {
//            if (di != null && di.Codigo != null && e != null && e.Codigo != null && e.Codigo == di.Codigo)
//                _importacao.GridComponente.list.splice(i, 1);
//        });
//        recarregarGridComponente();
//    });   
//}

function excluirDestinatarioBloqueado(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.Pedido.RealmenteDesejaRemoverDestinatarioBloqueado, function () {
        $.each(_importacao.GridDestinatarioBloqueado.list, function (i, dest) {
            if (dest != null && dest.CNPJCPFDestinatarioBloqueado != null && e != null && e.CNPJCPFDestinatarioBloqueado != null && e.CNPJCPFDestinatarioBloqueado == dest.CNPJCPFDestinatarioBloqueado)
                _importacao.GridDestinatarioBloqueado.list.splice(i, 1);
        });
        recarregarGridDestinatarioBloqueado();
    });
}

function ExcluirTransbordo(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.Pedido.RealmenteDesejaRemoverTransbordoSeleceionado, function () {
        $.each(_importacao.GridTransbordo.list, function (i, di) {
            if (di != null && di.Codigo != null && e != null && e.Codigo != null && e.Codigo == di.Codigo)
                _importacao.GridTransbordo.list.splice(i, 1);
        });
        recarregarGridTransbordo();
    });
}

function excluirDI(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.Pedido.RealmenteDesejaRemoverDI.format(e.NumeroDI), function () {
        $.each(_importacao.GridDI.list, function (i, di) {
            if (di != null && di.Codigo != null && e != null && e.Codigo != null && e.Codigo == di.Codigo)
                _importacao.GridDI.list.splice(i, 1);
        });
        recarregarGridDI();
    });
}

//*******MÉTODOS*******

//function recarregarGridComponente() {
//    var data = new Array();
//    $.each(_importacao.GridComponente.list, function (i, DI) {
//        var obj = new Object();

//        obj.Codigo = DI.Codigo;
//        obj.CodigoComponente = DI.CodigoComponente;
//        obj.DescricaoComponente = DI.DescricaoComponente;
//        obj.Valor = DI.Valor;

//        data.push(obj);
//    });
//    _gridComponente.CarregarGrid(data);
//}

function recarregarGridDestinatarioBloqueado() {
    var data = new Array();
    $.each(_importacao.GridDestinatarioBloqueado.list, function (i, dest) {
        var obj = new Object();

        obj.CNPJCPFDestinatarioBloqueado = dest.CNPJCPFDestinatarioBloqueado;

        data.push(obj);
    });
    _gridDestinatarioBloqueado.CarregarGrid(data);
}

function recarregarGridTransbordo() {
    var data = new Array();
    $.each(_importacao.GridTransbordo.list, function (i, DI) {
        var obj = new Object();

        obj.Codigo = DI.Codigo;
        obj.SequenciaTransbordo = DI.SequenciaTransbordo;
        obj.CodigoPortoTransbordo = DI.CodigoPortoTransbordo;
        obj.PortoTransbordo = DI.PortoTransbordo;
        obj.CodigoTerminalTransbordo = DI.CodigoTerminalTransbordo;
        obj.TerminalTransbordo = DI.TerminalTransbordo;
        obj.CodigoNavioTransbordo = DI.CodigoNavioTransbordo;
        obj.NavioTransbordo = DI.NavioTransbordo;
        obj.PedidoViagemNavioTransbordo = DI.PedidoViagemNavioTransbordo;
        obj.CodigoPedidoViagemNavioTransbordo = DI.CodigoPedidoViagemNavioTransbordo;

        data.push(obj);
    });
    _gridTransbordo.CarregarGrid(data);
}

function recarregarGridDI() {
    var data = new Array();
    $.each(_importacao.GridDI.list, function (i, DI) {
        var obj = new Object();

        obj.Codigo = DI.Codigo;
        obj.NumeroDI = DI.NumeroDI;
        obj.CodigoImportacao = DI.CodigoImportacao;
        obj.CodigoReferencia = DI.CodigoReferencia;
        obj.ValorCarga = DI.ValorCarga;
        obj.Volume = DI.Volume;
        obj.Peso = DI.Peso;

        data.push(obj);
    });
    _gridDI.CarregarGrid(data);
}

function VerificarDadosImportacao() {
    if (_pedido.TelaResumida.val())
        return true;

    if (ValidarCamposObrigatorios(_importacao)) {

        if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal && _importacao.Container.codEntity() > 0 && (_importacao.TaraContainerMultimodal.val() === "0" || _importacao.TaraContainerMultimodal.val() === "" || _importacao.TaraContainerMultimodal.val() === null || _importacao.TaraContainerMultimodal.val() === undefined)) {
            $("#myTab a[href='#tabImportacao']").tab("show");
            $("#myTab a[href='#knockoutImportacao']").tab("show");
            exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Pedidos.Pedido.PorFavorInformeTaraDoContainer);
            return false;
        }

        //_pedido.NumeroContainer.val(_importacao.NumeroContainer.val());
        _pedido.NumeroBL.val(_importacao.NumeroBL.val());
        _pedido.NumeroNavio.val(_importacao.NumeroNavio.val());
        _pedido.Porto.val(_importacao.Porto.codEntity());
        _pedido.TipoTerminalImportacao.val(_importacao.TipoTerminalImportacao.codEntity());
        _pedido.EnderecoEntregaImportacao.val(_importacao.EnderecoEntregaImportacao.val());
        _pedido.BairroEntregaImportacao.val(_importacao.BairroEntregaImportacao.val());
        _pedido.CEPEntregaImportacao.val(_importacao.CEPEntregaImportacao.val());
        _pedido.LocalidadeEntregaImportacao.val(_importacao.LocalidadeEntregaImportacao.codEntity());
        _pedido.DataVencimentoArmazenamentoImportacao.val(_importacao.DataVencimentoArmazenamentoImportacao.val());
        _pedido.ArmadorImportacao.val(_importacao.ArmadorImportacao.val());
        _pedido.NumeroDI.val(_importacao.NumeroDI.val());
        _pedido.CodigoImportacao.val(_importacao.CodigoImportacao.val());
        _pedido.CodigoReferencia.val(_importacao.CodigoReferencia.val());
        _pedido.ValorCarga.val(_importacao.ValorCarga.val());
        _pedido.Volume.val(_importacao.Volume.val());
        _pedido.Peso.val(_importacao.Peso.val());

        _pedido.Navio.val(_importacao.Navio.codEntity());
        _pedido.PortoDestino.val(_importacao.PortoDestino.codEntity());
        _pedido.TerminalOrigem.val(_importacao.TerminalOrigem.codEntity());
        _pedido.TerminalDestino.val(_importacao.TerminalDestino.codEntity());
        _pedido.DirecaoViagemMultimodal.val(_importacao.DirecaoViagemMultimodal.val());
        _pedido.Container.val(_importacao.Container.codEntity());
        _pedido.LacreContainerUmMultimodal.val(_importacao.LacreContainerUmMultimodal.val());
        _pedido.LacreContainerDoisMultimodal.val(_importacao.LacreContainerDoisMultimodal.val());
        _pedido.LacreContainerTresMultimodal.val(_importacao.LacreContainerTresMultimodal.val());
        _pedido.TaraContainerMultimodal.val(_importacao.TaraContainerMultimodal.val());
        _pedido.ContainerTipoReserva.val(_importacao.ContainerTipoReserva.codEntity());

        _pedido.GridDI.list = new Array();
        var listaDI = new Array();
        $.each(_importacao.GridDI.list, function (i, di) {
            listaDI.push({ DI: di });
        });
        _pedido.GridDI.val(JSON.stringify(listaDI));

        _pedido.GridTransbordo.list = new Array();
        var listaTransbordo = new Array();
        $.each(_importacao.GridTransbordo.list, function (i, transb) {
            listaTransbordo.push({ Transbordo: transb });
        });
        _pedido.GridTransbordo.val(JSON.stringify(listaTransbordo));

        //_pedido.GridComponente.list = new Array();
        //var listaComponente = new Array();
        //$.each(_importacao.GridComponente.list, function (i, comp) {
        //    listaComponente.push({ Componente: comp });
        //});
        //_pedido.GridComponente.val(JSON.stringify(listaComponente));

        _pedido.GridDestinatarioBloqueado.list = new Array();
        var listaDestinatarioBloqueado = new Array();
        $.each(_importacao.GridDestinatarioBloqueado.list, function (i, dest) {
            listaDestinatarioBloqueado.push(dest);
        });
        _pedido.GridDestinatarioBloqueado.val(JSON.stringify(listaDestinatarioBloqueado));

        return true;
    } else {
        $("#myTab a[href='#tabImportacao']").tab("show");
        $("#myTab a[href='#knockoutImportacao']").tab("show");
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Pedidos.Pedido.InformeCampoObrigatorio);
        return false;
    }
}

function limparDadosTransbordo() {
    _importacao.SequenciaTransbordo.val("");
    LimparCampoEntity(_importacao.PortoTransbordo);
    LimparCampoEntity(_importacao.TerminalTransbordo);
    LimparCampoEntity(_importacao.NavioTransbordo);
    LimparCampoEntity(_importacao.PedidoViagemNavioTransbordo);
}

function limparDadosDI() {
    _importacao.NumeroDI.val("");
    _importacao.CodigoImportacao.val("");
    _importacao.CodigoReferencia.val("");
    _importacao.ValorCarga.val("");
    _importacao.Volume.val("");
    _importacao.Peso.val("");
}

//function limparDadosComponente() {
//    LimparCampoEntity(_importacao.ComponenteFrete);
//    _importacao.Valor.val("");
//}

function limparDadosDestinatarioBloqueado() {
    _importacao.CNPJCPFDestinatarioBloqueado.val("");
}

function limparDI() {
    LimparCampos(_importacao);
}

function SetarCampoAbaImportacao(exibir) {
    _importacao.PortoDestino.visible(exibir);
    _importacao.TerminalOrigem.visible(exibir);
    _importacao.TerminalDestino.visible(exibir);

    if (exibir)
        $("#divTransbordo").show();
    else
        $("#divTransbordo").hide();
}