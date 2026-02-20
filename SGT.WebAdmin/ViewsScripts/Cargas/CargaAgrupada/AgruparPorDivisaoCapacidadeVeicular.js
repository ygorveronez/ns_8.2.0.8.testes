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
/// <reference path="../../Enumeradores/EnumSituacaoCarregamento.js" />
/// <reference path="cargaagrupada.js" />
/// <reference path="../../Consultas/Tranportador.js" />

var _agruparDivisoesCapacidadeModeloVeicular;
var _arrayDivisoesModeloVeicular = new Array();

var AgruparPorDivisaoCapacidadeModeloVeicular = function () {
    this.DivisoesCapacidadeVeicular = ko.observableArray([]);
};


var DivisaoCapacidadeVeicular = function (divisaoCapacidade) {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ val: ko.observable(""), def: "" });
    this.Capacidade = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal });
    this.CapacidadeOcupada = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal });

    this.GridCargasAgrupar = PropertyEntity({ type: types.local });

    PreencherObjetoKnout(this, { Data: divisaoCapacidade });
};


function loadAgruparPorDivisaoCapacidade() {
    _agruparDivisoesCapacidadeModeloVeicular = new AgruparPorDivisaoCapacidadeModeloVeicular();
    KoBindings(_agruparDivisoesCapacidadeModeloVeicular, "knockoutDivisaoCapacidade");
}


function BuscarDadosDivisaoCapacidadeVeiculo(codVeiculo) {
    var p = new promise.Promise();
    var data = new Object();
    data.codigoVeiculo = codVeiculo;

    executarReST("CargaAgrupada/ObterDivisoesCapacidadeVeicular", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                if (arg.Data != undefined) {
                    preencherKnoutDivisaoCapacidade(arg.Data);
                    $("#knockoutDivisaoCapacidade").show();
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }

        p.done();
    });

    return p;
}



function preencherKnoutDivisaoCapacidade(data) {

    if (data.DivisaoCapacidadeVeicular == null || data.DivisaoCapacidadeVeicular.length == 0) {
        $("#divNenhumRegistroEncontrado").show();
        $("#divResumoCapacidade").hide();
    }

    var divisoes = data.DivisaoCapacidadeVeicular;

    if (divisoes != undefined && Array.isArray(divisoes) && divisoes.length > 0) {
        divisoes.forEach(function (divisao, i) {
            var data = new Array();
            var knoutDivisaoVeicular = new DivisaoCapacidadeVeicular(divisao);
            _agruparDivisoesCapacidadeModeloVeicular.DivisoesCapacidadeVeicular.push(knoutDivisaoVeicular);

            CriarHTMLDinamicoResumoCapacidade(divisao);

            var excluir = {
                descricao: "Excluir", id: guid(), metodo: function (carga) { excluirCargasDivisaoSelecionadasClick(gridCargasDivisaoAgrupar, carga); }
            };

            var menuOpcoes = {
                tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [excluir]
            };

            var header = [
                { data: "Codigo", visible: false },
                { data: "CodigoCargaEmbarcador", title: "Número", width: "20%" },
                { data: "OrigemDestino", title: "Origem e Destino", width: "35%" },
                { data: "ZonaTransporte", title: "Zona de Transporte", width: "15%", visible: true },
                { data: "Ordem", title: "Ordem", width: "5%", visible: true },
                { data: "Veiculo", title: "Veículo", width: "15%" }
            ];

            var ordenacao = { column: 4, dir: orderDir.asc };
            var gridCargasDivisaoAgrupar = new BasicDataTable(knoutDivisaoVeicular.GridCargasAgrupar.id, header, menuOpcoes, ordenacao, null, null, null, true, null, function (retornoOrdenacao) { callbackOrdenacaoDivisao(gridCargasDivisaoAgrupar, retornoOrdenacao); });
            gridCargasDivisaoAgrupar.CarregarGrid(data);

            _arrayDivisoesModeloVeicular.push(knoutDivisaoVeicular);

            $("#grid-" + knoutDivisaoVeicular.GridCargasAgrupar.id).show();
            $("#grid-" + knoutDivisaoVeicular.GridCargasAgrupar.id).droppable({
                drop: function (event, ui) {
                    var id = parseInt(ui.draggable[0].id);

                    var arrastando = true;
                    if (ui.helper[0].className == "ui-sortable-helper")
                        arrastando = false;

                    droppableCargaDivisaoCapacidade(id, gridCargasDivisaoAgrupar, knoutDivisaoVeicular.Codigo.val(), arrastando);
                },
                hoverClass: "ui-state-active bgPreCarga"
            });
        });

        $("#divResumoCapacidade").show();
    }
    else
        _agruparDivisoesCapacidadeModeloVeicular.DivisoesCapacidadeVeicular([]);
}

function droppableCargaDivisaoCapacidade(idCarga, grid, codigoDivisao, arrastando) {
    setTimeout(function () {
        var dataRow = _gridCarga.obterDataRow(idCarga);

        if (dataRow)
            adicionarCargaNaDivisaoCapacidade(dataRow.data, true, grid, codigoDivisao, arrastando);
    }, 50);
}

