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

var _gridAnexos;
var _anexos;

var Anexos = function () {
    this.Pet = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: ko.observable(Localization.Resources.Gerais.Geral.Descricao.getFieldDescription()), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Gerais.Geral.Anexo.getFieldDescription()), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        RenderizarGridAnexos();
    });

    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexos.NomeArquivo.val(nomeArquivo);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: ko.observable(true) });

    this.config = PropertyEntity({
        msg: {
            situacaonaopermite: "Situação não permite excluir arquivos."
        },
        url: {
            excluir: "Pet/ExcluirAnexo",
            download: "Pet/DownloadAnexo",
            anexar: "Pet/AnexarArquivos?callback=?"
        }
    });
};

// Codigo principal da tela que esta sendo usada
function GetCodigoTela() {
    return _formulario.Codigo.val();
}

// Retorna o nome do parametro da entidade principal
function GetNomeParametro() {
    return "Pet";
}

// Verifica se pode ou nao anexar
function PodeGerenciarAnexos() {
    var codigo = _formulario.Codigo.val();

    return codigo > 0;
}

function PodeExcluirAnexos() {
    return true;
}

//*******EVENTOS*******

function loadAnexos() {
    CarregarHTMLAnexos();
    _anexos = new Anexos();
    KoBindings(_anexos, "knockoutAnexos");

    //-- Grid Anexos
    // Opcoes
    var download = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadAnexoClick, icone: "", visibilidade: possuiCodigoPet };
    var remover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoClick, icone: "", visibilidade: possuiGuid };

    // Menu
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 9, opcoes: [download, remover] };

    // Cabecalho
    var header = [
        { data: "Codigo", title: "Codigo", width: "30%", visible: false },
        { data: "Guid", title: "Guid", width: "40%", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "30%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "25%", className: "text-align-left" },
    ];

    // Grid
    var linhasPorPaginas = 7;
    _gridAnexos = new BasicDataTable(_anexos.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexos.CarregarGrid([]);
}

function CarregarHTMLAnexos() {
    var html = $("#anexo-container script").html();
    $("#knockoutAnexos").html(html);
}

function possuiCodigoPet(dataRow) {
    return GetCodigoTela() > 0
}

function possuiGuid(dataRow) {
    console.log('dataRow.Guid', dataRow.Guid)
    console.log('possui', typeof dataRow.Guid === 'string' && dataRow.Guid.trim() != '')
    return typeof dataRow.Guid === 'string' && dataRow.Guid.trim() != '';
}

function downloadAnexoClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload(_anexos.config.url.download, data);
}

function removerAnexoClick(dataRow) {
    var listaAnexos = GetAnexos();

    // Funcao auxiliar
    var RemoveDaGrid = function () {
        listaAnexos.forEach(function (anexo, i) {
            if (dataRow.Guid === anexo.Guid) {
                listaAnexos.splice(i, 1);
            }
        });

        _anexos.Anexos.val(listaAnexos);
    }

    if (!isNaN(dataRow.Codigo) && dataRow.Codigo == 0) {
        console.log('Removendo da grid...')
        RemoveDaGrid();
    }
    else {
        console.log('Removendo do bd...')
        executarReST(_anexos.config.url.excluir, dataRow, function (arg) {
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

function adicionarAnexoClick() {

    // Busca o input de arquivos
    var file = document.getElementById(_anexos.Arquivo.id);

    // Valida
    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexo, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);

    // Monta objeto anexo
    var anexo = {
        Codigo: 0,
        Guid: guid(),
        NomeTela: "Pet",
        Descricao: _anexos.Descricao.val(),
        NomeArquivo: _anexos.NomeArquivo.val(),
        Arquivo: file.files[0]
    };

    // Se ja esta cadastrada a ocorrencia, envia o anexo direto
    if (GetCodigoTela() > 0) {
        EnviarAnexo(anexo);
    } else {
        // Clona a lista e adiciona o form data
        var listaAnexos = GetAnexos();
        listaAnexos.push(anexo);
        _anexos.Anexos.val(listaAnexos.slice());
    }

    // Limpa os campos
    LimparCampos(_anexos);
    file.value = null;
    _anexos.Arquivo.val("");
}

//#region *******MÉTODOS*******

function GetAnexos() {
    // Retorna um clone do array para não prender a referencia
    return _anexos.Anexos.val().slice();
}

function RenderizarGridAnexos() {
    // Busca a lista
    var anexos = GetAnexos();

    // E chama o metodo da grid
    _gridAnexos.CarregarGrid(anexos);
}

function EnviarArquivosAnexados() {
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
        if (anexo.Codigo == 0) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Guid", anexo.Guid);
            formData.append("Descricao", anexo.Descricao);
        }
    });

    enviarArquivo(_anexos.config.url.anexar, dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _anexos.Anexos.val(arg.Data.Anexos);
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ArquivoAnexadoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function EditarListarAnexos(arg) {
    _anexos.Anexos.val(arg.Data.Anexos);
    $("#liTabAnexos").show();
}

function EnviarAnexo(anexo) {
    var anexos = [anexo];
    var dados = {};
    dados[GetNomeParametro()] = GetCodigoTela();

    CriaEEnviaFormData(anexos, dados);
}

function limparAnexosTela() {
    LimparCampos(_anexos);
    _anexos.Anexos.val(_anexos.Anexos.def);
}

function AlternaTelaDeAnexos() {
    // Chamar esse metodo quando deseja verificar visibilidade de anexos
    if (PodeGerenciarAnexos()) {
        _anexos.Anexos.visible(true);
    } else {
        _anexos.Anexos.visible(false);
    }
}
//#endregion