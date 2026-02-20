/// <reference path="../../Consultas/Cliente.js" />
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
/// <reference path="../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../Enumeradores/EnumTipoCTe.js" />
/// <reference path="CTeImportarXMLNFe.js" />
/// <reference path="CTeImportarNFeSiteSEFAZ.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridConhecimentoEletronico;
var _pesquisaConhecimentoEletronico;
var _codigosVeiculosCarga, _codigosMotoristasCarga;
var _modalSolicitacaoCancelamentoCTe;

var _statusCTe = [
    { text: "Todos", value: EnumStatusCTe.TODOS },
    { text: "Pendente", value: EnumStatusCTe.PENDENTE },
    { text: "Enviado", value: EnumStatusCTe.ENVIADO },
    { text: "Rejeição", value: EnumStatusCTe.REJEICAO },
    { text: "Autorizado", value: EnumStatusCTe.AUTORIZADO },
    { text: "Cancelado", value: EnumStatusCTe.CANCELADO },
    { text: "Inutilizado", value: EnumStatusCTe.INUTILIZADO },
    { text: "Denegado", value: EnumStatusCTe.DENEGADO },
    { text: "Em Digitação", value: EnumStatusCTe.EMDIGITACAO },
    { text: "Em Cancelamento", value: EnumStatusCTe.EMCANCELAMENTO },
    { text: "Em Inutilização", value: EnumStatusCTe.EMINUTILIZACAO },
    { text: "Anulado", value: EnumStatusCTe.ANULADO }
];

var _finalidadeCTe = [
    { text: "Todos", value: -1 },
    { text: "Normal", value: EnumTipoCTe.Normal },
    { text: "Complementar", value: EnumTipoCTe.Complementar },
    { text: "Anulação", value: EnumTipoCTe.Anulacao },
    { text: "Substituição", value: EnumTipoCTe.Substituicao }
];

