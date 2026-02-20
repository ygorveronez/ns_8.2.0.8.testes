/// <r/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _observacaoRelatorioEmbarque;

/*
 * Declaração das Classes
 */

var ObservacaoRelatorioEmbarque = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 400 });

    this.Salvar = PropertyEntity({ eventClick: salvarObservacaoRelatorioEmbarqueClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadObservacaoRelatorioEmbarque() {
        _observacaoRelatorioEmbarque = new ObservacaoRelatorioEmbarque();
        KoBindings(_observacaoRelatorioEmbarque, "knockoutObservacaoRelatorioEmbarque");
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function salvarObservacaoRelatorioEmbarqueClick(data) {
    executarReST("CargaImpressaoDocumentos/SalvarObservacaoRelatorioEmbarque", { Codigo: data.Codigo.val(), ObservacaoRelatorioDeEmbarque: data.Observacao.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Observação do Relatório de Embarque atualizada com Sucesso.");
                fecharModalObservacaoRelatorioEmbarque();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

/*
 * Declaração das Funções Públicas
 */

function exibirObservacaoDoRelatorioEmbarque(cargaImpressao) {
    _observacaoRelatorioEmbarque.Codigo.val(cargaImpressao.Codigo.val());
    _observacaoRelatorioEmbarque.Observacao.val(cargaImpressao.ObservacaoRelatorioDeEmbarque.val());

    exibirModalObservacaoRelatorioEmbarque();
}

/*
 * Declaração das Funções Privadas
 */

function exibirModalObservacaoRelatorioEmbarque() {
    Global.abrirModal('divModalObservacaoRelatorioEmbarque');
    $("#divModalObservacaoRelatorioEmbarque").one('hidden.bs.modal', function () {
        LimparCampos(_observacaoRelatorioEmbarque);
    });
}

function fecharModalObservacaoRelatorioEmbarque() {
    Global.fecharModal('divModalObservacaoRelatorioEmbarque');
}
