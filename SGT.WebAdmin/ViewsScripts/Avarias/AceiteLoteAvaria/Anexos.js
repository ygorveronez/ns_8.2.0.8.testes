/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />>
/// <reference path="AceiteLoteAvaria.js" />

var _anexos;
var $modalAnexos;
var file;

var Anexos = function () {
    this.Lote = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexos.NomeArquivo.val(nomeArquivo);
    });
    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });

    this.config = PropertyEntity({
        url: {
            excluir: "AceiteLoteAvaria/ExcluirAnexao",
            download: "AceiteLoteAvaria/DownloadAnexo",
            anexar: "AceiteLoteAvaria/AnexarArquivos?callback=?"
        }
    });
}



//*******EVENTOS*******

function loadAnexos() {
    _anexos = new Anexos();
    KoBindings(_anexos, "knockoutAnexos");
    
    $modalAnexos = $("#divModalGerenciarAnexos");
    $modalAnexos.on('hidden.bs.modal', LimparCamposAnexos);

    file = document.getElementById(_anexos.Arquivo.id);
}

function downloadAnexoClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload(_anexos.config.url.download, data);
}

function removerAnexoClick(dataRow, row) {
    // Exclui do sistema
    executarReST(_anexos.config.url.excluir, dataRow, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridAnexos.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function adicionarAnexoClick() {
    // Valida
    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

    // Monta objeto anexo
    var anexo = {
        Codigo: guid(),
        Descricao: _anexos.Descricao.val(),
        NomeArquivo: _anexos.NomeArquivo.val(),
        Arquivo: file.files[0]
    };

    // Se ja esta cadastrada a ocorrencia, envia o anexo direto
    EnviarAnexo(anexo);

    // Limpa os campos
    LimparCamposAnexos();
}




//*******MÉTODOS*******
function CriaEEnviaFormData(anexos, dados) {
    // Busca todos file
    var formData = new FormData();
    anexos.forEach(function (anexo) {
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        }
    });

    enviarArquivo(_anexos.config.url.anexar, dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo anexado com sucesso");
                $modalAnexos.modal('hide');
                _gridAnexos.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, "Não foi possível anexar arquivo.", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function EnviarAnexo(anexo) {
    var anexos = [anexo];
    var dados = {
        Codigo: _aceite.Codigo.val()
    };

    CriaEEnviaFormData(anexos, dados);
}

function LimparCamposAnexos() {
    LimparCampos(_anexos);
    file.value = null;
}

function ModalAnexo() {
    $modalAnexos.modal("show");
}