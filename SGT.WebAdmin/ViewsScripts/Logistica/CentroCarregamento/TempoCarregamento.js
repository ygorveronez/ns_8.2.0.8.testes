/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="CentroCarregamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _crudTempoCarregamentoCadastro;
var _gridTempoCarregamento;
var _opcoesTipoCarga = new Array();
var _tempoCarregamento;
var _tempoCarregamentoCadastro;

/*
 * Declaração das Classes
 */

var CRUDTempoCarregamentoCadastro = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarTempoCarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarTempoCarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirTempoCarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
};

var TempoCarregamento = function () {
    this.ListaTempoCarregamento = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), idBtnSearch: guid() });
    this.Adicionar = PropertyEntity({ eventClick: adicionarTempoCarregamentoModalClick, type: types.event, text: Localization.Resources.Logistica.CentroCarregamento.AdicionarDadoDeCarregamento });

    this.TipoCargaPesquisa = PropertyEntity({ type: types.map, getType: typesKnockout.selectMultiple, options: ko.observable([]), def: [], text: Localization.Resources.Logistica.CentroCarregamento.TipoDeCarga.getFieldDescription() });
    this.ModeloVeicularPesquisa = PropertyEntity({ type: types.map, getType: typesKnockout.selectMultiple, options: ko.observable([]), def: [], text: Localization.Resources.Logistica.CentroCarregamento.ModeloDoVeiculo.getFieldDescription() });
    this.Pesquisar = PropertyEntity({ eventClick: pesquisarTempoCarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar });

    this.Exportar = PropertyEntity({ eventClick: exportarTempoCarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Exportar });
    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Gerais.Geral.Importar,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        ManterArquivoServidor: true,
        UrlImportacao: "CentroCarregamento/Importar",
        UrlConfiguracao: "CentroCarregamento/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O041_TempoCarregamento,
        CallbackImportacao: function (arg) {
            adicionarTemposImportados(arg.Data.Retorno)
        },
    });
};

var TempoCarregamentoCadastro = function () {
    var self = this;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.HoraInicio = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: Localization.Resources.Logistica.CentroCarregamento.Inicio.getFieldDescription(), visible: _CONFIGURACAO_TMS.UtilizarTempoCarregamentoPorPeriodo });
    this.HoraTermino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: Localization.Resources.Logistica.CentroCarregamento.Termino.getFieldDescription(), visible: _CONFIGURACAO_TMS.UtilizarTempoCarregamentoPorPeriodo });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: Localization.Resources.Logistica.CentroCarregamento.ModeloDoVeiculo.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, issue: 44 });
    this.TipoCarga = PropertyEntity({ type: types.map, options: ko.observable(_opcoesTipoCarga), text: Localization.Resources.Logistica.CentroCarregamento.TipoDeCarga.getRequiredFieldDescription(), issue: 53, required: true });
    this.Tempo = PropertyEntity({ type: types.map, text: ko.observable(Localization.Resources.Logistica.CentroCarregamento.TempoMinutos.getRequiredFieldDescription()), val: ko.observable(""), def: "", getType: typesKnockout.int, visible: ko.observable(true), required: ko.observable(true), maxlength: 4 });
    this.QuantidadeMaximaEntregasRoteirizar = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.MaximoEntregasMontagemCarregamento.getFieldDescription(), visible: ko.observable(true) });
    this.QuantidadeMinimaEntregasRoteirizar = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Quantidade Minima Entregas", visible: ko.observable(true) });
    this.QuantidadeVagasOcuparGradeNaCarregamento = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.QuantidadeVagasOcuparGradeNaCarregamento.getFieldDescription(), visible: ko.observable(false) });

    this.TipoCarga.val.subscribe(function () {
        LimparCampo(_tempoCarregamentoCadastro.ModeloVeicular);
    });

    this.Tempo.required.subscribe(function (campoObrigatorio) {
        self.Tempo.text(campoObrigatorio ? Localization.Resources.Logistica.CentroCarregamento.TempoMinutos.getRequiredFieldDescription() : Localization.Resources.Logistica.CentroCarregamento.TempoMinutos.getFieldDescription());
    });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadTempoCarregamento() {
    _tempoCarregamento = new TempoCarregamento();
    KoBindings(_tempoCarregamento, "knockoutTempoCarregamento");

    _tempoCarregamentoCadastro = new TempoCarregamentoCadastro();
    KoBindings(_tempoCarregamentoCadastro, "knockoutTempoCarregamentoCadastro");

    _crudTempoCarregamentoCadastro = new CRUDTempoCarregamentoCadastro();
    KoBindings(_crudTempoCarregamentoCadastro, "knockoutCRUDTempoCarregamentoCadastro");

    new BuscarModelosVeicularesCarga(_tempoCarregamentoCadastro.ModeloVeicular, callbackModeloVeicular, null, null, null, null, null, _tempoCarregamentoCadastro.TipoCarga);

    loadGridTempoCarregamento();
    listaTempoCarregamentoAlterado();
}

