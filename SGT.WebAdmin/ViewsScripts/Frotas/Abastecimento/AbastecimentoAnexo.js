/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridAnexosAbastecimento;
var _anexosAbastecimento;

var Anexos = function () {
    this.Abastecimento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Anexo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        RenderizarGridAnexosAbastecimento();
    });

    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexosAbastecimento.NomeArquivo.val(nomeArquivo);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });

    this.config = PropertyEntity({
        msg: {
            situacaonaopermite: "Situação não permite excluir arquivos."
        },
        url: {
            excluir: "AbastecimentoAnexo/ExcluirAnexo",
            download: "AbastecimentoAnexo/DownloadAnexo",
            anexar: "AbastecimentoAnexo/AnexarArquivos?callback=?"
        }
    });
}

// Codigo principal da tela que esta sendo usada
function GetCodigoTela() {
    return _abastecimento.Codigo.val();
}

// Retorna o nome do parametro da entidade principal
function GetNomeParametro() {
    return "Abastecimento";
}

// Verifica se pode ou nao anexar
function PodeGerenciarAnexos() {
    var codigo = _abastecimento.Codigo.val();

    return codigo > 0;
}

function PodeExcluirAnexos() {
    var permiteRemover = true;

    return permiteRemover;
}

//*******EVENTOS*******

function loadAnexosAbastecimento() {
    CarregarHTMLAnexos();
    _anexosAbastecimento = new Anexos();
    KoBindings(_anexosAbastecimento, "knockoutAnexos");

    //-- Grid Anexos
    // Opcoes
    var download = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadAnexoClick, icone: "", visibilidade: visibleDownload };
    var remover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoClick, icone: "", visibilidade: visibleRemover };

    // Menu
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 9, opcoes: [download, remover] };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "25%", className: "text-align-left" },
    ];

    // Grid
    var linhasPorPaginas = 7;
    _gridAnexosAbastecimento = new BasicDataTable(_anexosAbastecimento.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosAbastecimento.CarregarGrid([]);
}

function CarregarHTMLAnexos() {
    var html = $("#anexo-container script").html();
    $("#knockoutAnexos").html(html);
}

function visibleDownload(dataRow) {
    return !isNaN(dataRow.Codigo);
}

function visibleRemover(dataRow) {
    return PodeGerenciarAnexos() && PodeExcluirAnexos();
}

function downloadAnexoClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload(_anexosAbastecimento.config.url.download, data);
}

function removerAnexoClick(dataRow, row) {
    var listaAnexos = GetAnexos();

    // Funcao auxiliar
    var RemoveDaGrid = function () {
        listaAnexos.forEach(function (anexo, i) {
            if (dataRow.Codigo == anexo.Codigo) {
                listaAnexos.splice(i, 1);
            }
        });

        _anexosAbastecimento.Anexos.val(listaAnexos);
    }

    // Se for arquivo local, apenas remove do array
    if (isNaN(dataRow.Codigo)) {
        RemoveDaGrid();
    } else {
        // Permissao
        if (!PodeGerenciarAnexos())
            return exibirMensagem(tipoMensagem.atencao, "Anexos", _anexosAbastecimento.config.msg.situacaonaopermite);

        // Exclui do sistema
        executarReST(_anexosAbastecimento.config.url.excluir, dataRow, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    RemoveDaGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

function adicionarAnexoClick() {
    // Permissao
    if (!PodeGerenciarAnexos())
        return exibirMensagem(tipoMensagem.atencao, "Anexos", _anexosAbastecimento.config.msg.situacaonaopermite);

    // Busca o input de arquivos
    var file = document.getElementById(_anexosAbastecimento.Arquivo.id);

    // Valida
    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum Arquivo Selecionado");

    // Monta objeto anexo
    var anexo = {
        Codigo: guid(),
        Descricao: _anexosAbastecimento.Descricao.val(),
        NomeArquivo: _anexosAbastecimento.NomeArquivo.val(),
        Arquivo: file.files[0]
    };

    // Se ja esta cadastrada a ocorrencia, envia o anexo direto
    if (GetCodigoTela() > 0) {
        EnviarAnexoAbastecimento(anexo);
    } else {
        // Clona a lista e adiciona o form data
        var listaAnexos = GetAnexos();
        listaAnexos.push(anexo);
        _anexosAbastecimento.Anexos.val(listaAnexos.slice());
    }

    // Limpa os campos
    LimparCampos(_anexosAbastecimento);
    file.value = null;
    _anexosAbastecimento.Arquivo.val("");
}




//*******MÉTODOS*******
function GetAnexos() {
    // Retorna um clone do array para não prender a referencia
    return _anexosAbastecimento.Anexos.val().slice();
}

function RenderizarGridAnexosAbastecimento() {
    // Busca a lista
    var anexos = GetAnexos();

    // E chama o metodo da grid
    _gridAnexosAbastecimento.CarregarGrid(anexos);
}

function EnviarArquivosAnexadosAbastecimento() {
    // Busca a lista
    var anexos = GetAnexos();

    if (anexos.length > 0) {
        var dados = {};
        dados[GetNomeParametro()] = GetCodigoTela();

        CriaEEnviaFormData(anexos, dados);
    }
}

function CriaEEnviaFormData(anexos, dados) {
    // Busca todos file
    var formData = new FormData();
    anexos.forEach(function (anexo) {
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
            formData.append("NomeArquivo", anexo.NomeArquivo);
        }
    });

    enviarArquivo(_anexosAbastecimento.config.url.anexar, dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _anexosAbastecimento.Anexos.val(arg.Data.Anexos);
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Arquivo anexado com sucesso");
            } else {
                exibirMensagem(tipoMensagem.falha,"Não foi possível anexar arquivo", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function EditarListarAnexosAbastecimento(arg) {
    _anexosAbastecimento.Anexos.val(arg.Data.Anexos);
}

function EnviarAnexoAbastecimento(anexo) {
    var anexos = [anexo];
    var dados = {};
    dados[GetNomeParametro()] = GetCodigoTela();

    CriaEEnviaFormData(anexos, dados);
}

function limparAnexosAbastecimentoTela() {
    LimparCampos(_anexosAbastecimento);
    _anexosAbastecimento.Anexos.val(_anexosAbastecimento.Anexos.def);
}


function AlternaTelaDeAnexos() {
    // Chamar esse metodo quando deseja verificar visibilidade de anexos
    if (PodeGerenciarAnexos()) {
        _anexosAbastecimento.Anexos.visible(true);
    } else {
        _anexosAbastecimento.Anexos.visible(false);
    }
}