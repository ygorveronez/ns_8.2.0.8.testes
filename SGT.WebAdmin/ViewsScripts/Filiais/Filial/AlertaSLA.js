/// <reference path="../../Enumeradores/EnumTipoAlertaSlnEmail.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />

var _cadastroAlertaSln;
var _alertaSln;
var _CRUDAlertaSln;
var _gridAlertaSln;

//Mapeamento Knockout

var AlertaSln = function () {
    this.Alertas = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.AdicionarAlerta = PropertyEntity({ eventClick: adicionarAlertaSlnModalClick, type: types.event, text: Localization.Resources.Filiais.Filial.AdicionarAlerta });
};

var CadastroAlertaSln = function () {
    var self = this;

    this.Codigo = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });
    this.NomeAlerta = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: Localization.Resources.Filiais.Filial.NomeAlerta.getRequiredFieldDescription(), required: true, maxlength: 50 });
    this.Etapas = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: Localization.Resources.Filiais.Filial.Etapas.getFieldDescription(), options: EnumEtapaFluxoGestaoPatio.obterOpcoesGatilhoOcorrenciaFinal(), visible: ko.observable(true), enable: ko.observable(true) });
    this.CorAlertaTempoFaltante = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: Localization.Resources.Filiais.Filial.CorEtapa.getFieldDescription() });
    this.CorAlertaTempoExcedido = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: Localization.Resources.Filiais.Filial.CorEtapa.getFieldDescription() });
    this.TempoFaltante = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), text: Localization.Resources.Filiais.Filial.TempoParaAlertar.getFieldDescription() });
    this.TempoFaltanteTransportador = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), text: Localization.Resources.Filiais.Filial.TempoParaAlertarTransportador.getFieldDescription(), visible: ko.observable(false) });
    this.TempoExcedido = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), text: Localization.Resources.Filiais.Filial.TempoParaAlertar.getRequiredFieldDescription(), configInt: { precision: 0, allowZero: true }, required: true });
    this.TempoExcedidoTransportador = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), text: Localization.Resources.Filiais.Filial.TempoParaAlertarTransportador.getFieldDescription(), visible: ko.observable(false) });
    this.AlertasEnviarEmail = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: Localization.Resources.Filiais.Filial.AlertarEmail.getFieldDescription(), options: EnumTipoAlertaSlnEmail.obterOpcoes(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Emails = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: Localization.Resources.Filiais.Filial.EmailsSeparadosPontoEVirgula.getFieldDescription(), maxlength: 300, visible: ko.observable(false) });
    this.AlertarTransportadorPorEmail = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Alertar transportador por e-mail", def: false, visible: ko.observable(false) });
  
    this.AlertasEnviarEmail.val.subscribe(function (novoValor) {
        if (novoValor > 0) {
            var exibirDadosEmail = true
        }

        self.Emails.visible(exibirDadosEmail);
        self.AlertarTransportadorPorEmail.visible(exibirDadosEmail);
        self.TempoFaltanteTransportador.visible(exibirDadosEmail && self.AlertarTransportadorPorEmail.val());
        self.TempoExcedidoTransportador.visible(exibirDadosEmail && self.AlertarTransportadorPorEmail.val());
    });

    this.AlertarTransportadorPorEmail.val.subscribe(function (novoValor) {
        var exibirDadosEmail = self.AlertasEnviarEmail.val().length > 0;

        self.TempoFaltanteTransportador.visible(exibirDadosEmail && novoValor);
        self.TempoExcedidoTransportador.visible(exibirDadosEmail && novoValor);
    });
};

var CRUDAlertaSln = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarFilialAlertaSlnClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarFilialAlertaSlnClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirFilialAlertaSlnClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
};

function loadAlertaSln() {
    _cadastroAlertaSln = new CadastroAlertaSln();
    KoBindings(_cadastroAlertaSln, "knockoutCadastroAlertaSln");

    _alertaSln = new AlertaSln();
    KoBindings(_alertaSln, "knockoutGestaoPatioAlertasSLA");

    _CRUDAlertaSln = new CRUDAlertaSln();
    KoBindings(_CRUDAlertaSln, "knockoutCRUDCadastroAlertaSln");

    loadGridAlertaSln();
}

function loadGridAlertaSln() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 1, dir: orderDir.asc };
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarAlertaSlnClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "NomeAlerta", title: Localization.Resources.Filiais.Filial.NomeAlerta, width: "80%", className: "text-align-left", orderable: false }
    ];

    _gridAlertaSln = new BasicDataTable(_alertaSln.Alertas.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridAlertaSln.CarregarGrid([]);
}

//Funções Públicas

function adicionarAlertaSlnModalClick() {
    abrirModalAlertaSln();
}

function atualizarFilialAlertaSlnClick() {
    if (!validarFilialAlertaSln())
        return;

    var listaFilialAlertasSln = obterListaFilialAlertasSln();

    for (var i = 0; i < listaFilialAlertasSln.length; i++) {
        if (_cadastroAlertaSln.Codigo.val() == listaFilialAlertasSln[i].Codigo) {
            listaFilialAlertasSln.splice(i, 1, obterFilialAlertaSlnSalvar());
            break;
        }
    }

    _alertaSln.Alertas.val(listaFilialAlertasSln);

    recarregarGridFilialAlertasSln();
    Global.fecharModal("divModalFilialAlertaSlnCadastro");
}

function adicionarFilialAlertaSlnClick() {
    if (!validarFilialAlertaSln())
        return;

    _alertaSln.Alertas.val().push(obterFilialAlertaSlnSalvar());

    recarregarGridFilialAlertasSln();
    Global.fecharModal("divModalFilialAlertaSlnCadastro");
}

