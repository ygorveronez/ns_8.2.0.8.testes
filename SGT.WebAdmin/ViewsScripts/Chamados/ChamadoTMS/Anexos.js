/// <reference path="../../Enumeradores/EnumSituacaoChamadoTMS.js" />
/// <reference path="ChamadoTMS.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _anexosChamados;
var _gridAnexosChamados;

var AnexosChamados = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexosChamados.NomeArquivo.val(nomeArquivo);
    });

    this.Anexos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(function () {
        RenderizarGridAnexosChamados();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoChamadoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

//*******EVENTOS*******


function loadAnexosChamados() {
    _anexosChamados = new AnexosChamados();
    KoBindings(_anexosChamados, "knockoutCadastroAnexosChamado");

    //-- Grid AnexosChamados
    // Opcoes
    var download = { descricao: "Download", id: guid(), metodo: downloadAnexoChamadoClick, icone: "", visibilidade: visibleDownloadChamado };
    var remover = { descricao: "Remover", id: guid(), metodo: removerAnexoChamadoClick, icone: "", visibilidade: visibleRemoverChamado };

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
    _gridAnexosChamados = new BasicDataTable(_anexosChamados.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosChamados.CarregarGrid([]);

    //-- Controle de anexosChamados
    _chamadoTMS.Situacao.val.subscribe(function () {
        AlternaTelaDeAnexosChamados(_anexosChamados);
    })
}

function visibleDownloadChamado(dataRow) {
    return !isNaN(dataRow.Codigo);
}

function visibleRemoverChamado(dataRow) {
    return PodeGerenciarAnexosChamados();
}

function downloadAnexoChamadoClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("ChamadoTMSAnexos/DownloadAnexo", data);
}

function removerAnexoChamadoClick(dataRow) {
    var listaAnexosChamados = GetAnexosChamados();
    RemoverAnexoChamado(dataRow, listaAnexosChamados, _anexosChamados, "ChamadoTMSAnexos/ExcluirAnexo");
}

function gerenciarAnexosChamadosClick() {
    LimparCamposAnexosChamado();
    Global.abrirModal('divModalGerenciarAnexosChamados');
}

function adicionarAnexoChamadoClick() {
    // Permissao
    if (!PodeGerenciarAnexosChamados())
        return exibirMensagem(tipoMensagem.atencao, "AnexosChamados", "Situação da Ocorrência não permite anexar arquivos.");

    // Busca o input de arquivos
    var file = document.getElementById(_anexosChamados.Arquivo.id);

    // Valida
    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "AnexosChamados", "Nenhum arquivo selecionado.");

    // Monta objeto anexo
    var anexo = {
        Codigo: guid(),
        Descricao: _anexosChamados.Descricao.val(),
        NomeArquivo: _anexosChamados.NomeArquivo.val(),
        Arquivo: file.files[0]
    };

    // Se ja esta cadastrada a ocorrencia, envia o anexo direto
    if (_chamadoTMS.Codigo.val() > 0) {
        EnviarAnexoChamado(anexo);
    } else {
        // Clona a lista e adiciona o form data
        var listaAnexosChamados = GetAnexosChamados();
        listaAnexosChamados.push(anexo);
        _anexosChamados.Anexos.val(listaAnexosChamados.slice());
    }

    // Limpa os campos
    LimparCamposAnexosChamado();
}


//*******MÉTODOS*******
function LimparCamposAnexosChamado() {
    var file = document.getElementById(_anexosChamados.Arquivo.id);
    LimparCampos(_anexosChamados);
    _anexosChamados.Arquivo.val("");
    file.value = null;
}

function GetAnexosChamados() {
    // Retorna um clone do array para não prender a referencia
    return _anexosChamados.Anexos.val().slice();
}

function CarregarAnexosChamados(data) {
    _anexosChamados.Anexos.val(data.Anexos);
}

function RenderizarGridAnexosChamados() {
    // Busca a lista
    var anexosChamados = GetAnexosChamados();

    // E chama o metodo da grid
    _gridAnexosChamados.CarregarGrid(anexosChamados);
}

function EnviarArquivosAnexadosChamado(cb) {
    // Busca a lista
    var anexosChamados = GetAnexosChamados();

    if (anexosChamados.length > 0) {
        var dados = {
            Chamado: _chamadoTMS.Codigo.val()
        }
        CriaEEnviaFormDataChamado(anexosChamados, dados, cb);
    } else if (cb != null) {
        cb();
    }
}

function RemoverAnexoChamado(dataRow, listaAnexosChamados, ko, url) {
    // Funcao auxiliar
    var RemoveDaGrid = function () {
        listaAnexosChamados.forEach(function (anexo, i) {
            if (dataRow.Codigo == anexo.Codigo) {
                listaAnexosChamados.splice(i, 1);
            }
        });

        ko.Anexos.val(listaAnexosChamados);
    }

    // Se for arquivo local, apenas remove do array
    if (isNaN(dataRow.Codigo)) {
        RemoveDaGrid();
    } else {
        // Permissao
        if (!PodeGerenciarAnexosChamados())
            return exibirMensagem(tipoMensagem.atencao, "AnexosChamados", "Situação da Ocorrência não permite excluir arquivos.");

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

function CriaEEnviaFormDataChamado(anexosChamados, dados, cb) {
    // Busca todos file
    var formData = new FormData();
    anexosChamados.forEach(function (anexo) {
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        }
    });

    enviarArquivo("ChamadoTMSAnexos/AnexarArquivos?callback=?", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _anexosChamados.Anexos.val(arg.Data.Anexos);
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

function EnviarAnexoChamado(anexo) {
    var anexosChamados = [anexo];
    var dados = {
        Chamado: _chamadoTMS.Codigo.val()
    }

    CriaEEnviaFormDataChamado(anexosChamados, dados);
}

function limparOcorrenciaAnexosChamados() {
    LimparCampos(_anexosChamados);
    _anexosChamados.Anexos.val(_anexosChamados.Anexos.def);
    _anexosChamados.Adicionar.visible(true);
}

function PodeGerenciarAnexosChamados() {
    var situacao = _chamadoTMS.Situacao.val();

    return situacao != EnumSituacaoChamadoTMS.Finalizado && situacao != EnumSituacaoChamadoTMS.LiberadaOcorrencia && situacao != EnumSituacaoChamadoTMS.Cancelado;
}

function AlternaTelaDeAnexosChamados(ko) {
    if (PodeGerenciarAnexosChamados()) {
        ko.Anexos.visible(true);
    } else {
        ko.Anexos.visible(false);
    }
}