/// <reference path="Analise.js" />

var _gridNFeDevolucaoAnalise;
var _chamadoOcorrenciaModalObservacaoMotorista;
var _botoesNotaFiscalDevolucao;
var _dropZoneNotaFiscalDevolucao;
var _dadosXmlNotaFiscalDevolucaoConfirmados;
var retornoController;
var GlobalPercentualNotaFiscalDevolucao = 0;

var NFeDevolucaoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.CodigoNotaOrigem = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });

    this.Chave = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Numero = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Serie = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.DataEmissao = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorTotalProdutos = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorTotal = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.PesoDevolvido = PropertyEntity({ type: types.map, val: ko.observable("") });

    this.PossuiImagem = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.NotaOrigem = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ObservacaoMotorista = PropertyEntity({ type: types.map, val: ko.observable("") });
};

var knockoutDropZoneNotaFiscalDevolucao = function () {
    this.Dropzone = PropertyEntity({ type: types.local, idTab: guid(), visible: ko.observable(true), enable: ko.observable(true) });
};

var knockoutBotoesNotaFiscalDevolucao = function () {
    this.Cancelar = PropertyEntity({ eventClick: function () { _adicionarXmlNotaFiscalDevolucaoModal.hide(); }, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Voltar, visible: ko.observable(true) });

    this.Confirmar = PropertyEntity({
        eventClick: confirmarImportacaoXmlClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Confirmar, visible: ko.observable(true), enable: ko.observable(false), url: ko.observable("") 
    });
};

function loadNFeDevolucaoAnalise() {
    if (_gridNFeDevolucaoAnalise != null && _gridNFeDevolucaoAnalise.Destroy)
        _gridNFeDevolucaoAnalise.Destroy();

    var verObservacao = { descricao: "Observacao", id: guid(), metodo: exibirObservacaoMotorista, icone: "", visibilidade: visibilidadeVisualizarJustificativa };
    var baixarNFe = { descricao: "Baixar Imagem NF-e", id: guid(), metodo: baixarImagemNotaDevolucaoClick, icone: "", visibilidade: visibilidadeBaixarImagem };
    var excluir = { descricao: "Excluir", id: guid(), metodo: excluirNFeDevolucaoAnaliseClick, icone: "", visibilidade: visibilidadeExcluir };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [excluir, baixarNFe, verObservacao] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "PossuiImagem", visible: false },
        { data: "ObservacaoMotorista", visible: false },
        { data: "Chave", title: "Chave", width: "30%" },
        { data: "Numero", title: "Número", width: "10%" },
        { data: "Serie", title: "Série", width: "10%" },
        { data: "DataEmissao", title: "Data Emissão", width: "10%" },
        { data: "ValorTotalProdutos", title: "Valor Total Produto", width: "15%" },
        { data: "ValorTotal", title: "Valor Total NF", width: "15%" },
        { data: "PesoDevolvido", title: "Peso Devolvido", width: "20%" },
        { data: "NotaOrigem", title: "NF-e Origem", width: "15%" },
        { data: "CodigoNotaOrigem", visible: false },
    ];

    _gridNFeDevolucaoAnalise = new BasicDataTable(_analise.GridNFeDevolucaoAnalise.id, header, menuOpcoes);

    _chamadoOcorrenciaModalObservacaoMotorista = new bootstrap.Modal(document.getElementById("divModalObservacaoMotorista"), { backdrop: 'static' });
}

function visibilidadeExcluir() {
    var status = _chamado.Situacao.val() === EnumSituacaoChamado.Aberto || _chamado.Situacao.val() === EnumSituacaoChamado.EmTratativa;
    return status;
}

function visibilidadeBaixarImagem(data) {
    return data.PossuiImagem;
}

function visibilidadeVisualizarJustificativa(data) {
    return !string.IsNullOrWhiteSpace(data.ObservacaoMotorista);
}

