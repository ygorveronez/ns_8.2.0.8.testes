/// <reference path="../../Enumeradores/EnumDiaSemana.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/MotivoReagendamento.js" />

var _alterarHorarioAgendamentoDescarga;
var _gridPeriodosDisponiveisAgendamentoEntrega;

var AlterarHorarioAgendamentoDescarga = function () {
    this.Codigos = [];
    this.CodigosCargaEntrega = [];
    this.Titulo = PropertyEntity({ text: ko.observable("") });
    this.DatasPrevisoesEntrega = PropertyEntity({ type: types.local, val: [], def: "" });
    this.NovoHorario = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), required: true, text: "*Horário", enable: ko.observable(true) });
    this.Observacoes = PropertyEntity({ getType: typesKnockout.string, maxlength: 300, val: ko.observable(""), required: false, text: "Observação do Horário" });
    this.PeriodosDisponiveis = PropertyEntity({ text: ko.observable("Períodos Disponíveis para"), type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.PeriodoDia = PropertyEntity({ type: types.local, val: ko.observable([]) });
    this.Reagendamento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SugestaoReagendamento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.MotivoReagendamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motivo do Reagendamento", idBtnSearch: guid(), visible: ko.observable(false), required: this.Reagendamento.val });
    this.ResponsavelMotivoReagendamento = PropertyEntity({ options: ko.observable([]), required: this.Reagendamento.val, getType: typesKnockout.options, visible: ko.observable(false), val: ko.observable(0), def: 0, text: "*Responsável do Reagendamento", idBtnSearch: guid(), enable: ko.observable(true) });
    this.ObservacaoReagendamento = PropertyEntity({ getType: typesKnockout.string, maxlength: 150, val: ko.observable(""), required: false, text: "Observação de Reagendamento", visible: ko.observable(false) });

    this.ExigeSenhaAgendamento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SenhaEntregaAgendamento = PropertyEntity({ getType: typesKnockout.string, maxlength: 100, val: ko.observable(""), required: this.ExigeSenhaAgendamento.val, text: "*Senha do Agendamento" });

    this.CodigoCargaEntrega = PropertyEntity({ type: types.int, val: ko.observable(0), def: 0, required: false});
    this.NovoHorario.val.subscribe(function (valor) {
        if (string.IsNullOrWhiteSpace(valor)) {
            _gridPeriodosDisponiveisAgendamentoEntrega.CarregarGrid([]);
            return;
        }

        var data = Global.criarData(valor);
        var dia = data.getDay() + 1;

        _alterarHorarioAgendamentoDescarga.DiaSemana.val(EnumDiaSemana.obterDescricaoSemConfiguracao(dia));

        var listaPeriodos = _alterarHorarioAgendamentoDescarga.PeriodoDia.val().filter(function (p) {
            return p.Dia == dia;
        });

        if (listaPeriodos.length > 0)
            _gridPeriodosDisponiveisAgendamentoEntrega.CarregarGrid(listaPeriodos[0].Periodos);
        else
            _gridPeriodosDisponiveisAgendamentoEntrega.CarregarGrid([]);
    });

    this.DiaSemana = PropertyEntity({ val: ko.observable(EnumDiaSemana.obterDescricaoSemConfiguracao(Global.criarData(Global.DataAtual()).getDay() + 1)), def: EnumDiaSemana.obterDescricaoSemConfiguracao(Global.criarData(Global.DataAtual()).getDay() + 1), getType: typesKnockout.select });

    this.Periodos = PropertyEntity({ type: types.event, eventClick: abrirModalPeriodosAgendamento, text: "Periodos", id: guid() });
    this.Salvar = PropertyEntity({ type: types.event, eventClick: salvarHorarioAgendamentoDescargaClick, text: "Salvar", id: guid() });
}

function abrirModalPeriodosAgendamento() {
    Global.abrirModal('divModalPeriodosAgendamento');
}

function loadAlterarHorarioAgendamentoDescarga() {
    _alterarHorarioAgendamentoDescarga = new AlterarHorarioAgendamentoDescarga();
    KoBindings(_alterarHorarioAgendamentoDescarga, "knockoutHorarioAgendamentoDescarregamento");

    new BuscarClientes(_alterarHorarioAgendamentoDescarga.ResponsavelMotivoReagendamento);
    new BuscarMotivoReagendamento(_alterarHorarioAgendamentoDescarga.MotivoReagendamento, retornoMotivoReagendamento);

    loadGridAgendamentoEntregaPeriodosDisponiveis();
}

function loadGridAgendamentoEntregaPeriodosDisponiveis() {
    var linhasPorPaginas = 5;
    var header = [
        { data: "Inicio", title: "Início", width: "50%", className: "text-align-center" },
        { data: "Fim", title: "Fim", width: "50%", className: "text-align-center" }
    ];

    _gridPeriodosDisponiveisAgendamentoEntrega = new BasicDataTable(_alterarHorarioAgendamentoDescarga.PeriodosDisponiveis.idGrid, header, null, null, null, linhasPorPaginas);
    _gridPeriodosDisponiveisAgendamentoEntrega.CarregarGrid([]);
}

function salvarHorarioAgendamentoDescargaClick() {
    if (!ValidarCamposObrigatorios(_alterarHorarioAgendamentoDescarga)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    if (isDataDescargaAnteriorDataPrevisaoEntrega())
        exibirConfirmacao("Confirmação", "A data agendada é inferior a data prevista da entrega, deseja prosseguir?", function () {
            salvarHorarioAgendamentoDescarga(false)
        });
    else
        salvarHorarioAgendamentoDescarga(false);
}

function salvarHorarioAgendamentoDataRetroativa() {
    salvarHorarioAgendamentoDescarga(true);
}

function salvarHorarioAgendamentoDescarga(salvarComDataRetroativa) {
    var data = {
        Codigos: JSON.stringify(_alterarHorarioAgendamentoDescarga.Codigos),
        Data: _alterarHorarioAgendamentoDescarga.NovoHorario.val(),
        Observacoes: _alterarHorarioAgendamentoDescarga.Observacoes.val(),
        SalvarComDataRetroativa: salvarComDataRetroativa,
        Reagendamento: _alterarHorarioAgendamentoDescarga.Reagendamento.val(),
        ObservacaoReagendamento: _alterarHorarioAgendamentoDescarga.ObservacaoReagendamento.val(),
        ResponsavelMotivoReagendamento: _alterarHorarioAgendamentoDescarga.ResponsavelMotivoReagendamento.val(),
        MotivoReagendamento: _alterarHorarioAgendamentoDescarga.MotivoReagendamento.codEntity(),
        CodigosCargaEntrega: JSON.stringify(_alterarHorarioAgendamentoDescarga.CodigosCargaEntrega),
        CodigoCargaEntrega: _alterarHorarioAgendamentoDescarga.CodigoCargaEntrega.val(),
        ExigeSenhaAgendamento: _alterarHorarioAgendamentoDescarga.ExigeSenhaAgendamento.val(),
        SenhaEntregaAgendamento: _alterarHorarioAgendamentoDescarga.SenhaEntregaAgendamento.val(),
    }

    if (_alterarHorarioAgendamentoDescarga.SugestaoReagendamento.val()) {
        executarReST("AgendamentoEntregaPedido/SalvarSugestaoReagendamento", data, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Sugestão de data de reagendamento salva com sucesso!");
                    Global.fecharModal("divModalHorarioAgendamentoDescarga");
                    _gridAgendamentoEntregaPedido.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });

    } else {
        executarReST("AgendamentoEntregaPedido/AgendarDescarga", data, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {

                    if (retorno.Data.DataRetroativa) {
                        exibirConfirmacao("Confirmação", retorno.Data.Mensagem + " Deseja prosseguir?", salvarHorarioAgendamentoDataRetroativa);
                        return;
                    }

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Data de agendamento salva com sucesso!");
                    Global.fecharModal("divModalHorarioAgendamentoDescarga");
                    _gridAgendamentoEntregaPedido.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function ValidarHorarioAgendado() {
    var data = Global.criarData(_alterarHorarioAgendamentoDescarga.NovoHorario.val());

    return _gridPeriodosDisponiveisAgendamentoEntrega.BuscarRegistros().filter(function (registro) {
        return (new Date(data.getFullYear(), data.getMonth(), data.getDate(), registro.Inicio.substr(0, 2), registro.Inicio.substr(3, 2)) <= data && new Date(data.getFullYear(), data.getMonth(), data.getDate(), registro.Fim.substr(0, 2), registro.Fim.substr(3, 2)) > data);
    }).length > 0;
}

function exibirModalAgendamentoDescarga(codigos, cpfCnpjDestinatario, listaDatasPrevisaoEntrega, codigosCargaEntrega, exigeSenhaAgendamento, senhaEntregaAgendamento) {
    _alterarHorarioAgendamentoDescarga.Codigos = codigos;
    _alterarHorarioAgendamentoDescarga.DatasPrevisoesEntrega.val = listaDatasPrevisaoEntrega;
    _alterarHorarioAgendamentoDescarga.CodigosCargaEntrega = codigosCargaEntrega;
    _alterarHorarioAgendamentoDescarga.ExigeSenhaAgendamento.val(exigeSenhaAgendamento);
    _alterarHorarioAgendamentoDescarga.SenhaEntregaAgendamento.val(senhaEntregaAgendamento);

    buscarResponsaveisEntrega(cpfCnpjDestinatario);
}

function buscarResponsaveisEntrega(cpfCnpjDestinatario) {
    executarReST("TipoResponsavelAtrasoEntrega/BuscarTiposResponsavelAtrasoAtivos", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _alterarHorarioAgendamentoDescarga.ResponsavelMotivoReagendamento.options(arg.Data.TipoResponsavel);
                buscarPeriodosDescarga(cpfCnpjDestinatario);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function buscarPeriodosDescarga(cpfCnpjDestinatario) {
    executarReST("AgendamentoEntregaPedido/BuscarPeriodosDestinatario", { CPFCnpjDestinatario: cpfCnpjDestinatario }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _alterarHorarioAgendamentoDescarga.PeriodoDia.val(retorno.Data.PeriodoDia);

                var listaPeriodos = retorno.Data.PeriodoDia.filter(function (p) {
                    return p.Dia == new Date().getDay() + 1;
                });

                if (listaPeriodos.length > 0)
                    _gridPeriodosDisponiveisAgendamentoEntrega.CarregarGrid(listaPeriodos[0].Periodos);

                $("#divModalHorarioAgendamentoDescarga")
                    .modal("show").on("show.bs.modal", function () {
                    }).on("hidden.bs.modal", function () {
                        LimparCampos(_alterarHorarioAgendamentoDescarga);
                        Global.ResetarAba("divModalHorarioAgendamentoDescarga");
                    });
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);

            _gridAgendamentoEntregaPedido.AtualizarRegistrosNaoSelecionados([]);
            _gridAgendamentoEntregaPedido.AtualizarRegistrosSelecionados([]);
            _gridAgendamentoEntregaPedido.DrawTable(true);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function isDataDescargaAnteriorDataPrevisaoEntrega() {
    if (_alterarHorarioAgendamentoDescarga.DatasPrevisoesEntrega.val.length == 0)
        return false;

    for (var i = 0; i < _alterarHorarioAgendamentoDescarga.DatasPrevisoesEntrega.val.length; i++) {
        var dataDescarga = _alterarHorarioAgendamentoDescarga.NovoHorario.val().split("/");
        var dataPrevisaoEntrega = _alterarHorarioAgendamentoDescarga.DatasPrevisoesEntrega.val[i].split("/");

        var dataDescargaMoment = moment({ year: dataDescarga[2].split(" ")[0], month: dataDescarga[1], day: dataDescarga[0], hour: dataDescarga[2].slice(-5).split(":")[0], minute: dataDescarga[2].slice(-5).split(":")[1] });
        var dataPrevisaoMoment = moment({ year: dataPrevisaoEntrega[2].split(" ")[0], month: dataPrevisaoEntrega[1], day: dataPrevisaoEntrega[0], hour: dataPrevisaoEntrega[2].slice(-5).split(":")[0], minute: dataPrevisaoEntrega[2].slice(-5).split(":")[1] });

        if (dataPrevisaoMoment > dataDescargaMoment)
            return true;
    }

    return false;
}

function retornoMotivoReagendamento(data) {
    _alterarHorarioAgendamentoDescarga.MotivoReagendamento.codEntity(data.Codigo);
    _alterarHorarioAgendamentoDescarga.MotivoReagendamento.val(data.Descricao);

    if (data.TipoResponsavelAtrasoEntrega != undefined && data.TipoResponsavelAtrasoEntrega > 0)
        _alterarHorarioAgendamentoDescarga.ResponsavelMotivoReagendamento.val(data.TipoResponsavelAtrasoEntrega);
}

function gravarSenhaAgendamentoEntrega() {
    if (!ValidarCamposObrigatorios(_gravarSenhaAgendamentoEntrega)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    let data = {
        CodigoPedido: _gravarSenhaAgendamentoEntrega.CodigoPedido.val(),
        CodigoCargaEntrega: _gravarSenhaAgendamentoEntrega.CodigoCargaEntrega.val(),
        SenhaEntregaAgendamento: _gravarSenhaAgendamentoEntrega.SenhaEntregaAgendamento.val(),
    }
    executarReST("AgendamentoEntregaPedido/GravarSenhaAgendamentoEntrega", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Senha de Agendamento salva com sucesso!");
                Global.fecharModal("divModalGravarSenhaAgendamentoEntrega");
                _gridAgendamentoEntregaPedido.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}