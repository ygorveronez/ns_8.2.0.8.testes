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

var _grupoTipoOperacao;
var _pesquisaGrupoTipoOperacao;
var _gridGrupoTipoOperacao;

var GrupoTipoOperacao = function () {
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

var PesquisaGrupoTipoOperacao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:" });
    this.Ativo = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridGrupoTipoOperacao.CarregarGrid();
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
function LoadGrupoTipoOperacao() {
    _pesquisaGrupoTipoOperacao = new PesquisaGrupoTipoOperacao();
    KoBindings(_pesquisaGrupoTipoOperacao, "knockoutPesquisaGrupoTipoOperacao", false, _pesquisaGrupoTipoOperacao.Pesquisar.id);

    _grupoTipoOperacao = new GrupoTipoOperacao();
    KoBindings(_grupoTipoOperacao, "knockoutGrupoTipoOperacao");

    HeaderAuditoria("GrupoTipoOperacao", _grupoTipoOperacao);
    BuscarGrupoTipoOperacao();
    loadCoresStatus();
}

function AdicionarClick(e, sender) {
    Salvar(_grupoTipoOperacao, "GrupoTipoOperacao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridGrupoTipoOperacao.CarregarGrid();
                LimparCamposGrupoTipoOperacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_grupoTipoOperacao, "GrupoTipoOperacao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridGrupoTipoOperacao.CarregarGrid();
                LimparCamposGrupoTipoOperacao();
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
        ExcluirPorCodigo(_grupoTipoOperacao, "GrupoTipoOperacao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridGrupoTipoOperacao.CarregarGrid();
                    LimparCamposGrupoTipoOperacao();
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
    LimparCamposGrupoTipoOperacao();
}

function EditarGrupoTipoOperacaoClick(itemGrid) {
    LimparCamposGrupoTipoOperacao();
    _grupoTipoOperacao.Codigo.val(itemGrid.Codigo);
    BuscarPorCodigo(_grupoTipoOperacao, "GrupoTipoOperacao/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaGrupoTipoOperacao.ExibirFiltros.visibleFade(false);
                _grupoTipoOperacao.Atualizar.visible(true);
                _grupoTipoOperacao.Excluir.visible(true);
                _grupoTipoOperacao.Cancelar.visible(true);
                _grupoTipoOperacao.Adicionar.visible(false);
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
function BuscarGrupoTipoOperacao() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [{ descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarGrupoTipoOperacaoClick, tamanho: "20", icone: "" }]
    };
    var ordenacaoPadrao = { column: 1, dir: orderDir.asc };
    _gridGrupoTipoOperacao = new GridView(_pesquisaGrupoTipoOperacao.Pesquisar.idGrid, "GrupoTipoOperacao/Pesquisa", _pesquisaGrupoTipoOperacao, menuOpcoes, ordenacaoPadrao);
    _gridGrupoTipoOperacao.CarregarGrid();
}

function LimparCamposGrupoTipoOperacao() {
    _grupoTipoOperacao.Atualizar.visible(false);
    _grupoTipoOperacao.Cancelar.visible(false);
    _grupoTipoOperacao.Excluir.visible(false);
    _grupoTipoOperacao.Adicionar.visible(true);
    LimparCampos(_grupoTipoOperacao);
    setarCorStatus();
}

function loadCoresStatus() {
    $("#" + _grupoTipoOperacao.Cor.id).colorselector({
        callback: function (value) {
            _grupoTipoOperacao.Cor.val(value);
        }
    });
    setarCorStatus(_corPadrao);
}
function setarCorStatus(cor) {
    if (cor == null) cor = _corPadrao;
    $("#" + _grupoTipoOperacao.Cor.id).colorselector("setValue", cor);
}