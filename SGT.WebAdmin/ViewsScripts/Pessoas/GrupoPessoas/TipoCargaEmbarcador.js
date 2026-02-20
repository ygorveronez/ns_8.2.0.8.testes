var _gridTipoCargaEmbarcador, _pesquisaTipoCargaEmbarcador, _tipoCargaEmbarcador;

var PesquisaTipoCargaEmbarcador = function () {
    this.GrupoPessoas = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoCargaEmbarcador.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ObterTiposCargaEmbarcador = PropertyEntity({
        eventClick: function (e) {
            ObterTiposCargaEmbarcador();
        }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.BuscarTiposCargaEmbarcador, idGrid: guid(), visible: ko.observable(true)
    });
};

var TipoCargaEmbarcador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TipoCargaEmbarcador = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.TipoCargaEmbarcador.getRequiredFieldDescription(), options: ko.observable([]), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true), enable: ko.observable(true), required: true });
    this.TipoCarga = PropertyEntity({ type: types.entity, text: Localization.Resources.Pessoas.GrupoPessoas.TipoCarga.getRequiredFieldDescription(), idBtnSearch: guid(), issue: 0, codEntity: ko.observable(0), visible: ko.observable(true), required: true, enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarTipoCargaEmbarcadorClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarTipoCargaEmbarcadorClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Atualizar, visible: ko.observable(false), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: LimparCamposTipoCargaEmbarcador, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Cancelar, visible: ko.observable(false), enable: ko.observable(true) });
};

function LoadTipoCargaEmbarcador() {
    _pesquisaTipoCargaEmbarcador = new PesquisaTipoCargaEmbarcador();
    KoBindings(_pesquisaTipoCargaEmbarcador, "knockoutPesquisaTipoCargaEmbarcador");

    _tipoCargaEmbarcador = new TipoCargaEmbarcador();
    KoBindings(_tipoCargaEmbarcador, "knockoutTipoCargaEmbarcador");

    _pesquisaTipoCargaEmbarcador.GrupoPessoas = _grupoPessoas.Codigo;

    new BuscarTiposdeCarga(_tipoCargaEmbarcador.TipoCarga);

    LoadGridTipoCargaEmbarcador();
}

function LoadGridTipoCargaEmbarcador() {
    var editar = { descricao: Localization.Resources.Pessoas.GrupoPessoas.Editar, id: guid(), evento: "onclick", metodo: EditarTipoCargaEmbarcador, tamanho: "15", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridTipoCargaEmbarcador = new GridView(_pesquisaTipoCargaEmbarcador.Pesquisar.idGrid, "GrupoPessoasTipoCargaEmbarcador/Pesquisa", _pesquisaTipoCargaEmbarcador, menuOpcoes);

    $("#liConfiguracaoMultiEmbarcador").on("click", function () {
        _gridTipoCargaEmbarcador.CarregarGrid();
        ObterTiposCargaDoEmbarcador();
    });
}

function ObterTiposCargaDoEmbarcador() {
    executarReST("GrupoPessoasTipoCargaEmbarcador/ObterTiposCargaEmbarcador", { GrupoPessoas: _grupoPessoas.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {

                var opcoes = new Array();

                for (var i = 0; i < r.Data.length; i++)
                    opcoes.push({ text: r.Data[i].Descricao, value: r.Data[i].Codigo });

                _tipoCargaEmbarcador.TipoCargaEmbarcador.options(opcoes);

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.GrupoPessoas.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.GrupoPessoas.Falha, r.Msg);
        }
    });
}

function EditarTipoCargaEmbarcador(obj) {
    executarReST("GrupoPessoasTipoCargaEmbarcador/BuscarPorCodigo", { Codigo: obj.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _tipoCargaEmbarcador.Adicionar.visible(false);
                _tipoCargaEmbarcador.Atualizar.visible(true);
                _tipoCargaEmbarcador.Cancelar.visible(true);

                _tipoCargaEmbarcador.Codigo.val(r.Data.Codigo);
                _tipoCargaEmbarcador.TipoCargaEmbarcador.val(r.Data.DescricaoTipoCargaEmbarcador);
                _tipoCargaEmbarcador.TipoCarga.val(r.Data.TipoCarga.Descricao);
                _tipoCargaEmbarcador.TipoCarga.codEntity(r.Data.TipoCarga.Codigo);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.GrupoPessoas.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.GrupoPessoas.Falha, r.Msg);
        }
    });
}

function AdicionarTipoCargaEmbarcadorClick() {
    Salvar(_tipoCargaEmbarcador, "GrupoPessoasTipoCargaEmbarcador/Adicionar", function (r) {
        if (r.Success) {
            if (r.Data) {
                LimparCamposTipoCargaEmbarcador();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Pessoas.GrupoPessoas.Sucesso, Localization.Resources.Pessoas.GrupoPessoas.RegistroAdicionadoSucesso);
                _gridTipoCargaEmbarcador.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.GrupoPessoas.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.GrupoPessoas.Falha, r.Msg);
        }
    });
}

function AtualizarTipoCargaEmbarcadorClick() {
    Salvar(_tipoCargaEmbarcador, "GrupoPessoasTipoCargaEmbarcador/Atualizar", function (r) {
        if (r.Success) {
            if (r.Data) {
                LimparCamposTipoCargaEmbarcador();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Pessoas.GrupoPessoas.Sucesso, Localization.Resources.Pessoas.GrupoPessoas.RegistroAtualizadoSucesso);
                _gridTipoCargaEmbarcador.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.GrupoPessoas.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.GrupoPessoas.Falha, r.Msg);
        }
    });
}

function ObterTiposCargaEmbarcador() {
    exibirConfirmacao(Localization.Resources.Pessoas.GrupoPessoas.Atencao, Localization.Resources.Pessoas.GrupoPessoas.DesejaBuscarNovamenteTiposCarga, function () {
        executarReST("GrupoPessoasTipoCargaEmbarcador/BuscarTiposCargaEmbarcador", {
            GrupoPessoas: _grupoPessoas.Codigo.val(),
            URLIntegracaoMultiEmbarcador: _grupoPessoas.URLIntegracaoMultiEmbarcador.val(),
            TokenIntegracaoMultiEmbarcador: _grupoPessoas.TokenIntegracaoMultiEmbarcador.val()
        }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Pessoas.GrupoPessoas.Sucesso, Localization.Resources.Pessoas.GrupoPessoas.RegistrosObtidosSucesso);
                    ObterTiposCargaDoEmbarcador();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.GrupoPessoas.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.GrupoPessoas.Falha, r.Msg);
            }
        });
    });
}

function LimparCamposTipoCargaEmbarcador() {
    LimparCampos(_tipoCargaEmbarcador);

    _tipoCargaEmbarcador.Adicionar.visible(true);
    _tipoCargaEmbarcador.Atualizar.visible(false);
    _tipoCargaEmbarcador.Cancelar.visible(false);
}