var PesquisaConhecimentoEletronico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int });
    this.Finalidade = PropertyEntity({ val: ko.observable(0), options: _finalidadeCTe, def: 0, text: "Tipo: " });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusCTe.TODOS), options: _statusCTe, def: EnumStatusCTe.TODOS, text: "Status: " });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente: ", idBtnSearch: guid(), visible: true });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário: ", idBtnSearch: guid(), visible: true });
    this.ChaveCTe = PropertyEntity({ text: "Chave: " });

    this.NovoConhecimentoEletronico = PropertyEntity({ eventClick: NovoConhecimentoEletronicoClick, type: types.event, text: "Novo CT-e", icon: ko.observable("fal fa-plus"), idGrid: guid(), visible: ko.observable(true) });
    this.CTeImportarNFeSEFAZ = PropertyEntity({ eventClick: CTeImportarNFeSEFAZClick, type: types.event, text: "Importar NF-e da SEFAZ", visible: ko.observable(true) });
    this.ArquivoNFe = PropertyEntity({ type: types.file, eventChange: function () { }, codEntity: ko.observable(0), text: "NF-e(s):", val: ko.observable(""), visible: ko.observable(false) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConhecimentoEletronico.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};

var CancelamentoConhecimento = function () {
    this.Observacao = PropertyEntity({ type: types.map, text: "*Justificativa: ", required: true });
    this.CodigoCTe = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.SituacaoCTe = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.ConfirmarCancelamento = PropertyEntity({ eventClick: confirmarConfirmarCancelamentoCTeClick, type: types.event, text: "Confirmar", visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadConhecimentoEletronico() {
    _pesquisaConhecimentoEletronico = new PesquisaConhecimentoEletronico();
    KoBindings(_pesquisaConhecimentoEletronico, "knockoutPesquisaConhecimentoEletronico", false, _pesquisaConhecimentoEletronico.Pesquisar.id);

    new BuscarClientes(_pesquisaConhecimentoEletronico.Remetente);
    new BuscarClientes(_pesquisaConhecimentoEletronico.Destinatario);

    buscarConhecimentoEletronicos();
    LoadCTeImportarXMLNFe();
    LoadImportarNFeSiteSEFAZ();

    _modalSolicitacaoCancelamentoCTe = new bootstrap.Modal(document.getElementById("divModalSolicitarCancelamento"), { backdrop: 'static', keyboard: true });
}

function NovoConhecimentoEletronicoClick(e, sender) {
    abrirModalCTe(0);
}

function abrirModalCTe(codigoCTe, documentos, duplicar) {
    let permissoesTotal = [EnumPermissoesEdicaoCTe.total];

    let instanciaEmissao = new EmissaoCTe(codigoCTe, function () {

        instanciaEmissao.CRUDCTe.Emitir.visible(true);
        instanciaEmissao.CRUDCTe.Salvar.visible(true);

        instanciaEmissao.CRUDCTe.Salvar.eventClick = function () {
            if (instanciaEmissao.Validar() === true) {
                let objetoCTe = ObterObjetoCTe(instanciaEmissao);
                SalvarCTe(objetoCTe, instanciaEmissao);
            }
        };

        instanciaEmissao.CRUDCTe.Emitir.eventClick = function () {
            if (instanciaEmissao.Validar() === true) {
                let objetoCTe = ObterObjetoCTe(instanciaEmissao);
                EmitirCTe(objetoCTe, instanciaEmissao);
            }
        };

        instanciaEmissao.TotalServico.IncluirICMSFrete.val(true);
        instanciaEmissao.Componente.IncluirBaseCalculoICMS.def = true;
        instanciaEmissao.Componente.IncluirBaseCalculoICMS.val(true);

        instanciaEmissao.ObterInformacoesModalRodoviario(_codigosVeiculosCarga, _codigosMotoristasCarga);

        if (documentos != null) {

            let peso = 0;

            for (let i = 0; i < documentos.length; i++) {
                var documentoLista = documentos[i];

                instanciaEmissao.Documento.TipoDocumento.val(EnumTipoDocumentoCTe.NFeNotaFiscalEletronica);
                instanciaEmissao.Documento.Numero.val(documentoLista.Numero);
                instanciaEmissao.Documento.Chave.val(documentoLista.Chave);
                instanciaEmissao.Documento.DataEmissao.val(documentoLista.DataEmissao);
                instanciaEmissao.Documento.ValorNotaFiscal.val(Globalize.format(documentoLista.ValorTotal, "n2"));
                instanciaEmissao.Documento.Peso.val(Globalize.format(documentoLista.Peso, "n2"));

                instanciaEmissao.Documento.AdicionarDocumento();

                peso += documentoLista.Peso;
            }

            let documento = documentos[0];

            instanciaEmissao.Remetente.CPFCNPJ.val(documento.Remetente);
            instanciaEmissao.Remetente.BuscarDadosPorCPFCNPJ();

            instanciaEmissao.Destinatario.CPFCNPJ.val(documento.Destinatario);
            instanciaEmissao.Destinatario.BuscarDadosPorCPFCNPJ();

            instanciaEmissao.QuantidadeCarga.Quantidade.val(Globalize.format(peso, "n4"));
            instanciaEmissao.QuantidadeCarga.UnidadeMedida.val(documento.UnidadeMedida.Descricao);
            instanciaEmissao.QuantidadeCarga.UnidadeMedida.codEntity(documento.UnidadeMedida.Codigo);
            instanciaEmissao.QuantidadeCarga.AdicionarQuantidadeCarga();

            if (documento.FormaPagamento == 0)
                instanciaEmissao.CTe.TipoPagamento.val(EnumTipoPagamento.Pago);
            else if (documento.FormaPagamento == 1)
                instanciaEmissao.CTe.TipoPagamento.val(EnumTipoPagamento.A_Pagar);
            else
                instanciaEmissao.CTe.TipoPagamento.val(EnumTipoPagamento.Outros);
        }
    }, permissoesTotal, null, duplicar);


}

function confirmarConfirmarCancelamentoCTeClick(e, sender) {
    if (ValidarCamposObrigatorios(e)) {
        if (e.Observacao.val().length >= 20) {
            let url = "CargaCTe/CancelarCTe";
            if (e.SituacaoCTe.val() == EnumStatusCTe.REJEICAO || e.SituacaoCTe.val() == EnumStatusCTe.EMDIGITACAO)
                url = "CargaCTe/InutilizarCTe";

            let data = { CodigoCTe: e.CodigoCTe.val(), Justificativa: e.Observacao.val() };
            executarReST(url, data, function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        _gridConhecimentoEletronico.CarregarGrid();
                        _modalSolicitacaoCancelamentoCTe.hide();
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação de cancelamento/inutilização enviada com sucesso");
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 20000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
                }
            });
        } else {
            exibirMensagem(tipoMensagem.atencao, "Dados Obrigatório", "É obrigatório que a justificativa tenha ao menos 20 caracteres.");
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatório", "É obrigatório informar uma justificativa.");
    }
}

//*******MÉTODOS*******

function EmitirCTe(cte, instancia) {
    let dados = { CTe: cte };
    executarReST("ConhecimentoEletronico/EmitirCTe", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                instancia.FecharModal();
                _gridConhecimentoEletronico.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "CT-e emitido com sucesso");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    });
}

function SalvarCTe(cte, instancia) {
    let dados = { CTe: cte };
    executarReST("ConhecimentoEletronico/SalvarCTe", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                instancia.FecharModal();
                _gridConhecimentoEletronico.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "CT-e salvo com sucesso");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    });
}

