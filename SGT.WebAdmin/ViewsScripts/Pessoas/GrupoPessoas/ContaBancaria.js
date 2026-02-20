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
/// <reference path="../../Consultas/Banco.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="GrupoPessoa.js" />
/// <reference path="../../Enumeradores/EnumTipoChavePix.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridDadoBancario;
var _dadoBancario;
var _gridPessoaContaBancaria
var _pessoaContaBancaria;
//*******EVENTOS*******

function LoadGrupoPessoaContaBancaria2(configuracao) {

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirGrupoPessoaContaBancariaClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "15%" },
    ];

    _gridPessoaContaBancaria = new BasicDataTable(configuracao.GridContaBancaria.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarContasBancarias(configuracao.ContaBancaria, null, _gridPessoaContaBancaria);

    _grupoPessoas.ContaBancaria.basicTable = _gridOrigem;
    _grupoPessoas.ContaBancaria.basicTable.CarregarGrid(new Array());
}

function ExcluirGrupoPessoaContaBancariaClick2(data) {
    var PessoaContaBancariaGrid = _grupoPessoas.ContaBancaria.basicTable.BuscarRegistros();

    for (var i = 0; i < PessoaContaBancariaGrid.length; i++) {
        if (data.Codigo == PessoaContaBancariaGrid[i].Codigo) {
            PessoaContaBancariaGrid.splice(i, 1);
            break;
        }
    }

    _grupoPessoas.ContaBancaria.basicTable.CarregarGrid(PessoaContaBancariaGrid);
}

function LimparCamposGrupoPessoaContaBancaria2() {
    _grupoPessoas.ContaBancaria.basicTable.CarregarGrid(new Array());
}