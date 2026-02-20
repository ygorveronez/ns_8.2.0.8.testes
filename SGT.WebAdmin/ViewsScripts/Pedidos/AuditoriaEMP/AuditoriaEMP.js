/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../Enumeradores/EnumSituacaoMDFe.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridAuditoriaEMP;
var _gridAuditoriaEMPRecebido;
var _pesquisaAuditoriaEMPEnvio;
var _pesquisaAuditoriaEMP;
var _pesquisaAuditoriaEMPRecebido;
var _inserirJustificativaRecebimento;
var _inserirJustificativaEnvio;

var AuditoriaEMP = function () {
    var dataAtual = moment().add(-7, 'days').format("DD/MM/YYYY");

    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(dataAtual), def: dataAtual });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.Topic = PropertyEntity({ text: "Topic:", getType: typesKnockout.string, val: ko.observable(""), def: ko.observable("") });
    this.Booking = PropertyEntity({ text: "Booking:", getType: typesKnockout.string, val: ko.observable(""), def: ko.observable("") });
    this.Customer = PropertyEntity({ text: "Customer:", getType: typesKnockout.string, val: ko.observable(""), def: ko.observable("") });
    this.Schedule = PropertyEntity({ text: "Schedule (VVD):", getType: typesKnockout.string, val: ko.observable(""), def: ko.observable("") });
    this.Justificativa = PropertyEntity({ getType: typesKnockout.select, val: ko.observable(EnumSimNaoPesquisa.Todos2), def: EnumSimNaoPesquisa.Todos2, options: EnumSimNaoPesquisa.obterOpcoesPesquisa2(), text: "Justificativa: " });
    this.Status = PropertyEntity({ getType: typesKnockout.select, val: ko.observable(EnumSituacaoIntegracaoEMP.NaoInformado), def: EnumSituacaoIntegracaoEMP.NaoInformado, options: EnumSituacaoIntegracaoEMP.obterOpcoes(), text: "Situação Integração: " });
    this.TipoIntegracao = PropertyEntity({ getType: typesKnockout.select, val: ko.observable(EnumTipoIntegracaoEMP.NaoInformado), def: EnumTipoIntegracaoEMP.NaoInformado, options: EnumTipoIntegracaoEMP.obterOpcoes(), text: "Tipo de Integração: " });
    this.Fatura = PropertyEntity({ text: "Fatura:", getType: typesKnockout.string, val: ko.observable(""), def: ko.observable("") });
    this.Boleto = PropertyEntity({ text: "Boleto:", getType: typesKnockout.string, val: ko.observable(""), def: ko.observable("") });
};

var AuditoriaEMPEnvio = function () {
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAuditoriaEMP.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid()
    });
};

var AuditoriaEMPRecebido = function () {
    this.PesquisarRecebido = PropertyEntity({
        eventClick: function (e) {
            _gridAuditoriaEMPRecebido.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid()
    });
};

var InserirJustificativaRecebimento = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.SituacaoIntegracaoEMPRecebido = PropertyEntity({ getType: typesKnockout.select, val: ko.observable(EnumSituacaoIntegracaoEMP.NotPersist), def: EnumSituacaoIntegracaoEMP.NotPersist, options: EnumSituacaoIntegracaoEMP.obterOpcoes(), text: "Situação Integração EMP: " });
    this.JustificativaRecebido = PropertyEntity({ text: "Justificativa:", maxlength: 2000, visible: ko.observable(true) });
    this.AtualizarJustificativaRecebimento = PropertyEntity({ type: types.event, eventClick: atualizarJustificativaRecebimentoClick, text: "Atualizar Justificativa", visible: ko.observable(true) });
};