function exibirObservacaoMotorista(e) {
    $('#PMensagemObservacaoMotorista').html(e.ObservacaoMotorista);
    _chamadoOcorrenciaModalObservacaoMotorista.show();
}

function baixarImagemNotaDevolucaoClick(e) {
    var data = { Codigo: e.Codigo };
    executarDownload("ChamadoAnalise/DownloadImagemNFDevolucao", data);
}

function adicionarNFeDevolucaoAnaliseClick(e, sender) {
    if (!_analise.NumeroNFeOrigemAnalise.val())
        return exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Informe o Número da NF-e Origem.");

    if (Boolean(_analise.ChaveNFeDevolucaoAnalise.val()) && !ValidarChaveAcesso(_analise.ChaveNFeDevolucaoAnalise.val()))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Chave informada é inválida!");

    var lista = new NFeDevolucaoMap();

    lista.Codigo.val = guid();
    lista.Chave.val = _analise.ChaveNFeDevolucaoAnalise.val();
    lista.Numero.val = _analise.NumeroNFeDevolucaoAnalise.val();
    lista.Serie.val = _analise.SerieNFeDevolucaoAnalise.val();
    lista.DataEmissao.val = _analise.DataEmissaoNFeDevolucaoAnalise.val();
    lista.ValorTotalProdutos.val = _analise.ValorTotalProdutosNFeDevolucaoAnalise.val();
    lista.ValorTotal.val = _analise.ValorTotalNFeDevolucaoAnalise.val();
    lista.PesoDevolvido.val = _analise.PesoDevolvidoNFeDevolucaoAnalise.val();
    lista.NotaOrigem.val = _analise.NumeroNFeOrigemAnalise.val();
    lista.CodigoNotaOrigem.val = _analise.NumeroNFeOrigemAnalise.codEntity();
    _analise.NFeDevolucaoAnalise.list.push(lista);

    limparCamposNFeDevolucaoAnalise();

    $("#" + _analise.ChaveNFeDevolucaoAnalise.id).focus();
    recarregarGridNFeDevolucaoAnalise();
}

function excluirNFeDevolucaoAnaliseClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a NF-e de Devolução?", function () {
        $.each(_analise.NFeDevolucaoAnalise.list, function (i, lista) {
            if (data.Codigo == lista.Codigo.val) {
                _analise.NFeDevolucaoAnalise.list.splice(i, 1);
                return false;
            }
        });

        recarregarGridNFeDevolucaoAnalise();
    });
}

function notaFiscalAnaliseClick(e, sender) {
    handleImportarXmlNotaFiscalDevolucao("ImportarXmlAnalise");
}

function notaFiscalAberturaClick(e, sender) {
    handleImportarXmlNotaFiscalDevolucao("ImportarXmlAbertura");
}

function handleImportarXmlNotaFiscalDevolucao(origem) {
    _botoesNotaFiscalDevolucao = new knockoutBotoesNotaFiscalDevolucao();
    _botoesNotaFiscalDevolucao.Confirmar.url = origem;
    KoBindings(_botoesNotaFiscalDevolucao, "knockoutBotoesNotaFiscalDevolucao");

    _dropZoneNotaFiscalDevolucao = new knockoutDropZoneNotaFiscalDevolucao();
    KoBindings(_dropZoneNotaFiscalDevolucao, "knockoutNotaFiscalDevolucao");

    _adicionarXmlNotaFiscalDevolucaoModal.show();

    $("#divModalImportarXmlNFe").on('shown.bs.modal', function () {
        setTimeout(function () {
            LoadDropZoneNotaFiscalDevolucao();
        }, 100);
    });

    $('#divModalImportarXmlNFe').on('hide.bs.modal', function () {
        document.activeElement.blur();
        $('body').focus();
    });

    $('#divModalImportarXmlNFe').on('hidden.bs.modal', function () {
        if (typeof LimparDropzone === "function") {
            LimparDropzone();
        }
        retornoController = null;
    });
}

