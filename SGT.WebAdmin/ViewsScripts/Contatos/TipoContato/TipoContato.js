//*******MAPEAMENTO KNOUCKOUT*******

var _tipoContato;
var _pesquisaTipoContato;
var _gridTipoContato;

var TipoContato = function () {
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

var PesquisaTipoContato = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoContato.CarregarGrid();
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
function loadTipoContato() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaTipoContato = new PesquisaTipoContato();
    KoBindings(_pesquisaTipoContato, "knockoutPesquisaTipoContato", false, _pesquisaTipoContato.Pesquisar.id);

    // Instancia objeto principal
    _tipoContato = new TipoContato();
    KoBindings(_tipoContato, "knockoutTipoContato");

    HeaderAuditoria("TipoContato", _tipoContato);

    // Inicia busca
    buscarTipoContato();
}

function adicionarClick(e, sender) {
    Salvar(_tipoContato, "TipoContato/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridTipoContato.CarregarGrid();
                limparCamposTipoContato();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tipoContato, "TipoContato/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridTipoContato.CarregarGrid();
                limparCamposTipoContato();
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
        ExcluirPorCodigo(_tipoContato, "TipoContato/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridTipoContato.CarregarGrid();
                    limparCamposTipoContato();
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
    limparCamposTipoContato();
}

function editarTipoContatoClick(itemGrid) {
    // Limpa os campos
    limparCamposTipoContato();

    // Seta o codigo do objeto
    _tipoContato.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_tipoContato, "TipoContato/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaTipoContato.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _tipoContato.Atualizar.visible(true);
                _tipoContato.Excluir.visible(true);
                _tipoContato.Cancelar.visible(true);
                _tipoContato.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarTipoContato() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTipoContatoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridTipoContato = new GridView(_pesquisaTipoContato.Pesquisar.idGrid, "TipoContato/Pesquisa", _pesquisaTipoContato, menuOpcoes, null);
    _gridTipoContato.CarregarGrid();
}

function limparCamposTipoContato() {
    _tipoContato.Atualizar.visible(false);
    _tipoContato.Cancelar.visible(false);
    _tipoContato.Excluir.visible(false);
    _tipoContato.Adicionar.visible(true);
    LimparCampos(_tipoContato);
}