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
/// <reference path="../../Consultas/CargaEntrega.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/NotaFiscal.js" />
/// <reference path="../../Enumeradores/EnumTipoColetaEntregaDevolucao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoChamado.js" />
/// <reference path="../../Cargas/ControleEntregaDevolucao/ControleEntregaDevolucao.js" />

var _sobrasChamadoOcorrencia, _gridSobrasChamadoOcorrencia;

var SobrasChamadoOcorrencia = function () {
    this.Codigo = PropertyEntity({ def: 0, getType: typesKnockout.int, val: ko.observable(0) });

    this.PermiteSobras = PropertyEntity({ val: ko.observable(false) });

    this.PermiteSobras.val.subscribe((novoValor) => {
        //_sobrasChamadoOcorrencia.MostrarCamposSobra.visible(novoValor);
        $("#liTabAnaliseSobras").show();
    });

    this.MostrarCamposSobra = PropertyEntity({ eventClick: exibirCamposSobraChamadoOcorrencia, type: types.event, text: "Adicionar Sobra", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), icon: ko.observable("fal fa-plus") });
    this.GridSobra = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.CodigoSobra = PropertyEntity({ text: "Código do Produto:", val: ko.observable(""), def: "", visible: ko.observable(true), required: true });
    this.QuantidadeSobra = PropertyEntity({ getType: typesKnockout.int, text: "Quantidade:", val: ko.observable(""), def: "", visible: ko.observable(true), required: true });
    this.AdicionarSobra = PropertyEntity({ eventClick: adicionarSobraChamadoOcorrenciaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });

    this.Sobras = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
}

function loadSobrasChamadoOcorrencia() {
    $("#container-tab-analise-sobras").appendTo("#tab-analise-sobras");

    _sobrasChamadoOcorrencia = new SobrasChamadoOcorrencia();
    KoBindings(_sobrasChamadoOcorrencia, "knockoutSobrasChamadoOcorrencia");

    loadGridSobrasChamadoOcorrencia();
}

function adicionarSobraChamadoOcorrenciaClick() {
    if (!ValidarCamposObrigatorios(_sobrasChamadoOcorrencia)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }
    let itemGrid = new Object();

    _sobrasChamadoOcorrencia.Codigo.val(guid());
    itemGrid.Codigo = _sobrasChamadoOcorrencia.Codigo.val();
    itemGrid.CodigoSobra = _sobrasChamadoOcorrencia.CodigoSobra.val();
    itemGrid.QuantidadeSobra = _sobrasChamadoOcorrencia.QuantidadeSobra.val();

    _sobrasChamadoOcorrencia.Sobras.list.push(itemGrid);

    limparCamposCadastroSobrasChamadoOcorrencia();
    RecarregarGridSobrasChamadoOcorrencia();
}

function loadGridSobrasChamadoOcorrencia() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: excluirSobraChamadoOcorrenciaClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoSobra", title: "Código", width: "40%", className: "text-align-left" },
        { data: "QuantidadeSobra", title: "Quantidade", width: "25%", className: "text-align-left" }
    ];

    _gridSobrasChamadoOcorrencia = new BasicDataTable(_sobrasChamadoOcorrencia.GridSobra.idGrid, header, null, { column: 1, dir: orderDir.asc });
    _gridSobrasChamadoOcorrencia.CarregarGrid([]);
}

function excluirSobraChamadoOcorrenciaClick(data) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o registro?", function () {
        for (var i = 0; i < _sobrasChamadoOcorrencia.Sobras.list.length; i++) {
            if (data.Codigo == _sobrasChamadoOcorrencia.Sobras.list[i].Codigo.val) {
                _sobrasChamadoOcorrencia.Sobras.list.splice(i, 1);
                break;
            }
        }

        RecarregarGridSobrasChamadoOcorrencia();
    });
}

function exibirCamposSobraChamadoOcorrencia(e) {
    if (e.MostrarCamposSobra.visibleFade()) {
        e.MostrarCamposSobra.icon("fal fa-plus");
    } else {
        e.MostrarCamposSobra.icon("fal fa-minus");
    }
    e.MostrarCamposSobra.visibleFade(!e.MostrarCamposSobra.visibleFade());
}


function RecarregarGridSobrasChamadoOcorrencia() {
    var data = new Array();

    $.each(_sobrasChamadoOcorrencia.Sobras.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo;
        itemGrid.CodigoSobra = item.CodigoSobra;
        itemGrid.QuantidadeSobra = item.QuantidadeSobra;

        data.push(itemGrid);
    });

    _gridSobrasChamadoOcorrencia.CarregarGrid(data, !_sobrasChamadoOcorrencia.Codigo.val() > 0);
}

function limparCamposSobrasChamadoOcorrencia() {
    limparCamposCadastroSobrasChamadoOcorrencia();
    _buscouPorCodigo = false;
    _sobrasChamadoOcorrencia.PermiteSobras.val(false);
    _sobrasChamadoOcorrencia.MostrarCamposSobra.visibleFade(false);
    _gridSobrasChamadoOcorrencia.CarregarGrid([]);
}

function limparCamposCadastroSobrasChamadoOcorrencia() {
    _sobrasChamadoOcorrencia.Codigo.val("");
    _sobrasChamadoOcorrencia.CodigoSobra.val("");
    _sobrasChamadoOcorrencia.QuantidadeSobra.val("");
}

function ObterSobrasPorCodigoOcorrencia() {
    executarReST("ChamadoOcorrencia/ObterSobrasAnalise", { Codigo: _chamado.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                _sobrasChamadoOcorrencia.PermiteSobras.val(arg.Data.PermiteSobras);
                _sobrasChamadoOcorrencia.Sobras.list = arg.Data.Sobras;
                RecarregarGridSobrasChamadoOcorrencia();
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    });
}