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
/// <reference path="../../../js/Global/MapaDraw.js" />
/// <reference path="../../../js/Global/MapaGoogle.js" />
/// <reference path="../../../js/Global/Mapa.js"/>
/// <reference path="../../Enumeradores/EnumTipoTipoSubareaCliente.js" />
/// <reference path="Pessoa.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _gridListaSubarea;
var _listaSubarea;
var _subarea;
var _subareaAcoesFluxodePatio;
var _mapSubarea = null;
var _mapDrawSubarea = null;
var _tiposSubareaCliente = [];

var infowindow = new google.maps.InfoWindow({
    size: new google.maps.Size(150, 50)
});

var ListaSubarea = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Adicionar = PropertyEntity({ eventClick: exibirSubareaModal, type: types.event, text: Localization.Resources.Pessoas.Pessoa.AdicionarSubarea, visible: ko.observable(true) });
    this.Visualizar = PropertyEntity({ eventClick: exibirTodasSubareasModal, type: types.event, text: Localization.Resources.Pessoas.Pessoa.VisualizarTodas, visible: ko.observable(false) });
};

var Subarea = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), required: true, getType: typesKnockout.string, maxlength: 40, enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoSubarea = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Tipo.getFieldDescription(), required: true, val: ko.observable(""), options: ko.observableArray(_tiposSubareaCliente), def: "" });
    this.Ativo = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Situacao.getFieldDescription(), required: true, val: ko.observable(true), options: _status, def: true });
    this.Area = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Area.getFieldDescription(), required: true, getType: typesKnockout.string, maxlength: 10000, enable: ko.observable(true), visible: ko.observable(true) });
    this.FluxoDePatio = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.FluxoDePatio.getFieldDescription(), getType: typesKnockout.bool, maxlength: 10000, enable: ko.observable(true), visible: ko.observable(false) });
    this.CodigoTag = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.CodigoTag.getFieldDescription(), required: false, getType: typesKnockout.string, maxlength: 100, enable: ko.observable(true), visible: ko.observable(true) });

    this.Confirmar = PropertyEntity({ eventClick: confirmarSubareaClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Confirmar), visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarSubareaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, enable: ko.observable(true), visible: ko.observable(true) });

    this.TipoSubarea.val.subscribe(function (novoValor) {
        if (novoValor) {
            var fluxoDePatioVisible = obterPermissaoFluxoDePatioTipoSubarea(novoValor);
            _subarea.FluxoDePatio.visible(fluxoDePatioVisible);
        }
    })

    //SubareaClienteAcoesFluxoDePatio
    this.GridAcoesFluxoPatio = PropertyEntity({ type: types.local });

    this.CodigoAcao = PropertyEntity({ val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.AcaoMonitoramentoFluxoDePatio = PropertyEntity({ val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.string });
    this.CodigoAcaoMonitoramentoFluxoDePatio = PropertyEntity({ val: ko.observable(EnumMonitoramentoEventoData.EntradaCliente), options: EnumMonitoramentoEventoData.obterOpcoesFluxoDePatio(), def: ko.observable(EnumMonitoramentoEventoData.EntradaCliente), text: "Ação" });
    this.EtapaFluxoDePatio = PropertyEntity({ val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.string });
    this.CodigoEtapaFluxoDePatio = PropertyEntity({ val: ko.observable(null), options: EnumEtapaFluxoGestaoPatio.obterOpcoesGatilhoOcorrenciaFinal(), def: ko.observable(EnumEtapaFluxoGestaoPatio.Todas), text: "Etapa do Fluxo de Pátio" });
    this.AcaoFluxoDePatio = PropertyEntity({ val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.string });
    this.CodigoAcaoFluxoDePatio = PropertyEntity({ val: ko.observable(EnumAcaoFluxoGestaoPatio.Confirmar), options: EnumAcaoFluxoGestaoPatio.obterOpcoesSubareaCliente(), def: ko.observable(EnumAcaoFluxoGestaoPatio.Confirmar), text: "Ação de Pátio" });
    this.ListaSubareaClienteAcoesFluxoDePatio = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.AdicionarAcaoFluxoPatio = PropertyEntity({ eventClick: adicionarListaSubareaClienteAcoesFluxoDePatioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.AtualizarAcaoFluxoPatio = PropertyEntity({ eventClick: atualizarListaSubareaClienteAcoesFluxoDePatioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.CancelarAcaoFluxoPatio = PropertyEntity({ eventClick: limparCamposListaSubareaClienteAcoesFluxoDePatio, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.ExcluirAcaoFluxoPatio = PropertyEntity({ eventClick: excluirListaSubareaClienteAcoesFluxoDePatioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadListaSubarea() {
    $("#liGeoLocalizacao").show();

    _listaSubarea = new ListaSubarea();
    KoBindings(_listaSubarea, "knockoutListaSubarea");

    _subarea = new Subarea();
    KoBindings(_subarea, "knockoutCadastroSubarea");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarListaSubareaClick }, { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirListaSubareaClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Ativo", visible: false },
        { data: "TipoSubarea", visible: false },
        { data: "Area", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "70%" },
        { data: "TipoSubareaDescricao", title: Localization.Resources.Pessoas.Pessoa.Tipo, width: "20%" },
        { data: "AtivoDescricao", title: Localization.Resources.Gerais.Geral.Ativo, width: "10%" },
    ];

    _gridListaSubarea = new BasicDataTable(_listaSubarea.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    recarregarGridListaSubarea();

    //SubareaClienteAcoesFluxoDePatio
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarListaSubareaClienteAcoesFluxoDePatioClick }] };

    var header = [
        { data: "CodigoAcao", visible: false },
        { data: "AcaoMonitoramentoFluxoDePatio", title: "Ação" },
        { data: "EtapaFluxoDePatio", title: "Fluxo de Pátio" },
        { data: "AcaoFluxoDePatio", title: "Ação de Pátio" }
    ];

    _gridSubareaClienteAcoesFluxoDePatio = new BasicDataTable(_subarea.GridAcoesFluxoPatio.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    recarregarGridListaSubareaClienteAcoesFluxoDePatio();

}

function recarregarGridListaSubarea() {
    var data = _pessoa.ListaSubarea.val() || [];
    atualizarGridSubarea(data);
}

function excluirListaSubareaClick(data) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pessoas.Pessoa.RealmenteDesejaExcluirSubarea + data.Descricao + "\"?", function () {
        var indice = obterIndiceRegistroListaSubarea(data);
        if (indice >= 0) {
            subareas = _gridListaSubarea.BuscarRegistros();
            subareas.splice(indice, 1);
            atualizarGridSubarea(subareas);
        }
    });
}

function editarListaSubareaClick(data) {
    _subarea.Codigo.val(data.Codigo);
    _subarea.Descricao.val(data.Descricao);
    _subarea.TipoSubarea.val(data.TipoSubarea);
    _subarea.Ativo.val(data.Ativo);
    _subarea.Area.val(data.Area);
    _subarea.CodigoTag.val(data.CodigoTag);

    $.each(data.ListaSubareaClienteAcoesFluxoDePatio, function (i, acaoFluxoDePatio) {
        _subarea.ListaSubareaClienteAcoesFluxoDePatio.list.push(acaoFluxoDePatio);
    });
    recarregarGridListaSubareaClienteAcoesFluxoDePatio(_subarea.ListaSubareaClienteAcoesFluxoDePatio.list);
    exibirSubareaModal();
}

function confirmarSubareaClick(e, sender) {
    _subarea.Area.val(obterJsonSubareaMapa());
    if (ValidarCamposObrigatorios(_subarea)) {
        var record = obterSubareaSalvar();
        var subareas = _gridListaSubarea.BuscarRegistros();
        if (record.Codigo) {
            var indice = obterIndiceRegistroListaSubarea(record);
            subareas[indice] = record
        } else {
            record.Codigo = guid();
            subareas.push(record);
        }
        atualizarGridSubarea(subareas);

        $("#" + _listaSubarea.Adicionar.id).focus();
        limparCamposSubarea();
        fecharSubareaModal();

    } else {
        exibirMensagemCamposObrigatorio();
    }
}

function atualizarGridSubarea(subareas) {
    _listaSubarea.Visualizar.visible(subareas.length > 0);
    _gridListaSubarea.CarregarGrid(subareas);
    //_pessoa.ListaSubarea.val(subareas);
}

function cancelarSubareaClick() {
    fecharSubareaModal();
    limparCamposSubarea();
}

function exibirSubareaModal() {
    if (!validarGeolocalizacaoCliente()) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pessoas.Pessoa.AntesDeInformarAsSubareasDeveSerInformadaGeolocalizacaoPrincipal);
        return;
    }
    carregarTiposSubareaCliente();
    $("#divModalCadastroSubarea").modal("show")
        .one('hidden.bs.modal', function () {
            limparCamposSubarea();
        }).on('show.bs.modal', function (e) {

        });
    CarregarMapaSubarea();
}

function fecharSubareaModal() {
    Global.fecharModal("divModalCadastroSubarea");
}

function limparCamposListaSubarea() {
    LimparCampos(_listaSubarea);
    atualizarGridSubarea([]);
}

function limparCamposSubarea() {
    LimparCampos(_subarea);
}

function obterSubareaSalvar() {
    return {
        Codigo: _subarea.Codigo.val(),
        Descricao: _subarea.Descricao.val(),
        TipoSubarea: _subarea.TipoSubarea.val(),
        TipoSubareaDescricao: obterDescricaoTipoSubarea(_subarea.TipoSubarea.val()),
        Ativo: _subarea.Ativo.val(),
        AtivoDescricao: obterDescricaoStatus(_subarea.Ativo.val()),
        Area: _subarea.Area.val(),
        ListaSubareaClienteAcoesFluxoDePatio: _subarea.ListaSubareaClienteAcoesFluxoDePatio.list,
        CodigoTag: _subarea.CodigoTag.val()
    };
}

function obterDescricaoStatus(status) {
    for (var i = 0; i < _status.length; i++) {
        if (_status[i].value == status)
            return _status[i].text;
    }
}

function exibirTodasSubareasModal() {
    $("#divModalVisulializarTodasSubareas")
        .modal("show")
        .one('hidden.bs.modal', function () { })
        .on('show.bs.modal', function (e) { });
    CarregarMapaTodasSubareas("map-todas-subareas");
    setTimeout(function () { _mapDrawSubarea.centerShapes(); }, 500);
}

// Deve ter sido informada a localização, um raio ou uma área
function validarGeolocalizacaoCliente() {
    return _pessoa.Latitude.val() != null && _pessoa.Longitude.val() != null && _pessoa.Latitude.val() != "" && _pessoa.Longitude.val() != "" &&
        (
            (_pessoa.TipoArea.val() === 1 && parseInt(_pessoa.RaioEmMetros.val()) > 0)
            ||
            obterJsonPoligonoGeoLocalizacao() != ""
        );
}

function CarregarMapaSubarea() {
    var divMap = document.getElementById(_subarea.Area.id);
    CarregarMapaSA(divMap);
    CarregarMapaTodasSubareas(_subarea.Area.id, true);
    _mapDrawSubarea.ShowDrawPalette(divMap);
    if (_subarea.Area.val() != "") _mapDrawSubarea.setJson(_subarea.Area.val());
    setTimeout(function () { _mapDrawSubarea.centerShapes(); }, 500);
}

function CarregarMapaTodasSubareas(divMapId, apenasLinha = false) {
    var divMap = document.getElementById(divMapId);
    CarregarMapaSA(divMap);
    var data = _gridListaSubarea.BuscarRegistros();
    var json = [];
    for (var i = 0; i < data.length; i++) {
        var obj = JSON.parse(data[i].Area);
        for (var j = 0; j < obj.length; j++) {
            obj[j]['content'] = data[i].Descricao;
            if (apenasLinha) {
                obj[j]['fillOpacity'] = 0;
                obj[j]['strokeOpacity'] = 0.3;
                obj[j]['strokeWeight'] = 3;
                obj[j]['strokeColor'] = obj[j]['fillColor'];
            }
        }
        json = json.concat(obj);
    }
    _mapDrawSubarea.setJson(JSON.stringify(json));

    var shapes = _mapDrawSubarea.getShapesNotMarkers();
    for (var i = 0; i < shapes.length; i++) {
        AdicionarEventoClickSubarea(_mapSubarea, shapes[i]);
    }
}

function CarregarMapaSA(divMap) {
    _mapSubarea = new google.maps.Map(
        divMap,
        {
            zoom: 5,
            scaleControl: true,
            gestureHandling: 'greedy',
            center: {
                lat: -14.235004,
                lng: -51.925280
            }
        }
    );
    google.maps.event.addListener(_mapSubarea, 'click', function () { infowindow.close(); });

    _mapDrawSubarea = new MapaDraw(_mapSubarea);
    if (_pessoa.TipoArea.val() === 1) {
        var shapeCircle = new ShapeCircle();
        shapeCircle.center = { lat: parseFloat(_pessoa.Latitude.val()), lng: parseFloat(_pessoa.Longitude.val()) };
        shapeCircle.radius = parseInt(_pessoa.RaioEmMetros.val());
        shapeCircle = PadraoLinhaShapeSubarea(shapeCircle, Localization.Resources.Pessoas.Pessoa.RaioDoCliente);
        var newShape = _mapDrawSubarea.addShape(shapeCircle);
        AdicionarEventoClickSubarea(_mapSubarea, newShape);
    } else {
        var shapes = _mapDraw.getShapesNotMarkers();
        for (var i = 0; i < shapes.length; i++) {
            shapes[i] = PadraoLinhaShapeSubarea(shapes[i], Localization.Resources.Pessoas.Pessoa.AreaPrincipalDoCliente);
            if (shapes[i].type == google.maps.drawing.OverlayType.POLYGON) {
                shapes[i].paths = shapes[i].getPaths();
            }
            var newShape = _mapDrawSubarea.addShape(shapes[i]);
            AdicionarEventoClickSubarea(_mapSubarea, newShape);
        }
    }
}

function PadraoLinhaShapeSubarea(shape, content, strokeColor) {
    shape.content = content;
    shape.zIndex = -1;
    shape.fillOpacity = 0;
    shape.strokeWeight = 3;
    shape.strokeOpacity = 0.3;
    shape.strokeColor = (strokeColor == null) ? "#FF0000" : strokeColor;
    return shape;
}

function AdicionarEventoClickSubarea(map, shape) {
    google.maps.event.addListener(shape, 'click', function (event) {
        infowindow.setContent(this.content);
        infowindow.setPosition(event.latLng);
        infowindow.open(map);
    });
}

function obterJsonSubareaMapa() {
    var shapesSubarea = []
    var listShapes = _mapDrawSubarea.getShapes();
    for (var i = 0; i < listShapes.length; i++) {
        // Apenas os shapes com possibilidade de mover são a subárea
        // Os shapes que não poder ser movidos são os shapes da localização principal do cliente
        if (listShapes[i].draggable) {
            shapesSubarea.push(listShapes[i]);
        }
    }
    return _mapDraw.getJson(shapesSubarea);
}

function obterIndiceRegistroListaSubarea(record) {
    var subareas = _gridListaSubarea.BuscarRegistros();
    for (var i = 0; i < subareas.length; i++) {
        if (record.Codigo == subareas[i].Codigo) {
            return i;
        }
    }
    return -1;
}

function carregarTiposSubareaCliente() {
    executarReST("Pessoa/ConsultaTipoSubareaCliente", null, function (response) {
        if (response.Success) {
            if (response.Data != null) {
                var tipos = response.Data;
                _tiposSubareaCliente = [];
                for (var i = 0; i < tipos.length; i++) {
                    _tiposSubareaCliente.push({
                        text: tipos[i].Descricao,
                        value: tipos[i].Codigo,
                        PermiteFluxoDePatio: tipos[i].PermiteMovimentacaoDoPatioPorEntradaOuSaidaDaArea
                    });
                }
                _subarea.TipoSubarea.options(_tiposSubareaCliente);
            }
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.Pessoa.Atencao, response.Msg);
        }
    });
}

function obterDescricaoTipoSubarea(codigo) {
    for (var i = 0; i < _tiposSubareaCliente.length; i++) {
        if (codigo == _tiposSubareaCliente[i].value)
            return _tiposSubareaCliente[i].text;
    }
}
function obterPermissaoFluxoDePatioTipoSubarea(codigo) {
    for (var i = 0; i < _tiposSubareaCliente.length; i++) {
        if (codigo == _tiposSubareaCliente[i].value)
            return _tiposSubareaCliente[i].PermiteFluxoDePatio;
    }
}

function obterListaSubareas() {
    var string = JSON.stringify(_gridListaSubarea.BuscarRegistros());
    return string;
}