/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Enumeradores/EnumAnaliticoSintetico.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridCentroResultado, _centroResultado, _pesquisaCentroResultado, _crudCentroResultado;

var _AnaliticoSintetico = [
    { text: "Analitico", value: EnumAnaliticoSintetico.Analitico },
    { text: "Sintético", value: EnumAnaliticoSintetico.Sintetico }
];

var PesquisaCentroResultado = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Plano = PropertyEntity({ text: "Número do centro: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCentroResultado.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var CentroResultado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });
    this.Plano = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), validaEscritaBusca: false, text: "*Número do centro:", idBtnSearch: guid(), required: true, val: ko.observable("") });
    this.CentroResultadoSintetico = PropertyEntity({ text: "Centro de resultado sintético: ", required: false, enable: false, issue: 362 });
    this.PlanoContabilidade = PropertyEntity({ text: "Código integração contábil: ", required: false, issue: 358, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-6 col-lg-6") });
    this.CodigoCompanhia = PropertyEntity({ text: "Código da Companhia: ", required: false, visible: ko.observable(false) });

    this.AnaliticoSintetico = PropertyEntity({ val: ko.observable(EnumAnaliticoSintetico.Analitico), options: _AnaliticoSintetico, text: "*Tipo do centro: ", def: EnumAnaliticoSintetico.Analitico });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });

    this.Veiculos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TiposOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

var CRUDCentroResultado = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadCentroResultado() {

    _centroResultado = new CentroResultado();
    KoBindings(_centroResultado, "knockoutCadastroCentroResultado");

    _crudCentroResultado = new CRUDCentroResultado();
    KoBindings(_crudCentroResultado, "knockoutCRUDCentroResultado");

    HeaderAuditoria("CentroResultado", _centroResultado);

    _pesquisaCentroResultado = new PesquisaCentroResultado();
    KoBindings(_pesquisaCentroResultado, "knockoutPesquisaCentroResultado", false, _pesquisaCentroResultado.Pesquisar.id);

    new BuscarCentroResultado(_centroResultado.Plano, "Selecione a Conta Sintética", "Contas Sintéticas", RetornoSelecaoContaSintetica, EnumAnaliticoSintetico.Sintetico);

    LoadVeiculo();
    LoadTipoOperacao();

    buscarCentroResultados();
}

function RetornoSelecaoContaSintetica(data) {
    if (data != null) {
        executarReST("CentroResultado/ProximaNumeracao", { Codigo: data.Codigo, Plano: data.Plano }, function (e) {
            if (e.Success) {
                _centroResultado.Plano.val(e.Data.Plano);
                _centroResultado.Plano.codEntity(e.Data.Plano);
                _centroResultado.CentroResultadoSintetico.val(e.Data.CentroResultadoSintetico);
            } else
                exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
        });
    }
}

function adicionarClick(e, sender) {
    PreencherListasSelecao();
    _centroResultado.Plano.codEntity(_centroResultado.Plano.val());
    
    Salvar(_centroResultado, "CentroResultado/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridCentroResultado.CarregarGrid();
                limparCamposCentroResultado();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    PreencherListasSelecao();
    _centroResultado.Plano.codEntity(_centroResultado.Plano.val());

    Salvar(_centroResultado, "CentroResultado/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridCentroResultado.CarregarGrid();
                limparCamposCentroResultado();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o centro de resultado " + _centroResultado.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_centroResultado, "CentroResultado/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridCentroResultado.CarregarGrid();
                    limparCamposCentroResultado();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposCentroResultado();
}

function ConsultaPlanoClick(e, sender) {

}

//*******MÉTODOS*******

function PreencherListasSelecao() {
    var veiculosSelecao = new Array();
    var tiposOperacaoSelecao = new Array();

    $.each(_veiculo.Veiculo.basicTable.BuscarRegistros(), function (i, veiculo) {
        veiculosSelecao.push(veiculo.Codigo);
    });

    $.each(_tipoOperacao.TipoOperacao.basicTable.BuscarRegistros(), function (i, tipoOperacao) {
        tiposOperacaoSelecao.push(tipoOperacao.Codigo);
    });

    _centroResultado.Veiculos.val(JSON.stringify(veiculosSelecao));
    _centroResultado.TiposOperacao.val(JSON.stringify(tiposOperacaoSelecao));
}

function buscarCentroResultados() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarCentroResultado, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "CentroResultado/ExportarPesquisa",
        titulo: "Centro de Resultados"
    };    
    _gridCentroResultado = new GridViewExportacao(_pesquisaCentroResultado.Pesquisar.idGrid, "CentroResultado/Pesquisa", _pesquisaCentroResultado, menuOpcoes, configExportacao, { column: 2, dir: orderDir.asc } );
    _gridCentroResultado.CarregarGrid();
}

function editarCentroResultado(centroResultadoGrid) {
    limparCamposCentroResultado();
    _centroResultado.Codigo.val(centroResultadoGrid.Codigo);
    BuscarPorCodigo(_centroResultado, "CentroResultado/BuscarPorCodigo", function (arg) {
        _pesquisaCentroResultado.ExibirFiltros.visibleFade(false);
        _crudCentroResultado.Atualizar.visible(true);
        _crudCentroResultado.Cancelar.visible(true);
        _crudCentroResultado.Excluir.visible(true);
        _crudCentroResultado.Adicionar.visible(false);

        RecarregarGridVeiculo();
        RecarregarGridTipoOperacao();
    }, null);
}

function limparCamposCentroResultado() {
    _crudCentroResultado.Atualizar.visible(false);
    _crudCentroResultado.Cancelar.visible(false);
    _crudCentroResultado.Excluir.visible(false);
    _crudCentroResultado.Adicionar.visible(true);
    LimparCampos(_centroResultado);
    LimparCamposVeiculo();
    LimparCamposTipoOperacao();
    RecarregarGridVeiculo();
    RecarregarGridTipoOperacao();

    Global.ResetarAbas();
}
