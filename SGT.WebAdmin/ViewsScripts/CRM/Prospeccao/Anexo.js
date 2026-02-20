/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _anexos;
var _gridAnexos;
var _modalGerenciarAnexos;

var Anexos = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexos.NomeArquivo.val(nomeArquivo);
    });

    this.Anexos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(function () {
        RenderizarGridAnexos();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

//*******EVENTOS*******


function loadAnexos() {
    _anexos = new Anexos();
    KoBindings(_anexos, "knockoutCadastroAnexos");

    //-- Grid Anexos
    // Opcoes
    var download = { descricao: "Download", id: guid(), metodo: downloadAnexoClick, icone: "", visibilidade: visibleDownload };
    var remover = { descricao: "Remover", id: guid(), metodo: removerAnexoClick, icone: "", visibilidade: visibleRemover };

    // Menu
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 9, opcoes: [download, remover] };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "70%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "25%", className: "text-align-left" }
    ];

    // Grid
    var linhasPorPaginas = 7;
    _gridAnexos = new BasicDataTable(_anexos.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexos.CarregarGrid([]);

    //-- Controle de anexos
    _prospeccao.Situacao.val.subscribe(function () {
        AlternaTelaDeAnexos(_anexos);
    })
    _modalGerenciarAnexos = new bootstrap.Modal(document.getElementById("divModalGerenciarAnexos"), { backdrop: true, keyboard: true });
}

function visibleDownload(dataRow) {
    return !isNaN(dataRow.Codigo);
}

function visibleRemover(dataRow) {
    return PodeGerenciarAnexos();
}

function downloadAnexoClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("ProspeccaoAnexo/DownloadAnexo", data);
}

function removerAnexoClick(dataRow) {
    var listaAnexos = GetAnexos();
    RemoverAnexo(dataRow, listaAnexos, _anexos, "ProspeccaoAnexo/ExcluirAnexo");
}

function gerenciarAnexosClick() {
    _modalGerenciarAnexos.show();
    LimparCamposAnexos();
}

function adicionarAnexoClick() {
    // Permissao
    if (!PodeGerenciarAnexos())
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Situação da Ocorrência não permite anexar arquivos.");

    // Busca o input de arquivos
    var file = document.getElementById(_anexos.Arquivo.id);

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
    if (_prospeccao.Codigo.val() > 0) {
        EnviarAnexo(anexo);
    } else {
        // Clona a lista e adiciona o form data
        var listaAnexos = GetAnexos();
        listaAnexos.push(anexo);
        _anexos.Anexos.val(listaAnexos.slice());
    }

    // Limpa os campos
    LimparCampoxsAnexos();
}



//*******MÉTODOS*******
function LimparCamposAnexos() {
    var file = document.getElementById(_anexos.Arquivo.id);
    LimparCampos(_anexos);
    _anexos.Arquivo.val("");
    file.value = null;
}

function GetAnexos() {
    // Retorna um clone do array para não prender a referencia
    return _anexos.Anexos.val().slice();
}

function CarregarAnexos(data) {
    _anexos.Anexos.val(data.Anexos);
}

function RenderizarGridAnexos() {
    // Busca a lista
    var anexos = GetAnexos();

    // E chama o metodo da grid
    _gridAnexos.CarregarGrid(anexos);
}

function EnviarArquivosAnexados(cb) {
    // Busca a lista
    var anexos = GetAnexos();

    if (anexos.length > 0) {
        var dados = {
            Prospeccao: _prospeccao.Codigo.val()
        }
        CriaEEnviaFormData(anexos, dados, cb);
    } else if (cb != null) {
        cb();
    }
}

function RemoverAnexo(dataRow, listaAnexos, ko, url) {
    // Funcao auxiliar
    var RemoveDaGrid = function () {
        listaAnexos.forEach(function (anexo, i) {
            if (dataRow.Codigo == anexo.Codigo) {
                listaAnexos.splice(i, 1);
            }
        });

        ko.Anexos.val(listaAnexos);
    }

    // Se for arquivo local, apenas remove do array
    if (isNaN(dataRow.Codigo)) {
        RemoveDaGrid();
    } else {
        // Permissao
        if (!PodeGerenciarAnexos())
            return exibirMensagem(tipoMensagem.atencao, "Anexos", "Situação da Ocorrência não permite excluir arquivos.");

        // Exclui do sistema
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

function CriaEEnviaFormData(anexos, dados, cb) {
    // Busca todos file
    var formData = new FormData();
    anexos.forEach(function (anexo) {
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        }
    });

    enviarArquivo("ProspeccaoAnexo/AnexarArquivos?callback=?", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _anexos.Anexos.val(arg.Data.Anexos);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo anexado com sucesso");
            } else {
                exibirMensagem(tipoMensagem.falha, "Não foi possível anexar arquivo.", arg.Msg);
            }
            if (cb)
                cb();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function EnviarAnexo(anexo) {
    var anexos = [anexo];
    var dados = {
        Prospeccao: _prospeccao.Codigo.val()
    }

    CriaEEnviaFormData(anexos, dados);
}

function limparAnexos() {
    LimparCampos(_anexos);
    _anexos.Anexos.val(_anexos.Anexos.def);
    _anexos.Adicionar.visible(true);
}

function PodeGerenciarAnexos() {
    var situacao = _prospeccao.EnumSituacao.val();

    return EnumSituacaoProspeccao.Pendente == situacao;
}

function AlternaTelaDeAnexos(ko) {
    if (PodeGerenciarAnexos()) {
        ko.Anexos.visible(true);
    } else {
        ko.Anexos.visible(false);
    }
}