/// <reference path="DetalheAlteracao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCargaEDI.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridControleGeracaoEDI;
var _controleGeracaoEDI;
var _CRUDConfiguracaoControleGeracaoEDI;

/*
 * Declaração das Classes
 */

var ControleGeracaoEDI = function () {
    var dataAtual = moment().add(-1, 'days').format("DD/MM/YYYY");
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Cargas.ControleGeracaoEDI.DataInicial.getFieldDescription(), val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, required: ko.observable(true), visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Cargas.ControleGeracaoEDI.DataFinal.getFieldDescription(), getType: typesKnockout.date, required: ko.observable(true) });
    this.HoraInicial = PropertyEntity({ text: Localization.Resources.Cargas.ControleGeracaoEDI.HoraInicial.getFieldDescription(), getType: typesKnockout.time });
    this.HoraFinal = PropertyEntity({ text: Localization.Resources.Cargas.ControleGeracaoEDI.HoraFinal.getFieldDescription(), getType: typesKnockout.time });
    this.IDOC = PropertyEntity({ text: Localization.Resources.Cargas.ControleGeracaoEDI.IDOC.getFieldDescription(), maxlength: 100 });
    this.NomeArquivo = PropertyEntity({ text: Localization.Resources.Cargas.ControleGeracaoEDI.NomeArquivo.getFieldDescription(), maxlength: 100 });
    this.NumeroDT = PropertyEntity({ text: Localization.Resources.Cargas.ControleGeracaoEDI.Viagem.getFieldDescription(), maxlength: 50 });
    this.Placa = PropertyEntity({ text: Localization.Resources.Cargas.ControleGeracaoEDI.Placa.getFieldDescription(), maxlength: 10 });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoIntegracaoCargaEDI.Todos), options: EnumSituacaoIntegracaoCargaEDI.obterOpcoes(), def: EnumSituacaoIntegracaoCargaEDI.Todos, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleGeracaoEDI.Transportador.getFieldDescription(), idBtnSearch: guid() });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridControleGeracaoEDI.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.Upload = PropertyEntity({ type: types.event, eventChange: UploadChange, idFile: guid(), accept: ".txt, .xml", text: Localization.Resources.Cargas.ControleGeracaoEDI.EnviarArquivoEDI, icon: "fal fa-file" });

    this.Configuracao = PropertyEntity({ eventClick: exibirModalCRUDConfiguracaoControleGeracaoEDI, type: types.event, text: Localization.Resources.Cargas.ControleGeracaoEDI.Configuracao, visible: ko.observable(false) });
    this.ReenviarComFalhas = PropertyEntity({ eventClick: reenviarComFalhasClick, type: types.event, text: Localization.Resources.Cargas.ControleGeracaoEDI.ReenviarComFalhas, visible: ko.observable(_CONFIGURACAO_TMS.ExibirOpcaoReenviarNotfisComFalhas) });
};

