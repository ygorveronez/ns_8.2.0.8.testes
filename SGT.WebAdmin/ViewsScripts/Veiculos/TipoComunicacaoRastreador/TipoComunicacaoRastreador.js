//*******MAPEAMENTO KNOUCKOUT*******

var _tipoComunicacaoRastreador;
var _pesquisaTipoComunicacaoRastreador;
var _gridTipoComunicacaoRastreador;

var TipoComunicacaoRastreador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "*Código de Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Ativo = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: _status, def: true });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var PesquisaTipoComunicacaoRastreador = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:" });
    this.Ativo = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoComunicacaoRastreador.CarregarGrid();
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

//*******EVENTOS*******

function LoadTipoComunicacaoRastreador() {

    _pesquisaTipoComunicacaoRastreador = new PesquisaTipoComunicacaoRastreador();
    KoBindings(_pesquisaTipoComunicacaoRastreador, "knockoutPesquisaTipoComunicacaoRastreador", false, _pesquisaTipoComunicacaoRastreador.Pesquisar.id);

    _tipoComunicacaoRastreador = new TipoComunicacaoRastreador();
    KoBindings(_tipoComunicacaoRastreador, "knockoutTipoComunicacaoRastreador");

    HeaderAuditoria("TipoComunicacaoRastreador", _tipoComunicacaoRastreador);

    BuscarTipoComunicacaoRastreador();
}

function AdicionarClick(e, sender) {
    Salvar(_tipoComunicacaoRastreador, "TipoComunicacaoRastreador/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridTipoComunicacaoRastreador.CarregarGrid();
                LimparCamposTipoComunicacaoRastreador();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_tipoComunicacaoRastreador, "TipoComunicacaoRastreador/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridTipoComunicacaoRastreador.CarregarGrid();
                LimparCamposTipoComunicacaoRastreador();
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
        ExcluirPorCodigo(_tipoComunicacaoRastreador, "TipoComunicacaoRastreador/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridTipoComunicacaoRastreador.CarregarGrid();
                    LimparCamposTipoComunicacaoRastreador();
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
    LimparCamposTipoComunicacaoRastreador();
}

function EditarTipoComunicacaoRastreadorClick(itemGrid) {

    LimparCamposTipoComunicacaoRastreador();

    _tipoComunicacaoRastreador.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_tipoComunicacaoRastreador, "TipoComunicacaoRastreador/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaTipoComunicacaoRastreador.ExibirFiltros.visibleFade(false);

                _tipoComunicacaoRastreador.Atualizar.visible(true);
                _tipoComunicacaoRastreador.Excluir.visible(true);
                _tipoComunicacaoRastreador.Cancelar.visible(true);
                _tipoComunicacaoRastreador.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******

function BuscarTipoComunicacaoRastreador() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarTipoComunicacaoRastreadorClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridTipoComunicacaoRastreador = new GridView(_pesquisaTipoComunicacaoRastreador.Pesquisar.idGrid, "TipoComunicacaoRastreador/Pesquisa", _pesquisaTipoComunicacaoRastreador, menuOpcoes, null);
    _gridTipoComunicacaoRastreador.CarregarGrid();
}

function LimparCamposTipoComunicacaoRastreador() {
    _tipoComunicacaoRastreador.Atualizar.visible(false);
    _tipoComunicacaoRastreador.Cancelar.visible(false);
    _tipoComunicacaoRastreador.Excluir.visible(false);
    _tipoComunicacaoRastreador.Adicionar.visible(true);

    LimparCampos(_tipoComunicacaoRastreador);
}