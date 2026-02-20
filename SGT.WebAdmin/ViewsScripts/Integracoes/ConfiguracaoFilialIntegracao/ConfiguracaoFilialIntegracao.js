/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaConfiguracaoFilialIntegracao;
var _configuracaoFilialIntegracao;
var _gridConfiguracaoFilialIntegracao;
var _CRUDConfiguracaoFilialIntegracao;

var PesquisaConfiguracaoFilialIntegracao = function () {
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação: ", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _statusPesquisa, def: 0, text: "Situação: " });    
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: function (e) { _gridConfiguracaoFilialIntegracao.CarregarGrid(); }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.ExibirFiltros = PropertyEntity({ eventClick: function (e) { e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade()); }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
};

var ConfiguracaoFilialIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação: ", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TiposIntegracao = PropertyEntity({ val: ko.observable(new Array()) });
};

var CRUDConfiguracaoFilialIntegracao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadConfiguracaoFilialIntegracao() {
    _configuracaoFilialIntegracao = new ConfiguracaoFilialIntegracao();
    KoBindings(_configuracaoFilialIntegracao, "knockoutConfiguracaoFilialIntegracao");

    _CRUDConfiguracaoFilialIntegracao = new CRUDConfiguracaoFilialIntegracao();
    KoBindings(_CRUDConfiguracaoFilialIntegracao, "knoutCRUDConfiguracaoFilialIntegracao");

    _pesquisaConfiguracaoFilialIntegracao = new PesquisaConfiguracaoFilialIntegracao();
    KoBindings(_pesquisaConfiguracaoFilialIntegracao, "knockoutPesquisaConfiguracaoFilialIntegracao", _pesquisaConfiguracaoFilialIntegracao.Pesquisar.id);

    HeaderAuditoria("ConfiguracaoFilialIntegracao", _configuracaoFilialIntegracao);
        
    new BuscarTiposOperacao(_configuracaoFilialIntegracao.TipoOperacao);
    new BuscarFilial(_configuracaoFilialIntegracao.Filial);
    new BuscarTiposOperacao(_pesquisaConfiguracaoFilialIntegracao.TipoOperacao);    

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _configuracaoFilialIntegracao.Filial.visible(false);             
    }

    BuscarConfiguracaoFilialIntegracao();
    loadGridFilialIntegracao();    
}


//*******MÉTODOS*******

function editarConfiguracaoFilialIntegracao(itemGrid) {
    limparCamposConfigFilialInt();    
    _configuracaoFilialIntegracao.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_configuracaoFilialIntegracao, "ConfiguracaoFilialIntegracao/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaConfiguracaoFilialIntegracao.ExibirFiltros.visibleFade(false);

                _CRUDConfiguracaoFilialIntegracao.Atualizar.visible(true);
                _CRUDConfiguracaoFilialIntegracao.Cancelar.visible(true);
                _CRUDConfiguracaoFilialIntegracao.Excluir.visible(true);
                _CRUDConfiguracaoFilialIntegracao.Adicionar.visible(false);
                                
                SetaGridIntegracoes(arg.Data.TiposIntegracao);                

            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}


function BuscarConfiguracaoFilialIntegracao() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarConfiguracaoFilialIntegracao, tamanho: "20", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridConfiguracaoFilialIntegracao = new GridView(_pesquisaConfiguracaoFilialIntegracao.Pesquisar.idGrid, "ConfiguracaoFilialIntegracao/Pesquisar", _pesquisaConfiguracaoFilialIntegracao, menuOpcoes, null);
    _gridConfiguracaoFilialIntegracao.CarregarGrid();
}

function adicionarClick(e, sender) {        
    ObterTiposIntegracaoSalvar();

    Salvar(_configuracaoFilialIntegracao, "ConfiguracaoFilialIntegracao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
                _gridConfiguracaoFilialIntegracao.CarregarGrid();
                limparCamposConfigFilialInt();                
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function limparCamposConfigFilialInt() {
    _CRUDConfiguracaoFilialIntegracao.Atualizar.visible(false);
    _CRUDConfiguracaoFilialIntegracao.Cancelar.visible(false);
    _CRUDConfiguracaoFilialIntegracao.Excluir.visible(false);
    _CRUDConfiguracaoFilialIntegracao.Adicionar.visible(true);
    LimparCampos(_configuracaoFilialIntegracao);     
    limparCamposIntegracoes();    
    Global.ResetarAbas();
}

function atualizarClick(e, sender) {    
    ObterTiposIntegracaoSalvar();    

    Salvar(_configuracaoFilialIntegracao, "ConfiguracaoFilialIntegracao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Gerais.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                _gridConfiguracaoFilialIntegracao.CarregarGrid();
                limparCamposConfigFilialInt();                
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Gerais.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Gerais.Falha, arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_configuracaoFilialIntegracao, "ConfiguracaoFilialIntegracao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridConfiguracaoFilialIntegracao.CarregarGrid();
                    limparCamposConfigFilialInt();                    
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposConfigFilialInt();
}