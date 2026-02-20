/// <reference path="RegrasMultaAtrasoRetirada.js" />
/// <reference path="../../Consultas/TipoOperacao.js"/>

//*******MAPEAMENTO KNOUCKOUT*******

var RegrasMultaAtrasoRetiradaTiposOperacoes;

//*******EVENTOS*******

function loadRegrasMultaAtrasoRetiradaTiposOperacoes() {

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirRegrasMultaAtrasoRetiradaTiposOperacoes(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Tipo de Operação", width: "80%", className: "text-align-left" }
    ];

    RegrasMultaAtrasoRetiradaTiposOperacoes = new BasicDataTable(_regrasMultaAtrasoRetirada.GridRegrasMultaAtrasoRetiradaTiposOperacoes.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });

    new BuscarTiposOperacao(_regrasMultaAtrasoRetirada.TiposOperacoes, null, null, null, RegrasMultaAtrasoRetiradaTiposOperacoes);
    _regrasMultaAtrasoRetirada.TiposOperacoes.basicTable = RegrasMultaAtrasoRetiradaTiposOperacoes;

    RecarregarGridRegrasMultaAtrasoRetiradaTiposOperacoes();
}

function RecarregarGridRegrasMultaAtrasoRetiradaTiposOperacoes() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaTiposOperacoes.val())) {
        $.each(_regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaTiposOperacoes.val(), function (i, TiposOperacoes) {
            var obj = new Object();

            obj.Codigo = TiposOperacoes.Codigo;
            obj.Descricao = TiposOperacoes.Descricao;

            data.push(obj);
        });
    }
    RegrasMultaAtrasoRetiradaTiposOperacoes.CarregarGrid(data);
}

function excluirRegrasMultaAtrasoRetiradaTiposOperacoes(data) {
    var regrasMultaAtrasoRetiradaTiposOperacoes = _regrasMultaAtrasoRetirada.TiposOperacoes.basicTable.BuscarRegistros();

    for (var i = 0; i < regrasMultaAtrasoRetiradaTiposOperacoes.length; i++) {
        if (data.Codigo == regrasMultaAtrasoRetiradaTiposOperacoes[i].Codigo) {
            regrasMultaAtrasoRetiradaTiposOperacoes.splice(i, 1);
            break;
        }
    }

    _regrasMultaAtrasoRetirada.TiposOperacoes.basicTable.CarregarGrid(regrasMultaAtrasoRetiradaTiposOperacoes);
}

function LimparRegrasMultaAtrasoRetiradaTiposOperacoes() {
    _regrasMultaAtrasoRetirada.TiposOperacoes.basicTable.CarregarGrid(new Array());
}