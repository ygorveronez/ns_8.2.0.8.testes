/// <reference path="Ocorrencia.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridOcorrenciaObservacaoFiscoContribuinte;

var ObservacaoFiscoContribuinteMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ type: types.map, val: "" });
    this.TipoCodigo = PropertyEntity({ type: types.map, val: "" });
    this.Identificador = PropertyEntity({ type: types.map, val: "" });
    this.Descricao = PropertyEntity({ type: types.map, val: "" });
};

//*******EVENTOS*******

function adicionarObservacaoFiscoContribuinteOcorrenciaClick() {
    if (_ocorrencia.Codigo.val() > 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.Ocorrencia.Atencao, Localization.Resources.Ocorrencias.Ocorrencia.NaoEPermitidoAdicionarEdicaoOcorrencia);

    _ocorrencia.DescricaoObservacaoFiscoContribuinte.required(true);
    _ocorrencia.IdentificadorObservacaoFiscoContribuinte.required(true);

    var valido = true;
    if (!ValidarCampoObrigatorioMap(_ocorrencia.IdentificadorObservacaoFiscoContribuinte) || !ValidarCampoObrigatorioMap(_ocorrencia.DescricaoObservacaoFiscoContribuinte))
        valido = false;

    _ocorrencia.DescricaoObservacaoFiscoContribuinte.required(false);
    _ocorrencia.IdentificadorObservacaoFiscoContribuinte.required(false);

    if (!valido)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.Ocorrencia.Atencao, Localization.Resources.Gerais.Geral.InformeOsCamposObrigatorios);

    var _observacaoFiscoContribuinte = new ObservacaoFiscoContribuinteMap();
    _observacaoFiscoContribuinte.Codigo.val = guid();
    _observacaoFiscoContribuinte.TipoCodigo.val = _ocorrencia.TipoObservacaoFiscoContribuinte.val();
    _observacaoFiscoContribuinte.Tipo.val = EnumTipoObservacaoCTe.ObterDescricao(_ocorrencia.TipoObservacaoFiscoContribuinte.val());
    _observacaoFiscoContribuinte.Identificador.val = _ocorrencia.IdentificadorObservacaoFiscoContribuinte.val();
    _observacaoFiscoContribuinte.Descricao.val = _ocorrencia.DescricaoObservacaoFiscoContribuinte.val();

    _ocorrencia.ObservacoesFiscoContribuinte.list.push(_observacaoFiscoContribuinte);

    recarregarGridOcorrenciaObservacaoFiscoContribuinte();
    limparCamposOcorrenciaObservacaoFiscoContribuinte();
}

function excluirObservacaoFiscoContribuinteClick(data) {
    if (_ocorrencia.Codigo.val() > 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.Ocorrencia.Atencao, Localization.Resources.Ocorrencias.Ocorrencia.NaoEPermitidoAdicionarEdicaoOcorrencia);

    exibirConfirmacao(Localization.Resources.Ocorrencias.Ocorrencia.Atencao, Localization.Resources.Ocorrencias.Ocorrencia.DesejaRealmenteExcluirObservacao, function () {
        for (var i = 0; i < _ocorrencia.ObservacoesFiscoContribuinte.list.length; i++) {
            if (data.Codigo == _ocorrencia.ObservacoesFiscoContribuinte.list[i].Codigo.val) {
                _ocorrencia.ObservacoesFiscoContribuinte.list.splice(i, 1);
                break;
            }
        }

        recarregarGridOcorrenciaObservacaoFiscoContribuinte();
    });
}

//*******MÉTODOS*******

function loadGridOcorrenciaObservacaoFiscoContribuinte() {
    var linhasPorPaginas = 2;

    var excluir = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Excluir, id: guid(), metodo: excluirObservacaoFiscoContribuinteClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Ocorrencias.Ocorrencia.Excluir, tamanho: 15, opcoes: [excluir] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Tipo", title: Localization.Resources.Ocorrencias.Ocorrencia.Tipo, width: "20%", className: "text-align-left" },
        { data: "Identificador", title: Localization.Resources.Ocorrencias.Ocorrencia.Identificador, width: "20%", className: "text-align-left" },
        { data: "Descricao", title: Localization.Resources.Ocorrencias.Ocorrencia.Descricao, width: "50%", className: "text-align-left" }
    ];

    _gridOcorrenciaObservacaoFiscoContribuinte = new BasicDataTable(_ocorrencia.GridObservacoesFiscoContribuinte.id, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridOcorrenciaObservacaoFiscoContribuinte.CarregarGrid([]);
}

function recarregarGridOcorrenciaObservacaoFiscoContribuinte() {
    var data = new Array();

    $.each(_ocorrencia.ObservacoesFiscoContribuinte.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.Tipo = item.Tipo.val;
        itemGrid.Identificador = item.Identificador.val;
        itemGrid.Descricao = item.Descricao.val;

        data.push(itemGrid);
    });

    _gridOcorrenciaObservacaoFiscoContribuinte.CarregarGrid(data);
}

function controlarCamposOcorrenciaObservacaoFiscoContribuinte(enable) {
    _ocorrencia.TipoObservacaoFiscoContribuinte.enable(enable);
    _ocorrencia.IdentificadorObservacaoFiscoContribuinte.enable(enable);
    _ocorrencia.DescricaoObservacaoFiscoContribuinte.enable(enable);
    _ocorrencia.AdicionarObservacaoFiscoContribuinte.visible(enable);
}

function limparCamposOcorrenciaObservacaoFiscoContribuinte() {
    LimparCampo(_ocorrencia.TipoObservacaoFiscoContribuinte);
    LimparCampo(_ocorrencia.IdentificadorObservacaoFiscoContribuinte);
    LimparCampo(_ocorrencia.DescricaoObservacaoFiscoContribuinte);
}