/// <reference path="../../Enumeradores/EnumSituacaoOrdemServicoFrota.js" />
/// <reference path="Fatura.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _aprovacaoFatura;
var _detalheAutorizacaoFatura;
var _gridAutorizacoes;

/*
 * Declaração das Classes
 */

var AprovacaoFatura = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.AprovacoesNecessarias = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações Necessárias:", enable: ko.observable(true) });
    this.Aprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações:", enable: ko.observable(true) });
    this.Reprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Reprovações:", enable: ko.observable(true) });

    this.DetalheAprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.bool, visible: ko.observable(false), val: ko.observable(true), def: true });
    this.PossuiRegras = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(true), def: true });
    this.Regras = PropertyEntity({ type: types.map, idGrid: guid() });

    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasClick, type: types.event, text: "Reprocessar Regras", visible: ko.observable(false) });
    this.ExibirAprovadores = PropertyEntity({
        eventClick: toggleExibirAprovadoresClick, type: types.event, text: ko.pureComputed(function () {
            return this.ExibirAprovadores.val() ? "Exibir Aprovadores" : "Ocultar Aprovadores";
        }, this), visible: ko.observable(false), val: ko.observable(true), def: true
    });

    this.PossuiRegrasBooleano = PropertyEntity({ type: types.bool, text: "Possui Regras de Aprovação", val: ko.observable(false), visible: ko.observable(false) });
};

var DetalheAutorizacaoFatura = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Regra = PropertyEntity({ text: "Regra:", val: ko.observable("") });
    this.Data = PropertyEntity({ text: "Data: ", val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable("") });
    this.Usuario = PropertyEntity({ text: "Usuário:", val: ko.observable("") });
    this.Motivo = PropertyEntity({ text: "Motivo:", val: ko.observable(""), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadAprovacaoAprovacaoFatura() {
    _aprovacaoFatura = new AprovacaoFatura();
    KoBindings(_aprovacaoFatura, "knockoutAprovacaoFatura");

    _detalheAutorizacaoFatura = new DetalheAutorizacaoFatura();
    KoBindings(_detalheAutorizacaoFatura, "knockoutDetalheAutorizacaoFatura");

    loadGridAutorizacoes();
    buscarRegrasCadastradas();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && _aprovacaoFatura.PossuiRegrasBooleano.val())
        _etapaFatura.Etapa6.visible(true);
}

function CarregarAprovacaoAprovacaoFatura() {
    LimparCamposAprovacaoAprovacaoFatura();

    executarReST("FaturaAprovacao/ObterDetalhesGeraisAprovacao", { Codigo: _fatura.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                preencherAprovacaoAprovacaoFatura(r.Data);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function loadGridAutorizacoes() {
    var detalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: detalharAutorizacaoClick, tamanho: "4", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    _gridAutorizacoes = new GridView(_aprovacaoFatura.Regras.idGrid, "FaturaAprovacao/PesquisaAutorizacoes", _aprovacaoFatura, menuOpcoes);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function toggleExibirAprovadoresClick() {
    var valorAtual = _aprovacaoFatura.ExibirAprovadores.val();

    _aprovacaoFatura.ExibirAprovadores.val(!valorAtual);
    _aprovacaoFatura.DetalheAprovadores.visible(valorAtual);
}

function reprocessarRegrasClick(e, sender) {
    executarReST("FaturaAprovacao/ReprocessarRegras", { Codigo: _fatura.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                var dados = retorno.Data;
                if (dados.Situacao === EnumSituacoesFatura.SemRegraAprovacao)
                    exibirMensagem(tipoMensagem.aviso, "Sem Regra", "Nenhuma regra para aprovar a Ordem de Serviço.");
                else if (dados.Situacao === EnumSituacoesFatura.AguardandoAprovacao)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Ordem de Serviço está aguardando aprovação.");

                preencherAprovacaoAprovacaoFatura(dados);

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function detalharAutorizacaoClick(registroSelecionado) {
    _detalheAutorizacaoFatura.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_detalheAutorizacaoFatura, "FaturaAprovacao/DetalhesAutorizacao", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data != null) {
                Global.abrirModal('divModalDetalhesAutorizacaoFatura');
                $("#divModalDetalhesAutorizacaoFatura").one('hidden.bs.modal', function () {
                    limparCamposDetalheAutorizacao();
                });
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções
 */

function preencherAprovacaoAprovacaoFatura(dados) {
    if (dados.Situacao === EnumSituacoesFatura.SemRegraAprovacao)
        _aprovacaoFatura.PossuiRegras.val(false);
    else {
        _aprovacaoFatura.PossuiRegras.val(true);

        PreencherObjetoKnout(_aprovacaoFatura, { Data: dados });

        _gridAutorizacoes.CarregarGrid();
    }
}

function limparCamposDetalheAutorizacao() {
    LimparCampos(_detalheAutorizacaoFatura);
}

function LimparCamposAprovacaoAprovacaoFatura() {
    LimparCampos(_detalheAutorizacaoFatura);
    LimparCampos(_aprovacaoFatura);
}

function buscarRegrasCadastradas() {
    executarReST("FaturaAprovacao/BuscarRegrasCadastradas", null, function (retorno) {

        if (retorno.Success) {
            if (retorno.Data) {
                _aprovacaoFatura.PossuiRegrasBooleano.val(retorno.Data.PossuiRegrasAprovacao)
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