function callbackModeloVeicular(dados) {
    _tempoCarregamentoCadastro.ModeloVeicular.codEntity(dados.Codigo);
    _tempoCarregamentoCadastro.ModeloVeicular.val(dados.Descricao);
    _tempoCarregamentoCadastro.ModeloVeicular.codIntegracao = dados.CodigoIntegracao;
}

function loadGridTempoCarregamento() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarTempoCarregamentoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoTipoCarga", visible: false },
        { data: "CodigoTipoCargaIntegracao", visible: false },
        { data: "CodigoModeloVeicular", visible: false },
        { data: "CodigoModeloVeicularIntegracao", visible: false },
        { data: "HoraInicio", visible: false },
        { data: "HoraTermino", visible: false },
        { data: "DescricaoTipoCarga", title: Localization.Resources.Logistica.CentroCarregamento.TipoDaCarga, width: "20%" },
        { data: "DescricaoModeloVeicular", title: Localization.Resources.Logistica.CentroCarregamento.ModeloDoVeiculo, width: "20%" },
        { data: "PeriodoCarregamento", title: Localization.Resources.Logistica.CentroCarregamento.Periodo, width: "20%", visible: _CONFIGURACAO_TMS.UtilizarTempoCarregamentoPorPeriodo },
        { data: "Tempo", title: Localization.Resources.Logistica.CentroCarregamento.Tempo, width: "20%" },
        { data: "QuantidadeMaximaEntregasRoteirizar", title: Localization.Resources.Logistica.CentroCarregamento.QuantidadeMaximaEntregas, width: "20%" },
        { data: "QuantidadeVagasOcuparGradeNaCarregamento", title: Localization.Resources.Logistica.CentroCarregamento.QuantidadeVagasOcuparGradeNaCarregamento, width: "20%", visible: false },
        { data: "QuantidadeMinimaEntregasRoteirizar", title: "Quantidade Minima Entregas", width: "20%" }
    ];

    _gridTempoCarregamento = new BasicDataTable(_tempoCarregamento.ListaTempoCarregamento.idGrid, header, menuOpcoes);
    _gridTempoCarregamento.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarTemposImportados(tempos) {
    tempos.map(function (tempo) {
        var registro = {
            Codigo: guid(),
            CodigoModeloVeicular: tempo.CodigoModeloVeicular,
            CodigoModeloVeicularIntegracao: tempo.CodigoModeloVeicularIntegracao,
            CodigoTipoCarga: tempo.CodigoTipoCarga,
            CodigoTipoCargaIntegracao: tempo.CodigoTipoCargaIntegracao,
            DescricaoModeloVeicular: tempo.DescricaoModeloVeicular,
            DescricaoTipoCarga: tempo.DescricaoTipoCarga,
            HoraInicio: tempo.HoraInicio,
            HoraTermino: tempo.HoraTermino,
            PeriodoCarregamento: obterDescricaoPeriodoCarregamentoSelecionadoNoCadastroTempoCarregamento(tempo.HoraInicio, tempo.HoraTermino),
            Tempo: tempo.Tempo,
            QuantidadeMaximaEntregasRoteirizar: tempo.QuantidadeMaximaEntregasRoteirizar,
            QuantidadeVagasOcuparGradeNaCarregamento: tempo.QuantidadeVagasOcuparGradeNaCarregamento,
            QuantidadeMinimaEntregasRoteirizar: tempo.QuantidadeMinimaEntregasRoteirizar
        };
        _tempoCarregamento.ListaTempoCarregamento.val().push(registro);
    });

    listaTempoCarregamentoAlterado();
    recarregarGridTempoCarregamento();
    fecharModalTempoCarregamentoCadastro();
}

