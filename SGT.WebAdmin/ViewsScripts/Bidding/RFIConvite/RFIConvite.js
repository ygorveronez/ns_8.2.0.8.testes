/// <reference path="RFIAnexoConvite.js" />
/// <reference path="RFIConvidadosConvite.js" />
/// <reference path="RFIChecklist.js" />
/// <reference path="EtapaRFIConvite.js" />
/// <reference path="../../Enumeradores/EnumSituacaoRFIConvite.js" />

/*Declaração Objetos*/
var _CRUDRFIConvite;
var _RFIConvite;
var _pesquisaRFIConvite;
var _gridRFIConvite;

var _situacaoOptions = [
    { text: "Inativo", value: 0 },
    { text: "Ativo", value: 1 }
];

var CRUDRFIConvite = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Salvar RFI", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: LimparClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
};

var PesquisaRFIConvite = function () {
    this.Descricao = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: "Descrição:" });
    this.DataInicio = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.date, text: "Data Inicio:" });
    this.DataLimite = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.date, text: "Data Limite" });
    this.Situacao = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: "Situação:", options: EnumSituacaoRFIConvite.ObterOpcoesPesquisa() });
    this.Pesquisar = PropertyEntity({ eventClick: PesquisarConvite, type: types.event, text: "Pesquisar", visible: ko.observable(true), idGrid: guid() });
    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
}

var RFIConvite = function () {
    this.Codigo = PropertyEntity({ text: "Número RFI", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(false) });
    this.Situacao = PropertyEntity({ text: "*Situação:", required: true, getType: typesKnockout.dynamic, options: _situacaoOptions, val: ko.observable("1"), def: "1", enable: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "*Data Início RFI:", required: true, getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "*Data Limite RFI:", required: true, getType: typesKnockout.date });
    this.PrazoAceiteConvite = PropertyEntity({ text: "*Prazo para aceite do convite:", required: true, getType: typesKnockout.dateTime, val: ko.observable(""), enable: ko.observable(true) });
    this.DescritivoConvite = PropertyEntity({ text: "*Descritivo do convite:", required: true, getType: typesKnockout.string, maxlength: 5000, enable: ko.observable(true) });
    this.ExigirPreenchimentoChecklistConvitePeloTransportador = PropertyEntity({ text: "Exigir o preenchimento do checklist no convite por parte do transportador", getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });

    this.ExigirPreenchimentoChecklistConvitePeloTransportador.val.subscribe(function () {
        if (_RFIConvite.ExigirPreenchimentoChecklistConvitePeloTransportador.val()) {
            _checklist.PrazoChecklist.required = true;
            _checklist.PrazoChecklist.visible(true);
        } else {
            _checklist.PrazoChecklist.required = false;
            _checklist.PrazoChecklist.visible(false);
        }
    });

    this.DataInicio.val.subscribe(function () {
        if (_RFIConvite.DataInicio.val() != "") {
            _RFIConvite.PrazoAceiteConvite.enable(true);
            _RFIConvite.PrazoAceiteConvite.minDate(_RFIConvite.DataInicio.val());
        }
        else {
            _RFIConvite.PrazoAceiteConvite.enable(false);
            _RFIConvite.PrazoAceiteConvite.val("");
        }
    });
}

//Métodos Globais
function LoadRFI() {
    _RFIConvite = new RFIConvite();
    KoBindings(_RFIConvite, "knockoutRFI");

    HeaderAuditoria("RFIConvite", _RFIConvite);

    _CRUDRFIConvite = new CRUDRFIConvite();
    KoBindings(_CRUDRFIConvite, "knockoutCRUDRFIConvite");

    _pesquisaRFIConvite = new PesquisaRFIConvite();
    KoBindings(_pesquisaRFIConvite, "knockoutPesquisaRFIConvite");

    LoadGridRFI();
    LoadAnexoRFI();
    LoadConvidadosRFI();
    LoadChecklistRFI();
    LoadEtapaRFIConvite();
}

//Métodos Privados
function LoadGridRFI() {
    const opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (registroSelecionado) { EditarClick(registroSelecionado, false); }, tamanho: "10", icone: "" };
    const duplicar = { descricao: "Duplicar", id: guid(), evento: "onclick", metodo: function (registroSelecionado) { EditarClick(registroSelecionado, true); }, tamanho: "10", icone: "" };

    const menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoEditar, duplicar], tamanho: "10" };
    _gridRFIConvite = new GridViewExportacao(_pesquisaRFIConvite.Pesquisar.idGrid, "RFIConvite/Pesquisar", _pesquisaRFIConvite, menuOpcoes);
    _gridRFIConvite.CarregarGrid();
}

// Eventos de Clique
function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function PesquisarConvite() {
    _gridRFIConvite.CarregarGrid();
}

