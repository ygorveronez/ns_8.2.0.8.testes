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

var EntregaSimplificadoDocumento = function (cte) {
    var instancia = this;

    this.GridEntregasDocumentos = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoLocalidadeOrigem = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoLocalidadeDestino = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Chave = PropertyEntity({ text: Localization.Resources.CTes.CTe.Chave.getRequiredFieldDescription(), getType: typesKnockout.string, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ text: Localization.Resources.CTes.CTe.DataDeEmissao.getRequiredFieldDescription(), getType: typesKnockout.date, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorNotaFiscal = PropertyEntity({ text: Localization.Resources.CTes.CTe.ValorDaNotaFiscal.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Modelo = PropertyEntity({ val: ko.observable("55"), def: ko.observable("55"), options: ko.observable(_modelosNFe), text: Localization.Resources.CTes.CTe.Modelo.getRequiredFieldDescription(), required: ko.observable(true), visible: ko.observable(false), enable: ko.observable(true) });
    this.Numero = PropertyEntity({ text: Localization.Resources.CTes.CTe.Numero.getRequiredFieldDescription(), getType: typesKnockout.int, maxlength: 15, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.Serie = PropertyEntity({ text: Localization.Resources.CTes.CTe.Serie.getRequiredFieldDescription(), maxlength: 3, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.CFOP = PropertyEntity({ text: ko.observable(Localization.Resources.CTes.CTe.CFOP.getFieldDescription()), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.BaseCalculoICMS = PropertyEntity({ text: Localization.Resources.CTes.CTe.BaseCalculoICMS.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorICMS = PropertyEntity({ text: Localization.Resources.CTes.CTe.ValorICMS.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.BaseCalculoICMSST = PropertyEntity({ text: Localization.Resources.CTes.CTe.BaseCalculoICMSST.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorICMSST = PropertyEntity({ text: Localization.Resources.CTes.CTe.ValorICMSST.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.Peso = PropertyEntity({ text: ko.observable(Localization.Resources.CTes.CTe.PesoKg.getFieldDescription()), getType: typesKnockout.decimal, maxlength: 15, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.PIN = PropertyEntity({ text: Localization.Resources.CTes.CTe.PIN.getFieldDescription(), maxlength: 9, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.ValorProdutos = PropertyEntity({ text: Localization.Resources.CTes.CTe.ValorTotalProdutos.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 100, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.NumeroReferenciaEDI = PropertyEntity({ text: Localization.Resources.CTes.CTe.NumeroReferenciaEDI.getFieldDescription(), maxlength: 50, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.PINSuframa = PropertyEntity({ text: Localization.Resources.CTes.CTe.PINSuframa.getFieldDescription(), maxlength: 50, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.NCMPredominante = PropertyEntity({ text: Localization.Resources.CTes.CTe.NCMPredominante.getFieldDescription(), maxlength: 4, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroControleCliente = PropertyEntity({ text: Localization.Resources.CTes.CTe.NumeroControleCliente.getFieldDescription(), maxlength: 50, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });

    this.AdicionarDocumento = PropertyEntity({ eventClick: function () { instancia.AdicionarDocumentoSimplificado(); }, type: types.event, text: Localization.Resources.CTes.CTe.SalvarDocumento, visible: ko.observable(true), enable: ko.observable(false) });

    this.Load = function () {

        cte.EntregasSimplificadoDocumentos = new Array();

        KoBindings(instancia, cte.IdKnockoutEntregaSimplificadoDocumento);

        $("#" + instancia.Chave.id).mask("0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000", { selectOnFocus: true, clearIfNotMatch: true });
        $("#" + instancia.CFOP.id).mask("0000", { selectOnFocus: true, clearIfNotMatch: true });

        cte.GridEntregasDocumentos = new BasicDataTable(instancia.GridEntregasDocumentos.id, headerDocumentosCTeSimplificado(), menuOpcoesDocumentosCTeSimplificado(instancia), { column: 1, dir: orderDir.asc }, null, 5);

        instancia.RecarregarGridSimplificadoDocumentos();
    };

    this.AdicionarDocumentoSimplificado = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {
            for (var i = 0; i < cte.EntregasSimplificadoDocumentos.length; i++) {
                if (instancia.Codigo.val() > "0" && instancia.Codigo.val() != undefined) {
                    if (instancia.Codigo.val() == cte.EntregasSimplificadoDocumentos[i].Codigo) {
                        continue;
                    }
                }

                if (instancia.Chave.val().replace(/\s/g, "").replace(" ", "") == cte.EntregasSimplificadoDocumentos[i].Chave) {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.DocumentoEmEdicao, "Chave de acesso já informada!");
                    return;
                }
            }

            if (instancia.Codigo.val() > "0" && instancia.Codigo.val() != undefined) {
                for (var i = 0; i < cte.EntregasSimplificadoDocumentos.length; i++) {
                    if (instancia.Codigo.val() == cte.EntregasSimplificadoDocumentos[i].Codigo) {
                        cte.EntregasSimplificadoDocumentos.splice(i, 1);
                        break;
                    }
                }
            }

            instancia.Codigo.val(guid());
            instancia.CodigoLocalidadeOrigem.val(cte.EntregaSimplificado.LocalidadeOrigem.codEntity());
            instancia.CodigoLocalidadeDestino.val(cte.EntregaSimplificado.LocalidadeDestino.codEntity());
            instancia.Chave.val(instancia.Chave.val().replace(/\s/g, "").replace(" ", ""));
            instancia.Numero.val(Globalize.parseInt(instancia.Chave.val().substring(25, 34)));

            cte.EntregasSimplificadoDocumentos.push(RetornarObjetoPesquisa(instancia));

            instancia.RecarregarGridSimplificadoDocumentos();

            LimparCampos(instancia);

            instancia.AtualizarValoresMercadoriaSimplificado();

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    };

    this.ExcluirDocumentoSimplificado = function (documento) {
        if (instancia.Codigo.val() > 0) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.DocumentoEmEdicao, Localization.Resources.CTes.CTe.PorFavorVerifiqueOsDocumentosPoisExisteUmEmEdicao);
            return;
        }

        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.CTes.CTe.RealmenteDesejaExcluirDocumento, function () {
            for (var i = 0; i < cte.EntregasSimplificadoDocumentos.length; i++) {
                if (documento.Codigo == cte.EntregasSimplificadoDocumentos[i].Codigo) {
                    cte.EntregasSimplificadoDocumentos.splice(i, 1);
                    break;
                }
            }

            instancia.RecarregarGridSimplificadoDocumentos();
            instancia.AtualizarValoresMercadoriaSimplificado();
        });
    };

    this.AtualizarValoresMercadoriaSimplificado = function () {
        var valorTotalMercadoria = 0;

        for (var i = 0; i < cte.EntregasSimplificadoDocumentos.length; i++) {
            var valorTotalNF = Globalize.parseFloat(cte.EntregasSimplificadoDocumentos[i].ValorNotaFiscal);

            if (isNaN(valorTotalNF))
                valorTotalNF = 0;

            valorTotalMercadoria += valorTotalNF;
        }
        if (valorTotalMercadoria > 0)
            cte.InformacaoCarga.ValorTotalCarga.val(Globalize.format(valorTotalMercadoria, "n2"));
    };

    this.RecarregarGridSimplificadoDocumentos = function () {
        cte.GridEntregasDocumentos.CarregarGrid(instancia.BuscarDocumentosSimplificado(), cte.EntregaSimplificado.PermitirEdicao.val());
    };

    this.BuscarDocumentosSimplificado = function () {
        var data = new Array();

        $.each(cte.EntregasSimplificadoDocumentos, function (i, documento) {
            if (cte.EntregaSimplificado.LocalidadeOrigem.codEntity() === documento.CodigoLocalidadeOrigem && cte.EntregaSimplificado.LocalidadeDestino.codEntity() === documento.CodigoLocalidadeDestino) {
                var documentoGrid = new Object();

                documentoGrid.CodigoLocalidadeOrigem = documento.CodigoLocalidadeOrigem;
                documentoGrid.CodigoLocalidadeDestino = documento.CodigoLocalidadeDestino;
                documentoGrid.Codigo = documento.Codigo;
                documentoGrid.Numero = documento.Numero;
                documentoGrid.DataEmissao = documento.DataEmissao;
                documentoGrid.Chave = documento.Chave;
                documentoGrid.ValorNotaFiscal = Globalize.format(documento.ValorNotaFiscal, "n2");
                documentoGrid.Descricao = documento.Descricao;
                documentoGrid.Peso = Globalize.format(documento.Peso, "n2");
                documentoGrid.Serie = documento.Serie;

                documentoGrid.CFOP = documento.CFOP;
                documentoGrid.BaseCalculoICMS = documento.BaseCalculoICMS;
                documentoGrid.ValorICMS = documento.ValorICMS;
                documentoGrid.BaseCalculoICMSST = documento.BaseCalculoICMSST;
                documentoGrid.ValorICMSST = documento.ValorICMSST;
                documentoGrid.PIN = documento.PIN;
                documentoGrid.ValorProdutos = documento.ValorProdutos;
                documentoGrid.Modelo = documento.Modelo;
                documentoGrid.NumeroReferenciaEDI = documento.NumeroReferenciaEDI;
                documentoGrid.NumeroControleCliente = documento.NumeroControleCliente;
                documentoGrid.PINSuframa = documento.PINSuframa;
                documentoGrid.NCMPredominante = documento.NCMPredominante;

                data.push(documentoGrid);
            }
        });
        return data;
    };

    this.EditarDocumentoSimplificado = function (documento) {
        cte.EntregaSimplificadoDocumento.Modelo.val(documento.Modelo);
        cte.EntregaSimplificadoDocumento.Codigo.val(documento.Codigo);
        cte.EntregaSimplificadoDocumento.Chave.val(documento.Chave);
        cte.EntregaSimplificadoDocumento.DataEmissao.val(documento.DataEmissao);
        cte.EntregaSimplificadoDocumento.ValorNotaFiscal.val(Globalize.format(documento.ValorNotaFiscal, "n2"));
        cte.EntregaSimplificadoDocumento.Serie.val(documento.Serie);
        cte.EntregaSimplificadoDocumento.Numero.val(documento.Numero);
        cte.EntregaSimplificadoDocumento.CFOP.val(documento.CFOP);
        cte.EntregaSimplificadoDocumento.BaseCalculoICMS.val(Globalize.format(documento.BaseCalculoICMS, "n2"));
        cte.EntregaSimplificadoDocumento.ValorICMS.val(Globalize.format(documento.ValorICMS, "n2"));
        cte.EntregaSimplificadoDocumento.BaseCalculoICMSST.val(Globalize.format(documento.BaseCalculoICMSST, "n2"));
        cte.EntregaSimplificadoDocumento.ValorICMSST.val(Globalize.format(documento.ValorICMSST, "n2"));
        cte.EntregaSimplificadoDocumento.Peso.val(Globalize.format(documento.Peso, "n2"));
        cte.EntregaSimplificadoDocumento.PIN.val(documento.PIN);
        cte.EntregaSimplificadoDocumento.ValorProdutos.val(Globalize.format(documento.ValorProdutos, "n2"));
        cte.EntregaSimplificadoDocumento.Descricao.val(documento.Descricao);
        cte.EntregaSimplificadoDocumento.NumeroReferenciaEDI.val(documento.NumeroReferenciaEDI);
        cte.EntregaSimplificadoDocumento.NumeroControleCliente.val(documento.NumeroControleCliente);
        cte.EntregaSimplificadoDocumento.PINSuframa.val(documento.PINSuframa);
        cte.EntregaSimplificadoDocumento.NCMPredominante.val(documento.NCMPredominante);
    };
};

function headerDocumentosCTeSimplificado() {

    return [
        //{ data: "CodigoEntrega", visible: false },
        { data: "CodigoLocalidadeOrigem", visible: false },
        { data: "CodigoLocalidadeDestino", visible: false },
        { data: "Codigo", visible: false },
        { data: "Modelo", visible: false },
        { data: "CFOP", visible: false },
        { data: "BaseCalculoICMS", visible: false },
        { data: "ValorICMS", visible: false },
        { data: "BaseCalculoICMSST", visible: false },
        { data: "ValorICMSST", visible: false },
        { data: "PIN", visible: false },
        { data: "ValorProdutos", visible: false },
        { data: "NumeroReferenciaEDI", visible: false },
        { data: "NumeroControleCliente", visible: false },
        { data: "PINSuframa", visible: false },
        { data: "NCMPredominante", visible: false },

        { data: "Numero", title: Localization.Resources.CTes.CTe.Numero, width: "15%" },
        { data: "DataEmissao", title: Localization.Resources.CTes.CTe.DataDeEmissao, width: "15%" },
        { data: "Chave", title: Localization.Resources.CTes.CTe.Chave, width: "35%"},
        { data: "ValorNotaFiscal", title: Localization.Resources.CTes.CTe.ValorTotal, width: "15%" }
    ];
}

function menuOpcoesDocumentosCTeSimplificado(local) {
    var editarItem = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: local.EditarDocumentoSimplificado };
    var excluirItem = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: local.ExcluirDocumentoSimplificado };

    return menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [editarItem, excluirItem] };
}