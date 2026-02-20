/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoChamado.js" />
/// <reference path="Chamado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _anexosChamados;
var _gridAnexosChamados;
var _chamadoOcorrenciaModalGerenciarAnexosChamados;

var AnexosChamados = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Chamado.ChamadoOcorrencia.Anexo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexosChamados.NomeArquivo.val(nomeArquivo);
    });

    this.Anexos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(function () {
        RenderizarGridAnexosChamados();
    });
    this.QuantidadeImagensEsperada = PropertyEntity({ val: ko.observable(0), def: 0, text: ko.observable(""), getType: typesKnockout.int, visible: ko.observable(false) });
    this.NotaFiscalServico = PropertyEntity({ val: ko.observable(false), def: false, text: ko.observable(Localization.Resources.Chamado.ChamadoOcorrencia.EUmaNFSe), getType: typesKnockout.bool, visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoChamadoClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Adicionar, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadAnexosChamados() {
    _anexosChamados = new AnexosChamados();
    KoBindings(_anexosChamados, "knockoutCadastroAnexosChamado");

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
        _anexosChamados.NotaFiscalServico.visible(true);

    var download = { descricao: Localization.Resources.Chamado.ChamadoOcorrencia.Download, id: guid(), metodo: downloadAnexoChamadoClick, icone: "", visibilidade: visibleDownloadChamado };
    var remover = { descricao: Localization.Resources.Chamado.ChamadoOcorrencia.Remover, id: guid(), metodo: removerAnexoChamadoClick, icone: "", visibilidade: visibleRemoverChamado };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Chamado.ChamadoOcorrencia.Opcoes, tamanho: 9, opcoes: [download, remover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Chamado.ChamadoOcorrencia.Descricao, width: "60%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Chamado.ChamadoOcorrencia.Nome, width: "25%", className: "text-align-left" },
        { data: "NotaFiscalServico", title: "NFS-e?", width: "10%", className: "text-align-left", visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS }
    ];

    var linhasPorPaginas = 7;
    _gridAnexosChamados = new BasicDataTable(_anexosChamados.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosChamados.CarregarGrid([]);

    _chamado.Situacao.val.subscribe(function () {
        AlternaTelaDeAnexosChamados(_anexosChamados);
    });

    _chamadoOcorrenciaModalGerenciarAnexosChamados = new bootstrap.Modal(document.getElementById("divModalGerenciarAnexosChamados"), { backdrop: 'static' });
}

function visibleDownloadChamado(dataRow) {
    return !isNaN(dataRow.Codigo);
}

function visibleRemoverChamado(dataRow) {
    return PodeGerenciarAnexosChamados();
}

function downloadAnexoChamadoClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("ChamadoAnexos/DownloadAnexo", data);
}

function removerAnexoChamadoClick(dataRow) {
    var listaAnexosChamados = GetAnexosChamados();
    RemoverAnexoChamado(dataRow, listaAnexosChamados, _anexosChamados, "ChamadoAnexos/ExcluirAnexo");
}

function gerenciarAnexosChamadosClick() {
    LimparCamposAnexosChamado();
    _chamadoOcorrenciaModalGerenciarAnexosChamados.show();
}

function adicionarAnexoChamadoClick() {
    if (!PodeGerenciarAnexosChamados())
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Chamado.ChamadoOcorrencia.Atencao, Localization.Resources.Chamado.ChamadoOcorrencia.SituacaoOcorrenciaNaoPermiteAnexarAquivos);

    var file = document.getElementById(_anexosChamados.Arquivo.id);

    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Chamado.ChamadoOcorrencia.Atencao, Localization.Resources.Chamado.ChamadoOcorrencia.NenhumArquivoSelecionado);

    var anexo = {
        Codigo: guid(),
        Descricao: _anexosChamados.Descricao.val(),
        NomeArquivo: _anexosChamados.NomeArquivo.val(),
        Arquivo: file.files[0],
        NotaFiscalServico: _anexosChamados.NotaFiscalServico.val()
    };

    if (_chamado.Codigo.val() > 0) {
        EnviarAnexoChamado(anexo);
    } else {
        var listaAnexosChamados = GetAnexosChamados();
        listaAnexosChamados.push(anexo);
        _anexosChamados.Anexos.val(listaAnexosChamados.slice());
    }

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
    _anexosChamados.QuantidadeImagensEsperada.val(data.QuantidadeImagensEsperada);

    var diferencaAnexos = _anexosChamados.QuantidadeImagensEsperada.val() - data.Anexos.length;
    if (diferencaAnexos > 0) {
        _anexosChamados.QuantidadeImagensEsperada.text(Localization.Resources.Chamado.ChamadoOcorrencia.AindaFaltamSerSincronizadosAnexosPorFavorAguarde.format(diferencaAnexos));
        _anexosChamados.QuantidadeImagensEsperada.visible(true);
    } else {
        _anexosChamados.QuantidadeImagensEsperada.text("");
        _anexosChamados.QuantidadeImagensEsperada.visible(false);
    }
}

