/// <reference path="Reforma.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoReformaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _arquivoImportacaoXmlNfeRetorno;
var _gridNfeRetorno;
var _nfeManualRetornoReformaPallet;
var _nfeRetornoReformaPallet;

/*
 * Declaração das Classes
 */

var NfeManualRetornoReformaPallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.date, required: true, text: "*Data: " });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: !_isTMS, text: "*Filial:", idBtnSearch: guid(), visible: !_isTMS });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Fornecedor:", idBtnSearch: guid() });
    this.Numero = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, required: true, maxlength: 7, configInt: { precision: 0, allowZero: false, thousands: '' }, text: "*Número: " });
    this.Quantidade = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, required: true, maxlength: 7, text: "*Quantidade:" });
    this.Serie = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, required: true, maxlength: 3, text: "*Série: " });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: _isTMS, text: "*Empresa/Filial:", idBtnSearch: guid(), visible: _isTMS });
    this.ValorUnitario = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.decimal, required: true, maxlength: 10, text: "*Valor Unitário:" });

    this.Adicionar = PropertyEntity({ eventClick: adicionarNfeRetornoClick, type: types.event, text: ko.observable("Adicionar") });
}

var NfeRetornoReformaPallet = function () {
    this.PermiteAdicionarNfe = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ListaNfeRetorno = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaNfeRetorno.val.subscribe(function () {
        recarregarGridNfeRetorno();
        preencherNfeRetornoResumoDadosNfe(obterListaNfeRetorno());
    });

    this.AdicionarNfe = PropertyEntity({ eventClick: adicionarNfeRetornoModalClick, type: types.event, text: ko.observable("Adicionar NF-e"), visible: ko.observable(true) });
    this.ImportarXml = PropertyEntity({ eventClick: abrirImportacaoXmlNfeRetornoClick, eventChange: confirmarImportacaoXmlNfeRetorno, type: types.event, text: "Importar XML NF-e", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridNfeRetorno() {
    var linhasPorPaginas = 5;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadXmlNfeRetorno, icone: "", visibilidade: isNfeRetornoPossuiXml };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerNfeRetorno, icone: "", visibilidade: isSituacaoPermiteGerenciarNfeRetorno };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 21, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: "Número", width: "30%", className: "text-align-left" },
        { data: "Chave", title: "Chave", width: "60%", className: "text-align-left" },
        { data: "Quantidade", visible: false },
        { data: "Valor", visible: false }
    ];

    _gridNfeRetorno = new BasicDataTable(_nfeRetornoReformaPallet.ListaNfeRetorno.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridNfeRetorno.CarregarGrid([]);
}

function loadNfeRetornoReformaPallet() {
    _nfeRetornoReformaPallet = new NfeRetornoReformaPallet();
    KoBindings(_nfeRetornoReformaPallet, "knockoutNfeRetornoReformaPallet");

    _arquivoImportacaoXmlNfeRetorno = _nfeRetornoReformaPallet.ImportarXml.get$()[0];

    _nfeManualRetornoReformaPallet = new NfeManualRetornoReformaPallet();
    KoBindings(_nfeManualRetornoReformaPallet, "knockoutNfeManualRetornoReformaPallet");

    new BuscarFilial(_nfeManualRetornoReformaPallet.Filial);
    new BuscarClientes(_nfeManualRetornoReformaPallet.Fornecedor);
    new BuscarTransportadores(_nfeManualRetornoReformaPallet.Transportador);

    loadGridNfeRetorno();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function abrirImportacaoXmlNfeRetornoClick() {
    if (isSituacaoPermiteGerenciarNfeRetorno())
        _nfeRetornoReformaPallet.ImportarXml.get$().click();
}

function adicionarNfeRetornoClick() {
    if (isSituacaoPermiteGerenciarNfeRetorno()) {
        if (ValidarCamposObrigatorios(_nfeManualRetornoReformaPallet)) {
            _nfeManualRetornoReformaPallet.Codigo.val(_reformaPallet.Codigo.val());

            var nfeManual = RetornarObjetoPesquisa(_nfeManualRetornoReformaPallet);

            executarReST("Reforma/AdicionarNfeRetorno", nfeManual, function (retorno) {
                if (retorno.Success) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "NF-e adicionada com sucesso");

                    preencherNfeRetorno(retorno.Data.ListaNfeRetorno);
                    fecharNfeRetornoModal()
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }, null);
        }
        else
            exibirMensagemCamposObrigatorio();
    }
}

