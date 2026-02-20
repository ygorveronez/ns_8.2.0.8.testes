/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _vistoriaRecebimento;

var VistoriaRecebimento = function () {
    this.Categorias = ko.observableArray([]);
}


//*******EVENTOS*******
function LoadVistoriaRecebimento() {
    _vistoriaRecebimento = new VistoriaRecebimento();
    KoBindings(_vistoriaRecebimento, "knockoutVistoriaRecebimento");

    var data = [
        {
            Descricao: "Motorista",
            Perguntas: [
                {
                    Descricao: "1-Alguma avaria Constatada?",
                    Codigo: 1,
                    Opcao: false,
                    Tipo: 1, // SimNao
                },
                {
                    Descricao: "Manutenção programada na O.S. Nº:",
                    Codigo: 2,
                    Tipo: 3, // Informativo
                    Resposta: "12345"
                },
                {
                    Descricao: "Tipo de Avaria:",
                    Codigo: 3,
                    Tipo: 2, // Opcoes
                    Alternativas: [
                        { Codigo: 17, Marcado: true, Descricao: "1-Amassado" },
                        { Codigo: 18, Marcado: false, Descricao: "2-Arranhado / Riscado" },
                        { Codigo: 19, Marcado: true, Descricao: "3-Quebrado / Trincado" },
                        { Codigo: 20, Marcado: false, Descricao: "4-Outros:" }
                    ]
                },
                {
                    Descricao: "Onde:",
                    Codigo: 4,
                    Tipo: 2, // Opcoes
                    Alternativas: [
                        { Codigo: 1, Marcado: false, Descricao: "1-FR1" },
                        { Codigo: 2, Marcado: false, Descricao: "2-FR2" },
                        { Codigo: 3, Marcado: false, Descricao: "3-FR3" },
                        { Codigo: 4, Marcado: false, Descricao: "4-FR4" },

                        { Codigo: 5, Marcado: false, Descricao: "5-TR1" },
                        { Codigo: 6, Marcado: false, Descricao: "6-TR2" },
                        { Codigo: 7, Marcado: false, Descricao: "7-TR3" },
                        { Codigo: 8, Marcado: false, Descricao: "8-TR4" },

                        { Codigo: 9, Marcado: false, Descricao: "9-LF1" },
                        { Codigo: 10, Marcado: false, Descricao: "10-LF2" },
                        { Codigo: 11, Marcado: false, Descricao: "11-LF3" },
                        { Codigo: 12, Marcado: false, Descricao: "12-LF4" },

                        { Codigo: 13, Marcado: false, Descricao: "13-LC1" },
                        { Codigo: 14, Marcado: false, Descricao: "14-LC2" },
                        { Codigo: 15, Marcado: false, Descricao: "15-LC3" },
                        { Codigo: 16, Marcado: false, Descricao: "16-LC4" },
                    ]
                },
                {
                    Descricao: "2-Necessidade de limpeza / higienização da cabine / baú?",
                    Codigo: 5,
                    Opcao: false,
                    Tipo: 1, // SimNao
                },
                {
                    Descricao: "Manutenção programada na O.S. Nº:",
                    Codigo: 6,
                    Tipo: 3, // Informativo
                    Resposta: ""
                },
            ]
        },
        {
            Descricao: "Manutenção",
            Perguntas: [
                {
                    Descricao: "3-Vencimento de prazos de manutenção preventiva?",
                    Codigo: 7,
                    Opcao: false,
                    Tipo: 1, // SimNao
                },
                {
                    Descricao: "Manutenção programada na O.S. Nº:",
                    Codigo: 8,
                    Tipo: 3, // Informativo
                    Resposta: ""
                },
                {
                    Descricao: "",
                    Codigo: 9,
                    Tipo: 2, // Opcoes
                    Alternativas: [
                        { Codigo: 21, Marcado: false, Descricao: "1-Veículo (Garantia)" },
                        { Codigo: 22, Marcado: false, Descricao: "2-Veículo (Planejamento de Manutenção)" },
                        { Codigo: 23, Marcado: false, Descricao: "3-Equip. de Regrigeração (Garantia)" },
                        { Codigo: 24, Marcado: false, Descricao: "4-Equip. de Regrigeração (Planejamento de Manutenção)" },
                        { Codigo: 23, Marcado: false, Descricao: "5-Bateria (Garantia)" },
                        { Codigo: 24, Marcado: false, Descricao: "6-Bateria (Planejamento de Manutenção)" },
                        { Codigo: 25, Marcado: false, Descricao: "7-Desintetização" },
                        { Codigo: 26, Marcado: false, Descricao: "8-Rodízio de pneus" },
                        { Codigo: 27, Marcado: false, Descricao: "9-Outros:" },
                    ]
                },
                {
                    Descricao: "4-Necessidade de realimentação / abastecimento de:",
                    Codigo: 10,
                    Tipo: 2, // Opcoes
                    Alternativas: [
                        { Codigo: 28, Marcado: false, Descricao: "1-Combustível" },
                        { Codigo: 29, Marcado: false, Descricao: "2-Óleo Lubrificante" },
                        { Codigo: 30, Marcado: false, Descricao: "3-Água" },
                        { Codigo: 31, Marcado: false, Descricao: "4-Calibração dos Pneus" },
                        { Codigo: 32, Marcado: false, Descricao: "5-Outros:" },
                        
                    ]
                },
                {
                    Descricao: "5-Necessidade de ajustes:",
                    Codigo: 11,
                    Tipo: 2, // Opcoes
                    Alternativas: [
                        { Codigo: 33, Marcado: false, Descricao: "1-Direção" },
                        { Codigo: 34, Marcado: false, Descricao: "2-Elétricos" },
                        { Codigo: 35, Marcado: false, Descricao: "3-Eletrônicos" },
                        { Codigo: 36, Marcado: false, Descricao: "4-Embreagem" },
                        { Codigo: 37, Marcado: false, Descricao: "5-Ferios" },
                        { Codigo: 38, Marcado: false, Descricao: "6-Outros:" },

                    ]
                },
            ]
        }
    ];
}

//*******METODOS*******
function EditarVistoriaRecebimento(data) {
}

function LimparVistoriaRecebimento() {
    _vistoriaRecebimento.Categorias([]);
}

function GetSetCheckList() {
    if (arguments.length == 0)
        return GetCheckList();
    else
        SetCheckList(arguments[0]);
}


function GetCheckList() {
    return JSON.stringify(_vistoriaRecebimento.Categorias());
}
function SetCheckList(data) {
    _vistoriaRecebimento.Categorias(data.slice());
}