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
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="CTe.js" />

var Motorista = function (cte) {

    var instancia = this;

    this.Grid = PropertyEntity({ type: types.local });

    this.CPF = PropertyEntity({ text: Localization.Resources.CTes.CTe.CPF.getRequiredFieldDescription(), getType: typesKnockout.cpf, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Nome = PropertyEntity({ text: Localization.Resources.CTes.CTe.Nome.getRequiredFieldDescription(), getType: typesKnockout.string, maxlength: 200, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.Motorista.getRequiredFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: function () { instancia.AdicionarMotorista() }, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {
        cte.Motoristas = new Array();

        KoBindings(instancia, cte.IdKnockoutMotorista);

        new BuscarMotoristasPorCPF(instancia.Motorista, function (motorista) {
            instancia.Nome.val(motorista.Nome);
            instancia.CPF.val(motorista.CPF);
        });

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: instancia.Excluir }] };

        var header = [{ data: "Codigo", visible: false },
            { data: "CPF", title: Localization.Resources.CTes.CTe.CPF, width: "25%" },
            { data: "Nome", title: Localization.Resources.CTes.CTe.Nome, width: "55%" }];

        cte.GridMotorista = new BasicDataTable(instancia.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

        instancia.RecarregarGrid();
    }

    this.DestivarMotorista = function () {
        DesabilitarCamposInstanciasCTe(instancia);
        cte.GridMotorista.CarregarGrid(cte.Motoristas, false);
    }

    this.AdicionarMotorista = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {

            cte.Motoristas.push({
                Codigo: guid(),
                CPF: instancia.CPF.val(),
                Nome: instancia.Nome.val()
            });

            instancia.RecarregarGrid();

            LimparCampos(instancia);

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    }

    this.Excluir = function (motorista) {
        for (var i = 0; i < cte.Motoristas.length; i++) {
            if (motorista.Codigo == cte.Motoristas[i].Codigo) {
                cte.Motoristas.splice(i, 1);
                break;
            }
        }

        instancia.RecarregarGrid();
    }

    this.Validar = function () {
        if (cte.Rodoviario.IndicadorLotacao.val() == true) {
            if (cte.Motoristas == null || cte.Motoristas.length <= 0) {
                $('a[href="#divModalRodoviario_' + cte.IdModal + '"]').tab("show");
                $('a[href="#knockoutMotorista_' + cte.IdModal + '"]').tab("show");
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.MotoristaObrigatorio, Localization.Resources.CTes.CTe.QuandoIndicadorDeLotacaoEstiverMarcadoNecessarioAdicionarAoMenosUmMotorista);
                return false;
            }
        }

        return true;
    }

    this.RecarregarGrid = function () {
        cte.GridMotorista.CarregarGrid(cte.Motoristas);
    }
}