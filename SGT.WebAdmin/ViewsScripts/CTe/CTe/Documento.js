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
/// <reference path="../../Enumeradores/EnumTipoDocumentoCTe.js" />

var _modelosNFe = [{ text: "55 - Nota Fiscal Eletrônica", value: "55" }];

var DocumentoCTe = function (cte) {
    var instancia = this;

    this.Grid = PropertyEntity({ type: types.local });

    this.TipoDocumento = PropertyEntity({ val: ko.observable(EnumTipoDocumentoCTe.NFeNotaFiscalEletronica), def: EnumTipoDocumentoCTe.NFeNotaFiscalEletronica, options: EnumTipoDocumentoCTe.obterOpcoes(), text: Localization.Resources.CTes.CTe.TipoDeDocumento.getRequiredFieldDescription(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
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

    this.Adicionar = PropertyEntity({ eventClick: function () { instancia.AdicionarDocumento(); }, type: types.event, text: Localization.Resources.CTes.CTe.SalvarDocumento, visible: ko.observable(true), enable: ko.observable(true) });

    this.TipoDocumento.val.subscribe(function (tipo) {
        AlterarEstadoTipoDocumento(tipo, cte);
    });

    this.Load = function () {

        cte.Documentos = new Array();

        KoBindings(instancia, cte.IdKnockoutDocumento);

        $("#" + instancia.Chave.id).mask("0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000", { selectOnFocus: true, clearIfNotMatch: true });
        $("#" + instancia.CFOP.id).mask("0000", { selectOnFocus: true, clearIfNotMatch: true });

        cte.GridDocumento = new BasicDataTable(instancia.Grid.id, headerDocumentosCTe(instancia.TipoDocumento.val()), menuOpcoesDocumentosCTe(instancia), { column: 1, dir: orderDir.asc }, null, 5);

        instancia.RecarregarGrid();
    };

    this.DestivarDocumento = function () {
        DesabilitarCamposInstanciasCTe(instancia);
        cte.GridDocumento.CarregarGrid(instancia.BuscarDocumentos(), false);
    };

    this.AdicionarDocumento = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {
            if (instancia.Codigo.val() > "0" && instancia.Codigo.val() != undefined) {
                for (var i = 0; i < cte.Documentos.length; i++) {
                    if (instancia.Codigo.val() == cte.Documentos[i].Codigo) {
                        cte.Documentos.splice(i, 1);
                        break;
                    }
                }
            }

            instancia.Codigo.val(guid());

            if (instancia.TipoDocumento.val() == EnumTipoDocumentoCTe.NFeNotaFiscalEletronica) {
                instancia.Chave.val(instancia.Chave.val().replace(/\s/g, "").replace(" ", ""));
                instancia.Numero.val(Globalize.parseInt(instancia.Chave.val().substring(25, 34)));
            }

            cte.Documentos.push(RetornarObjetoPesquisa(instancia));

            instancia.RecarregarGrid();

            LimparCampos(instancia);
            instancia.TipoDocumento.enable(true);

            instancia.AtualizarValoresMercadoria();

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    };

    this.ExcluirDocumento = function (documento) {
        if (instancia.Codigo.val() > 0) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.DocumentoEmEdicao, Localization.Resources.CTes.CTe.PorFavorVerifiqueOsDocumentosPoisExisteUmEmEdicao);
            return;
        }

        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.CTes.CTe.RealmenteDesejaExcluirDocumento, function () {
            for (var i = 0; i < cte.Documentos.length; i++) {
                if (documento.Codigo == cte.Documentos[i].Codigo) {
                    cte.Documentos.splice(i, 1);
                    break;
                }
            }

            instancia.RecarregarGrid();

            instancia.AtualizarValoresMercadoria();
        });
    };

    this.Validar = function () {
        if (cte.Documentos == null || cte.Documentos.length <= 0) {
            $('a[href="#divDocumento_' + cte.IdModal + '"]').tab("show");
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.DocumentoObrigatorio, Localization.Resources.CTes.CTe.NecessarioInformarAoMenosUmDocumento);
            return false;
        }

        return true;
    };

    this.AtualizarValoresMercadoria = function () {
        var valorTotalMercadoria = 0;

        for (var i = 0; i < cte.Documentos.length; i++) {
            var valorTotalNF = Globalize.parseFloat(cte.Documentos[i].ValorNotaFiscal);

            if (isNaN(valorTotalNF))
                valorTotalNF = 0;

            valorTotalMercadoria += valorTotalNF;
        }
        if (valorTotalMercadoria > 0)
            cte.InformacaoCarga.ValorTotalCarga.val(Globalize.format(valorTotalMercadoria, "n2"));
    };

    this.RecarregarGrid = function () {
        cte.GridDocumento.CarregarGrid(instancia.BuscarDocumentos());
    };

    this.BuscarDocumentos = function () {
        var data = new Array();

        $.each(cte.Documentos, function (i, documento) {
            if (instancia.TipoDocumento.val() === documento.TipoDocumento) {
                var documentoGrid = new Object();

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

    this.EditarDocumento = function (documento) {
        cte.Documento.Modelo.val(documento.Modelo);
        cte.Documento.Codigo.val(documento.Codigo);
        cte.Documento.Chave.val(documento.Chave);
        cte.Documento.DataEmissao.val(documento.DataEmissao);
        cte.Documento.ValorNotaFiscal.val(Globalize.format(documento.ValorNotaFiscal, "n2"));
        cte.Documento.Serie.val(documento.Serie);
        cte.Documento.Numero.val(documento.Numero);
        cte.Documento.CFOP.val(documento.CFOP);
        cte.Documento.BaseCalculoICMS.val(Globalize.format(documento.BaseCalculoICMS, "n2"));
        cte.Documento.ValorICMS.val(Globalize.format(documento.ValorICMS, "n2"));
        cte.Documento.BaseCalculoICMSST.val(Globalize.format(documento.BaseCalculoICMSST, "n2"));
        cte.Documento.ValorICMSST.val(Globalize.format(documento.ValorICMSST, "n2"));
        cte.Documento.Peso.val(Globalize.format(documento.Peso, "n2"));
        cte.Documento.PIN.val(documento.PIN);
        cte.Documento.ValorProdutos.val(Globalize.format(documento.ValorProdutos, "n2"));
        cte.Documento.Descricao.val(documento.Descricao);
        cte.Documento.NumeroReferenciaEDI.val(documento.NumeroReferenciaEDI);
        cte.Documento.NumeroControleCliente.val(documento.NumeroControleCliente);
        cte.Documento.PINSuframa.val(documento.PINSuframa);
        cte.Documento.NCMPredominante.val(documento.NCMPredominante);

        instancia.TipoDocumento.enable(false);
    };
};

function AlterarEstadoTipoDocumento(tipo, cte) {
    if (cte.GridDocumento == null)
        return;

    cte.Documento.TipoDocumento.def = tipo;

    if (tipo === EnumTipoDocumentoCTe.NFeNotaFiscalEletronica) {

        cte.Documento.Modelo.options(_modelosNFe);
        cte.Documento.Modelo.val("55");
        cte.Documento.Modelo.def = "55";
        cte.Documento.Modelo.visible(false);

        cte.Documento.Chave.required(true);
        cte.Documento.Chave.visible(true);
        cte.Documento.DataEmissao.required(true);
        cte.Documento.DataEmissao.visible(true);
        cte.Documento.ValorNotaFiscal.required(true);
        cte.Documento.ValorNotaFiscal.visible(true);

        cte.Documento.Descricao.required(false);
        cte.Documento.Descricao.visible(false);
        cte.Documento.Serie.required(false);
        cte.Documento.Serie.visible(false);
        cte.Documento.Numero.required(false);
        cte.Documento.Numero.visible(false);
        cte.Documento.CFOP.required(false);
        cte.Documento.CFOP.visible(true);
        cte.Documento.CFOP.text(Localization.Resources.CTes.CTe.CFOP.getFieldDescription());
        cte.Documento.BaseCalculoICMS.required(false);
        cte.Documento.BaseCalculoICMS.visible(false);
        cte.Documento.ValorICMS.required(false);
        cte.Documento.ValorICMS.visible(false);
        cte.Documento.BaseCalculoICMSST.required(false);
        cte.Documento.BaseCalculoICMSST.visible(false);
        cte.Documento.ValorICMSST.required(false);
        cte.Documento.ValorICMSST.visible(false);
        cte.Documento.Peso.text(Localization.Resources.CTes.CTe.PesoKg.getFieldDescription());
        cte.Documento.Peso.required(false);
        cte.Documento.Peso.visible(true);
        cte.Documento.PIN.required(false);
        cte.Documento.PIN.visible(false);
        cte.Documento.ValorProdutos.required(false);
        cte.Documento.ValorProdutos.visible(false);
        cte.Documento.Modelo.visible(false);

    } else if (tipo === EnumTipoDocumentoCTe.NotaFiscal) {

        cte.Documento.Modelo.options(EnumModelosNotaFiscal.obterOpcoes());
        cte.Documento.Modelo.val("01");
        cte.Documento.Modelo.def = "01";
        cte.Documento.Modelo.visible(true);

        cte.Documento.Serie.required(true);
        cte.Documento.Serie.visible(true);
        cte.Documento.Numero.required(true);
        cte.Documento.Numero.visible(true);
        cte.Documento.DataEmissao.required(true);
        cte.Documento.DataEmissao.visible(true);
        cte.Documento.CFOP.required(true);
        cte.Documento.CFOP.visible(true);
        cte.Documento.CFOP.text(Localization.Resources.CTes.CTe.CFOP.getRequiredFieldDescription());
        cte.Documento.BaseCalculoICMS.required(false);
        cte.Documento.BaseCalculoICMS.visible(true);
        cte.Documento.ValorICMS.required(false);
        cte.Documento.ValorICMS.visible(true);
        cte.Documento.BaseCalculoICMSST.required(false);
        cte.Documento.BaseCalculoICMSST.visible(true);
        cte.Documento.ValorICMSST.required(false);
        cte.Documento.ValorICMSST.visible(true);
        cte.Documento.Peso.required(true);
        cte.Documento.Peso.visible(true);
        cte.Documento.Peso.text(Localization.Resources.CTes.CTe.PesoKg.getRequiredFieldDescription());
        cte.Documento.PIN.required(false);
        cte.Documento.PIN.visible(true);
        cte.Documento.ValorProdutos.required(false);
        cte.Documento.ValorProdutos.visible(true);
        cte.Documento.ValorNotaFiscal.required(true);
        cte.Documento.ValorNotaFiscal.visible(true);

        cte.Documento.Descricao.required(false);
        cte.Documento.Descricao.visible(false);
        cte.Documento.Chave.required(false);
        cte.Documento.Chave.visible(false);

    } else if (tipo === EnumTipoDocumentoCTe.OutrosDocumentos) {

        cte.Documento.Modelo.options(EnumModelosOutrosDocumentos.obterOpcoes());
        cte.Documento.Modelo.val("00");
        cte.Documento.Modelo.def = "00";
        cte.Documento.Modelo.visible(true);

        cte.Documento.Descricao.required(true);
        cte.Documento.Descricao.visible(true);
        cte.Documento.Numero.required(true);
        cte.Documento.Numero.visible(true);
        cte.Documento.DataEmissao.required(true);
        cte.Documento.DataEmissao.visible(true);
        cte.Documento.ValorNotaFiscal.required(true);
        cte.Documento.ValorNotaFiscal.visible(true);

        cte.Documento.Serie.required(false);
        cte.Documento.Serie.visible(false);
        cte.Documento.CFOP.required(false);
        cte.Documento.CFOP.visible(true);
        cte.Documento.CFOP.text(Localization.Resources.CTes.CTe.CFOP.getFieldDescription());
        cte.Documento.BaseCalculoICMS.required(false);
        cte.Documento.BaseCalculoICMS.visible(false);
        cte.Documento.ValorICMS.required(false);
        cte.Documento.ValorICMS.visible(false);
        cte.Documento.BaseCalculoICMSST.required(false);
        cte.Documento.BaseCalculoICMSST.visible(false);
        cte.Documento.ValorICMSST.required(false);
        cte.Documento.ValorICMSST.visible(false);
        cte.Documento.Peso.required(false);
        cte.Documento.Peso.visible(true);
        cte.Documento.Peso.text(Localization.Resources.CTes.CTe.PesoKg);
        cte.Documento.PIN.required(false);
        cte.Documento.PIN.visible(false);
        cte.Documento.ValorProdutos.required(false);
        cte.Documento.ValorProdutos.visible(false);
        cte.Documento.Chave.required(false);
        cte.Documento.Chave.visible(false);
    }

    cte.GridDocumento.Destroy();
    cte.GridDocumento = new BasicDataTable(cte.Documento.Grid.id, headerDocumentosCTe(tipo), menuOpcoesDocumentosCTe(cte.Documento), { column: 1, dir: orderDir.asc });

    cte.Documento.RecarregarGrid();

    LimparCampos(cte.Documento);
}

function headerDocumentosCTe(tipo) {

    var visibleNFe = false;
    var visibleNotaFiscal = false;
    var visibleOutros = false;
    if (tipo === EnumTipoDocumentoCTe.NFeNotaFiscalEletronica)
        visibleNFe = true;
    else if (tipo === EnumTipoDocumentoCTe.NotaFiscal)
        visibleNotaFiscal = true;
    else if (tipo === EnumTipoDocumentoCTe.OutrosDocumentos)
        visibleOutros = true;

    return [
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
        { data: "Chave", title: Localization.Resources.CTes.CTe.Chave, width: "35%", visible: visibleNFe },
        { data: "Serie", title: Localization.Resources.CTes.CTe.Serie, width: "16%", visible: visibleNotaFiscal },
        { data: "Peso", title: Localization.Resources.CTes.CTe.PesoKg, width: "16%", visible: visibleNotaFiscal },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "35%", visible: visibleOutros },
        { data: "ValorNotaFiscal", title: Localization.Resources.CTes.CTe.ValorTotal, width: "15%" }
    ];
}

function menuOpcoesDocumentosCTe(local) {
    var editarItem = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: local.EditarDocumento };
    var excluirItem = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: local.ExcluirDocumento };

    return menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [editarItem, excluirItem] };
}