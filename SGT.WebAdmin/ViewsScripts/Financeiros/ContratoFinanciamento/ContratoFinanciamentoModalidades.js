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
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="ContratoFinanciamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _contratoFinanciamentoModalidade, _gridModalidadesContratoFinanciamento;

var ContratoFinanciamentoModalidades = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Modalidades = PropertyEntity({ type: types.event, text: "Adicionar Modalidade(s)", idBtnSearch: guid(), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadContratoFinanciamentoModalidade() {
    _contratoFinanciamentoModalidade = new ContratoFinanciamentoModalidades();
    KoBindings(_contratoFinanciamentoModalidade, "knockoutModalidadeContratoFinanciamento");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: function (data) { ExcluirModalidadeClick(_contratoFinanciamentoModalidade.Modalidades, data) } }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "30%" },
        { data: "Status", title: "Status", width: "10%" }
    ];
    _gridModalidadesContratoFinanciamento = new BasicDataTable(_contratoFinanciamentoModalidade.Grid.id, header, menuOpcoes);
    _contratoFinanciamentoModalidade.Modalidades.basicTable = _gridModalidadesContratoFinanciamento;

    new BuscarModalidade(_contratoFinanciamentoModalidade.Modalidades, retornBuscaModalidades, _gridModalidadesContratoFinanciamento);

    RecarregarGridContratoFinanciamentoModalidades();
}

//*******MÉTODOS*******

function RecarregarGridContratoFinanciamentoModalidades() {
    var data = new Array();
 
    $.each(_contratoFinanciamento.Modalidades.val(), function (i, modalidade) {
        var modalidadeGrid = new Object();

        modalidadeGrid.Codigo = modalidade.MODALIDADE.Codigo;
        modalidadeGrid.Descricao = modalidade.MODALIDADE.Descricao;
        modalidadeGrid.Status = modalidade.MODALIDADE.DescricaoAtivo;

        data.push(modalidadeGrid);
    });

    _gridModalidadesContratoFinanciamento.CarregarGrid(data);
}

function ExcluirModalidadeClick(knoutModalidade, data) {
    var modalidadeGrid = knoutModalidade.basicTable.BuscarRegistros();

    for (var i = 0; i < modalidadeGrid.length; i++) {
        if (data.Codigo == modalidadeGrid[i].Codigo) {
            modalidadeGrid.splice(i, 1);
            break;
        }
    }

    knoutModalidade.basicTable.CarregarGrid(modalidadeGrid);
}

function preencherListasSelecaoContratoFinanciamentoModalidades() {
    var modalidades = new Array();

    $.each(_contratoFinanciamentoModalidade.Modalidades.basicTable.BuscarRegistros(), function (i, modalidade) {
        modalidades.push({ MODALIDADE: modalidade });
    });

    _contratoFinanciamento.Modalidades.val(JSON.stringify(modalidades));
}

function limparCamposContratoFinanciamentoVeiculo() {
    LimparCampos(_contratoFinanciamentoModalidade);
    RecarregarGridContratoFinanciamentoVeiculo();
}

function retornBuscaModalidades(modalidadesSelecionadas) {
    if (modalidadesSelecionadas.length == 0)
        return;

    var modalidadesAtuais = _gridModalidadesContratoFinanciamento.BuscarRegistros();
    for (var i = 0; i < modalidadesSelecionadas.length; i++)
        modalidadesAtuais.push({
            Codigo: modalidadesSelecionadas[i].Codigo,
            Descricao: modalidadesSelecionadas[i].Descricao,
            Status: modalidadesSelecionadas[i].DescricaoAtivo
        });

    _gridModalidadesContratoFinanciamento.CarregarGrid(modalidadesAtuais);
}