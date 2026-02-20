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
/// <reference path="../../Consultas/SegmentoVeiculo.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/Filial.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTabelaDiaria;
var _tabelaDiaria;
var _GRUDTabelaDiaria;
var _pesquisaTabelaDiaria;

var PesquisaTabelaDiaria = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTabelaDiaria.CarregarGrid();
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

var TabelaDiaria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });

    this.DataVigenciaInicial = PropertyEntity({ text: "Data Vigência Inicial: ", getType: typesKnockout.date });
    this.DataVigenciaFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Modelo do Veículo:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.SegmentoVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Segmento do Veículo:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable((_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Centro de Resultado:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable((_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)) });
    this.GerarMovimentoSaidaFixaMotorista = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Gerar movimento de saída na fixa do motorista?", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable((_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)) });

    //Campo Embarcador
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Filial:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable((_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador)) });

    this.DadosPeriodos = PropertyEntity({ ftype: types.listEntity, getType: typesKnockout.listEntity, def: [], val: ko.observableArray([]), codEntity: ko.observable(0) });
    this.Periodos = PropertyEntity({ type: types.listEntity, getType: typesKnockout.listEntity, def: [], val: ko.observableArray([]), codEntity: ko.observable(0) });
};

var CRUDTabelaDiaria = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadTabelaDiaria() {

    _tabelaDiaria = new TabelaDiaria();
    KoBindings(_tabelaDiaria, "knockoutCadastroTabelaDiaria");

    _GRUDTabelaDiaria = new CRUDTabelaDiaria();
    KoBindings(_GRUDTabelaDiaria, "knockoutCRUDTabelaDiaria");

    _pesquisaTabelaDiaria = new PesquisaTabelaDiaria();
    KoBindings(_pesquisaTabelaDiaria, "knockoutPesquisaTabelaDiaria", false, _pesquisaTabelaDiaria.Pesquisar.id);

    new BuscarSegmentoVeiculo(_tabelaDiaria.SegmentoVeiculo);
    new BuscarModelosVeicularesCarga(_tabelaDiaria.ModeloVeicularCarga);
    new BuscarCentroResultado(_tabelaDiaria.CentroResultado);
    new BuscarFilial(_tabelaDiaria.Filial);

    HeaderAuditoria("TabelaDiaria", _tabelaDiaria);

    buscarTabelaDiarias();
    LoadPeriodo();
    limparCamposTabelaDiaria();
}

function adicionarClick(e, sender) {
    Salvar(_tabelaDiaria, "TabelaDiaria/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTabelaDiaria.CarregarGrid();
                limparCamposTabelaDiaria();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tabelaDiaria, "TabelaDiaria/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTabelaDiaria.CarregarGrid();
                limparCamposTabelaDiaria();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a tabela de diária " + _tabelaDiaria.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tabelaDiaria, "TabelaDiaria/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTabelaDiaria.CarregarGrid();
                    limparCamposTabelaDiaria();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTabelaDiaria();
}

//*******MÉTODOS*******

function buscarTabelaDiarias() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTabelaDiaria, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTabelaDiaria = new GridView(_pesquisaTabelaDiaria.Pesquisar.idGrid, "TabelaDiaria/Pesquisa", _pesquisaTabelaDiaria, menuOpcoes, null);
    _gridTabelaDiaria.CarregarGrid();
}

function editarTabelaDiaria(tabelaDiariaGrid) {
    limparCamposTabelaDiaria();
    _tabelaDiaria.Codigo.val(tabelaDiariaGrid.Codigo);
    BuscarPorCodigo(_tabelaDiaria, "TabelaDiaria/BuscarPorCodigo", function (arg) {
        _pesquisaTabelaDiaria.ExibirFiltros.visibleFade(false);

        RenderizaGridPeriodo(_periodo);

        _GRUDTabelaDiaria.Atualizar.visible(true);
        _GRUDTabelaDiaria.Cancelar.visible(true);
        _GRUDTabelaDiaria.Excluir.visible(true);
        _GRUDTabelaDiaria.Adicionar.visible(false);
    }, null);
}

function limparCamposTabelaDiaria() {
    _GRUDTabelaDiaria.Atualizar.visible(false);
    _GRUDTabelaDiaria.Cancelar.visible(false);
    _GRUDTabelaDiaria.Excluir.visible(false);
    _GRUDTabelaDiaria.Adicionar.visible(true);
    LimparCampos(_tabelaDiaria);
    LimparCamposPeriodo();

    $("#tabDados").click();

    _tabelaDiaria.Periodos.list = new Array();
}
