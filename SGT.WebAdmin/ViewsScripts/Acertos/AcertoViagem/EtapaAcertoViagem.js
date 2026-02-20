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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="CabecalhoAcertoViagem.js" />
/// <reference path="PedagioAcertoViagem.js" />
/// <reference path="CargaAcertoViagem.js" />
/// <reference path="DespesaAcertoViagem.js" />
/// <reference path="AbastecimentoAcertoViagem.js" />
/// <reference path="FechamentoAcertoViagem.js" />
/// <reference path="DiariaAcertoViagem.js" />
/// <reference path="AcertoViagem.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaAcertoViagem;
var _etapaAtual;

var EtapaAcertoViagem = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("14%") });

    this.Etapa1 = PropertyEntity({
        text: "Acerto", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaAcertoClick,
        step: ko.observable(1),
        tooltip: ko.observable("É onde se informar os dados iniciais para iniciar o acerto de viagem."),
        tooltipTitle: ko.observable("Acerto")
    });
    this.Etapa2 = PropertyEntity({
        text: "Cargas", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaCargaClick,
        step: ko.observable(2),
        tooltip: ko.observable("É onde são selecionados todas as cargas que foram realizadas pelos veículos e motorista."),
        tooltipTitle: ko.observable("Cargas")
    });
    this.Etapa21 = PropertyEntity({
        text: "Ocorrências", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaOcorrenciaClick,
        step: ko.observable(3),
        tooltip: ko.observable("É onde são selecionados todas as ocorrências que foram realizadas pelos veículos e motorista."),
        tooltipTitle: ko.observable("Ocorrências")
    });
    this.Etapa3 = PropertyEntity({
        text: "Abastecimentos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaAbastecimentoClick,
        step: ko.observable(4),
        tooltip: ko.observable("Esta etapa é referente aos abastecimentos que serão lançados para o acerto de viagem."),
        tooltipTitle: ko.observable("Abastecimentos")
    });
    this.Etapa4 = PropertyEntity({
        text: "Pedágios", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaPedagioClick,
        step: ko.observable(5),
        tooltip: ko.observable("A etapa 4 é são selecionados os pedágios."),
        tooltipTitle: ko.observable("Pedágios")
    });
    this.Etapa5 = PropertyEntity({
        text: "Outras Despesas", type: types.local, enable: ko.observable(true), visible: ko.observable(true), eventClick: etapaOutrasDespesaClick, idGrid: guid(), idTab: guid(),
        step: ko.observable(6),
        tooltip: ko.observable("É onde são lançadas as outras despesas que ocorreram durante a viagem."),
        tooltipTitle: ko.observable("Outras Despesas")
    });
    this.Etapa51 = PropertyEntity({
        text: "Diárias", type: types.local, enable: ko.observable(true), visible: ko.observable(false), eventClick: etapaDiariaClick, idGrid: guid(), idTab: guid(),
        step: ko.observable(7),
        tooltip: ko.observable("É onde são lançadas as diárias a serem pagas ao motorista."),
        tooltipTitle: ko.observable("Diárias")
    });
    this.Etapa6 = PropertyEntity({
        text: "Fechamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), eventClick: etapaFechamentoClick, idGrid: guid(), idTab: guid(),
        step: ko.observable(8),
        tooltip: ko.observable("Conferencia final do acerto de viagem, contendo todas as receitas e despesas da mesma."),
        tooltipTitle: ko.observable("Fechamento")
    });
}


//*******EVENTOS*******

function loadEtapaAcertoViagem(callback) {
    _etapaAcertoViagem = new EtapaAcertoViagem();
    KoBindings(_etapaAcertoViagem, "knockoutEtapaAcerto");

    if (_CONFIGURACAO_TMS.AcertoDeViagemComDiaria) {
        _etapaAcertoViagem.Etapa51.visible(true);
        _etapaAcertoViagem.TamanhoEtapa.val("12%");
    }

    $("#" + _etapaAcertoViagem.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAcertoViagem.Etapa1.idTab + " .step").attr("class", "step lightgreen");
    Global.ExibirStep(_etapaAcertoViagem.Etapa1.idTab);    
}

function etapaAcertoClick(e, sender) {
    if (!ValidarCargaNovaLancada()) {
        exibirConfirmacao("Confirmação", "Não foi salvo a(s) carga(s) lançadas, deseja retornar e salvar?", function () {
            Global.ExibirStep(_etapaAcertoViagem.Etapa2.idTab);
            etapaCargaClick(e, sender);
            _etapaAtual = 2;
            return;
        }, function () {
            _etapaAtual = 1;
            return;
        });
    }
    if (!ValidarPedagioNovoLancado()) {
        exibirConfirmacao("Confirmação", "Não foi salvo o(s) pedágio(s) lançados, deseja retornar e salvar?", function () {
            Global.ExibirStep(_etapaAcertoViagem.Etapa4.idTab);
            etapaPedagioClick(e, sender);
            _etapaAtual = 4;
            return;
        }, function () {
            _etapaAtual = 1;
            return;
        });
    }
    _etapaAtual = 1;
    PosicionarCorEtapaAtual();
}

