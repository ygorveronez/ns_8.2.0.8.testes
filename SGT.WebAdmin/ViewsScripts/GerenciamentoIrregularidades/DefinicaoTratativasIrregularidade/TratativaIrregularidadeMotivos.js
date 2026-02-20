/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />


var _gridTratativaIrregularidadeMotivos;


function loadGridTratativaIrregularidadeMotivos() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerMotivo, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "33%", className: "text-align-left" },
        { data: "Situacao", title: "Situação", width: "33%", className: "text-align-center" }
    ];

    _gridTratativaIrregularidadeMotivos = new BasicDataTable(_tratativaIrregularidade.TratativaIrregularidadeMotivos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    BuscarMotivosIrregularidade(_tratativaIrregularidade.TratativaIrregularidadeMotivos, null, _gridTratativaIrregularidadeMotivos, _definicaoTratativasIrregularidade.Irregularidade);
    _tratativaIrregularidade.TratativaIrregularidadeMotivos.basicTable = _gridTratativaIrregularidadeMotivos;

    _gridTratativaIrregularidadeMotivos.CarregarGrid([]);
}

function obterListaMotivos() {
    if (!string.IsNullOrWhiteSpace(_tratativaIrregularidade.TratativaIrregularidadeMotivos.val()))
        return _tratativaIrregularidade.TratativaIrregularidadeMotivos.val();
    else
        return _gridTratativaIrregularidadeMotivos.BuscarRegistros();
}

function removerMotivo(registroSelecionado) {
    var motivos = obterListaMotivos();

    for (var i = 0; i < motivos.length; i++) {
        if (registroSelecionado.Codigo == motivos[i].Codigo) {
            motivos.splice(i, 1);
            break;
        }
    }

    _gridTratativaIrregularidadeMotivos.CarregarGrid(motivos);
}

function renderizarGridTratativaIrregularidadeMotivos() {

    motivos = obterListaMotivos();

    _gridTratativaIrregularidadeMotivos.CarregarGrid(motivos);
}

function recarregarGridTratativaIrregularidadeMotivos() {
    _gridTratativaIrregularidadeMotivos.CarregarGrid([]);
}