function LoadDropZoneNotaFiscalDevolucao() {
    if (Dropzone.instances.length > 0) {
        Dropzone.instances.forEach(instance => {
            if (instance.element.id === _dropZoneNotaFiscalDevolucao.Dropzone.id) {
                instance.destroy();
            }
        });
    }

    $("#" + _dropZoneNotaFiscalDevolucao.Dropzone.id).dropzone({
        autoProcessQueue: true,
        acceptedFiles: ".xml",
        maxFiles: 1,
        addRemoveLinks: true,
        dictRemoveFile: Localization.Resources.Chamado.ChamadoOcorrencia.Remover,
        dictDefaultMessage: `<div class="row"><i class="fal fa-upload"></i><label class="text-center"><span class="font-lg"><span class="font-md">  ${Localization.Resources.Chamado.ChamadoOcorrencia.ArrasteOsArquivosParaOEnvio}</span><span><br/><h5 > (${Localization.Resources.Chamado.ChamadoOcorrencia.OuCliqueESelecione})</h5></label></div>`,
        dictResponseError: Localization.Resources.Chamado.ChamadoOcorrencia.FalhaAoEnviarOArquivo,
        dictInvalidFileType: Localization.Resources.Chamado.ChamadoOcorrencia.AExtensaoDoArquivoEInvalida,
        processing: function () {
            this.options.url = "ChamadoOcorrencia/ProcessarXmlNotaFiscalDevolucao";
        },
        url: "ChamadoOcorrencia/ProcessarXmlNotaFiscalDevolucao",
        success: DropZoneSucessoNotaFiscalDevolucao,
        queuecomplete: DropZoneCompletoNotaFiscalDevolucao,
        totaluploadprogress: TotalUploadProgressoNotaFiscalDevolucao,

        init: function () {
            var dropzone = this;

            LimparDropzone = function () {
                dropzone.removeAllFiles(true);
                _botoesNotaFiscalDevolucao.Confirmar.enable(false);
            };

            this.on("addedfile", function (file) {
                if (dropzone.files.length > 1) {
                    dropzone.removeFile(dropzone.files[0]);
                }
                _botoesNotaFiscalDevolucao.Confirmar.enable(true);
            });

            this.on("removedfile", function (file) {
                if (this.files.length === 0) {
                    this.removeAllFiles();
                    _botoesNotaFiscalDevolucao.Confirmar.enable(false);
                }
            });

            this.on("maxfilesexceeded", function (file) {
                this.removeAllFiles();
                this.addFile(file);
            });

            this.on("error", function (file) {
                this.removeAllFiles();
                _botoesNotaFiscalDevolucao.Confirmar.enable(false);
            });

            this.on("success", function (file) {
                _botoesNotaFiscalDevolucao.Confirmar.enable(true);
            });
        }
    });
}

function DropZoneSucessoNotaFiscalDevolucao(file, response, i, b) {
    if (!file) return;

    var arg = typeof response === 'object' ? response : JSON.parse(response);

    if (!arg.Success) {
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Chamado.ChamadoOcorrencia.FalhaAoEnviarOArquivo, arg.Msg || arg.Data.Msg)
        return this.removeFile(file);
    }

    if (arg.Data == false) {
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Chamado.ChamadoOcorrencia.FalhaAoEnviarOArquivo, arg.Msg || arg.Data.Msg)
        return this.removeFile(file);
    }

    if (!string.IsNullOrWhiteSpace(arg.Data.Msg)) {
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Chamado.ChamadoOcorrencia.FalhaAoEnviarOArquivo, arg.Data.Msg)
        return this.removeFile(file);
    }

    retornoController = arg.Data;
    return file.previewElement.classList.add("dz-success");
}

