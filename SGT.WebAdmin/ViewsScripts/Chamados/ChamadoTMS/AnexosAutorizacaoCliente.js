/// <reference path="../../Enumeradores/EnumSituacaoChamadoTMS.js" />
/// <reference path="AutorizacaoCliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _anexosAutorizacaoCliente;
var _gridAnexosAutorizacaoCliente;

var AnexosAutorizacaoCliente = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexosAutorizacaoCliente.NomeArquivo.val(nomeArquivo);
    });

    this.AnexosEmail = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.AnexosEmail.val.subscribe(function () {
        RenderizarGridAnexosAutorizacaoCliente();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoAutorizacaoClienteClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

//*******EVENTOS*******


function loadAnexosAutorizacaoCliente() {
    _anexosAutorizacaoCliente = new AnexosAutorizacaoCliente();
    KoBindings(_anexosAutorizacaoCliente, "knockoutCadastroAnexosAutorizacaoCliente");

    //-- Grid AnexosAutorizacaoCliente
    // Opcoes
    var download = { descricao: "Download", id: guid(), metodo: downloadAnexoAutorizacaoClienteClick, icone: "", visibilidade: visibleDownloadAutorizacaoCliente };
    var remover = { descricao: "Remover", id: guid(), metodo: removerAnexoAutorizacaoClienteClick, icone: "", visibilidade: visibleRemoverAutorizacaoCliente };

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
    _gridAnexosAutorizacaoCliente = new BasicDataTable(_anexosAutorizacaoCliente.AnexosEmail.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosAutorizacaoCliente.CarregarGrid([]);

    //-- Controle de anexosAutorizacaoCliente
    _chamadoTMS.Situacao.val.subscribe(function () {
        AlternaTelaDeAnexosAutorizacaoCliente(_anexosAutorizacaoCliente);
    })
}

function visibleDownloadAutorizacaoCliente(dataRow) {
    return !isNaN(dataRow.Codigo);
}

function visibleRemoverAutorizacaoCliente(dataRow) {
    return PodeGerenciarAnexosAutorizacaoCliente();
}

function downloadAnexoAutorizacaoClienteClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("ChamadoTMSAnexos/DownloadAnexoAutorizacaoCliente", data);
}

function removerAnexoAutorizacaoClienteClick(dataRow) {
    var listaAnexosAutorizacaoCliente = GetAnexosAutorizacaoCliente();
    RemoverAnexoAutorizacaoCliente(dataRow, listaAnexosAutorizacaoCliente, _anexosAutorizacaoCliente, "ChamadoTMSAnexos/ExcluirAnexoAutorizacaoCliente");
}

function gerenciarAnexosAutorizacaoClienteClick() {
    LimparCamposAnexosAutorizacaoCliente();
    Global.abrirModal('divModalGerenciarAnexosAutorizacaoCliente');
}

function adicionarAnexoAutorizacaoClienteClick() {
    // Permissao
    if (!PodeGerenciarAnexosAutorizacaoCliente())
        return exibirMensagem(tipoMensagem.atencao, "AnexosAutorizacaoCliente", "Situação da Ocorrência não permite anexar arquivos.");

    // Busca o input de arquivos
    var file = document.getElementById(_anexosAutorizacaoCliente.Arquivo.id);

    // Valida
    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "AnexosAutorizacaoCliente", "Nenhum arquivo selecionado.");

    // Monta objeto anexo
    var anexo = {
        Codigo: guid(),
        Descricao: _anexosAutorizacaoCliente.Descricao.val(),
        NomeArquivo: _anexosAutorizacaoCliente.NomeArquivo.val(),
        Arquivo: file.files[0]
    };

    // Se ja esta cadastrada a ocorrencia, envia o anexo direto
    if (_chamadoTMS.Codigo.val() > 0) {
        EnviarAnexoAutorizacaoCliente(anexo);
    } else {
        // Clona a lista e adiciona o form data
        var listaAnexosAutorizacaoCliente = GetAnexosAutorizacaoCliente();
        listaAnexosAutorizacaoCliente.push(anexo);
        _anexosAutorizacaoCliente.AnexosEmail.val(listaAnexosAutorizacaoCliente.slice());
    }

    // Limpa os campos
    LimparCamposAnexosAutorizacaoCliente();
}



//*******MÉTODOS*******
function LimparCamposAnexosAutorizacaoCliente() {
    var file = document.getElementById(_anexosAutorizacaoCliente.Arquivo.id);
    LimparCampos(_anexosAutorizacaoCliente);
    _anexosAutorizacaoCliente.Arquivo.val("");
    file.value = null;
}

function GetAnexosAutorizacaoCliente() {
    // Retorna um clone do array para não prender a referencia
    return _anexosAutorizacaoCliente.AnexosEmail.val().slice();
}

function CarregarAnexosAutorizacaoCliente(data) {
    _anexosAutorizacaoCliente.AnexosEmail.val(data.AnexosEmail);
}

function RenderizarGridAnexosAutorizacaoCliente() {
    // Busca a lista
    var anexosAutorizacaoCliente = GetAnexosAutorizacaoCliente();

    // E chama o metodo da grid
    _gridAnexosAutorizacaoCliente.CarregarGrid(anexosAutorizacaoCliente);
}

function EnviarArquivosAnexadosAutorizacaoCliente(cb) {
    // Busca a lista
    var anexosAutorizacaoCliente = GetAnexosAutorizacaoCliente();

    if (anexosAutorizacaoCliente.length > 0) {
        var dados = {
            Chamado: _chamadoTMS.Codigo.val()
        }
        CriaEEnviaFormDataAutorizacaoCliente(anexosAutorizacaoCliente, dados, cb);
    } else if (cb != null) {
        cb();
    }
}

function RemoverAnexoAutorizacaoCliente(dataRow, listaAnexosAutorizacaoCliente, ko, url) {
    // Funcao auxiliar
    var RemoveDaGrid = function () {
        listaAnexosAutorizacaoCliente.forEach(function (anexo, i) {
            if (dataRow.Codigo == anexo.Codigo) {
                listaAnexosAutorizacaoCliente.splice(i, 1);
            }
        });

        ko.AnexosEmail.val(listaAnexosAutorizacaoCliente);
    }

    // Se for arquivo local, apenas remove do array
    if (isNaN(dataRow.Codigo)) {
        RemoveDaGrid();
    } else {
        // Permissao
        if (!PodeGerenciarAnexosAutorizacaoCliente())
            return exibirMensagem(tipoMensagem.atencao, "AnexosAutorizacaoCliente", "Situação da Ocorrência não permite excluir arquivos.");

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

function CriaEEnviaFormDataAutorizacaoCliente(anexosAutorizacaoCliente, dados, cb) {
    // Busca todos file
    var formData = new FormData();
    anexosAutorizacaoCliente.forEach(function (anexo) {
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        }
    });

    enviarArquivo("ChamadoTMSAnexos/AnexarArquivosAutorizacaoCliente?callback=?", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _anexosAutorizacaoCliente.AnexosEmail.val(arg.Data.Anexos);
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

function EnviarAnexoAutorizacaoCliente(anexo) {
    var anexosAutorizacaoCliente = [anexo];
    var dados = {
        Chamado: _chamadoTMS.Codigo.val()
    }

    CriaEEnviaFormDataAutorizacaoCliente(anexosAutorizacaoCliente, dados);
}

function limparOcorrenciaAnexosAutorizacaoCliente() {
    LimparCampos(_anexosAutorizacaoCliente);
    _anexosAutorizacaoCliente.AnexosEmail.val(_anexosAutorizacaoCliente.AnexosEmail.def);
    _anexosAutorizacaoCliente.Adicionar.visible(true);
}

function PodeGerenciarAnexosAutorizacaoCliente() {
    var situacao = _chamadoTMS.Situacao.val();

    return situacao != EnumSituacaoChamadoTMS.Finalizado && situacao != EnumSituacaoChamadoTMS.LiberadaOcorrencia && situacao != EnumSituacaoChamadoTMS.Cancelado;
}

function AlternaTelaDeAnexosAutorizacaoCliente(ko) {
    if (PodeGerenciarAnexosAutorizacaoCliente()) {
        ko.AnexosEmail.visible(true);
    } else {
        ko.AnexosEmail.visible(false);
    }
}