/// <reference path="RegrasMultaAtrasoRetirada.js" />
/// <reference path="../../Consultas/Cliente.js"/>

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCEP;

//*******EVENTOS*******

function LoadFaixaCEP() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirCEPClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Faixa dos CEP's", width: "80%" }
    ];

    _gridCEP = new BasicDataTable(_regrasMultaAtrasoRetirada.GridCEP.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridCEP();
}

function RecarregarGridCEP() {

    var data = new Array();

    $.each(_regrasMultaAtrasoRetirada.CEPs.val(), function (i, cep) {
        var cepGrid = new Object();

        cepGrid.Codigo = cep.Codigo;
        cepGrid.Descricao = "De " + cep.CEPInicial + " até " + cep.CEPFinal;

        data.push(cepGrid);
    });

    _gridCEP.CarregarGrid(data);
}

function ExcluirCEPClick(data) {
    var ceps = _regrasMultaAtrasoRetirada.CEPs.val();

    for (var i = 0; i < ceps.length; i++) {
        if (data.Codigo == ceps[i].Codigo) {
            ceps.splice(i, 1);
            break;
        }
    }

    _regrasMultaAtrasoRetirada.CEPs.val(ceps);

    RecarregarGridCEP();
}

function AdicionarCEPClick(e, sender) {

    var cepInicial = Globalize.parseInt(string.OnlyNumbers(_regrasMultaAtrasoRetirada.CEPInicial.val()));
    var cepFinal = Globalize.parseInt(string.OnlyNumbers(_regrasMultaAtrasoRetirada.CEPFinal.val()));

    if (isNaN(cepInicial) || isNaN(cepFinal)) {
        exibirMensagem(tipoMensagem.aviso, "Faixa de CEP inválida", "Informe uma faixa de CEP válida.");
        return;
    }

    if (cepFinal < cepInicial) {
        exibirMensagem(tipoMensagem.aviso, "Faixa de CEP inválida", "O CEP inicial não pode ser menor que o CEP final.");
        return;
    }

    var ceps = _regrasMultaAtrasoRetirada.CEPs.val();

    if (ceps.some(function (item) {
        var cepInicialCadastrado = Globalize.parseInt(string.OnlyNumbers(item.CEPInicial));
        var cepFinalCadastrado = Globalize.parseInt(string.OnlyNumbers(item.CEPFinal));

        if ((cepInicialCadastrado >= cepInicial && cepInicialCadastrado <= cepFinal) ||
            (cepFinalCadastrado >= cepInicial && cepFinalCadastrado <= cepFinal) ||
            (cepInicial >= cepInicialCadastrado && cepInicial <= cepFinalCadastrado) ||
            (cepFinal >= cepInicialCadastrado && cepFinal <= cepFinalCadastrado))
            return true;
    })) {
        exibirMensagem(tipoMensagem.aviso, "Faixa de CEP já existe", "Esta faixa de CEP entrou em conflito com outra já cadastrada.");
        return;
    }

    ceps.push({ Codigo: guid(), CEPInicial: _regrasMultaAtrasoRetirada.CEPInicial.val(), CEPFinal: _regrasMultaAtrasoRetirada.CEPFinal.val() })

    _regrasMultaAtrasoRetirada.CEPs.val(ceps);

    RecarregarGridCEP();

    LimparCamposCEP();
}

function LimparCamposCEP() {
    _regrasMultaAtrasoRetirada.CEPInicial.val("");
    _regrasMultaAtrasoRetirada.CEPFinal.val("");
}