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
/// <reference path="../../../ViewsScripts/Enumeradores/EnumRelacionamentoCarga.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../ViewsScripts/Consultas/TipoDeCarga.js" />
/// <reference path="../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../../ViewsScripts/Consultas/CanalEntrega.js" />
/// <reference path="../../../ViewsScripts/Consultas/Tranportador.js" />


var _cargaRelacionada;
var _gridCargaRelacionada;

var CargaRelacionada = function () {
    this.NumeroCarga = PropertyEntity({ text: "Número da Carga: ", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500 });

    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacoesCarga.Todos), options: EnumSituacoesCarga.obterOpcoesPesquisa(), def: EnumSituacoesCarga.Todos });

    this.Filial = PropertyEntity({ text: "Filial:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoDeCarga = PropertyEntity({ text: "Tipo de Carga:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ text: "Tipo de Operação:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.CanalEntrega = PropertyEntity({ text: "Canal de Entrega:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ text: "Transportador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) });

    this.Relacionada = PropertyEntity({ text: "Relacionada: ", val: ko.observable(EnumRelacionamentoCarga.Todos), options: EnumRelacionamentoCarga.obterOpcoesPesquisa(), def: EnumRelacionamentoCarga.Todos });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCargaRelacionada.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
}

function loadCargaRelacionada() {
    _cargaRelacionada = new CargaRelacionada();
    KoBindings(_cargaRelacionada, "knockoutCargaRelacionada");

    HeaderAuditoria("CargaRelacionada", _cargaRelacionada);

    new BuscarFilial(_cargaRelacionada.Filial);
    new BuscarTiposdeCarga(_cargaRelacionada.TipoDeCarga);
    new BuscarTiposOperacao(_cargaRelacionada.TipoOperacao);
    new BuscarCanaisEntrega(_cargaRelacionada.CanalEntrega);
    new BuscarTransportadores(_cargaRelacionada.Transportador);

    loadJustificativa();
    loadCargaRelacionar();
    BuscarCargaRelacionada();
}

function BuscarCargaRelacionada() {
    var relacionar = { descricao: "Relacionamento da Carga", id: guid(), metodo: relacionarClick, icone: "" };
    var justificar = { descricao: "Justificar", id: guid(), metodo: justificarClick, icone: "" };
    var auditar = { descricao: "Auditar", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("Carga"), tamanho: "10", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };

    var configExportacao = {
        url: "CargaRelacionada/ExportarPesquisa",
        titulo: "Carga Relacionada",
    }

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 9, opcoes: [relacionar, justificar, auditar] };

    _gridCargaRelacionada = new GridView(_cargaRelacionada.Pesquisar.idGrid, "CargaRelacionada/Pesquisa", _cargaRelacionada, menuOpcoes, null, null, null, null, null, null, null, null, configExportacao);
    _gridCargaRelacionada.CarregarGrid();
}

function salvarRelacionamentoClick(e, sender) {
    Salvar(_cargaRelacionada, "CargaRelacionada/Relacionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridCargaRelacionada.CarregarGrid();
                limparCamposCargaRelacionada();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposCargaRelacionada();
}

function limparCamposCargaRelacionada() {
    LimparCampos(_cargaRelacionada);
}

function relacionarClick(e) {
    _relacionarCarga.Codigo.val(e.Codigo);
    buscarCargaRelacionada(e.Codigo);
    Global.abrirModal("divModalRelacionarCarga");
}

function justificarClick(e) {
    _justificativa.Codigo.val(e.Codigo);
    buscarJustificativa(e.Codigo);
    Global.abrirModal("divModalJustificar");
}
