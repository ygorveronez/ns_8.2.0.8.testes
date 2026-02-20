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


//*******MAPEAMENTO KNOUCKOUT*******

var _aliquotaICMS;
var _pesquisaAliquotaICMS;
var _gridAliquotaICMS;

var AliquotaICMS = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.EstadoEmpresa = PropertyEntity({ text: "*UF Empresa:", required: false, getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true) });
    this.EstadoOrigem = PropertyEntity({ text: "*UF Origem:", required: false, getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true) });
    this.EstadoDestino = PropertyEntity({ text: "*UF Destino:", required: false, getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true) });
    this.AtividadeTomador = PropertyEntity({ text: "*Atividade Tomador:", required: false, getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true) });
    this.AtividadeDestinatario = PropertyEntity({ text: "Atividade Desinatário:", required: false, getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true) });
    this.CFOP = PropertyEntity({ text: "*CFOP:", required: false, getType: typesKnockout.int, val: ko.observable(""), enable: ko.observable(true) });
    this.Percentual = PropertyEntity({ text: "Alíquota:", required: false, getType: typesKnockout.decimal, val: ko.observable(""), enable: ko.observable(true) });
    this.CST = PropertyEntity({ text: "*CST:", required: false, getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", required: false, getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true) });
    this.AliquotaFCP = PropertyEntity({ text: "Alíquota FCP:", required: false, getType: typesKnockout.decimal, val: ko.observable(""), enable: ko.observable(true) });

    // CRUD    
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaAliquotaICMS = function () {
    this.EstadoEmpresa = PropertyEntity({ text: "UF Empresa:", required: false, getType: typesKnockout.string, val: ko.observable("") });
    this.EstadoOrigem = PropertyEntity({ text: "UF Origem:", required: false, getType: typesKnockout.string, val: ko.observable("") });
    this.EstadoDestino = PropertyEntity({ text: "UF Destino:", required: false, getType: typesKnockout.string, val: ko.observable("") });
    this.CFOP = PropertyEntity({ text: "CFOP:", getType: typesKnockout.int, val: ko.observable(""), enable: ko.observable(true) });
    this.AtividadeTomador = PropertyEntity({ text: "Atividade Tomador:", getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true) });


    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAliquotaICMS.CarregarGrid();
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
}


//*******EVENTOS*******
function loadAliquotaICMS() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaAliquotaICMS = new PesquisaAliquotaICMS();
    KoBindings(_pesquisaAliquotaICMS, "knockoutPesquisaAliquotaICMS", false, _pesquisaAliquotaICMS.Pesquisar.id);

    // Instancia objeto principal
    _aliquotaICMS = new AliquotaICMS();
    KoBindings(_aliquotaICMS, "knockoutAliquotaICMS");

    HeaderAuditoria("Aliquota", _aliquotaICMS);

    // Inicia busca
    buscarAliquotaICMS();
    desabilitarCampos();
}

function atualizarClick(e, sender) {
    Salvar(_aliquotaICMS, "AliquotaICMS/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridAliquotaICMS.CarregarGrid();
                limparCamposAliquotaICMS();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}


function cancelarClick(e) {
    limparCamposAliquotaICMS();
}

function editarAliquotaICMSClick(itemGrid) {
    // Limpa os campos
    limparCamposAliquotaICMS();

    // Seta o codigo do objeto
    _aliquotaICMS.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_aliquotaICMS, "AliquotaICMS/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaAliquotaICMS.ExibirFiltros.visibleFade(false);
                desabilitarCampos();

                // Alternas os campos de CRUD
                _aliquotaICMS.Atualizar.visible(true);
                _aliquotaICMS.Cancelar.visible(true);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarAliquotaICMS() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarAliquotaICMSClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridAliquotaICMS = new GridView(_pesquisaAliquotaICMS.Pesquisar.idGrid, "AliquotaICMS/Pesquisa", _pesquisaAliquotaICMS, menuOpcoes, null);
    _gridAliquotaICMS.CarregarGrid();
}

function limparCamposAliquotaICMS() {
    _aliquotaICMS.Atualizar.visible(false);
    _aliquotaICMS.Cancelar.visible(false);
    LimparCampos(_aliquotaICMS);
    desabilitarCampos();
}

function desabilitarCampos() {
    _aliquotaICMS.EstadoEmpresa.enable(false);
    _aliquotaICMS.EstadoOrigem.enable(false);
    _aliquotaICMS.EstadoDestino.enable(false);
    _aliquotaICMS.AtividadeTomador.enable(false);
    _aliquotaICMS.AtividadeDestinatario.enable(false);
    _aliquotaICMS.CFOP.enable(false);
    _aliquotaICMS.Percentual.enable(true);    
    _aliquotaICMS.Observacao.enable(false);    
    if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        _aliquotaICMS.CST.enable(false);
        _aliquotaICMS.AliquotaFCP.enable(false);
    }
}