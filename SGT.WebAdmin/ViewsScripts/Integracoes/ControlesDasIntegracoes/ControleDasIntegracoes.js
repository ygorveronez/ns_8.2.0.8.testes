//#region Variaveis Globais
var _pesquisaControleDasIntegracoes;
var _gridControleDasIntegracoes;
//#endregion

//#region Funções Principales
var PesquisaControleDasIntegracoes = function () {

    this.DataInicial = PropertyEntity({ text: "Data Inicial:", val: ko.observable(Global.DataAtual()), getType: typesKnockout.dateTime });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.dateTime });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable(""), def: "", options: Global.ObterOpcoesBooleano("Sim", "Não"), getType: typesKnockout.bool });
    this.Integradora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Integradora:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Metodo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Metodo:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true),val:ko.observable("") });
    this.Origem = PropertyEntity({ val: ko.observable(EnumOrigemAuditado.Todas), def: EnumOrigemAuditado.Todas, options: EnumOrigemAuditado.obterOpcoes() });

    this.DataInicial.dateLimit = this.DataFinal;
    this.DataFinal.dateInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridControleDasIntegracoes.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

function LoadControleDasIntegracoes() {
    _pesquisaControleDasIntegracoes = new PesquisaControleDasIntegracoes();
    KoBindings(_pesquisaControleDasIntegracoes, "knockoutControleDasIntegracoes", _pesquisaControleDasIntegracoes.Pesquisar.id);

    new BuscarIntegradora(_pesquisaControleDasIntegracoes.Integradora);
    new BuscarMetodosRest(_pesquisaControleDasIntegracoes.Metodo,retornobuscaMetodo);
    BuscarIntegracoesRealizadas();
}

//#endregion

//#region Funções Auxiliares
const DownloadArquivosIntegracao = (integracao) => executarDownload("ControleDasIntegracoes/DownloadArquivosIntegracao", { Codigo: integracao.Codigo });

const BuscarIntegracoesRealizadas = () => {
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [{ id: guid(), descricao: "Download Arquivos", metodo: DownloadArquivosIntegracao, tamanho: "15", icone: "" }] };

    _gridControleDasIntegracoes = new GridView(_pesquisaControleDasIntegracoes.Pesquisar.idGrid, "ControleDasIntegracoes/Pesquisar", _pesquisaControleDasIntegracoes, menuOpcoes, { column: 2, dir: orderDir.desc });
    _gridControleDasIntegracoes.CarregarGrid();
}

const retornobuscaMetodo = (retorno) => {
    _pesquisaControleDasIntegracoes.Metodo.val(retorno.NomeMetodo);
    _pesquisaControleDasIntegracoes.Metodo.codEntity(retorno.Codigo);
}
//#endregion