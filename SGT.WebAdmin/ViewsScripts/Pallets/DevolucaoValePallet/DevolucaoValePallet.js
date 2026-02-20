/// <reference path="Etapas.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Enumeradores/EnumAbaDevolucaoValePallet.js" />
/// <reference path="../../Enumeradores/EnumSituacaoDevolucaoValePallet.js" />
/// <reference path="../../Enumeradores/EnumSituacaoValePallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _abaAtiva;
var _CRUDDevolucaoValePallet;
var _gridValePallet;
var _pesquisaValePallet;
var _devolucaoValePallet;

/*
 * Declaração das Classes
 */

var CRUDDevolucaoValePallet = function () {
    this.Baixar = PropertyEntity({ eventClick: baixarClick, type: types.event, text: ko.observable("Baixar Vale Pallet"), visible: ko.observable(true) });
    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasClick, type: types.event, text: "Reprocessar Regras", visible: ko.observable(false) });
}

var PesquisaValePallet = function () {
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid() });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Final: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Nfe = PropertyEntity({ text: "Nf-e: ", getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoValePallet.Todas), options: EnumSituacaoValePallet.obterOpcoes(), def: EnumSituacaoValePallet.Todas, text: "Situação: " });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridValePallet.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
}

var DevolucaoValePallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoValePallet = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Filial:", idBtnSearch: guid(), enable: ko.observable(false) });
    this.Numero = PropertyEntity({ val: ko.observable(""), def: "", text: "Número: ", enable: false });
    this.NumeroValePallet = PropertyEntity({ val: ko.observable(""), def: "", text: "Número do Vale Pallets: ", enable: false });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 400, enable: ko.observable(false) });
    this.Quantidade = PropertyEntity({ val: ko.observable(""), def: "", text: "Quantidade: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 7, enable: false });
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Setor:", idBtnSearch: guid(), required: true, enable: ko.observable(false) });
    this.DataDevolucao = PropertyEntity({ type: types.map, text: "*Data:", getType: typesKnockout.date, idBtnSearch: guid(), required: false, enable: ko.observable(false), visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoDevolucaoValePallet.Todas), def: EnumSituacaoDevolucaoValePallet.Todas, getType: typesKnockout.int });

    this.Filial.codEntity.subscribe(controlarSetorSolicitacao);
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridValePallets() {
    var opcaoDevolver = { descricao: "Devolução", id: guid(), metodo: devolucaoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 11, opcoes: [opcaoDevolver] };

    var configuracaoExportacao = {
        url: "DevolucaoValePallet/ExportarPesquisa",
        titulo: "Devolução de Vale Pallets"
    };

    _gridValePallet = new GridViewExportacao(_pesquisaValePallet.Pesquisar.idGrid, "DevolucaoValePallet/Pesquisa", _pesquisaValePallet, menuOpcoes, configuracaoExportacao);
    _gridValePallet.CarregarGrid();
}

function loadDevolucaoValePallet() {
    _devolucaoValePallet = new DevolucaoValePallet();
    KoBindings(_devolucaoValePallet, "knockoutDadosDevolucaoValePallet");

    _pesquisaValePallet = new PesquisaValePallet();
    KoBindings(_pesquisaValePallet, "knockoutPesquisaValePallet", false, _pesquisaValePallet.Pesquisar.id);

    _CRUDDevolucaoValePallet = new CRUDDevolucaoValePallet();
    KoBindings(_CRUDDevolucaoValePallet, "knockoutCRUDDevolucaoValePallet");

    new BuscarFilial(_pesquisaValePallet.Filial);
    new BuscarClientes(_pesquisaValePallet.Cliente);

    new BuscarFilial(_devolucaoValePallet.Filial);
    new BuscarSetorFuncionario(_devolucaoValePallet.Setor, null, _devolucaoValePallet.Filial);

    loadEtapaDevolucaoValePallet();
    loadAprovacaoDevolucaoValePallet();
    loadGridValePallets();

    DefineCamposDevolucaoValePallet();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function baixarClick() {
    if (_devolucaoValePallet.Situacao.val() === EnumSituacaoDevolucaoValePallet.Todas) {
        exibirConfirmacao("Confirmação", "Realmente deseja solicitar a baixa do vale pallets?", function () {
            if (ValidarCamposObrigatorios(_devolucaoValePallet)) {
                executarReST("DevolucaoValePallet/Baixar", RetornarObjetoPesquisa(_devolucaoValePallet), function (retorno) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação de baixa de pallets realizada com sucesso");

                        fecharModalDevolucaoValePallet();
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
                    }
                }, null);
            }
            else
                exibirMensagemCamposObrigatorio();
        });
    }
}

