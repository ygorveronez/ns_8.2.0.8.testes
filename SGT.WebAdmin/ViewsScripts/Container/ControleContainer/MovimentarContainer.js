/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../Enumeradores/EnumstatusColetaContainer.js" />
/// <reference path="../../Consultas/Cliente.js" />


var _buscaCargaExpRetiradaContainer;
var _movimentarContainer;
var _statusMovimentacaoContainerAnterior;

var MovimentarContainer = function () {
    this.TipoContainer = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), def: 0 });

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumStatusColetaContainer.AguardandoColeta), text: "Situação:", def: EnumStatusColetaContainer.AguardandoColeta, options: EnumStatusColetaContainer.obterOpcoes(), visible: ko.observable(true) });
    this.DataMovimentacao = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual(), text: "*Data Movimentação:", visible: ko.observable(true), required: true });
    this.LocalAtual = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: "*Local Atual:", required: false, visible: ko.observable(false), idBtnSearch: guid() });
    this.LocalEmbarque = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: "*Local Embarque:", required: false, visible: ko.observable(false), idBtnSearch: guid() });
    this.DataEmbarque = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual(), text: "Data Porto:", visible: ko.observable(false), required: false });

    this.DataEmbarqueNavio = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual(), text: "Data Embarque Navio:", visible: ko.observable(false), required: false });

    //this.BuscarCargaExp = PropertyEntity({ eventClick: buscarCargaRetiradaContainerClick, type: types.event, text: "Buscar Carga retirada Container", visible: ko.observable(false) });
    this.CargaExpRetirada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), text: "Carga retirada Container:", required: false, visible: ko.observable(false) });
    this.TransferirContainerOutraCarga = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: "Transferir container para outra carga", visible: ko.observable(true) });

    this.CargaTransferida = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), text: "Carga a ser Transferida:", required: false, visible: ko.observable(false) });

    this.Situacao.val.subscribe(function (valor) {

        _movimentarContainer.LocalEmbarque.visible(valor == EnumStatusColetaContainer.Porto);
        _movimentarContainer.LocalEmbarque.required = (valor == EnumStatusColetaContainer.Porto);
        _movimentarContainer.DataEmbarque.visible(valor == EnumStatusColetaContainer.Porto);
        _movimentarContainer.DataEmbarque.required = (valor == EnumStatusColetaContainer.Porto);

        _movimentarContainer.DataEmbarqueNavio.visible(valor == EnumStatusColetaContainer.EmbarcadoNavio);
        _movimentarContainer.DataEmbarqueNavio.required = (valor == EnumStatusColetaContainer.EmbarcadoNavio);

        if (_statusMovimentacaoContainerAnterior == EnumStatusColetaContainer.EmAreaEsperaVazio)
            _movimentarContainer.CargaExpRetirada.visible(valor != EnumStatusColetaContainer.EmAreaEsperaVazio && valor != EnumStatusColetaContainer.Cancelado && valor != EnumStatusColetaContainer.AguardandoColeta);

        _movimentarContainer.LocalAtual.visible(valor == EnumStatusColetaContainer.EmAreaEsperaVazio || valor == EnumStatusColetaContainer.EmAreaEsperaCarregado || valor == EnumStatusColetaContainer.EmCarregamento);
        _movimentarContainer.LocalAtual.required = (valor == EnumStatusColetaContainer.EmAreaEsperaVazio || valor == EnumStatusColetaContainer.EmAreaEsperaCarregado || valor == EnumStatusColetaContainer.EmCarregamento);

    });

    this.ConfirmarMovimentacao = PropertyEntity({ type: types.event, eventClick: confirmarMovimentacaoContainerClick, text: "Confirmar" });

};

function loadMovimentarContainer() {
    _movimentarContainer = new MovimentarContainer();
    KoBindings(_movimentarContainer, "knockoutMovimentarContainer");

    new BuscarClientes(_movimentarContainer.LocalEmbarque);
    new BuscarClientes(_movimentarContainer.LocalAtual);

    new BuscarCargaEXPRetiradaContainer(_movimentarContainer.CargaExpRetirada, null, _movimentarContainer.TipoContainer);

    new BuscarCargas(_movimentarContainer.CargaTransferida);
}

function confirmarMovimentacaoContainerClick() {
    if (_movimentarContainer.TransferirContainerOutraCarga.val() && _movimentarContainer.CargaTransferida.val() == "") {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Para transferencia de containers entre cargas, o campo carga de transferencia é obrigatorio.");
        return;
    }

    if (!ValidarCamposObrigatorios(_movimentarContainer)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    };

    if (_statusMovimentacaoContainerAnterior == EnumStatusColetaContainer.EmAreaEsperaVazio && _movimentarContainer.CargaExpRetirada.val() == "") {
        exibirConfirmacao("Movimentar Container", "Atenção, você esta movimentando um container sem a informação de carga, deseja continuar?", function () {
            executarReST("ControleContainer/MovimentarContainer", RetornarObjetoPesquisa(_movimentarContainer), function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        Global.fecharModal("divModalMovimentarContainer");
                        _gridControleContainer.CarregarGrid();
                        exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        });
    } else if (_movimentarContainer.TransferirContainerOutraCarga.val() && _movimentarContainer.CargaTransferida.val() != "") {

        exibirConfirmacao("Movimentar Container", "Tem certeza que deseja transferir este container para outra carga?", function () {
            executarReST("ControleContainer/MovimentarContainer", RetornarObjetoPesquisa(_movimentarContainer), function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        Global.fecharModal("divModalMovimentarContainer");
                        _gridControleContainer.CarregarGrid();
                        exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        });
    }
    else {
        executarReST("ControleContainer/MovimentarContainer", RetornarObjetoPesquisa(_movimentarContainer), function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    Global.fecharModal("divModalMovimentarContainer");
                    _gridControleContainer.CarregarGrid();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}
