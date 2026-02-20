/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="Constantes.js" />
/// <reference path="Etapa.js" />
/// <reference path="TipoIntegracao.js" />
/// <reference path="Atividades.js" />
/// <reference path="Funcionarios.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _grupoMotoristasIntegracao;
var _historicoGrupoMotoristas;
var _gridGrupoMotoristasIntegracao;
var _gridHistoricoGrupoMotoristas;

var _situacaoGrupoMotoristasIntegracao = [
    { value: "", text: "Todas" },
    { value: EnumSituacaoGrupoMotoristas.AgIntegracao, text: "Aguardando Integração" },
    { value: EnumSituacaoGrupoMotoristas.Integrado, text: "Integrado" },
    { value: EnumSituacaoGrupoMotoristas.ProblemaIntegracao, text: "Falha na Integração" }
];

var GrupoMotoristasIntegracao = function () {
    this.CodigoGrupoMotoristas = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoGrupoMotoristasIntegracao, text: "Situação:", def: "", issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });
    this.TipoConsultaGrupoMotoristas = PropertyEntity({ val: ko.observable("-1"), options: ko.observable([]), text: "Tipo de Integração", def: "-1", enable: ko.observable(true) });
    this.ArquivosGrupoMotoristasIntegracao = PropertyEntity({ type: types.map, required: false, text: "Arquivos Grupo Motorista Integracao", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });


    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridGrupoMotoristasIntegracao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.FinalizarLoteMesmoComFalhaIntegracao = PropertyEntity({
        eventClick: function (e) {
            FinalizarLoteMesmoComFalha();
        }, type: types.event, text: "Finalizar lote mesmo com Falha na Integração", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(false)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosGrupoMotoristas();
        }, type: types.event, text: "Reenviar Todos", idGrid: guid(), visible: ko.observable(true)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisGrupoMotoristas();
        }, type: types.event, text: "Obter Totais", idGrid: guid(), visible: ko.observable(true)
    });
}

var HistoricoGrupoMotoristas = function () {
    this.Integracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({});
}

//*******EVENTOS*******
function loadGrupoMotoristasIntegracao() {
    _grupoMotoristasIntegracao = new GrupoMotoristasIntegracao();
    KoBindings(_grupoMotoristasIntegracao, "knockoutIntegracaoEtapa");

    _historicoGrupoMotoristas = new HistoricoGrupoMotoristas();
    KoBindings(_historicoGrupoMotoristas, "knockoutHistoricoGrupoMotoristas");
    CarregarHistoricoGrupoMotoristas();
}

function ReenviarGrupoMotoristas(data) {
    executarReST("GrupoMotoristasIntegracao/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                _gridGrupoMotoristasIntegracao.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Falha!", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function HistoricoGrupoMotoristasClick(data) {
    _historicoGrupoMotoristas.Integracao.val(data.Codigo);
    _gridHistoricoGrupoMotoristas.CarregarGrid(function () {
        Global.abrirModal('divModalHistoricoIntegracao');
    });
}

function DownloadArquivosHistoricoIntegracaoPagamento(historicoConsulta) {
    executarDownload("GrupoMotoristasIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo, Integracao: _historicoGrupoMotoristas.Integracao.val() });
}

//*******MÉTODOS*******
function CarregaGrupoMotoristasIntegracao() {
    _grupoMotoristasIntegracao.CodigoGrupoMotoristas.val(_grupoMotoristas.Codigo.val());
    ConfigurarPesquisaGrupoMotoristas();
}

function ObterTotaisGrupoMotoristas() {
    executarReST("GrupoMotoristasIntegracao/ObterTotais", { Pagamento: _grupoMotoristasIntegracao.Pagamento.val() }, function (r) {
        if (r.Success) {
            _grupoMotoristasIntegracao.TotalGeral.val(r.Data.TotalGeral);
            _grupoMotoristasIntegracao.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _grupoMotoristasIntegracao.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _grupoMotoristasIntegracao.TotalIntegrado.val(r.Data.TotalIntegrado);
            BuscarLoteComFalhaIntegracao();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ReenviarTodosGrupoMotoristas() {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todas as integrações?", function () {
        executarReST("GrupoMotoristasIntegracao/ReenviarTodos", { GrupoMotoristas: _grupoMotoristas.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                    _gridGrupoMotoristasIntegracao.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Falha!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function BuscarLoteComFalhaIntegracao() {
    _grupoMotoristasIntegracao.FinalizarLoteMesmoComFalhaIntegracao.visible(false);
    executarReST("GrupoMotoristasIntegracao/BuscarSituacaoFalhaIntegracao", { CodigoPagamento: _grupoMotoristasIntegracao.Pagamento.val() }, function (retorno) {
        if (retorno.Success && retorno.Data) {
            _grupoMotoristasIntegracao.FinalizarLoteMesmoComFalhaIntegracao.visible(retorno.Data.MostrarBotaoFinalizarLote);
            _grupoMotoristasIntegracao.FinalizarLoteMesmoComFalhaIntegracao.enable(retorno.Data.MostrarBotaoFinalizarLote);
        }
    });
}

function FinalizarLoteMesmoComFalha() {
    exibirConfirmacao("Atenção!", "Deseja realmente Finalizar o lote com Falha na Integração?", function () {
        executarReST("GrupoMotoristasIntegracao/FinalizarLoteComFalhaIntegracao", { CodigoPagamento: _grupoMotoristasIntegracao.Pagamento.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Finalizado com sucesso.");
                _pagamento.Situacao.val(EnumSituacaoPagamento.Finalizado);
                _resumo.Situacao.val("Finalizado");
                SetarEtapasPagamento();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function CarregarHistoricoGrupoMotoristas() {
    var editar = { descricao: "Download Arquivos", id: "clasEditar", evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoPagamento, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridHistoricoGrupoMotoristas = new GridView("tblHistoricoIntegracaoLote", "GrupoMotoristasIntegracao/PesquisarHistorico", _historicoGrupoMotoristas, menuOpcoes);
}

function ConfigurarPesquisaGrupoMotoristas() {
    var historico = { descricao: "Histórico", id: guid(), metodo: HistoricoGrupoMotoristasClick, tamanho: "20", icone: "" };
    var reenviar = { descricao: "Reenviar", id: guid(), metodo: ReenviarGrupoMotoristas, tamanho: "20", icone: "" };
    var auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("GrupoMotoristasIntegracao"), icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [historico, reenviar, auditar] };

    _gridGrupoMotoristasIntegracao = new GridView(_grupoMotoristasIntegracao.Pesquisar.idGrid, "GrupoMotoristasIntegracao/Pesquisar", _grupoMotoristasIntegracao, menuOpcoes);
    _gridGrupoMotoristasIntegracao.CarregarGrid();
}