function devolucaoClick(registroSelecionado) {
    buscarDevolucaoValePallet(registroSelecionado.Codigo);
}

function reprocessarRegrasClick() {
    if (_devolucaoValePallet.Situacao.val() === EnumSituacaoDevolucaoValePallet.SemRegraAprovacao) {
        executarReST("DevolucaoValePallet/ReprocessarRegras", { Codigo: _devolucaoValePallet.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Regras de aprovação reprocessadas com sucesso.");

                    buscarDevolucaoValePallet(_devolucaoValePallet.Codigo.val());
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sem Regra", "Nenhuma regra para aprovar a Devolução de Vale Pallets.");
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    }
}

/*
 * Declaração das Funções Públicas
 */

function DefineCamposDevolucaoValePallet() {
    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Pallets_PermiteDataRetroativa_ValePallet, _PermissoesPersonalizadasPallets)) {
        _devolucaoValePallet.DataDevolucao.visible(true);
        _devolucaoValePallet.DataDevolucao.required = true;
    }
}

function buscarDevolucaoValePallet(codigo) {
    executarReST("DevolucaoValePallet/BuscarPorCodigoValePallet", { Codigo: codigo }, function (retorno) {
        if (retorno.Success) {
            preencherDevolucaoValePallet(retorno.Data);
            controlarCamposHabilitados();
            setarEtapas();
            exibirModalDevolucaoValePallet();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function controlarBotoesHabilitados() {
    var botaoBaixarVisivel = false;
    var botaoReprocessarRegrasVisivel = false;

    switch (_devolucaoValePallet.Situacao.val()) {
        case EnumSituacaoDevolucaoValePallet.Todas:
            botaoBaixarVisivel = true;
            break;

        case EnumSituacaoDevolucaoValePallet.SemRegraAprovacao:
            if (_abaAtiva === EnumAbaDevolucaoValePallet.Aprovacao) {
                botaoReprocessarRegrasVisivel = true;
            }
            break;
    }

    _devolucaoValePallet.DataDevolucao.enable(botaoBaixarVisivel);
    _CRUDDevolucaoValePallet.Baixar.visible(botaoBaixarVisivel);
    _CRUDDevolucaoValePallet.ReprocessarRegras.visible(botaoReprocessarRegrasVisivel);
}

function controlarCamposHabilitados() {
    if (_devolucaoValePallet.Codigo.val() > 0) {
        _devolucaoValePallet.Filial.enable(false);
        _devolucaoValePallet.Setor.enable(false);
        _devolucaoValePallet.Observacao.enable(false);
        _devolucaoValePallet.DataDevolucao.enable(false);
    }
    else {
        _devolucaoValePallet.Filial.enable(!_devolucaoValePallet.Filial.val());
        _devolucaoValePallet.Setor.enable(_devolucaoValePallet.Filial.val() && !_devolucaoValePallet.Setor.val());
        _devolucaoValePallet.Observacao.enable(true);
    }
}

function controlarSetorSolicitacao(codigoFilial) {
    _devolucaoValePallet.Setor.enable(codigoFilial > 0);
    _devolucaoValePallet.Setor.val("");
    _devolucaoValePallet.Setor.codEntity(0);
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function exibirModalDevolucaoValePallet() {
    Global.abrirModal('divModalDevolucaoValePallet');
    $("#divModalDevolucaoValePallet").one('hidden.bs.modal', function () {
        limparCamposDevolucaoValePallet();
    });
}

function fecharModalDevolucaoValePallet() {
    Global.fecharModal('divModalDevolucaoValePallet');
}

function limparCamposDevolucaoValePallet() {
    LimparCampos(_devolucaoValePallet);
}

function preencherDevolucaoValePallet(dados) {
    PreencherObjetoKnout(_devolucaoValePallet, dados.DadosDevolucao);
    preencherAprovacao(dados.ResumoAprovacao, _devolucaoValePallet.Codigo.val(), _devolucaoValePallet.Situacao.val());
}