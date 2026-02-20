/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="JanelaCarregamentoTransportador.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _placaDisponivel;
var _gridPlacasDisponiveis;

/*
 * Declaração das Classes
 */

var PlacasDisponiveis = function () {
    this.PlacaDisponivel = PropertyEntity({ text: "Placa:", idGrid: guid(), required: true });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", val: ko.observable(""), def: "", idBtnSearch: guid() });

    this.AdicionarPlacaDisponivel = PropertyEntity({ eventClick: adicionarPlacaDisponivelClick, type: types.event, text: "Confirmar", idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridPlacasDisponiveis() {
    var excluir = { descricao: "Excluir", id: "clasEditar", evento: "onclick", metodo: excluirVeiculoDisponivelClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    _gridPlacasDisponiveis = new GridView(_placaDisponivel.PlacaDisponivel.idGrid, "JanelaCarregamentoTransportador/PesquisaVeiculosDisponiveis", _placaDisponivel, menuOpcoes, null, null, null, false);
    _gridPlacasDisponiveis.CarregarGrid();
}

function loadPlacasDisponiveis() {
    $.get("Content/Static/Logistica/VeiculosDisponiveis.html?dyn=" + guid(), function (data) {
        $("#" + _pesquisaJanelaCarregamentoTransportador.Requisicao.idTab).html(data);

        _placaDisponivel = new PlacasDisponiveis();
        KoBindings(_placaDisponivel, "knoutPlacasDisponiveis");

        loadGridPlacasDisponiveis();

        $("#" + _placaDisponivel.PlacaDisponivel.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });

        new BuscarVeiculos(_placaDisponivel.Veiculo, function (arg) {
            _placaDisponivel.PlacaDisponivel.val(arg.Placa);
        });

        $('#knoutPlacasDisponiveis').keypress(function (e) {
            var keyCode = e.keyCode || e.which;

            if (keyCode == 13)
                $('#' + _placaDisponivel.AdicionarPlacaDisponivel.id).trigger('click');
        });
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarPlacaDisponivelClick() {
    var dadosPlacaDisponivel = {
        PlacaDisponivel: _placaDisponivel.PlacaDisponivel.val(),
        Veiculo: _placaDisponivel.Veiculo.codEntity(),
        SelecaoQualquerVeiculoConfirmada: false
    }

    adicionarPlacaDisponivel(dadosPlacaDisponivel);
}

function excluirVeiculoDisponivelClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja indisponibilizar o veículo " + row.Placa + "?", function () {
        executarReST("JanelaCarregamentoTransportador/IndisponibilizarVeiculo", { Codigo: row.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "sucesso", "Indisponibilizado com sucesso.");
                    _gridPlacasDisponiveis.CarregarGrid();
                    limparCamposPlacaDisponivel();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

/*
 * Declaração das Funções Públicas
 */

function adicionarPlacaDisponivel(dadosPlacaDisponivel) {
    executarReST("JanelaCarregamentoTransportador/AdicionarVeiculoDisponivel", dadosPlacaDisponivel, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Msg) {
                    exibirConfirmacao("Confirmação", retorno.Msg + " Deseja realmente continuar?", function () {
                        dadosPlacaDisponivel.SelecaoQualquerVeiculoConfirmada = true
                        adicionarPlacaDisponivel(dadosPlacaDisponivel);
                    });

                    return;
                }

                exibirMensagem(tipoMensagem.ok, "sucesso", "Liberado com sucesso.");
                _gridPlacasDisponiveis.CarregarGrid();
                limparCamposPlacaDisponivel();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function limparCamposPlacaDisponivel() {
    LimparCampos(_placaDisponivel);
}