function buscarConhecimentoEletronicos() {

    let baixarDACTE = { descricao: "Baixar DACTE", id: guid(), metodo: baixarDACTEConhecimentoEletronico, icone: "" };
    let baixarXML = { descricao: "Baixar XML", id: guid(), metodo: baixarXMLConhecimentoEletronico, icone: "" };
    let baixarXMLCancelamento = { descricao: "Baixar XML Cancelamento", id: guid(), metodo: baixarXMLCancelamentoConhecimentoEletronico, icone: "" };
    let editar = { descricao: "Editar", id: guid(), metodo: editarConhecimentoEletronico, icone: "" };
    let autorizar = { descricao: "Autorizar", id: guid(), metodo: autorizarConhecimentoEletronico, icone: "" };
    let duplicar = { descricao: "Duplicar", id: guid(), metodo: duplicarConhecimentoEletronico, icone: "" };
    let cancelarCTe = { descricao: "Cancelar CT-e", id: guid(), metodo: cancelarCTeConhecimentoEletronico, icone: "" };
    let inutilizarCTe = { descricao: "Inutilizar CT-e", id: guid(), metodo: inutilizarCTeConhecimentoEletronico, icone: "" };
    let reenviarEmail = { descricao: "Reenviar e-mails", id: guid(), metodo: reenviarEmailConhecimentoEletronico, icone: "" };
    let cartaCorrecao = { descricao: "Carta de Correção", id: guid(), metodo: cartaCorrecaoConhecimentoEletronico, icone: "" };
    let baixarPreCTe = { descricao: "Baixar Pré-CT-e", id: guid(), metodo: baixarPreCTeConhecimentoEletronico, icone: "" };
    let visualizarPreDACTE = { descricao: "Visualizar Pré-DACTE", id: guid(), metodo: visualizarPreDACTeConhecimentoEletronico, icone: "" };
    let auditar = { descricao: "Auditoria", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("ConhecimentoDeTransporteEletronico", "Codigo"), tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };

    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [baixarDACTE, baixarXML, baixarXMLCancelamento, editar, autorizar, duplicar, cancelarCTe, inutilizarCTe, reenviarEmail, visualizarPreDACTE, auditar] };

    _gridConhecimentoEletronico = new GridView(_pesquisaConhecimentoEletronico.Pesquisar.idGrid, "ConhecimentoEletronico/Pesquisa", _pesquisaConhecimentoEletronico, menuOpcoes, null);
    _gridConhecimentoEletronico.CarregarGrid();
}

function baixarPreCTeConhecimentoEletronico(conhecimentoEletronicoGrid) {

}

function cartaCorrecaoConhecimentoEletronico(conhecimentoEletronicoGrid) {

}

