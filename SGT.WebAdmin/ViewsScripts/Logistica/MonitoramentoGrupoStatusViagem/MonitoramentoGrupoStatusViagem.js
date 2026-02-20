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

var _monitoramentoGrupoStatusViagem;
var _pesquisaMonitoramentoGrupoStatusViagem;
var _gridMonitoramentoGrupoStatusViagem;

var MonitoramentoGrupoStatusViagem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, maxlength: 40, getType: typesKnockout.string, val: ko.observable("") });
    this.Ordem = PropertyEntity({ text: "*Ordem:", required: true, maxlength: 2, getType: typesKnockout.int, val: ko.observable("") });
    this.Cor = PropertyEntity({ text: "*Cor: ", val: ko.observable(_corPadrao), options: _ListaCores });
    this.Ativo = PropertyEntity({ text: "*Situação: ", required: true, val: ko.observable(true), options: _status, def: true });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var PesquisaMonitoramentoGrupoStatusViagem = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:" });
    this.Ativo = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMonitoramentoGrupoStatusViagem.CarregarGrid();
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
function LoadMonitoramentoGrupoStatusViagem() {

    _pesquisaMonitoramentoGrupoStatusViagem = new PesquisaMonitoramentoGrupoStatusViagem();
    KoBindings(_pesquisaMonitoramentoGrupoStatusViagem, "knockoutPesquisaMonitoramentoGrupoStatusViagem", false, _pesquisaMonitoramentoGrupoStatusViagem.Pesquisar.id);

    _monitoramentoGrupoStatusViagem = new MonitoramentoGrupoStatusViagem();
    KoBindings(_monitoramentoGrupoStatusViagem, "knockoutMonitoramentoGrupoStatusViagem");

    HeaderAuditoria("MonitoramentoGrupoStatusViagem", _monitoramentoGrupoStatusViagem);

    BuscarMonitoramentoGrupoStatusViagem();

    loadCoresStatus();
}

function AdicionarClick(e, sender) {
    Salvar(_monitoramentoGrupoStatusViagem, "MonitoramentoGrupoStatusViagem/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMonitoramentoGrupoStatusViagem.CarregarGrid();
                LimparCamposMonitoramentoGrupoStatusViagem();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_monitoramentoGrupoStatusViagem, "MonitoramentoGrupoStatusViagem/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridMonitoramentoGrupoStatusViagem.CarregarGrid();
                LimparCamposMonitoramentoGrupoStatusViagem();
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
        ExcluirPorCodigo(_monitoramentoGrupoStatusViagem, "MonitoramentoGrupoStatusViagem/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridMonitoramentoGrupoStatusViagem.CarregarGrid();
                    LimparCamposMonitoramentoGrupoStatusViagem();
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
    LimparCamposMonitoramentoGrupoStatusViagem();
}

function EditarMonitoramentoGrupoStatusViagemClick(itemGrid) {
    LimparCamposMonitoramentoGrupoStatusViagem();
    _monitoramentoGrupoStatusViagem.Codigo.val(itemGrid.Codigo);
    BuscarPorCodigo(_monitoramentoGrupoStatusViagem, "MonitoramentoGrupoStatusViagem/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaMonitoramentoGrupoStatusViagem.ExibirFiltros.visibleFade(false);
                _monitoramentoGrupoStatusViagem.Atualizar.visible(true);
                _monitoramentoGrupoStatusViagem.Excluir.visible(true);
                _monitoramentoGrupoStatusViagem.Cancelar.visible(true);
                _monitoramentoGrupoStatusViagem.Adicionar.visible(false);
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
function BuscarMonitoramentoGrupoStatusViagem() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [{ descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarMonitoramentoGrupoStatusViagemClick, tamanho: "20", icone: "" }]
    };
    var ordenacaoPadrao = { column: 1, dir: orderDir.asc };
    _gridMonitoramentoGrupoStatusViagem = new GridView(_pesquisaMonitoramentoGrupoStatusViagem.Pesquisar.idGrid, "MonitoramentoGrupoStatusViagem/Pesquisa", _pesquisaMonitoramentoGrupoStatusViagem, menuOpcoes, ordenacaoPadrao);
    _gridMonitoramentoGrupoStatusViagem.CarregarGrid();
}

function LimparCamposMonitoramentoGrupoStatusViagem() {
    _monitoramentoGrupoStatusViagem.Atualizar.visible(false);
    _monitoramentoGrupoStatusViagem.Cancelar.visible(false);
    _monitoramentoGrupoStatusViagem.Excluir.visible(false);
    _monitoramentoGrupoStatusViagem.Adicionar.visible(true);
    LimparCampos(_monitoramentoGrupoStatusViagem);
    setarCorStatus();
}

function loadCoresStatus() {
    $("#" + _monitoramentoGrupoStatusViagem.Cor.id).colorselector({
        callback: function (value) {
            _monitoramentoGrupoStatusViagem.Cor.val(value);
        }
    });
    setarCorStatus(_corPadrao);
}
function setarCorStatus(cor) {
    if (cor == null) cor = _corPadrao;
    $("#" + _monitoramentoGrupoStatusViagem.Cor.id).colorselector("setValue", cor);
}