function etapaCargaClick(e, sender) {
    if (!ValidarPedagioNovoLancado()) {
        exibirConfirmacao("Confirmação", "Não foi salvo o(s) pedágio(s) lançados, deseja retornar e salvar?", function () {
            Global.ExibirStep(_etapaAcertoViagem.Etapa4.idTab);
            etapaPedagioClick(e, sender);
            _etapaAtual = 4;
            return;
        }, function () {
            _etapaAtual = 2;
            return;
        });
    }
    _etapaAtual = 2;
    PosicionarCorEtapaAtual();
}

function etapaOcorrenciaClick(e, sender) {
    if (!ValidarCargaNovaLancada()) {
        exibirConfirmacao("Confirmação", "Não foi salvo a(s) carga(s) lançadas, deseja retornar e salvar?", function () {
            Global.ExibirStep(_etapaAcertoViagem.Etapa2.idTab);
            etapaCargaClick(e, sender);
            _etapaAtual = 2;
            return;
        }, function () {
            _etapaAtual = 21;
            CarregarOcorrenciasAcerto();
            return;
        });
    }
    if (!ValidarPedagioNovoLancado()) {
        exibirConfirmacao("Confirmação", "Não foi salvo o(s) pedágio(s) lançados, deseja retornar e salvar?", function () {
            Global.ExibirStep(_etapaAcertoViagem.Etapa4.idTab);
            etapaPedagioClick(e, sender);
            _etapaAtual = 4;
            return;
        }, function () {
            _etapaAtual = 21;
            CarregarOcorrenciasAcerto();
            return;
        });
    }
    _etapaAtual = 21;
    PosicionarCorEtapaAtual();
}

function etapaAbastecimentoClick(e, sender) {
    if (!ValidarCargaNovaLancada()) {
        exibirConfirmacao("Confirmação", "Não foi salvo a(s) carga(s) lançadas, deseja retornar e salvar?", function () {
            Global.ExibirStep(_etapaAcertoViagem.Etapa2.idTab);
            etapaCargaClick(e, sender);
            _etapaAtual = 2;
            return;
        }, function () {
            _etapaAtual = 3;
            return;
        });
    }
    if (!ValidarPedagioNovoLancado()) {
        exibirConfirmacao("Confirmação", "Não foi salvo o(s) pedágio(s) lançados, deseja retornar e salvar?", function () {
            Global.ExibirStep(_etapaAcertoViagem.Etapa4.idTab);
            etapaPedagioClick(e, sender);
            _etapaAtual = 4;
            return;
        }, function () {
            _etapaAtual = 3;
            return;
        });
    }
    _etapaAtual = 3;
    PosicionarCorEtapaAtual();
}

function etapaPedagioClick(e, sender) {
    if (!ValidarCargaNovaLancada()) {
        exibirConfirmacao("Confirmação", "Não foi salvo a(s) carga(s) lançadas, deseja retornar e salvar?", function () {
            Global.ExibirStep(_etapaAcertoViagem.Etapa2.idTab);
            etapaCargaClick(e, sender);
            _etapaAtual = 2;
            return;
        }, function () {
            _etapaAtual = 4;
            return;
        });
    }
    _etapaAtual = 4;
    PosicionarCorEtapaAtual();
}

function etapaOutrasDespesaClick(e, sender) {
    if (!ValidarCargaNovaLancada()) {
        exibirConfirmacao("Confirmação", "Não foi salvo a(s) carga(s) lançadas, deseja retornar e salvar?", function () {
            Global.ExibirStep(_etapaAcertoViagem.Etapa2.idTab);
            etapaCargaClick(e, sender);
            _etapaAtual = 2;
            return;
        }, function () {
            _etapaAtual = 5;
            return;
        });
    }
    if (!ValidarPedagioNovoLancado()) {
        exibirConfirmacao("Confirmação", "Não foi salvo o(s) pedágio(s) lançados, deseja retornar e salvar?", function () {
            Global.ExibirStep(_etapaAcertoViagem.Etapa4.idTab);
            etapaPedagioClick(e, sender);
            _etapaAtual = 4;
            return;
        }, function () {
            _etapaAtual = 5;
            return;
        });
    }
    _etapaAtual = 5;
    PosicionarCorEtapaAtual();
}

function etapaDiariaClick(e, sender) {
    if (!ValidarCargaNovaLancada()) {
        exibirConfirmacao("Confirmação", "Não foi salvo a(s) carga(s) lançadas, deseja retornar e salvar?", function () {
            Global.ExibirStep(_etapaAcertoViagem.Etapa2.idTab);
            etapaCargaClick(e, sender);
            _etapaAtual = 2;
            return;
        }, function () {
            _etapaAtual = 51;
            return;
        });
    }
    if (!ValidarPedagioNovoLancado()) {
        exibirConfirmacao("Confirmação", "Não foi salvo o(s) pedágio(s) lançados, deseja retornar e salvar?", function () {
            Global.ExibirStep(_etapaAcertoViagem.Etapa4.idTab);
            etapaPedagioClick(e, sender);
            _etapaAtual = 5;
            return;
        }, function () {
            _etapaAtual = 51;
            return;
        });
    }
    _etapaAtual = 51;
    if (_gridDiaria !== null && _gridDiaria !== undefined && _diariaAcertoViagem !== null && _diariaAcertoViagem !== undefined) {
        _diariaAcertoViagem.CodigoAcerto.val(_acertoViagem.Codigo.val());
        _gridDiaria.CarregarGrid();
    }
    PosicionarCorEtapaAtual();
}