function adicionarNfeRetornoModalClick() {
    Global.abrirModal('divModalNfeRetorno');
    $("#divModalNfeRetorno").one('hidden.bs.modal', function () {
        LimparCampos(_nfeManualRetornoReformaPallet);
    });
}

function confirmarImportacaoXmlNfeRetorno() {
    var totalArquivosSelecionados = _arquivoImportacaoXmlNfeRetorno.files.length;

    if (totalArquivosSelecionados > 0)
        exibirConfirmacao("Importar XML", "Tem certeza que deseja importar " + totalArquivosSelecionados + " arquivo(s)?", importarXmlNfeRetorno);
}

/*
 * Declaração das Funções
 */

function downloadXmlNfeRetorno(registroSelecionado) {
    executarDownload("Reforma/DownloadXmlNfeRetorno", { Codigo: registroSelecionado.Codigo });
}

function fecharNfeRetornoModal() {
    Global.fecharModal("divModalNfeRetorno");
}

function importarXmlNfeRetorno() {
    var formData = new FormData();

    for (var i = 0; i < _arquivoImportacaoXmlNfeRetorno.files.length; i++) {
        formData.append("XML", _arquivoImportacaoXmlNfeRetorno.files[i]);
    }

    enviarArquivo("Reforma/ImportacaoXmlNfeRetorno", { Codigo: _reformaPallet.Codigo.val() }, formData, function (retorno) {
        limparArquivoImportacaoXmlNfeRetorno();

        if (retorno.Success) {
            if (retorno.Data != null) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.TotalXmlAdicionados + " de " + retorno.Data.TotalXml + " importado(s).");

                preencherNfeRetorno(retorno.Data.ListaNfeRetorno);
            } else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        } else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function isNfeRetornoPossuiXml(registroSelecionado) {
    return registroSelecionado.Chave
}

function isSituacaoPermiteGerenciarNfeRetorno() {
    return (_reformaPallet.Situacao.val() === EnumSituacaoReformaPallet.AguardandoRetorno);
}

function limparNfeRetorno() {
    _nfeRetornoReformaPallet.PermiteAdicionarNfe.val(false);
    _nfeRetornoReformaPallet.ListaNfeRetorno.val(new Array());

    limparArquivoImportacaoXmlNfeRetorno();
}

function limparArquivoImportacaoXmlNfeRetorno() {
    _arquivoImportacaoXmlNfeRetorno.value = null;
}

function obterListaNfeRetorno() {
    return _nfeRetornoReformaPallet.ListaNfeRetorno.val().slice();
}

function preencherNfeRetorno(dadosNfeRetorno) {
    _nfeRetornoReformaPallet.ListaNfeRetorno.val(dadosNfeRetorno);
    _nfeRetornoReformaPallet.PermiteAdicionarNfe.val(isSituacaoPermiteGerenciarNfeRetorno());
}

function recarregarGridNfeRetorno() {
    var listaNfeRetorno = obterListaNfeRetorno();

    _gridNfeRetorno.CarregarGrid(listaNfeRetorno);
}

function removerNfeRetorno(registroSelecionado) {
    if (isSituacaoPermiteGerenciarNfeRetorno()) {
        exibirConfirmacao("Confirmação", "Realmente deseja excluir a da NF-e", function () {
            executarReST("Reforma/ExcluirNfeRetorno", { Codigo: registroSelecionado.Codigo }, function (retorno) {
                if (retorno.Data) {
                    removerNfeRetornoLocal(registroSelecionado);

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }, null);
        });
    }
}

function removerNfeRetornoLocal(registroSelecionado) {
    var listaNfeRetorno = obterListaNfeRetorno();

    listaNfeRetorno.forEach(function (nfeRetorno, i) {
        if (registroSelecionado.Codigo == nfeRetorno.Codigo) {
            listaNfeRetorno.splice(i, 1);
        }
    });

    _nfeRetornoReformaPallet.ListaNfeRetorno.val(listaNfeRetorno);
}

function validarNfeRetornoInformadas() {
    if (_nfeRetornoReformaPallet.ListaNfeRetorno.val().length == 0) {
        exibirMensagem("atencao", "NF-e de Retorno", "Por Favor, importe ou cadastre ao menos uma NF-e de Retorno");

        return false;
    }

    return true;
}