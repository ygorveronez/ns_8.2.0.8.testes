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

var EntregaSimplificadoDocumentoTransporteAnterior = function (cte) {
    var instancia = this;

    this.GridEntregasDocumentosTransporteAnterior = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoLocalidadeOrigem = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoLocalidadeDestino = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoEmitente = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Emitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.Emitente.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.Chave = PropertyEntity({ text: Localization.Resources.CTes.CTe.Chave.getRequiredFieldDescription(), required: true, visible: ko.observable(true), enable: ko.observable(true) });

    this.AdicionarDocumentoTransporteAnterior = PropertyEntity({ eventClick: function () { instancia.AdicionarDocumentoTransporteAnteriorSimplificado(); }, type: types.event, text: Localization.Resources.CTes.CTe.SalvarDocumento, visible: ko.observable(true), enable: ko.observable(false) });

    this.Load = function () {

        cte.EntregasSimplificadoDocumentosTransporteAnterior = new Array();

        KoBindings(instancia, cte.IdKnockoutEntregaSimplificadoDocumentoTransporteAnterior);

        new BuscarClientes(instancia.Emitente);

        $("#" + instancia.Chave.id).mask("0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000", { selectOnFocus: true, clearIfNotMatch: true });

        cte.GridEntregasDocumentosTransporteAnterior = new BasicDataTable(instancia.GridEntregasDocumentosTransporteAnterior.id, headerDocumentosTransporteAnteriorCTeSimplificado(), menuOpcoesDocumentosTransporteAnteriorCTeSimplificado(instancia), { column: 1, dir: orderDir.asc }, null, 5);

        instancia.RecarregarGridSimplificadoDocumentosTransporteAnterior();
    };

    this.AdicionarDocumentoTransporteAnteriorSimplificado = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {
            for (var i = 0; i < cte.EntregasSimplificadoDocumentosTransporteAnterior.length; i++) {
                if (instancia.Codigo.val() > "0" && instancia.Codigo.val() != undefined) {
                    if (instancia.Codigo.val() == cte.EntregasSimplificadoDocumentosTransporteAnterior[i].Codigo) {
                        continue;
                    }
                }

                if (instancia.Chave.val().replace(/\s/g, "").replace(" ", "") == cte.EntregasSimplificadoDocumentosTransporteAnterior[i].Chave) {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.DocumentoEmEdicao, "Chave de acesso já informada!");
                    return;
                }
            }

            if (instancia.Codigo.val() > "0" && instancia.Codigo.val() != undefined) {
                for (var i = 0; i < cte.EntregasSimplificadoDocumentosTransporteAnterior.length; i++) {
                    if (instancia.Codigo.val() == cte.EntregasSimplificadoDocumentosTransporteAnterior[i].Codigo) {
                        cte.EntregasSimplificadoDocumentosTransporteAnterior.splice(i, 1);
                        break;
                    }
                }
            }

            instancia.Codigo.val(guid());
            instancia.CodigoLocalidadeOrigem.val(cte.EntregaSimplificado.LocalidadeOrigem.codEntity());
            instancia.CodigoLocalidadeDestino.val(cte.EntregaSimplificado.LocalidadeDestino.codEntity());
            instancia.Chave.val(instancia.Chave.val().replace(/\s/g, "").replace(" ", ""));
            instancia.CodigoEmitente.val(instancia.Emitente.codEntity());
            instancia.Emitente.val(instancia.Emitente.val());

            cte.EntregasSimplificadoDocumentosTransporteAnterior.push(RetornarObjetoPesquisa(instancia));

            instancia.RecarregarGridSimplificadoDocumentosTransporteAnterior();

            LimparCampos(instancia);

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    };

    this.ExcluirDocumentoTransporteAnteriorSimplificado = function (documento) {
        if (instancia.Codigo.val() > 0) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.DocumentoEmEdicao, Localization.Resources.CTes.CTe.PorFavorVerifiqueOsDocumentosPoisExisteUmEmEdicao);
            return;
        }

        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.CTes.CTe.RealmenteDesejaExcluirDocumento, function () {
            for (var i = 0; i < cte.EntregasSimplificadoDocumentosTransporteAnterior.length; i++) {
                if (documento.Codigo == cte.EntregasSimplificadoDocumentosTransporteAnterior[i].Codigo) {
                    cte.EntregasSimplificadoDocumentosTransporteAnterior.splice(i, 1);
                    break;
                }
            }

            instancia.RecarregarGridSimplificadoDocumentosTransporteAnterior();
        });
    };

    this.RecarregarGridSimplificadoDocumentosTransporteAnterior = function () {
        cte.GridEntregasDocumentosTransporteAnterior.CarregarGrid(instancia.BuscarDocumentosTransporteAnteriorSimplificado(), cte.EntregaSimplificado.PermitirEdicao.val());
    };

    this.BuscarDocumentosTransporteAnteriorSimplificado = function () {
        var data = new Array();

        $.each(cte.EntregasSimplificadoDocumentosTransporteAnterior, function (i, documento) {
            if (cte.EntregaSimplificado.LocalidadeOrigem.codEntity() === documento.CodigoLocalidadeOrigem && cte.EntregaSimplificado.LocalidadeDestino.codEntity() === documento.CodigoLocalidadeDestino) {
                var documentoGrid = new Object();

                documentoGrid.CodigoLocalidadeOrigem = documento.CodigoLocalidadeOrigem;
                documentoGrid.CodigoLocalidadeDestino = documento.CodigoLocalidadeDestino;
                documentoGrid.Codigo = documento.Codigo;
                documentoGrid.Chave = documento.Chave;
                documentoGrid.CodigoEmitente = documento.CodigoEmitente;
                documentoGrid.Emitente = documento.Emitente;

                data.push(documentoGrid);
            }
        });
        return data;
    };

    this.EditarDocumentoTransporteAnteriorSimplificado = function (documento) {
        cte.EntregaSimplificadoDocumentoTransporteAnterior.Codigo.val(documento.Codigo);
        cte.EntregaSimplificadoDocumentoTransporteAnterior.Chave.val(documento.Chave);
        cte.EntregaSimplificadoDocumentoTransporteAnterior.Emitente.codEntity(documento.CodigoEmitente);
        cte.EntregaSimplificadoDocumentoTransporteAnterior.Emitente.val(documento.Emitente);
    };
};

function headerDocumentosTransporteAnteriorCTeSimplificado() {

    return [
        { data: "CodigoLocalidadeOrigem", visible: false },
        { data: "CodigoLocalidadeDestino", visible: false },
        { data: "Codigo", visible: false },
        { data: "Chave", title: Localization.Resources.CTes.CTe.Chave, width: "50%" },
        { data: "Emitente", title: Localization.Resources.CTes.CTe.Emitente, width: "30%" },
        { data: "CodigoEmitente", visible: false}
    ];
}

function menuOpcoesDocumentosTransporteAnteriorCTeSimplificado(local) {
    var editarItem = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: local.EditarDocumentoTransporteAnteriorSimplificado };
    var excluirItem = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: local.ExcluirDocumentoTransporteAnteriorSimplificado };

    return menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [editarItem, excluirItem] };
}