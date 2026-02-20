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
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumGrupoNC.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumSubGrupoNC.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumAreaNC.js" />




//*******MAPEAMENTO KNOUCKOUT*******
var _gridTipoOperacao;
var _tipoOperacao;
var _crudItemNaoConformidade;
var _tagsParticipantes = "*Recebedor (<entrega>)  -- Destinatário (<dest>)  -- Remetente (<emit>) -- Expedidor (Não mapeado)"
var _itemNaoConformidade;
var _itemNaoConformidadeRegras;
var _pesquisaItemNaoConformidade;
var _gridItemNaoConformidade;


var ItemNaoConformidade = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.NotaFiscal = PropertyEntity({ text: "Nota Fiscal:", visible: ko.observable(false), val: ko.observable("") });
    this.Grupo = PropertyEntity({ val: ko.observable(EnumGrupoNC.Transporte), options: EnumGrupoNC.obterOpcoes(), def: EnumGrupoNC.Transporte, text: "*Grupo: ", visible: ko.observable(true), enable: ko.observable(true) });
    this.SubGrupo = PropertyEntity({ val: ko.observable(EnumSubGrupoNC.DTOCCancelada), options: EnumSubGrupoNC.obterOpcoes(), def: EnumSubGrupoNC.DTOCCancelada, text: "Sub Grupo: ", visible: ko.observable(true), enable: ko.observable(true) });
    this.Area = PropertyEntity({ val: ko.observable(EnumAreaGrupoNC.Transporte), options: EnumAreaGrupoNC.obterOpcoes(), def: EnumAreaGrupoNC.Transporte, text: "Área: ", visible: ko.observable(true), enable: ko.observable(true) });
    this.IrrelevanteParaNC = PropertyEntity({ text: "Irrelevante para Não Conformidade", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteContingencia = PropertyEntity({ text: "Permite Contigência", getType: typesKnockout.bool, val: ko.observable(false) });
    this.TipoOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação:", val: ko.observable(true), options: Global.ObterOpcoesBooleano("Ativo", "Inativo"), def: true, visible: true });
    this.Participacao = PropertyEntity({ text: "Participantes:", val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoParticipacao.obterOpcoes(), visible: ko.observable(false) });

    this.Filiais = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.CFOP = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.Fornecedor = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });


}
function CRUDItemNaoConformidade() {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

function RegrasItemNacaoConformidade() {
    this.TipoRegra = PropertyEntity({ val: ko.observable(EnumTipoRegraNaoConformidade.ExisteXmlNota), options: EnumTipoRegraNaoConformidade.obterOpcoes(), def: EnumTipoRegraNaoConformidade.ExisteXmlNota, text: "Tipo de regra: ", visible: ko.observable(true), enable: ko.observable(true) });
    this.Participacao = PropertyEntity({ text: "Participantes:", val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoParticipacao.obterOpcoes(), visible: ko.observable(false) });
    this.DescricaoRegra = PropertyEntity({ text: "Descrição da Regra:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable(EnumTipoRegraNaoConformidade.obterOpcoesTexto(EnumTipoRegraNaoConformidade.ExisteXmlNota)) });
    this.TagParticipante = PropertyEntity({ text: "Tags dos participantes da nota:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable(_tagsParticipantes) });
    

    this.TipoRegra.val.subscribe(function (novoValor) {
        alterarTipoRegra(novoValor);
        descricaoRegraClickChange(novoValor);

    });



    this.VisualizarDescricaoRegra = PropertyEntity({ eventClick: visualizarDescricaoRegraClick, type: types.event, text: "Visualizar descrição da regra", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: fecharDescricaoRegraClick, type: types.event, text: "Fechar", visible: ko.observable(true) });

}
var PesquisaItemNaoConformidade = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação", val: ko.observable(true), options: Global.ObterOpcoesPesquisaBooleano("Ativo", "Inativo"), def: true });
    this.Grupo = PropertyEntity({ val: ko.observable(EnumGrupoNC.Todos), options: EnumGrupoNC.obterOpcoesPesquisa(), def: EnumGrupoNC.Todos, text: "Grupo: ", visible: ko.observable(true), enable: ko.observable(true) });
    this.SubGrupo = PropertyEntity({ val: ko.observable(EnumSubGrupoNC.Todos), options: EnumSubGrupoNC.obterOpcoesPesquisa(), def: EnumSubGrupoNC.Todos, text: "Sub Grupo: ", visible: ko.observable(true), enable: ko.observable(true) });
    this.Area = PropertyEntity({ val: ko.observable(EnumAreaGrupoNC.Todos), options: EnumAreaGrupoNC.obterOpcoesPesquisa(), def: EnumAreaGrupoNC.Todos, text: "Área: ", visible: ko.observable(true), enable: ko.observable(true) });
    this.IrrelevanteParaNC = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, text: "Irrelevante para Não Conformidade: ", visible: ko.observable(true), enable: ko.observable(true) });
    this.PermiteContingencia = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, text: "Permite Contigência: ", visible: ko.observable(true), enable: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridItemNaoConformidade.CarregarGrid();
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
function loadItemNaoConformidade() {

    _pesquisaItemNaoConformidade = new PesquisaItemNaoConformidade();
    KoBindings(_pesquisaItemNaoConformidade, "knockoutPesquisaItemNaoConformidade", false, _pesquisaItemNaoConformidade.Pesquisar.id);

    _itemNaoConformidade = new ItemNaoConformidade();
    KoBindings(_itemNaoConformidade, "knockoutItemNaoConformidade");

    _itemNaoConformidadeRegras = new RegrasItemNacaoConformidade();
    KoBindings(_itemNaoConformidadeRegras, "knockoutItemNaoConformidadeRegras");

    _crudItemNaoConformidade = new CRUDItemNaoConformidade();
    KoBindings(_crudItemNaoConformidade, "knockoutCRUDItemNaoConformidade");

    HeaderAuditoria("ItemNaoConformidade", _itemNaoConformidade);

    CarregarGridItemNaoConformidade();
    new BuscarTiposOperacao(_itemNaoConformidade.TipoOperacao);

    loadTabsItemNaoConformidades();
}


function loadGridTipoOperacao() {
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarTipoOperacaoClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoTipoOperacao", visible: false },
        { data: "TipoOperacao", title: Localization.Resources.Logistica.CentroCarregamento.TipoDeOperacao, width: "60%", className: 'text-align-left' },
        { data: "Tipo", visible: false },
        { data: "TipoDescricao", title: Localization.Resources.Gerais.Geral.Tipo, width: "35%" }
    ];

    _gridCentroCarregamentoTipoOperacao = new BasicDataTable(_centroCarregamentoTipoOperacao.Grid.id, header, menuOpcoes);
    _gridCentroCarregamentoTipoOperacao.CarregarGrid([]);
}


function adicionarClick(e, sender) {    
    PreecherValoresTabs();
    Salvar(_itemNaoConformidade, "ItemNaoConformidade/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridItemNaoConformidade.CarregarGrid();
                LimparCamposItemNaoConformidade();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    PreecherValoresTabs();
    Salvar(_itemNaoConformidade, "ItemNaoConformidade/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridItemNaoConformidade.CarregarGrid();
                LimparCamposItemNaoConformidade();
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
        ExcluirPorCodigo(_itemNaoConformidade, "ItemNaoConformidade/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridItemNaoConformidade.CarregarGrid();
                    LimparCamposItemNaoConformidade();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}


function descricaoRegraClickChange(tipoRegra) {
    _itemNaoConformidadeRegras.DescricaoRegra.val(EnumTipoRegraNaoConformidade.obterOpcoesTexto(tipoRegra))
}

function visualizarDescricaoRegraClick() {
        Global.abrirModal('divModalDescricaoRegras')
}

function fecharDescricaoRegraClick() {
    Global.fecharModal('divModalDescricaoRegras')
}

function cancelarClick(e) {
    LimparCamposItemNaoConformidade();
}

function editarItemNaoConformidadeClick(itemGrid) {
    // Limpa os campos
    LimparCamposItemNaoConformidade();

    // Seta o codigo do objeto
    _itemNaoConformidade.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_itemNaoConformidade, "ItemNaoConformidade/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaItemNaoConformidade.ExibirFiltros.visibleFade(false);
                CarregarGridsTabs(arg.Data);
                // Alternas os campos de CRUD
                _crudItemNaoConformidade.Atualizar.visible(true);
                _crudItemNaoConformidade.Excluir.visible(true);
                _crudItemNaoConformidade.Cancelar.visible(true);
                _crudItemNaoConformidade.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}


function visibleTipoParticipacao(p) {    
    _itemNaoConformidadeRegras.Participacao.visible(p);
}

function alterarTipoRegra(val) {
    if (val == EnumTipoRegraNaoConformidade.EstendidoFilial || val == EnumTipoRegraNaoConformidade.Nacionalizacao) {
        visibleTipoParticipacao(true);
        $("#tagsParticipantes").show();
    }
    else if (val == EnumTipoRegraNaoConformidade.RecebedorNotaFiscal || val == EnumTipoRegraNaoConformidade.LocalEntrega || val == EnumTipoRegraNaoConformidade.RecebedorArmazenagem
        || val == EnumTipoRegraNaoConformidade.CapacidadeExcedida) {
        visibleTipoParticipacao(false);
        $("#tagsParticipantes").hide();
    }
    else if (val == EnumTipoRegraNaoConformidade.ValidarRaiz || val == EnumTipoRegraNaoConformidade.ValidarCNPJ || val == EnumTipoRegraNaoConformidade.SituacaoCadastral
        || val == EnumTipoRegraNaoConformidade.Nacionalizacao || val == EnumTipoRegraNaoConformidade.EstendidoFilial) {
        visibleTipoParticipacao(true);
        $("#tagsParticipantes").show();
    }
    else {
        visibleTipoParticipacao(false);
        $("#tagsParticipantes").hide();
    }
}


//*******MÉTODOS*******
function CarregarGridItemNaoConformidade() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarItemNaoConformidadeClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridItemNaoConformidade = new GridView(_pesquisaItemNaoConformidade.Pesquisar.idGrid, "ItemNaoConformidade/Pesquisa", _pesquisaItemNaoConformidade, menuOpcoes, null);
    _gridItemNaoConformidade.CarregarGrid();
}

function LimparCamposItemNaoConformidade() {
    _crudItemNaoConformidade.Atualizar.visible(false);
    _crudItemNaoConformidade.Cancelar.visible(false);
    _crudItemNaoConformidade.Excluir.visible(false);
    _crudItemNaoConformidade.Adicionar.visible(true);
    LimparCampos(_itemNaoConformidade);
    limparGridsTabs();
}