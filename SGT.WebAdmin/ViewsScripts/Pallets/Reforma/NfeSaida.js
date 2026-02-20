/// <reference path="NfeSaidaResumo.js" />
/// <reference path="Reforma.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoReformaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _arquivoImportacaoXmlNfeSaida;
var _gridNfeSaida;
var _nfeManualSaidaReformaPallet;
var _nfeSaidaReformaPallet;

/*
 * Declaração das Classes
 */

var NfeManualSaidaReformaPallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.date, required: true, text: "*Data: " });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: !_isTMS, text: "*Filial:", idBtnSearch: guid(), visible: !_isTMS });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Fornecedor:", idBtnSearch: guid() });
    this.Numero = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, required: true, maxlength: 7, configInt: { precision: 0, allowZero: false, thousands: '' }, text: "*Número: " });
    this.Quantidade = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, required: true, maxlength: 7, text: "*Quantidade:" });
    this.Serie = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, required: true, maxlength: 3, text: "*Série: " });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: _isTMS, text: "*Empresa/Filial:", idBtnSearch: guid(), visible: _isTMS });
    this.ValorUnitario = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.decimal, required: true, maxlength: 10, text: "*Valor Unitário:" });

    this.Adicionar = PropertyEntity({ eventClick: adicionarNfeSaidaClick, type: types.event, text: ko.observable("Adicionar") });
}

var NfeSaidaReformaPallet = function () {
    this.PermiteAdicionarNfe = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ListaNfeSaida = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaNfeSaida.val.subscribe(function () {
        recarregarGridNfeSaida();
        preencherNfeSaidaResumoDadosNfe(obterListaNfeSaida());
    });

    this.AdicionarNfe = PropertyEntity({ eventClick: adicionarNfeSaidaModalClick, type: types.event, text: ko.observable("Adicionar NF-e"), visible: ko.observable(true) });
    this.ImportarXml = PropertyEntity({ eventClick: abrirImportacaoXmlNfeSaidaClick, eventChange: confirmarImportacaoXmlNfeSaida, type: types.event, text: "Importar XML NF-e", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridNfeSaida() {
    var linhasPorPaginas = 5;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadXmlNfeSaida, icone: "", visibilidade: isNfeSaidaPossuiXml };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerNfeSaida, icone: "", visibilidade: isSituacaoPermiteGerenciarNfeSaida };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 21, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: "Número", width: "30%", className: "text-align-left" },
        { data: "Chave", title: "Chave", width: "60%", className: "text-align-left" },
        { data: "Quantidade", visible: false },
        { data: "Valor", visible: false }
    ];

    _gridNfeSaida = new BasicDataTable(_nfeSaidaReformaPallet.ListaNfeSaida.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridNfeSaida.CarregarGrid([]);
}

function loadNfeSaidaReformaPallet() {
    _nfeSaidaReformaPallet = new NfeSaidaReformaPallet();
    KoBindings(_nfeSaidaReformaPallet, "knockoutNfeSaidaReformaPallet");

    _arquivoImportacaoXmlNfeSaida = _nfeSaidaReformaPallet.ImportarXml.get$()[0];

    _nfeManualSaidaReformaPallet = new NfeManualSaidaReformaPallet();
    KoBindings(_nfeManualSaidaReformaPallet, "knockoutNfeManualSaidaReformaPallet");

    new BuscarFilial(_nfeManualSaidaReformaPallet.Filial);
    new BuscarClientes(_nfeManualSaidaReformaPallet.Fornecedor);
    new BuscarTransportadores(_nfeManualSaidaReformaPallet.Transportador);

    loadGridNfeSaida();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function abrirImportacaoXmlNfeSaidaClick() {
    if (isSituacaoPermiteGerenciarNfeSaida())
        _nfeSaidaReformaPallet.ImportarXml.get$().click();
}

function adicionarNfeSaidaClick() {
    if (isSituacaoPermiteGerenciarNfeSaida()) {
        if (ValidarCamposObrigatorios(_nfeManualSaidaReformaPallet)) {
            _nfeManualSaidaReformaPallet.Codigo.val(_reformaPallet.Codigo.val());

            var nfeManual = RetornarObjetoPesquisa(_nfeManualSaidaReformaPallet);

            executarReST("Reforma/AdicionarNfeSaida", nfeManual, function (retorno) {
                if (retorno.Success) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "NF-e adicionada com sucesso");

                    preencherNfeSaida(retorno.Data.ListaNfeSaida);
                    fecharNfeSaidaModal()
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }, null);
        }
        else
            exibirMensagemCamposObrigatorio();
    }
}

function adicionarNfeSaidaModalClick() {
    Global.abrirModal('divModalNfeSaida');
    $("#divModalNfeSaida").one('hidden.bs.modal', function () {
        LimparCampos(_nfeManualSaidaReformaPallet);
    });
}

