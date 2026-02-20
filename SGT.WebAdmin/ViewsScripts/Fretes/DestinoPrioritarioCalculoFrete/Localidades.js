//*******MAPEAMENTO KNOUCKOUT*******
var _codigosLocalidade = [];
var _gridLocalidades;
var _auxGridLocalidades;

var LocalidadeMap = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Ativo = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
}


//*******EVENTOS*******
function ProcessaLinhasReorndenadas() {
    var listaAtualizada = [];
    var listaExistente = GetLocalidadesList();

    var BuscaRegraPorCodigo = function (codigo) {
        for (var i in listaExistente)
            if (listaExistente[i].Localidade.codEntity() == codigo)
                return listaExistente[i];

        return null;
    }

    $("#" + _destinoPrioritarioCalculoFrete.Localidades.idGrid + " table tbody tr").each(function (i) {
        var regra = BuscaRegraPorCodigo($(this).data('codigo'));
        regra.Ordem.val(i + 1);
        listaAtualizada.push(regra);
    });

    SetLocalidadesList(listaAtualizada);
}

function RetornoLocalidades(data) {
    if (!$.isArray(data)) data = [data];
    var dadosGrid = GetLocalidadesList();
    var ultimaOrdem = dadosGrid.length;

    var novosDados = [];
    var i = 0;
    data.map(function (loc) {
        if ($.inArray(loc.Codigo, _codigosLocalidade) >= 0)
            return;
        i++;
        _codigosLocalidade.push(loc.Codigo);

        var _localidade = new LocalidadeMap();
        _localidade.Codigo.val(guid());
        _localidade.Localidade.val(loc.Descricao);
        _localidade.Localidade.codEntity(loc.Codigo);
        _localidade.Ordem.val(ultimaOrdem + i);
        novosDados.push(_localidade);
    });

    var novaGrid = dadosGrid.concat(novosDados);
    SetLocalidadesList(novaGrid);
    RecarregarDadosGridLocalidade();
}

function ExcluirLocalidade(codigo) {
    var dataGrid = GetLocalidadesList().sort(function (a, b) { return a.Ordem.val() < b.Ordem.val() });
   
    for (var i = 0; i < dataGrid.length; i++) {
        if (codigo == dataGrid[i].Codigo.val()) {
            var localidadeRemovida = dataGrid[i].Localidade.codEntity();
            var index = $.inArray(localidadeRemovida + "", _codigosLocalidade);
            if (index >= 0)
                _codigosLocalidade.splice(index, 1);

            dataGrid.splice(i, 1);
            diminuirOrdem = true;
        }
    }

    for (var i = 0; i < dataGrid.length; i++)
        dataGrid[i].Ordem.val(i + 1);

    SetLocalidadesList(dataGrid);
    RecarregarDadosGridLocalidade();
}

function AtivarDesativarLocalidade(codigo, status) {
    var dataGrid = GetLocalidadesList();

    for (var i = 0; i < dataGrid.length; i++) {
        if (codigo == dataGrid[i].Codigo.val()) {
            dataGrid[i].Ativo.val(status);
            RecarregarDadosGridLocalidade();
            break;
        }
    }
}



//*******METODOS*******
function GetLocalidadesList() {
    return _destinoPrioritarioCalculoFrete.Localidades.list.slice();
}

function SetLocalidadesList(dados) {
    dados = dados.sort(function (a, b) { return a.Ordem.val() < b.Ordem.val() });
    
    _destinoPrioritarioCalculoFrete.Localidades.list = dados.slice();
}

function GetLocalidades() {
    var dados = GetLocalidadesList().map(function (l) {
        return {
            Codigo: l.Codigo.val(),
            Ordem: l.Ordem.val(),
            Ativo: l.Ativo.val(),
            Localidade: l.Localidade.codEntity(),
        }
    });
    return JSON.stringify(dados);
}

function CarregarGridLocalidades() {
    //-- Grid
    var headHtml = '';
    headHtml += '<tr>';
    headHtml += '    <th width="5%">Ordem</th>';
    headHtml += '    <th width="88%">Descrição</th>';
    headHtml += '    <th class="text-align-center" width="7%">Opções</th>';
    headHtml += '</tr > ';

    _gridLocalidades = new GridReordering("Mova as linhas conforme a prefêrencia de prioridades", _destinoPrioritarioCalculoFrete.Localidades.idGrid, headHtml, "");
    
    var header = [{ data: "Codigo" }, { data: "Descricao" }];
    _auxGridLocalidades = new BasicDataTable("aux-" + _destinoPrioritarioCalculoFrete.Localidades.idGrid, header);
    _auxGridLocalidades.CarregarGrid([]);

    $("#" + _destinoPrioritarioCalculoFrete.Localidades.idGrid).on('sortstop', function () {
        ProcessaLinhasReorndenadas();
    });
}

function RecarregarDadosGridLocalidade() {
    var html = "";
    var dados = GetLocalidadesList();

    $.each(dados, function (i, localidade) {
        html += '<tr data-position="' + localidade.Ordem.val() + '" id="sort_' + localidade.Ordem.val() + '" data-codigo="' + localidade.Localidade.codEntity() + '" ' + (localidade.Ativo.val() ? '' : 'style="background-color: #9a9a9a;color: #fff;"') + '>';
        html += '    <td>' + localidade.Ordem.val() + '</td>';
        html += '    <td>' + localidade.Localidade.val() + '</td>';
        html += '    <td class="sorting_disabled_opcao text-align-center" style="overflow: visible;">' + GerarOpcoesGrid(localidade) + '</td>';
        html += '</tr>';
    });

    _gridLocalidades.RecarregarGrid(html);
    _auxGridLocalidades.CarregarGrid(_codigosLocalidade.map(function (cod) {
        return {
            DT_RowId: cod,
            Codigo: cod,
            Descricao: "_"
        }
    }));
}


function GerarOpcoesGrid(localidade) {
    var codigo = localidade.Codigo.val();
    var htmlOpcoes = '';
    
    htmlOpcoes += '<div class="btn-group btn-b" style="width: 100%;">';
    htmlOpcoes += '    <button class="btn btn-default btn-xs btn-block dropdown-toggle" data-bs-toggle="dropdown"><i class="glyphicon glyphicon-list"></i>&nbsp;<i class="fal fa-align-justify"></i></button>';
    htmlOpcoes += '    <ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1">';
    if (localidade.Ativo.val())
        htmlOpcoes += '        <li><a class="dropdown-item" href="javascript:;" onclick="AtivarDesativarLocalidade(\'' + codigo + '\', false);" style="cursor: pointer;">Desativar</a></li>';
    else
        htmlOpcoes += '        <li><a class="dropdown-item" href="javascript:;" onclick="AtivarDesativarLocalidade(\'' + codigo + '\', true);" style="cursor: pointer;">Ativar</a></li>';
    
    htmlOpcoes += '        <li><a class="dropdown-item" href="javascript:;" onclick="ExcluirLocalidade(\'' + codigo + '\');" style="cursor: pointer;">Excluir</a></li>';
    htmlOpcoes += '    </ul>';
    htmlOpcoes += '</div>';

    return htmlOpcoes;
}