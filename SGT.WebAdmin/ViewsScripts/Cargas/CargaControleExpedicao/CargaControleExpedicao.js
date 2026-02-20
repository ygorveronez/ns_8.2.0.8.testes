/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaControleExpedicao.js" />
/// <reference path="ConferenciaSeparacao.js" />
/// <reference path="ConferenciaSeparacaoVolume.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridCargaControleExpedicao;
var _pesquisaCargaControleExpedicao;
var $modalConferenciaSeparacao;

/*
 * Declaração das Classes
 */

var PesquisaCargaControleExpedicao = function () {
    var dataInicioPadrao = moment().add(-1, 'days').format("DD/MM/YYYY");
    var dataFimPadrao = moment().format("DD/MM/YYYY");
    var isTMS = (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS);

    this.CodigoCargaEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.CargaControleExpedicao.NumeroCarga, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: Localization.Resources.Cargas.CargaControleExpedicao.DataFinalCarregamento, val: ko.observable(dataFimPadrao), def: dataFimPadrao, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: Localization.Resources.Cargas.CargaControleExpedicao.DataInicialCarregamento, val: ko.observable(dataInicioPadrao), def: dataInicioPadrao, getType: typesKnockout.date, visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: (isTMS ? "" : "*") + Localization.Resources.Gerais.Geral.Filial, idBtnSearch: guid(), required: !isTMS });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Motorista.getFieldDescription(), idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCargaControleExpedicao.AguardandoLiberacao), options: EnumSituacaoCargaControleExpedicao.obterOpcoesPesquisa(), def: EnumSituacaoCargaControleExpedicao.AguardandoLiberacao, text: Localization.Resources.Cargas.CargaControleExpedicao.SituacaoExpedicao.getFieldDescription()});
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Transportador.getFieldDescription(), idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Veiculo.getFieldDescription(), idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    // Notificação

    this.NotificaoPlaca = PropertyEntity({ type: types.map, val: ko.observable(""), text: Localization.Resources.Gerais.Geral.Placa, required: false });
    this.NotificaoViagem = PropertyEntity({ type: types.map, val: ko.observable(""), text: Localization.Resources.Gerais.Geral.Viagem, required: false });
    this.Notificar = PropertyEntity({ eventClick: notificarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Notificar, visible: ko.observable(false) });

    // Notificação

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaCargaControleExpedicao)) {
                _pesquisaCargaControleExpedicao.Pesquisar.visibleFade(true);
                _gridCargaControleExpedicao.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.AtencaoAcentuado, Localization.Resources.Gerais.Geral.VerifiqueCamposObrigatorios);
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true), visibleFade: ko.observable(false)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            }
            else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadCargaControleExpedicao() {
    _pesquisaCargaControleExpedicao = new PesquisaCargaControleExpedicao();
    KoBindings(_pesquisaCargaControleExpedicao, "knockoutPesquisaCargaControleExpedicao", _pesquisaCargaControleExpedicao.Pesquisar.id);

    new BuscarFilial(_pesquisaCargaControleExpedicao.Filial);
    new BuscarTransportadores(_pesquisaCargaControleExpedicao.Transportador, null, null, true, null);
    new BuscarMotoristas(_pesquisaCargaControleExpedicao.Motorista, null, _pesquisaCargaControleExpedicao.Transportador);
    new BuscarVeiculos(_pesquisaCargaControleExpedicao.Veiculo, null, _pesquisaCargaControleExpedicao.Transportador);

    loadGridCargaControleExpedicao();

    $("#" + _pesquisaCargaControleExpedicao.NotificaoPlaca.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });

    $modalConferenciaSeparacao = $("#divModalConferenciaSeparacao");
    $modalConferenciaSeparacaoVolume = $("#divModalConferenciaSeparacaoVolume");

    loadAnexosCargaControleExpedicao();
    loadFiliaisPadrao();
    loadConferenciaSeparacao();
    loadConferenciaSeparacaoVolume();
}

function loadGridCargaControleExpedicao() {
    var informarInicioCarregamento = { descricao: Localization.Resources.Cargas.CargaControleExpedicao.InfomarInicioCarregamento, id: guid(), evento: "onclick", metodo: informarInicioCarregamentoClick, icone: "", visibilidade: visibilidadeInicioCarregamento };
    var confirmarPlaca = { descricao: Localization.Resources.Gerais.Geral.Confirmar, id: guid(), evento: "onclick", metodo: confirmarPlacaClick, icone: "", visibilidade: VisibilidadeConfirmarPlaca };
    var conferenciaSeparacao = { descricao: Localization.Resources.Cargas.CargaControleExpedicao.ConferenciaSeparacao, id: guid(), evento: "onclick", metodo: EditarConferenciaSeparacaoClick, icone: "", visibilidade: VisibilidadeConferenciaSeparacao };
    var auditar = { descricao: Localization.Resources.Gerais.Geral.Auditoria, id: guid(), evento: "onclick", metodo: OpcaoAuditoria("CargaControleExpedicao", "Codigo"), tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var conferenciaSeparacaoVolume = { descricao: Localization.Resources.Cargas.CargaControleExpedicao.conferenciaSeparacaoVolume, id: guid(), evento: "onclick", metodo: EditarConferenciaSeparacaoVolumeClick, icone: "", visibilidade: VisibilidadeConferenciaSeparacaoVolume };
    var anexoCargaControleExpedicao = { descricao: Localization.Resources.Gerais.Geral.Anexos, id: guid(), evento: "onclick", metodo: gerenciarAnexosCargaControleExpedicaoClick, icone: "" };

    var editarColuna = { permite: true, callback: callbackEditarColuna, atualizarRow: false };
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        tamanho: 7,
        opcoes: [informarInicioCarregamento, auditar, conferenciaSeparacao, conferenciaSeparacaoVolume, anexoCargaControleExpedicao]
    };

    var configExportacao = {
        url: "CargaControleExpedicao/ExportarPesquisa",
        titulo: "Expedição"
    };

    _gridCargaControleExpedicao = new GridView(_pesquisaCargaControleExpedicao.Pesquisar.idGrid, "CargaControleExpedicao/Pesquisa", _pesquisaCargaControleExpedicao, menuOpcoes, { column: 2, dir: orderDir.asc }, 25, null, null, null, null, null, editarColuna, configExportacao);
}

