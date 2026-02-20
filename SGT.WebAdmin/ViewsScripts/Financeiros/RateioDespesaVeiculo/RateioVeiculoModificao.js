/// <reference path="RateioDespesaVeiculo.js" />

var feitaModificaoGrid = false;
var valorTemporario = 0;


// ########## Grids Modificaveis ##########
function LoadGridVeiculosModificavel() {
    const menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirElementoGridClick(_rateioDespesa.Veiculo, data);
            }
        }]
    };

    const editarColuna = {
        permite: true,
        atualizarRow: true,
        callback: AtualizarValorGridVeiculo
    };

    const header = [
        { data: "Codigo", visible: false },
        { data: "Placa", title: "Placa", width: "12%" },
        { data: "NumeroFrota", title: "Nº Frota", width: "12%" },
        { data: "ModeloVeicularCarga", title: "Modelo Veicular", width: "33%" },
        { data: "SegmentoVeiculo", title: "Segmento do Veículo", width: "33%" },
        { data: "Valor", title: "Valor", width: "12%", editableCell: { editable: true } }
    ];


    _gridVeiculoDespesa = new BasicDataTable(_rateioDespesa.GridVeiculosDespesa.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, null, null, null, editarColuna);

    new BuscarVeiculos(_rateioDespesa.Veiculo, RetornoVeiculos, null, null, null, null, null, null, null, null, null, null, null, _gridVeiculoDespesa);

    _rateioDespesa.Veiculo.basicTable = _gridVeiculoDespesa;

    RecarregarGridModificavel('Veiculo');
}

function LoadGridCentroResultadoModificavel() {

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirElementoGridClick(_rateioDespesa.CentroResultado, data);
            }
        }]
    };


    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "60%" },
        { data: "Plano", title: "Número do centro", width: "20%" },
        { data: "Valor", title: "Valor", width: "10%", editableCell: { editable: true } }
    ];

    const editarColuna = {
        permite: true,
        atualizarRow: true,
        callback: AtualizarValorGridCentroResultado
    };


    _gridCentroResultadoDespesa = new BasicDataTable(_rateioDespesa.GridCentroResultadoDespesa.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, null, null, null, editarColuna);

    new BuscarCentroResultado(_rateioDespesa.CentroResultado, null, null, RetornoCentroResultado, null, null, _gridCentroResultadoDespesa, function () { ControlarVisibilidadeSegmentoEVeiculo(); });

    _rateioDespesa.CentroResultado.basicTable = _gridCentroResultadoDespesa;

    RecarregarGridModificavel('CentroResultado');

}

//########## Callback Busca ##########

function RetornoVeiculos(selecionados) {
    const elementosGrid = ObterElementosGrid('Veiculo');
    const novaLista = [...elementosGrid, ...selecionados];
    const valorFracionado = ObterFracaoValor(novaLista.length);

    novaLista.forEach((item) => {
        item.Valor = valorFracionado.toFixed(2);
    })

    _gridVeiculoDespesa.CarregarGrid(novaLista);
    ControlarVisibilidadeSegmentoEVeiculo();

}
function RetornoCentroResultado(selecionados) {
    const elementosGrid = ObterElementosGrid('CentroResultado');
    const novaLista = [...elementosGrid, ...selecionados];
    const valorFracionado = ObterFracaoValor(novaLista.length);

    novaLista.forEach((item) => {
        item.Valor = valorFracionado.toFixed(2);
    })

    _gridCentroResultadoDespesa.CarregarGrid(novaLista);
    ControlarVisibilidadeSegmentoEVeiculo();

}

//########## Funções utilizadas nas grids ########## 

function RecarregarGridModificavel(nomeGrid) {
    const data = new Array();
    let valorGrid = [];

    if (nomeGrid === "Veiculo")
        valorGrid = _rateioDespesa.VeiculosDespesa.val();

    if (nomeGrid === "CentroResultado")
        valorGrid = _rateioDespesa.CentrosResultadoDespesa.val();

    if (!string.IsNullOrWhiteSpace(valorGrid)) {
        $.each(valorGrid, function (i, item) {
            const objetoGrid = new Object();

            if (nomeGrid === "Veiculo") {
                objetoGrid.Codigo = item.Codigo;
                objetoGrid.Placa = item.Placa;
                objetoGrid.Valor = item.Valor;
                objetoGrid.ModeloVeicularCarga = item.ModeloVeicularCarga;
                objetoGrid.SegmentoVeiculo = item.SegmentoVeiculo;
                objetoGrid.NumeroFrota = item.NumeroFrota;
            }

            if (nomeGrid === "CentroResultado") {
                objetoGrid.Codigo = item.Codigo;
                objetoGrid.Descricao = item.Descricao;
                objetoGrid.Plano = item.Plano;
                objetoGrid.Valor = item.Valor;
            }

            data.push(objetoGrid);
        });
    }

    if (nomeGrid === "Veiculo")
        if (_gridVeiculoDespesa.CarregarGrid(data))
            _gridVeiculoDespesa.CarregarGrid(data);

    if (nomeGrid === "CentroResultado")
        if (_gridCentroResultadoDespesa.CarregarGrid(data))
            _gridCentroResultadoDespesa.CarregarGrid(data);

    ControlarVisibilidadeSegmentoEVeiculo();
}

