/// <reference path="../../Consultas/TipoOcorrencia.js" />

var _grupoPessoasOcorrencia;
var _gridOcorrencias;
var arrayOcorrencias = new Array();

var GrupoPessoaOcorrencia = function () {
    this.CodigoOcorrencia = PropertyEntity({ type: types.string });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.CodigoIntegracao.getRequiredFieldDescription(), required: true, visible: true, maxlength: 50, enable: ko.observable(true) });
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.GrupoPessoas.TipoOcorrencia.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarOcorrenciaClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirOcorrenciaClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Excluir, visible: ko.observable(false), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarOcorrenciaClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Atualizar, visible: ko.observable(false), enable: ko.observable(true) });
    this.Ocorrencias = PropertyEntity({ type: types.local });
}

function loadGrupoPessoasOcorrencia() {
    _grupoPessoasOcorrencia = new GrupoPessoaOcorrencia();
    KoBindings(_grupoPessoasOcorrencia, "knockoutOcorrencias");

    loadGridOcorrencias();
    new BuscarTipoOcorrencia(_grupoPessoasOcorrencia.TipoOcorrencia, null, null, null, null, null, null, null, null);
}

function loadGridOcorrencias() {
    var linhasPorPagina = 2;
    var opcaoEditar = { descricao: Localization.Resources.Pessoas.GrupoPessoas.Editar, id: guid(), evento: "onclick", metodo: EditarOcorrenciaClick, icone: "", visiblidade: true };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoTipoOcorrencia", visible: false },
        { data: "TipoOcorrencia", title: Localization.Resources.Pessoas.GrupoPessoas.TipoOcorrencia, width: "60%", className: "text-align-left" },
        { data: "CodigoIntegracao", title: Localization.Resources.Pessoas.GrupoPessoas.CodigoIntegracao, width: "40%", className: "text-align-left" }
    ];
    _gridOcorrencias = new BasicDataTable(_grupoPessoasOcorrencia.Ocorrencias.id, header, menuOpcoes, null, null, linhasPorPagina);
    _gridOcorrencias.CarregarGrid([]);
}

function AdicionarOcorrenciaClick() {
    if (!ValidarCampoObrigatorioEntity(_grupoPessoasOcorrencia.TipoOcorrencia) || !ValidarCamposObrigatorios(_grupoPessoasOcorrencia)) {
        exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.InformeCamposObrigatorios);
        return;
    }

    let ocorrencia = {
        Codigo: guid(),
        CodigoTipoOcorrencia: _grupoPessoasOcorrencia.TipoOcorrencia.codEntity(),
        TipoOcorrencia: _grupoPessoasOcorrencia.TipoOcorrencia.val(),
        CodigoIntegracao: _grupoPessoasOcorrencia.CodigoIntegracao.val()
    }
    arrayOcorrencias.push(ocorrencia);
    LimparCamposOcorrencia();
}

function AtualizarOcorrenciaClick() {
    if (!ValidarCampoObrigatorioEntity(_grupoPessoasOcorrencia.TipoOcorrencia) || !ValidarCamposObrigatorios(_grupoPessoasOcorrencia)) {
        exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.InformeCamposObrigatorios);
        return;
    }

    for (let i = 0; i < arrayOcorrencias.length; i++) {
        if (arrayOcorrencias[i].Codigo == _grupoPessoasOcorrencia.CodigoOcorrencia.val()) {
            arrayOcorrencias[i].TipoOcorrencia = _grupoPessoasOcorrencia.TipoOcorrencia.val();
            arrayOcorrencias[i].CodigoIntegracao = _grupoPessoasOcorrencia.CodigoIntegracao.val();
            arrayOcorrencias[i].CodigoTipoOcorrencia = _grupoPessoasOcorrencia.TipoOcorrencia.codEntity();
            break;
        }
    }
    LimparCamposOcorrencia();
    trocarBotoes();
}

function ExcluirOcorrenciaClick() {
    for (let i = 0; i < arrayOcorrencias.length; i++) {
        if (arrayOcorrencias[i].Codigo == _grupoPessoasOcorrencia.CodigoOcorrencia.val()) {
            arrayOcorrencias.splice(i, 1);
            break;
        }
    }
    LimparCamposOcorrencia();
    trocarBotoes();
}

function EditarOcorrenciaClick(registroSelecionado) {
    trocarBotoes();
    _grupoPessoasOcorrencia.CodigoOcorrencia.val(registroSelecionado.Codigo);
    _grupoPessoasOcorrencia.TipoOcorrencia.val(registroSelecionado.TipoOcorrencia);
    _grupoPessoasOcorrencia.CodigoIntegracao.val(registroSelecionado.CodigoIntegracao);
    _grupoPessoasOcorrencia.TipoOcorrencia.codEntity(registroSelecionado.CodigoTipoOcorrencia);
}

function LimparCamposOcorrencia() {
    _gridOcorrencias.CarregarGrid(arrayOcorrencias);
    _grupoPessoasOcorrencia.TipoOcorrencia.val("");
    _grupoPessoasOcorrencia.CodigoIntegracao.val("");
}

function RecarregarGridOcorrencias() {
    _gridOcorrencias.CarregarGrid(arrayOcorrencias);
}

function trocarBotoes() {
    _grupoPessoasOcorrencia.Adicionar.visible(!_grupoPessoasOcorrencia.Adicionar.visible());
    _grupoPessoasOcorrencia.Atualizar.visible(!_grupoPessoasOcorrencia.Atualizar.visible());
    _grupoPessoasOcorrencia.Excluir.visible(!_grupoPessoasOcorrencia.Excluir.visible());
}