function etapaFechamentoClick(e, sender) {
    if (!ValidarCargaNovaLancada()) {
        exibirConfirmacao("Confirmação", "Não foi salvo a(s) carga(s) lançadas, deseja retornar e salvar?", function () {
            Global.ExibirStep(_etapaAcertoViagem.Etapa2.idTab);
            etapaCargaClick(e, sender);
            _etapaAtual = 2;
            return;
        }, function () {
            _etapaAtual = 6;
            return;
        });
    }
    if (!ValidarPedagioNovoLancado()) {
        exibirConfirmacao("Confirmação", "Não foi salvo o(s) pedágio(s) lançado(s), deseja retornar e salvar?", function () {
            Global.ExibirStep(_etapaAcertoViagem.Etapa4.idTab);
            etapaPedagioClick(e, sender);
            _etapaAtual = 4;
            return;
        }, function () {
            _etapaAtual = 6;
            return;
        });
    }
    _etapaAtual = 6;
    CarregarDadosFechamentoAcerto();
    PosicionarCorEtapaAtual();
}

function ValidarCargaNovaLancada() {
    if (_gridCargas !== null && _gridCargas !== undefined) {
        var cargaGrid = _gridCargas.BuscarRegistros();

        for (var i = 0; i < cargaGrid.length; i++) {
            if (cargaGrid[i].CodigoAcertoCarga <= 0) {
                return false;
                break;
            }
        }
        return true;
    }
    return true;
}

function ValidarPedagioNovoLancado() {
    if (_gridPedagios !== null && _gridPedagios !== undefined) {
        var pedagioGrid = _gridPedagios.BuscarRegistros();

        for (var i = 0; i < pedagioGrid.length; i++) {
            if (pedagioGrid[i].CodigoAcertoPedagio <= 0) {
                return false;
                break;
            }
        }
        return true;
    }
    return true;
}

