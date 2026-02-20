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
/// <reference path="EntregaSimplificadoDocumento.js" />
/// <reference path="EntregaSimplificadoDocumentoTransporteAnterior.js" />
/// <reference path="EntregaSimplificadoComponentePrestacaoServico.js" />

var _modelosNFe = [{ text: "55 - Nota Fiscal Eletrônica", value: "55" }];

var EntregaSimplificado = function (cte) {
    var instancia = this;

    this.PermitirEdicao = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool });
    this.GridEntregas = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoLocalidadeOrigem = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoLocalidadeDestino = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Origem = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Destino = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.LocalidadeOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: "Origem", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.LocalidadeDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: "Destino", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorFrete = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: Localization.Resources.CTes.CTe.ValorDoFrete.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorPrestacaoServico = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: Localization.Resources.CTes.CTe.ValorDaPrestacao.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorAReceber = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: Localization.Resources.CTes.CTe.ValorReceber.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.AdicionarEntrega = PropertyEntity({ eventClick: function () { instancia.AdicionarEntregaSimplificado(); }, type: types.event, text: "Salvar Entrega", visible: ko.observable(true), enable: ko.observable(true) });

    this.LocalidadeDestino.val.subscribe(function (destino) {
        AlterarEstadoEntregaSimplificado(cte);
    });

    this.Load = function () {
        cte.EntregasSimplificado = new Array();

        KoBindings(instancia, cte.IdKnockoutEntregaSimplificado);

        $('#' + instancia.ValorFrete.id).bind('blur', function () { instancia.AtualizarValoresFreteEntrega(); });
        BuscarLocalidades(instancia.LocalidadeOrigem);
        BuscarLocalidades(instancia.LocalidadeDestino);

        cte.GridEntregas = new BasicDataTable(instancia.GridEntregas.id, headerEntregasCTeSimplificado(), menuOpcoesEntregasCTeSimplificado(instancia), { column: 1, dir: orderDir.asc }, null, 5);
        instancia.RecarregarGridSimplificadoEntregas();

        cte.EntregaSimplificadoDocumento.Load();
        cte.EntregaSimplificadoDocumentoTransporteAnterior.Load();
        cte.EntregaSimplificadoComponentePrestacaoServico.Load();
    };

    this.DesativarEntregaSimplificado = function () {
        DesabilitarCamposInstanciasCTe(instancia);
        DesabilitarCamposInstanciasCTe(cte.EntregaSimplificadoDocumento);
        DesabilitarCamposInstanciasCTe(cte.EntregaSimplificadoDocumentoTransporteAnterior);
        DesabilitarCamposInstanciasCTe(cte.EntregaSimplificadoComponentePrestacaoServico);
        cte.EntregaSimplificado.PermitirEdicao.val(false);
        cte.GridEntregas.Destroy();
        cte.GridEntregas = new BasicDataTable(cte.EntregaSimplificado.GridEntregas.id, headerEntregasCTeSimplificado(), menuOpcoesEntregasCTeSimplificado(cte.EntregaSimplificado), { column: 1, dir: orderDir.asc });
        cte.GridEntregas.CarregarGrid(instancia.BuscarEntregasSimplificado());
        cte.GridEntregasDocumentos.CarregarGrid(cte.EntregaSimplificadoDocumento.BuscarDocumentosSimplificado(), false);
        cte.GridEntregasDocumentosTransporteAnterior.CarregarGrid(cte.EntregaSimplificadoDocumentoTransporteAnterior.BuscarDocumentosTransporteAnteriorSimplificado(), false);
        cte.GridComponentePrestacaoServico.CarregarGrid(cte.EntregaSimplificadoComponentePrestacaoServico.BuscarComponentesPrestacaoServicoSimplificado(), false);
    };

    this.AdicionarEntregaSimplificado = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {
            for (var i = 0; i < cte.EntregasSimplificado.length; i++) {
                if (instancia.Codigo.val() > "0" && instancia.Codigo.val() != undefined) {
                    if (instancia.Codigo.val() == cte.EntregasSimplificado[i].Codigo) {
                        continue;
                    }
                }

                if (instancia.LocalidadeOrigem.codEntity() == cte.EntregasSimplificado[i].CodigoLocalidadeOrigem && instancia.LocalidadeDestino.codEntity() == cte.EntregasSimplificado[i].CodigoLocalidadeDestino) {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.DocumentoEmEdicao, "Local de origem e destino já informado!");
                    return;
                }
            }

            if (instancia.Codigo.val() > "0" && instancia.Codigo.val() != undefined) {
                for (var i = 0; i < cte.EntregasSimplificado.length; i++) {
                    if (instancia.Codigo.val() == cte.EntregasSimplificado[i].Codigo) {
                        cte.EntregasSimplificado.splice(i, 1);
                        break;
                    }
                }
            }

            instancia.Codigo.val(guid());
            instancia.CodigoLocalidadeOrigem.val(instancia.LocalidadeOrigem.codEntity());
            instancia.CodigoLocalidadeDestino.val(instancia.LocalidadeDestino.codEntity());
            instancia.Origem.val(instancia.LocalidadeOrigem.val());
            instancia.Destino.val(instancia.LocalidadeDestino.val());

            cte.EntregasSimplificado.push(RetornarObjetoPesquisa(instancia));

            instancia.RecarregarGridSimplificadoEntregas();
            LimparCampos(instancia);
            instancia.AtualizarValoresTotal(true);
            instancia.AtualizarValoresComponentesPrestacaoServicoImpostoEFrete();
            instancia.AtualizarValoresComponentesPrestacaoServicoEntrega();

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    };

    this.ExcluirEntregaSimplificado = function (entrega) {
        if (instancia.Codigo.val() > 0) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.DocumentoEmEdicao, Localization.Resources.CTes.CTe.PorFavorVerifiqueOsDocumentosPoisExisteUmEmEdicao);
            return;
        }

        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.CTes.CTe.RealmenteDesejaExcluirDocumento, function () {
            for (var i = 0; i < cte.EntregasSimplificado.length; i++) {
                if (documento.Codigo == cte.EntregasSimplificado[i].Codigo) {
                    cte.EntregasSimplificado.splice(i, 1);
                    break;
                }
            }

            instancia.RecarregarGridSimplificadoEntregas();
        });
    };

    this.Validar = function () {
        if (cte.CTe.Tipo.val() === EnumTipoCTe.Simplificado) {
            if (cte.EntregasSimplificadoDocumentos == null || cte.EntregasSimplificadoDocumentos.length <= 0) {
                $('a[href="#divEntregaSimplificado_' + cte.IdModal + '"]').tab("show");
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.DocumentoObrigatorio, Localization.Resources.CTes.CTe.NecessarioInformarAoMenosUmDocumento);
                return false;
            }
        }

        return true;
    };

    this.RecarregarGridSimplificadoEntregas = function () {
        cte.GridEntregas.CarregarGrid(instancia.BuscarEntregasSimplificado());
        cte.EntregaSimplificadoDocumento.AdicionarDocumento.enable(false);
        cte.EntregaSimplificadoDocumentoTransporteAnterior.AdicionarDocumentoTransporteAnterior.enable(false);
        cte.EntregaSimplificadoComponentePrestacaoServico.AdicionarComponentePrestacaoServico.enable(false);
    };

    this.BuscarEntregasSimplificado = function () {
        var data = new Array();

        $.each(cte.EntregasSimplificado, function (i, entrega) {
            var documentoGrid = new Object();

            documentoGrid.Codigo = entrega.Codigo;
            documentoGrid.CodigoLocalidadeOrigem = entrega.CodigoLocalidadeOrigem;
            documentoGrid.CodigoLocalidadeDestino = entrega.CodigoLocalidadeDestino;
            documentoGrid.Origem = entrega.Origem;
            documentoGrid.Destino = entrega.Destino;
            documentoGrid.ValorFrete = Globalize.format(entrega.ValorFrete, "n2");
            documentoGrid.ValorPrestacaoServico = Globalize.format(entrega.ValorPrestacaoServico, "n2");
            documentoGrid.ValorAReceber = Globalize.format(entrega.ValorAReceber, "n2");

            data.push(documentoGrid);
        });

        return data;
    };

    this.EditarEntregaSimplificado = function (entrega) {
        instancia.Codigo.val(entrega.Codigo);
        instancia.LocalidadeOrigem.codEntity(entrega.CodigoLocalidadeOrigem);
        instancia.LocalidadeOrigem.val(entrega.Origem);
        instancia.LocalidadeDestino.codEntity(entrega.CodigoLocalidadeDestino);
        instancia.LocalidadeDestino.val(entrega.Destino);
        instancia.ValorFrete.val(entrega.ValorFrete);
        instancia.ValorPrestacaoServico.val(entrega.ValorPrestacaoServico);
        instancia.ValorAReceber.val(entrega.ValorAReceber);
        if (cte.EntregaSimplificado.PermitirEdicao.val() === true) {
            cte.EntregaSimplificadoDocumento.AdicionarDocumento.enable(true);
            cte.EntregaSimplificadoDocumentoTransporteAnterior.AdicionarDocumentoTransporteAnterior.enable(true);
            cte.EntregaSimplificadoComponentePrestacaoServico.AdicionarComponentePrestacaoServico.enable(true);
        }
    }

    this.AtualizarValoresFreteEntrega = function () {
        var totalCompomente = 0, totalComponenteDescontarTotalReceber = 0, totalComponenteIncluirBCICMS = 0;
        for (var i = 0; i < cte.EntregasSimplificadoComponentesPrestacaoServico.length; i++) {

            if (cte.EntregasSimplificadoComponentesPrestacaoServico[i].DescricaoComponente == "IMPOSTOS" || cte.EntregasSimplificadoComponentesPrestacaoServico[i].DescricaoComponente == "FRETE VALOR") {
                continue;
            }

            if (cte.EntregaSimplificado.LocalidadeOrigem.codEntity() === cte.EntregasSimplificadoComponentesPrestacaoServico[i].CodigoLocalidadeOrigem && cte.EntregaSimplificado.LocalidadeDestino.codEntity() === cte.EntregasSimplificadoComponentesPrestacaoServico[i].CodigoLocalidadeDestino) {

                var valorComponente = Globalize.parseFloat(cte.EntregasSimplificadoComponentesPrestacaoServico[i].Valor);

                if (cte.EntregasSimplificadoComponentesPrestacaoServico[i].DescontarTotalReceber === true)
                    totalComponenteDescontarTotalReceber += valorComponente;
                else {
                    totalCompomente += valorComponente;

                    if (cte.EntregasSimplificadoComponentesPrestacaoServico[i].IncluirBaseCalculoICMS === true)
                        totalComponenteIncluirBCICMS += valorComponente;
                }
            }
        };

        var elemento = $('#' + cte.TotalServico.IncluirICMSFrete.id);

        var cstICMS = cte.TotalServico.ICMS.val();
        var valorFrete = Globalize.parseFloat(instancia.ValorFrete.val());
        var valorAReceber = totalCompomente + valorFrete;
        var valorPrestacaoServico = valorAReceber;

        valorAReceber += totalComponenteDescontarTotalReceber;

        var valorBaseCalculoICMS = valorFrete + totalComponenteIncluirBCICMS;

        var aliquota = cstICMS != EnumICMSCTe.Isencao_40 && cstICMS != EnumICMSCTe.NaoTributado_41 && cstICMS != EnumICMSCTe.Diferido_51 && cstICMS != EnumICMSCTe.SimplesNacional ? Globalize.parseFloat(cte.TotalServico.AliquotaICMS.val()) : 0;
        var percentualReducaoBaseCalculoICMS = Globalize.parseFloat(cte.TotalServico.PercentualReducaoBaseCalculoICMS.val());
        valorBaseCalculoICMS -= valorBaseCalculoICMS * (percentualReducaoBaseCalculoICMS / 100);

        var valorICMS = valorBaseCalculoICMS * (aliquota / 100);
        var valorCreditoICMS = Globalize.parseFloat(cte.TotalServico.ValorCredito.val());

        if (elemento.prop('checked')) {
            var percentualICMSRecolhido = Globalize.parseFloat(cte.TotalServico.PercentualICMSIncluir.val());

            valorBaseCalculoICMS += (aliquota > 0 ? Global.roundNumber(valorBaseCalculoICMS / ((100 - aliquota) / 100), 2) - valorBaseCalculoICMS : 0);
            valorICMS = valorBaseCalculoICMS * (aliquota / 100);
            var valorRecolhido = valorICMS * (percentualICMSRecolhido / 100);

            if (cstICMS == EnumICMSCTe.CobradoPorSubstituicaoTributaria_60) {
                instancia.ValorAReceber.val(Globalize.format((valorAReceber + valorCreditoICMS), "n2"));
                instancia.ValorPrestacaoServico.val(Globalize.format((valorPrestacaoServico + valorRecolhido), "n2"));
            } else {
                instancia.ValorAReceber.val(Globalize.format((valorAReceber + valorRecolhido + valorCreditoICMS), "n2"));
                instancia.ValorPrestacaoServico.val(Globalize.format((valorPrestacaoServico + valorRecolhido + valorCreditoICMS), "n2"));
            }
        } else {
            instancia.ValorAReceber.val(Globalize.format((valorAReceber + valorCreditoICMS), "n2"));
            instancia.ValorPrestacaoServico.val(Globalize.format((valorPrestacaoServico + valorCreditoICMS), "n2"));
        }
    };

    this.AtualizarValoresTotal = function (atualizarTotais) {

        var valorTotalFrete = 0;
        var ValorTotalPrestacaoServico = 0;
        var valorTotalAReceber = 0;

        for (var i = 0; i < cte.EntregasSimplificado.length; i++) {
            var valorFrete = Globalize.parseFloat(cte.EntregasSimplificado[i].ValorFrete);
            var valorPrestacaoServico = Globalize.parseFloat(cte.EntregasSimplificado[i].ValorPrestacaoServico);
            var valorAReceber = Globalize.parseFloat(cte.EntregasSimplificado[i].ValorAReceber);

            if (isNaN(valorFrete))
                valorFrete = 0;

            if (isNaN(ValorTotalPrestacaoServico))
                ValorTotalPrestacaoServico = 0;

            if (isNaN(valorTotalAReceber))
                valorTotalAReceber = 0;

            valorTotalFrete += valorFrete;
            valorTotalAReceber += valorPrestacaoServico;
            ValorTotalPrestacaoServico += valorAReceber;
        }
        
        cte.TotalServico.ValorFrete.val(Globalize.format(valorTotalFrete, "n2"));
        if (atualizarTotais) {
            cte.TotalServico.AtualizarTotaisCTe();
        }
        cte.TotalServico.ValorPrestacaoServico.val(Globalize.format(valorTotalAReceber, "n2"));
        cte.TotalServico.ValorReceber.val(Globalize.format(ValorTotalPrestacaoServico, "n2"));
    };

    this.AtualizarValoresFreteEntregaGrid = function () {
        
        for (var i = 0; i < cte.EntregasSimplificado.length; i++) {
            instancia.Codigo.val(guid());
            instancia.LocalidadeOrigem.codEntity(cte.EntregasSimplificado[i].CodigoLocalidadeOrigem);
            instancia.LocalidadeOrigem.val(cte.EntregasSimplificado[i].Origem);
            instancia.LocalidadeDestino.codEntity(cte.EntregasSimplificado[i].CodigoLocalidadeDestino);
            instancia.LocalidadeDestino.val(cte.EntregasSimplificado[i].Destino);
            instancia.ValorFrete.val(cte.EntregasSimplificado[i].ValorFrete);
            instancia.ValorPrestacaoServico.val(cte.EntregasSimplificado[i].ValorPrestacaoServico);
            instancia.ValorAReceber.val(cte.EntregasSimplificado[i].ValorAReceber);
            instancia.AtualizarValoresFreteEntrega();

            cte.EntregasSimplificado.splice(i, 1);
            cte.EntregasSimplificado.push(RetornarObjetoPesquisa(instancia));
            LimparCampos(instancia);
        }

        instancia.RecarregarGridSimplificadoEntregas();
        instancia.AtualizarValoresTotal(true);
        instancia.AtualizarValoresComponentesPrestacaoServicoImpostoEFrete();
        instancia.AtualizarValoresComponentesPrestacaoServicoEntrega();
    }

    this.AtualizarValoresComponentesPrestacaoServicoImpostoEFrete = function () {

        for (var i = 0; i < cte.EntregasSimplificadoComponentesPrestacaoServico.length; i++) {
            if (cte.EntregasSimplificadoComponentesPrestacaoServico[i].DescricaoComponente == "IMPOSTOS" || cte.EntregasSimplificadoComponentesPrestacaoServico[i].DescricaoComponente == "FRETE VALOR") {
                cte.EntregasSimplificadoComponentesPrestacaoServico.splice(i, 1);
                i--; // Decrementar o índice para revisar o novo elemento na posição atual
            }
        }

        //if ((cte.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Nao && cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim) || (cte.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim && cte.Empresa.Configuracao != null && cte.Empresa.Configuracao.PercentualImpostoSimplesNacional > 0))

        var elemento = $('#' + cte.TotalServico.IncluirICMSFrete.id);

        for (var i = 0; i < cte.EntregasSimplificado.length; i++) {
            if (elemento.prop('checked')) {
                LimparCampos(cte.EntregaSimplificadoComponentePrestacaoServico);

                cte.EntregaSimplificadoComponentePrestacaoServico.Codigo.val(guid());
                cte.EntregaSimplificadoComponentePrestacaoServico.CodigoLocalidadeOrigem.val(cte.EntregasSimplificado[i].CodigoLocalidadeOrigem);
                cte.EntregaSimplificadoComponentePrestacaoServico.CodigoLocalidadeDestino.val(cte.EntregasSimplificado[i].CodigoLocalidadeDestino);
                cte.EntregaSimplificadoComponentePrestacaoServico.DescricaoComponente.val("IMPOSTOS");
                var valorImposto = (Globalize.parseFloat(cte.EntregasSimplificado[i].ValorFrete) * Globalize.parseFloat(cte.TotalServico.ValorICMS.val())) / Globalize.parseFloat(cte.TotalServico.ValorFrete.val());
                cte.EntregaSimplificadoComponentePrestacaoServico.Valor.val(valorImposto);
                cte.EntregaSimplificadoComponentePrestacaoServico.IncluirBaseCalculoICMS.val(false);
                cte.EntregaSimplificadoComponentePrestacaoServico.IncluirTotalReceber.val(false);
                cte.EntregaSimplificadoComponentePrestacaoServico.DescontarTotalReceber.val(false);

                cte.EntregasSimplificadoComponentesPrestacaoServico.push(RetornarObjetoPesquisa(cte.EntregaSimplificadoComponentePrestacaoServico));
            }

            LimparCampos(cte.EntregaSimplificadoComponentePrestacaoServico);

            cte.EntregaSimplificadoComponentePrestacaoServico.Codigo.val(guid());
            cte.EntregaSimplificadoComponentePrestacaoServico.CodigoLocalidadeOrigem.val(cte.EntregasSimplificado[i].CodigoLocalidadeOrigem);
            cte.EntregaSimplificadoComponentePrestacaoServico.CodigoLocalidadeDestino.val(cte.EntregasSimplificado[i].CodigoLocalidadeDestino);
            cte.EntregaSimplificadoComponentePrestacaoServico.DescricaoComponente.val("FRETE VALOR");
            cte.EntregaSimplificadoComponentePrestacaoServico.Valor.val(Globalize.format(cte.EntregasSimplificado[i].ValorFrete, "n2"));
            cte.EntregaSimplificadoComponentePrestacaoServico.IncluirBaseCalculoICMS.val(false);
            cte.EntregaSimplificadoComponentePrestacaoServico.IncluirTotalReceber.val(true);
            cte.EntregaSimplificadoComponentePrestacaoServico.DescontarTotalReceber.val(false);

            cte.EntregasSimplificadoComponentesPrestacaoServico.push(RetornarObjetoPesquisa(cte.EntregaSimplificadoComponentePrestacaoServico));
        }
    }

    this.AtualizarValoresComponentesPrestacaoServicoEntrega = function () {
        for (var f = 0; f < cte.Componentes.length; f++) {

            for (var i = 0; i < cte.EntregasSimplificadoComponentesPrestacaoServico.length; i++) {
                if (cte.EntregasSimplificadoComponentesPrestacaoServico[i].DescricaoComponente == cte.Componentes[f].DescricaoComponente) {
                    cte.EntregasSimplificadoComponentesPrestacaoServico.splice(i, 1);
                    i--; // Decrementar o índice para revisar o novo elemento na posição atual
                }
            }

            for (var i = 0; i < cte.EntregasSimplificado.length; i++) {
                LimparCampos(cte.EntregaSimplificadoComponentePrestacaoServico);

                cte.EntregaSimplificadoComponentePrestacaoServico.Codigo.val(guid());
                cte.EntregaSimplificadoComponentePrestacaoServico.CodigoLocalidadeOrigem.val(cte.EntregasSimplificado[i].CodigoLocalidadeOrigem);
                cte.EntregaSimplificadoComponentePrestacaoServico.CodigoLocalidadeDestino.val(cte.EntregasSimplificado[i].CodigoLocalidadeDestino);
                cte.EntregaSimplificadoComponentePrestacaoServico.DescricaoComponente.val(cte.Componentes[f].DescricaoComponente);

                var valorComponente = Globalize.parseFloat(cte.Componentes[f].Valor) / cte.EntregasSimplificado.length;
                cte.EntregaSimplificadoComponentePrestacaoServico.Valor.val(Globalize.format(valorComponente, "n2"));
                cte.EntregaSimplificadoComponentePrestacaoServico.IncluirBaseCalculoICMS.val(cte.Componentes[f].IncluirBaseCalculoICMS);
                cte.EntregaSimplificadoComponentePrestacaoServico.IncluirTotalReceber.val(cte.Componentes[f].IncluirTotalReceber);
                cte.EntregaSimplificadoComponentePrestacaoServico.DescontarTotalReceber.val(cte.Componentes[f].DescontarTotalReceber);

                cte.EntregasSimplificadoComponentesPrestacaoServico.push(RetornarObjetoPesquisa(cte.EntregaSimplificadoComponentePrestacaoServico));
            }
        }
    }
};

