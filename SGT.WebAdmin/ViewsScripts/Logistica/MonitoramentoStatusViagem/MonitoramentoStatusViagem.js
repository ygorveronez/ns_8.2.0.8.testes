var _corPadrao = '#FFFFFF';
var _ListaCores = [
    { value: '#FFFFFF' },
    { value: '#ED6464' },
    { value: '#ED8664' },
    { value: '#EDA864' },
    { value: '#EDCB64' },
    { value: '#EDED64' },
    { value: '#CBED64' },
    { value: '#A8ED64' },
    { value: '#86ED64' },
    { value: '#64ED64' },
    { value: '#64ED86' },
    { value: '#64EDA8' },
    { value: '#64EDCB' },
    { value: '#64EDED' },
    { value: '#64CBED' },
    { value: '#64A8ED' },
    { value: '#6495ED' },
    { value: '#6486ED' },
    { value: '#6464ED' },
    { value: '#8664ED' },
    { value: '#A864ED' },
    { value: '#CB64ED' },
    { value: '#ED64ED' },
    { value: '#ED64CB' },
    { value: '#ED64A8' },
    { value: '#ED6486' },
    { value: '#8B4513' },
    { value: '#E06F1F' },
    { value: '#EDA978' },
    { value: '#F9E2D2' },
    { value: '#000000' },
    { value: '#708090' },
    { value: '#9AA6B1' },
    { value: '#C5CCD3' },
    { value: '#F1F2F4' }
];

var _monitoramentoStatusViagem;
var _pesquisaMonitoramentoStatusViagem;
var _gridMonitoramentoStatusViagem;
var _gruposStatusViagem = [];

var MonitoramentoStatusViagem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, maxlength: 40, getType: typesKnockout.string, val: ko.observable("") });
    this.Sigla = PropertyEntity({ text: "*Sigla:", required: true, maxlength: 10, getType: typesKnockout.string, val: ko.observable("") });
    this.Grupo = PropertyEntity({ text: "*Grupo: ", required: true, val: ko.observable(""), options: ko.observableArray(_gruposStatusViagem), def: "" });
    this.Ordem = PropertyEntity({ text: "*Ordem:", required: true, maxlength: 2, getType: typesKnockout.int, val: ko.observable("") });
    this.TipoRegra = PropertyEntity({ text: "*Tipo de regra: ", required: true, options: EnumMonitoramentoStatusViagemTipoRegra.obterOpcoes(), val: ko.observable(0), def: EnumMonitoramentoStatusViagemTipoRegra.SemViagem });
    this.Cor = PropertyEntity({ text: "*Cor: ", val: ko.observable(_corPadrao), options: _ListaCores });
    this.Ativo = PropertyEntity({ text: "*Situação: ", required: true, val: ko.observable(true), options: _status, def: true });
    this.ValidarStatusCargaAoTrocarStatusViagem = PropertyEntity({ text: "", required: false, visible: ko.observable(false), val: ko.observable(false), def: false });
    this.StatusCargaAoTrocarStatusViagem = PropertyEntity({ text: "Validar Status da Carga ao Trocar Status da Viagem: ", options: ko.observable(null), val: ko.observable(EnumSituacaoCargaValidacaoStatusViagemMonitoramentoHelper.Nenhum), def: EnumSituacaoCargaValidacaoStatusViagemMonitoramentoHelper.Nenhum, enable: ko.observable(false), visible: ko.observable(false), required: ko.observable(false) });
    this.NaoUtilizarStatusParaCalculoTemperaturaDentroFaixa = PropertyEntity({ getType: typesKnockout.bool, text: "Não utilizar status para o cálculo de % de temperatura dentro da faixa", val: ko.observable(false), def: false });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.TipoRegra.val.subscribe(function (valor) {
        _monitoramentoStatusViagem.ValidarStatusCargaAoTrocarStatusViagem.visible(false);
        _monitoramentoStatusViagem.StatusCargaAoTrocarStatusViagem.visible(false);
        _monitoramentoStatusViagem.StatusCargaAoTrocarStatusViagem.required(false);

        if (valor == EnumMonitoramentoStatusViagemTipoRegra.Transito) {
            _monitoramentoStatusViagem.ValidarStatusCargaAoTrocarStatusViagem.visible(true);
            _monitoramentoStatusViagem.StatusCargaAoTrocarStatusViagem.visible(true);
            _monitoramentoStatusViagem.StatusCargaAoTrocarStatusViagem.required(true);
            _monitoramentoStatusViagem.StatusCargaAoTrocarStatusViagem.options(EnumSituacaoCargaValidacaoStatusViagemMonitoramento.obterOpcoesTipoTransito());
        }
    });

    this.ValidarStatusCargaAoTrocarStatusViagem.val.subscribe(function () {
        if (_monitoramentoStatusViagem.ValidarStatusCargaAoTrocarStatusViagem.val()) {
            _monitoramentoStatusViagem.StatusCargaAoTrocarStatusViagem.enable(true);
            _monitoramentoStatusViagem.StatusCargaAoTrocarStatusViagem.required(true);
            _monitoramentoStatusViagem.StatusCargaAoTrocarStatusViagem.val(EnumSituacaoCargaValidacaoStatusViagemMonitoramento.Nenhum);
        }
        else {
            _monitoramentoStatusViagem.StatusCargaAoTrocarStatusViagem.enable(false);
            _monitoramentoStatusViagem.StatusCargaAoTrocarStatusViagem.required(false);
            _monitoramentoStatusViagem.StatusCargaAoTrocarStatusViagem.val(EnumSituacaoCargaValidacaoStatusViagemMonitoramento.Nenhum);
        }
    });
};

