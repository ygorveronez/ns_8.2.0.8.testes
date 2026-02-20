var _gridModeloVeicularEmbarcador, _pesquisaModeloVeicularEmbarcador, _modeloVeicularEmbarcador;

var PesquisaModeloVeicularEmbarcador = function () {
    this.GrupoPessoas = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridModeloVeicularEmbarcador.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ObterModelosVeicularesEmbarcador = PropertyEntity({
        eventClick: function (e) {
            ObterModelosVeicularesEmbarcador();
        }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.BuscarModelosVeicularesEmbarcador, idGrid: guid(), visible: ko.observable(true)
    });
};

var ModeloVeicularEmbarcador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.ModeloVeicularEmbarcador = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ModeloVeicularEmbarcador.getRequiredFieldDescription(), options: ko.observable([]), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true), enable: ko.observable(true), required: true });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, text: Localization.Resources.Pessoas.GrupoPessoas.ModeloVeicular.getRequiredFieldDescription(), idBtnSearch: guid(), issue: 0, codEntity: ko.observable(0), visible: ko.observable(true), required: true, enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarModeloVeicularEmbarcadorClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarModeloVeicularEmbarcadorClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Atualizar, visible: ko.observable(false), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: LimparCamposModeloVeicularEmbarcador, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Cancelar, visible: ko.observable(false), enable: ko.observable(true) });
};

function LoadModeloVeicularEmbarcador() {
    _pesquisaModeloVeicularEmbarcador = new PesquisaModeloVeicularEmbarcador();
    KoBindings(_pesquisaModeloVeicularEmbarcador, "knockoutPesquisaModeloVeicularEmbarcador");

    _modeloVeicularEmbarcador = new ModeloVeicularEmbarcador();
    KoBindings(_modeloVeicularEmbarcador, "knockoutModeloVeicularEmbarcador");

    _pesquisaModeloVeicularEmbarcador.GrupoPessoas = _grupoPessoas.Codigo;

    new BuscarModelosVeicularesCarga(_modeloVeicularEmbarcador.ModeloVeicular);

    LoadGridModeloVeicularEmbarcador();
}

function LoadGridModeloVeicularEmbarcador() {
    var editar = { descricao: Localization.Resources.Pessoas.GrupoPessoas.Editar, id: guid(), evento: "onclick", metodo: EditarModeloVeicularEmbarcador, tamanho: "15", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridModeloVeicularEmbarcador = new GridView(_pesquisaModeloVeicularEmbarcador.Pesquisar.idGrid, "GrupoPessoasModeloVeicularEmbarcador/Pesquisa", _pesquisaModeloVeicularEmbarcador, menuOpcoes);

    $("#liConfiguracaoMultiEmbarcador").on("click", function () {
        _gridModeloVeicularEmbarcador.CarregarGrid();
        ObterModelosVeicularesDoEmbarcador();
    });
}

function ObterModelosVeicularesDoEmbarcador() {
    executarReST("GrupoPessoasModeloVeicularEmbarcador/ObterModelosDoEmbarcador", { GrupoPessoas: _grupoPessoas.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {

                var opcoes = new Array();

                for (var i = 0; i < r.Data.length; i++) 
                    opcoes.push({ text: r.Data[i].Descricao, value: r.Data[i].Codigo });
                
                _modeloVeicularEmbarcador.ModeloVeicularEmbarcador.options(opcoes);

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.GrupoPessoas.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.GrupoPessoas.Falha, r.Msg);
        }
    });
}

function EditarModeloVeicularEmbarcador(obj) {
    executarReST("GrupoPessoasModeloVeicularEmbarcador/BuscarPorCodigo", { Codigo: obj.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _modeloVeicularEmbarcador.Adicionar.visible(false);
                _modeloVeicularEmbarcador.Atualizar.visible(true);
                _modeloVeicularEmbarcador.Cancelar.visible(true);
                
                _modeloVeicularEmbarcador.Codigo.val(r.Data.Codigo);
                _modeloVeicularEmbarcador.ModeloVeicularEmbarcador.val(r.Data.DescricaoModeloVeicularEmbarcador);
                _modeloVeicularEmbarcador.ModeloVeicular.val(r.Data.ModeloVeicular.Descricao);
                _modeloVeicularEmbarcador.ModeloVeicular.codEntity(r.Data.ModeloVeicular.Codigo);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.GrupoPessoas.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.GrupoPessoas.Falha, r.Msg);
        }
    });
}

function AdicionarModeloVeicularEmbarcadorClick() {
    Salvar(_modeloVeicularEmbarcador, "GrupoPessoasModeloVeicularEmbarcador/Adicionar", function (r) {
        if (r.Success) {
            if (r.Data) {
                LimparCamposModeloVeicularEmbarcador();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Pessoas.GrupoPessoas.Sucesso, Localization.Resources.Pessoas.GrupoPessoas.RegistroAdicionadoSucesso);
                _gridModeloVeicularEmbarcador.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.GrupoPessoas.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.GrupoPessoas.Falha, r.Msg);
        }
    });
}

function AtualizarModeloVeicularEmbarcadorClick() {
    Salvar(_modeloVeicularEmbarcador, "GrupoPessoasModeloVeicularEmbarcador/Atualizar", function (r) {
        if (r.Success) {
            if (r.Data) {
                LimparCamposModeloVeicularEmbarcador();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Pessoas.GrupoPessoas.Sucesso, Localization.Resources.Pessoas.GrupoPessoas.RegistroAtualizadoSucesso);
                _gridModeloVeicularEmbarcador.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.GrupoPessoas.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.GrupoPessoas.Falha, r.Msg);
        }
    });
}

function ObterModelosVeicularesEmbarcador() {
    exibirConfirmacao(Localization.Resources.Pessoas.GrupoPessoas.Atencao, Localization.Resources.Pessoas.GrupoPessoas.DesejaBuscarNovamenteModelosVeiculares, function () {
        executarReST("GrupoPessoasModeloVeicularEmbarcador/BuscarModelosVeicularesEmbarcador", {
            GrupoPessoas: _grupoPessoas.Codigo.val(),
            URLIntegracaoMultiEmbarcador: _grupoPessoas.URLIntegracaoMultiEmbarcador.val(),
            TokenIntegracaoMultiEmbarcador: _grupoPessoas.TokenIntegracaoMultiEmbarcador.val()
        }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Pessoas.GrupoPessoas.Sucesso, Localization.Resources.Pessoas.GrupoPessoas.RegistrosObtidosSucesso);
                    ObterModelosVeicularesDoEmbarcador();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.GrupoPessoas.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.GrupoPessoas.Falha, r.Msg);
            }
        });
    });
}

function LimparCamposModeloVeicularEmbarcador() {
    LimparCampos(_modeloVeicularEmbarcador);

    _modeloVeicularEmbarcador.Adicionar.visible(true);
    _modeloVeicularEmbarcador.Atualizar.visible(false);
    _modeloVeicularEmbarcador.Cancelar.visible(false);
}