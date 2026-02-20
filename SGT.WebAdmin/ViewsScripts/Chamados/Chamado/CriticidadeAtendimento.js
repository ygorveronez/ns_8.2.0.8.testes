/// <reference path="Analise.js" />

var _gridCriticidadeAtendimento;
var _gerenciaisDisponiveis = [];
var _causasProblemaDisponiveis = [];
var _criticidadeInicializada = false;

var CriticidadeAtendimentoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.Critico = PropertyEntity({ type: types.map, val: ko.observable(false) });
    this.CodigoGerencial = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.CodigoCausaProblema = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.FUP = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Gerencial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), idBtnSearch: guid() });
    this.CausaProblema = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), idBtnSearch: guid() });
};

function loadCriticidadeAtendimento() {
    loadGridCriticidadeAtendimento();
}

function VerificarEConfigurarCriticidade(codigoMotivo) {
    if (!_motivoChamadoConfiguracao || !_motivoChamadoConfiguracao.HabilitarClassificacaoCriticos || !_CONFIGURACAO_TMS.TipoServicoMultisoftware) {
        $("#liTabCriticidadeAtendimento").hide();
        controleCamposCriticidade(false);
        return;
    }

    if (!_criticidadeInicializada) {
        _buscaGerencial = new BuscarTiposCriticidadeAtendimento(_analise.Gerencial, "Gerencial", _abertura.MotivoChamado);
        _buscaCausaProblema = new BuscarTiposCriticidadeAtendimento(_analise.CausaProblema, "CausaProblema", _abertura.MotivoChamado);
        _criticidadeInicializada = true;
    }

    $("#liTabCriticidadeAtendimento").show();
    controleCamposCriticidade(true);

    if (codigoMotivo && codigoMotivo > 0) {
        carregarTiposCriticidadePorMotivo(codigoMotivo);
    }
}

function carregarTiposCriticidadePorMotivo(codigoMotivo) {
    if (!codigoMotivo || codigoMotivo <= 0) {
        _gerenciaisDisponiveis = [];
        _causasProblemaDisponiveis = [];
        LimparCampoEntity(_analise.Gerencial);
        LimparCampoEntity(_analise.CausaProblema);
        return;
    }

    executarReST("MotivoChamado/BuscarTiposCriticidadePorMotivo",
        { CodigoMotivo: codigoMotivo },
        function (result) {
            if (!result.Success) {
                return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, result.Msg);
            }

            _gerenciaisDisponiveis = result.Data.Gerenciais || [];
            _causasProblemaDisponiveis = result.Data.CausasProblema || [];

            if (_gerenciaisDisponiveis.length > 0) {
                var codGerAtual = (typeof _analise.Gerencial.codEntity === "function" && _analise.Gerencial.codEntity()) || 0;

                if (codGerAtual > 0) {
                    var itemGer = _gerenciaisDisponiveis.find(x => x.Codigo === codGerAtual);
                    if (itemGer) {
                        _analise.Gerencial.codEntity(itemGer.Codigo);
                        _analise.Gerencial.val(itemGer.Descricao);
                    } else {
                        LimparCampoEntity(_analise.Gerencial);
                    }
                } else {
                    LimparCampoEntity(_analise.Gerencial);
                }
            } else {
                LimparCampoEntity(_analise.Gerencial);
            }

            if (_causasProblemaDisponiveis.length > 0) {
                var codCausaAtual = (typeof _analise.CausaProblema.codEntity === "function" && _analise.CausaProblema.codEntity()) || 0;
                if (codCausaAtual > 0) {
                    var itemCausa = _causasProblemaDisponiveis.find(x => x.Codigo === codCausaAtual);
                    if (itemCausa) {
                        _analise.CausaProblema.codEntity(itemCausa.Codigo);
                        _analise.CausaProblema.val(itemCausa.Descricao);
                    } else {
                        LimparCampoEntity(_analise.CausaProblema);
                    }
                } else {
                    LimparCampoEntity(_analise.CausaProblema);
                }
            } else {
                LimparCampoEntity(_analise.CausaProblema);
            }
        }
    );
}

function validarCriticidadeObrigatoria() {
    if (!_motivoChamadoConfiguracao || !_motivoChamadoConfiguracao.HabilitarClassificacaoCriticos) {
        return true;
    }

    var criticoVal = _analise.Critico.val();
    if (criticoVal !== 1) {
        return true;
    }

    var gerencialOk = _analise.Gerencial.codEntity() > 0;
    var causaProblemaOk = _analise.CausaProblema.codEntity() > 0;
    var fupOk = _analise.FUP.val() && _analise.FUP.val().trim().length > 0;

    if (!gerencialOk || !causaProblemaOk || !fupOk) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Chamado.ChamadoOcorrencia);
        $("#liTabCriticidadeAtendimento").tab('show');
        return false;
    }

    return true;
}