function ExcluirElementoGridClick(knout, data) {
    let listaRegistroGrid = knout.basicTable.BuscarRegistros();
    let novoValorGeral = 0;

    for (var i = 0; i < listaRegistroGrid.length; i++) {
        if (data.Codigo == listaRegistroGrid[i].Codigo) {
            listaRegistroGrid.splice(i, 1);
            break;
        }
    }

    listaRegistroGrid.forEach(elemento => novoValorGeral += ObterSomenteNumeros(elemento.Valor));

    knout.basicTable.CarregarGrid(listaRegistroGrid);
    ModificarValorGeral(novoValorGeral);

    ControlarVisibilidadeSegmentoEVeiculo();
}

function AtualizarValorGridVeiculo(elemento) {
    const elementosGrid = ObterElementosGrid('Veiculo');
    let novoValorTotal = 0;
    feitaModificaoGrid = true;
    for (let i = 0; i < elementosGrid.length; i++) {
        if (elemento.Placa === elementosGrid[i].Placa) {
            let valorNumerico = ObterSomenteNumeros(elemento.Valor);
            elementosGrid[i].Valor = valorNumerico;
        }
        novoValorTotal += ObterSomenteNumeros(elementosGrid[i].Valor);
    }

    _gridVeiculoDespesa.CarregarGrid(elementosGrid);

    ModificarValorGeral(novoValorTotal);
}

function AtualizarValorGridCentroResultado(elemento) {
    const elementosGrid = ObterElementosGrid('CentroResultado');
    let novoValorTotal = 0;
    feitaModificaoGrid = true;
    for (let i = 0; i < elementosGrid.length; i++) {
        if (elemento.Codigo === elementosGrid[i].Codigo) {
            let valorNumerico = ObterSomenteNumeros(elemento.Valor);
            elementosGrid[i].Valor = valorNumerico;
        }
        novoValorTotal += ObterSomenteNumeros(elementosGrid[i].Valor);
    }

    _gridCentroResultadoDespesa.CarregarGrid(elementosGrid);

    ModificarValorGeral(novoValorTotal);
}

function ObterElementosGrid(grid) {
    if (grid === 'Veiculo')
        return _rateioDespesa.Veiculo.basicTable.BuscarRegistros();

    if (grid === 'CentroResultado')
        return _rateioDespesa.CentroResultado.basicTable.BuscarRegistros();
}

// ########## Funções Gerais ##########

function ObterSomenteNumeros(valor) {
    if (typeof valor !== "string")
        return valor;

    if (!valor.includes(',') && !valor.includes('.'))
        return Number(valor);

    if (valor.includes(',')) {
        let [enteros, decimal] = valor.split(',');

        if (enteros.includes('.'))
            enteros = enteros.replace(/[^0-9]/g, '');

        if (Number(decimal) > 0)
            return parseFloat(`${enteros}.${decimal}`);

        return Number(enteros);
    }

    if (valor.includes('.'))
        return Number(valor.replace('[^0-9]', ''));
}

function DistribuirValorGrid(novoValor) {
    let valorFracionado = 0;
    const listaElementosGridVeiculo = ObterElementosGrid('Veiculo');
    const listaElementosGridCentroResultado = ObterElementosGrid('CentroResultado');

    if (feitaModificaoGrid && valorTemporario == novoValor)
        return;

    if (listaElementosGridVeiculo.length === 0 && listaElementosGridCentroResultado.length === 0)
        return;


    if (listaElementosGridVeiculo.length > 0) {

        valorFracionado = ObterFracaoValor(listaElementosGridVeiculo.length);

        listaElementosGridVeiculo.forEach((item) => {
            item.Valor = valorFracionado.toFixed(2);
        })

        return _gridVeiculoDespesa.CarregarGrid(listaElementosGridVeiculo);
    }

    valorFracionado = ObterFracaoValor(listaElementosGridCentroResultado.length);
    listaElementosGridCentroResultado.forEach((item) => {
        item.Valor = valorFracionado.toFixed(2);
    })

    return _gridCentroResultadoDespesa.CarregarGrid(listaElementosGridCentroResultado);

}

function ModificarValorGeral(novoValor) {
    valorTemporario = novoValor.toFixed(2);
    _rateioDespesa.Valor.val(novoValor.toFixed(2));

}

function ObterFracaoValor(divisor) {
    const valortotal = ObterValorGeral();
    return valortotal / divisor;
}

const ObterValorGeral = () => ObterSomenteNumeros(_rateioDespesa.Valor.val());