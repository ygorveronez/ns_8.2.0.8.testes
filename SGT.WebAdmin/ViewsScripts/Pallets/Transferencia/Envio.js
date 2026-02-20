/// <reference path="Transferencia.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />
/// <reference path="../../Consultas/Turno.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTransferenciaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _envioTransferenciaPallet;

/*
 * Declaração das Classes
 */

var EnvioTransferenciaPallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Filial:", idBtnSearch: guid(), enable: false });
    this.Quantidade = PropertyEntity({ val: ko.observable(""), def: "", text: "*Quantidade: ", getType: typesKnockout.int, required: true, enable: ko.observable(true) });
    this.Remetente = PropertyEntity({ val: ko.observable(""), def: "", text: "*Remetente: ", required: true, enable: ko.observable(true) });
    this.Responsavel = PropertyEntity({ val: ko.observable(""), def: "", text: "*Responsável: ", required: true, enable: ko.observable(true) });
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Setor:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Turno = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Turno:", idBtnSearch: guid(), enable: ko.observable(true) });

    this.Quantidade.val.subscribe(preencherQuantidadeEnviada);
    this.Setor.codEntity.subscribe(controlarTurnoEnvio);
}

/*
 * Declaração das Funções de Inicialização
 */

function loadEnvioTransferenciaPallet() {
    _envioTransferenciaPallet = new EnvioTransferenciaPallet();
    KoBindings(_envioTransferenciaPallet, "knockoutEnvioTransferenciaPallet");

    new BuscarFilial(_envioTransferenciaPallet.Filial);
    new BuscarSetorFuncionario(_envioTransferenciaPallet.Setor, null, _envioTransferenciaPallet.Filial);
    new BuscarTurno(_envioTransferenciaPallet.Turno, null, null, _envioTransferenciaPallet.Filial, _envioTransferenciaPallet.Setor);
}

/*
 * Declaração das Funções
 */

function adicionarEnvio() {
    exibirConfirmacao("Confirmação", "Realmente deseja enviar a transferência de pallets?", function () {
        if (ValidarCamposObrigatorios(_envioTransferenciaPallet)) {
            var transferenciaEnvio = {
                Codigo: _transferenciaPallet.Codigo.val(),
                Envio: JSON.stringify(RetornarObjetoPesquisa(_envioTransferenciaPallet))
            };

            executarReST("Transferencia/AdicionarEnvio", transferenciaEnvio, function (retorno) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Transferência de pallets enviada com sucesso");

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

function controlarCamposEnvioHabilitados() {
    var habilitarCampo = (_transferenciaPallet.Situacao.val() === EnumSituacaoTransferenciaPallet.AguardandoEnvio);

    _envioTransferenciaPallet.Quantidade.enable(habilitarCampo);
    _envioTransferenciaPallet.Remetente.enable(habilitarCampo);
    _envioTransferenciaPallet.Responsavel.enable(habilitarCampo);
    _envioTransferenciaPallet.Setor.enable(habilitarCampo);
    _envioTransferenciaPallet.Turno.enable(false);

    if (habilitarCampo) {
        _envioTransferenciaPallet.Filial.codEntity(_solicitacaoTransferenciaPallet.Filial.codEntity());
        _envioTransferenciaPallet.Filial.val(_solicitacaoTransferenciaPallet.Filial.val());

        if (_dadosSetorUsuario.Setor.codEntity() > 0) {
            _envioTransferenciaPallet.Setor.codEntity(_dadosSetorUsuario.Setor.codEntity());
            _envioTransferenciaPallet.Setor.val(_dadosSetorUsuario.Setor.val());
            _envioTransferenciaPallet.Setor.enable(false);
        }

        if (_dadosSetorUsuario.Turno.codEntity() > 0) {
            _envioTransferenciaPallet.Turno.codEntity(_dadosSetorUsuario.Turno.codEntity());
            _envioTransferenciaPallet.Turno.val(_dadosSetorUsuario.Turno.val());
            _envioTransferenciaPallet.Turno.enable(false);
        }
    }
}

function controlarTurnoEnvio(codigoSetor) {
    _envioTransferenciaPallet.Turno.enable(codigoSetor > 0);
    _envioTransferenciaPallet.Turno.val("");
    _envioTransferenciaPallet.Turno.codEntity(0);
}

function limparCamposEnvio() {
    LimparCampos(_envioTransferenciaPallet);
}

function preencherEnvio(dadosEnvio) {
    if (dadosEnvio)
        PreencherObjetoKnout(_envioTransferenciaPallet, { Data: dadosEnvio });
}