// #region Objetos Globais do Arquivo
var _detalhesNotaFiscal;
// #endregion Objetos Globais do Arquivo

//#region Classes
var DetalhesNotaFiscal = function () {
    this.Codigo = PropertyEntity({ text: "Codigo", val: ko.observable(""), visible: ko.observable(true)});
    this.Chave = PropertyEntity({ text: "Chave de Acesso", val: ko.observable(""), visible: ko.observable(true), eventClick: copyToClipboardChave });
    this.Numero = PropertyEntity({ text: "Nota Fiscal:", val: ko.observable(""), visible: ko.observable(true), eventClick: copyToClipboardNumero });
    this.Serie = PropertyEntity({ text: "Série", val: ko.observable(""), visible: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ text: "Data de Emissão", val: ko.observable(""), visible: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "Valor", val: ko.observable(""), visible: ko.observable(true) });
    this.Volume = PropertyEntity({ text: "Volumes", val: ko.observable(""), visible: ko.observable(true) });
    this.Peso = PropertyEntity({ text: "Peso", val: ko.observable(""), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: "Filial", val: ko.observable(""), visible: ko.observable(true) });
}
//#endregion Classes

// #region Funções de Inicialização
function loadDetalhesNotaFiscal(detalhesDaNota) {
    if (!detalhesDaNota) return;
    let div = $("#divDetalhesNotaFiscal");
    if (!div) {
        console.log('loadDetalhesNotaFiscal -------- Esqueceu da div divDetalhesNotaFiscal: <div id="divDetalhesNotaFiscal"></div>');
        return;
    }
    $.get("Content/Static/NotaFiscal/DetalhesNotaFiscal.html?dyn=" + guid(), function (html) {
        $("#divDetalhesNotaFiscal").html(html);

        _detalhesNotaFiscal = new DetalhesNotaFiscal();
        KoBindings(_detalhesNotaFiscal, "knockoutDetalhesNotaFiscal");

        PreencherObjetoKnout(_detalhesNotaFiscal, { Data: detalhesDaNota });

        Global.abrirModal("divModalDetalhesNotaFiscal");
    });
}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function copyToClipboardChave(e) {
    copyToClipboard(e.Chave.val());
}
function copyToClipboardNumero(e) {
    copyToClipboard(e.Numero.val());
}
// #endregion Funções Associadas a Eventos

// #region Funções Públicas
// #endregion Funções Públicas

// #region Funções Privadas
function copyToClipboard(value) {
    Global.copyTextToClipboard(value, function () { exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Informação copiada com sucesso!"); });
}
// #endregion Funções Privadas