function DropZoneCompletoNotaFiscalDevolucao(arg) {
    $("#" + _dropZoneNotaFiscalDevolucao.Dropzone.idTab).parent().css("visibility", "hidden");
    $("#" + _dropZoneNotaFiscalDevolucao.Dropzone.idTab).text("");
    $("#" + _dropZoneNotaFiscalDevolucao.Dropzone.idTab).css("width", "0%");

    GlobalPercentualNotaFiscalDevolucao = 0;
}

function TotalUploadProgressoNotaFiscalDevolucao(percentualProgresso) {
    if (GlobalPercentualNotaFiscalDevolucao < Math.round(percentualProgresso)) {
        GlobalPercentualNotaFiscalDevolucao = Math.round(percentualProgresso);
        $("#" + _dropZoneNotaFiscalDevolucao.Dropzone.idTab).parent().css("visibility", "visible");
        if (GlobalPercentualNotaFiscalDevolucao < 100) {
            $("#" + _dropZoneNotaFiscalDevolucao.Dropzone.idTab).css("width", GlobalPercentualNotaFiscalDevolucao + "%");
        } else {
            $("#" + _dropZoneNotaFiscalDevolucao.Dropzone.idTab).text(Localization.Resources.Chamado.ChamadoOcorrencia.FinalizandoEnvio);
            $("#" + _dropZoneNotaFiscalDevolucao.Dropzone.idTab).css("width", "100%");
        }
    }
}

function confirmarImportacaoXmlClick() {
    if (!retornoController || _botoesNotaFiscalDevolucao.Confirmar.url === "")
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Chamado.ChamadoOcorrencia.Atencao, Localization.Resources.Chamado.ChamadoOcorrencia.SelecioneUmArquivoXMLValidaoParaImportar);

    if (_botoesNotaFiscalDevolucao.Confirmar.url === "ImportarXmlAbertura") {
        LimparCamposNFeDevolucaoAbertura();
        _dadosXmlNotaFiscalDevolucaoConfirmados = retornoController;
        _koNfdAbertura.ChaveNFeDevolucaoAbertura.val(_dadosXmlNotaFiscalDevolucaoConfirmados.Chave || "");
        _koNfdAbertura.NumeroNFeDevolucaoAbertura.val(_dadosXmlNotaFiscalDevolucaoConfirmados.Numero || "");
        _koNfdAbertura.SerieNFeDevolucaoAbertura.val(_dadosXmlNotaFiscalDevolucaoConfirmados.Serie || "");
        _koNfdAbertura.DataEmissaoNFeDevolucaoAbertura.val(_dadosXmlNotaFiscalDevolucaoConfirmados.DataEmissao || "");
        _koNfdAbertura.ValorTotalProdutosNFeDevolucaoAbertura.val(_dadosXmlNotaFiscalDevolucaoConfirmados.ValorTotalProdutos || "");
        _koNfdAbertura.ValorTotalNFeDevolucaoAbertura.val(_dadosXmlNotaFiscalDevolucaoConfirmados.ValorTotal || "");
        _koNfdAbertura.PesoDevolvidoNFeDevolucaoAbertura.val(_dadosXmlNotaFiscalDevolucaoConfirmados.PesoDevolvido || "");
    }
    else if (_botoesNotaFiscalDevolucao.Confirmar.url === "ImportarXmlAnalise") {
        limparCamposNFeDevolucaoAnalise();
        _dadosXmlNotaFiscalDevolucaoConfirmados = retornoController;
        _analise.ChaveNFeDevolucaoAnalise.val(_dadosXmlNotaFiscalDevolucaoConfirmados.Chave || "");
        _analise.NumeroNFeDevolucaoAnalise.val(_dadosXmlNotaFiscalDevolucaoConfirmados.Numero || "");
        _analise.SerieNFeDevolucaoAnalise.val(_dadosXmlNotaFiscalDevolucaoConfirmados.Serie || "");
        _analise.DataEmissaoNFeDevolucaoAnalise.val(_dadosXmlNotaFiscalDevolucaoConfirmados.DataEmissao || "");
        _analise.ValorTotalProdutosNFeDevolucaoAnalise.val(_dadosXmlNotaFiscalDevolucaoConfirmados.ValorTotalProdutos || "");
        _analise.ValorTotalNFeDevolucaoAnalise.val(_dadosXmlNotaFiscalDevolucaoConfirmados.ValorTotal || "");
        _analise.PesoDevolvidoNFeDevolucaoAnalise.val(_dadosXmlNotaFiscalDevolucaoConfirmados.PesoDevolvido || "");
    }
    
    exibirMensagem(tipoMensagem.ok, Localization.Resources.Chamado.ChamadoOcorrencia.Sucesso, Localization.Resources.Chamado.ChamadoOcorrencia.XMLAdicionadoComSucesso);
    _adicionarXmlNotaFiscalDevolucaoModal.hide();
}