function headerEntregasCTeSimplificado() {

    return [
        { data: "Codigo", visible: false },
        { data: "CodigoLocalidadeOrigem", visible: false },
        { data: "CodigoLocalidadeDestino", visible: false },
        { data: "Origem", title: "Origem", width: "35%" },
        { data: "Destino", title: "Destino", width: "35%" },
        { data: "ValorFrete", title: "Valor Frete", width: "30%" },
        { data: "ValorPrestacaoServico", title: "Valor Prestação Serviço", width: "30%" },
        { data: "ValorAReceber", title: "Valor Prestação Serviço", width: "30%" }
    ];
}

function menuOpcoesEntregasCTeSimplificado(local) {
    var editarItem = { descricao: local.PermitirEdicao.val() ? Localization.Resources.Gerais.Geral.Editar : "Selecionar", id: guid(), metodo: local.EditarEntregaSimplificado };
    var excluirItem = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: local.ExcluirEntregaSimplificado, icone: "", visibilidade: local.PermitirEdicao.val() };

    if (local.PermitirEdicao.val() === true)
        return menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [ editarItem, excluirItem ] };
    else
        return menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [ editarItem ] };
}

function AlterarEstadoEntregaSimplificado(cte) {
    if (cte.GridEntregas == null)
        return;

    cte.GridEntregasDocumentos.Destroy();
    cte.GridEntregasDocumentos = new BasicDataTable(cte.EntregaSimplificadoDocumento.GridEntregasDocumentos.id, headerDocumentosCTeSimplificado(), menuOpcoesDocumentosCTeSimplificado(cte.EntregaSimplificadoDocumento), { column: 1, dir: orderDir.asc });
    cte.EntregaSimplificadoDocumento.RecarregarGridSimplificadoDocumentos();

    cte.GridEntregasDocumentosTransporteAnterior.Destroy();
    cte.GridEntregasDocumentosTransporteAnterior = new BasicDataTable(cte.EntregaSimplificadoDocumentoTransporteAnterior.GridEntregasDocumentosTransporteAnterior.id, headerDocumentosTransporteAnteriorCTeSimplificado(), menuOpcoesDocumentosTransporteAnteriorCTeSimplificado(cte.EntregaSimplificadoDocumentoTransporteAnterior), { column: 1, dir: orderDir.asc });
    cte.EntregaSimplificadoDocumentoTransporteAnterior.RecarregarGridSimplificadoDocumentosTransporteAnterior();

    cte.GridComponentePrestacaoServico.Destroy();
    cte.GridComponentePrestacaoServico = new BasicDataTable(cte.EntregaSimplificadoComponentePrestacaoServico.GridComponentePrestacaoServico.id, headerComponentesPrestacaoServicoSimplificado(), menuOpcoesComponentesPrestacaoServicoCTeSimplificado(cte.EntregaSimplificadoComponentePrestacaoServico), { column: 1, dir: orderDir.asc });
    cte.EntregaSimplificadoComponentePrestacaoServico.RecarregarGridSimplificadoComponentesPrestacaoServico();

    LimparCampos(cte.EntregaSimplificadoDocumento);
    LimparCampos(cte.EntregaSimplificadoDocumentoTransporteAnterior);
    LimparCampos(cte.EntregaSimplificadoComponentePrestacaoServico);
}