function adicionarTempoCarregamentoClick() {
    if (!validarTempoCarregamento())
        return;

    _tempoCarregamento.ListaTempoCarregamento.val().push(obterTempoCarregamentoSalvar());

    listaTempoCarregamentoAlterado();
    recarregarGridTempoCarregamento();
    fecharModalTempoCarregamentoCadastro();
}

function pesquisarTempoCarregamentoClick() {
    recarregarGridTempoCarregamento();
}

function exportarTempoCarregamentoClick() {
    var listaTempoCarregamento = obterListaTempoCarregamento();

    executarDownloadPost("CentroCarregamento/ExportarDadosCarregamento", { Dados: JSON.stringify(listaTempoCarregamento) });
}

function importarTempoCarregamentoClick() {

}

function adicionarTempoCarregamentoModalClick() {
    _tempoCarregamentoCadastro.Codigo.val(guid());

    controlarBotoesTempoCarregamentoCadastroHabilitados(false);
    exibirModalTempoCarregamentoCadastro();
}

function atualizarTempoCarregamentoClick() {
    if (!validarTempoCarregamento())
        return;

    var listaTempoCarregamento = obterListaTempoCarregamento();

    for (var i = 0; i < listaTempoCarregamento.length; i++) {
        if (_tempoCarregamentoCadastro.Codigo.val() == listaTempoCarregamento[i].Codigo) {
            listaTempoCarregamento.splice(i, 1, obterTempoCarregamentoSalvar());
            break;
        }
    }

    _tempoCarregamento.ListaTempoCarregamento.val(listaTempoCarregamento)

    listaTempoCarregamentoAlterado();
    recarregarGridTempoCarregamento();
    fecharModalTempoCarregamentoCadastro();
}

function editarTempoCarregamentoClick(registroSelecionado) {
    var tempoCarregamento = obterTempoCarregamentoPorCodigo(registroSelecionado.Codigo);

    if (!tempoCarregamento)
        return;

    _tempoCarregamentoCadastro.Codigo.val(tempoCarregamento.Codigo);
    _tempoCarregamentoCadastro.HoraInicio.val(tempoCarregamento.HoraInicio);
    _tempoCarregamentoCadastro.HoraTermino.val(tempoCarregamento.HoraTermino);
    _tempoCarregamentoCadastro.Tempo.val(tempoCarregamento.Tempo);
    _tempoCarregamentoCadastro.QuantidadeMaximaEntregasRoteirizar.val(tempoCarregamento.QuantidadeMaximaEntregasRoteirizar);
    _tempoCarregamentoCadastro.QuantidadeMinimaEntregasRoteirizar.val(tempoCarregamento.QuantidadeMinimaEntregasRoteirizar);
    _tempoCarregamentoCadastro.QuantidadeVagasOcuparGradeNaCarregamento.val(tempoCarregamento.QuantidadeVagasOcuparGradeNaCarregamento);
    _tempoCarregamentoCadastro.TipoCarga.val(tempoCarregamento.CodigoTipoCarga);
    _tempoCarregamentoCadastro.TipoCarga.codIntegracao = tempoCarregamento.CodigoTipoCargaIntegracao;

    if (!obterDescricaoTipoCargaSelecionadoNoCadastroTempoCarregamento()) {
        _tempoCarregamentoCadastro.TipoCarga.val("");
        _tempoCarregamentoCadastro.TipoCarga.codIntegracao = "";
    }
    else {
        _tempoCarregamentoCadastro.ModeloVeicular.codEntity(tempoCarregamento.CodigoModeloVeicular);
        _tempoCarregamentoCadastro.ModeloVeicular.val(tempoCarregamento.DescricaoModeloVeicular);
        _tempoCarregamentoCadastro.ModeloVeicular.codIntegracao = tempoCarregamento.CodigoModeloVeicularIntegracao;
    }

    controlarBotoesTempoCarregamentoCadastroHabilitados(true);
    exibirModalTempoCarregamentoCadastro();
}

function excluirTempoCarregamentoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Logistica.CentroCarregamento.RealmenteDesejaExcluirDadoDeCarregamento, function () {
        var listaTempoCarregamento = obterListaTempoCarregamento();

        for (var i = 0; i < listaTempoCarregamento.length; i++) {
            if (_tempoCarregamentoCadastro.Codigo.val() == listaTempoCarregamento[i].Codigo)
                listaTempoCarregamento.splice(i, 1);
        }

        _tempoCarregamento.ListaTempoCarregamento.val(listaTempoCarregamento);

        listaTempoCarregamentoAlterado();
        recarregarGridTempoCarregamento();
        fecharModalTempoCarregamentoCadastro();
    });
}

