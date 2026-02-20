/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../Global/CentralProcessamento/CentralProcessamento.js" />

var _aprovadoresAutorizacaoIntegracaoCTe;
var _detalhesAprovacaoAutorizacaoIntegracaoCTe;
var _gridAprovadoresAutorizacaoIntegracaoCTe;

var AprovadoresAutorizacaoIntegracaoCTe = function () {
    this.NumeroAprovadoresNecessarios = PropertyEntity({ getType: typesKnockout.string, text: "Aprovações Necessárias: " });
    this.Aprovacoes = PropertyEntity({ getType: typesKnockout.string, text: "Aprovações: " });
    this.Rejeicoes = PropertyEntity({ getType: typesKnockout.string, text: "Rejeições: " });
    this.Pendentes = PropertyEntity({ getType: typesKnockout.string, text: "Pendentes: " });
    this.Situacao = PropertyEntity({ getType: typesKnockout.string, text: "Situação: " });
    this.AprovadoresAutorizacaoIntegracaoCTe = PropertyEntity({ type: types.local, idGrid: guid() });
};

var DetalhesAprovacaoIntegracaoCTe = function () {
    this.Regra = PropertyEntity({ text: "Regra: ", getType: typesKnockout.string });
    this.Usuario = PropertyEntity({ text: "Usuário: ", getType: typesKnockout.string });
    this.Situacao = PropertyEntity({ text: "Situação: ", getType: typesKnockout.string });
    this.Motivo = PropertyEntity({ text: "Motivo: ", getType: typesKnockout.string });
}

function loadAprovadoresAutorizacaoIntegracaoCTe() {
    _aprovadoresAutorizacaoIntegracaoCTe = new AprovadoresAutorizacaoIntegracaoCTe();
    _detalhesAprovacaoAutorizacaoIntegracaoCTe = new DetalhesAprovacaoIntegracaoCTe();

    KoBindings(_aprovadoresAutorizacaoIntegracaoCTe, "knockoutAprovadoresAutorizacaoIntegracaoCTe");
    KoBindings(_detalhesAprovacaoAutorizacaoIntegracaoCTe, "knockoutDetalhesAprovadorAutorizacaoIntegracaoCTe");

    loadGridAprovadoresAutorizacaoIntegracaoCTe();
    buscarDadosAprovacaoAutorizacaoIntegracaoCTe();
}

function loadGridAprovadoresAutorizacaoIntegracaoCTe() {
    var detalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: detalhesAprovadorIntegracaoCTeClick, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(detalhes);

    var header = [
        { data: "Codigo", visible: false },
        { data: "Regra", visible: false },
        { data: "Motivo", visible: false },
        { data: "Usuario", title: "Usuário", width: "50%" },
        { data: "Prioridade", title: "Prioridade", width: "25%", className: "text-align-center" },
        { data: "Situacao", title: "Situação", width: "25%", className: "text-align-center" }
    ];

    _gridAprovadoresAutorizacaoIntegracaoCTe = new BasicDataTable(_aprovadoresAutorizacaoIntegracaoCTe.AprovadoresAutorizacaoIntegracaoCTe.idGrid, header, menuOpcoes, { column: 0, dir: orderDir.asc });
    _gridAprovadoresAutorizacaoIntegracaoCTe.CarregarGrid([]);
}

function buscarDadosAprovacaoAutorizacaoIntegracaoCTe() {
    executarReST("AutorizacaoIntegracaoCTe/BuscarDadosAprovacao", { Codigo: _cargaAtual.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _gridAprovadoresAutorizacaoIntegracaoCTe.CarregarGrid(r.Data.Aprovacoes);
                PreencherObjetoKnout(_aprovadoresAutorizacaoIntegracaoCTe, { Data: r.Data.Detalhes });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function detalhesAprovadorIntegracaoCTeClick(registroSelecionado) {
    preencherDetalhesAprovador(registroSelecionado);
    Global.abrirModal("divModalDetalhesAprovadorAutorizacaoIntegracaoCTe");
}

function preencherDetalhesAprovador(data) {
    _detalhesAprovacaoAutorizacaoIntegracaoCTe.Usuario.val(data.Usuario);
    _detalhesAprovacaoAutorizacaoIntegracaoCTe.Situacao.val(data.Situacao);
    _detalhesAprovacaoAutorizacaoIntegracaoCTe.Regra.val(data.Regra);
    _detalhesAprovacaoAutorizacaoIntegracaoCTe.Motivo.val(data.Motivo);
}