//*******MÉTODOS*******

function recarregarGridNFeDevolucaoAnalise() {

    var data = new Array();

    $.each(_analise.NFeDevolucaoAnalise.list, function (i, lista) {
        var listaGrid = new Object();

        listaGrid.Codigo = lista.Codigo.val;
        listaGrid.CodigoNotaOrigem = lista.CodigoNotaOrigem.val;
        listaGrid.Chave = lista.Chave.val;
        listaGrid.Numero = lista.Numero.val;
        listaGrid.Serie = lista.Serie.val;
        listaGrid.DataEmissao = lista.DataEmissao.val;
        listaGrid.ValorTotalProdutos = lista.ValorTotalProdutos.val;
        listaGrid.ValorTotal = lista.ValorTotal.val;
        listaGrid.PesoDevolvido = lista.PesoDevolvido.val;
        listaGrid.PossuiImagem = lista.PossuiImagem.val;
        listaGrid.NotaOrigem = lista.NotaOrigem.val;
        listaGrid.ObservacaoMotorista = lista.ObservacaoMotorista.val;

        data.push(listaGrid);
    });

    _gridNFeDevolucaoAnalise.CarregarGrid(data);
}

function ControleCamposNFeDevolucaoAnalise() {
    var status = (_chamado.Situacao.val() === EnumSituacaoChamado.Aberto || _chamado.Situacao.val() === EnumSituacaoChamado.EmTratativa) && (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe);

    _analise.ChaveNFeDevolucaoAnalise.enable(status);
    _analise.NumeroNFeOrigemAnalise.enable(status);
    _analise.NumeroNFeDevolucaoAnalise.enable(status);
    _analise.SerieNFeDevolucaoAnalise.enable(status);
    _analise.DataEmissaoNFeDevolucaoAnalise.enable(status);
    _analise.ValorTotalProdutosNFeDevolucaoAnalise.enable(status);
    _analise.ValorTotalNFeDevolucaoAnalise.enable(status);
    _analise.PesoDevolvidoNFeDevolucaoAnalise.enable(status);

    _analise.ImportarXmlNotaFiscalDevolucaoAnalise.enable(status);
    _analise.AdicionarNFeDevolucaoAnalise.enable(status);
}

function limparCamposNFeDevolucaoAnalise() {
    _analise.ChaveNFeDevolucaoAnalise.val("");
    _analise.NumeroNFeDevolucaoAnalise.val("");
    _analise.SerieNFeDevolucaoAnalise.val("");
    _analise.DataEmissaoNFeDevolucaoAnalise.val("");
    _analise.ValorTotalProdutosNFeDevolucaoAnalise.val("");
    _analise.ValorTotalNFeDevolucaoAnalise.val("");
    _analise.PesoDevolvidoNFeDevolucaoAnalise.val("");
    _analise.NumeroNFeOrigemAnalise.val("");
    _analise.NumeroNFeOrigemAnalise.codEntity(0);
}
