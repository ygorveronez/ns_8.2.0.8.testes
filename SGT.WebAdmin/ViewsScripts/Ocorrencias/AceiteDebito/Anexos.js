/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />

var _anexos;
var _modalAnexos;
var $modalAnexos;
var file;
var _modalGerenciarAnexos;

var Anexos = function () {
    this.Anexos = PropertyEntity({ type: types.local, text: "Anexos", val: ko.observable(""), idGrid: guid() });
    this.AdicionarAnexo = PropertyEntity({ eventClick: gerenciarAnexoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}


var ModalAnexos = function () {
    this.Aceite = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _modalAnexos.NomeArquivo.val(nomeArquivo);
    });
    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });

    this.config = PropertyEntity({
        url: {
            excluir: "AceiteDebitoAnexo/ExcluirAnexo",
            download: "AceiteDebitoAnexo/DownloadAnexo",
            anexar: "AceiteDebitoAnexo/AnexarArquivos?callback=?"
        }
    });
}



//*******EVENTOS*******
function loadAnexos() {
    _anexos = new Anexos();
    KoBindings(_anexos, "knockoutAnexos");

    _modalAnexos = new ModalAnexos();
    KoBindings(_modalAnexos, "knockoutModalAnexos");

    $modalAnexos = $("#divModalGerenciarAnexos");
    $modalAnexos.on('hidden.bs.modal', LimparCamposAnexos);

    file = document.getElementById(_modalAnexos.Arquivo.id);
    GridAnexos();
    _modalGerenciarAnexos = new bootstrap.Modal(document.getElementById("divModalGerenciarAnexos"), { backdrop: true, keyboard: true });
}

function gerenciarAnexoClick() {
    ModalAnexo();
}

function GridAnexos() {
    //-- Grid Anexos
    // Opcoes
    var download = { descricao: "Download", id: guid(), tamanho: 5, metodo: downloadAnexoClick, icone: "" };
    var remover = { descricao: "Remover", id: guid(), metodo: removerAnexoClick, icone: "" };
    var opcoes = [download];
    var _typeOptionMenu = TypeOptionMenu.link;

    if (PodeEditarAnexos()) {
        opcoes.push(remover);
        _typeOptionMenu = TypeOptionMenu.list;
    }

    // Menu
    var menuOpcoes = { tipo: _typeOptionMenu, descricao: "Opções", tamanho: 3, opcoes: opcoes};

    var ko_ocorrencia = {
        Codigo: _aceite.Ocorrencia
    };

    _gridAnexos = new GridView(_anexos.Anexos.idGrid, "AceiteDebitoAnexo/Pesquisa", ko_ocorrencia, menuOpcoes);
}

function downloadAnexoClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload(_modalAnexos.config.url.download, data);
}

function PodeEditarAnexos() {
    return _aceite.Situacao.val() == EnumSituacaoAceiteDebito.AgAceite;
}

function removerAnexoClick(dataRow, row) {
    // Exclui do sistema
    executarReST(_modalAnexos.config.url.excluir, dataRow, function (arg) {
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
        Descricao: _modalAnexos.Descricao.val(),
        NomeArquivo: _modalAnexos.NomeArquivo.val(),
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

    enviarArquivo(_modalAnexos.config.url.anexar, dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo anexado com sucesso");
                _modalGerenciarAnexos.hide();
                _gridAnexos.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, "Não foi possível anexar arquivo.", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function ReloadAnexos() {
    _gridAnexos.Destroy();
    GridAnexos();
    _gridAnexos.CarregarGrid();
}

function EnviarAnexo(anexo) {
    var anexos = [anexo];
    var dados = {
        Codigo: _aceite.Codigo.val()
    };

    CriaEEnviaFormData(anexos, dados);
}

function LimparCamposAnexos() {
    LimparCampos(_modalAnexos);
    file.value = null;
}

function ModalAnexo() {
    _modalGerenciarAnexos.show();
}