/*
 * Declaração das Funções Públicas
 */

function listaTempoCarregamentoAlterado() {
    var listaTempoCarregamento = obterListaTempoCarregamento();

    var _codigosTipoCarga = [];
    var _codigosModeloVeicular = [];
    var _opcoesTipoCarga = [];
    var _opcoesModeloVeicular = [];

    for (var i = 0; i < listaTempoCarregamento.length; i++) {
        var tempoCarregamento = listaTempoCarregamento[i];

        if (_codigosTipoCarga.indexOf(tempoCarregamento.CodigoTipoCarga) < 0) {
            _codigosTipoCarga.push(tempoCarregamento.CodigoTipoCarga);
            _opcoesTipoCarga.push({
                text: tempoCarregamento.DescricaoTipoCarga,
                value: tempoCarregamento.CodigoTipoCarga
            });
        }

        if (_codigosModeloVeicular.indexOf(tempoCarregamento.CodigoModeloVeicular) < 0) {
            _codigosModeloVeicular.push(tempoCarregamento.CodigoModeloVeicular);
            _opcoesModeloVeicular.push({
                text: tempoCarregamento.DescricaoModeloVeicular,
                value: tempoCarregamento.CodigoModeloVeicular
            });
        }
    }

    _tempoCarregamento.TipoCargaPesquisa.options(_opcoesTipoCarga);
    _tempoCarregamento.ModeloVeicularPesquisa.options(_opcoesModeloVeicular);

    SetarSelectMultiple(_tempoCarregamento.TipoCargaPesquisa);
    SetarSelectMultiple(_tempoCarregamento.ModeloVeicularPesquisa);
}

function isTipoCargaPossuiTempoCarregamentoVinculado(codigoTipoCarga) {
    var listaTempoCarregamento = obterListaTempoCarregamento();

    for (var i = 0; i < listaTempoCarregamento.length; i++) {
        if (listaTempoCarregamento[i].CodigoTipoCarga == codigoTipoCarga)
            return true;
    }

    return false;
}

function limparCamposTempoCarregamento() {
    _tempoCarregamento.ListaTempoCarregamento.val([]);

    listaTempoCarregamentoAlterado();
    recarregarGridTempoCarregamento();
}

function preencherTempoCarregamento(dadosTempoCarregamento) {
    _tempoCarregamento.ListaTempoCarregamento.val(dadosTempoCarregamento);

    listaTempoCarregamentoAlterado();
    recarregarGridTempoCarregamento();
}

function preencherTempoCarregamentoSalvar(centroCarregamento) {
    centroCarregamento["TemposCarregamento"] = obterListaTempoCarregamentoSalvar();
}

/*
 * Declaração das Funções
 */

function controlarComponentesVisiveisPorLimiteCarregamentoAlterado() {
    var centroCarregamentoControladoPorTempo = IsCentroCarregamentoControladoPorTempo();

    _tempoCarregamentoCadastro.Tempo.visible(centroCarregamentoControladoPorTempo);
    _tempoCarregamentoCadastro.Tempo.required(centroCarregamentoControladoPorTempo);
    _tempoCarregamentoCadastro.QuantidadeVagasOcuparGradeNaCarregamento.visible(!centroCarregamentoControladoPorTempo);

    _gridTempoCarregamento.ControlarExibicaoColuna("Tempo", centroCarregamentoControladoPorTempo);
    _gridTempoCarregamento.ControlarExibicaoColuna("QuantidadeVagasOcuparGradeNaCarregamento", !centroCarregamentoControladoPorTempo);
}

function controlarBotoesTempoCarregamentoCadastroHabilitados(isEdicao) {
    _crudTempoCarregamentoCadastro.Atualizar.visible(isEdicao);
    _crudTempoCarregamentoCadastro.Excluir.visible(isEdicao);
    _crudTempoCarregamentoCadastro.Adicionar.visible(!isEdicao);
}

function exibirModalTempoCarregamentoCadastro() {
    Global.abrirModal('divModalTempoCarregamentoCadastro');
    $("#divModalTempoCarregamentoCadastro").one('hidden.bs.modal', function () {
        limparCamposTempoCarregamentoCadastro();
    });
}

function fecharModalTempoCarregamentoCadastro() {
    Global.fecharModal('divModalTempoCarregamentoCadastro');
}

