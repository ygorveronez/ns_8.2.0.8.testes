/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="ApoliceSeguro.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _anexosApolice;
var _gridAnexosApolice;

var AnexosApolice = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexosApolice.NomeArquivo.val(nomeArquivo);
    });

    this.Anexos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(function () {
        RenderizarGridAnexosApolice();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoApoliceClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadAnexosApolice() {
    _anexosApolice = new AnexosApolice();
    KoBindings(_anexosApolice, "knockoutCadastroAnexosApolice");

    var download = { descricao: "Download", id: guid(), metodo: downloadAnexoApoliceClick, icone: "", visibilidade: visibleDownloadAnexoApolice };
    var remover = { descricao: "Remover", id: guid(), metodo: removerAnexoApoliceClick, icone: ""};

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 9, opcoes: [download, remover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "60%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "25%", className: "text-align-left" }
    ];

    var linhasPorPaginas = 7;
    _gridAnexosApolice = new BasicDataTable(_anexosApolice.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosApolice.CarregarGrid([]);
}

function visibleDownloadAnexoApolice(dataRow) {
    return !isNaN(dataRow.Codigo);
}

function downloadAnexoApoliceClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("ApoliceSeguroAnexos/DownloadAnexo", data);
}

function removerAnexoApoliceClick(dataRow) {
    var listaAnexosApolice = GetAnexosApolice();
    RemoverAnexoApolice(dataRow, listaAnexosApolice, _anexosApolice, "ApoliceSeguroAnexos/ExcluirAnexo");
}

function gerenciarAnexosClick() {
    LimparCamposAnexosApolice();
    Global.abrirModal('divModalGerenciarAnexosApolice');
}

function adicionarAnexoApoliceClick() {
    var file = document.getElementById(_anexosApolice.Arquivo.id);

    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Atenção", "Nenhum arquivo selecionado.");

    var anexo = {
        Codigo: guid(),
        Descricao: _anexosApolice.Descricao.val(),
        NomeArquivo: _anexosApolice.NomeArquivo.val(),
        Arquivo: file.files[0]
    };

    if (_apoliceSeguro.Codigo.val() > 0) {
        EnviarAnexoApolice(anexo);
    } else {
        var listaAnexosApolice = GetAnexosApolice();
        listaAnexosApolice.push(anexo);
        _anexosApolice.Anexos.val(listaAnexosApolice.slice());
    }

    LimparCamposAnexosApolice();
}

//*******MÉTODOS*******

function LimparCamposAnexosApolice() {
    var file = document.getElementById(_anexosApolice.Arquivo.id);
    LimparCampos(_anexosApolice);
    _anexosApolice.Arquivo.val("");
    file.value = null;
}

function GetAnexosApolice() {
    // Retorna um clone do array para não prender a referencia
    return _anexosApolice.Anexos.val().slice();
}

function CarregarAnexosApolice(data) {
    _anexosApolice.Anexos.val(data.Anexos);
}

function RenderizarGridAnexosApolice() {
    var anexosApolice = GetAnexosApolice();

    var lista = new Array();
    anexosApolice.forEach(function (anexo, i) {
        var data = new Object();

        data.Codigo = anexo.Codigo;
        data.Descricao = anexo.Descricao;
        data.NomeArquivo = anexo.NomeArquivo;

        lista.push(data);
    });

    _gridAnexosApolice.CarregarGrid(lista);
}

function EnviarArquivosAnexadosApolice(codigoApoliceSeguro) {
    // Busca a lista
    var anexosApolice = GetAnexosApolice();

    if (anexosApolice.length > 0) {
        var dados = {
            ApoliceSeguro: codigoApoliceSeguro
        }
        CriaEEnviaFormDataApolice(anexosApolice, dados);
    }
}

function RemoverAnexoApolice(dataRow, listaAnexosApolice, ko, url) {
    // Funcao auxiliar
    var RemoveDaGrid = function () {
        listaAnexosApolice.forEach(function (anexo, i) {
            if (dataRow.Codigo == anexo.Codigo) {
                listaAnexosApolice.splice(i, 1);
            }
        });

        ko.Anexos.val(listaAnexosApolice);
    }

    // Se for arquivo local, apenas remove do array
    if (isNaN(dataRow.Codigo)) {
        RemoveDaGrid();
    } else {
        executarReST(url, dataRow, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    RemoveDaGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function CriaEEnviaFormDataApolice(anexosApolice, dados) {
    var formData = new FormData();
    anexosApolice.forEach(function (anexo) {
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
            formData.append("NotaFiscalServico", anexo.NotaFiscalServico);
        }
    });

    enviarArquivo("ApoliceSeguroAnexos/AnexarArquivos?callback=?", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _anexosApolice.Anexos.val(arg.Data.Anexos);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo anexado com sucesso");
            } else {
                exibirMensagem(tipoMensagem.falha, "Não foi possível anexar arquivo.", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function EnviarAnexoApolice(anexo) {
    var anexosApolice = [anexo];
    var dados = {
        ApoliceSeguro: _apoliceSeguro.Codigo.val()
    }

    CriaEEnviaFormDataApolice(anexosApolice, dados);
}

function limparAnexosApolice() {
    LimparCampos(_anexosApolice);
    _anexosApolice.Anexos.val(_anexosApolice.Anexos.def);
    _anexosApolice.Adicionar.visible(true);
}

function isPossuiAnexo() {
    return GetAnexosApolice().length > 0;
}