/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
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
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="CTe.js" />


var ComponentePrestacaoServico = function (cte) {

    var instancia = this;

    this.Grid = PropertyEntity({ type: types.local });

    this.Valor = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: Localization.Resources.Gerais.Geral.Valor.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.Componente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.Componente.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.IncluirBaseCalculoICMS = PropertyEntity({ text: Localization.Resources.CTes.CTe.IncluirNaBaseDeCalculoDoICMS, val: ko.observable(true), def: true, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(true) });
    this.IncluirTotalReceber = PropertyEntity({ text: Localization.Resources.CTes.CTe.IncluirNoTotalReceber, val: ko.observable(true), def: true, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(false) });
    this.DescontarTotalReceber = PropertyEntity({ text: Localization.Resources.CTes.CTe.DescontarNoTotalReceber, val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(false) });
    this.ExibirDescricaoComponente = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.DescricaoComponente = PropertyEntity({ text: Localization.Resources.CTes.CTe.ExibirOutraDescricaoParaEsseComponente, required: ko.observable(false), enable: ko.observable(false), maxlength: 15, visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: function () { instancia.AdicionarComponente() }, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {
        cte.Componentes = new Array();

        KoBindings(instancia, cte.IdKnockoutComponente);

        new BuscarComponentesDeFrete(instancia.Componente, instancia.RetornoConsultaComponenteFrete);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: instancia.Excluir }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "25%" },
            { data: "Valor", title: Localization.Resources.Gerais.Geral.Valor, width: "15%" },
            { data: "IncluirBaseCalculoICMS", title: Localization.Resources.CTes.CTe.IncluirNoICMS, width: "15%" },
            { data: "IncluirTotalReceber", title: Localization.Resources.CTes.CTe.IncluirNoTotalReceber, width: "15%" },
            { data: "DescontarTotalReceber", title: Localization.Resources.CTes.CTe.DescontarNoTotalReceber, width: "15%" }
        ];

        cte.GridComponente = new BasicDataTable(instancia.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);
        instancia.RecarregarGrid();
    };

    this.RetornoConsultaComponenteFrete = function (componente) {
        instancia.Componente.val(componente.Descricao);
        instancia.Componente.codEntity(componente.Codigo);
        //instancia.IncluirTotalReceber.val(componente.AcrescentaValorTotalAReceber);
        instancia.DescontarTotalReceber.val(componente.DescontarValorTotalAReceber);
        $("#" + instancia.Valor.id).focus();

        var incluirComponenteICMS = true;

        if (componente.TipoComponenteFrete == EnumTipoComponenteFrete.PEDAGIO && cte.DadosGeraisControleCTe != null && cte.DadosGeraisControleCTe.RegrasPedagioPorEstado != null) {
            for (var i = 0; i < cte.DadosGeraisControleCTe.RegrasPedagioPorEstado.length; i++) {
                if (cte.DadosGeraisControleCTe.RegrasPedagioPorEstado[i].Estado == cte.CTe.EstadoInicioPrestacao.val()) {
                    incluirComponenteICMS = cte.DadosGeraisControleCTe.RegrasPedagioPorEstado[i].IncluirBaseCalculoICMS;
                }
            }
        }

        instancia.IncluirBaseCalculoICMS.val(incluirComponenteICMS);
    };

    this.DestivarComponentePrestacaoServico = function () {
        DesabilitarCamposInstanciasCTe(instancia);
        cte.GridComponente.CarregarGrid(instancia.BuscarComponentes(), false);
    };

    this.AdicionarComponente = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {

            if (cte.Componentes.some(function (componente) { return componente.CodigoComponente == instancia.Componente.codEntity(); })) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.CTes.CTe.JaExisteUmComponenteParaEsteCTe.format(instancia.Componente.val()));
                return;
            }

            var valor = Globalize.parseFloat(instancia.Valor.val());

            if (instancia.DescontarTotalReceber.val())
                valor = -valor;

            cte.Componentes.push({
                Codigo: guid(),
                CodigoComponente: instancia.Componente.codEntity(),
                DescricaoComponente: !string.IsNullOrWhiteSpace(instancia.DescricaoComponente.val()) ? instancia.DescricaoComponente.val() : instancia.Componente.val(),
                Valor: Globalize.format(valor, "n2"),
                IncluirBaseCalculoICMS: instancia.IncluirBaseCalculoICMS.val(),
                IncluirTotalReceber: instancia.IncluirTotalReceber.val(),
                DescontarTotalReceber: instancia.DescontarTotalReceber.val()
            });

            instancia.RecarregarGrid();
            cte.TotalServico.AtualizarTotaisCTe();
            if (cte.CTe.Tipo.val() === EnumTipoCTe.Simplificado) {
                cte.EntregaSimplificado.AtualizarValoresComponentesPrestacaoServicoEntrega();
                cte.EntregaSimplificado.AtualizarValoresFreteEntregaGrid();
            }
            LimparCampos(instancia);

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    };

    this.Excluir = function (componente) {
        for (var i = 0; i < cte.Componentes.length; i++) {
            if (componente.Codigo == cte.Componentes[i].Codigo) {
                cte.Componentes.splice(i, 1);
                break;
            }
        }

        instancia.RecarregarGrid();
        cte.TotalServico.AtualizarTotaisCTe();
    };

    this.RecarregarGrid = function () {
        cte.GridComponente.CarregarGrid(instancia.BuscarComponentes());
    };

    this.BuscarComponentes = function () {
        var componentesGrid = new Array();
        for (var i = 0; i < cte.Componentes.length; i++) {
            var componente = cte.Componentes[i];
            componentesGrid.push({
                Codigo: componente.Codigo,
                Descricao: componente.DescricaoComponente,
                Valor: componente.Valor,
                IncluirBaseCalculoICMS: componente.IncluirBaseCalculoICMS ? "Sim" : "Não",
                IncluirTotalReceber: componente.IncluirTotalReceber ? "Sim" : "Não",
                DescontarTotalReceber: componente.DescontarTotalReceber ? "Sim" : "Não"
            });
        }
        return componentesGrid;
    };

    this.ExibirDescricaoComponente.val.subscribe(function (novoValor) {
        if (novoValor) {
            instancia.DescricaoComponente.required(true);
            instancia.DescricaoComponente.enable(true);
        } else {
            instancia.DescricaoComponente.enable(false);
            instancia.DescricaoComponente.required(false);
            instancia.DescricaoComponente.val("");
        }
    });
};