var InserirJustificativaEnvio = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.SituacaoIntegracaoEMPEnvio = PropertyEntity({ getType: typesKnockout.select, val: ko.observable(EnumSituacaoIntegracaoEMP.NotPersist), def: EnumSituacaoIntegracaoEMP.NotPersist, options: EnumSituacaoIntegracaoEMP.obterOpcoes(), text: "Situação Integração EMP: " });
    this.JustificativaEnvio = PropertyEntity({ text: "Justificativa:", maxlength: 2000, visible: ko.observable(true) });
    this.AtualizarJustificativaEnvio = PropertyEntity({ type: types.event, eventClick: atualizarJustificativaEnvioClick, text: "Atualizar Justificativa", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadAuditoriaEMP() {
    _pesquisaAuditoriaEMP = new AuditoriaEMP();
    KoBindings(_pesquisaAuditoriaEMP, "knockoutAuditoriaEMP");

    _pesquisaAuditoriaEMPEnvio = new AuditoriaEMPEnvio();
    KoBindings(_pesquisaAuditoriaEMPEnvio, "knockoutAuditoriaEMPEnvio", false, _pesquisaAuditoriaEMPEnvio.Pesquisar.id);

    _pesquisaAuditoriaEMPRecebido = new AuditoriaEMPRecebido();
    KoBindings(_pesquisaAuditoriaEMPRecebido, "knockoutAuditoriaEMPRecebido", false, _pesquisaAuditoriaEMPRecebido.PesquisarRecebido.id);

    _inserirJustificativaRecebimento = new InserirJustificativaRecebimento();
    KoBindings(_inserirJustificativaRecebimento, "knockoutInserirJustificativaRecebimento");

    _inserirJustificativaEnvio = new InserirJustificativaEnvio();
    KoBindings(_inserirJustificativaEnvio, "knockoutInserirJustificativaEnvio");

    BuscarAuditoriaEMP();
}

function ReprocessarRecebimento(e) {
    executarReST("AuditoriaEMP/ReprocessarRecebimento", { Codigo: e.Codigo }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Reprocessado com sucesso.");
                _gridAuditoriaEMPRecebido.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function DownloadRecebimento(e) {
    var data = { Codigo: e.Codigo };
    executarDownload("AuditoriaEMP/DownloadArquivoRecebimento", data);
}

function DownloadEnvio(e) {
    var data = { Codigo: e.Codigo };
    executarDownload("AuditoriaEMP/DownloadArquivoEnvio", data);
}

function DownloadRetorno(e) {
    var data = { Codigo: e.Codigo };
    executarDownload("AuditoriaEMP/DownloadArquivoRetorno", data);
}

function InserirJustificativaRecebimentoOpcao(e) {
    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.AuditoriaEMP_AcessoJustificativa, _PermissoesPersonalizadas)) {
        _inserirJustificativaRecebimento.Codigo.val(e.Codigo);
        Global.abrirModal('divModalInserirJustificativaRecebimento');
    } else
        exibirMensagem(tipoMensagem.atencao, "Sem Permissão", "Seu usuário não possui permissão para inserir Justificativa.");
}

function InserirJustificativaEnvioOpcao(e) {
    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.AuditoriaEMP_AcessoJustificativa, _PermissoesPersonalizadas)) {
        _inserirJustificativaEnvio.Codigo.val(e.Codigo);
        Global.abrirModal('divModalInserirJustificativaEnvio');
    } else
        exibirMensagem(tipoMensagem.atencao, "Sem Permissão", "Seu usuário não possui permissão para inserir Justificativa.");
}

