/// <reference path="Anexos.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _aceite;
var _pesquisaAceite;
var _gridAceite;

var _situacao = [
    { text: "Todos", value: "" },
    { text: "Ag. Aceite", value: EnumSituacaoAceiteDebito.AgAceite },
    { text: "Aprovado", value: EnumSituacaoAceiteDebito.Aprovado },
    { text: "Rejeitado", value: EnumSituacaoAceiteDebito.Rejeitado },
];


var PesquisaAceiteDebito = function () {
    this.NumeroOcorrencia = PropertyEntity({ text: "Número da Ocorrência:", val: ko.observable(""), def: "" });

    this.DataCriacaoInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date });
    this.DataCriacaoFim = PropertyEntity({ text: "Data Fim: ", getType: typesKnockout.date });
    this.DataCriacaoInicio.dateRangeLimit = this.DataCriacaoFim;
    this.DataCriacaoFim.dateRangeInit = this.DataCriacaoInicio;

    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacao, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAceite.CarregarGrid();
        }, type: types.event, text: "Atualizar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var AceiteDebito = function () {
    this.Codigo = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0) });
    this.Situacao = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0) });
    this.Ocorrencia = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0) });

    //this.Termos = PropertyEntity({ eventClick: termosClick, type: types.event, text: "Download Termos", visible: ko.observable(true) });
    this.Aprovar = PropertyEntity({ eventClick: aprovarAceiteClick, type: types.event, text: "Aprovar", visible: ko.observable(true) });
    this.Reprovar = PropertyEntity({ eventClick: reprovarAceiteClick, type: types.event, text: "Reprovar", visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", enable: ko.observable(true), visible: ko.observable(true) });
}



//*******EVENTOS*******
function loadAceiteDebito() {
    _aceite = new AceiteDebito();
    KoBindings(_aceite, "knockoutAceiteDebito");

    _pesquisaAceite = new PesquisaAceiteDebito();
    KoBindings(_pesquisaAceite, "knockoutPesquisaAceiteDebito", false, _pesquisaAceite.Pesquisar.id);

    // Modulos
    loadOcorrencia();
    loadAnexos();
    loadDocumentos();

    // Buscar dados
    BuscarAceites();
}

function detalharAceite(dataGrid) {
    _aceite.Codigo.val(dataGrid.Codigo);

    BuscaAceitePorCodigo();
}

function aprovarAceiteClick(e, sender) {
    RespostaAceite("Aprovado", true);
}

function reprovarAceiteClick(e, sender) {
    RespostaAceite("Reprovado", false);
}

function termosClick(e, sender) {
    var data = { Codigo: _aceite.Aceite.val() };
    executarDownload("AceiteDebito/TermoAceite", data);
}

function detalharClick(dataGrid) {
    ManutencaoAvaria(dataGrid.Codigo, _aceite.Aceite.val());
}




//*******MÉTODOS*******
function BuscarAceites() {
    //-- Cabecalho
    var detalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharAceite,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    _gridAceite = new GridView(_pesquisaAceite.Pesquisar.idGrid, "AceiteDebito/Pesquisa", _pesquisaAceite, menuOpcoes);
    _gridAceite.CarregarGrid();
}

function BuscaAceitePorCodigo() {
    BuscarPorCodigo(_aceite, "AceiteDebito/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                // Anexos
                ReloadAnexos();

                _gridCTeComplementar.CarregarGrid();

                CarregarDadosOcorrencia(arg.Data);

                // Mostra tela
                $("#wid-id-4").removeClass("d-none");
                
                // Esconde filtro
                _pesquisaAceite.ExibirFiltros.visibleFade(false);

                if (_aceite.Situacao.val() == EnumSituacaoAceiteDebito.AgAceite)
                    ControleCampos(true);
                else
                    ControleCampos(false);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function RespostaAceite(titulo, resposta, sender) {
    // Dados para aprovar/reprovar
    var dados = {
        Codigo: _aceite.Codigo.val(),
        Resposta: resposta,
        Observacao: _aceite.Observacao.val()
    };

    // Executa requisicao
    executarReST("AceiteDebito/ResponderAceite", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "O débito foi " + titulo + " com sucesso");
                LimparCamposAceite();
                _gridAceite.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function LimparCamposAceite() {
    LimparCampos(_aceite);

    // Esconde tela
    $("#wid-id-4").addClass("d-none");
    

    // Esconde filtro
    _pesquisaAceite.ExibirFiltros.visibleFade(true);
}

function ControleCampos(status) {
    _aceite.Observacao.enable(status);
    _aceite.Aprovar.visible(status);
    _aceite.Reprovar.visible(status);

    _anexos.AdicionarAnexo.visible(status);
}