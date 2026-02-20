/// <reference path="../../Consultas/DocumentoEntrada.js" />
/// <reference path="../../Consultas/Justificativa.js" />
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
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="CargaAcertoViagem.js" />
/// <reference path="AcertoViagem.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Pedagio.js" />
/// <reference path="../../Enumeradores/EnumTipoFinalidadeJustificativa.js" />
/// <reference path="AbastecimentoAcertoViagem.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="PedagioAcertoViagem.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="EtapaAcertoViagem.js" />
/// <reference path="FechamentoAcertoViagem.js" />
/// <reference path="../../../js/plugin/chartjs/chart.js" />
/// <reference path="OcorrenciaAcertoViagem.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _diariaAcertoViagem;
var _gridDiaria;
var _HTMLDiariaAcertoViagem;
var _novaDiaria;

var DiariaAcertoViagem = function () {
    this.CodigoAcerto = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoDiaria = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.CalcularDiarias = PropertyEntity({ eventClick: CalcularDiariasClick, type: types.event, text: "Cálcular Diária pela Tabela", visible: ko.observable(true), enable: ko.observable(true) });

    this.Diarias = PropertyEntity({ type: types.map, required: false, text: "Diarias", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(true) });

    this.AdicionarDiaria = PropertyEntity({ eventClick: InformarDiariaClick, type: types.event, text: "Adicionar nova diária", visible: ko.observable(true), enable: ko.observable(false) });
    this.RemoverDiarias = PropertyEntity({ eventClick: RemoverDiariasClick, type: types.event, text: "Remover diárias selecionadas", visible: ko.observable(true), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });

    this.RetornarDespesaDiaria = PropertyEntity({ eventClick: RetornarDespesaDiariaClick, type: types.event, text: "Retornar Outras Despesas", visible: ko.observable(false), enable: ko.observable(true) });
    this.FechamentoDiaria = PropertyEntity({ eventClick: FechamentoDiariaClick, type: types.event, text: "Salvar Diarias", visible: ko.observable(true), enable: ko.observable(true) });
}

var AdicionarDiaria = function () {
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), required: true });
    this.Descricao = PropertyEntity({ getType: typesKnockout.string, required: true, maxlength: 300, text: "*Descrição:" });
    this.Data = PropertyEntity({ getType: typesKnockout.date, required: true, text: "*Data:" });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "*Valor:", maxlength: 10, visible: ko.observable(true), configDecimal: { precision: 2, allowZero: false, allowNegative: false } });
    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarDiariaClick, text: "Adicionar", visible: ko.observable(true) });
}


//*******EVENTOS*******

function loadDiariaAcertoViagem() {
    $("#contentDiariaAcertoViagem").html("");
    var idDiv = guid();
    $("#contentDiariaAcertoViagem").append(_HTMLDiariaAcertoViagem.replace(/#diariaAcertoViagem/g, idDiv));
    _diariaAcertoViagem = new DiariaAcertoViagem();
    KoBindings(_diariaAcertoViagem, idDiv);

    _novaDiaria = new AdicionarDiaria();
    KoBindings(_novaDiaria, "knoutAdicionarDiaria");

    new BuscarJustificativas(_novaDiaria.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.AcertoViagemMotorista, EnumTipoFinalidadeJustificativa.AcertoViagemOutrasDespesas]);

    CarregarDiariaAcertoViagem();
}


function CalcularDiariasClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja buscar as diárias a serem pagas?", function () {

        _gridDiaria.CarregarGrid(executarPesquisaGridDiaria);
    });
}

function executarPesquisaGridDiaria(retornoData) {
    setTimeout(function () {
        var limparRegistros = false;
        if (retornoData !== null && retornoData !== undefined) {

            if (retornoData.recordsTotal > 0) {
                setTimeout(function () {
                    exibirConfirmacao("Confirmação", "Deseja limpar as diárias geradas anteriormente?", function () {
                        limparRegistros = true;
                        BuscarDiarias(limparRegistros);
                    }, function () {
                        limparRegistros = false;
                        BuscarDiarias(limparRegistros);
                    });
                }, 1000);
            } else {
                BuscarDiarias(limparRegistros);
            }
        } else {
            BuscarDiarias(limparRegistros);
        }
    }, 100);
}