function adicionarCargaNaDivisaoCapacidade(carga, atualizar, grid, codigoDivisao, arrastando) {
    if (arrastando) {
        if (validarCarga(carga)) {

            carga.DivisaoModeloVeicular = codigoDivisao;

            if ((carga.PlacaDeAgrupamento) && (carga.PlacaDeAgrupamento != ""))
                carga.Placa = carga.PlacaDeAgrupamento;

            var recarregar = false;
            if (atualizar && !_CONFIGURACAO_TMS.PermitirAlterarInformacoesAgrupamentoCarga) {
                if (_cargaAgrupada.Empresa.codEntity() == 0 && carga.EmpresaCodigo > 0) {
                    _cargaAgrupada.Empresa.codEntity(carga.EmpresaCodigo);
                    _cargaAgrupada.Empresa.val(carga.Transportador);
                    recarregar = true;
                }
            }

            $.each(_arrayDivisoesModeloVeicular, function (i, divisao) {
                if (divisao.Codigo.val() == codigoDivisao) {
                    var capacidadeOcupada = divisao.CapacidadeOcupada.val();
                    if (carga.PesoTotal != "")
                        capacidadeOcupada += parseFloat(carga.PesoTotal.replace(".", "").replace(",", "."));

                    if (capacidadeOcupada < 0)
                        capacidadeOcupada = 0;

                    divisao.CapacidadeOcupada.val(capacidadeOcupada);
                    $("#capacidade_" + codigoDivisao).text(" " + capacidadeOcupada.toFixed(2));
                }
            });

            var count = 1;
            $.each(_listaCargasAgrupadas, function (i, cargaAgrupada) {
                if (cargaAgrupada.DivisaoModeloVeicular == codigoDivisao) {
                    count += 1;
                }
            });
            carga.Ordem = count;

            _listaCargasAgrupadas.push(carga);
            recarregarGridNovaCargaDivisao(grid, codigoDivisao);

            if (recarregar)
                _gridCarga.CarregarGrid();

            _gridCarga.setarCorGridPorID(carga.Codigo, !carga.CargaDePreCarga ? _cargaAgrupadaCorAdicionado : _preCargaAgrupadaCorAdicionado);

            return true;

        } else {
            return false;
        }
    }
}

function recarregarGridNovaCargaDivisao(grid, codigoDivisao) {
    var data = new Array();
    $.each(_listaCargasAgrupadas, function (i, cargaAgrupada) {
        if (cargaAgrupada.DivisaoModeloVeicular == codigoDivisao) {
            cargaAgrupada.DT_RowColor = !cargaAgrupada.CargaDePreCarga ? "" : "#D3D3D3";
            data.push(cargaAgrupada);
        }
    });

    grid.CarregarGrid(data);
}

function recarregarGridDivisoes(grid, codigoDivisao) {
    var data = new Array();

    $.each(_listaCargasAgrupadas, function (i, cargaAgrupada) {
        if (cargaAgrupada.DivisaoModeloVeicular == codigoDivisao) {
            cargaAgrupada.DT_RowColor = !cargaAgrupada.CargaDePreCarga ? "" : "#D3D3D3";
            data.push(cargaAgrupada);
        }
    });

    grid.CarregarGrid(data);
}

function excluirCargasDivisaoSelecionadasClick(grid, carga) {

    $.each(_listaCargasAgrupadas, function (i, cargasSelecionadas) {
        if (carga.Codigo == cargasSelecionadas.Codigo) {
            _listaCargasAgrupadas.splice(i, 1);
            _gridCarga.setarCorGridPorID(carga.Codigo, !carga.CargaDePreCarga ? "" : "#D3D3D3");

            $.each(_arrayDivisoesModeloVeicular, function (i, divisao) {
                if (divisao.Codigo.val() == carga.DivisaoModeloVeicular) {
                    var capacidadeOcupada = divisao.CapacidadeOcupada.val();
                    if (carga.PesoTotal != "")
                        capacidadeOcupada -= parseFloat(carga.PesoTotal.replace(".", "").replace(",", "."));

                    if (capacidadeOcupada < 0)
                        capacidadeOcupada = 0;

                    divisao.CapacidadeOcupada.val(capacidadeOcupada);
                    $("#capacidade_" + carga.DivisaoModeloVeicular).text(" " + capacidadeOcupada.toFixed(2));
                }
            });

            return false;
        }
    });

    recarregarGridDivisoes(grid, carga.DivisaoModeloVeicular);
}


function CriarHTMLDinamicoResumoCapacidade(divisaoCapacidade) {
    var html = '';

    html += '<div>';
    html += '<h5>' + divisaoCapacidade.Descricao + '</h5>';
    html += '    <p class="mb-1"><span class="text-primary fw-500">Capacidade:</span><span> ' + divisaoCapacidade.Capacidade + '</span></p>';
    html += '    <p class="mb-1"><span class="text-danger fw-500">Cap. Ocupada:</span><span class="capacidade-carregamento-valor" id="capacidade_' + divisaoCapacidade.Codigo + '"> ' + divisaoCapacidade.Capacidade + '</span></p>';
    html += '</div>';

    $('#divResumoCapacidadeDados').append(html);
}

function callbackOrdenacaoDivisao(grid, retornoOrdenacao) {
    var listaRegistros = grid.BuscarRegistros();
    var listaRegistrosReordenada = [];

    for (var i = 0; i < retornoOrdenacao.listaRegistrosReordenada.length; i++) {
        var registroReordenado = retornoOrdenacao.listaRegistrosReordenada[i];

        for (var j = 0; j < listaRegistros.length; j++) {
            var registro = listaRegistros[j];

            if (registro.DT_RowId == registroReordenado.idLinha) {
                registro.Ordem = registroReordenado.posicao;
                listaRegistrosReordenada.push(registro);
                break;
            }
        }
    }

    //odernar a lista geral de cargas agrupadas (a ordem é por DivisaoModeloVeicular)
    for (var y = 0; y < listaRegistrosReordenada.length; y++) {
        var reordenado = listaRegistrosReordenada[y];

        for (var j = 0; j < _listaCargasAgrupadas.length; j++) {
            var carga = _listaCargasAgrupadas[j];

            if (carga.Codigo == reordenado.Codigo) {
                carga.Ordem = reordenado.Ordem;
                break;
            }
        }
    }

    grid.CarregarGrid(listaRegistrosReordenada);
}