function limparCamposTempoCarregamentoCadastro() {
    LimparCampos(_tempoCarregamentoCadastro);
}

function obterDescricaoPeriodoCarregamentoSelecionadoNoCadastroTempoCarregamento(horaInicio, horaTermino) {
    if (Boolean(horaInicio) && Boolean(horaTermino))
        return Localization.Resources.Logistica.CentroCarregamento.Das + " " + horaInicio + " " + Localization.Resources.Logistica.CentroCarregamento.AteAs + " " + horaTermino;

    if (Boolean(horaInicio))
        return Localization.Resources.Logistica.CentroCarregamento.PartirDas + " " + horaInicio;

    if (Boolean(horaTermino))
        return Localization.Resources.Logistica.CentroCarregamento.AteAs + " " + horaTermino;

    return "";
}

function obterDescricaoTipoCargaSelecionadoNoCadastroTempoCarregamento() {
    var $opcaoSelecionada = $("#" + _tempoCarregamentoCadastro.TipoCarga.id + " option[value='" + _tempoCarregamentoCadastro.TipoCarga.val() + "']");

    if ($opcaoSelecionada.length > 0)
        return $opcaoSelecionada.text();

    return "";
}

function obterTempoCarregamentoPorCodigo(codigo) {
    var listaTempoCarregamento = obterListaTempoCarregamento();

    for (var i = 0; i < listaTempoCarregamento.length; i++) {
        var tempoCarregamento = listaTempoCarregamento[i];

        if (codigo == tempoCarregamento.Codigo)
            return tempoCarregamento;
    }

    return undefined;
}

function obterTempoCarregamentoSalvar() {
    var horaInicio = _tempoCarregamentoCadastro.HoraInicio.val();
    var horaTermino = _tempoCarregamentoCadastro.HoraTermino.val();

    return {
        Codigo: _tempoCarregamentoCadastro.Codigo.val(),
        CodigoModeloVeicular: _tempoCarregamentoCadastro.ModeloVeicular.codEntity(),
        CodigoModeloVeicularIntegracao: _tempoCarregamentoCadastro.ModeloVeicular.codIntegracao,
        CodigoTipoCarga: _tempoCarregamentoCadastro.TipoCarga.val(),
        CodigoTipoCargaIntegracao: ObterCodigoIntegracaoTipoCargaPorCodigo(_tempoCarregamentoCadastro.TipoCarga.val()),
        DescricaoModeloVeicular: _tempoCarregamentoCadastro.ModeloVeicular.val(),
        DescricaoTipoCarga: obterDescricaoTipoCargaSelecionadoNoCadastroTempoCarregamento(),
        HoraInicio: _tempoCarregamentoCadastro.HoraInicio.val(),
        HoraTermino: _tempoCarregamentoCadastro.HoraTermino.val(),
        PeriodoCarregamento: obterDescricaoPeriodoCarregamentoSelecionadoNoCadastroTempoCarregamento(horaInicio, horaTermino),
        Tempo: _tempoCarregamentoCadastro.Tempo.val(),
        QuantidadeMaximaEntregasRoteirizar: _tempoCarregamentoCadastro.QuantidadeMaximaEntregasRoteirizar.val(),
        QuantidadeMinimaEntregasRoteirizar: _tempoCarregamentoCadastro.QuantidadeMinimaEntregasRoteirizar.val(),
        QuantidadeVagasOcuparGradeNaCarregamento: _tempoCarregamentoCadastro.QuantidadeVagasOcuparGradeNaCarregamento.val()
    };
}

function obterListaTempoCarregamento() {
    return _tempoCarregamento.ListaTempoCarregamento.val().slice();
}

function obterListaTempoCarregamentoSalvar() {
    var listaTempoCarregamento = obterListaTempoCarregamento();
    var listaTempoCarregamentoSalvar = new Array();

    for (var i = 0; i < listaTempoCarregamento.length; i++) {
        var tempoCarregamento = listaTempoCarregamento[i];

        listaTempoCarregamentoSalvar.push({
            Codigo: tempoCarregamento.Codigo,
            CodigoModeloVeicular: tempoCarregamento.CodigoModeloVeicular,
            CodigoTipoCarga: tempoCarregamento.CodigoTipoCarga,
            HoraInicio: tempoCarregamento.HoraInicio,
            HoraTermino: tempoCarregamento.HoraTermino,
            Tempo: tempoCarregamento.Tempo,
            QuantidadeMaximaEntregasRoteirizar: tempoCarregamento.QuantidadeMaximaEntregasRoteirizar,
            QuantidadeVagasOcuparGradeNaCarregamento: tempoCarregamento.QuantidadeVagasOcuparGradeNaCarregamento,
            QuantidadeMinimaEntregasRoteirizar: tempoCarregamento.QuantidadeMinimaEntregasRoteirizar
        });
    }

    return JSON.stringify(listaTempoCarregamentoSalvar);
}