function BuscarDiarias(limparRegistros) {
    var data = {
        CodigoAcerto: _acertoViagem.Codigo.val(),
        LimparRegistros: limparRegistros,
        DataHoraInicial: _acertoViagem.DataHoraInicial.val(),
        DataHoraFinal: _acertoViagem.DataHoraFinal.val()
    };
    executarReST("AcertoDiaria/BuscarDiaria", data, function (arg) {
        if (arg.Success) {
            _gridDiaria.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    });
}

function CarregarDiariaAcertoViagem() {
    _diariaAcertoViagem.CodigoAcerto.val(_acertoViagem.Codigo.val());

    var editar = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: RemoverDiariaClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: VisibilidadeOpcaoAuditoria() ? TypeOptionMenu.list : TypeOptionMenu.link, opcoes: [editar], descricao: "Opções", tamanho: 10 };

    _diariaAcertoViagem.SelecionarTodos.visible(true);
    _diariaAcertoViagem.SelecionarTodos.val(false);

    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: null,
        callbackNaoSelecionado: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _diariaAcertoViagem.SelecionarTodos,
        somenteLeitura: false
    };

    _gridDiaria = new GridView(_diariaAcertoViagem.Diarias.idGrid, "AcertoDiaria/PesquisarDiarias", _diariaAcertoViagem, menuOpcoes, null, null, null, null, null, multiplaescolha);
    _gridDiaria.CarregarGrid();
    VerificarBotoes();
}

function InformarDiariaClick(e, data) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    LimparCampos(_novaDiaria);    
    Global.abrirModal('divAdicionarDiaria');
}

function AdicionarDiariaClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    if (ValidarCamposObrigatorios(e)) {
        var data = {
            Codigo: _acertoViagem.Codigo.val(),
            Valor: e.Valor.val(),
            Descricao: e.Descricao.val(),
            Justificativa: e.Justificativa.codEntity(),
            Data: e.Data.val()
        };
        executarReST("AcertoDiaria/InserirDiaria", data, function (arg) {
            if (arg.Success) {
                LimparCampos(e);
                $("#" + e.Justificativa.id).focus();

                _diariaAcertoViagem.CodigoAcerto.val(_acertoViagem.Codigo.val());
                _gridDiaria.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function RemoverDiariaClick(e) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a diaria " + e.Descricao + "?", function () {

        var data = {
            Codigo: e.Codigo
        };
        executarReST("AcertoDiaria/RemoverDiaria", data, function (arg) {
            if (arg.Success) {

                _gridDiaria.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    });
}


function RemoverDiariasClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir as diárias selecionadas?", function () {

        var diariasSelecionados = _gridDiaria.ObterMultiplosSelecionados();

        var codigosDiarias = new Array();
        for (var i = 0; i < diariasSelecionados.length; i++)
            codigosDiarias.push(diariasSelecionados[i].Codigo);

        if (codigosDiarias && codigosDiarias.length > 0) {

            var data = {
                Codigos: JSON.stringify(codigosDiarias)
            };
            executarReST("AcertoDiaria/RemoverDiariasSelecionados", data, function (arg) {
                if (arg.Success) {
                    _gridDiaria.AtualizarRegistrosSelecionados([]);
                    _gridDiaria.CarregarGrid();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Diárias removidas com sucesso.");
                } else {
                    exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
                }
            });
        }
        else
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Nenhuma diária selecionado.");

    });

}

function FechamentoDiariaClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    preencherListasSelecao();
    _acertoViagem.Etapa.val(EnumEtapasAcertoViagem.Fechamento);
    Salvar(_acertoViagem, "AcertoDiaria/AtualizarDiaria", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Diarias salvas com sucesso.");

                CarregarDadosAcertoViagem(arg.Data.Codigo, null, EnumEtapaAcertoViagem.Diarias);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function RetornarDespesaDiariaClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    _acertoViagem.Etapa.val(EnumEtapasAcertoViagem.OutrasDespesas);

    $("#" + _etapaAcertoViagem.Etapa51.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAcertoViagem.Etapa51.idTab + " .step").attr("class", "step grey");

    $("#" + _etapaAcertoViagem.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAcertoViagem.Etapa5.idTab + " .step").attr("class", "step lightgreen");
    $("#" + _etapaAcertoViagem.Etapa5.idTab).click();

}

//*******MÉTODOS*******