function reenviarEmailConhecimentoEletronico(conhecimentoEletronicoGrid) {
    if (conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.ANULADO || conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.AUTORIZADO || conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.CANCELADO) {
        let data = { CodigoCTe: conhecimentoEletronicoGrid.CodigoCTE, CodigoEmpresa: conhecimentoEletronicoGrid.CodigoEmpresa };
        executarReST("ConhecimentoEletronico/EnviarEmailParaTodos", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação de envio do e-mail feita com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Situação não permite emissão", "A atual situação do CT-e (" + conhecimentoEletronicoGrid.Status + ")  não permite que ele seja emitido novamente");
    }
}

function inutilizarCTeConhecimentoEletronico(conhecimentoEletronicoGrid) {
    if (conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.REJEICAO || conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.EMDIGITACAO) {
        let knoutJustificativa = new CancelamentoConhecimento();
        knoutJustificativa.CodigoCTe.val(conhecimentoEletronicoGrid.CodigoCTE);
        knoutJustificativa.SituacaoCTe.val(conhecimentoEletronicoGrid.SituacaoCTe);
        KoBindings(knoutJustificativa, "knoutSolicitarCancelamento");
        _modalSolicitacaoCancelamentoCTe.show();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Situação não permite a inutilização", "A atual situação do CT-e (" + conhecimentoEletronicoGrid.Status + ")  não permite que ele seja inutilizado");
    }
}

function cancelarCTeConhecimentoEletronico(conhecimentoEletronicoGrid) {
    if (conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.AUTORIZADO) {
        let knoutJustificativa = new CancelamentoConhecimento();
        knoutJustificativa.CodigoCTe.val(conhecimentoEletronicoGrid.CodigoCTE);
        knoutJustificativa.SituacaoCTe.val(conhecimentoEletronicoGrid.SituacaoCTe);
        KoBindings(knoutJustificativa, "knoutSolicitarCancelamento");
        _modalSolicitacaoCancelamentoCTe.show();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Situação não permite o cancelamento", "A atual situação do CT-e (" + conhecimentoEletronicoGrid.Status + ")  não permite que ele seja cancelado");
    }
}

function duplicarConhecimentoEletronico(conhecimentoEletronicoGrid) {
    abrirModalCTe(parseInt(conhecimentoEletronicoGrid.CodigoCTE), null, true);
}

function autorizarConhecimentoEletronico(conhecimentoEletronicoGrid) {
    if (conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.REJEICAO || conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.EMDIGITACAO || conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.PENDENTE) {
        let data = { CodigoCTe: conhecimentoEletronicoGrid.CodigoCTE, CodigoEmpresa: conhecimentoEletronicoGrid.CodigoEmpresa };
        executarReST("ConhecimentoEletronico/EmitirNovamente", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridConhecimentoEletronico.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Situação não permite emissão", "A atual situação do CT-e (" + conhecimentoEletronicoGrid.Status + ")  não permite que ele seja emitido novamente");
    }
}

function editarConhecimentoEletronico(conhecimentoEletronicoGrid) {
    if (conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.EMDIGITACAO || conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.PENDENTE || conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.REJEICAO) {
        abrirModalCTe(parseInt(conhecimentoEletronicoGrid.CodigoCTE));
    } else {
        exibirMensagem(tipoMensagem.atencao, "Situação não permite editar o CT-e", "A atual situação do CT-e (" + conhecimentoEletronicoGrid.Status + ")  não permite esta operação");
    }
}

function baixarXMLCancelamentoConhecimentoEletronico(conhecimentoEletronicoGrid) {
    if (conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.CANCELADO) {
        let data = { CodigoCTe: conhecimentoEletronicoGrid.Codigo, CodigoEmpresa: 0 };
        executarDownload("CargaCTe/DownloadXMLCancelamento", data);
    } else {
        exibirMensagem(tipoMensagem.atencao, "Situação não permite baixar o XML do Cancelamento do CT-e", "A atual situação do CT-e (" + conhecimentoEletronicoGrid.Status + ")  não permite esta operação");
    }
}

function baixarDACTEConhecimentoEletronico(conhecimentoEletronicoGrid) {
    if (conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.AUTORIZADO || conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.ANULADO || conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.CANCELADO) {
        let data = { CodigoCTe: conhecimentoEletronicoGrid.Codigo, CodigoEmpresa: 0 };
        executarDownload("CargaCTe/DownloadDacte", data);
    } else {
        exibirMensagem(tipoMensagem.atencao, "Situação não permite baixar a DACTE do CT-e", "A atual situação do CT-e (" + conhecimentoEletronicoGrid.Status + ")  não permite esta operação");
    }
}

function baixarXMLConhecimentoEletronico(conhecimentoEletronicoGrid) {
    if (conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.AUTORIZADO || conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.ANULADO || conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.CANCELADO) {
        let data = { CodigoCTe: conhecimentoEletronicoGrid.Codigo, CodigoEmpresa: 0 };
        executarDownload("CargaCTe/DownloadXML", data);
    } else {
        exibirMensagem(tipoMensagem.atencao, "Situação não permite baixar o XML do CT-e", "A atual situação do CT-e (" + conhecimentoEletronicoGrid.Status + ")  não permite esta operação");
    }
}

function visualizarPreDACTeConhecimentoEletronico(conhecimentoEletronicoGrid) {
    if (conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.EMDIGITACAO || conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.PENDENTE || conhecimentoEletronicoGrid.SituacaoCTe === EnumStatusCTe.REJEICAO) {
        let data = { CodigoCTe: conhecimentoEletronicoGrid.Codigo };
        executarReST("ConhecimentoEletronico/VisualizarPreDACTE", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Situação não permite visualizar a Pré-DACTE", "A atual situação do CT-e (" + conhecimentoEletronicoGrid.Status + ")  não permite esta operação");
    }
}

function limparCamposConhecimentoEletronico() {

}
