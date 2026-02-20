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
/// <reference path="../../../ViewsScripts/Consultas/Estado.js" />
/// <reference path="../../../ViewsScripts/Consultas/TipoCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pautaFiscal;
var _crudPautaFiscal;
var _pesquisaPautaFiscal;
var _gridPautaFiscal;
var _gridTipoCarga;
var _tiposCarga = new Array();

var PautaFiscal = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Estado = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: "*Estado:", idBtnSearch: guid(), required: true });
    this.Tarifa = PropertyEntity({ text: "*Tarifa/Descrição: ", required: true });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });

    this.DistanciaKMInicial = PropertyEntity({ text: "Distância KM inicial: ", getType: typesKnockout.int });
    this.DistanciaKMFinal = PropertyEntity({ text: "Distância KM final: ", getType: typesKnockout.int });
    this.ValorTonelada = PropertyEntity({ text: "Valor tonelada: ", getType: typesKnockout.decimal, enable: ko.observable(true), configDecimal: { precision: 4, allowZero: true } });
    this.ValorViagem = PropertyEntity({ text: "Valor viagem: ", getType: typesKnockout.decimal, enable: ko.observable(true), configDecimal: { precision: 4, allowZero: true } });
    this.ValorM3 = PropertyEntity({ text: "Valor M3: ", getType: typesKnockout.decimal, enable: ko.observable(true), configDecimal: { precision: 4, allowZero: true } });
    this.ValorVolumeMST = PropertyEntity({ text: "Valor volume MST: ", getType: typesKnockout.decimal, enable: ko.observable(true), configDecimal: { precision: 4, allowZero: true } });
    this.ValorTonelada.val.subscribe(onChangeValor);
    this.ValorViagem.val.subscribe(onChangeValor);
    this.ValorM3.val.subscribe(onChangeValor);
    this.ValorVolumeMST.val.subscribe(onChangeValor);

    //Lista Tipo Carga
    this.GridTipoCarga = PropertyEntity({ type: types.local });
    this.TiposCarga = PropertyEntity({ type: types.map, getType: typesKnockout.string, val: ko.observable(""), def: "", idGrid: guid() });
    this.AdicionarTipoCarga = PropertyEntity({ type: types.event, text: "Adicionar Tipo de Carga", idBtnSearch: guid() });
};

var CRUDPautaFiscal = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var PesquisaPautaFiscal = function () {
    this.Tarifa = PropertyEntity({ text: "Tarifa: " });
    this.Estado = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: "Estado:", idBtnSearch: guid(), required: true });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPautaFiscal.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadPautaFiscal() {
    _pesquisaPautaFiscal = new PesquisaPautaFiscal();
    KoBindings(_pesquisaPautaFiscal, "knockoutPesquisaPautaFiscal", false, _pesquisaPautaFiscal.Pesquisar.id);

    _pautaFiscal = new PautaFiscal();
    KoBindings(_pautaFiscal, "knockoutPautaFiscal");

    HeaderAuditoria("PautaFiscal", _pautaFiscal);

    _crudPautaFiscal = new CRUDPautaFiscal();
    KoBindings(_crudPautaFiscal, "knockoutCRUDPautaFiscal");

    new BuscarEstados(_pautaFiscal.Estado);
    new BuscarEstados(_pesquisaPautaFiscal.Estado);

    buscarPautaFiscal();

    loadGridTipoCarga();
}

function loadGridTipoCarga() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        tamanho: 7,
        opcoes: [
            { descricao: "Excluir", id: guid(), metodo: ExcluirTipoCargaClick }
        ]
    };

    var header = [{ data: "Codigo", visible: false }, { data: "Descricao", title: "Descrição", width: "80%" }];

    _gridTipoCarga = new BasicDataTable(_pautaFiscal.GridTipoCarga.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposdeCarga(_pautaFiscal.AdicionarTipoCarga, function (r) {
        if (r == null) return;

        for (var i = 0; i < r.length; i++)
            _tiposCarga.push({ Codigo: r[i].Codigo, Descricao: r[i].Descricao });

        setarArrayTipoCarga(_tiposCarga);
    }, null, _gridTipoCarga);

    setarArrayTipoCarga([]);
    _pautaFiscal.AdicionarTipoCarga.basicTable = _gridTipoCarga;
}

