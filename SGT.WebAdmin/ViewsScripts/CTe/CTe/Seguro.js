/// <reference path="../../Enumeradores/EnumResponsavelSeguro.js" />
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
/// <reference path="../../Enumeradores/EnumResponsavelSeguroCTe.js" />
/// <reference path="CTe.js" />

var Seguro = function (cte) {

    var instancia = this;

    this.Grid = PropertyEntity({ type: types.local });

    this.Responsavel = PropertyEntity({ val: ko.observable(0), def: 0, options: EnumResponsavelSeguroCTe.obterOpcoes(), text: Localization.Resources.CTes.CTe.Responsavel.getRequiredFieldDescription(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.CNPJSeguradora = PropertyEntity({ text: Localization.Resources.CTes.CTe.CNPJSeguradora.getRequiredFieldDescription(), required: true, getType: typesKnockout.cnpj, visible: ko.observable(true), enable: ko.observable(true) });
    this.ConsultarCNPJSeguradora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.CTes.CTe.Buscar, idBtnSearch: guid(), enable: ko.observable(true) });
    this.Seguradora = PropertyEntity({ text: Localization.Resources.CTes.CTe.Seguradora.getFieldDescription(), maxlength: 30, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroApolice = PropertyEntity({ text: Localization.Resources.CTes.CTe.NumeroDaApolice.getFieldDescription(), maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroAverbacao = PropertyEntity({ text: Localization.Resources.CTes.CTe.NumeroDaAverbacao.getFieldDescription(), maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.Valor = PropertyEntity({ text: Localization.Resources.CTes.CTe.Valor.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, required: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: function () { instancia.AdicionarSeguro() }, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {

        KoBindings(instancia, cte.IdKnockoutSeguro);

        BuscarClientes(instancia.ConsultarCNPJSeguradora, function (data) {
            instancia.CNPJSeguradora.val(data.CPF_CNPJ);
            instancia.Seguradora.val(data.Nome);
        }, null);

        $("#" + instancia.NumeroAverbacao.id).on("blur", function () {
            var val = $(this).val();
            if (val.length != 20 && val.length != 0) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.NumeroDaAverbacaoInvalido, Localization.Resources.CTes.CTe.NumeroDaAverbacaoDevePossuirVinteCaracteres);
                $(this).val("");
            }
        });

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: instancia.Excluir }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "Responsavel", title: Localization.Resources.CTes.CTe.Responsavel, width: "10%" },
            { data: "CNPJSeguradora", title: Localization.Resources.CTes.CTe.CNPJSeguradora, width: "10%" },
            { data: "Seguradora", title: Localization.Resources.CTes.CTe.Seguradora, width: "20%" },
            { data: "NumeroApolice", title: Localization.Resources.CTes.CTe.NumeroDaApolice, width: "15%" },
            { data: "NumeroAverbacao", title: Localization.Resources.CTes.CTe.NumeroDaAverbacao, width: "15%" },
            { data: "Valor", title: Localization.Resources.CTes.CTe.Valor, width: "10%" }];

        cte.GridSeguro = new BasicDataTable(instancia.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);
        cte.Seguros = new Array();

        instancia.RecarregarGrid();
    };

    this.DestivarSeguro = function () {
        DesabilitarCamposInstanciasCTe(instancia);
        cte.GridSeguro.CarregarGrid(cte.Seguros, false);
    };

    this.AdicionarSeguro = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {

            if (!ValidarCNPJ(instancia.CNPJSeguradora.val(), true)) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.CNPJInvalido, Localization.Resources.CTes.CTe.InformeUmCNPJValido);
                instancia.CNPJSeguradora.requiredClass("form-control is-invalid");
                return;
            }
            else if (cte.CTe.TipoModal.val() === EnumTipoModal.Multimodal && cte.Seguros.length >= 1) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.QuantidadeDeSeguros, Localization.Resources.CTes.CTe.SoPermitidoInserirUmSeguroParaTipoModalMultimodal);
                return;
            }

            cte.Seguros.push({
                Codigo: guid(),
                CodigoResponsavel: instancia.Responsavel.val(),
                Responsavel: ObterDescricaoResponsavelSeguro(instancia.Responsavel.val()),
                CNPJSeguradora: instancia.CNPJSeguradora.val(),
                Seguradora: instancia.Seguradora.val(),
                NumeroApolice: instancia.NumeroApolice.val(),
                NumeroAverbacao: instancia.NumeroAverbacao.val(),
                Valor: instancia.Valor.val()
            });

            instancia.RecarregarGrid();

            LimparCampos(instancia);

            cte.SeguroAdicionadoManualmente = true;

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    };

    this.Excluir = function (seguro) {
        for (var i = 0; i < cte.Seguros.length; i++) {
            if (seguro.Codigo == cte.Seguros[i].Codigo) {
                cte.Seguros.splice(i, 1);
                break;
            }
        }

        instancia.RecarregarGrid();
    };

    this.Validar = function () {
        //Não tem a obrigatoriedade de informar o seguro no CT-e
        /*if (cte.CTe.TipoModal.val() === EnumTipoModal.Rodoviario) {
            if (cte.Seguros == null || cte.Seguros.length <= 0) {
                $('a[href="#divInformacoes_' + cte.IdModal + '"]').tab("show");
                $('a[href="#knockoutSeguro_' + cte.IdModal + '"]').tab("show");
                exibirMensagem(tipoMensagem.atencao, "Seguro é obrigatório!", "É necessário adicionar ao menos um seguro para emitir o CT-e!");
                return false;
            }
        }*/
        return true;
    };

    this.RecarregarGrid = function () {
        cte.GridSeguro.CarregarGrid(cte.Seguros);
    };

    this.ObterSeguroTomador = function () {
        if (cte.SeguroAdicionadoManualmente === true && cte.Seguros.length > 0)
            return;
        else if (cte.CTe.TipoModal.val() !== EnumTipoModal.Rodoviario)
            return;

        var cpfCnpjTomador = cte.ObterTomador().CPFCNPJ.val();

        if (!(ValidarCPFCNPJ(cpfCnpjTomador)))
            return;

        var tipoTomador = cte.CTe.TipoTomador.val();

        if (cte.Seguros.length > 0 &&
            cte.FiltroSeguroAutomatico != null &&
            cte.FiltroSeguroAutomatico.Tomador == cpfCnpjTomador &&
            cte.FiltroSeguroAutomatico.TipoTomador == tipoTomador) //já foi pesquisado
            return;

        cte.FiltroSeguroAutomatico = {
            Tomador: cpfCnpjTomador,
            TipoTomador: tipoTomador
        };

        executarReST("CTe/ObterInformacoesSeguro", { Tomador: cpfCnpjTomador }, function (r) {
            if (r.Success) {
                if (r.Data !== false) {
                    if (cte.SeguroAdicionadoManualmente === true && cte.Seguros.length > 0)
                        return;

                    if (r.Data !== null) {
                        var responsavelSeguroComParametro = instancia.ObterResponsavelSeguroAutomatico(r.Data.Responsavel);

                        cte.Seguros = [{
                            Codigo: guid(),
                            CodigoResponsavel: responsavelSeguroComParametro,
                            Responsavel: ObterDescricaoResponsavelSeguro(responsavelSeguroComParametro),
                            CNPJSeguradora: r.Data.Seguradora.CNPJ,
                            Seguradora: r.Data.Seguradora.Descricao,
                            NumeroApolice: r.Data.NumeroApolice,
                            NumeroAverbacao: r.Data.NumeroAverbacao,
                            Valor: "0,00"
                        }];
                    } else {
                        var responsavelSeguro = instancia.ObterResponsavelSeguroAutomatico();

                        cte.Seguros = [{
                            Codigo: guid(),
                            CodigoResponsavel: responsavelSeguro,
                            Responsavel: ObterDescricaoResponsavelSeguro(responsavelSeguro),
                            CNPJSeguradora: '',
                            Seguradora: '',
                            NumeroApolice: '',
                            NumeroAverbacao: '',
                            Valor: "0,00"
                        }];
                    }

                    instancia.RecarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    };

    this.ObterResponsavelSeguroAutomatico = function (responsavel) {
        var responsavelSeguro = EnumResponsavelSeguroCTe.Emitente;

        if (responsavel == null || responsavel == EnumResponsavelSeguro.Embarcador) {
            switch (cte.CTe.TipoTomador.val()) {
                case EnumTipoTomador.Destinatario:
                    responsavelSeguro = EnumResponsavelSeguroCTe.Destinatario;
                    break;
                case EnumTipoTomador.Expedidor:
                    responsavelSeguro = EnumResponsavelSeguroCTe.Expedidor;
                    break;
                case EnumTipoTomador.Outros:
                    responsavelSeguro = EnumResponsavelSeguroCTe.Tomador;
                    break;
                case EnumTipoTomador.Recebedor:
                    responsavelSeguro = EnumResponsavelSeguroCTe.Recebedor;
                    break;
                case EnumTipoTomador.Remetente:
                    responsavelSeguro = EnumResponsavelSeguroCTe.Remetente;
                    break;
                default:
                    break;
            }
        }

        return responsavelSeguro;
    };
};

function ObterDescricaoResponsavelSeguro(responsavel) {
    if (responsavel == undefined || responsavel == 0)
        return "Remetente";

    if (responsavel == 1)
        return "Expedidor"
    else if (responsavel == 2)
        return "Recebedor"
    else if (responsavel == 3)
        return "Destinatario"
    else if (responsavel == 4)
        return "Emitente"
    else if (responsavel == 5)
        return "Tomador"
    else
        return "Remetente";
}