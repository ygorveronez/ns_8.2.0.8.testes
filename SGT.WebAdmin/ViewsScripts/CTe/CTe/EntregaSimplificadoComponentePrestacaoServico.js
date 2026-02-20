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
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Atividade.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="CTe.js" />

var EntregaSimplificadoComponentePrestacaoServico = function (cte) {
    var instancia = this;

    this.GridComponentePrestacaoServico = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoLocalidadeOrigem = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoLocalidadeDestino = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Valor = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: Localization.Resources.Gerais.Geral.Valor.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 20, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.Componente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.Componente.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, visible: ko.observable(false), enable: ko.observable(false) });
    this.IncluirBaseCalculoICMS = PropertyEntity({ text: Localization.Resources.CTes.CTe.IncluirNaBaseDeCalculoDoICMS, val: ko.observable(true), def: true, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(false) });
    this.IncluirTotalReceber = PropertyEntity({ text: Localization.Resources.CTes.CTe.IncluirNoTotalReceber, val: ko.observable(true), def: true, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(false) });
    this.DescontarTotalReceber = PropertyEntity({ text: Localization.Resources.CTes.CTe.DescontarNoTotalReceber, val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(false) });
    this.ExibirDescricaoComponente = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.DescricaoComponente = PropertyEntity({ text: Localization.Resources.CTes.CTe.ExibirOutraDescricaoParaEsseComponente, required: ko.observable(false), enable: ko.observable(false), maxlength: 15, visible: ko.observable(false) });

    this.AdicionarComponentePrestacaoServico = PropertyEntity({ eventClick: function () { instancia.AdicionarComponentePrestacaoServicoSimplificado(); }, type: types.event, text: "Salvar Componente", visible: ko.observable(false), enable: ko.observable(false) });

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

    this.Load = function () {

        cte.EntregasSimplificadoComponentesPrestacaoServico = new Array();

        KoBindings(instancia, cte.IdKnockoutEntregaSimplificadoComponentePrestacaoServico);

        new BuscarComponentesDeFrete(instancia.Componente, instancia.RetornoConsultaComponenteFrete);

        cte.GridComponentePrestacaoServico = new BasicDataTable(instancia.GridComponentePrestacaoServico.id, headerComponentesPrestacaoServicoSimplificado(), menuOpcoesComponentesPrestacaoServicoCTeSimplificado(instancia), { column: 1, dir: orderDir.asc }, null, 5);

        instancia.RecarregarGridSimplificadoComponentesPrestacaoServico();
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

    this.AdicionarComponentePrestacaoServicoSimplificado = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {
            if (instancia.Codigo.val() > "0" && instancia.Codigo.val() != undefined) {
                for (var i = 0; i < cte.EntregasSimplificadoComponentesPrestacaoServico.length; i++) {
                    if (instancia.Codigo.val() == cte.EntregasSimplificadoComponentesPrestacaoServico[i].Codigo) {
                        cte.EntregasSimplificadoComponentesPrestacaoServico.splice(i, 1);
                        break;
                    }
                }
            }

            instancia.Codigo.val(guid());
            instancia.CodigoLocalidadeOrigem.val(cte.EntregaSimplificado.LocalidadeOrigem.codEntity());
            instancia.CodigoLocalidadeDestino.val(cte.EntregaSimplificado.LocalidadeDestino.codEntity());

            cte.EntregasSimplificadoComponentesPrestacaoServico.push(RetornarObjetoPesquisa(instancia));

            instancia.RecarregarGridSimplificadoComponentesPrestacaoServico();

            LimparCampos(instancia);

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    };

    this.ExcluirComponentePrestacaoServicoSimplificado = function (documento) {
        if (instancia.Codigo.val() > 0) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.DocumentoEmEdicao, Localization.Resources.CTes.CTe.PorFavorVerifiqueOsDocumentosPoisExisteUmEmEdicao);
            return;
        }

        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.CTes.CTe.RealmenteDesejaExcluirDocumento, function () {
            for (var i = 0; i < cte.EntregasSimplificadoComponentesPrestacaoServico.length; i++) {
                if (documento.Codigo == cte.EntregasSimplificadoComponentesPrestacaoServico[i].Codigo) {
                    cte.EntregasSimplificadoComponentesPrestacaoServico.splice(i, 1);
                    break;
                }
            }

            instancia.RecarregarGridSimplificadoComponentesPrestacaoServico();
        });
    };

    this.RecarregarGridSimplificadoComponentesPrestacaoServico = function () {
        cte.GridComponentePrestacaoServico.CarregarGrid(instancia.BuscarComponentesPrestacaoServicoSimplificado(), cte.EntregaSimplificado.PermitirEdicao.val());
    };

    this.BuscarComponentesPrestacaoServicoSimplificado = function () {
        var data = new Array();

        $.each(cte.EntregasSimplificadoComponentesPrestacaoServico, function (i, componente) {
            if (cte.EntregaSimplificado.LocalidadeOrigem.codEntity() === componente.CodigoLocalidadeOrigem && cte.EntregaSimplificado.LocalidadeDestino.codEntity() === componente.CodigoLocalidadeDestino) {
                var documentoGrid = new Object();

                documentoGrid.Codigo = componente.Codigo;
                documentoGrid.CodigoLocalidadeOrigem = componente.CodigoLocalidadeOrigem;
                documentoGrid.CodigoLocalidadeDestino = componente.CodigoLocalidadeDestino;
                documentoGrid.CodigoComponente = componente.CodigoComponente;
                documentoGrid.Descricao = componente.DescricaoComponente;
                documentoGrid.Valor = Globalize.format(componente.Valor, "n2");
                documentoGrid.IncluirBaseCalculoICMS = componente.IncluirBaseCalculoICMS ? "Sim" : "Não";
                documentoGrid.IncluirTotalReceber = componente.IncluirTotalReceber ? "Sim" : "Não";
                documentoGrid.DescontarTotalReceber = componente.DescontarTotalReceber ? "Sim" : "Não";

                data.push(documentoGrid);
            }
        });
        return data;
    };

    this.EditarComponentePrestacaoServicoSimplificado = function (componente) {
        cte.EntregaSimplificadoComponentePrestacaoServico.Codigo.val(componente.Codigo);
        cte.EntregaSimplificadoComponentePrestacaoServico.DescricaoComponente.val(componente.Descricao);
        cte.EntregaSimplificadoComponentePrestacaoServico.Valor.val(Globalize.format(componente.Valor, "n2"));
        cte.EntregaSimplificadoComponentePrestacaoServico.IncluirBaseCalculoICMS.val(componente.IncluirBaseCalculoICMS);
        cte.EntregaSimplificadoComponentePrestacaoServico.IncluirTotalReceber.val(componente.IncluirTotalReceber);
        cte.EntregaSimplificadoComponentePrestacaoServico.DescontarTotalReceber.val(componente.DescontarTotalReceber);
    };
};

function headerComponentesPrestacaoServicoSimplificado() {

    return [
        { data: "Codigo", visible: false },
        { data: "CodigoLocalidadeOrigem", visible: false },
        { data: "CodigoLocalidadeDestino", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "25%" },
        { data: "Valor", title: Localization.Resources.Gerais.Geral.Valor, width: "15%" },
        { data: "IncluirBaseCalculoICMS", title: Localization.Resources.CTes.CTe.IncluirNoICMS, width: "15%" },
        { data: "IncluirTotalReceber", title: Localization.Resources.CTes.CTe.IncluirNoTotalReceber, width: "15%" },
        { data: "DescontarTotalReceber", title: Localization.Resources.CTes.CTe.DescontarNoTotalReceber, width: "15%" }
    ];
}

function menuOpcoesComponentesPrestacaoServicoCTeSimplificado(local) {
    var editarItem = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: local.EditarComponentePrestacaoServicoSimplificado };
    var excluirItem = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: local.ExcluirComponentePrestacaoServicoSimplificado };

    //return menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [editarItem, excluirItem  ] };
    return null;
}