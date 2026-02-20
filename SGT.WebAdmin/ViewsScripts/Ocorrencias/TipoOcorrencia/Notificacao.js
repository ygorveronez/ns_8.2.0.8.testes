/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="TipoOcorrencia.js" />

var _notificacao;
var _gridListaEmail;

var Notificacao = function () {
    this.NotificarTransportadorPorEmail = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.NotificarTransportadorPorEmail, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NotificarEmail = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.NotificarEmail, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NotificarClientePorEmail = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.NotificarClientePorEmail, getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.AssuntoEmail = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.AssuntoEmail.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 100 });

    this.TagAssuntoNumeroOcorrencia = PropertyEntity({ eventClick: function (e) { InserirTag(_notificacao.AssuntoEmail.id, "#NumeroOcorrencia"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.NumeroOcorrencia });
    this.TagAssuntoRazaoTransportador = PropertyEntity({ eventClick: function (e) { InserirTag(_notificacao.AssuntoEmail.id, "#RazaoTransportador"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.RazaoTransportador });
    this.TagAssuntoCNPJTransportador = PropertyEntity({ eventClick: function (e) { InserirTag(_notificacao.AssuntoEmail.id, "#CNPJTransportador"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.CNPJTransportador });
    this.TagAssuntoTipoOcorrencia = PropertyEntity({ eventClick: function (e) { InserirTag(_notificacao.AssuntoEmail.id, "#TipoOcorrencia"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.TipoDeOcorrencia });
    this.TagAssuntoValorOcorrencia = PropertyEntity({ eventClick: function (e) { InserirTag(_notificacao.AssuntoEmail.id, "#ValorOcorrencia"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.ValorOcorrencia });
    this.TagAssuntoNumeroCarga = PropertyEntity({ eventClick: function (e) { InserirTag(_notificacao.AssuntoEmail.id, "#NumeroCarga"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.NumeroCarga });
    this.TagAssuntoNumeroPedido = PropertyEntity({ eventClick: function (e) { InserirTag(_notificacao.AssuntoEmail.id, "#NumeroPedido"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.NumeroPedido });
    this.TagAssuntoNumeroOrdem = PropertyEntity({ eventClick: function (e) { InserirTag(_notificacao.AssuntoEmail.id, "#NumeroOrdem"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.NumeroOrdem });

    this.CorpoEmail = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.CorpoEmail.getFieldDescription() });

    this.TagCorpoNumeroOcorrencia = PropertyEntity({ eventClick: function (e) { inserirTagCorpoEmail("#NumeroOcorrencia"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.NumeroOcorrencia });
    this.TagCorpoRazaoTransportador = PropertyEntity({ eventClick: function (e) { inserirTagCorpoEmail("#RazaoTransportador"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.RazaoTransportador });
    this.TagCorpoCNPJTransportador = PropertyEntity({ eventClick: function (e) { inserirTagCorpoEmail("#CNPJTransportador"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.CNPJTransportador });
    this.TagCorpoTipoOcorrencia = PropertyEntity({ eventClick: function (e) { inserirTagCorpoEmail("#TipoOcorrencia"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.TipoDeOcorrencia });
    this.TagCorpoValorOcorrencia = PropertyEntity({ eventClick: function (e) { inserirTagCorpoEmail("#ValorOcorrencia"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.ValorOcorrencia });
    this.TagCorpoNumeroCarga = PropertyEntity({ eventClick: function (e) { inserirTagCorpoEmail("#NumeroCarga"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.NumeroCarga });
    this.TagCorpoNumeroPedido = PropertyEntity({ eventClick: function (e) { inserirTagCorpoEmail("#NumeroPedido"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.NumeroPedido });
    this.TagCorpoNumeroOrdem = PropertyEntity({ eventClick: function (e) { inserirTagCorpoEmail("#NumeroOrdem"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.NumeroOrdem });

    this.ListaEmail = PropertyEntity({ type: types.local, val: ko.observableArray([]) });

    this.Grid = PropertyEntity({ type: types.local });
    this.Email = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.Email, required: false, getType: typesKnockout.email, maxlength: 1000 });

    this.AdicionarEmail = PropertyEntity({ eventClick: adicionarListaEmailClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });

};

function loadNotificacao() {
    _notificacao = new Notificacao();
    KoBindings(_notificacao, "knockoutNotificacao");

    $("#corpoEmail").summernote({
        toolbar: [
            ['style', ['style']],
            ['font', ['bold', 'underline', 'clear']],
            ['fontname', ['fontname']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['table', ['table']],
            ['insert', ['link', 'picture']],
            ['view', ['fullscreen', 'codeview']],
        ]
    });

    $("#liTabNotificacao").hide();
    $("#" + _tipoOcorrencia.TipoOcorrenciaControleEntrega.id).click(habilitarAbaNotificacao);
    habilitarAbaNotificacao();

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirListaEmailClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Email", title: Localization.Resources.Ocorrencias.TipoOcorrencia.Email, width: "70%" },
    ];

    _gridListaEmail = new BasicDataTable(_notificacao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

}


function recarregarGridListaEmail() {

    var data = new Array();

    $.each(_notificacao.ListaEmail.val(), function (i, listaEmail) {
        var listaEmailGrid = new Object();

        listaEmailGrid.Codigo = 0;
        listaEmailGrid.Email = listaEmail.Email;

        data.push(listaEmailGrid);
    });

    _gridListaEmail.CarregarGrid(data);
}

function preencherNotificacao(dadosNotificacao) {
    PreencherObjetoKnout(_notificacao, { Data: dadosNotificacao });

    $('#corpoEmail').summernote('code', _notificacao.CorpoEmail.val());

    _notificacao.ListaEmail.val(dadosNotificacao.ListaEmail);

    recarregarGridListaEmail();
}


function excluirListaEmailClick(data) {
    $.each(_notificacao.ListaEmail.val(), function (i, listaEmail) {
        if (data.Email == listaEmail.Email) {
            _notificacao.ListaEmail.val().splice(i, 1);
            return false;
        }
    });

    recarregarGridListaEmail();
}

function adicionarListaEmailClick(e, sender) {
    var valido = true;
    var vemailValido = true;

    valido = _notificacao.Email.val() != "";
    _notificacao.Email.requiredClass("form-control");

    if (!ValidarEmail(_notificacao.Email.val())) {
        valido = false;
        vemailValido = false;
    }

    if (valido) {
        var existe = false;
        $.each(_notificacao.ListaEmail.val(), function (i, listaEmail) {
            if (listaEmail.Email == _notificacao.Email.val()) {
                existe = true;
                return;
            }
        });

        if (existe) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Ocorrencias.TipoOcorrencia.EmailJaExiste, Localization.Resources.Ocorrencias.TipoOcorrencia.EmailJaCadastrado.format(_notificacao.Email.val()));
            return;
        }

        var listaEmailGrid = new Object();
        listaEmailGrid.Codigo = 0;
        listaEmailGrid.Email = _notificacao.Email.val();

        _notificacao.ListaEmail.val().push(listaEmailGrid);
        recarregarGridListaEmail();

        $("#" + _notificacao.Email.id).focus();
        _notificacao.Email.val("");

    } else {
        if (!vemailValido)
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorioss, Localization.Resources.Ocorrencias.TipoOcorrencia.InformeUmEmailValido);
        else
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        _notificacao.Email.requiredClass("form-control is-invalid");
    }
}

function preencherNotificacaoSalvar(tipoOcorrencia) {
    _notificacao.CorpoEmail.val($('#corpoEmail').summernote('code'));

    var notificacao = {
        NotificarTransportadorPorEmail: _notificacao.NotificarTransportadorPorEmail.val(),
        NotificarEmail: _notificacao.NotificarEmail.val(),
        NotificarClientePorEmail: _notificacao.NotificarClientePorEmail.val(),
        AssuntoEmail: _notificacao.AssuntoEmail.val(),
        CorpoEmail: _notificacao.CorpoEmail.val(),
        ListaEmail: carregarListaEmailsSalvar()
    }

    tipoOcorrencia["Notificacao"] = JSON.stringify(notificacao);
}


function carregarListaEmailsSalvar() {
    var data = new Array();
    $.each(_notificacao.ListaEmail.val(), function (i, listaEmail) {
        var listaEmailGrid = new Object();

        listaEmailGrid.Codigo = 0;
        listaEmailGrid.Email = listaEmail.Email;

        data.push(listaEmailGrid);
    });

    return data;

}

function habilitarAbaNotificacao() {
        $("#liTabNotificacao").show();
}

function inserirTagCorpoEmail(tag) {
    $('#corpoEmail').summernote('editor.insertText', tag);
}

function limparCamposNotificacao() {
    LimparCampos(_notificacao);
    $('#corpoEmail').summernote('code', '');
    _notificacao.Email.requiredClass("form-control");
    habilitarAbaNotificacao();
}
