/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../js/Global/Rest.js" />
/// <reference path="../../js/Global/Mensagem.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/bootstrap/bootstrap.js" />
/// <reference path="../../js/libs/jquery.blockui.js" />
/// <reference path="../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../Configuracao/Sistema/ConfiguracaoTMS.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridAlertaLicencaTMS;


//*******EVENTOS*******

function LoadAlertaLicencaTMS() {
    $("#knockoutAlertaLicenca").hide();
    CarregarAlertasLicencasTMS();
}

function CarregarAlertasLicencasTMS() {
    executarReST("ControleAlerta/CarregarAlertasLicencas", null, function (arg) {
        if (arg.Success) {
            if (arg.Data) {

                var ocultar = {
                    descricao: "Ocultar", id: guid(), evento: "onclick", metodo: function (data) {
                        OcultarLicencaTMSClick(data)
                    }, tamanho: "10", icone: "fa-check-square-o"
                };
                var menuOpcoes = new Object();
                menuOpcoes.tipo = TypeOptionMenu.link;
                menuOpcoes.opcoes = new Array();
                menuOpcoes.opcoes.push(ocultar);

                var header = [{ data: "Codigo", visible: false },
                { data: "Descricao", title: "Descrição", width: "60%", className: "text-align-left" },
                { data: "Data", title: "Data", width: "20%", className: "text-align-center" }
                ];

                _gridAlertaLicencaTMS = new BasicDataTable("gridAlertasTMS", header, menuOpcoes, null, null, 10);

                var data = new Array();
                $.each(arg.Data, function (i, listaAlerta) {
                    var tarefa = new Object();

                    tarefa.Codigo = listaAlerta.Codigo;
                    tarefa.Descricao = listaAlerta.Descricao;
                    tarefa.Data = listaAlerta.Data;

                    data.push(tarefa);
                });

                _gridAlertaLicencaTMS.CarregarGrid(data);
                if (data.length > 0)
                    $("#knockoutAlertaLicenca").show();

            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
        }
    });
}

function OcultarLicencaTMSClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja ocultar o alerta " + data.Descricao + "?", function () {
        var dataEnvio = {
            Codigo: data.Codigo
        };
        executarReST("ControleAlerta/OcultarAlerta", dataEnvio, function (arg) {
            if (!arg.Success) {
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            } else {
                CarregarAlertasLicencasTMS();
            }
        });
    });
}