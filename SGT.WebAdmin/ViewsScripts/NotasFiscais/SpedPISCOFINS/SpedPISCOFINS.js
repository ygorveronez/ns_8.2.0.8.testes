/// <reference path="../../../js/libs/jquery-2.1.1.js" />
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
/// <reference path="../../Enumeradores/EnumStatusArquivoSpedFiscal.js" />
/// <reference path="../../Enumeradores/EnumTipoMovimentoSpedFiscal.js" />
/// <reference path="../../Consultas/Empresa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridSpedPISCOFINS;
var _spedPISCOFINS;
var _pesquisaSpedPISCOFINS;

var _tipoSpedPISCOFINS = [
    { text: "Todos", value: EnumTipoMovimentoSpedFiscal.Todos },
    { text: "Entrada", value: EnumTipoMovimentoSpedFiscal.Entrada },
    { text: "Saída", value: EnumTipoMovimentoSpedFiscal.Saida }
];

var PesquisaSpedPISCOFINS = function () {

    var _statusPesquisaSpedPISCOFINS = [
        { text: "Todos", value: 0 },
        { text: "Aguardando Geração", value: EnumStatusArquivoSpedFiscal.AguardandoGeracao },
        { text: "Gerado", value: EnumStatusArquivoSpedFiscal.Gerado },
        { text: "Em Processo", value: EnumStatusArquivoSpedFiscal.EmProcesso },
        { text: "Erro de Validação", value: EnumStatusArquivoSpedFiscal.ErroValidacao }
    ];

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Status = PropertyEntity({ val: ko.observable(0), options: _statusPesquisaSpedPISCOFINS, def: 0, text: "Status: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSpedPISCOFINS.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var SpedPISCOFINS = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicial = PropertyEntity({ text: "*Data Inicial: ", getType: typesKnockout.date, required: true });
    this.DataFinal = PropertyEntity({ text: "*Data Final: ", getType: typesKnockout.date, required: true });
    this.TipoMovimento = PropertyEntity({ val: ko.observable(EnumTipoMovimentoSpedFiscal.Todos), options: _tipoSpedPISCOFINS, def: EnumTipoMovimentoSpedFiscal.Todos, text: "*Tipo Movimento: ", enable: ko.observable(true), required: true });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Solicitar Geração", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

//*******EVENTOS*******


function loadSpedPISCOFINS() {

    _pesquisaSpedPISCOFINS = new PesquisaSpedPISCOFINS();
    KoBindings(_pesquisaSpedPISCOFINS, "knockoutPesquisaSpedPISCOFINS", false, _pesquisaSpedPISCOFINS.Pesquisar.id);

    _spedPISCOFINS = new SpedPISCOFINS();
    KoBindings(_spedPISCOFINS, "knockoutCadastroSpedPISCOFINS");

    HeaderAuditoria("SpedPISCOFINS", _spedPISCOFINS);

    new BuscarEmpresa(_spedPISCOFINS.Empresa);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _spedPISCOFINS.Empresa.visible(true);
        _spedPISCOFINS.Empresa.required(true);
    }

    buscarSpedPISCOFINSs();
}

function adicionarClick(e, sender) {
    if (_spedPISCOFINS.DataInicial.val() > _spedPISCOFINS.DataFinal.val()) {
        exibirMensagem("atencao", "Atenção", "Data Inicial não pode ser maior que a Data Final.");
        return;
    }

    Salvar(e, "SpedPISCOFINS/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Geração do Sped solicitada com sucesso");
                _gridSpedPISCOFINS.CarregarGrid();
                limparCamposSpedPISCOFINS();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposSpedPISCOFINS();
}

//*******MÉTODOS*******


function buscarSpedPISCOFINSs() {
    var baixar = { descricao: "Baixar", id: "clasBaixar", evento: "onclick", metodo: baixarTXTSpedPISCOFINS, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(baixar);

    _gridSpedPISCOFINS = new GridView(_pesquisaSpedPISCOFINS.Pesquisar.idGrid, "SpedPISCOFINS/Pesquisa", _pesquisaSpedPISCOFINS, menuOpcoes, null);
    _gridSpedPISCOFINS.CarregarGrid();
}

function baixarTXTSpedPISCOFINS(spedPISCOFINSGrid) {
    if (spedPISCOFINSGrid.CodigoStatus == EnumStatusArquivoSpedFiscal.Gerado) {
        var dados = { Codigo: spedPISCOFINSGrid.Codigo }
        executarDownload("SpedPISCOFINS/DownloadTXT", dados);
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Só é possível baixar o arquivo txt do Sped PIS COFINS com status Gerado", 60000);
}

function limparCamposSpedPISCOFINS() {
    LimparCampos(_spedPISCOFINS);
}