var CRUDConfiguracaoControleGeracaoEDI = function () {
    this.ProcessarPrioritario = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Cargas.ControleGeracaoEDI.ProcessarSomentePrioritarios, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.Confirmar = PropertyEntity({ eventClick: atualizarProcessarPrioritarioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: ocultarConfiguracaoControleGeracaoEDIClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
};


/*
 * Declaração das Funções de Inicialização
 */

function LoadControleGeracaoEDI() {
    _controleGeracaoEDI = new ControleGeracaoEDI();
    KoBindings(_controleGeracaoEDI, "knockoutControleGeracaoEDI", false, _controleGeracaoEDI.Pesquisar.id);

    _CRUDConfiguracaoControleGeracaoEDI = new CRUDConfiguracaoControleGeracaoEDI();
    KoBindings(_CRUDConfiguracaoControleGeracaoEDI, "knockoutCRUDConfiguracaoControleGeracaoEDI");

    HeaderAuditoria("ConfiguracaoControleIntegracaoCargaEDI", _CRUDConfiguracaoControleGeracaoEDI);

    new BuscarTransportadores(_controleGeracaoEDI.Transportador);

    loadDetalheAlteracao();
    loadGridControleGeracaoEDI();

    _controleGeracaoEDI.Upload.file = document.getElementById(_controleGeracaoEDI.Upload.idFile);
}

function loadGridControleGeracaoEDI() {
    var configuracaoExportacao = { url: "ControleGeracaoEDI/ExportarPesquisa", titulo: Localization.Resources.Cargas.ControleGeracaoEDI.ControleDeGeracaoDeCargasPorEDI };
    var ordenacaoPadrao = null;
    var quantidadePorPagina = 10;
    var opcaoDetalhes = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), evento: "onclick", metodo: detalhesControleGeracaoEDIClick, tamanho: "20", icone: "" };
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), evento: "onclick", metodo: baixarNotfisClick, tamanho: "20", icone: "" };
    var opcaoReenviar = { descricao: Localization.Resources.Cargas.ControleGeracaoEDI.Reenviar, id: guid(), evento: "onclick", metodo: ReenviarControleGeracaoEDIClick, tamanho: "20", icone: "" };
    var opcaoPrioridade = { descricao: Localization.Resources.Cargas.ControleGeracaoEDI.AlterarPrioridade, id: guid(), evento: "onclick", metodo: alterarPrioridade, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoDetalhes, opcaoDownload, opcaoReenviar, opcaoPrioridade], tamanho: "5" };

    _gridControleGeracaoEDI = new GridViewExportacao(_controleGeracaoEDI.Pesquisar.idGrid, "ControleGeracaoEDI/Pesquisa", _controleGeracaoEDI, menuOpcoes, configuracaoExportacao, ordenacaoPadrao, quantidadePorPagina);
    _gridControleGeracaoEDI.SetPermitirRedimencionarColunas(true);
    _gridControleGeracaoEDI.CarregarGrid();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function detalhesControleGeracaoEDIClick(registroSelecionado) {
    executarReST("ControleGeracaoEDI/PesquisaDetalheAlteracao", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                preencherDetalheAlteracao(retorno.Data);
                exibirModalDetalhesControleGeracaoEDI();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function alterarPrioridade(registroSelecionado, linhaSelecionada) {
    var novaPrioridade = !registroSelecionado.Prioritario
    executarReST("ControleGeracaoEDI/AlterarPrioridade", { Codigo: registroSelecionado.Codigo, Prioritario: novaPrioridade }, function (retorno) {
        if (retorno.Success) {
            registroSelecionado.Prioritario = novaPrioridade;
            registroSelecionado.DT_RowColor = "";
            if (registroSelecionado.Prioritario)
                registroSelecionado.DT_RowColor = "#ADD8E6";

            _gridControleGeracaoEDI.AtualizarDataRow(linhaSelecionada, registroSelecionado);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}


function baixarNotfisClick(registroSelecionado) {
    executarDownload("ControleGeracaoEDI/DownloadArquivoEDI", { Codigo: registroSelecionado.Codigo });
}

function ReenviarControleGeracaoEDIClick(registroSelecionado) {
    executarReST("ControleGeracaoEDI/Reenviar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleGeracaoEDI.ReenvioRealizadoComSucesso);
                _gridControleGeracaoEDI.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Geris.Geral.Atencao, retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function UploadChange() {
    if (_controleGeracaoEDI.Upload.file.files.length > 0) {
        var data = new FormData();

        for (var i = 0, s = _controleGeracaoEDI.Upload.file.files.length; i < s; i++) {
            data.append("Arquivo", _controleGeracaoEDI.Upload.file.files[i]);
        }

        enviarArquivo("ControleGeracaoEDI/EnviarArquivo", {}, data, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    if (retorno.Data.Adicionados > 0) {
                        if (retorno.Data.Adicionados > 1)
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleGeracaoEDI.ArquivosEnviados.format(retorno.Data.Adicionados));
                        else
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleGeracaoEDI.ArquivoEnviado.format(retorno.Data.Adicionados));
                    }

                    if (retorno.Data.Erros.length > 0)
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Cargas.ControleGeracaoEDI.OcorreuErro.format(retorno.Data.Erros));
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
            $("#" + _controleGeracaoEDI.Upload.idFile).val("");
        });
    }
}

function reenviarComFalhasClick() {
    Salvar(_controleGeracaoEDI, "ControleGeracaoEDI/ReenviarEmLote", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.ControleGeracaoEDI.Sucesso, Localization.Resources.Cargas.ControleGeracaoEDI.ReenvioEmLoteRealizadoComSucesso);
                _gridControleGeracaoEDI.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

/*
 * Declaração das Funções
 */

function exibirModalDetalhesControleGeracaoEDI() {
    Global.abrirModal('divModalDetalhesControleGeracaoEDI');
    $("#divModalDetalhesControleGeracaoEDI").one('hidden.bs.modal', function () {
        limparDetalheAlteracao();
    });
}

function atualizarProcessarPrioritarioClick() {
    executarReST("ControleGeracaoEDI/ProcessarPrioritario", { ProcessarPrioritario: _CRUDConfiguracaoControleGeracaoEDI.ProcessarPrioritario.val() }, function (retorno) {
        if (retorno.Success) {
            ocultarConfiguracaoControleGeracaoEDIClick();
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function obterConfiguracaoPrioridade() {
    executarReST("ControleGeracaoEDI/ObterConfiguracaoSomentePrioritario", {}, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _CRUDConfiguracaoControleGeracaoEDI.ProcessarPrioritario.val(retorno.Data.ProcessarSomentePrioritario);
            }

        }
    });
}

function exibirModalCRUDConfiguracaoControleGeracaoEDI() {
    obterConfiguracaoPrioridade();
    Global.abrirModal('divModalConfiguracaoControleGeracaoEDI');
    $("#divModalConfiguracaoControleGeracaoEDI").one('hidden.bs.modal', function () {

    });
}

function ocultarConfiguracaoControleGeracaoEDIClick() {
    Global.fecharModal('divModalConfiguracaoControleGeracaoEDI');
}