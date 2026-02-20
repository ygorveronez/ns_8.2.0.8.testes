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
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOcorrenciaLote.js" />
/// <reference path="../../Enumeradores/EnumTipoRateioOcorrenciaLote.js" />
/// <reference path="Ocorrencia.js" />
/// <reference path="Etapa.js" />
/// <reference path="Carga.js" />
/// <reference path="OcorrenciaLoteSignalR.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridOcorrenciaLote;
var _pesquisaOcorrenciaLote;
var _ocorrenciaLote;
var _CRUDOcorrenciaLote;

var PesquisaOcorrenciaLote = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoOcorrenciaLote.Todos), options: EnumSituacaoOcorrenciaLote.obterOpcoesPesquisa(), def: EnumSituacaoOcorrenciaLote.Todos });

    this.TipoOcorrencia = PropertyEntity({ text: "Tipo de Ocorrência:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridOcorrenciaLote.CarregarGrid();
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

var OcorrenciaLote = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoOcorrenciaLote.EmGeracao), options: EnumSituacaoOcorrenciaLote.obterOpcoes(), def: EnumSituacaoOcorrenciaLote.EmGeracao });

    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.TipoRateio = PropertyEntity({ text: "*Tipo do Rateio: ", val: ko.observable(EnumTipoRateioOcorrenciaLote.Peso), options: EnumTipoRateioOcorrenciaLote.obterOpcoes(), def: EnumTipoRateioOcorrenciaLote.Peso, enable: ko.observable(true) });
    this.ValorFreteLiquido = PropertyEntity({ getType: typesKnockout.decimal, text: "*Valor Frete Líquido:", val: ko.observable(""), def: "", required: true, enable: ko.observable(true) });
    this.TipoOcorrencia = PropertyEntity({ text: "*Tipo de Ocorrência:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true, enable: ko.observable(true) });

    this.FiltrosCargas = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.ListaCargas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

var CRUDOcorrenciaLote = function () {
    this.Gerar = PropertyEntity({ eventClick: GerarClick, type: types.event, text: "Gerar Ocorrências", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: LimparCamposClick, type: types.event, text: "Limpar Campos / Novo", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadOcorrenciaLote() {
    _ocorrenciaLote = new OcorrenciaLote();
    KoBindings(_ocorrenciaLote, "knockoutCadastroOcorrenciaLote");

    HeaderAuditoria("OcorrenciaLote", _ocorrenciaLote);

    _CRUDOcorrenciaLote = new CRUDOcorrenciaLote();
    KoBindings(_CRUDOcorrenciaLote, "knockoutCRUDOcorrenciaLote");

    _pesquisaOcorrenciaLote = new PesquisaOcorrenciaLote();
    KoBindings(_pesquisaOcorrenciaLote, "knockoutPesquisaOcorrenciaLote", false, _pesquisaOcorrenciaLote.Pesquisar.id);

    new BuscarTipoOcorrencia(_pesquisaOcorrenciaLote.TipoOcorrencia);
    new BuscarTipoOcorrencia(_ocorrenciaLote.TipoOcorrencia);

    LoadEtapaOcorrenciaLote();
    LoadOcorrenciaLoteCarga();
    LoadOcorrenciaLoteOcorrencia();
    LoadConexaoSignalROcorrenciaLote();

    BuscarOcorrenciasLote();
}

function GerarClick() {
    PreencheCargas();

    Salvar(_ocorrenciaLote, "OcorrenciaLote/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso, aguarde a geração das ocorrências!");
                _gridOcorrenciaLote.CarregarGrid();
                LimparCamposOcorrenciaLote();

                preencherOcorrenciaLoteRetorno(retorno);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function LimparCamposClick() {
    LimparCamposOcorrenciaLote();
}

////*******MÉTODOS*******

function BuscarOcorrenciasLote() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarOcorrenciaLote, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridOcorrenciaLote = new GridView(_pesquisaOcorrenciaLote.Pesquisar.idGrid, "OcorrenciaLote/Pesquisa", _pesquisaOcorrenciaLote, menuOpcoes);
    _gridOcorrenciaLote.CarregarGrid();
}

function editarOcorrenciaLote(ocorrenciaLoteGrid) {
    buscarOcorrenciaLote(ocorrenciaLoteGrid.Codigo);
}

function buscarOcorrenciaLote(codigo) {
    LimparCamposOcorrenciaLote();
    _ocorrenciaLote.Codigo.val(codigo);
    BuscarPorCodigo(_ocorrenciaLote, "OcorrenciaLote/BuscarPorCodigo", function (retorno) {
        _pesquisaOcorrenciaLote.ExibirFiltros.visibleFade(false);

        controleCamposOcorrenciaLote(retorno.Data);
    });
}

function preencherOcorrenciaLoteRetorno(retorno) {
    PreencherObjetoKnout(_ocorrenciaLote, retorno);
    controleCamposOcorrenciaLote(retorno.Data);
}

function controleCamposOcorrenciaLote(retorno) {
    _ocorrenciaLoteCarga.Codigo.val(_ocorrenciaLote.Codigo.val());
    _ocorrenciaLoteOcorrencia.MotivoRejeicao.val(retorno.MotivoRejeicao);

    ControleCamposOcorrenciaLoteOcorrencia();

    _CRUDOcorrenciaLote.Gerar.visible(false);

    SetarEnableCamposKnockout(_ocorrenciaLote, false);
    SetarEnableCamposKnockout(_ocorrenciaLoteCarga, false);

    PesquisarCargasClick(SetarEtapaOcorrenciaLote);
}

function LimparCamposOcorrenciaLote() {
    LimparCampos(_ocorrenciaLote);
    LimparCamposOcorrenciaLoteCarga();
    LimparCamposOcorrenciaLoteOcorrencia();

    _CRUDOcorrenciaLote.Gerar.visible(true);
    SetarEnableCamposKnockout(_ocorrenciaLote, true);

    SetarEtapaInicioOcorrenciaLote();
}