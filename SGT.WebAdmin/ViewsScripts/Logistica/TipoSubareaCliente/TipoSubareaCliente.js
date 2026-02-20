var _tipoSubareaCliente;
var _pesquisaTipoSubareaCliente;
var _gridTipoSubareaCliente;

var TipoSubareaCliente = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Tipo = PropertyEntity({ text: "Tipo: ", required: true, options: EnumTipoTipoSubareaCliente.obterOpcoes(), val: ko.observable(EnumTipoTipoSubareaCliente.Balanca), def: 0 });
    this.Ativo = PropertyEntity({ text: "Situação: ", required: true, val: ko.observable(true), options: _status, def: true });
    this.PermiteMovimentacaoDoPatioPorEntradaOuSaidaDaArea = PropertyEntity({ text: "Permite movimentação do pátio por entrada ou saída da área", val: ko.observable(false), def: false });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var PesquisaTipoSubareaCliente = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:" });
    this.Ativo = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoSubareaCliente.CarregarGrid();
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
function LoadTipoSubareaCliente() {

    _pesquisaTipoSubareaCliente = new PesquisaTipoSubareaCliente();
    KoBindings(_pesquisaTipoSubareaCliente, "knockoutPesquisaTipoSubareaCliente", false, _pesquisaTipoSubareaCliente.Pesquisar.id);

    _tipoSubareaCliente = new TipoSubareaCliente();
    KoBindings(_tipoSubareaCliente, "knockoutTipoSubareaCliente");

    HeaderAuditoria("TipoSubareaCliente", _tipoSubareaCliente);

    BuscarTipoSubareaCliente();
}

function AdicionarClick(e, sender) {
    Salvar(_tipoSubareaCliente, "TipoSubareaCliente/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridTipoSubareaCliente.CarregarGrid();
                LimparCamposTipoSubareaCliente();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_tipoSubareaCliente, "TipoSubareaCliente/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridTipoSubareaCliente.CarregarGrid();
                LimparCamposTipoSubareaCliente();
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
        ExcluirPorCodigo(_tipoSubareaCliente, "TipoSubareaCliente/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridTipoSubareaCliente.CarregarGrid();
                    LimparCamposTipoSubareaCliente();
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
    LimparCamposTipoSubareaCliente();
}

function EditarTipoSubareaClienteClick(itemGrid) {
    LimparCamposTipoSubareaCliente();
    _tipoSubareaCliente.Codigo.val(itemGrid.Codigo);
    BuscarPorCodigo(_tipoSubareaCliente, "TipoSubareaCliente/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaTipoSubareaCliente.ExibirFiltros.visibleFade(false);
                _tipoSubareaCliente.Atualizar.visible(true);
                _tipoSubareaCliente.Excluir.visible(true);
                _tipoSubareaCliente.Cancelar.visible(true);
                _tipoSubareaCliente.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

// Métodos
function BuscarTipoSubareaCliente() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [{ descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarTipoSubareaClienteClick, tamanho: "20", icone: "" }]
    };
    var ordenacaoPadrao = { column: 1, dir: orderDir.asc };
    _gridTipoSubareaCliente = new GridView(_pesquisaTipoSubareaCliente.Pesquisar.idGrid, "TipoSubareaCliente/Pesquisa", _pesquisaTipoSubareaCliente, menuOpcoes, ordenacaoPadrao);
    _gridTipoSubareaCliente.CarregarGrid();
}

function LimparCamposTipoSubareaCliente() {
    _tipoSubareaCliente.Atualizar.visible(false);
    _tipoSubareaCliente.Cancelar.visible(false);
    _tipoSubareaCliente.Excluir.visible(false);
    _tipoSubareaCliente.Adicionar.visible(true);

    LimparCampos(_tipoSubareaCliente);
}