function onChangeValor() {
    var campos = ['ValorTonelada', 'ValorViagem', 'ValorM3', 'ValorVolumeMST'];

    for (var i in campos) {
        var prop = campos[i];
        _pautaFiscal[prop].enable(true);
    }

    var valorToneladaInformado = (Globalize.parseFloat(_pautaFiscal.ValorTonelada.val()) || 0);
    var valorViagemInformado = (Globalize.parseFloat(_pautaFiscal.ValorViagem.val()) || 0);
    var valorM3Informado = (Globalize.parseFloat(_pautaFiscal.ValorM3.val()) || 0);
    var valorVolumeMSTInformado = (Globalize.parseFloat(_pautaFiscal.ValorVolumeMST.val()) || 0);

    if (valorToneladaInformado) {
        _pautaFiscal.ValorViagem.enable(false);
        _pautaFiscal.ValorM3.enable(false);
        _pautaFiscal.ValorVolumeMST.enable(false);
    } else if (valorViagemInformado) {
        _pautaFiscal.ValorTonelada.enable(false);
        _pautaFiscal.ValorM3.enable(false);
        _pautaFiscal.ValorVolumeMST.enable(false);
    } else if (valorM3Informado || valorVolumeMSTInformado) {
        _pautaFiscal.ValorTonelada.enable(false);
        _pautaFiscal.ValorViagem.enable(false);
    }
}

function ExcluirTipoCargaClick(data) {
    for (var i = 0; i < _tiposCarga.length; i++) {
        if (_tiposCarga[i].Codigo == data.Codigo) {
            _tiposCarga.splice(i, 1);
            break;
        }
    }

    setarArrayTipoCarga(_tiposCarga);
}

function adicionarClick(e, sender) {
    Salvar(_pautaFiscal, "PautaFiscal/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridPautaFiscal.CarregarGrid();
                limparCamposPautaFiscal();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_pautaFiscal, "PautaFiscal/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridPautaFiscal.CarregarGrid();
                limparCamposPautaFiscal();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_pautaFiscal, "PautaFiscal/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridPautaFiscal.CarregarGrid();
                    limparCamposPautaFiscal();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCamposPautaFiscal();
}

function editarPautaFiscalClick(itemGrid) {
    limparCamposPautaFiscal();

    _pautaFiscal.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_pautaFiscal, "PautaFiscal/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaPautaFiscal.ExibirFiltros.visibleFade(false);

                setarArrayTipoCarga(arg.Data.TiposCarga);

                _crudPautaFiscal.Atualizar.visible(true);
                _crudPautaFiscal.Excluir.visible(false);
                _crudPautaFiscal.Cancelar.visible(true);
                _crudPautaFiscal.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******

function buscarPautaFiscal() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPautaFiscalClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridPautaFiscal = new GridView(_pesquisaPautaFiscal.Pesquisar.idGrid, "PautaFiscal/Pesquisa", _pesquisaPautaFiscal, menuOpcoes, null);
    _gridPautaFiscal.CarregarGrid();
}

function setarArrayTipoCarga(val) {
    _tiposCarga = val;
    _gridTipoCarga.CarregarGrid(_tiposCarga);
    _pautaFiscal.TiposCarga.val(JSON.stringify(_tiposCarga));
}

function limparCamposPautaFiscal() {
    _crudPautaFiscal.Atualizar.visible(false);
    _crudPautaFiscal.Cancelar.visible(false);
    _crudPautaFiscal.Excluir.visible(false);
    _crudPautaFiscal.Adicionar.visible(true);
    LimparCampos(_pautaFiscal);

    setarArrayTipoCarga([]);
}