function RenderizarGridAnexosChamados() {
    var anexosChamados = GetAnexosChamados();

    var lista = new Array();
    anexosChamados.forEach(function (anexo, i) {
        var data = new Object();

        data.Codigo = anexo.Codigo;
        data.Descricao = anexo.Descricao;
        data.NomeArquivo = anexo.NomeArquivo;
        data.NotaFiscalServico = anexo.NotaFiscalServico ? Localization.Resources.Chamado.ChamadoOcorrencia.Sim : Localization.Resources.Chamado.ChamadoOcorrencia.Nao;

        lista.push(data);
    });

    _gridAnexosChamados.CarregarGrid(lista);
}

function EnviarArquivosAnexadosChamado(cb) {
    // Busca a lista
    var anexosChamados = GetAnexosChamados();

    if (anexosChamados.length > 0) {
        var dados = {
            Chamado: _chamado.Codigo.val()
        }
        CriaEEnviaFormDataChamado(anexosChamados, dados, cb);
    } else if (cb != null) {
        cb();
    }
}

function RemoverAnexoChamado(dataRow, listaAnexosChamados, ko, url) {
    exibirConfirmacao(Localization.Resources.Chamado.ChamadoOcorrencia.Atencao, Localization.Resources.Chamado.ChamadoOcorrencia.DesejaRealemtenRemoverAnexoAtendimento , function () {
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
            if (!PodeGerenciarAnexosChamados())
                return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Chamado.ChamadoOcorrencia.Atencao, Localization.Resources.Chamado.ChamadoOcorrencia.SituacaoOcorrenciaNaoPermiteExcluirAqruivos );

            executarReST(url, dataRow, function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Chamado.ChamadoOcorrencia.ExcluidoComSucesso);
                        RemoveDaGrid();
                    } else {
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Chamado.ChamadoOcorrencia.Sugestao , arg.Msg, 16000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });
        }
    });
}

function CriaEEnviaFormDataChamado(anexosChamados, dados, cb) {
    var formData = new FormData();
    anexosChamados.forEach(function (anexo) {
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
            formData.append("NotaFiscalServico", anexo.NotaFiscalServico);
        }
    });

    enviarArquivo("ChamadoAnexos/AnexarArquivos?callback=?", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _anexosChamados.Anexos.val(arg.Data.Anexos);
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Chamado.ChamadoOcorrencia.ArquivoAnexadoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Chamado.ChamadoOcorrencia.NaoFoiPossivelAnexarArquivo , arg.Msg);
            }
            if (cb)
                cb();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function EnviarAnexoChamado(anexo) {
    var anexosChamados = [anexo];
    var dados = {
        Chamado: _chamado.Codigo.val()
    }

    CriaEEnviaFormDataChamado(anexosChamados, dados);
}

function limparOcorrenciaAnexosChamados() {
    LimparCampos(_anexosChamados);
    _anexosChamados.Anexos.val(_anexosChamados.Anexos.def);
    _anexosChamados.Adicionar.visible(true);
}

function PodeGerenciarAnexosChamados() {
    var situacao = _chamado.Situacao.val();

    return situacao != EnumSituacaoChamado.Cancelada;
}

function AlternaTelaDeAnexosChamados(ko) {
    if (PodeGerenciarAnexosChamados()) {
        ko.Anexos.visible(true);
    } else {
        ko.Anexos.visible(false);
    }
}

function isPossuiAnexoChamado() {
    return GetAnexosChamados().length > 0;
}