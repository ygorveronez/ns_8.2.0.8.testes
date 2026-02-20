//*******MAPEAMENTO KNOUCKOUT*******

var _tecnologiaRastreador, _pesquisaTecnologiaRastreador, _gridTecnologiaRastreador, _crudTecnologiaRastreador;

var TecnologiaRastreador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.NomeConta = PropertyEntity({ text: "*Nome da conta:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "*Código de Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Ativo = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: _status, def: true });
};

var PesquisaTecnologiaRastreador = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:" });
    this.Ativo = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTecnologiaRastreador.CarregarGrid();
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

var CRUDTecnologiaRastreador = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function LoadTecnologiaRastreador() {

    _pesquisaTecnologiaRastreador = new PesquisaTecnologiaRastreador();
    KoBindings(_pesquisaTecnologiaRastreador, "knockoutPesquisaTecnologiaRastreador", false, _pesquisaTecnologiaRastreador.Pesquisar.id);

    _tecnologiaRastreador = new TecnologiaRastreador();
    KoBindings(_tecnologiaRastreador, "knockoutTecnologiaRastreador");

    _crudTecnologiaRastreador = new CRUDTecnologiaRastreador();
    KoBindings(_crudTecnologiaRastreador, "knockoutCRUDTecnologiaRastreador");

    HeaderAuditoria("TecnologiaRastreador", _tecnologiaRastreador);

    BuscarTecnologiaRastreador();

    ConfigurarIntegracoesDisponiveis();
}

function AdicionarClick(e, sender) {
    Salvar(_tecnologiaRastreador, "TecnologiaRastreador/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridTecnologiaRastreador.CarregarGrid();
                LimparCamposTecnologiaRastreador();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_tecnologiaRastreador, "TecnologiaRastreador/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridTecnologiaRastreador.CarregarGrid();
                LimparCamposTecnologiaRastreador();
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
        ExcluirPorCodigo(_tecnologiaRastreador, "TecnologiaRastreador/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridTecnologiaRastreador.CarregarGrid();
                    LimparCamposTecnologiaRastreador();
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
    LimparCamposTecnologiaRastreador();
}

function EditarTecnologiaRastreadorClick(itemGrid) {

    LimparCamposTecnologiaRastreador();

    _tecnologiaRastreador.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_tecnologiaRastreador, "TecnologiaRastreador/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaTecnologiaRastreador.ExibirFiltros.visibleFade(false);

                _crudTecnologiaRastreador.Atualizar.visible(true);
                _crudTecnologiaRastreador.Excluir.visible(true);
                _crudTecnologiaRastreador.Cancelar.visible(true);
                _crudTecnologiaRastreador.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******

function BuscarTecnologiaRastreador() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarTecnologiaRastreadorClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridTecnologiaRastreador = new GridView(_pesquisaTecnologiaRastreador.Pesquisar.idGrid, "TecnologiaRastreador/Pesquisa", _pesquisaTecnologiaRastreador, menuOpcoes, null);
    _gridTecnologiaRastreador.CarregarGrid();
}

function LimparCamposTecnologiaRastreador() {
    _crudTecnologiaRastreador.Atualizar.visible(false);
    _crudTecnologiaRastreador.Cancelar.visible(false);
    _crudTecnologiaRastreador.Excluir.visible(false);
    _crudTecnologiaRastreador.Adicionar.visible(true);

    LimparCampos(_tecnologiaRastreador);

    Global.ResetarAbas();
}

function ConfigurarIntegracoesDisponiveis() {
    executarReST("Integracao/ObterIntegracoesConfiguradas", {}, function (r) {
        if (r.Success && r.Data) {
            var data = r.Data;
            if (data.TiposExistentes != null && data.TiposExistentes.length > 0) {

                if (data.TiposExistentes.some(function (o) { return o === EnumTipoIntegracao.A52; })) {
                    LoadConfiguracaoA52();
                    $("#liA52").removeClass("hidden");
                }

            }
        }
    });
}