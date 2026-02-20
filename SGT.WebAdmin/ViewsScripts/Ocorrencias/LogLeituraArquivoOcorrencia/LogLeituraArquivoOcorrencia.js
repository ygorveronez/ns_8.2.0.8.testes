/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCargaEDI.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridLogLeituraArquivoOcorrencia;
var _logLeituraArquivoOcorrencia;

/*
 * Declaração das Classes
 */

var LogLeituraArquivoOcorrencia = function () {
    var dataAtual = moment().add(-1, 'days').format("DD/MM/YYYY");
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.NomeArquivo = PropertyEntity({ text: "Nome do Arquivo: ", maxlength: 100 });
    this.Ocorrencia = PropertyEntity({ text: "Ocorrência: ", maxlength: 50 });
    this.TipoEnvioArquivo = PropertyEntity({ val: ko.observable(EnumTipoEnvioArquivo.Todos), options: EnumTipoEnvioArquivo.obterOpcoesPesquisa(), def: EnumTipoEnvioArquivo.Todos, text: "Tipo de Envio:" });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Upload = PropertyEntity({ type: types.event, eventChange: UploadChange, idFile: guid(), accept: ".txt", text: "Enviar EDI", icon: "fal fa-upload", visible: ko.observable(true) });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridLogLeituraArquivoOcorrencia.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
}


/*
 * Declaração das Funções de Inicialização
 */

function LoadLogLeituraArquivoOcorrencia() {
    _logLeituraArquivoOcorrencia = new LogLeituraArquivoOcorrencia();
    KoBindings(_logLeituraArquivoOcorrencia, "knockoutLogLeituraArquivoOcorrencia");
    new BuscarTransportadores(_logLeituraArquivoOcorrencia.Transportador);
    loadGridLogLeituraArquivoOcorrencia();

    _logLeituraArquivoOcorrencia.Upload.file = document.getElementById(_logLeituraArquivoOcorrencia.Upload.idFile);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
        _logLeituraArquivoOcorrencia.Transportador.visible(false);
}

function loadGridLogLeituraArquivoOcorrencia() {
    var configuracaoExportacao = { url: "LogLeituraArquivoOcorrencia/ExportarPesquisa", titulo: "Controle de Geração de Cargas por EDI" };
    var ordenacaoPadrao = null;
    var quantidadePorPagina = 10;
    var opcaoDownload = { descricao: "Download", id: guid(), evento: "onclick", metodo: baixarNotfisClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoDownload], tamanho: "5" };

    _gridLogLeituraArquivoOcorrencia = new GridViewExportacao(_logLeituraArquivoOcorrencia.Pesquisar.idGrid, "LogLeituraArquivoOcorrencia/Pesquisa", _logLeituraArquivoOcorrencia, menuOpcoes, configuracaoExportacao, ordenacaoPadrao, quantidadePorPagina);
    _gridLogLeituraArquivoOcorrencia.SetPermitirRedimencionarColunas(true);
    _gridLogLeituraArquivoOcorrencia.CarregarGrid();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function baixarNotfisClick(registroSelecionado) {
    executarDownload("LogLeituraArquivoOcorrencia/DownloadArquivoEDI", { Codigo: registroSelecionado.Codigo });
}

function UploadChange() {
    if (_logLeituraArquivoOcorrencia.Upload.file.files.length > 0) {
        var data = new FormData();

        for (var i = 0, s = _logLeituraArquivoOcorrencia.Upload.file.files.length; i < s; i++) {
            data.append("Arquivo", _logLeituraArquivoOcorrencia.Upload.file.files[i]);
        }

        enviarArquivo("LogLeituraArquivoOcorrencia/EnviarArquivo", {}, data, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    if (retorno.Data.Adicionados > 0) {
                        if (retorno.Data.Adicionados > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.Adicionados + " arquivos importados.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.Adicionados + " arquivo importados.");
                    }

                    if (retorno.Data.Erros.length > 0)
                        exibirMensagem(tipoMensagem.aviso, "Aviso", "Ocorreu " + retorno.Data.Erros + " erro(s).");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }
            $("#" + _logLeituraArquivoOcorrencia.Upload.idFile).val("");
        });
    }
}