function atualizarJustificativaRecebimentoClick() {
    executarReST("AuditoriaEMP/InserirJustificativaRecebimento", { Codigo: _inserirJustificativaRecebimento.Codigo.val(), SituacaoIntegracaoEMPRecebido: _inserirJustificativaRecebimento.SituacaoIntegracaoEMPRecebido.val(), JustificativaRecebido: _inserirJustificativaRecebimento.JustificativaRecebido.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso.");
                _gridAuditoriaEMPRecebido.CarregarGrid();
                LimparCampos(_inserirJustificativaRecebimento);
                Global.fecharModal('divModalInserirJustificativaRecebimento');
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function atualizarJustificativaEnvioClick(e) {
    executarReST("AuditoriaEMP/InserirJustificativaEnvio", { Codigo: _inserirJustificativaEnvio.Codigo.val(), SituacaoIntegracaoEMPEnvio: _inserirJustificativaEnvio.SituacaoIntegracaoEMPEnvio.val(), JustificativaEnvio: _inserirJustificativaEnvio.JustificativaEnvio.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso.");
                _gridAuditoriaEMP.CarregarGrid();
                LimparCampos(_inserirJustificativaEnvio);
                Global.fecharModal('divModalInserirJustificativaEnvio');
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******

function BuscarAuditoriaEMP() {
    var downloadEnvio = { descricao: "Baixar Envio", id: guid(), evento: "onclick", metodo: DownloadEnvio, tamanho: "20", icone: "fa fa-download", visibilidade: true };
    var downloadRetorno = { descricao: "Baixar Retorno", id: guid(), evento: "onclick", metodo: DownloadRetorno, tamanho: "20", icone: "fa fa-exchange-alt", visibilidade: true };
    var inserirJustificativaEnvio = { descricao: "Inserir Justificativa", id: guid(), evento: "onclick", metodo: InserirJustificativaEnvioOpcao, tamanho: "20", icone: "", visibilidade: VerificaSePossuiPermissaoEspecialVisualizarJustificativa };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [downloadEnvio, downloadRetorno, inserirJustificativaEnvio],
        tamanho: 7
    };

    var configExportacaoEnviados = {
        url: "AuditoriaEMP/ExportarPesquisa",
        titulo: "Consulta de Auditorias EMP Enviadas"
    };

    _gridAuditoriaEMP = new GridViewExportacao(_pesquisaAuditoriaEMPEnvio.Pesquisar.idGrid, "AuditoriaEMP/Pesquisa", _pesquisaAuditoriaEMP, menuOpcoes, configExportacaoEnviados, { column: 4, dir: orderDir.desc }, 10);
    _gridAuditoriaEMP.CarregarGrid();

    var downloadRecebimento = { descricao: "Baixar Recebimento", id: guid(), evento: "onclick", metodo: DownloadRecebimento, tamanho: "20", icone: "fa fa-download", visibilidade: true };
    //var downloadRetorno = { descricao: "Baixar Retorno", id: guid(), evento: "onclick", metodo: DownloadRetorno, tamanho: "20", icone: "fa fa-exchange-alt", visibilidade: true };
    var inserirJustificativaRecebimento = { descricao: "Inserir Justificativa", id: guid(), evento: "onclick", metodo: InserirJustificativaRecebimentoOpcao, tamanho: "20", icone: "", visibilidade: VerificaSePossuiPermissaoEspecialVisualizarJustificativa };
    var reprocessarRecebimento = { descricao: "Reprocessar Recebimento", id: guid(), evento: "onclick", metodo: ReprocessarRecebimento, tamanho: "20", icone: "", visibilidade: false };

    var menuOpcoesRecebimento = {
        tipo: TypeOptionMenu.list,
        opcoes: [downloadRecebimento, inserirJustificativaRecebimento, reprocessarRecebimento],
        tamanho: 7
    };

    var configExportacaoRecebidos = {
        url: "AuditoriaEMP/ExportarPesquisaRecebido",
        titulo: "Consulta de Auditorias EMP Recebidas"
    };

    _gridAuditoriaEMPRecebido = new GridViewExportacao(_pesquisaAuditoriaEMPRecebido.PesquisarRecebido.idGrid, "AuditoriaEMP/PesquisaRecebido", _pesquisaAuditoriaEMP, menuOpcoesRecebimento, configExportacaoRecebidos, { column: 4, dir: orderDir.desc }, 10);
    _gridAuditoriaEMPRecebido.CarregarGrid();
}

function VerificaSePossuiPermissaoEspecialVisualizarJustificativa() {
    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.AuditoriaEMP_PermiteVisualizarJustificativa, _PermissoesPersonalizadas)) {
        return true;
    }
}
