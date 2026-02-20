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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="GrupoServico.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridLocalManutencao;
var _localManutencao;

var LocalManutencao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Cliente = PropertyEntity({ type: types.event, text: "Adicionar Cliente", idBtnSearch: guid() });

    this.Map = PropertyEntity();
};

//*******EVENTOS*******

function LoadLocalManutencao() {
    _localManutencao = new LocalManutencao();
    KoBindings(_localManutencao, "knockoutGrupoServicoLocalManutencao");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirLocalManutencaoClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CNPJCPF", visible: false },
        { data: "Descricao", title: "Cliente", width: "50%" },
        { data: "Localidade", title: "Cidade", width: "20%" },
        { data: "Latitude", visible: false },
        { data: "Longitude", visible: false }
    ];

    _gridLocalManutencao = new BasicDataTable(_localManutencao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_localManutencao.Cliente, function (retorno) {
        var localManutencaoGrid = _gridLocalManutencao.BuscarRegistros();

        $.each(retorno, function (i, localManutencao) {
            localManutencaoGrid.push({
                Codigo: guid(),
                CNPJCPF: localManutencao.Codigo,
                Descricao: localManutencao.Descricao,
                Localidade: localManutencao.Localidade,
                Latitude: localManutencao.Latitude,
                Longitude: localManutencao.Longitude
            });
        });

        _gridLocalManutencao.CarregarGrid(localManutencaoGrid);

        setarCoordenadas();
    }, null, [EnumModalidadePessoa.Fornecedor], null, _gridLocalManutencao);

    RecarregarGridLocalManutencao();
}

function RecarregarGridLocalManutencao() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_grupoServico.LocaisManutencao.val())) {

        $.each(_grupoServico.LocaisManutencao.val(), function (i, localManutencao) {
            var localManutencaoGrid = new Object();

            localManutencaoGrid.Codigo = localManutencao.Codigo;
            localManutencaoGrid.CNPJCPF = localManutencao.CNPJCPF;
            localManutencaoGrid.Descricao = localManutencao.Descricao;
            localManutencaoGrid.Localidade = localManutencao.Localidade;
            localManutencaoGrid.Latitude = localManutencao.Latitude;
            localManutencaoGrid.Longitude = localManutencao.Longitude;

            data.push(localManutencaoGrid);
        });
    }

    _gridLocalManutencao.CarregarGrid(data);

    setarCoordenadas();
}

function ExcluirLocalManutencaoClick(data) {
    var localManutencaoGrid = _gridLocalManutencao.BuscarRegistros();

    for (var i = 0; i < localManutencaoGrid.length; i++) {
        if (data.Codigo == localManutencaoGrid[i].Codigo) {
            localManutencaoGrid.splice(i, 1);
            break;
        }
    }

    _gridLocalManutencao.CarregarGrid(localManutencaoGrid);
}

function LimparCamposLocalManutencao() {
    LimparCampos(_localManutencao);
    limparCamposMapaRequest();
    _gridLocalManutencao.CarregarGrid(new Array());
}