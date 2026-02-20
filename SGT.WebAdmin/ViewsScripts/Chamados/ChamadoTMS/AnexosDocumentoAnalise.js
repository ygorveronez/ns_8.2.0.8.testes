/// <reference path="../../Enumeradores/EnumSituacaoChamadoTMS.js" />
/// <reference path="Analise.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _anexosDocumentoAnalise;
var _gridAnexosDocumentoAnalise;

var AnexosDocumentoAnalise = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexosDocumentoAnalise.NomeArquivo.val(nomeArquivo);
    });

    this.AnexosDocumentoAnalise = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.AnexosDocumentoAnalise.val.subscribe(function () {
        RenderizarGridAnexosDocumentoAnalise();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoDocumentoAnaliseClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

//*******EVENTOS*******


function loadAnexosDocumentoAnalise() {
    _anexosDocumentoAnalise = new AnexosDocumentoAnalise();
    KoBindings(_anexosDocumentoAnalise, "knockoutCadastroAnexosDocumentoAnalise");

    //-- Grid AnexosDocumentoAnalise
    // Opcoes
    var download = { descricao: "Download", id: guid(), metodo: downloadAnexoDocumentoAnaliseClick, icone: "", visibilidade: visibleDownloadDocumentoAnalise };
    var remover = { descricao: "Remover", id: guid(), metodo: removerAnexoDocumentoAnaliseClick, icone: "", visibilidade: visibleRemoverDocumentoAnalise };

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
    _gridAnexosDocumentoAnalise = new BasicDataTable(_anexosDocumentoAnalise.AnexosDocumentoAnalise.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosDocumentoAnalise.CarregarGrid([]);

    //-- Controle de anexosDocumentoAnalise
    _chamadoTMS.Situacao.val.subscribe(function () {
        AlternaTelaDeAnexosDocumentoAnalise(_anexosDocumentoAnalise);
    })
}

function visibleDownloadDocumentoAnalise(dataRow) {
    return !isNaN(dataRow.Codigo);
}

function visibleRemoverDocumentoAnalise(dataRow) {
    return PodeGerenciarAnexosDocumentoAnalise();
}

function downloadAnexoDocumentoAnaliseClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("ChamadoTMSAnexos/DownloadAnexoDocumentoAnalise", data);
}

function removerAnexoDocumentoAnaliseClick(dataRow) {
    var listaAnexosDocumentoAnalise = GetAnexosDocumentoAnalise();
    RemoverAnexoDocumentoAnalise(dataRow, listaAnexosDocumentoAnalise, _anexosDocumentoAnalise, "ChamadoTMSAnexos/ExcluirAnexoDocumentoAnalise");
}

function gerenciarAnexosDocumentoAnaliseClick() {
    LimparCamposAnexosDocumentoAnalise();
    Global.abrirModal('divModalGerenciarAnexosDocumentoAnalise');
}

function adicionarAnexoDocumentoAnaliseClick() {
    // Permissao
    if (!PodeGerenciarAnexosDocumentoAnalise())
        return exibirMensagem(tipoMensagem.atencao, "AnexosDocumentoAnalise", "Situação da Ocorrência não permite anexar arquivos.");

    // Busca o input de arquivos
    var file = document.getElementById(_anexosDocumentoAnalise.Arquivo.id);

    // Valida
    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "AnexosDocumentoAnalise", "Nenhum arquivo selecionado.");

    // Monta objeto anexo
    var anexo = {
        Codigo: guid(),
        Descricao: _anexosDocumentoAnalise.Descricao.val(),
        NomeArquivo: _anexosDocumentoAnalise.NomeArquivo.val(),
        Arquivo: file.files[0]
    };

    // Se ja esta cadastrada a ocorrencia, envia o anexo direto
    if (_chamadoTMS.Codigo.val() > 0) {
        EnviarAnexoDocumentoAnalise(anexo);
    } else {
        // Clona a lista e adiciona o form data
        var listaAnexosDocumentoAnalise = GetAnexosDocumentoAnalise();
        listaAnexosDocumentoAnalise.push(anexo);
        _anexosDocumentoAnalise.AnexosDocumentoAnalise.val(listaAnexosDocumentoAnalise.slice());
    }

    // Limpa os campos
    LimparCamposAnexosDocumentoAnalise();
}



//*******MÉTODOS*******
function LimparCamposAnexosDocumentoAnalise() {
    var file = document.getElementById(_anexosDocumentoAnalise.Arquivo.id);
    LimparCampos(_anexosDocumentoAnalise);
    _anexosDocumentoAnalise.Arquivo.val("");
    file.value = null;
}

function GetAnexosDocumentoAnalise() {
    // Retorna um clone do array para não prender a referencia
    return _anexosDocumentoAnalise.AnexosDocumentoAnalise.val().slice();
}

function CarregarAnexosDocumentoAnalise(data) {
    _anexosDocumentoAnalise.AnexosDocumentoAnalise.val(data.AnexosDocumentoAnalise);
}

function RenderizarGridAnexosDocumentoAnalise() {
    // Busca a lista
    var anexosDocumentoAnalise = GetAnexosDocumentoAnalise();

    // E chama o metodo da grid
    _gridAnexosDocumentoAnalise.CarregarGrid(anexosDocumentoAnalise);
}

function EnviarArquivosAnexadosDocumentoAnalise(cb) {
    // Busca a lista
    var anexosDocumentoAnalise = GetAnexosDocumentoAnalise();

    if (anexosDocumentoAnalise.length > 0) {
        var dados = {
            Chamado: _chamadoTMS.Codigo.val()
        }
        CriaEEnviaFormDataDocumentoAnalise(anexosDocumentoAnalise, dados, cb);
    } else if (cb != null) {
        cb();
    }
}

function RemoverAnexoDocumentoAnalise(dataRow, listaAnexosDocumentoAnalise, ko, url) {
    // Funcao auxiliar
    var RemoveDaGrid = function () {
        listaAnexosDocumentoAnalise.forEach(function (anexo, i) {
            if (dataRow.Codigo == anexo.Codigo) {
                listaAnexosDocumentoAnalise.splice(i, 1);
            }
        });

        ko.AnexosDocumentoAnalise.val(listaAnexosDocumentoAnalise);
    }

    // Se for arquivo local, apenas remove do array
    if (isNaN(dataRow.Codigo)) {
        RemoveDaGrid();
    } else {
        // Permissao
        if (!PodeGerenciarAnexosDocumentoAnalise())
            return exibirMensagem(tipoMensagem.atencao, "AnexosDocumentoAnalise", "Situação da Ocorrência não permite excluir arquivos.");

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

function CriaEEnviaFormDataDocumentoAnalise(anexosDocumentoAnalise, dados, cb) {
    // Busca todos file
    var formData = new FormData();
    anexosDocumentoAnalise.forEach(function (anexo) {
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        }
    });

    enviarArquivo("ChamadoTMSAnexos/AnexarArquivosDocumentoAnalise?callback=?", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _anexosDocumentoAnalise.AnexosDocumentoAnalise.val(arg.Data.Anexos);
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

function EnviarAnexoDocumentoAnalise(anexo) {
    var anexosDocumentoAnalise = [anexo];
    var dados = {
        Chamado: _chamadoTMS.Codigo.val()
    }

    CriaEEnviaFormDataDocumentoAnalise(anexosDocumentoAnalise, dados);
}

function limparOcorrenciaAnexosDocumentoAnalise() {
    LimparCampos(_anexosDocumentoAnalise);
    _anexosDocumentoAnalise.AnexosDocumentoAnalise.val(_anexosDocumentoAnalise.AnexosDocumentoAnalise.def);
    _anexosDocumentoAnalise.Adicionar.visible(true);
}

function PodeGerenciarAnexosDocumentoAnalise() {
    var situacao = _chamadoTMS.Situacao.val();

    return situacao != EnumSituacaoChamadoTMS.Finalizado && situacao != EnumSituacaoChamadoTMS.LiberadaOcorrencia && situacao != EnumSituacaoChamadoTMS.Cancelado;
}

function AlternaTelaDeAnexosDocumentoAnalise(ko) {
    if (PodeGerenciarAnexosDocumentoAnalise()) {
        ko.AnexosDocumentoAnalise.visible(true);
    } else {
        ko.AnexosDocumentoAnalise.visible(false);
    }
}