function confirmarImportacaoXmlNfeSaida() {
    var totalArquivosSelecionados = _arquivoImportacaoXmlNfeSaida.files.length;

    if (totalArquivosSelecionados > 0)
        exibirConfirmacao("Importar XML", "Tem certeza que deseja importar " + totalArquivosSelecionados + " arquivo(s)?", importarXmlNfeSaida);
}

/*
 * Declaração das Funções
 */

function downloadXmlNfeSaida(registroSelecionado) {
    executarDownload("Reforma/DownloadXmlNfeSaida", { Codigo: registroSelecionado.Codigo });
}

function fecharNfeSaidaModal() {
    Global.fecharModal("divModalNfeSaida");
}

function finalizarNfeSaida() {
    if (isSituacaoPermiteGerenciarNfeSaida()) {
        exibirConfirmacao("Confirmação", "Realmente deseja finalizar a importação de NF-e de Saída?", function () {
            if (validarNfeSaidaInformadas()) {
                validarDiferencaQuantidadeEnviada(function () {
                    executarReST("Reforma/FinalizarNfeSaida", { Codigo: _reformaPallet.Codigo.val() }, function (retorno) {
                        if (retorno.Data) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Importação de NF-e de Saída finalizada com sucesso");

                            novaReforma();
                        }
                        else
                            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
                    });
                });
            }
        });
    }
}

function importarXmlNfeSaida() {
    var formData = new FormData();

    for (var i = 0; i < _arquivoImportacaoXmlNfeSaida.files.length; i++) {
        formData.append("XML", _arquivoImportacaoXmlNfeSaida.files[i]);
    }

    enviarArquivo("Reforma/ImportacaoXmlNfeSaida", { Codigo: _reformaPallet.Codigo.val() }, formData, function (retorno) {
        limparArquivoImportacaoXmlNfeSaida();

        if (retorno.Success) {
            if (retorno.Data != null) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.TotalXmlAdicionados + " de " + retorno.Data.TotalXml + " importado(s).");

                preencherNfeSaida(retorno.Data.ListaNfeSaida);
            } else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        } else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function isNfeSaidaPossuiXml(registroSelecionado) {
    return registroSelecionado.Chave
}

function isSituacaoPermiteGerenciarNfeSaida() {
    return (_reformaPallet.Situacao.val() === EnumSituacaoReformaPallet.AguardandoNfeSaida);
}

function limparNfeSaida() {
    _nfeSaidaReformaPallet.PermiteAdicionarNfe.val(false);
    _nfeSaidaReformaPallet.ListaNfeSaida.val(new Array());

    limparArquivoImportacaoXmlNfeSaida();
}

function limparArquivoImportacaoXmlNfeSaida() {
    _arquivoImportacaoXmlNfeSaida.value = null;
}

function obterListaNfeSaida() {
    return _nfeSaidaReformaPallet.ListaNfeSaida.val().slice();
}

function preencherNfeSaida(dadosNfeSaida) {
    _nfeSaidaReformaPallet.ListaNfeSaida.val(dadosNfeSaida);
    _nfeSaidaReformaPallet.PermiteAdicionarNfe.val(isSituacaoPermiteGerenciarNfeSaida());
}

function recarregarGridNfeSaida() {
    var listaNfeSaida = obterListaNfeSaida();

    _gridNfeSaida.CarregarGrid(listaNfeSaida);
}

function removerNfeSaida(registroSelecionado) {
    if (isSituacaoPermiteGerenciarNfeSaida()) {
        exibirConfirmacao("Confirmação", "Realmente deseja excluir a da NF-e", function () {
            executarReST("Reforma/ExcluirNfeSaida", { Codigo: registroSelecionado.Codigo }, function (retorno) {
                if (retorno.Data) {
                    removerNfeSaidaLocal(registroSelecionado);

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }, null);
        });
    }
}

function removerNfeSaidaLocal(registroSelecionado) {
    var listaNfeSaida = obterListaNfeSaida();

    listaNfeSaida.forEach(function (nfeSaida, i) {
        if (registroSelecionado.Codigo == nfeSaida.Codigo) {
            listaNfeSaida.splice(i, 1);
        }
    });

    _nfeSaidaReformaPallet.ListaNfeSaida.val(listaNfeSaida);
}

function validarNfeSaidaInformadas() {
    if (_nfeSaidaReformaPallet.ListaNfeSaida.val().length == 0) {
        exibirMensagem("atencao", "NF-e de Saída", "Por Favor, importe ao menos uma NF-e de Saída");

        return false;
    }

    return true;
}

function validarDiferencaQuantidadeEnviada(callbackFinalizarNfeSaida) {
    if (_nfeSaidaResumoReformaPallet.QuantidadeEnvio.val() !== _nfeSaidaResumoReformaPallet.QuantidadeNfe.val())
        exibirConfirmacao("Confirmação", "A quantidade de envio é diferente da quantidade total das NF-e de saída. Deseja continuar?", callbackFinalizarNfeSaida);
    else 
        callbackFinalizarNfeSaida();
}