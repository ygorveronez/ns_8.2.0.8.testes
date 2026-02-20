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
/// <reference path="../../Enumeradores/EnumSituacaoVigencia.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridAliquotaISS;
var _aliquotaISS;
var _pesquisaAliquotaISS;

var PesquisaAliquotaISS = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });
    this.Localidade = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: "Municipio:", idBtnSearch: guid() });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Vigencia = PropertyEntity({ val: ko.observable(EnumSituacaoVigencia.Todos), options: EnumSituacaoVigencia.obterOpcoesPesquisa(), def: EnumSituacaoVigencia.Todos, text: "Vigência:" });


    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAliquotaISS.CarregarGrid();
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

var AliquotaISS = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true) });
    this.Localidade = PropertyEntity({ val: ko.observable(""), codEntity: ko.observable(0), text: "*Municipio:", idBtnSearch: guid(), type: types.entity });
    this.Aliquota = PropertyEntity({ text: "*Alíquota:", required: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 5, scale: 2, allowZero: false }, val: ko.observable("0,00"), def: ko.observable("0,00"), maxlength: 7 });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.RetemISS = PropertyEntity({ text: "Retém ISS ", val: ko.observable(false), def: false });

    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
};

var CRUDAliquotaISS = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarAliquotaISSClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarAliquotaISSClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirAliquotaISSClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAliquotaISSClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "AliquotaISS/Importar",
        UrlConfiguracao: "AliquotaISS/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O057_ISSAliquotaISS,
        ParametrosRequisicao: function () {
            return {
                Inserir: true,
                Atualizar: true
            };
        }
    });
};

//*******EVENTOS*******

function loadAliquotaISS() {
    _pesquisaAliquotaISS = new PesquisaAliquotaISS();
    KoBindings(_pesquisaAliquotaISS, "knockoutPesquisaAliquotaISS", false, _pesquisaAliquotaISS.Pesquisar.id);

    _aliquotaISS = new AliquotaISS();
    KoBindings(_aliquotaISS, "knockoutCadastroAliquotaISS");

    _crudAliquotaISS = new CRUDAliquotaISS();
    KoBindings(_crudAliquotaISS, "knockoutCRUDAliquotaISS");

    new BuscarLocalidades(_pesquisaAliquotaISS.Localidade);
    new BuscarLocalidades(_aliquotaISS.Localidade);

    HeaderAuditoria("AliquotaISS", _aliquotaISS);

    buscarAliquotaISS();
}

function adicionarAliquotaISSClick(e, sender) {
    ValidarCamposObrigatorios(_aliquotaISS);

    Salvar(_aliquotaISS, "AliquotaISS/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridAliquotaISS.CarregarGrid();
                limparCamposAliquotaISS();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarAliquotaISSClick(e, sender) {
    ValidarCamposObrigatorios(_aliquotaISS);

    Salvar(_aliquotaISS, "AliquotaISS/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridAliquotaISS.CarregarGrid();
                limparCamposAliquotaISS();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirAliquotaISSClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Alíquota de ISS: " + _aliquotaISS.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_aliquotaISS, "AliquotaISS/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridAliquotaISS.CarregarGrid();
                    limparCamposAliquotaISS();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarAliquotaISSClick(e) {
    limparCamposAliquotaISS();
}

//*******MÉTODOS*******

function buscarAliquotaISS() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarAliquotaISS, tamanho: "15", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    var configExportacao = {
        url: "AliquotaISS/ExportarPesquisa",
        titulo: "Alíquota ISS"
    }

    _gridAliquotaISS = new GridView(_pesquisaAliquotaISS.Pesquisar.idGrid, "AliquotaISS/Pesquisa", _pesquisaAliquotaISS, menuOpcoes, null, null, null, null, null, null, null, null, configExportacao);
    _gridAliquotaISS.CarregarGrid();
}

function editarAliquotaISS(aliquotaISSGrid) {
    limparCamposAliquotaISS();
    _aliquotaISS.Codigo.val(aliquotaISSGrid.Codigo);

    BuscarPorCodigo(_aliquotaISS, "AliquotaISS/BuscarPorCodigo", function (arg) {
        _pesquisaAliquotaISS.ExibirFiltros.visibleFade(false);
        _crudAliquotaISS.Atualizar.visible(true);
        _crudAliquotaISS.Cancelar.visible(true);
        _crudAliquotaISS.Excluir.visible(true);
        _crudAliquotaISS.Adicionar.visible(false);
    }, null);
}

function limparCamposAliquotaISS() {
    _crudAliquotaISS.Atualizar.visible(false);
    _crudAliquotaISS.Cancelar.visible(false);
    _crudAliquotaISS.Excluir.visible(false);
    _crudAliquotaISS.Adicionar.visible(true);
    LimparCampos(_aliquotaISS);
}