function recarregarGridTempoCarregamento() {
    var listaTempoCarregamento = obterListaTempoCarregamento();

    var filtroTipoCarga = _tempoCarregamento.TipoCargaPesquisa.val();
    var filtroModeloVeicular = _tempoCarregamento.ModeloVeicularPesquisa.val();

    listaTempoCarregamento = listaTempoCarregamento.filter(function (tempo) {
        if (filtroTipoCarga.length > 0 && filtroTipoCarga.indexOf(tempo.CodigoTipoCarga) < 0) return false;
        if (filtroModeloVeicular.length > 0 && filtroModeloVeicular.indexOf(tempo.CodigoModeloVeicular) < 0) return false;

        return true;
    });

    _gridTempoCarregamento.CarregarGrid(listaTempoCarregamento);
}

function validarTempoCarregamento() {
    if (!ValidarCamposObrigatorios(_tempoCarregamentoCadastro)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return false;
    }

    var listaTempoCarregamento = obterListaTempoCarregamento();
    var horaInicioPadrao = moment("00:00", "HH:mm");
    var horaTerminoPadrao = moment("23:59", "HH:mm");
    var horaInicioSalvar = Boolean(_tempoCarregamentoCadastro.HoraInicio.val()) ? moment(_tempoCarregamentoCadastro.HoraInicio.val(), "HH:mm") : horaInicioPadrao;
    var horaTerminoSalvar = Boolean(_tempoCarregamentoCadastro.HoraTermino.val()) ? moment(_tempoCarregamentoCadastro.HoraTermino.val(), "HH:mm") : horaTerminoPadrao;

    if (!horaTerminoSalvar.isAfter(horaInicioSalvar)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Logistica.CentroCarregamento.ValoresInvalidos, Localization.Resources.Logistica.CentroCarregamento.HoraDeInicioDeveSerSuperiorDeTermino);
        return false;
    }

    for (var i = 0; i < listaTempoCarregamento.length; i++) {
        var tempoCarregamento = listaTempoCarregamento[i];

        if (
            (tempoCarregamento.Codigo != _tempoCarregamentoCadastro.Codigo.val()) &&
            (tempoCarregamento.CodigoTipoCarga == _tempoCarregamentoCadastro.TipoCarga.val()) &&
            (tempoCarregamento.CodigoModeloVeicular == _tempoCarregamentoCadastro.ModeloVeicular.codEntity())
        ) {
            if (!_CONFIGURACAO_TMS.UtilizarTempoCarregamentoPorPeriodo) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Logistica.CentroCarregamento.JaFoiCadastradoUmDadoDeCarregamentoParaTipoDeCargaModeloVeicularSelecionados);
                return false;
            }

            var horaInicio = Boolean(tempoCarregamento.HoraInicio) ? moment(tempoCarregamento.HoraInicio, "HH:mm") : horaInicioPadrao;
            var horaTermino = Boolean(tempoCarregamento.HoraTermino) ? moment(tempoCarregamento.HoraTermino, "HH:mm") : horaTerminoPadrao;

            var periodoDuplicado = (
                (!horaInicioSalvar.isBefore(horaInicio) && !horaInicioSalvar.isAfter(horaTermino)) ||
                (!horaTerminoSalvar.isBefore(horaInicio) && !horaTerminoSalvar.isAfter(horaTermino)) ||
                (!horaInicio.isBefore(horaInicioSalvar) && !horaInicio.isAfter(horaTerminoSalvar)) ||
                (!horaTermino.isBefore(horaInicioSalvar) && !horaTermino.isAfter(horaTerminoSalvar))
            );

            if (periodoDuplicado) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Logistica.CentroCarregamento.JaFoiCadastradoUmDadoDeCarregamentoParaTipoDeCargaModeloVeicularSelecionados);
                return false;
            }
        }
    }

    return true;
}
