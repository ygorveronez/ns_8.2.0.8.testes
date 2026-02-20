/// <autosync enabled="true" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridHorariosInfoCarregamento;
var _infoCarregamento;

var InfoCarregamento = function () {
    this.DataCarregamentoDisponibilidade = PropertyEntity({ text: "Data: ", required: true, getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), diminuirData: diminuirDataClick, aumentarData: aumentarDataClick, idGrid: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.DataCarregamentoDisponibilidadeLimiteSelecaoInicial = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.Filial = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.TipoOperacao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.TipoOperacaoPedido = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.ModeloVeiculo = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.NaoPermitirAgendarCargasNoMesmoDia = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Próximo", visible: ko.observable(true) });
    this.Proximo = PropertyEntity({ eventClick: DataAgendamentoProximaEtapa, type: types.event, text: "Próximo", visible: ko.observable(false) });

    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function alterarDataCarregamentoLista() {
    _gridHorariosInfoCarregamento.CarregarGrid();
}

function loadDataCarregamento() {
    _infoCarregamento = new InfoCarregamento();

    KoBindings(_infoCarregamento, "knoutAlterarCarregamento");

    CarregarGridHorarios();

    _infoCarregamento.DataCarregamentoDisponibilidade.val.subscribe(function () {
        AtualizarHorarios(_infoCarregamento);
    });

    _retiradaProduto.Filial.val.subscribe(function () {
        _infoCarregamento.Filial.val(_retiradaProduto.Filial.val());
        _pedido.Filial.codEntity(_retiradaProduto.Filial.val());
        _pedido.Filial.val(_retiradaProduto.Filial.val());
        validarInformacoesPorCentro();
    });

    _retiradaProduto.TipoOperacao.val.subscribe(function () {
        _infoCarregamento.TipoOperacao.val(_retiradaProduto.TipoOperacao.val());
    });

    _retiradaProduto.ModeloVeiculo.val.subscribe(function () {
        _infoCarregamento.ModeloVeiculo.val(_retiradaProduto.ModeloVeiculo.val());
    });
}

function aumentarDataClick(e, sender) {
    SetarData(e, 1);
}

function diminuirDataClick(e, sender) {
    SetarData(e, -1, _infoCarregamento.DataCarregamentoDisponibilidadeLimiteSelecaoInicial.val());
}


//*******MÉTODOS*******

function CarregarGridHorarios() {
    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: function (obj, data) {
            HorarioSelecionado(_infoCarregamento, data);
        },
        callbackNaoSelecionado: function () {
        },
        eventos: function () {
        },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        somenteLeitura: false
    };

    if (_infoCarregamento.DataCarregamentoDisponibilidade == null) {
        var tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        _infoCarregamento.DataCarregamentoDisponibilidade.val(tomorrow.toLocaleDateString());
    }

    _gridHorariosInfoCarregamento = new GridView(_infoCarregamento.DataCarregamentoDisponibilidade.idGrid, "RetiradaProduto/ObterHorariosDisponiveis", _infoCarregamento, null, null, 10, null, null, null, multiplaescolha);
    _gridHorariosInfoCarregamento.onAfterGridLoad(function (data) {
        if (_retiradaProduto.Hora.val() == '') {
            return;
        }
        var dados = data.data;
        for (var i = 0; i < dados.length; i++) {
            if (_retiradaProduto.Hora.val() + ':00' == dados[i].HoraInicio) {
                HorarioSelecionado(_infoCarregamento, dados[i]);
                return;
            }
        }
    });
}

function HorarioSelecionado(e, data) {
    _gridHorariosInfoCarregamento.AtualizarRegistrosSelecionados([data]);
    _gridHorariosInfoCarregamento.DrawTable(true);
    var dataHoraComSegundos = e.DataCarregamentoDisponibilidade.val() + ' ' + data.HoraInicio;
    if (e.DataCarregamentoDisponibilidade.val().length > 10)
        var dataHoraComSegundos = e.DataCarregamentoDisponibilidade.val();
    _retiradaProduto.DataRetirada.val(dataHoraComSegundos);
}

function LimparHorarioSelecionado(e) {
    _gridHorariosInfoCarregamento.AtualizarRegistrosSelecionados([]);
    _gridHorariosInfoCarregamento.DrawTable(true);

}

function SetarData(e, dias, dataLimiteSelecaoInicial, dataLimiteSelecaoFinal) {
    if (!e.DataCarregamentoDisponibilidade.val())
        return;

    var objData = moment(e.DataCarregamentoDisponibilidade.val(), 'DD/MM/YYYY');
    objData.add(dias, 'day');

    if (dataLimiteSelecaoInicial != undefined || dataLimiteSelecaoInicial != null) {
        if (objData._i == dataLimiteSelecaoInicial)
            objData.add(1, 'day');

        LimparHorarioSelecionado(e);
    }
    else if (dataLimiteSelecaoFinal != undefined || dataLimiteSelecaoFinal != null) {
        if (objData._i == dataLimiteSelecaoFinal)
            objData.add(-1, 'day');

    }

    LimparHorarioSelecionado(e);
    e.DataCarregamentoDisponibilidade.val(objData.format('DD/MM/YYYY'));
}

function AtualizarHorarios(e) {
    if (!e.DataCarregamentoDisponibilidade.val()) return;
    _gridHorariosInfoCarregamento.CarregarGrid();
}

function DataAgendamentoProximaEtapa() {
    ExibirProximaEtapa("tabConfirmacao");
}

function validarInformacoesPorCentro() {
    executarReST("RetiradaProduto/ValidarInformacoesPorCentro", { Filial: _infoCarregamento.Filial.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.NaoPermitirAgendarCargasNoMesmoDia) {
                    _infoCarregamento.NaoPermitirAgendarCargasNoMesmoDia.val(arg.Data.NaoPermitirAgendarCargasNoMesmoDia);
                    _infoCarregamento.DataCarregamentoDisponibilidade.minDate(arg.Data.DataCarregamentoDisponibilidade);
                    _infoCarregamento.DataCarregamentoDisponibilidade.val(arg.Data.DataCarregamentoDisponibilidade);
                    _infoCarregamento.DataCarregamentoDisponibilidadeLimiteSelecaoInicial.val(arg.Data.DataCarregamentoDisponibilidade);
                    _gridHorariosInfoCarregamento.CarregarGrid();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}