function obterDadosCriticidade() {
    if (!_motivoChamadoConfiguracao || !_motivoChamadoConfiguracao.HabilitarClassificacaoCriticos) {
        return null;
    }

    var criticoVal = _analise.Critico && typeof _analise.Critico.val === "function" ? _analise.Critico.val() : false;

    var criticidadeAtendimento =
    {
        critico: (criticoVal === 1) || (criticoVal === true),
        codigoGerencial: _analise.Gerencial ? (_analise.Gerencial.codEntity() || 0) : 0,
        codigoCausaProblema: _analise.CausaProblema ? (_analise.CausaProblema.codEntity() || 0) : 0,
        fup: _analise.FUP ? (_analise.FUP.val() || "") : ""
    }
    return JSON.stringify(criticidadeAtendimento);
}

function loadGridCriticidadeAtendimento() {
    if (_gridCriticidadeAtendimento != null && _gridCriticidadeAtendimento.Destroy)
        _gridCriticidadeAtendimento.Destroy();

    var excluir = {
        descricao: "Excluir",
        id: guid(),
        metodo: excluirCriticidadeClick,
        icone: "",
        visibilidade: visibilidadeExcluirCriticidade
    };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [excluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoGerencial", visible: false },
        { data: "CodigoCausaProblema", visible: false },
        { data: "Critico", title: "Crítico", width: "10%" },
        { data: "Gerencial", title: "Gerencial", width: "25%" },
        { data: "CausaProblema", title: "Causa do Problema", width: "30%" },
        { data: "FUP", title: "FUP", width: "35%" }
    ];
    const gridId = _analise.GridCriticidade.idGrid;
    if (!document.getElementById(gridId)) return;
    _gridCriticidadeAtendimento = new BasicDataTable(_analise.GridCriticidade.id, header, menuOpcoes);
}

function visibilidadeExcluirCriticidade() {
    return _chamado.Situacao.val() === EnumSituacaoChamado.Aberto ||
        _chamado.Situacao.val() === EnumSituacaoChamado.EmTratativa;
}

function excluirCriticidadeClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir este registro de criticidade?", function () {
        for (var i = 0; i < _analise.CriticidadeAtendimento.list.length; i++) {
            if (data.Codigo == _analise.CriticidadeAtendimento.list[i].Codigo.val) {
                _analise.CriticidadeAtendimento.list.splice(i, 1);
                break;
            }
        }
        recarregarGridCriticidade();
    });
}

function recarregarGridCriticidade() {
    var data = [];

    for (var i = 0; i < _analise.CriticidadeAtendimento.list.length; i++) {
        var lista = _analise.CriticidadeAtendimento.list[i];

        var listaGrid = {
            Codigo: lista.Codigo.val,
            Critico: lista.Critico.val ? "Sim" : "Não",
            CodigoGerencial: lista.CodigoGerencial.val,
            Gerencial: lista.Gerencial.val,
            CodigoCausaProblema: lista.CodigoCausaProblema.val,
            CausaProblema: lista.CausaProblema.val,
            FUP: lista.FUP.val
        };

        data.push(listaGrid);
    }

    _gridCriticidadeAtendimento.CarregarGrid(data);
}

function controleCamposCriticidade(novoValor) {
    _analise.Critico.visible(novoValor);
    _analise.Gerencial.visible(novoValor);
    _analise.CausaProblema.visible(novoValor);
    _analise.FUP.visible(novoValor);
}

function preencherCriticidadeDoChamado(ch) {
    if (!ch) return;

    if (_analise.Critico && typeof _analise.Critico.val === "function") {
        var crit = (ch.Critico === 1 || ch.Critico === true);
        _analise.Critico.val(crit ? 1 : 0);
    }

    if (_analise.FUP && typeof _analise.FUP.val === "function") {
        _analise.FUP.val(ch.FUP || "");
    }

    if (_analise.Gerencial && typeof _analise.Gerencial.codEntity === "function") {
        var codGer = ch.CodigoGerencial || ch.Gerencial?.Codigo || 0;
        var descGer = ch.GerencialDescricao || ch.Gerencial?.Descricao || "";
        _analise.Gerencial.codEntity(codGer);
        if (descGer) _analise.Gerencial.val(descGer);
    }

    if (_analise.CausaProblema && typeof _analise.CausaProblema.codEntity === "function") {
        var codCausa = ch.CodigoCausaProblema || ch.CausaProblema?.Codigo || 0;
        var descCausa = ch.CausaProblemaDescricao || ch.CausaProblema?.Descricao || "";
        _analise.CausaProblema.codEntity(codCausa);
        if (descCausa) _analise.CausaProblema.val(descCausa);
    }
}