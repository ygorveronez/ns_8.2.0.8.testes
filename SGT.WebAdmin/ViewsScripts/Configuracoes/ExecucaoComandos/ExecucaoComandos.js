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

var _execucaoComandos;

var ExecucaoComandos = function () {
    this.AlterarStatusCTeRejeitado = PropertyEntity({ text: "Alterar Status do CT-e de Em Emissão para Rejeitado:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.ExecutarAlterarStatusCTeRejeitado = PropertyEntity({ eventClick: executarAlterarStatusCTeRejeitadoClick, type: types.event, text: "Executar", visible: ko.observable(true) });

    this.AlterarStatusCTeAutorizado = PropertyEntity({ text: "Alterar Status do CT-e de Em Cancelamento para Autorizado:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.ExecutarAlterarStatusCTeAutorizado = PropertyEntity({ eventClick: executarAlterarStatusCTeAutorizadoClick, type: types.event, text: "Executar", visible: ko.observable(true) });

    this.AlterarStatusCTeInutilizacaoRejeitado = PropertyEntity({ text: "Alterar Status do CT-e de Em Inutilização para Rejeitado:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.ExecutarAlterarStatusCTeInutilizacaoRejeitado = PropertyEntity({ eventClick: executarAlterarStatusCTeInutilizacaoRejeitadoClick, type: types.event, text: "Executar", visible: ko.observable(true) });

    this.ReverterAnulacaoGerencialCTe = PropertyEntity({ text: "Reverter Anulação Gerencial do CT-e (Anulado para Autorizado):", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.ExecutarReverterAnulacaoGerencialCTe = PropertyEntity({ eventClick: executarReverterAnulacaoGerencialCTeClick, type: types.event, text: "Executar", visible: ko.observable(true) });

    this.CancelarCargaNovaNaoFechada = PropertyEntity({ text: "Cancelar Carga Nova e Não Fechada:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.ExecutarCancelarCargaNovaNaoFechada = PropertyEntity({ eventClick: executarCancelarCargaNovaNaoFechadaClick, type: types.event, text: "Executar", visible: ko.observable(true) });

    this.ExecutarScriptPreCadastrado = PropertyEntity({ text: "Executar Script Pré-cadastrado:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.ExecutarExecutarScriptPreCadastrado = PropertyEntity({ eventClick: executarExecutarScriptPreCadastradoClick, type: types.event, text: "Executar", visible: ko.observable(true) });
}

function loadExecucaoComandos() {
    _execucaoComandos = new ExecucaoComandos();
    KoBindings(_execucaoComandos, "knockoutExecucaoComandos");

    BuscarCTes(_execucaoComandos.AlterarStatusCTeRejeitado, null, ['E']);
    BuscarCTes(_execucaoComandos.AlterarStatusCTeAutorizado, null, ['K']);
    BuscarCTes(_execucaoComandos.AlterarStatusCTeInutilizacaoRejeitado, null, ['L']);
    BuscarCTes(_execucaoComandos.ReverterAnulacaoGerencialCTe, null, ['Z']);
    BuscarCargas(_execucaoComandos.CancelarCargaNovaNaoFechada, null, null, null, null, null, null, null, null, null, null, null, null, null, 1, null, null, null, true);
    BuscarScripts(_execucaoComandos.ExecutarScriptPreCadastrado);
}

function executarAlterarStatusCTeRejeitadoClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente executar essa ação?", function () {
        Salvar(_execucaoComandos, "ExecucaoComandos/AlterarStatusCTeRejeitado", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso.");
                    LimparCamposExecucao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function executarAlterarStatusCTeAutorizadoClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente executar essa ação?", function () {
        Salvar(_execucaoComandos, "ExecucaoComandos/AlterarStatusCTeAutorizado", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso.");
                    LimparCamposExecucao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function executarAlterarStatusCTeInutilizacaoRejeitadoClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente executar essa ação?", function () {
        Salvar(_execucaoComandos, "ExecucaoComandos/AlterarStatusCTeEmInutilizacaoRejeitado", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso.");
                    LimparCamposExecucao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function executarReverterAnulacaoGerencialCTeClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente executar essa ação?", function () {
        Salvar(_execucaoComandos, "ExecucaoComandos/ReverterAnulacaoGerencialCTe", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso.");
                    LimparCamposExecucao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function executarCancelarCargaNovaNaoFechadaClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente executar essa ação?", function () {
        Salvar(_execucaoComandos, "ExecucaoComandos/CancelarCargaNovaNaoFechada", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso.");
                    LimparCamposExecucao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function executarExecutarScriptPreCadastradoClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente executar essa ação?", function () {
        Salvar(_execucaoComandos, "ExecucaoComandos/ExecutarScriptPreCadastrado", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso.");
                    LimparCamposExecucao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function LimparCamposExecucao() {
    LimparCampos(_execucaoComandos);
}