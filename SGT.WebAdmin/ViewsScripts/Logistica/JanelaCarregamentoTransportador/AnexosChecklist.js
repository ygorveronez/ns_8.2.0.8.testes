/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="Checklist.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _anexosChecklist;
var _gridAnexosChecklist;
var _checklistCargaAtual;

var AnexosChecklist = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Arquivo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        const nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexosChecklist.NomeArquivo.val(nomeArquivo);
    });

    this.Anexos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(function () {
        RecarregarGridAnexos();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoChecklistClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadAnexosChecklist(knoutChecklistCargaAtual) {
    _checklistCargaAtual = knoutChecklistCargaAtual;

    _anexosChecklist = new AnexosChecklist();
    KoBindings(_anexosChecklist, "knockoutCadastroAnexosChecklist");

    const download = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadAnexoChecklistClick, icone: "", visibilidade: visibleDownloadChecklist };
    const remover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoChecklistClick, icone: "", visibilidade: visibleRemoverChecklist };

    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 9, opcoes: [download, remover] };

    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "70%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "25%", className: "text-align-left" }
    ];

    const linhasPorPaginas = 7;
    _gridAnexosChecklist = new BasicDataTable(_anexosChecklist.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosChecklist.CarregarGrid([]);

    ObterAnexos(_checklistCargaAtual.CodigoChecklist.val());
}

function visibleDownloadChecklist(dataRow) {
    return !isNaN(dataRow.Codigo);
}

function visibleRemoverChecklist(dataRow) {
    return PodeGerenciarAnexos() && !_FormularioSomenteLeitura;
}

function downloadAnexoChecklistClick(dataRow) {
    const data = { Codigo: dataRow.Codigo };
    executarDownload("CargaJanelaCarregamentoTransportadorChecklistAnexo/DownloadAnexo", data);
}

function removerAnexoChecklistClick(dataRow) {
    const listaAnexos = GetAnexos();
    RemoverAnexoChecklist(dataRow, listaAnexos, _anexosChecklist);
}

function adicionarAnexoChecklistClick() {
    // Permissao
    if (!PodeGerenciarAnexos())
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoEPossivelEditarOChecklist);

    // Busca o input de arquivos
    const file = document.getElementById(_anexosChecklist.Arquivo.id);

    // Valida
    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);

    // Monta objeto anexo
    const anexo = {
        Codigo: guid(),
        Descricao: _anexosChecklist.Descricao.val(),
        NomeArquivo: _anexosChecklist.NomeArquivo.val(),
        Arquivo: file.files[0]
    };

    // Se ja esta cadastrada a Checklist, envia o anexo direto
    if (_checklistCargaAtual.CodigoChecklist.val() > 0) {
        EnviarAnexo(anexo);
    } else {
        // Clona a lista e adiciona o form data
        const listaAnexos = GetAnexos();
        listaAnexos.push(anexo);
        _anexosChecklist.Anexos.val(listaAnexos.slice());
    }

    _checklistCargaAtual.Anexos.val(_anexosChecklist.Anexos.val());

    LimparCampos(_anexosChecklist);
    _anexosChecklist.Arquivo.val('');

    file.value = null;
}

//*******MÉTODOS*******

function GetAnexos() {
    // Retorna um clone do array para não prender a referencia
    return _anexosChecklist.Anexos.val().slice();
}

function RecarregarGridAnexos() {
    const anexos = GetAnexos();

    _checklistCargaAtual.Anexos.val(anexos);
    _gridAnexosChecklist.CarregarGrid(anexos);
}

function EnviarArquivosAnexadosChecklist() {
    const anexos = GetAnexos();

    if (anexos.length > 0) {
        const dados = {
            Codigo: _checklistCargaAtual.CodigoVeiculo.val(),
        }
        CriaEEnviaFormData(anexos, dados);
    }
}

function RemoverAnexoChecklist(dataRow, listaAnexos, knout) {
    const RemoverItem = function () {
        listaAnexos.forEach(function (anexo, i) {
            if (dataRow.Codigo == anexo.Codigo) {
                listaAnexos.splice(i, 1);
            }
        });

        knout.Anexos.val(listaAnexos);
    }

    // Se for arquivo local, apenas remove do array
    if (isNaN(dataRow.Codigo)) {
        return RemoverItem();
    }

    if (!PodeGerenciarAnexos())
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoEPossivelEditarOChecklist);

    executarReST("CargaJanelaCarregamentoTransportadorChecklistAnexo/ExcluirAnexo", dataRow, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AnexoExcluidoComSucesso);
                RemoverItem();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function CriaEEnviaFormData(anexos, dados) {
    if (anexos.length === 0)
        return;

    const formData = new FormData();
    anexos.forEach(function (anexo) {
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        }
    });

    enviarArquivo("CargaJanelaCarregamentoTransportadorChecklistAnexo/AnexarArquivos?callback=?", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _anexosChecklist.Anexos.val(arg.Data.Anexos);
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ArquivoAnexadoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function EnviarAnexo(anexo) {
    const anexos = [anexo];
    const dados = {
        Codigo: _checklistCargaAtual.CodigoChecklist.val(),
    }

    CriaEEnviaFormData(anexos, dados);
}


function limparChecklistAnexos() {
    LimparCampos(_anexosChecklist);
    _anexosChecklist.Anexos.val(_anexosChecklist.Anexos.def);
}

function PodeGerenciarAnexos() {
    return _cadastroChecklistVeiculo.Salvar.enable();
}

function enviarArquivos(knoutChecklists, data) {
    for (let i = 0; i < knoutChecklists.length; i++) {

        let knoutChecklist = knoutChecklists[i];
        let dataEnviar = data.find(item => item.Item2 === knoutChecklist.OrdemCargaChecklist.val());

        const dados = {
            Codigo: dataEnviar.Item1
        }

        CriaEEnviaFormData(knoutChecklist.Anexos.val(), dados);
    }
}

function ObterAnexos(codigo) {
    if (codigo === 0)
        return;

    executarReST("CargaJanelaCarregamentoTransportadorChecklistAnexo/ObterAnexo", { Codigo: codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _anexosChecklist.Anexos.val(retorno.Data.Anexos);
                RecarregarGridAnexos();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}