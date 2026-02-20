/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOcorrencia.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="CTeComplementar.js" />
/// <reference path="../../Creditos/ControleSaldo/ControleSaldo.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="EtapasOcorrencia.js" />
/// <reference path="Autorizacao.js" />
/// <reference path="ResumoOcorrencia.js" />
/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../Enumeradores/EnumTipoTomador.js" />
/// <reference path="../../Consultas/CTe.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _anexosOcorrencia;
var _gridAnexosOcorrencia;

var AnexosOcorrencia = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.Anexo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexosOcorrencia.NomeArquivo.val(nomeArquivo);
    });

    this.Anexos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(function () {
        RenderizarGridAnexos();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Adicionar, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadAnexosOcorrencia() {
    _anexosOcorrencia = new AnexosOcorrencia();
    KoBindings(_anexosOcorrencia, "knockoutCadastroAnexos");

    //-- Grid Anexos
    // Opcoes
    var download = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Download, id: guid(), metodo: downloadAnexoOcorrenciaClick, icone: "", visibilidade: visibleDownloadOcorrencia };
    var remover = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Remover, id: guid(), metodo: removerAnexoOcorrenciaClick, icone: "", visibilidade: visibleRemoverOcorrencia };

    // Menu
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Ocorrencias.Ocorrencia.Opcoes, tamanho: 9, opcoes: [download, remover] };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Ocorrencias.Ocorrencia.Descricao, width: "70%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Ocorrencias.Ocorrencia.Nome, width: "25%", className: "text-align-left" }
    ];

    // Grid
    var linhasPorPaginas = 7;
    _gridAnexosOcorrencia = new BasicDataTable(_anexosOcorrencia.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosOcorrencia.CarregarGrid([]);

    //-- Controle de anexos
    _ocorrencia.SituacaoOcorrencia.val.subscribe(function () {
        AlternaTelaDeAnexos(_anexosOcorrencia);
    });
}

function visibleDownloadOcorrencia(dataRow) {
    return !isNaN(dataRow.Codigo);
}

function visibleRemoverOcorrencia(dataRow) {
    return PodeGerenciarAnexos() && !_FormularioSomenteLeitura;
}

function downloadAnexoOcorrenciaClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("OcorrenciaAnexos/DownloadAnexo", data);
}

function removerAnexoOcorrenciaClick(dataRow) {
    var listaAnexos = GetAnexos();
    RemoverAnexoOcorrencia(dataRow, listaAnexos, _anexosOcorrencia, "OcorrenciaAnexos/ExcluirAnexao");
}

function gerenciarAnexosClick() {
    Global.abrirModal('divModalGerenciarAnexos');
}

function adicionarAnexoOcorrenciaClick() {
    // Permissao
    if (!PodeGerenciarAnexos())
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.Ocorrencia.Anexos, Localization.Resources.Ocorrencias.Ocorrencia.SituacaoOcorrenciaNaoPermiteAnexarAquivos);

    // Busca o input de arquivos
    var file = document.getElementById(_anexosOcorrencia.Arquivo.id);

    // Valida
    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.Ocorrencia.Anexos, Localization.Resources.Ocorrencias.Ocorrencia.NenhumArquivoSelecionado);

    // Monta objeto anexo
    var anexo = {
        Codigo: guid(),
        Descricao: _anexosOcorrencia.Descricao.val(),
        NomeArquivo: _anexosOcorrencia.NomeArquivo.val(),
        Arquivo: file.files[0]
    };

    // Se ja esta cadastrada a ocorrencia, envia o anexo direto
    if (_ocorrencia.Codigo.val() > 0) {
        EnviarAnexo(anexo);
    } else {
        // Clona a lista e adiciona o form data
        var listaAnexos = GetAnexos();
        listaAnexos.push(anexo);
        _anexosOcorrencia.Anexos.val(listaAnexos.slice());
    }

    // Limpa os campos
    LimparCampos(_anexosOcorrencia);
    _anexosOcorrencia.Arquivo.val('');

    file.value = null;
}

//*******MÉTODOS*******

function GetAnexos() {
    // Retorna um clone do array para não prender a referencia
    return _anexosOcorrencia.Anexos.val().slice();
}

function RenderizarGridAnexos() {
    // Busca a lista
    var anexos = GetAnexos();

    // E chama o metodo da grid
    _gridAnexosOcorrencia.CarregarGrid(anexos);
}

function EnviarArquivosAnexadosOcorrencia(hashAnexos) {
    // Busca a lista
    var anexos = GetAnexos();

    if (anexos.length > 0) {
        var dados = {
            CodigoOcorrencia: _ocorrencia.Codigo.val(),
            CargaOcorrenciaVinculada: _ocorrencia.CargaOcorrenciaVinculada.val(),
            HashAnexos: hashAnexos
        }
        CriaEEnviaFormData(anexos, dados);
    }
}

function RemoverAnexoOcorrencia(dataRow, listaAnexos, knout, url) {
    // Funcao auxiliar
    var RemoveDaGrid = function () {
        listaAnexos.forEach(function (anexo, i) {
            if (dataRow.Codigo == anexo.Codigo) {
                listaAnexos.splice(i, 1);
            }
        });

        knout.Anexos.val(listaAnexos);
    }

    // Se for arquivo local, apenas remove do array
    if (isNaN(dataRow.Codigo)) {
        RemoveDaGrid();
    } else {
        // Permissao
        if (!PodeGerenciarAnexos())
            return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.Ocorrencia.Anexos, Localization.Resources.Ocorrencias.Ocorrencia.SituacaoOcorrenciaNaoPermiteExcluirArquivos);

        // Exclui do sistema
        executarReST(url, dataRow, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.Ocorrencia.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.ExcluidoComSucesso);
                    RemoveDaGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Ocorrencias.Ocorrencia.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
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

    enviarArquivo("OcorrenciaAnexos/AnexarArquivos?callback=?", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _anexosOcorrencia.Anexos.val(arg.Data.Anexos);
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.Ocorrencia.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.ArquivoAnexadoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Ocorrencias.Ocorrencia.NaoFoiPossivelAnexarArquivo, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            limparCamposOcorrencia();
            _gridViewOcorrencia.CarregarGrid();
        }
    });
}

function EnviarAnexo(anexo) {
    var anexos = [anexo];
    var dados = {
        CodigoOcorrencia: _ocorrencia.Codigo.val(),
        CargaOcorrenciaVinculada: _ocorrencia.CargaOcorrenciaVinculada.val()
    }

    CriaEEnviaFormData(anexos, dados);
}

function limparOcorrenciaAnexos() {
    LimparCampos(_anexosOcorrencia);
    _anexosOcorrencia.Anexos.val(_anexosOcorrencia.Anexos.def);
}

function PodeGerenciarAnexos() {
    var situacao = _ocorrencia.SituacaoOcorrencia.val();

    return EnumSituacaoOcorrencia.AgAprovacao == situacao || EnumSituacaoOcorrencia.Todas == situacao || EnumSituacaoOcorrencia.AgInformacoes == situacao || EnumSituacaoOcorrencia.Finalizada == situacao;
}

function AlternaTelaDeAnexos(ko) {
    if (PodeGerenciarAnexos()) {
        ko.Anexos.visible(true);
    } else {
        ko.Anexos.visible(false);
    }
}