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

var _gridAnexos;
var _anexos;

var Anexos = function () {
    this.Funcionario = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao, type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Anexo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexos.NomeArquivo.val(nomeArquivo);
    });

    this.Anexos = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Anexos, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(function () {
        RenderizarGridAnexos();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });

    this.config = PropertyEntity({
        msg: {
            situacaonaopermite: Localization.Resources.Pessoas.Usuario.SituacaoNaoPermiteExcluirArquivos
        },
        url: {
            excluir: "FuncionarioAnexo/ExcluirAnexo",
            download: "FuncionarioAnexo/DownloadAnexo",
            anexar: "FuncionarioAnexo/AnexarArquivos?callback=?"
        }
    });
};

// Codigo principal da tela que esta sendo usada
function GetCodigoTela() {
    return _usuario.Codigo.val();
}

// Retorna o nome do parametro da entidade principal
function GetNomeParametro() {
    return "Funcionario";
}

// Verifica se pode ou nao anexar
function PodeGerenciarAnexos() {
    var codigo = _usuario.Codigo.val();
    return codigo > 0;
}

function PodeExcluirAnexos() {
    var permiteRemover = true;
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Usuario_PermiteRemoverAnexos, _PermissoesPersonalizadas))
        permiteRemover = false;

    return permiteRemover;
}

//*******EVENTOS*******

function loadAnexos() {
    CarregarHTMLAnexos();
    _anexos = new Anexos();
    KoBindings(_anexos, "knockoutAnexos");

    //-- Grid Anexos
    // Opcoes
    var download = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadAnexoClick, icone: "", visibilidade: visibleDownload };
    var remover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoClick, icone: "", visibilidade: visibleRemover };

    // Menu
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 9, opcoes: [download, remover] };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "NomeTela", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "25%", className: "text-align-left" }
    ];

    // Grid
    var linhasPorPaginas = 7;
    _gridAnexos = new BasicDataTable(_anexos.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexos.CarregarGrid([]);
}

function CarregarHTMLAnexos() {
    var html = $("#anexo-container script").html();
    $("#knockoutAnexos").html(html);
    LocalizeCurrentPage();
}

function visibleDownload(dataRow) {
    return !isNaN(dataRow.Codigo);
}

function visibleRemover(dataRow) {
    return PodeGerenciarAnexos() && PodeExcluirAnexos();
}

function downloadAnexoClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload(_anexos.config.url.download, data);
}

function removerAnexoClick(dataRow, row) {
    var listaAnexos = GetAnexos();

    // Funcao auxiliar
    var RemoveDaGrid = function () {
        listaAnexos.forEach(function (anexo, i) {
            if (dataRow.Codigo === anexo.Codigo) {
                listaAnexos.splice(i, 1);
            }
        });

        _anexos.Anexos.val(listaAnexos);
    }

    // Se for arquivo local, apenas remove do array
    if (isNaN(dataRow.Codigo)) {
        RemoveDaGrid();
    } else {
        // Permissao
        if (!PodeGerenciarAnexos())
            return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, _anexos.config.msg.situacaonaopermite);

        // Exclui do sistema
        executarReST(_anexos.config.url.excluir, dataRow, function (arg) {
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
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, _anexos.config.msg.situacaonaopermite);

    // Busca o input de arquivos
    var file = document.getElementById(_anexos.Arquivo.id);

    // Valida
    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);

    // Monta objeto anexo
    var anexo = {
        Codigo: guid(),
        NomeTela: "Usuario",
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

//*******MÉTODOS*******
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
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        }
    });

    enviarArquivo(_anexos.config.url.anexar, dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
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