function loadFiliaisPadrao() {
    executarReST("DadosPadrao/ObterFiliais", {}, function (retorno) {
        if (retorno.Success && retorno.Data) {
            _pesquisaCargaControleExpedicao.Filial.multiplesEntities(retorno.Data);
        }
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function confirmarPlacaClick(dataRow, row) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.VoceRealmenteDesejaConfirmarCarregamentoViagem, function () {
        var data = { codigo: dataRow.Codigo };

        if (dataRow.ExigeConfirmacaoPlaca) {
            executarReST("CargaControleExpedicao/ConfirmarDadosExpedicao", data, function (arg) {
                if (arg.Success) {
                    if (arg.Data !== false) {
                        if (arg.Data.Autorizou) {
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ConfirmacaoRealizadaSucesso);
                        } else {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Data.Mensagem);
                        }
                        CompararEAtualizarGridEditableDataRow(dataRow, arg.Data.DadosGrid)
                        _gridCargaControleExpedicao.AtualizarDataRow(row, dataRow);
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });
        }
        else {
            var data = { codigo: dataRow.Codigo };
            executarReST("CargaControleExpedicao/InformarTerminoCarregamento", data, function (arg) {
                if (arg.Success) {
                    if (arg.Data !== false) {
                        if (arg.Data.Autorizou) {
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ConfirmacaoRealizadaSucesso);
                        } else {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Data.Mensagem);
                        }
                        CompararEAtualizarGridEditableDataRow(dataRow, arg.Data.DadosGrid)
                        _gridCargaControleExpedicao.AtualizarDataRow(row, dataRow);
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });
        }
    });
}

function informarInicioCarregamentoClick(dataRow, row) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.VoceRealmenteDesejaInfomarInicioCarregamentoViagem, function () {
        var data = { codigo: dataRow.Codigo };
        executarReST("CargaControleExpedicao/InformarInicioCarregamento", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.Autorizou) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.InicioCarregamentoRealizadoSucesso);
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Data.Mensagem);
                    }
                    CompararEAtualizarGridEditableDataRow(dataRow, arg.Data.DadosGrid)
                    _gridCargaControleExpedicao.AtualizarDataRow(row, dataRow);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function notificarClick() {
    if (ValidaNotificao()) {
        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.VoceRealmenteDesejaNotificar, function () {
            NotificarUsuarios();
        });
    }
    else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.InformePlacaViagem);
    }
}

/*
 * Declaração das Funções
 */

function callbackEditarColuna(dataRow, row, head, callbackTabPress) {
    var data = { codigo: dataRow.Codigo, Placa: dataRow.Placa, Doca: dataRow.Doca };;
    executarReST("CargaControleExpedicao/AlterarDados", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                CompararEAtualizarGridEditableDataRow(dataRow, arg.Data)
                _gridCargaControleExpedicao.AtualizarDataRow(row, dataRow, callbackTabPress);
            } else {
                ExibirErroDataRow(row, arg.Msg, tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso);
            }
        } else {
            ExibirErroDataRow(row, arg.Msg, tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha);
        }
    });
}

function NotificarUsuarios() {
    var data = {
        Placa: _pesquisaCargaControleExpedicao.NotificaoPlaca.val(),
        Viagem: _pesquisaCargaControleExpedicao.NotificaoViagem.val()
    };

    executarReST("CargaControleExpedicao/NotificarUsuarios", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ConfirmacaoRealizadaSucesso);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function ExibirErroDataRow(row, mensagem, tipoMensagem, titulo) {
    _gridCargaControleExpedicao.DesfazerAlteracaoDataRow(row);
    exibirMensagem(tipoMensagem, titulo, mensagem);
}

function ValidaNotificao() {
    var validViagem = ValidarCampoObrigatorioMap(_pesquisaCargaControleExpedicao.NotificaoViagem);
    var validPlaca = ValidarCampoObrigatorioMap(_pesquisaCargaControleExpedicao.NotificaoPlaca) && _pesquisaCargaControleExpedicao.NotificaoPlaca.val().length == 7;

    return (validViagem && validPlaca);
}

function VisibilidadeConferenciaSeparacao(dataRow) {
    return dataRow.PossuiSeparacao;
}

function VisibilidadeConferenciaSeparacaoVolume(dataRow) {
    return dataRow.PossuiSeparacaoVolume;
}

function VisibilidadeConfirmarPlaca() {
    return _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS;
}

function visibilidadeInicioCarregamento(e) {
    return (e.SituacaoCargaControleExpedicao == EnumSituacaoCargaControleExpedicao.AgInicioCarregamento);
}