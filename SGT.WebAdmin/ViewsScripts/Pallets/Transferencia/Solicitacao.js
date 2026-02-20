/// <reference path="Transferencia.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />
/// <reference path="../../Consultas/Turno.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTransferenciaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _solicitacaoTransferenciaPallet;

/*
 * Declaração das Classes
 */

var SolicitacaoTransferenciaPallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Filial:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Numero = PropertyEntity({ val: ko.observable(""), def: "", text: "Número: ", enable: false });
    this.Quantidade = PropertyEntity({ val: ko.observable(""), def: "", text: "*Quantidade: ", getType: typesKnockout.int, required: true, enable: ko.observable(true) });
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Setor:", idBtnSearch: guid(), enable: ko.observable(false) });
    this.Solicitante = PropertyEntity({ val: ko.observable(""), def: "", text: "*Solicitante: ", required: true, enable: ko.observable(true) });
    this.Turno = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Turno:", idBtnSearch: guid(), enable: ko.observable(false) });

    this.Filial.codEntity.subscribe(controlarSetorSolicitacao);
    this.Quantidade.val.subscribe(preencherQuantidadeSolicitada);
    this.Setor.codEntity.subscribe(controlarTurnoSolicitacao);
}

/*
 * Declaração das Funções de Inicialização
 */

function loadSolicitacaoTransferenciaPallet() {
    _solicitacaoTransferenciaPallet = new SolicitacaoTransferenciaPallet();
    KoBindings(_solicitacaoTransferenciaPallet, "knockoutSolicitacaoTransferenciaPallet");

    new BuscarFilial(_solicitacaoTransferenciaPallet.Filial);
    new BuscarSetorFuncionario(_solicitacaoTransferenciaPallet.Setor, null, _solicitacaoTransferenciaPallet.Filial);
    new BuscarTurno(_solicitacaoTransferenciaPallet.Turno, null, null, _solicitacaoTransferenciaPallet.Filial, _solicitacaoTransferenciaPallet.Setor);
}

/*
 * Declaração das Funções
 */

function adicionarSolicitacao() {
    exibirConfirmacao("Confirmação", "Realmente deseja solicitar a transferência de pallets?", function () {
        if (ValidarCamposObrigatorios(_solicitacaoTransferenciaPallet)) {
            executarReST("Transferencia/AdicionarSolicitacao", RetornarObjetoPesquisa(_solicitacaoTransferenciaPallet), function (retorno) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Transferência de pallets solicitada com sucesso");

                    _gridTransferenciaPallet.CarregarGrid();

                    limparCamposTransferenciaPallet();
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
                }
            }, null);
        }
        else
            exibirMensagemCamposObrigatorio();
    });
}

function controlarCamposSolicitacaoHabilitados() {
    var habilitarCampo = (_transferenciaPallet.Situacao.val() === EnumSituacaoTransferenciaPallet.Todas);

    _solicitacaoTransferenciaPallet.Filial.enable(habilitarCampo);
    _solicitacaoTransferenciaPallet.Quantidade.enable(habilitarCampo);
    _solicitacaoTransferenciaPallet.Setor.enable(false);
    _solicitacaoTransferenciaPallet.Solicitante.enable(habilitarCampo);
    _solicitacaoTransferenciaPallet.Turno.enable(false);

    if (habilitarCampo) {
        if (_dadosSetorUsuario.Filial.codEntity() > 0) {
            _solicitacaoTransferenciaPallet.Filial.codEntity(_dadosSetorUsuario.Filial.codEntity());
            _solicitacaoTransferenciaPallet.Filial.val(_dadosSetorUsuario.Filial.val());
            _solicitacaoTransferenciaPallet.Filial.enable(false);
        }

        if (_dadosSetorUsuario.Setor.codEntity() > 0) {
            _solicitacaoTransferenciaPallet.Setor.codEntity(_dadosSetorUsuario.Setor.codEntity());
            _solicitacaoTransferenciaPallet.Setor.val(_dadosSetorUsuario.Setor.val());
            _solicitacaoTransferenciaPallet.Setor.enable(false);
        }

        if (_dadosSetorUsuario.Turno.codEntity() > 0) {
            _solicitacaoTransferenciaPallet.Turno.codEntity(_dadosSetorUsuario.Turno.codEntity());
            _solicitacaoTransferenciaPallet.Turno.val(_dadosSetorUsuario.Turno.val());
            _solicitacaoTransferenciaPallet.Turno.enable(false);
        }
    }
}

function controlarSetorSolicitacao(codigoFilial) {
    _solicitacaoTransferenciaPallet.Setor.enable(codigoFilial > 0);
    _solicitacaoTransferenciaPallet.Setor.val("");
    _solicitacaoTransferenciaPallet.Setor.codEntity(0);
}

function controlarTurnoSolicitacao(codigoSetor) {
    _solicitacaoTransferenciaPallet.Turno.enable(codigoSetor > 0);
    _solicitacaoTransferenciaPallet.Turno.val("");
    _solicitacaoTransferenciaPallet.Turno.codEntity(0);
}

function limparCamposSolicitacao() {
    LimparCampos(_solicitacaoTransferenciaPallet);
}

function preencherSolicitacao(dadosSolicitacao) {
    PreencherObjetoKnout(_solicitacaoTransferenciaPallet, { Data: dadosSolicitacao });
}