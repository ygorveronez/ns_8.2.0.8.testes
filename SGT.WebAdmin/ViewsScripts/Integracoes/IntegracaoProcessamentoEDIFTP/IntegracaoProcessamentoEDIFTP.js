/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoProcessamentoEDIFTP.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridControleIntegracaoEDIFTP;
var _controle;
var _importacaoArquivo;

/*
 * Declaração das Classes
 */

var ControleGeracaoProcessamentoEDIFTP = function () {
    var dataAtual = moment().add(-1, 'days').format("DD/MM/YYYY");
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, required: ko.observable(true), visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, required: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ text: "Nome do Arquivo: ", maxlength: 100 });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoIntegracaoProcessamentoEDIFTP.Todos), options: EnumSituacaoIntegracaoProcessamentoEDIFTP.obterOpcoes(), def: EnumSituacaoIntegracaoProcessamentoEDIFTP.Todos, text: "Situação:" });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridControleIntegracaoEDIFTP.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.EnviarArquivo = PropertyEntity({
        eventClick: function (e) {
            exibirModalEnviarEDI();
        }, type: types.event, text: "Enviar Arquivo EDI", idGrid: guid(), visible: ko.observable(true)
    });


};

var importacaoArquivoEDI = function () {
    this.GrupoPessoaEnvioArquivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(true), required: true });
    this.LayoutEDI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Layout EDI:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: true });
    this.Upload = PropertyEntity({ type: types.event, eventChange: UploadChange, idFile: guid(), accept: ".txt", text: "Selecionar Arquivo EDI", icon: "fal fa-file-alt" });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadIntegracaoProcessamentoEDIFTP() {
    _controle = new ControleGeracaoProcessamentoEDIFTP();
    KoBindings(_controle, "knockoutIntegracaoProcessamentoEDIFTP");

    _importacaoArquivo = new importacaoArquivoEDI();
    KoBindings(_importacaoArquivo, "knockoutEnvioArquivos");

    new BuscarGruposPessoas(_controle.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarGruposPessoas(_importacaoArquivo.GrupoPessoaEnvioArquivo, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarLayoutsEDI(_importacaoArquivo.LayoutEDI, null, null, null, null, [EnumTipoLayoutEDI.NOTFIS_NOVA_IMPORTACAO], _importacaoArquivo.GrupoPessoaEnvioArquivo);

    loadGridControleGeracaoEDI();
    _importacaoArquivo.Upload.file = document.getElementById(_importacaoArquivo.Upload.idFile);
}


function loadGridControleGeracaoEDI() {
    var configuracaoExportacao = { url: "IntegracaoFTPProcessamentoEDI/ExportarPesquisa", titulo: "Controle de Geração de Cargas por EDI" };
    var ordenacaoPadrao = null;
    var quantidadePorPagina = 10;
    var opcaoDownload = { descricao: "Download", id: guid(), evento: "onclick", metodo: baixarNotfisClick, tamanho: "20", icone: "" };
    var opcaoReenviar = { descricao: "Reenviar", id: guid(), evento: "onclick", metodo: ReenviarControleGeracaoEDIClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoDownload, opcaoReenviar], tamanho: "5" };

    _gridControleIntegracaoEDIFTP = new GridViewExportacao(_controle.Pesquisar.idGrid, "IntegracaoFTPProcessamentoEDI/Pesquisa", _controle, menuOpcoes, configuracaoExportacao, ordenacaoPadrao, quantidadePorPagina);
    _gridControleIntegracaoEDIFTP.SetPermitirRedimencionarColunas(true);
    _gridControleIntegracaoEDIFTP.CarregarGrid();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function exibirModalEnviarEDI() {
    Global.abrirModal('divModalEnviarEDI');
}

function baixarNotfisClick(registroSelecionado) {
    executarDownload("IntegracaoFTPProcessamentoEDI/DownloadArquivoEDI", { Codigo: registroSelecionado.Codigo });
}

function ReenviarControleGeracaoEDIClick(registroSelecionado) {
    executarReST("IntegracaoFTPProcessamentoEDI/Reenviar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio realizado com sucesso.");
                _gridControleIntegracaoEDIFTP.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function UploadChange() {
    if (_importacaoArquivo.Upload.file.files.length > 0) {
        var data = new FormData();
        data.append("GrupoPessoaEnvioArquivo", _importacaoArquivo.GrupoPessoaEnvioArquivo.codEntity());
        data.append("LayoutEDI", _importacaoArquivo.LayoutEDI.codEntity());
        for (var i = 0, s = _importacaoArquivo.Upload.file.files.length; i < s; i++) {
            data.append("Arquivo", _importacaoArquivo.Upload.file.files[i]);
        }

        enviarArquivo("IntegracaoFTPProcessamentoEDI/EnviarArquivo", {}, data, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    if (retorno.Data.Adicionados > 0) {
                        if (retorno.Data.Adicionados > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.Adicionados + " arquivos enviados.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.Adicionados + " arquivo enviado.");

                        _gridControleIntegracaoEDIFTP.CarregarGrid();
                    }

                    if (retorno.Data.Erros.length > 0)
                        exibirMensagem(tipoMensagem.aviso, "Aviso", "Ocorreu " + retorno.Data.Erros + " erro(s).");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }
            $("#" + _importacaoArquivo.Upload.idFile).val("");
        });
    }
}

/*
 * Declaração das Funções
 */
