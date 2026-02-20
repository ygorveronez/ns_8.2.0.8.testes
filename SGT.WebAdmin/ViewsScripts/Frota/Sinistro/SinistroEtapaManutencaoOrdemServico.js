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
/// <reference path="SinistroEtapaManutencao.js" />
/// <reference path="../../Consultas/OrdemServico.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridOrdemServico;
var _etapaManutencaoSinistroOrdemServico;

var ManutencaoSinistroOrdemServico = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.OrdemServico = PropertyEntity({ type: types.event, text: "Adicionar Ordem de Serviço", idBtnSearch: guid(), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadManutencaoSinistroOrdemServico() {
    _etapaManutencaoSinistroOrdemServico = new ManutencaoSinistroOrdemServico();
    KoBindings(_etapaManutencaoSinistroOrdemServico, "knockoutManutencaoSinistroOrdemServico");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: function (data) { excluirOrdemServicoClick(data) } }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: "Número", width: "7%" },
        { data: "DataProgramada", title: "Data", width: "7%" },
        { data: "Veiculo", title: "Veículo", width: "10%" },
        { data: "Equipamento", title: "Equipamento", width: "10%" },
        { data: "NumeroFrota", title: "Número Frota", width: "8%" },
        { data: "Motorista", title: "Motorista", width: "13%" },
        { data: "LocalManutencao", title: "Local de Manutenção", width: "15%" },
        { data: "Operador", title: "Operador", width: "15%" },
        { data: "TipoManutencao", title: "Tipo de Manutenção", width: "10%" },
        { data: "Situacao", title: "Situação", width: "8%" },
        { data: "ValorOS", title: "Valor OS", width: "8%" }
    ];

    _gridOrdemServico = new BasicDataTable(_etapaManutencaoSinistroOrdemServico.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.desc });

    new BuscarOrdemServico(_etapaManutencaoSinistroOrdemServico.OrdemServico, null, _gridOrdemServico);
    _etapaManutencaoSinistroOrdemServico.OrdemServico.basicTable = _gridOrdemServico;

    recarregarGridManutencaoOrdemServico();
}

function recarregarGridManutencaoOrdemServico() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_etapaManutencaoSinistro.OrdensServico.val())) {
        $.each(_etapaManutencaoSinistro.OrdensServico.val(), function (i, ordemServico) {
            var ordemServicoGrid = new Object();

            ordemServicoGrid.Codigo = ordemServico.Codigo;
            ordemServicoGrid.Numero = ordemServico.Numero;
            ordemServicoGrid.DataProgramada = ordemServico.DataProgramada;
            ordemServicoGrid.Veiculo = ordemServico.Veiculo;
            ordemServicoGrid.Equipamento = ordemServico.Equipamento;
            ordemServicoGrid.NumeroFrota = ordemServico.NumeroFrota;
            ordemServicoGrid.Motorista = ordemServico.Motorista;
            ordemServicoGrid.LocalManutencao = ordemServico.LocalManutencao;
            ordemServicoGrid.Operador = ordemServico.Operador;
            ordemServicoGrid.TipoManutencao = ordemServico.TipoManutencao;
            ordemServicoGrid.Situacao = ordemServico.Situacao;
            ordemServicoGrid.ValorOS = ordemServico.ValorOS;

            data.push(ordemServicoGrid);
        });
    }
    _gridOrdemServico.CarregarGrid(data);
}

function excluirOrdemServicoClick(data) {
    var ordemServicoGrid = _etapaManutencaoSinistroOrdemServico.OrdemServico.basicTable.BuscarRegistros();

    for (var i = 0; i < ordemServicoGrid.length; i++) {
        if (data.Codigo == ordemServicoGrid[i].Codigo) {
            ordemServicoGrid.splice(i, 1);
            break;
        }
    }

    _etapaManutencaoSinistroOrdemServico.OrdemServico.basicTable.CarregarGrid(ordemServicoGrid);
}

function bloquearCamposManutencaoSinistroOrdemServico() {
    SetarEnableCamposKnockout(_etapaManutencaoSinistroOrdemServico, false);

    _gridOrdemServico.DesabilitarOpcoes();
}

function limparCamposManutencaoSinistroOrdemServico() {
    LimparCampos(_etapaManutencaoSinistroOrdemServico);
    _etapaManutencaoSinistroOrdemServico.OrdemServico.basicTable.CarregarGrid(new Array());
    SetarEnableCamposKnockout(_etapaManutencaoSinistroOrdemServico, true);

    _gridOrdemServico.HabilitarOpcoes();
}