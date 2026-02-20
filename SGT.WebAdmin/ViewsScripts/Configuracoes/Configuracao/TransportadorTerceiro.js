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
/// <reference path="../../Enumeradores/EnumTipoProprietarioVeiculo.js" />
/// <reference path="../../Enumeradores/EnumTipoQuitacaoCIOT.js" />
/// <reference path="../../Consultas/PagamentoMotoristaTipo.js" />
/// <reference path="../../Consultas/TipoTerceiro.js" />
/// <reference path="../../Consultas/ConfiguracaoCIOT.js" />
/// <reference path="Configuracao.js" />

var _gridTiposPagamentoCIOT;
//*******MAPEAMENTO KNOUCKOUT*******
var TipoPagamentoCIOTMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.OperadoraCIOT = PropertyEntity({ type: types.map, val: "" });
    this.TipoPagamentoCIOT = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoTipoPagamentoCIOT = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoOperadoraCIOT = PropertyEntity({ type: types.map, val: "" });
};

function loadTransportadorTerceiro() {
    loadGridTiposPagamentoCIOT();
}

function adicionarTipoPagamentoCIOTClick() {
    if (_configuracaoEmbarcador.TipoPagamentoCIOTOperadora.val() === "") {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    var tipo = new TipoPagamentoCIOTMap();
    tipo.Codigo.val = guid();
    tipo.OperadoraCIOT.val = _configuracaoEmbarcador.OperadoraTipoPagamentoCIOTOperadora.val();
    tipo.TipoPagamentoCIOT.val = _configuracaoEmbarcador.TipoPagamentoCIOTOperadora.val();
    tipo.DescricaoTipoPagamentoCIOT.val = EnumTipoPagamentoCIOT.obterDescricao(tipo.TipoPagamentoCIOT.val);
    tipo.DescricaoOperadoraCIOT.val = EnumOperadoraCIOT.obterDescricao(tipo.OperadoraCIOT.val); 

    _configuracaoEmbarcador.TiposPagamentoCIOTOperadora.list.push(tipo);
    recarregarGridTiposPagamentoCIOT();
}

function loadGridTiposPagamentoCIOT() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: excluirTipoPagamentoCIOTClick, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "OperadoraCIOT", visible: false },
        { data: "TipoPagamentoCIOT", visible: false },
        { data: "DescricaoOperadoraCIOT", title: "Operadora de CIOT", width: "50%" },
        { data: "DescricaoTipoPagamentoCIOT", title: "Tipo do Pagamento", width: "50%" }
    ];
    _gridTiposPagamentoCIOT = new BasicDataTable(_configuracaoEmbarcador.TiposPagamentoCIOTOperadora.idGrid, header, menuOpcoes);
    recarregarGridTiposPagamentoCIOT();
}

function recarregarGridTiposPagamentoCIOT() {
    var data = new Array();
    
    $.each(_configuracaoEmbarcador.TiposPagamentoCIOTOperadora.list, function (i, tipo) {
        var obj = new Object(); 
        obj.Codigo = tipo.Codigo.val;
        obj.TipoPagamentoCIOT = tipo.TipoPagamentoCIOT.val;
        obj.OperadoraCIOT = tipo.OperadoraCIOT.val;
        obj.DescricaoTipoPagamentoCIOT = tipo.DescricaoTipoPagamentoCIOT.val;
        obj.DescricaoOperadoraCIOT = tipo.DescricaoOperadoraCIOT.val;

        data.push(obj);
    });
    _gridTiposPagamentoCIOT.CarregarGrid(data);
}
function recarregarGridTiposPagamentoCIOTConsulta() {
    var data = new Array();

    $.each(_configuracaoEmbarcador.TiposPagamentoCIOTOperadora.list, function (i, tipo) {
        var obj = new Object();
        obj.Codigo = tipo.Codigo;
        obj.TipoPagamentoCIOT = tipo.TipoPagamentoCIOT;
        obj.OperadoraCIOT = tipo.OperadoraCIOT;
        obj.DescricaoTipoPagamentoCIOT = tipo.DescricaoTipoPagamentoCIOT;
        obj.DescricaoOperadoraCIOT = tipo.DescricaoOperadoraCIOT;

        data.push(obj);
    });
    _gridTiposPagamentoCIOT.CarregarGrid(data);
}
function excluirTipoPagamentoCIOTClick(data) {
    var listaAtualizada = new Array();
    $.each(_configuracaoEmbarcador.TiposPagamentoCIOTOperadora.list, function (i, tipo) {
        if (tipo.Codigo.val != data.Codigo) {
            listaAtualizada.push(tipo);
        }
    });
    _configuracaoEmbarcador.TiposPagamentoCIOTOperadora.list = listaAtualizada;
    recarregarGridTiposPagamentoCIOT();
    
}