/// <reference path="AcompanhamentoPreAgrupamentoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _preAgrupamentoCargaDetalhe;
var _gridPreAgrupamentoCargaDetalhes;
var _modalPreAgrupamentoCargaDetalhe;
/*
 * Declaração das Classes
 */

var PreAgrupamentoCargaDetalhe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoAgrupador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoAgrupamento = PropertyEntity({ text: "Código Agrupamento: " });
    this.CodigoViagem = PropertyEntity({ text: "Código Viagem: " });
    this.Carga = PropertyEntity({ text: "Carga: " });
    this.CnpjEmitente = PropertyEntity({ text: "CNPJ Emitente: " });
    this.Situacao = PropertyEntity({ text: "Situação: " });
    this.TipoViagem = PropertyEntity({ text: "Tipo Viagem: " });
    this.Pendencia = PropertyEntity({ text: "Pendência:", visible: ko.observable(false) });
    this.NotasFiscais = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });
    this.Auditar = PropertyEntity({ eventClick: auditarAgrupamentoClick, type: types.event, text: "Auditar" });
    this.Reenviar = PropertyEntity({ eventClick: reenviarClick, type: types.event, text: "Reenviar", visible: ko.observable(false) });

    this.NotasFiscais.val.subscribe(function (notasFiscais) {
        _gridPreAgrupamentoCargaDetalhes.CarregarGrid(notasFiscais);
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadPreAgrupamentoCargaDetalhe() {
    _preAgrupamentoCargaDetalhe = new PreAgrupamentoCargaDetalhe();
    KoBindings(_preAgrupamentoCargaDetalhe, "knockoutPreAgrupamentoCargaDetalhe");

    loadGridPreAgrupamentoCargaDetalhe();
}

function loadGridPreAgrupamentoCargaDetalhe() {
    var header = [
        { data: "NumeroNota", title: "Número da Nota", width: "25%", className: 'text-align-center', orderable: false },
        { data: "SerieNota", title: "Série da Nota", width: "25%", className: 'text-align-center', orderable: false },
        { data: "CnpjEmitente", title: "CNPJ Emitente", width: "25%", className: 'text-align-center', orderable: false },
        { data: "CnpjDestinatario", title: "CNPJ Destinatário", width: "25%", className: 'text-align-center', orderable: false }
    ];
    var menuOpcoes = null;
    var ordenacao = { column: 0, dir: orderDir.desc };

    _gridPreAgrupamentoCargaDetalhes = new BasicDataTable("grid-pre-agrupamento-carga-detalhe", header, menuOpcoes, ordenacao);
    _modalPreAgrupamentoCargaDetalhe = new bootstrap.Modal(document.getElementById("divModalPreAgrupamentoCargaDetalhe"), { backdrop: true, keyboard: true });
}


function auditarAgrupamentoClick(e, sender) {
    var data = { Codigo: e.CodigoAgrupador.val() };
    var closureAuditoria = OpcaoAuditoria("PreAgrupamentoCargaAgrupador", null, e);

    closureAuditoria(data);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function reenviarClick() {
    executarReST("AcompanhamentoPreAgrupamentoCarga/Reenviar", { Codigo: _preAgrupamentoCargaDetalhe.CodigoAgrupador.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Reenviado com sucesso");
                _gridPreAgrupamentoCarga.CarregarGrid();
                fecharModalDetalhes();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções Públicas
 */

function exibirDetalhes(registroSelecionado) {
    executarReST("AcompanhamentoPreAgrupamentoCarga/ObterDetalhes", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_preAgrupamentoCargaDetalhe, retorno);

                if (retorno.Data.EnumSituacao == EnumSituacaoPreAgrupamentoCarga.ProblemaCarregamento) {
                    _preAgrupamentoCargaDetalhe.Pendencia.visible(true);
                    _preAgrupamentoCargaDetalhe.Reenviar.visible(true);
                }

                exibirModalDetalhes();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções Privadas
 */

function exibirModalDetalhes() {
    _modalPreAgrupamentoCargaDetalhe.show();
    $("#divModalPreAgrupamentoCargaDetalhe").one('hidden.bs.modal', function () {
        limparCamposPreAgrupamentoCargaDetalhe();
    });
}

function fecharModalDetalhes() {
    _modalPreAgrupamentoCargaDetalhe.hide();
}

function limparCamposPreAgrupamentoCargaDetalhe() {
    LimparCampos(_preAgrupamentoCargaDetalhe);

    _preAgrupamentoCargaDetalhe.NotasFiscais.val(new Array());
    _preAgrupamentoCargaDetalhe.Pendencia.visible(false);
    _preAgrupamentoCargaDetalhe.Reenviar.visible(false);
}