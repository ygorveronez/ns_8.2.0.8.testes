/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
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

var _gridSintegra;
var _sintegra;
var _pesquisaSintegra;

var _tipoSintegra = [
    { text: "Todos", value: EnumTipoMovimentoSpedFiscal.Todos },
    { text: "Entrada", value: EnumTipoMovimentoSpedFiscal.Entrada },
    { text: "Saída", value: EnumTipoMovimentoSpedFiscal.Saida }
];

var PesquisaSintegra = function () {

    var _statusPesquisaSintegra = [
        { text: "Todos", value: 0 },
        { text: "Aguardando Geração", value: EnumStatusArquivoSpedFiscal.AguardandoGeracao },
        { text: "Gerado", value: EnumStatusArquivoSpedFiscal.Gerado },
        { text: "Em Processo", value: EnumStatusArquivoSpedFiscal.EmProcesso },
        { text: "Erro de Validação", value: EnumStatusArquivoSpedFiscal.ErroValidacao }
    ];

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Status = PropertyEntity({ val: ko.observable(0), options: _statusPesquisaSintegra, def: 0, text: "Status: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSintegra.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Sintegra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicial = PropertyEntity({ text: "*Data Inicial: ", getType: typesKnockout.date, required: true });
    this.DataFinal = PropertyEntity({ text: "*Data Final: ", getType: typesKnockout.date, required: true });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.TipoMovimento = PropertyEntity({ val: ko.observable(EnumTipoMovimentoSpedFiscal.Todos), options: _tipoSintegra, def: EnumTipoMovimentoSpedFiscal.Todos, text: "*Tipo Movimento: ", enable: ko.observable(true), required: true });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Solicitar Geração", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

//*******EVENTOS*******


function loadSintegra() {

    _pesquisaSintegra = new PesquisaSintegra();
    KoBindings(_pesquisaSintegra, "knockoutPesquisaSintegra", false, _pesquisaSintegra.Pesquisar.id);

    _sintegra = new Sintegra();
    KoBindings(_sintegra, "knockoutCadastroSintegra");

    HeaderAuditoria("Sintegra", _sintegra);

    new BuscarEmpresa(_sintegra.Empresa);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _sintegra.Empresa.visible(true);
        _sintegra.Empresa.required(true);
    }

    buscarSintegras();
}

function adicionarClick(e, sender) {
    Salvar(e, "Sintegra/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Geração do Sintegra solicitada com sucesso");
                _gridSintegra.CarregarGrid();
                limparCamposSintegra();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposSintegra();
}

//*******MÉTODOS*******


function buscarSintegras() {
    var baixar = { descricao: "Baixar", id: "clasBaixar", evento: "onclick", metodo: baixarTXTSintegra, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(baixar);

    _gridSintegra = new GridView(_pesquisaSintegra.Pesquisar.idGrid, "Sintegra/Pesquisa", _pesquisaSintegra, menuOpcoes, null);
    _gridSintegra.CarregarGrid();
}

function baixarTXTSintegra(sintegraGrid) {
    if (sintegraGrid.CodigoStatus === EnumStatusArquivoSpedFiscal.Gerado) {
        var dados = { Codigo: sintegraGrid.Codigo };
        executarDownload("Sintegra/DownloadTXT", dados);
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Só é possível baixar o arquivo txt do Sintegra com status Gerado", 60000);
}

function limparCamposSintegra() {
    LimparCampos(_sintegra);
}