function excluirFilialAlertaSlnClick() {
    var listaFilialAlertasSln = obterListaFilialAlertasSln();

    for (var i = 0; i < listaFilialAlertasSln.length; i++) {
        if (_cadastroAlertaSln.Codigo.val() == listaFilialAlertasSln[i].Codigo) {
            listaFilialAlertasSln.splice(i, 1);
            break;
        }
    }

    _alertaSln.Alertas.val(listaFilialAlertasSln);

    recarregarGridFilialAlertasSln();

    Global.fecharModal("divModalFilialAlertaSlnCadastro");
}

function editarAlertaSlnClick(registroSelecionado) {
    var registroCompleto = _alertaSln.Alertas.val().find(function (alerta) {
        return alerta.Codigo == registroSelecionado.Codigo;
    });

    PreencherObjetoKnout(_cadastroAlertaSln, { Data: registroCompleto });

    _CRUDAlertaSln.Atualizar.visible(true);
    _CRUDAlertaSln.Excluir.visible(true);
    _CRUDAlertaSln.Adicionar.visible(false);

    abrirModalAlertaSln();
}

function preencherAlertasSla(data) {
    _alertaSln.Alertas.val(data);
    recarregarGridFilialAlertasSln();
}

//Funções Privadas

function recarregarGridFilialAlertasSln() {
    var listaFilialAlertasSln = obterListaFilialAlertasSln();
    var listaFilialAlertasSlnCarregar = new Array();

    for (var i = 0; i < listaFilialAlertasSln.length; i++) {
        var filialAlertaSln = listaFilialAlertasSln[i];

        listaFilialAlertasSlnCarregar.push({
            Codigo: filialAlertaSln.Codigo,
            NomeAlerta: filialAlertaSln.NomeAlerta
        });
    }

    _gridAlertaSln.CarregarGrid(listaFilialAlertasSlnCarregar);
}

function limparCamposCadastroAlertaSln() {
    LimparCampos(_cadastroAlertaSln);
    _CRUDAlertaSln.Atualizar.visible(false);
    _CRUDAlertaSln.Excluir.visible(false);
    _CRUDAlertaSln.Adicionar.visible(true);
}

function limparCamposAlertasSln() {
    _alertaSln.Alertas.val([]);
    recarregarGridFilialAlertasSln();
}

function validarFilialAlertaSln() {
    if (!ValidarCamposObrigatorios(_cadastroAlertaSln)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return false;
    }

    return true;
}

function obterFilialAlertaSlnSalvar() {
    return {
        Codigo: !string.IsNullOrWhiteSpace(_cadastroAlertaSln.Codigo.val().toString()) ? _cadastroAlertaSln.Codigo.val() : guid(),
        NomeAlerta: _cadastroAlertaSln.NomeAlerta.val(),
        TempoFaltante: _cadastroAlertaSln.TempoFaltante.val(),
        TempoFaltanteTransportador: _cadastroAlertaSln.TempoFaltanteTransportador.val(),
        TempoExcedido: _cadastroAlertaSln.TempoExcedido.val(),
        TempoExcedidoTransportador: _cadastroAlertaSln.TempoExcedidoTransportador.val(),
        CorAlertaTempoFaltante: _cadastroAlertaSln.CorAlertaTempoFaltante.val(),
        CorAlertaTempoExcedido: _cadastroAlertaSln.CorAlertaTempoExcedido.val(),
        AlertasEnviarEmail: _cadastroAlertaSln.AlertasEnviarEmail.val(),
        AlertarTransportadorPorEmail: _cadastroAlertaSln.AlertarTransportadorPorEmail.val(),
        Emails: _cadastroAlertaSln.Emails.val(),
        Etapas: _cadastroAlertaSln.Etapas.val()
    };
}

function obterListaFilialAlertasSln() {
    return _alertaSln.Alertas.val().slice();
}

function obterListaFilialAlertasSlnSalvar() {
    var listaFilialAlertaSln = obterListaFilialAlertasSln();
    var listaFilialAlertasSlnSalvar = new Array();

    for (var i = 0; i < listaFilialAlertaSln.length; i++) {
        var filialAlertaSln = listaFilialAlertaSln[i];

        listaFilialAlertasSlnSalvar.push({
            Codigo: filialAlertaSln.Codigo,
            NomeAlerta: filialAlertaSln.NomeAlerta,
            TempoFaltante: filialAlertaSln.TempoFaltante,
            TempoFaltanteTransportador: filialAlertaSln.TempoFaltanteTransportador,
            TempoExcedido: filialAlertaSln.TempoExcedido,
            TempoExcedidoTransportador: filialAlertaSln.TempoExcedidoTransportador,
            CorAlertaTempoFaltante: filialAlertaSln.CorAlertaTempoFaltante,
            CorAlertaTempoExcedido: filialAlertaSln.CorAlertaTempoExcedido,
            AlertasEnviarEmail: filialAlertaSln.AlertasEnviarEmail,
            AlertarTransportadorPorEmail: filialAlertaSln.AlertarTransportadorPorEmail,
            Emails: filialAlertaSln.Emails,
            Etapas: filialAlertaSln.Etapas
        });
    }

    return JSON.stringify(listaFilialAlertasSlnSalvar);
}

function abrirModalAlertaSln() {
    $("#divModalFilialAlertaSlnCadastro")
        .modal("show")
        .on("hidden.bs.modal", function () {
            limparCamposCadastroAlertaSln();
        });
}

function preencherFilialAlertasSlnSalvar(filial) {
    filial["ListaAlertasSln"] = obterListaFilialAlertasSlnSalvar();
}