function EditarClick(registroSelecionado, duplicar) {
    LimparCamposRFIConvite();
    _CRUDRFIConvite.Adicionar.visible(duplicar);
    _CRUDRFIConvite.Atualizar.visible(!duplicar);

    _RFIConvite.Descricao.val(registroSelecionado.Descricao);
    _RFIConvite.DataInicio.val(registroSelecionado.DataInicio);
    _RFIConvite.DataLimite.val(registroSelecionado.DataLimite);

    executarReST("RFIConvite/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo, Duplicar: duplicar }, function (retorno) {

        if (retorno.Success) {
            if (retorno.Data) {
                _RFIConvite.Codigo.val(retorno.Data.Codigo);
                _pesquisaRFIConvite.ExibirFiltros.visibleFade(false);
                _listaAnexo.Anexos.val(retorno.Data.Anexos.slice());
                _convidados = retorno.Data.Convidados.slice();
                _RFIConvite.PrazoAceiteConvite.val(retorno.Data.PrazoAceiteConvite);
                _RFIConvite.DescritivoConvite.val(retorno.Data.DescritivoConvite);
                _RFIConvite.ExigirPreenchimentoChecklistConvitePeloTransportador.val(retorno.Data.ExigirPreenchimentoChecklistConvitePeloTransportador);
                _RFIConvite.Situacao.val(retorno.Data.Situacao);
                _convidado.Convidado.basicTable.CarregarGrid(_convidados);
                _checklist.PrazoChecklist.val(retorno.Data.RFIChecklist.Prazo);
                _checklist.Codigo.val(retorno.Data.RFIChecklist.Codigo);
                questionario = retorno.Data.RFIChecklist.Questionarios.slice()
                _gridChecklistQuestionarios.CarregarGrid(questionario);

                if (duplicar)
                    return;

                if (retorno.Data.Iniciado) {
                    _CRUDRFIConvite.Atualizar.visible(false);
                    _RFIConvite.Situacao.enable(false);
                    _gridAnexo.DesabilitarOpcoes();
                    _gridConvidado.DesabilitarOpcoes();
                }

                if (retorno.Data.Etapa != EnumSituacaoRFIConvite.Fechamento) {
                    _RFIConvite.ExigirPreenchimentoChecklistConvitePeloTransportador.enable(false);
                    _convidado.Convidado.visible(true);
                    _CRUDRFIConvite.Atualizar.visible(true);
                } else {
                    _RFIConvite.Situacao.enable(true);
                    _RFIConvite.Descricao.enable(true);
                    _RFIConvite.DescritivoConvite.enable(true);
                    _RFIConvite.ExigirPreenchimentoChecklistConvitePeloTransportador.enable(true);
                    _CRUDRFIConvite.Atualizar.visible(false);
                }
                return SetarEtapaRFIConvite();
            }
            return exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        return exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);

}

function AdicionarClick(e, sender) {

    if (!ValidarCamposObrigatorios(_RFIConvite) || !ValidarCamposObrigatorios(_checklist)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    adicionarDadosRFIConvite();
}

function LimparClick() {
    LimparCamposRFIConvite();
}

function AtualizarClick() {

    if (!ValidarCamposObrigatorios(_RFIConvite) || !ValidarCamposObrigatorios(_checklist)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    executarReST("RFIConvite/Atualizar", obterDados(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "RFI Atualizado com sucesso");
                _gridRFIConvite.CarregarGrid();
                LimparCamposRFIConvite();
                limparGridConvidados();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function CarregarAprovacao() {

}

// Funções privadas
function obterDados() {
    const dadosRFI = RetornarObjetoPesquisa(_RFIConvite);
    _listaAnexo.Anexos.val([]);
    preencherConvite(dadosRFI);
    preencherChecklist(dadosRFI)
    return dadosRFI;
}

function preencherConvite(dadosRFI) {
    dadosRFI["Convite"] = JSON.stringify(obterConvidados());
}

function adicionarDadosRFIConvite() {
    executarReST("RFIConvite/Adicionar", obterDados(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "RFI adicionado com sucesso");

                _gridRFIConvite.CarregarGrid();
                enviarArquivosAnexados(retorno.Data.Codigo);
                LimparCamposRFIConvite();
                limparGridConvidados();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);

}

function LimparCamposRFIConvite() {
    _gridChecklistQuestionarios.HabilitarOpcoes();
    _gridAnexo.HabilitarOpcoes();
    _gridConvidado.HabilitarOpcoes();
    _gridQuestionario.Adicionar.visible(true);
    _listaAnexo.Adicionar.visible(true);
    _convidado.Convidado.visible(true);
    _CRUDRFIConvite.Adicionar.visible(true);
    _CRUDRFIConvite.Atualizar.visible(false);
    LimparCampos(_RFIConvite);
    _listaAnexo.Anexos.val([]);
    LimparCampos(_checklist);
    limparCamposChecklist();
    limparGridConvidados();
    SetarEnableCamposKnockout(_RFIConvite, true);
    _RFIConvite.Codigo.enable(false);
    SetarEtapaInicioRFIConvite();
    SetaLista([]);
    RecarregarGridOpcoes();
    LimparCamposOpcao();
}