function SalvarObservacaoAcertoClick(e, sender) {
    if (_acertoViagem === null || _acertoViagem.Codigo === null || _acertoViagem.Codigo.val() === null || _acertoViagem.Codigo.val() === 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    preencherListasSelecao();
    Salvar(_acertoViagem, "AcertoViagem/SalvarObservacaoAcerto", function (arg) {
        if (arg.Success) {
            CarregarDadosAcertoViagem(arg.Data.Codigo, null, EnumEtapaAcertoViagem.Acerto);
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Observação salva com sucesso.");
        } else {
            exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function CancelarAcertoClick(e, sender) {
    if (_acertoViagem === null || _acertoViagem.Codigo === null || _acertoViagem.Codigo.val() === null || _acertoViagem.Codigo.val() === 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    preencherListasSelecao();
    exibirConfirmacao("Confirmação", "Deseja realmente cancelar este acerto de viagem?", function () {
        Salvar(_acertoViagem, "AcertoViagem/CancelarAcerto", function (arg) {
            if (arg.Success) {
                CarregarDadosAcertoViagem(arg.Data.Codigo, null, EnumEtapaAcertoViagem.Acerto);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Acerto de viagem cancelado com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    });
}

function iniciarAcertoClick(e, sender) {
    if (_acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    preencherListasSelecao();

    Salvar(_acertoViagem, "AcertoViagem/ValidarNovoAcerto", function (arg) {
        if (arg.Success) {
            if (!arg.Data.ContemAcertoEmAberto)
                IniciarAcertoViagem(e, sender);
            else {
                exibirConfirmacao("Confirmação", "Já existe um acerto de viagem em andamento para este mesmo motorista, deseja continuar?", function () {
                    IniciarAcertoViagem(e, sender);
                });
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function IniciarAcertoViagem(e, sender) {
    Salvar(_acertoViagem, "AcertoViagem/Iniciar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Primeira etapa realizada com sucesso.");

                CarregarDadosAcertoViagem(arg.Data.Codigo, null, EnumEtapaAcertoViagem.Todas);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

//*******MÉTODOS*******

function LimparOcultarAbas() {
    $("#" + _etapaAcertoViagem.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAcertoViagem.Etapa1.idTab + " .step").attr("class", "step");

    $("#" + _etapaAcertoViagem.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAcertoViagem.Etapa2.idTab + " .step").attr("class", "step");

    $("#" + _etapaAcertoViagem.Etapa21.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAcertoViagem.Etapa21.idTab + " .step").attr("class", "step");

    $("#" + _etapaAcertoViagem.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAcertoViagem.Etapa3.idTab + " .step").attr("class", "step");

    $("#" + _etapaAcertoViagem.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAcertoViagem.Etapa4.idTab + " .step").attr("class", "step");

    $("#" + _etapaAcertoViagem.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAcertoViagem.Etapa5.idTab + " .step").attr("class", "step");

    $("#" + _etapaAcertoViagem.Etapa51.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAcertoViagem.Etapa51.idTab + " .step").attr("class", "step");

    $("#" + _etapaAcertoViagem.Etapa6.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAcertoViagem.Etapa6.idTab + " .step").attr("class", "step");
}

function PosicionarCorEtapaAtual() {
    if (_acertoViagem.Codigo.val() > 0 && _acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.EmAndamento) {

        $("#" + _etapaAcertoViagem.Etapa6.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa6.idTab + " .step").attr("class", "step lightgreen");

        $("#" + _etapaAcertoViagem.Etapa1.idTab + " .step").attr("class", "step green");

        if (!_acertoViagem.CargaSalvo.val())
            $("#" + _etapaAcertoViagem.Etapa2.idTab + " .step").attr("class", "step");
        else
            $("#" + _etapaAcertoViagem.Etapa2.idTab + " .step").attr("class", "step green");

        if (!_acertoViagem.OcorrenciaSalvo.val())//
            $("#" + _etapaAcertoViagem.Etapa21.idTab + " .step").attr("class", "step");
        else
            $("#" + _etapaAcertoViagem.Etapa21.idTab + " .step").attr("class", "step green");

        if (!_acertoViagem.AbastecimentoSalvo.val())
            $("#" + _etapaAcertoViagem.Etapa3.idTab + " .step").attr("class", "step");
        else {
            if (_acertoViagem.AprovacaoAbastecimento.val())
                $("#" + _etapaAcertoViagem.Etapa3.idTab + " .step").attr("class", "step green");
            else
                $("#" + _etapaAcertoViagem.Etapa3.idTab + " .step").attr("class", "step yellow");
        }

        if (!_acertoViagem.PedagioSalvo.val())
            $("#" + _etapaAcertoViagem.Etapa4.idTab + " .step").attr("class", "step");
        else {
            if (_acertoViagem.AprovacaoPedagio.val())
                $("#" + _etapaAcertoViagem.Etapa4.idTab + " .step").attr("class", "step green");
            else
                $("#" + _etapaAcertoViagem.Etapa4.idTab + " .step").attr("class", "step yellow");
        }

        if (!_acertoViagem.DespesaSalvo.val())
            $("#" + _etapaAcertoViagem.Etapa5.idTab + " .step").attr("class", "step");
        else
            $("#" + _etapaAcertoViagem.Etapa5.idTab + " .step").attr("class", "step green");

        if (_CONFIGURACAO_TMS.AcertoDeViagemComDiaria) {
            if (!_acertoViagem.DiariaSalvo.val())//
                $("#" + _etapaAcertoViagem.Etapa51.idTab + " .step").attr("class", "step");
            else
                $("#" + _etapaAcertoViagem.Etapa51.idTab + " .step").attr("class", "step green");
        }

        if (_etapaAtual == 1) {
            $("#" + _etapaAcertoViagem.Etapa1.idTab).attr("data-bs-toggle", "tab");
            $("#" + _etapaAcertoViagem.Etapa1.idTab + " .step").attr("class", "step blue");
        }
        else if (_etapaAtual == 2) {
            $("#" + _etapaAcertoViagem.Etapa2.idTab).attr("data-bs-toggle", "tab");
            $("#" + _etapaAcertoViagem.Etapa2.idTab + " .step").attr("class", "step blue");
        }
        else if (_etapaAtual == 21) {
            $("#" + _etapaAcertoViagem.Etapa21.idTab).attr("data-bs-toggle", "tab");
            $("#" + _etapaAcertoViagem.Etapa21.idTab + " .step").attr("class", "step blue");
        }
        else if (_etapaAtual == 3) {
            $("#" + _etapaAcertoViagem.Etapa3.idTab).attr("data-bs-toggle", "tab");
            $("#" + _etapaAcertoViagem.Etapa3.idTab + " .step").attr("class", "step blue");
        }
        else if (_etapaAtual == 4) {
            $("#" + _etapaAcertoViagem.Etapa4.idTab).attr("data-bs-toggle", "tab");
            $("#" + _etapaAcertoViagem.Etapa4.idTab + " .step").attr("class", "step blue");
        }
        else if (_etapaAtual == 5) {
            $("#" + _etapaAcertoViagem.Etapa5.idTab).attr("data-bs-toggle", "tab");
            $("#" + _etapaAcertoViagem.Etapa5.idTab + " .step").attr("class", "step blue");
        }
        else if (_etapaAtual == 51) {
            $("#" + _etapaAcertoViagem.Etapa51.idTab).attr("data-bs-toggle", "tab");
            $("#" + _etapaAcertoViagem.Etapa51.idTab + " .step").attr("class", "step blue");
        }
        else if (_etapaAtual == 6) {
            $("#" + _etapaAcertoViagem.Etapa6.idTab).attr("data-bs-toggle", "tab");
            $("#" + _etapaAcertoViagem.Etapa6.idTab + " .step").attr("class", "step blue");            
        }
    }

}

function PosicionarEtapa(dado) {
    LimparOcultarAbas();

    if (dado == null || dado == undefined)
        return;

    if (dado.Situacao === EnumSituacoesAcertoViagem.EmAndamento) {
        $("#" + _etapaAcertoViagem.Etapa1.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa1.idTab + " .step").attr("class", "step green");
        $("#" + _etapaAcertoViagem.Etapa2.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa21.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa3.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa4.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa5.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa51.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa6.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa6.idTab + " .step").attr("class", "step lightgreen");

        if (!dado.CargaSalvo)
            $("#" + _etapaAcertoViagem.Etapa2.idTab + " .step").attr("class", "step");
        else
            $("#" + _etapaAcertoViagem.Etapa2.idTab + " .step").attr("class", "step green");

        if (!dado.OcorrenciaSalvo)//
            $("#" + _etapaAcertoViagem.Etapa21.idTab + " .step").attr("class", "step");
        else
            $("#" + _etapaAcertoViagem.Etapa21.idTab + " .step").attr("class", "step green");

        if (!dado.AbastecimentoSalvo)
            $("#" + _etapaAcertoViagem.Etapa3.idTab + " .step").attr("class", "step");
        else {
            if (dado.AprovacaoAbastecimento)
                $("#" + _etapaAcertoViagem.Etapa3.idTab + " .step").attr("class", "step green");
            else
                $("#" + _etapaAcertoViagem.Etapa3.idTab + " .step").attr("class", "step yellow");
        }

        if (!dado.PedagioSalvo)
            $("#" + _etapaAcertoViagem.Etapa4.idTab + " .step").attr("class", "step");
        else {
            if (dado.AprovacaoPedagio)
                $("#" + _etapaAcertoViagem.Etapa4.idTab + " .step").attr("class", "step green");
            else
                $("#" + _etapaAcertoViagem.Etapa4.idTab + " .step").attr("class", "step yellow");
        }

        if (!dado.DespesaSalvo)
            $("#" + _etapaAcertoViagem.Etapa5.idTab + " .step").attr("class", "step");
        else
            $("#" + _etapaAcertoViagem.Etapa5.idTab + " .step").attr("class", "step green");

        if (_CONFIGURACAO_TMS.AcertoDeViagemComDiaria) {
            if (!dado.DiariaSalvo)//
                $("#" + _etapaAcertoViagem.Etapa51.idTab + " .step").attr("class", "step");
            else
                $("#" + _etapaAcertoViagem.Etapa51.idTab + " .step").attr("class", "step green");
        }

        if (!dado.CargaSalvo) {
            Global.ExibirStep(_etapaAcertoViagem.Etapa2.idTab);
            etapaCargaClick();
        }
        else if (!dado.OcorrenciaSalvo) {
            Global.ExibirStep(_etapaAcertoViagem.Etapa21.idTab);
            etapaOcorrenciaClick();
        }
        else if (!dado.AbastecimentoSalvo) {
            Global.ExibirStep(_etapaAcertoViagem.Etapa3.idTab);
            etapaAbastecimentoClick();
        }
        else if (!dado.PedagioSalvo) {
            Global.ExibirStep(_etapaAcertoViagem.Etapa4.idTab);
            etapaPedagioClick();
        }
        else if (!dado.DespesaSalvo) {
            Global.ExibirStep(_etapaAcertoViagem.Etapa5.idTab);
            etapaOutrasDespesaClick();
        }
        else if (!dado.DiariaSalvo && _CONFIGURACAO_TMS.AcertoDeViagemComDiaria) {
            Global.ExibirStep(_etapaAcertoViagem.Etapa51.idTab);
            etapaDiariaClick();
        }
        else {
            Global.ExibirStep(_etapaAcertoViagem.Etapa6.idTab);
            etapaFechamentoClick();
        }

        VerificaVisibilidadeBotoesFechamento(true, false);
    } else if (dado.Situacao === EnumSituacoesAcertoViagem.Cancelado) {
        $("#" + _etapaAcertoViagem.Etapa1.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaAcertoViagem.Etapa2.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa2.idTab + " .step").attr("class", "step green");

        $("#" + _etapaAcertoViagem.Etapa21.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa21.idTab + " .step").attr("class", "step green");

        $("#" + _etapaAcertoViagem.Etapa3.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa3.idTab + " .step").attr("class", "step green");

        $("#" + _etapaAcertoViagem.Etapa4.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa4.idTab + " .step").attr("class", "step green");

        $("#" + _etapaAcertoViagem.Etapa5.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa5.idTab + " .step").attr("class", "step green");

        $("#" + _etapaAcertoViagem.Etapa51.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa51.idTab + " .step").attr("class", "step green");

        $("#" + _etapaAcertoViagem.Etapa6.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa6.idTab + " .step").attr("class", "step green");
                
        Global.ExibirStep(_etapaAcertoViagem.Etapa1.idTab);
        etapaAcertoClick();

        _fechamentoAcertoViagem.RetornarOutraDespesa.text("Retornar Outras Despesas");
        _fechamentoAcertoViagem.VisualizarDetalhe.text("Visualizar Detalhes");
        VerificaVisibilidadeBotoesFechamento(true, false);

        _fechamentoAcertoViagem.Adiantamentos.enable(false);
        _fechamentoAcertoViagem.Cheques.enable(false);
        _fechamentoAcertoViagem.Folgas.enable(false);
        _fechamentoAcertoViagem.FinalizarAcerto.enable(false);
        _fechamentoAcertoViagem.VisualizarDetalhe.enable(false);
        _fechamentoAcertoViagem.RetornarOutraDespesa.enable(false);
        _fechamentoAcertoViagem.VisualizarRecibo.enable(false);
        _fechamentoAcertoViagem.VeiculosFechamento.enable(false);
        _fechamentoAcertoViagem.VisualizarRecibo.visible(_CONFIGURACAO_TMS.AcertoDeViagemComDiaria);
        _fechamentoAcertoViagem.MoedaCotacaoBancoCentral.enable(false);
        _fechamentoAcertoViagem.ValorDevolucao.enable(false);
        _fechamentoAcertoViagem.DataBaseCRT.enable(false);
        _fechamentoAcertoViagem.ValorMoedaCotacao.enable(false);
        _fechamentoAcertoViagem.ValorOriginal.enable(false);
        _fechamentoAcertoViagem.Justificativa.enable(false);
        _fechamentoAcertoViagem.AdicionarDevolucao.enable(false);
        _fechamentoAcertoViagem.FormaRecebimentoMotoristaAcerto.enable(false);
        _fechamentoAcertoViagem.DataVencimentoMotoristaAcerto.enable(false);
        _fechamentoAcertoViagem.ObservacaoMotoristaAcerto.enable(false);
        _fechamentoAcertoViagem.TipoMovimentoMotoristaAcerto.enable(false);
        _fechamentoAcertoViagem.ObservacaoAcertoMotorista.enable(false);
        _fechamentoAcertoViagem.Titulo.enable(false);
        _fechamentoAcertoViagem.Banco.enable(false);

        if (_detalheSaldoMoedaEstrangeira != undefined && _detalheSaldoMoedaEstrangeira != null) {
            _detalheSaldoMoedaEstrangeira.MoedaCotacaoBancoCentralOrigem.enable(false);
            _detalheSaldoMoedaEstrangeira.ValorOrigem.enable(false);
            _detalheSaldoMoedaEstrangeira.MoedaCotacaoBancoCentralDestino.enable(false);
            _detalheSaldoMoedaEstrangeira.ValorCotacao.enable(false);
            _detalheSaldoMoedaEstrangeira.ValorFinal.enable(false);
            _detalheSaldoMoedaEstrangeira.Converter.enable(false);
        }
        CarregarDadosFechamentoAcerto();
        VerificarBotoes();
    }
    else {
        $("#" + _etapaAcertoViagem.Etapa1.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaAcertoViagem.Etapa2.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa2.idTab + " .step").attr("class", "step green");

        $("#" + _etapaAcertoViagem.Etapa21.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa21.idTab + " .step").attr("class", "step green");

        $("#" + _etapaAcertoViagem.Etapa3.idTab).attr("data-bs-toggle", "tab");
        if (dado.AprovacaoAbastecimento)
            $("#" + _etapaAcertoViagem.Etapa3.idTab + " .step").attr("class", "step green");
        else
            $("#" + _etapaAcertoViagem.Etapa3.idTab + " .step").attr("class", "step yellow");

        $("#" + _etapaAcertoViagem.Etapa4.idTab).attr("data-bs-toggle", "tab");
        if (dado.AprovacaoPedagio)
            $("#" + _etapaAcertoViagem.Etapa4.idTab + " .step").attr("class", "step green");
        else
            $("#" + _etapaAcertoViagem.Etapa4.idTab + " .step").attr("class", "step yellow");

        $("#" + _etapaAcertoViagem.Etapa5.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa5.idTab + " .step").attr("class", "step green");

        $("#" + _etapaAcertoViagem.Etapa51.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa51.idTab + " .step").attr("class", "step green");

        $("#" + _etapaAcertoViagem.Etapa6.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaAcertoViagem.Etapa6.idTab + " .step").attr("class", "step green");        

        if (_acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.Fechado) {
            _fechamentoAcertoViagem.RetornarOutraDespesa.text("Reabrir Acerto");
            _fechamentoAcertoViagem.VisualizarDetalhe.text("Imprimir");
            if (_CONFIGURACAO_TMS.AcertoDeViagemComDiaria) {
                _fechamentoAcertoViagem.VisualizarRecibo.enable(true);
                _fechamentoAcertoViagem.VisualizarRecibo.visible(true);
            }
            _fechamentoAcertoViagem.VeiculosFechamento.enable(false);
            VerificaVisibilidadeBotoesFechamento(false, true);
        } else {
            _fechamentoAcertoViagem.RetornarOutraDespesa.text("Retornar Outras Despesas");
            _fechamentoAcertoViagem.VisualizarDetalhe.text("Visualizar Detalhes");
            VerificaVisibilidadeBotoesFechamento(true, false);
            _fechamentoAcertoViagem.VeiculosFechamento.enable(true);
            _fechamentoAcertoViagem.VisualizarRecibo.visible(false);
        }
        CarregarDadosFechamentoAcerto();
        VerificarBotoes();
    }
}

function VerificarBotoes() {
    if (_acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.EmAndamento && !_FormularioSomenteLeitura && _acertoViagem.Codigo.val() > 0) {
        HabilitarTodosBotoes(true);
    } else {
        HabilitarTodosBotoes(false);
    }
}

function HabilitarTodosBotoes(v) {
    if (_FormularioSomenteLeitura)
        v = false;

    if (_acertoViagem.Codigo.val() <= 0 && v === false && !_FormularioSomenteLeitura) {
        _acertoViagem.IniciarAcerto.enable(true);
        _acertoViagem.Motorista.enable(true);
        _acertoViagem.SegmentoVeiculo.enable(true);
        _acertoViagem.NumeroFrota.enable(true);
        _acertoViagem.DataInicial.enable(true);
        _acertoViagem.DataFinal.enable(true);
        _acertoViagem.DataHoraInicial.enable(true);
        _acertoViagem.DataHoraFinal.enable(true);
        _acertoViagem.Observacao.enable(true);
        _acertoViagem.CancelarAcerto.enable(true);
        _acertoViagem.SalvarObservacaoAcerto.enable(true);
    } else {
        _acertoViagem.IniciarAcerto.enable(v);
        _acertoViagem.Motorista.enable(v);
        _acertoViagem.SegmentoVeiculo.enable(v);
        _acertoViagem.NumeroFrota.enable(v);
        _acertoViagem.DataInicial.enable(v);
        _acertoViagem.DataFinal.enable(v);
        _acertoViagem.DataHoraInicial.enable(v);
        _acertoViagem.DataHoraFinal.enable(v);
        _acertoViagem.Observacao.enable(v);
        _acertoViagem.CancelarAcerto.enable(v);
        _acertoViagem.SalvarObservacaoAcerto.enable(v);
    }

    if (_pedagioAcertoViagem !== null && _pedagioAcertoViagem !== undefined) {
        _pedagioAcertoViagem.AdicionarPedagio.enable(v);
        _pedagioAcertoViagem.RemoverPegadios.enable(v);
        if (_acertoViagem.AprovacaoPedagio.val() === true)
            _pedagioAcertoViagem.Autorizar.enable(false);
        else
            _pedagioAcertoViagem.Autorizar.enable(v);
        _pedagioAcertoViagem.Pedagios.enable(v);
        _pedagioAcertoViagem.RetornarAbastecimento.enable(v);
        _pedagioAcertoViagem.InformarOutrasDespesas.enable(v);

        _pedagioAcertoViagem.PedagiosCredito.enable(v);
        _pedagioAcertoViagem.AdicionarPedagioCredito.enable(v);
        _pedagioAcertoViagem.RemoverPegadiosCredito.enable(v);
    }

    if (_fechamentoAcertoViagem != null && _fechamentoAcertoViagem != undefined) {
        _fechamentoAcertoViagem.AdicionarDesconto.enable(v);
        _fechamentoAcertoViagem.AdicionarBonificacao.enable(v);
        _fechamentoAcertoViagem.Adiantamentos.enable(v);
        _fechamentoAcertoViagem.Cheques.enable(v);
        _fechamentoAcertoViagem.Folgas.enable(v);
        _fechamentoAcertoViagem.SegmentoVeiculo.enable(v);
        _fechamentoAcertoViagem.Cheque.enable(v);
        _fechamentoAcertoViagem.ValorAdiantamentoComprovado.enable(_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermitirAdicionarAlterarValorComprovadoSaldoViagem, _PermissoesPersonalizadas));
        _fechamentoAcertoViagem.ValorAlimentacaoComprovado.enable(_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermitirAdicionarAlterarValorComprovadoSaldoViagem, _PermissoesPersonalizadas));
        _fechamentoAcertoViagem.MoedaCotacaoBancoCentral.enable(v);
        _fechamentoAcertoViagem.ValorDevolucao.enable(v);
        _fechamentoAcertoViagem.Justificativa.enable(v);
        _fechamentoAcertoViagem.AdicionarDevolucao.enable(v);
        _fechamentoAcertoViagem.DataBaseCRT.enable(v);
        _fechamentoAcertoViagem.ValorMoedaCotacao.enable(v);
        _fechamentoAcertoViagem.ValorOriginal.enable(v);
        _fechamentoAcertoViagem.FormaRecebimentoMotoristaAcerto.enable(v);
        _fechamentoAcertoViagem.DataVencimentoMotoristaAcerto.enable(v);
        _fechamentoAcertoViagem.ObservacaoMotoristaAcerto.enable(v);
        _fechamentoAcertoViagem.TipoMovimentoMotoristaAcerto.enable(v);
        _fechamentoAcertoViagem.AcertoViagemTacografo.enable(v);
        _fechamentoAcertoViagem.ControleTacografo.enable(v);
        _fechamentoAcertoViagem.HouveExcesso.enable(v);
        _fechamentoAcertoViagem.AdicionarTacografo.enable(v);
        _fechamentoAcertoViagem.ObservacaoAcertoMotorista.enable(v);
        _fechamentoAcertoViagem.Titulo.enable(v);
        _fechamentoAcertoViagem.Banco.enable(v);
        SetarEnableCamposKnockout(_novoDesconto, v);
    }

    if (_detalheSaldoMoedaEstrangeira != undefined && _detalheSaldoMoedaEstrangeira != null) {
        _detalheSaldoMoedaEstrangeira.MoedaCotacaoBancoCentralOrigem.enable(v);
        _detalheSaldoMoedaEstrangeira.ValorOrigem.enable(v);
        _detalheSaldoMoedaEstrangeira.MoedaCotacaoBancoCentralDestino.enable(v);
        _detalheSaldoMoedaEstrangeira.ValorCotacao.enable(v);
        _detalheSaldoMoedaEstrangeira.ValorFinal.enable(v);
        _detalheSaldoMoedaEstrangeira.Converter.enable(v);
    }

    if (_despesaAcertoViagem !== null && _despesaAcertoViagem !== undefined) {
        _despesaAcertoViagem.RetornarPedagio.enable(v);
        _despesaAcertoViagem.Fechamento.enable(v);
    }

    if (_despesaDoVeiculo !== null && _despesaDoVeiculo !== undefined) {
        _despesaDoVeiculo.AdicionarDespesa.enable(v);
        _despesaDoVeiculo.DespesasComNota.enable(v);
    }

    if (_diariaAcertoViagem !== null && _diariaAcertoViagem !== undefined) {
        _diariaAcertoViagem.CalcularDiarias.enable(v);
        _diariaAcertoViagem.AdicionarDiaria.enable(v);
        _diariaAcertoViagem.RemoverDiarias.enable(v);
    }

    if (_cargaAcertoViagem !== null && _cargaAcertoViagem !== undefined) {
        _cargaAcertoViagem.Cargas.enable(v);
        _cargaAcertoViagem.BuscarCargas.enable(v);
        _cargaAcertoViagem.RetornarAcerto.enable(v);
        _cargaAcertoViagem.IniciarAbastecimento.enable(v);
    }

    if (_ocorrenciaAcertoViagem !== null && _ocorrenciaAcertoViagem !== undefined) {
        _ocorrenciaAcertoViagem.Ocorrencias.enable(v);
        _ocorrenciaAcertoViagem.BuscarOcorrencias.enable(v);
        _ocorrenciaAcertoViagem.BuscarTodasOcorrencias.enable(v);
        _ocorrenciaAcertoViagem.ProximaEtapa.enable(v);
    }

    if (_detalheAcerto !== null && _detalheAcerto !== undefined) {
        _detalheAcerto.VeiculoReboque.enable(v);
        _detalheAcerto.AdicionarVeiculo.enable(v);
        _detalheAcerto.ValorBonificacaoCliente.enable(false);
        _detalheAcerto.CargaFracionada.enable(v);
        _detalheAcerto.JustificativaBonificacao.enable(v);
        _detalheAcerto.ValorBonificacaoDesconto.enable(v);
        _detalheAcerto.AdicionarBonificacao.enable(v);
    }

    if (_abastecimentoAcertoViagem !== null && _abastecimentoAcertoViagem !== undefined) {
        _abastecimentoAcertoViagem.RetornarCarga.enable(v);
        _abastecimentoAcertoViagem.InformarPedagio.enable(v);
    }

    if (_abastecimentoDoVeiculo !== null && _abastecimentoDoVeiculo !== undefined) {
        _abastecimentoDoVeiculo.Abastecimentos.enable(v);
        _abastecimentoDoVeiculo.AdicionarAbastecimento.enable(v);
        _abastecimentoDoVeiculo.RemoverAbastecimento.enable(v);
        _abastecimentoDoVeiculo.Ideal.enable(v);
        _abastecimentoDoVeiculo.IdealHorimetro.enable(v);
        _abastecimentoDoVeiculo.HorimetroTotalAjustado.enable(v);
        _abastecimentoDoVeiculo.PercentalAjusteHorimetro.enable(v);

        _abastecimentoDoVeiculo.KMTotalAjustado.enable(v);
        _abastecimentoDoVeiculo.PercentalAjusteKM.enable(v);
        if (_acertoViagem.AprovacaoAbastecimento.val() === true)
            _abastecimentoDoVeiculo.Autorizar.enable(false);
        else
            _abastecimentoDoVeiculo.Autorizar.enable(v);
    }

    if (_arlaDoVeiculo !== null && _arlaDoVeiculo !== undefined) {
        _arlaDoVeiculo.Arlas.enable(v);
        _arlaDoVeiculo.AdicionarArla.enable(v);
        _arlaDoVeiculo.Ideal.enable(v);
        if (_acertoViagem.AprovacaoAbastecimento.val() === true)
            _arlaDoVeiculo.Autorizar.enable(false);
        else
            _arlaDoVeiculo.Autorizar.enable(v);
    }
}