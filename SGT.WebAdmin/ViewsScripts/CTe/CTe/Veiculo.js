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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="CTe.js" />

var Veiculo = function (cte) {
    var instancia = this;

    this.Grid = PropertyEntity({ type: types.local });

    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.Veiculo.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: function () { instancia.AdicionarVeiculo() }, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });

    this.AdicionarVeiculo = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {

            executarReST("Veiculo/ObterDetalhesPorVeiculo", { Codigo: instancia.Veiculo.codEntity() }, function (r) {

                if (r.Success) {
                    if (!cte.Veiculos.some(function (item) { return item.Placa == r.Data.Placa; })) {
                        cte.Veiculos.push({
                            Codigo: guid(),
                            CodigoVeiculo: r.Data.CodigoVeiculo,
                            Placa: r.Data.Placa,
                            Estado: r.Data.Estado,
                            RENAVAM: r.Data.RENAVAM,
                            Propriedade: r.Data.Propriedade,
                            Rodado: r.Data.Rodado,
                            Carroceria: r.Data.Carroceria
                        });
                    }

                    for (var i = 0; i < r.Data.VeiculosVinculados.length; i++) {
                        var veiculoVinculado = r.Data.VeiculosVinculados[i];

                        if (!cte.Veiculos.some(function (item) { return item.Placa == veiculoVinculado.Placa; })) {
                            cte.Veiculos.push({
                                Codigo: guid(),
                                CodigoVeiculo: veiculoVinculado.CodigoVeiculo,
                                Placa: veiculoVinculado.Placa,
                                Estado: veiculoVinculado.Estado,
                                RENAVAM: veiculoVinculado.RENAVAM,
                                Propriedade: veiculoVinculado.Propriedade,
                                Rodado: veiculoVinculado.Rodado,
                                Carroceria: veiculoVinculado.Carroceria
                            });
                        }
                    }

                    if (r.Data.Motorista != null) {
                        if (!cte.Motoristas.some(function (item) { return item.CPF == r.Data.Motorista.CPF; })) {
                            cte.Motoristas.push({
                                Codigo: guid(),
                                CPF: r.Data.Motorista.CPF,
                                Nome: r.Data.Motorista.Nome
                            });
                        }

                        cte.Motorista.RecarregarGrid();
                    }

                    instancia.RecarregarGrid();

                    LimparCampos(instancia);

                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                }
            });

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    }

    this.Load = function () {
        cte.Veiculos = new Array();

        KoBindings(instancia, cte.IdKnockoutVeiculo);

        new BuscarVeiculos(instancia.Veiculo);

        $("#" + instancia.Veiculo.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: instancia.Excluir }] };

        var header = [{ data: "Codigo", visible: false },
        { data: "CodigoVeiculo", visible: false },
        { data: "Placa", title: Localization.Resources.CTes.CTe.Placa, width: "14%" },
        { data: "Estado", title: Localization.Resources.CTes.CTe.UF, width: "10%" },
        { data: "RENAVAM", title: Localization.Resources.CTes.CTe.RENAVAM, width: "14%" },
        { data: "Propriedade", title: Localization.Resources.CTes.CTe.Propriedade, width: "14%" },
        { data: "Rodado", title: Localization.Resources.CTes.CTeRodado, width: "14%" },
        { data: "Carroceria", title: Localization.Resources.CTes.CTe.Carroceria, width: "14%" }];

        cte.GridVeiculo = new BasicDataTable(instancia.Grid.id, header, menuOpcoes, { column: 2, dir: orderDir.asc }, null, 5);

        instancia.RecarregarGrid();
    }

    this.DestivarVeiculo = function () {
        DesabilitarCamposInstanciasCTe(instancia);
        cte.GridVeiculo.CarregarGrid(cte.Veiculos, false);
    }

    this.Excluir = function (veiculo) {
        for (var i = 0; i < cte.Veiculos.length; i++) {
            if (veiculo.Codigo == cte.Veiculos[i].Codigo) {
                cte.Veiculos.splice(i, 1);
                break;
            }
        }

        instancia.RecarregarGrid();
    }

    this.Validar = function () {
        if (cte.Rodoviario.IndicadorLotacao.val() == true) {
            if (cte.Veiculos == null || cte.Veiculos.length <= 0) {
                $('a[href="#divModalRodoviario_' + cte.IdModal + '"]').tab("show");
                $('a[href="#knockoutVeiculo_' + cte.IdModal + '"]').tab("show");
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.VeículoObrigatorio, Localization.Resources.CTes.CTe.QuandoIndicadorDeLotacaoEstiverMarcadoNecessarioAdicionarAoMenosUmVeiculo);
                return false;
            }
        }

        return true;
    }

    this.RecarregarGrid = function () {
        cte.GridVeiculo.CarregarGrid(cte.Veiculos);
    }
}