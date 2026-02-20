/// <reference path="../../Enumeradores/EnumSituacaoChamadoTMS.js" />
/// <reference path="PendenciaMotorista.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _anexosPendenciaMotorista;
var _gridAnexosPendenciaMotorista;

var AnexosPendenciaMotorista = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexosPendenciaMotorista.NomeArquivo.val(nomeArquivo);
    });

    this.Anexos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(function () {
        RenderizarGridAnexosPendenciaMotorista();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoPendenciaMotoristaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

//*******EVENTOS*******


function loadAnexosPendenciaMotorista() {
    _anexosPendenciaMotorista = new AnexosPendenciaMotorista();
    KoBindings(_anexosPendenciaMotorista, "knockoutCadastroAnexosPendenciaMotorista");

    // Opcoes
    var download = { descricao: "Download", id: guid(), metodo: downloadAnexoPendenciaMotoristaClick, icone: "", visibilidade: visibleDownloadPendenciaMotorista };
    var remover = { descricao: "Remover", id: guid(), metodo: removerAnexoPendenciaMotoristaClick, icone: "", visibilidade: visibleRemoverPendenciaMotorista };

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
    _gridAnexosPendenciaMotorista = new BasicDataTable(_anexosPendenciaMotorista.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosPendenciaMotorista.CarregarGrid([]);

    /*
    _pendenciaMotorista.Situacao.val.subscribe(function () {
        AlternaTelaDeAnexosPendenciaMotorista(_anexosPendenciaMotorista);
    })*/
}

function visibleDownloadPendenciaMotorista(dataRow) {
    return !isNaN(dataRow.Codigo);
}

function visibleRemoverPendenciaMotorista(dataRow) {
    return true;
}

function downloadAnexoPendenciaMotoristaClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("PendenciaMotoristaAnexos/DownloadAnexo", data);
}

function removerAnexoPendenciaMotoristaClick(dataRow) {
    var listaAnexosPendenciaMotorista = GetAnexosPendenciaMotorista();
    RemoverAnexoPendenciaMotorista(dataRow, listaAnexosPendenciaMotorista, _anexosPendenciaMotorista, "PendenciaMotoristaAnexos/ExcluirAnexo");
}

function gerenciarAnexosPendenciaMotoristaClick() {
    LimparCamposAnexosPendenciaMotorista();
    Global.abrirModal('divModalGerenciarAnexosPendenciaMotorista');
}

function adicionarAnexoPendenciaMotoristaClick() {


    // Busca o input de arquivos
    var file = document.getElementById(_anexosPendenciaMotorista.Arquivo.id);

    // Valida
    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "AnexosPendenciaMotorista", "Nenhum arquivo selecionado.");

    // Monta objeto anexo
    var anexo = {
        Codigo: guid(),
        Descricao: _anexosPendenciaMotorista.Descricao.val(),
        NomeArquivo: _anexosPendenciaMotorista.NomeArquivo.val(),
        Arquivo: file.files[0]
    };

    // Se ja esta cadastrada a ocorrencia, envia o anexo direto
    if (_pendenciaMotorista.Codigo.val() > 0) {
        EnviarAnexoPendenciaMotorista(anexo);
    } else {
        // Clona a lista e adiciona o form data
        var listaAnexosPendenciaMotorista = GetAnexosPendenciaMotorista();
        listaAnexosPendenciaMotorista.push(anexo);
        _anexosPendenciaMotorista.Anexos.val(listaAnexosPendenciaMotorista.slice());
    }

    // Limpa os campos
    LimparCamposAnexosPendenciaMotorista();
}


//*******MÉTODOS*******
function LimparCamposAnexosPendenciaMotorista() {
    var file = document.getElementById(_anexosPendenciaMotorista.Arquivo.id);
    LimparCampos(_anexosPendenciaMotorista);
    _anexosPendenciaMotorista.Arquivo.val("");
    file.value = null;
}

function GetAnexosPendenciaMotorista() {
    // Retorna um clone do array para não prender a referencia
    console.log(_anexosPendenciaMotorista.Anexos);
    return _anexosPendenciaMotorista.Anexos.val().slice();
}

function CarregarAnexosPendenciaMotorista(data) {
    _anexosPendenciaMotorista.Anexos.val(data.Anexos);
}

function RenderizarGridAnexosPendenciaMotorista() {
    // Busca a lista
    var anexosPendenciaMotorista = GetAnexosPendenciaMotorista();

    // E chama o metodo da grid
    _gridAnexosPendenciaMotorista.CarregarGrid(anexosPendenciaMotorista);
}

function EnviarArquivosAnexadosPendenciaMotorista(cb) {
    // Busca a lista
    var anexosPendenciaMotorista = GetAnexosPendenciaMotorista();

    if (anexosPendenciaMotorista.length > 0) {
        var dados = {
            PendenciaMotorista: _pendenciaMotorista.Codigo.val()

        }
        CriaEEnviaFormDataPendenciaMotorista(anexosPendenciaMotorista, dados, cb);
    } else if (cb != null) {
        cb();
    }
}

function RemoverAnexoPendenciaMotorista(dataRow, listaAnexosPendenciaMotorista, ko, url) {
    // Funcao auxiliar
    var RemoveDaGrid = function () {
        listaAnexosPendenciaMotorista.forEach(function (anexo, i) {
            if (dataRow.Codigo == anexo.Codigo) {
                listaAnexosPendenciaMotorista.splice(i, 1);
            }
        });

        ko.Anexos.val(listaAnexosPendenciaMotorista);
    }

    // Se for arquivo local, apenas remove do array
    if (isNaN(dataRow.Codigo)) {
        RemoveDaGrid();
    } else {

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

function CriaEEnviaFormDataPendenciaMotorista(anexosPendenciaMotorista, dados, cb) {
    // Busca todos file
    var formData = new FormData();
    anexosPendenciaMotorista.forEach(function (anexo) {
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        }
    });

    enviarArquivo("PendenciaMotoristaAnexos/AnexarArquivos?callback=?", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _anexosPendenciaMotorista.Anexos.val(arg.Data.Anexos);
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

function EnviarAnexoPendenciaMotorista(anexo) {
    var anexosPendenciaMotorista = [anexo];
    var dados = {
        PendenciaMotorista: _pendenciaMotorista.Codigo.val()
    }

    CriaEEnviaFormDataPendenciaMotorista(anexosPendenciaMotorista, dados);
}

function limparOcorrenciaAnexosPendenciaMotorista() {
    LimparCampos(_anexosPendenciaMotorista);
    _anexosPendenciaMotorista.Anexos.val(_anexosPendenciaMotorista.Anexos.def);
    _anexosPendenciaMotorista.Adicionar.visible(true);
}



function AlternaTelaDeAnexosPendenciaMotorista(ko) {
    ko.Anexos.visible(true);

}