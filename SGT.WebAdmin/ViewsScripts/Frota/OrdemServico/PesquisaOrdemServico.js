/// <reference path="../../Consultas/GrupoServico.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOrdemServicoFrota.js" />
/// <reference path="../../Enumeradores/EnumTipoManutencaoOrdemServicoFrota.js" />
/// <reference path="../../Enumeradores/EnumTipoOficina.js"/>
/// <reference path="../../Enumeradores/EnumPrioridadeOrdemServico.js"/>s

//*******MAPEAMENTO KNOUCKOUT*******

var _gridOrdemServico;
var _pesquisaOrdemServico;

var PesquisaOrdemServico = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial:", getType: typesKnockout.int, maxlength: 10, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.NumeroFinal = PropertyEntity({ text: "Número Final:", getType: typesKnockout.int, maxlength: 10, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(""), def: "" });

    this.Situacao = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: "Situação:", options: EnumSituacaoOrdemServicoFrota.ObterOpcoesPesquisa() });
    this.TipoManutencao = PropertyEntity({ val: ko.observable(EnumTipoManutencaoOrdemServicoFrota.Todos), options: EnumTipoManutencaoOrdemServicoFrota.ObterOpcoesPesquisa(), def: EnumTipoManutencaoOrdemServicoFrota.Todos, text: "Tipo:" });
    this.TipoOrdemServico = PropertyEntity({ val: ko.observable(""), options: EnumTipoOficina.ObterOpcoesPesquisa(), text: "Tipo Ordem Serviço:" });

    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Equipamento:", idBtnSearch: guid() });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Serviço:", idBtnSearch: guid() });
    this.LocalManutencao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local de Manutenção:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Serviço:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Prioridade = PropertyEntity({ text: "Prioridade: ", val: ko.observable(""), options: EnumPrioridadeOrdemServico.obterOpcoesPesquisa() });
    this.NumeroFogoPneu = PropertyEntity({ col: 3, text: "Número Fogo Pneu:", getType: typesKnockout.string, def: ko.observable(""), val: ko.observable("")});

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridOrdemServico.CarregarGrid(callBackPesquisaOrdemServico);
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

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function LoadPesquisaOrdemServico() {

    _pesquisaOrdemServico = new PesquisaOrdemServico();
    KoBindings(_pesquisaOrdemServico, "knockoutPesquisaOrdemServico", false, _pesquisaOrdemServico.Pesquisar.id);

    new BuscarFuncionario(_pesquisaOrdemServico.Operador);
    new BuscarClientes(_pesquisaOrdemServico.LocalManutencao, null, true, [EnumModalidadePessoa.Fornecedor]);
    new BuscarVeiculos(_pesquisaOrdemServico.Veiculo);
    new BuscarMotorista(_pesquisaOrdemServico.Motorista, null, null, null);
    new BuscarServicoVeiculo(_pesquisaOrdemServico.Servico);
    new BuscarEquipamentos(_pesquisaOrdemServico.Equipamento);
    new BuscarGrupoServico(_pesquisaOrdemServico.GrupoServico);
    new BuscarCentroResultado(_pesquisaOrdemServico.CentroResultado);
    new BuscarTransportadores(_pesquisaOrdemServico.Empresa, null, null, null, null, null, null, null, null, null, true);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _pesquisaOrdemServico.GrupoServico.visible(false);
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Fornecedor) {
        _pesquisaOrdemServico.LocalManutencao.visible(false);
    }

    if (_CONFIGURACAO_TMS.LimitarOperacaoPorEmpresa)
        _pesquisaOrdemServico.Empresa.visible(true);

    BuscarOrdensServico();
}

//*******MÉTODOS*******

function BuscarOrdensServico() {
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [] };
    var menuOpcoes = new Object();

    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push({ descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarOrdemServico, tamanho: "5", icone: "" });
    menuOpcoes.opcoes.push({ descricao: "Enviar e-mail", id: guid(), evento: "onclick", metodo: EnviarEmailOrdemServico, tamanho: "5", icone: "" })

    _gridOrdemServico = new GridView(_pesquisaOrdemServico.Pesquisar.idGrid, "OrdemServico/Pesquisa", _pesquisaOrdemServico, menuOpcoes, { column: 1, dir: orderDir.desc }, null);

    _gridOrdemServico.CarregarGrid();
}

function EnviarEmailOrdemServico(e) {
    exibirConfirmacao("Atenção!", "Deseja realmente enviar o e-mail desta ordem de serviço?", function () {
        var data = { Codigo: e.Codigo };
        executarReST("OrdemServico/EnviarEmailOrdemServico", data, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Ordem de serviço enviada com sucesso!");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function callBackPesquisaOrdemServico(data) {
    if (data.data.length == 1)
        EditarOrdemServico(data.data[0]);
}