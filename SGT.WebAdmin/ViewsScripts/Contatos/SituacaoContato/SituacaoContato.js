//*******MAPEAMENTO KNOUCKOUT*******

var _situacaoContato;
var _pesquisaSituacaoContato;
var _gridSituacaoContato;

var SituacaoContato = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, maxlength: 150 });
    this.Observacao = PropertyEntity({ text: "Observação:", issue: 593, maxlength: 450 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "Situação: ", issue: 557 });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaSituacaoContato = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSituacaoContato.CarregarGrid();
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
}


//*******EVENTOS*******
function loadSituacaoContato() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaSituacaoContato = new PesquisaSituacaoContato();
    KoBindings(_pesquisaSituacaoContato, "knockoutPesquisaSituacaoContato", false, _pesquisaSituacaoContato.Pesquisar.id);

    // Instancia objeto principal
    _situacaoContato = new SituacaoContato();
    KoBindings(_situacaoContato, "knockoutSituacaoContato");

    HeaderAuditoria("SituacaoContato", _situacaoContato);

    // Inicia busca
    buscarSituacaoContato();
}

function adicionarClick(e, sender) {
    Salvar(_situacaoContato, "SituacaoContato/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridSituacaoContato.CarregarGrid();
                limparCamposSituacaoContato();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_situacaoContato, "SituacaoContato/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridSituacaoContato.CarregarGrid();
                limparCamposSituacaoContato();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_situacaoContato, "SituacaoContato/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridSituacaoContato.CarregarGrid();
                    limparCamposSituacaoContato();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCamposSituacaoContato();
}

function editarSituacaoContatoClick(itemGrid) {
    // Limpa os campos
    limparCamposSituacaoContato();

    // Seta o codigo do objeto
    _situacaoContato.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_situacaoContato, "SituacaoContato/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaSituacaoContato.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _situacaoContato.Atualizar.visible(true);
                _situacaoContato.Excluir.visible(true);
                _situacaoContato.Cancelar.visible(true);
                _situacaoContato.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarSituacaoContato() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarSituacaoContatoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridSituacaoContato = new GridView(_pesquisaSituacaoContato.Pesquisar.idGrid, "SituacaoContato/Pesquisa", _pesquisaSituacaoContato, menuOpcoes, null);
    _gridSituacaoContato.CarregarGrid();
}

function limparCamposSituacaoContato() {
    _situacaoContato.Atualizar.visible(false);
    _situacaoContato.Cancelar.visible(false);
    _situacaoContato.Excluir.visible(false);
    _situacaoContato.Adicionar.visible(true);
    LimparCampos(_situacaoContato);
}