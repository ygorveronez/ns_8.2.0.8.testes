//*******MAPEAMENTO KNOUCKOUT*******

var _gridAutorizadoDownloadDFe;
var _autorizadoDownloadDFe;

var AutorizadoDownloadDFe = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Pessoa = PropertyEntity({ type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.AdicionarPessoa, idBtnSearch: guid(), issue: 0, enable: ko.observable(true) });
};

//*******EVENTOS*******

function LoadAutorizadoDownloadDFe() {

    _autorizadoDownloadDFe = new AutorizadoDownloadDFe();
    KoBindings(_autorizadoDownloadDFe, "knockoutAutorizadoDownloadDFe");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Pessoas.GrupoPessoas.Excluir, id: guid(), metodo: function (data) {
                ExcluirAutorizadoDownloadDFeClick(_autorizadoDownloadDFe.Pessoa, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Pessoas.GrupoPessoas.Descricao, width: "80%" }
    ];

    _gridAutorizadoDownloadDFe = new BasicDataTable(_autorizadoDownloadDFe.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_autorizadoDownloadDFe.Pessoa, null, null, null, null, _gridAutorizadoDownloadDFe);

    _autorizadoDownloadDFe.Pessoa.basicTable = _gridAutorizadoDownloadDFe;

    RecarregarGridAutorizadoDownloadDFe();
}

function RecarregarGridAutorizadoDownloadDFe() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_grupoPessoas.AutorizadosDownloadDFe.val())) {
        $.each(_grupoPessoas.AutorizadosDownloadDFe.val(), function (i, autorizado) {
            var autorizadoGrid = new Object();

            autorizadoGrid.Codigo = autorizado.Pessoa.Codigo;
            autorizadoGrid.Descricao = autorizado.Pessoa.Descricao;

            data.push(autorizadoGrid);
        });
    }

    _gridAutorizadoDownloadDFe.CarregarGrid(data);
}


function ExcluirAutorizadoDownloadDFeClick(knoutAutorizados, data) {
    var autorizadosGrid = knoutAutorizados.basicTable.BuscarRegistros();

    for (var i = 0; i < autorizadosGrid.length; i++) {
        if (data.Codigo == autorizadosGrid[i].Codigo) {
            autorizadosGrid.splice(i, 1);
            break;
        }
    }

    knoutAutorizados.basicTable.CarregarGrid(autorizadosGrid);
}

function LimparCamposAutorizadoDownloadDFe() {
    LimparCampos(_autorizadoDownloadDFe);
}