var PesquisaMonitoramentoStatusViagem = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:" });
    this.Sigla = PropertyEntity({ text: "Sigla:" });
    this.Ativo = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMonitoramentoStatusViagem.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

// Eventos
function LoadMonitoramentoStatusViagem() {

    _pesquisaMonitoramentoStatusViagem = new PesquisaMonitoramentoStatusViagem();
    KoBindings(_pesquisaMonitoramentoStatusViagem, "knockoutPesquisaMonitoramentoStatusViagem", false, _pesquisaMonitoramentoStatusViagem.Pesquisar.id);

    _monitoramentoStatusViagem = new MonitoramentoStatusViagem();
    KoBindings(_monitoramentoStatusViagem, "knockoutMonitoramentoStatusViagem");

    HeaderAuditoria("MonitoramentoStatusViagem", _monitoramentoStatusViagem);

    BuscarMonitoramentoStatusViagem();
    carregarGruposStatusViagem();
    loadCoresStatus();
}

function AdicionarClick(e, sender) {
    Salvar(_monitoramentoStatusViagem, "MonitoramentoStatusViagem/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMonitoramentoStatusViagem.CarregarGrid();
                LimparCamposMonitoramentoStatusViagem();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_monitoramentoStatusViagem, "MonitoramentoStatusViagem/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridMonitoramentoStatusViagem.CarregarGrid();
                LimparCamposMonitoramentoStatusViagem();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function ExcluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir este cadastro?", function () {
        ExcluirPorCodigo(_monitoramentoStatusViagem, "MonitoramentoStatusViagem/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridMonitoramentoStatusViagem.CarregarGrid();
                    LimparCamposMonitoramentoStatusViagem();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function CancelarClick(e) {
    LimparCamposMonitoramentoStatusViagem();
}

function EditarMonitoramentoStatusViagemClick(itemGrid) {
    LimparCamposMonitoramentoStatusViagem();
    _monitoramentoStatusViagem.Codigo.val(itemGrid.Codigo);
    BuscarPorCodigo(_monitoramentoStatusViagem, "MonitoramentoStatusViagem/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaMonitoramentoStatusViagem.ExibirFiltros.visibleFade(false);
                _monitoramentoStatusViagem.Atualizar.visible(true);
                _monitoramentoStatusViagem.Excluir.visible(true);
                _monitoramentoStatusViagem.Cancelar.visible(true);
                _monitoramentoStatusViagem.Adicionar.visible(false);
                setarCorStatus(arg.Data.Cor);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

// Métodos
function BuscarMonitoramentoStatusViagem() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [{ descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarMonitoramentoStatusViagemClick, tamanho: "20", icone: "" }]
    };
    var ordenacaoPadrao = { column: 1, dir: orderDir.asc };
    _gridMonitoramentoStatusViagem = new GridView(_pesquisaMonitoramentoStatusViagem.Pesquisar.idGrid, "MonitoramentoStatusViagem/Pesquisa", _pesquisaMonitoramentoStatusViagem, menuOpcoes, ordenacaoPadrao);
    _gridMonitoramentoStatusViagem.CarregarGrid();
}

function LimparCamposMonitoramentoStatusViagem() {
    _monitoramentoStatusViagem.Atualizar.visible(false);
    _monitoramentoStatusViagem.Cancelar.visible(false);
    _monitoramentoStatusViagem.Excluir.visible(false);
    _monitoramentoStatusViagem.Adicionar.visible(true);
    LimparCampos(_monitoramentoStatusViagem);
    setarCorStatus();
}

function loadCoresStatus() {
    $("#" + _monitoramentoStatusViagem.Cor.id).colorselector({
        callback: function (value) {
            _monitoramentoStatusViagem.Cor.val(value);
        }
    });
    setarCorStatus(_corPadrao);
}
function setarCorStatus(cor) {
    if (cor == null) cor = _corPadrao;
    $("#" + _monitoramentoStatusViagem.Cor.id).colorselector("setValue", cor);
}

function carregarGruposStatusViagem() {
    executarReST("MonitoramentoGrupoStatusViagem/BuscarTodos", null, function (response) {
        if (response.Success) {
            if (response.Data != null) {
                _gruposStatusViagem = response.Data.GrupoStatusViagem;
                _monitoramentoStatusViagem.Grupo.options(_gruposStatusViagem);
            }
        } else {
            exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
        }
    });
}