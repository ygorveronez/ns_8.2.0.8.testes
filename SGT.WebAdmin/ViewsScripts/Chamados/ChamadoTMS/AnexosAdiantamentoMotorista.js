/// <reference path="../../Enumeradores/EnumSituacaoChamadoTMS.js" />
/// <reference path="AdiantamentoMotorista.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _anexosAdiantamentoMotorista;
var _gridAnexosAdiantamentoMotorista;

var AnexosAdiantamentoMotorista = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexosAdiantamentoMotorista.NomeArquivo.val(nomeArquivo);
    });

    this.AnexosAdiantamentoMotorista = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.AnexosAdiantamentoMotorista.val.subscribe(function () {
        RenderizarGridAnexosAdiantamentoMotorista();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoAdiantamentoMotoristaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

//*******EVENTOS*******


function loadAnexosAdiantamentoMotorista() {
    _anexosAdiantamentoMotorista = new AnexosAdiantamentoMotorista();
    KoBindings(_anexosAdiantamentoMotorista, "knockoutCadastroAnexosAdiantamentoMotorista");

    //-- Grid AnexosAdiantamentoMotorista
    // Opcoes
    var download = { descricao: "Download", id: guid(), metodo: downloadAnexoAdiantamentoMotoristaClick, icone: "", visibilidade: visibleDownloadAdiantamentoMotorista };
    var remover = { descricao: "Remover", id: guid(), metodo: removerAnexoAdiantamentoMotoristaClick, icone: "", visibilidade: visibleRemoverAdiantamentoMotorista };

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
    _gridAnexosAdiantamentoMotorista = new BasicDataTable(_anexosAdiantamentoMotorista.AnexosAdiantamentoMotorista.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosAdiantamentoMotorista.CarregarGrid([]);

    //-- Controle de anexosAdiantamentoMotorista
    _chamadoTMS.Situacao.val.subscribe(function () {
        AlternaTelaDeAnexosAdiantamentoMotorista(_anexosAdiantamentoMotorista);
    })
}

function visibleDownloadAdiantamentoMotorista(dataRow) {
    return !isNaN(dataRow.Codigo);
}

function visibleRemoverAdiantamentoMotorista(dataRow) {
    return PodeGerenciarAnexosAdiantamentoMotorista();
}

function downloadAnexoAdiantamentoMotoristaClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("ChamadoTMSAnexos/DownloadAnexoAdiantamentoMotorista", data);
}

function removerAnexoAdiantamentoMotoristaClick(dataRow) {
    var listaAnexosAdiantamentoMotorista = GetAnexosAdiantamentoMotorista();
    RemoverAnexoAdiantamentoMotorista(dataRow, listaAnexosAdiantamentoMotorista, _anexosAdiantamentoMotorista, "ChamadoTMSAnexos/ExcluirAnexoAdiantamentoMotorista");
}

function gerenciarAnexosAdiantamentoMotoristaClick() {
    LimparCamposAnexosAdiantamentoMotorista();
    Global.abrirModal('divModalGerenciarAnexosAdiantamentoMotorista');
}

function adicionarAnexoAdiantamentoMotoristaClick() {
    // Permissao
    if (!PodeGerenciarAnexosAdiantamentoMotorista())
        return exibirMensagem(tipoMensagem.atencao, "AnexosAdiantamentoMotorista", "Situação da Ocorrência não permite anexar arquivos.");

    // Busca o input de arquivos
    var file = document.getElementById(_anexosAdiantamentoMotorista.Arquivo.id);

    // Valida
    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "AnexosAdiantamentoMotorista", "Nenhum arquivo selecionado.");

    // Monta objeto anexo
    var anexo = {
        Codigo: guid(),
        Descricao: _anexosAdiantamentoMotorista.Descricao.val(),
        NomeArquivo: _anexosAdiantamentoMotorista.NomeArquivo.val(),
        Arquivo: file.files[0]
    };

    // Se ja esta cadastrada a ocorrencia, envia o anexo direto
    if (_chamadoTMS.Codigo.val() > 0) {
        EnviarAnexoAdiantamentoMotorista(anexo);
    } else {
        // Clona a lista e adiciona o form data
        var listaAnexosAdiantamentoMotorista = GetAnexosAdiantamentoMotorista();
        listaAnexosAdiantamentoMotorista.push(anexo);
        _anexosAdiantamentoMotorista.AnexosAdiantamentoMotorista.val(listaAnexosAdiantamentoMotorista.slice());
    }

    // Limpa os campos
    LimparCamposAnexosAdiantamentoMotorista();
}



//*******MÉTODOS*******
function LimparCamposAnexosAdiantamentoMotorista() {
    var file = document.getElementById(_anexosAdiantamentoMotorista.Arquivo.id);
    LimparCampos(_anexosAdiantamentoMotorista);
    _anexosAdiantamentoMotorista.Arquivo.val("");
    file.value = null;
}

function GetAnexosAdiantamentoMotorista() {
    // Retorna um clone do array para não prender a referencia
    return _anexosAdiantamentoMotorista.AnexosAdiantamentoMotorista.val().slice();
}

function CarregarAnexosAdiantamentoMotorista(data) {
    _anexosAdiantamentoMotorista.AnexosAdiantamentoMotorista.val(data.AnexosAdiantamentoMotorista);
}

function RenderizarGridAnexosAdiantamentoMotorista() {
    // Busca a lista
    var anexosAdiantamentoMotorista = GetAnexosAdiantamentoMotorista();

    // E chama o metodo da grid
    _gridAnexosAdiantamentoMotorista.CarregarGrid(anexosAdiantamentoMotorista);
}

function EnviarArquivosAnexadosAdiantamentoMotorista(cb) {
    // Busca a lista
    var anexosAdiantamentoMotorista = GetAnexosAdiantamentoMotorista();

    if (anexosAdiantamentoMotorista.length > 0) {
        var dados = {
            Chamado: _chamadoTMS.Codigo.val()
        }
        CriaEEnviaFormDataAdiantamentoMotorista(anexosAdiantamentoMotorista, dados, cb);
    } else if (cb != null) {
        cb();
    }
}

function RemoverAnexoAdiantamentoMotorista(dataRow, listaAnexosAdiantamentoMotorista, ko, url) {
    // Funcao auxiliar
    var RemoveDaGrid = function () {
        listaAnexosAdiantamentoMotorista.forEach(function (anexo, i) {
            if (dataRow.Codigo == anexo.Codigo) {
                listaAnexosAdiantamentoMotorista.splice(i, 1);
            }
        });

        ko.AnexosAdiantamentoMotorista.val(listaAnexosAdiantamentoMotorista);
    }

    // Se for arquivo local, apenas remove do array
    if (isNaN(dataRow.Codigo)) {
        RemoveDaGrid();
    } else {
        // Permissao
        if (!PodeGerenciarAnexosAdiantamentoMotorista())
            return exibirMensagem(tipoMensagem.atencao, "AnexosAdiantamentoMotorista", "Situação da Ocorrência não permite excluir arquivos.");

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

function CriaEEnviaFormDataAdiantamentoMotorista(anexosAdiantamentoMotorista, dados, cb) {
    // Busca todos file
    var formData = new FormData();
    anexosAdiantamentoMotorista.forEach(function (anexo) {
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        }
    });

    enviarArquivo("ChamadoTMSAnexos/AnexarArquivosAdiantamentoMotorista?callback=?", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _anexosAdiantamentoMotorista.AnexosAdiantamentoMotorista.val(arg.Data.Anexos);
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

function EnviarAnexoAdiantamentoMotorista(anexo) {
    var anexosAdiantamentoMotorista = [anexo];
    var dados = {
        Chamado: _chamadoTMS.Codigo.val()
    }

    CriaEEnviaFormDataAdiantamentoMotorista(anexosAdiantamentoMotorista, dados);
}

function limparOcorrenciaAnexosAdiantamentoMotorista() {
    LimparCampos(_anexosAdiantamentoMotorista);
    _anexosAdiantamentoMotorista.AnexosAdiantamentoMotorista.val(_anexosAdiantamentoMotorista.AnexosAdiantamentoMotorista.def);
    _anexosAdiantamentoMotorista.Adicionar.visible(true);
}

function PodeGerenciarAnexosAdiantamentoMotorista() {
    var situacao = _chamadoTMS.Situacao.val();

    return situacao != EnumSituacaoChamadoTMS.Finalizado && situacao != EnumSituacaoChamadoTMS.LiberadaOcorrencia && situacao != EnumSituacaoChamadoTMS.Cancelado;
}

function AlternaTelaDeAnexosAdiantamentoMotorista(ko) {
    if (PodeGerenciarAnexosAdiantamentoMotorista()) {
        ko.AnexosAdiantamentoMotorista.visible(true);
    } else {
        ko.AnexosAdiantamentoMotorista.visible(false);
    }
}