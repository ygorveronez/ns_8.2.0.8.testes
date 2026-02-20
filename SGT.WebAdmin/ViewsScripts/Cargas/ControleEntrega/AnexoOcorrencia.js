
/// <reference path="../../Enumeradores/EnumSituacaoDigitalizacaoCanhoto.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroAnexoOcorrencia;
var _gridAnexoOcorrencia;
var _anexoOcorrencia;

/*
 * Declaração das Classes
 */

var AnexoOcorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.SituacaoDigitalizacaoCanhoto = PropertyEntity({ val: ko.observable(EnumSituacaoDigitalizacaoCanhoto.Todas), options: EnumSituacaoDigitalizacaoCanhoto.ObterOpcoes(true), def: EnumSituacaoDigitalizacaoCanhoto.Todas });
    this.AnexoOcorrencias = PropertyEntity({ text: Localization.Resources.Gerais.Geral.AnexoOcorrencias, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.AnexoOcorrencias.val.subscribe(function () {
        recarregarGridAnexoOcorrencia();
    });
}


/*
 * Declaração das Funções de Inicialização
 */

function loadAnexoOcorrencia() {
    _anexoOcorrencia = new AnexoOcorrencia();
    KoBindings(_anexoOcorrencia, "knockoutAnexoOcorrencia");

    loadGridAnexoOcorrencia();
}

function loadGridAnexoOcorrencia() {
    var linhasPorPaginas = 5;
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadAnexoOcorrenciaClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 15, opcoes: [opcaoDownload] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "80%", className: "text-align-left" }
    ];

    _gridAnexoOcorrencia = new BasicDataTable(_anexoOcorrencia.AnexoOcorrencias.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoOcorrencia.CarregarGrid([]);
}


/*
 * Declaração das Funções Associadas a Eventos
 */



function downloadAnexoOcorrenciaClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("OcorrenciaColetaEntregaAnexo/DownloadAnexoOcorrencia", dados);
}

/*
 * Declaração das Funções Públicas
 */

function exibirAnexoOcorrencias(registroSelecionado) {
    _anexoOcorrencia.Codigo.val(registroSelecionado.Codigo);
    _anexoOcorrencia.SituacaoDigitalizacaoCanhoto.val(registroSelecionado.SituacaoDigitalizacaoCanhoto);

    executarReST("OcorrenciaColetaEntregaAnexo/ObterAnexo", { Codigo: _anexoOcorrencia.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _anexoOcorrencia.AnexoOcorrencias.val(retorno.Data.Anexos);
                recarregarGridAnexoOcorrencia();
               
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });

    Global.abrirModal("#divModalAnexoOcorrencia")
}

/*
 * Declaração das Funções Privadas
 */


function limparAnexoOcorrencia() {
    _anexoOcorrencia.AnexoOcorrencias.val(new Array());

    LimparCampos(_anexoOcorrencia);    
    recarregarGridAnexoOcorrencia();
}

function obterAnexoOcorrencias() {
    return _anexoOcorrencia.AnexoOcorrencias.val().slice();
}

function obterFormDataAnexoOcorrencias(anexoOcorrencias) {
    if (anexoOcorrencias.length > 0) {
        var formData = new FormData();

        anexoOcorrencias.forEach(function (anexoOcorrencia) {
            formData.append("Arquivo", anexoOcorrencia.Arquivo);
            formData.append("Descricao", anexoOcorrencia.Descricao);
        });

        return formData;
    }

    return undefined;
}

function recarregarGridAnexoOcorrencia() {
    _gridAnexoOcorrencia.CarregarGrid(obterAnexoOcorrencias());
}

