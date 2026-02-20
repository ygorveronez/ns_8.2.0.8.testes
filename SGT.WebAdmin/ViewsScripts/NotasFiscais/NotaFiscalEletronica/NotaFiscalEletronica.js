/// <reference path="../../Enumeradores/EnumPermissoesEdicaoNFe.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../../Areas/Relatorios/ViewsScripts/Relatorios/Global/Relatorio.js" />
/// <reference path="../../Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoNFe.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../Consultas/Atividade.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/NaturezaOperacao.js" />
/// <reference path="../../Enumeradores/EnumStatusNFe.js" />
/// <reference path="../NFe/NFe.js" />
/// <reference path="../../Consultas/PedidoVenda.js" />
/// <reference path="../../Consultas/DocumentoEntrada.js" />
/// <reference path="Integracoes.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridNotaFiscalEletronica;
var _pesquisaNotaFiscalEletronica;
var _justificativaCancelamento;
var _correcaoCartaCorrecao;
var _gridPedidos;
var _gridDocumentosEntrada;
var _inutilizarFaixaNotas;

var _gridDANFERelatorio;
var _gridCCeNFeRelatorio;

var _statusNFePesquisa = [
    { text: "Todos", value: 0 },
    { text: "Em Digitação", value: EnumStatusNFe.Emitido },
    { text: "Inutilizado", value: EnumStatusNFe.Inutilizado },
    { text: "Cancelado", value: EnumStatusNFe.Cancelado },
    { text: "Autorizado", value: EnumStatusNFe.Autorizado },
    { text: "Denegado", value: EnumStatusNFe.Denegado },
    { text: "Rejeitado", value: EnumStatusNFe.Rejeitado },
    { text: "Em Processamento", value: EnumStatusNFe.EmProcessamento },
    { text: "Aguardando Assinatura", value: EnumStatusNFe.AguardandoAssinar },
    { text: "Aguardando Cancelamento do XML", value: EnumStatusNFe.AguardandoCancelarAssinar },
    { text: "Aguardando Inutilizacao do XML", value: EnumStatusNFe.AguardandoInutilizarAssinar },
    { text: "Aguardando Carta Correção do XML", value: EnumStatusNFe.AguardandoCartaCorrecaoAssinar }
];

var PesquisaNotaFiscalEletronica = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa: ", idBtnSearch: guid(), visible: true });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Serie = PropertyEntity({ text: "Série: ", getType: typesKnockout.int });
    this.NaturezaOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Natureza da Operação: ", idBtnSearch: guid(), visible: true });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Status = PropertyEntity({ val: ko.observable(0), options: _statusNFePesquisa, def: 0, text: "Status: " });
    this.Chave = PropertyEntity({ text: "Chave: ", getType: typesKnockout.string, maxlength: 44 });

    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.Visible2 = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.Atividade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Atividade: ", idBtnSearch: guid(), visible: true });
    this.DataProcessamento = PropertyEntity({ text: "Data Processamento: ", getType: typesKnockout.date });
    this.DataSaida = PropertyEntity({ text: "Data Saída: ", getType: typesKnockout.date });
    this.TipoEmissao = PropertyEntity({ val: ko.observable(EnumTipoEmissaoNFe.Todos), options: EnumTipoEmissaoNFe.obterOpcoesPesquisa(), def: EnumTipoEmissaoNFe.Todos, text: "Tipo Emissão: " });

    this.NovaNotaFiscalEletronica = PropertyEntity({ eventClick: NovaNotaEletronicaClick, type: types.event, text: "Nova NF-e", icon: ko.observable("fal fa-plus"), idGrid: guid(), visible: ko.observable(true) });
    this.ImportarPedido = PropertyEntity({ type: types.map, required: false, text: "Importar de Pedido/OS", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
    this.ImportarDocumentoEntrada = PropertyEntity({ type: types.map, required: false, text: "Importar de Documento Entrada", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.Visible.visibleFade() === true) {
                e.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançadas", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
    this.OpcoesAvancadas = PropertyEntity({
        eventClick: function (e) {
            if (e.Visible2.visibleFade() === true) {
                e.Visible2.visibleFade(false);
            } else {
                e.Visible2.visibleFade(true);
            }
        }, type: types.event, text: "Opções Avançadas", icon: ko.observable("fal fa-gear"), visible: ko.observable(true)
    });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridNotaFiscalEletronica.CarregarGrid();
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

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.BaixarAssinador = PropertyEntity({ eventClick: BaixarAssinadorClick, type: types.event, text: "Download Assinador NF-e", icon: ko.observable("fal fa-download"), idGrid: guid(), visible: ko.observable(true) });
    this.BaixarArquivosNFe = PropertyEntity({ eventClick: BaixarArquivosNFeClick, type: types.event, text: "Download Arquivos NF-e", icon: ko.observable("fal fa-download"), idGrid: guid(), visible: ko.observable(true) });
    this.InutilizarFaixaNFe = PropertyEntity({ eventClick: InutilizarFaixaNFeClick, type: types.event, text: "Inutilizar Faixa", icon: ko.observable("fal fa-download"), idGrid: guid(), visible: ko.observable(true) });

    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo NF-e:", val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(true) });
    this.ImportarNFe = PropertyEntity({ eventClick: ImportarNFeClick, type: types.event, text: "Importar TXT NF-e", visible: ko.observable(true) });
};

var JustificativaCancelamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.dateTime, text: "*Data de Emissão:", required: true, enable: ko.observable(true), val: ko.observable(Global.DataAtual()), def: ko.observable(Global.DataAtual()) });
    this.Justificativa = PropertyEntity({ getType: typesKnockout.string, required: true, maxlength: 300, text: "*Justificativa do Cancelamento:" });

    this.Cancelar = PropertyEntity({ type: types.event, eventClick: CancelarNotaFiscalClick, text: "Cancelar NF-e", visible: ko.observable(true), required: true });
};

var CartaCorrecao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.dateTime, text: "*Data de Emissão:", required: true, enable: ko.observable(true), val: ko.observable(Global.DataAtual()), def: ko.observable(Global.DataAtual()) });
    this.Correcao = PropertyEntity({ getType: typesKnockout.string, required: true, maxlength: 300, text: "*Correção:" });

    this.Corrigir = PropertyEntity({ type: types.event, eventClick: CorrigirNotaFiscalClick, text: "Corrigir NF-e", visible: ko.observable(true), required: true });
};

var InutilizarFaixaNotas = function () {
    this.Empresa = PropertyEntity({ text: "*Empresa:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), required: _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiNFe });
    this.NumeroInicial = PropertyEntity({ text: "*Número Inicial:", required: true, getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "*Número Final:", required: true, getType: typesKnockout.int });
    this.Serie = PropertyEntity({ text: "*Série:", required: true, getType: typesKnockout.int });
    this.Justificativa = PropertyEntity({ text: "*Justificativa:", required: true });
    this.Enviar = PropertyEntity({ text: "Enviar", eventClick: InutilizarFaixaNotasClick, enable: ko.observable(true), visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadNotaFiscalEletronica() {
    _pesquisaNotaFiscalEletronica = new PesquisaNotaFiscalEletronica();
    KoBindings(_pesquisaNotaFiscalEletronica, "knockoutPesquisaNotaFiscalEletronica", false, _pesquisaNotaFiscalEletronica.Pesquisar.id);

    _justificativaCancelamento = new JustificativaCancelamento();
    KoBindings(_justificativaCancelamento, "knoutModalCancelarNFe");

    _correcaoCartaCorrecao = new CartaCorrecao();
    KoBindings(_correcaoCartaCorrecao, "knoutModalCartaCorrecaoNFe");

    _inutilizarFaixaNotas = new InutilizarFaixaNotas();
    KoBindings(_inutilizarFaixaNotas, "knoutModalInutilizarFaixaNotas")

    //HeaderAuditoria("NotaFiscal", {});

    CarregarGridPerdidos();
    CarregarGridDocumentosEntrada();

    new BuscarNaturezasOperacoesNotaFiscal(_pesquisaNotaFiscalEletronica.NaturezaOperacao, null, null, null, null, null, null, null, null, null, true);
    new BuscarAtividades(_pesquisaNotaFiscalEletronica.Atividade);
    new BuscarClientes(_pesquisaNotaFiscalEletronica.Pessoa);
    new BuscarEmpresa(_pesquisaNotaFiscalEletronica.Empresa);
    new BuscarEmpresa(_inutilizarFaixaNotas.Empresa);
    new BuscarPedidosVendas(_pesquisaNotaFiscalEletronica.ImportarPedido, retornoPedidosVendas, null, _gridPedidos, EnumStatusPedidoVenda.AbertaFinalizada);
    new BuscarDocumentoEntrada(_pesquisaNotaFiscalEletronica.ImportarDocumentoEntrada, retornoDocumentosEntrada, null, null, _gridDocumentosEntrada);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _pesquisaNotaFiscalEletronica.Empresa.visible(false);
        _inutilizarFaixaNotas.Empresa.visible(false);
    }
    buscarNotaFiscalEletronicas();
    LoadNotaFiscalEletronicaIntegracoes();
}

function retornoPedidosVendas(pedidos) {
    limparCamposNotaFiscalEletronica();

    var codigos = new Array();
    $.each(pedidos, function (i, pedido) {
        codigos.push({ Codigo: pedido.Codigo });
    });

    abrirModalNFe(0, codigos);
}

function retornoDocumentosEntrada(documentos) {
    limparCamposNotaFiscalEletronica();

    var codigos = new Array();
    $.each(documentos, function (i, documento) {
        codigos.push({ Codigo: documento.Codigo });
    });

    abrirModalNFe(0, null, codigos);
}

function BaixarAssinadorClick(e, sender) {
    executarDownload("NotaFiscalEletronica/DownloadAssinador", { Codigo: 0 });
}

function BaixarArquivosNFeClick(e, sender) {
    executarDownload("NotaFiscalEletronica/DownloadArquivosNFe", { Codigo: 0 });
}

function InutilizarFaixaNFeClick(e, sender) {
    LimparCampos(_inutilizarFaixaNotas);
    Global.abrirModal('divInutilizarFaixaNotaFiscalEletronica');
}

function InutilizarFaixaNotasClick(e, sender) {
    if (!ValidarCamposObrigatorios(_inutilizarFaixaNotas)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    exibirConfirmacao("Atenção", "Deseja realmente inutilizar a faixa de notas da série selecionada? Lembrando que o processo é irreversível!", function () {
        Salvar(_inutilizarFaixaNotas, "NotaFiscalEletronica/InutilizarFaixaNotas", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Msg);
                    LimparCampos(_inutilizarFaixaNotas);
                    Global.fecharModal('divInutilizarFaixaNotaFiscalEletronica');
                    _gridNotaFiscalEletronica.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function NovaNotaEletronicaClick(e, sender) {
    abrirModalNFe(0);
}

function ImportarNFeClick(e, sender) {
    var file = document.getElementById(_pesquisaNotaFiscalEletronica.Arquivo.id);

    var formData = new FormData();
    formData.append("upload", file.files[0]);
    _pesquisaNotaFiscalEletronica.Arquivo.requiredClass("form-control");

    if (_pesquisaNotaFiscalEletronica.Arquivo.val() != "") {
        enviarArquivo("NotaFiscalEletronica/ImportarNFe?callback=?", { Codigo: _pesquisaNotaFiscalEletronica.Codigo.val() }, formData, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);

                _pesquisaNotaFiscalEletronica.Arquivo.requiredClass("form-control");
                _pesquisaNotaFiscalEletronica.Arquivo.val("");
                _gridNotaFiscalEletronica.CarregarGrid();

            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                return;
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios para a importação!");
        if (_pesquisaNotaFiscalEletronica.Arquivo.val() == "")
            _pesquisaNotaFiscalEletronica.Arquivo.requiredClass("form-control is-invalid");
    }

}

//*******MÉTODOS*******

function CarregarGridPerdidos() {
    var pedidos = new Array();

    var header = [{ data: "Codigo", visible: false },
    { data: "Numero", visible: false },
    { data: "DataEmissao", visible: false },
    { data: "Cliente", visible: false },
    { data: "DescricaoTipo", visible: false },
    { data: "DescricaoStatus", visible: false },
    { data: "ValorTotal", visible: false }
    ];

    _gridPedidos = new BasicDataTable(_pesquisaNotaFiscalEletronica.ImportarPedido.idGrid, header);
    _gridPedidos.CarregarGrid(pedidos);
}

function CarregarGridDocumentosEntrada() {
    var documentos = new Array();

    var header = [{ data: "Codigo", visible: false }];

    _gridDocumentosEntrada = new BasicDataTable(_pesquisaNotaFiscalEletronica.ImportarDocumentoEntrada.idGrid, header);
    _gridDocumentosEntrada.CarregarGrid(documentos);
}

function abrirModalNFe(codigoNFe, codigosPedido, codigosDocumentoEntrada) {
    var permissoesTotal = new Array();
    permissoesTotal.push(EnumPermissoesEdicaoNFe.total);
    var instanciaEmissao = new EmissaoNFe(codigoNFe, function () {
        instanciaEmissao.CRUDNFe.Salvar.visible(true);
        instanciaEmissao.CRUDNFe.Emitir.visible(true);
        instanciaEmissao.CRUDNFe.Salvar.eventClick = function () {
            var objetoNFe = ObterObjetoNFe(instanciaEmissao);
            SalvarNFe(objetoNFe, instanciaEmissao);
        };
        instanciaEmissao.CRUDNFe.Emitir.eventClick = function () {
            var objetoNFe = ObterObjetoNFe(instanciaEmissao);
            EmitirNFe(objetoNFe, instanciaEmissao);
        };
    }, permissoesTotal, codigosPedido, codigosDocumentoEntrada);

}

function CorrigirNotaFiscalClick(e) {
    if (ValidarCamposObrigatorios(e)) {
        if (e.Correcao.val().length >= 15) {
            var dados = { Codigo: e.Codigo.val(), Correcao: e.Correcao.val(), DataEmissao: e.DataEmissao.val() };
            executarReST("NotaFiscalEletronica/CorrigirNFe", dados, function (arg) {
                if (arg.Success) {
                    if (arg.Data != false) {
                        LimparCampos(_correcaoCartaCorrecao);
                        Global.abrirModal('divModalCartaCorrecaoNFe');
                        _gridNotaFiscalEletronica.CarregarGrid();
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "NF-e corrigida com sucesso");
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        } else {
            exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe mais que 15 caracteres.");
        }
    } else
        exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
}

function CancelarNotaFiscalClick(e) {
    if (ValidarCamposObrigatorios(e)) {
        if (e.Justificativa.val().length >= 15) {
            var dados = { Codigo: e.Codigo.val(), Justificativa: e.Justificativa.val(), DataEmissao: e.DataEmissao.val() };
            executarReST("NotaFiscalEletronica/CancelarNFe", dados, function (arg) {
                if (arg.Success) {
                    if (arg.Data != false) {
                        LimparCampos(_justificativaCancelamento);
                        Global.abrirModal('divModalCancelarNFe');
                        _gridNotaFiscalEletronica.CarregarGrid();
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "NF-e cancelado com sucesso");
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        } else {
            exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe mais que 15 caracteres.");
        }
    } else
        exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
}

function EmitirNFe(nfe, instancia) {
    ValidarRegrasNFe(instancia, function () {
        if (ValidarCamposObrigatorios(instancia.NFe)) {
            if (ValidarCamposObrigatorios(instancia.Referencia)) {
                var data = {
                    NaturezaOperacao: instancia.NFe.NaturezaOperacao.codEntity(),
                    DataEmissao: instancia.NFe.DataEmissao.val()
                };

                executarReST("PlanoOrcamentario/ValidaPlanoOrcamentarioEmpresa", data, function (arg) {
                    if (arg.Success) {
                        if (arg.Data != true && arg.Data.Mensagem != "") {
                            if (arg.Data.TipoLancamentoFinanceiroSemOrcamento == EnumTipoLancamentoFinanceiroSemOrcamento.Avisar) {
                                exibirConfirmacao("Confirmação", arg.Data.Mensagem, function () {
                                    EmitirNFeAprovada(nfe, instancia);
                                });
                            } else
                                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                        } else {
                            EmitirNFeAprovada(nfe, instancia);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    }
                });
            } else
                exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        }
        else
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
    });
}

function EmitirNFeAprovada(nfe, instancia) {
    var dados = { NFe: nfe };
    executarReST("NotaFiscalEletronica/EmitirNFe", dados, function (arg) {
        if (arg.Success) {
            _gridNotaFiscalEletronica.CarregarGrid();
            if (arg.Data != false) {
                instancia.FecharModal();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "NF-e emitido com sucesso");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    });
}

function SalvarNFe(nfe, instancia) {
    ValidarRegrasNFe(instancia, function () {
        if (ValidarCamposObrigatorios(instancia.NFe)) {
            if (ValidarCamposObrigatorios(instancia.Referencia)) {
                var dados = { NFe: nfe };
                executarReST("NotaFiscalEletronica/SalvarNFe", dados, function (arg) {
                    if (arg.Success) {
                        instancia.NFe.Serie.required(true);
                        instancia.FecharModal();
                        _gridNotaFiscalEletronica.CarregarGrid();
                        if (arg.Data != false) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "NF-e salva com sucesso");
                        } else {
                            exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    }
                });
            } else
                exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        }
        else
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
    });
}

function ValidarRegrasNFe(instancia, callback) {
    var valorTotalParcelas = 0;

    for (var i = 0; i < instancia.Parcelas.length; i++) {
        valorTotalParcelas += Globalize.parseFloat(instancia.Parcelas[i].Valor);
    }
    valorTotalParcelas = parseFloat(valorTotalParcelas.toFixed(2));

    if (instancia.NFe.Serie.codEntity() != 0)
        instancia.NFe.Serie.required(false);
    else
        instancia.NFe.Serie.required(true);

    if (instancia.ProdutoServico.Codigo.val() > 0)
        exibirMensagem(tipoMensagem.atencao, "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
    else if (instancia.ProdutoServico.TipoItem.val() === 1 && instancia.ProdutoServico.Produto.val() !== "")
        exibirMensagem(tipoMensagem.atencao, "Item em Edição", "Por favor, verifique pois existe um produto sem salvar o mesmo.");
    else if (instancia.ProdutoServico.TipoItem.val() === 2 && instancia.ProdutoServico.Servico.val() !== "")
        exibirMensagem(tipoMensagem.atencao, "Item em Edição", "Por favor, verifique pois existe um serviço sem salvar o mesmo.");
    else if (instancia.NFe.Finalidade.val() === 4 && instancia.Referencia.TipoDocumento.val() === 0) //Finalidade Devolução
        exibirMensagem(tipoMensagem.atencao, "Documento de Referência", "Favor informar o documento de referência, já que a Finalidade da nota é Devolução.");
    else if (instancia.Referencia.TipoDocumento.val() === 1 && instancia.Referencia.Chave.val().trim().replace(/\s/g, "").length !== 44)
        exibirMensagem(tipoMensagem.atencao, "Documento de Referência", "Favor verificar a chave da nota referenciada, ela deve possuir 44 dígitos.");
    else if (Globalize.parseFloat(instancia.Totalizador.ValorTotalNFe.val()) < valorTotalParcelas)
        exibirMensagem(tipoMensagem.atencao, "Parcelamento da NF-e", "Favor gerar novamente as parcelas, pois a soma delas está maior que o valor total da nota.");
    else if (Globalize.parseFloat(instancia.Totalizador.ValorTotalProdutos.val()) != valorTotalParcelas && valorTotalParcelas > 0) {
        var valorTotalParcelasFormatado = valorTotalParcelas.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var valorTotalProdutosFormatado = Globalize.parseFloat(instancia.Totalizador.ValorTotalProdutos.val()).toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        exibirMensagem(tipoMensagem.aviso, "Aviso", `O valor total das parcelas (${valorTotalParcelasFormatado}) deve ser igual ao valor total dos produtos (${valorTotalProdutosFormatado})`);
    }
    else if (Globalize.parseFloat(instancia.Totalizador.ValorTotalNFe.val()) > valorTotalParcelas && valorTotalParcelas > 0)
        exibirConfirmacao("Confirmação", "Realmente deseja salvar a nota com a soma do valor das parcelas menor que o valor total da nota?", callback);
    else
        callback();
}

function VisibilidadeAutorizado(notaFiscalEletronicaGrid) {
    return notaFiscalEletronicaGrid.Status === EnumStatusNFe.Autorizado;
}

function VisibilidadeCancelado(notaFiscalEletronicaGrid) {
    return notaFiscalEletronicaGrid.Status === EnumStatusNFe.Cancelado;
}

function VisibilidadeEmitidoRejeitado(notaFiscalEletronicaGrid) {
    return notaFiscalEletronicaGrid.Status === EnumStatusNFe.Rejeitado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.Emitido;
}

function VisibilidadeEmitidoRejeitadoEmProcessamentoAguardandoAssinar(notaFiscalEletronicaGrid) {
    return notaFiscalEletronicaGrid.Status === EnumStatusNFe.Emitido || notaFiscalEletronicaGrid.Status === EnumStatusNFe.Rejeitado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.EmProcessamento || notaFiscalEletronicaGrid.Status === EnumStatusNFe.AguardandoAssinar;
}

function VisibilidadeAutorizadoCancelado(notaFiscalEletronicaGrid) {
    return notaFiscalEletronicaGrid.Status === EnumStatusNFe.Autorizado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.Cancelado;
}

function VisibilidadeEnvioSMS(notaFiscalEletronicaGrid) {
    return notaFiscalEletronicaGrid.AtivarEnvioDanfeSMS && VisibilidadeAutorizadoCancelado(notaFiscalEletronicaGrid);
}

function VisibilidadeEtiqueta(notaFiscalEletronicaGrid) {
    return notaFiscalEletronicaGrid.HabilitarEtiquetaProdutosNFe;
}

function buscarNotaFiscalEletronicas() {

    var editarNFE = { descricao: "Editar NF-e", id: guid(), metodo: editarNotaFiscalEletronica, icone: "", visibilidade: VisibilidadeEmitidoRejeitado };
    var duplicarNFe = { descricao: "Duplicar NF-e", id: guid(), metodo: duplicarNotaFiscalEletronica, icone: "" };
    var emitirNFe = { descricao: "Emitir/Autorizar NF-e", id: guid(), metodo: emitirNotaFiscalEletronica, icone: "", visibilidade: VisibilidadeEmitidoRejeitadoEmProcessamentoAguardandoAssinar };
    var cancelarNFe = { descricao: "Cancelar NF-e", id: guid(), metodo: cancelarNotaFiscalEletronica, icone: "", visibilidade: VisibilidadeAutorizado };
    var inutilizarNFe = { descricao: "Inutilizar NF-e", id: guid(), metodo: inutilizarNotaFiscalEletronica, icone: "", visibilidade: VisibilidadeEmitidoRejeitado };
    var cartaCorrecaoNFe = { descricao: "Gerar CCe NF-e", id: guid(), metodo: cartaCorrecaoNotaFiscalEletronica, icone: "", visibilidade: VisibilidadeAutorizado };
    var baixarDANFE = { descricao: "Baixar DANFE", id: guid(), metodo: baixarDANFENotaFiscalEletronica, icone: "", visibilidade: VisibilidadeAutorizadoCancelado };
    var baixarXML = { descricao: "Baixar XML NFe", id: guid(), metodo: baixarXMLNotaFiscalEletronica, icone: "", visibilidade: VisibilidadeAutorizadoCancelado };
    var baixarXMLCancelamento = { descricao: "Baixar XML Cancel.", id: guid(), metodo: baixarXLMCancelamentoNotaFiscalEletronica, icone: "", visibilidade: VisibilidadeCancelado };
    var baixarCCe = { descricao: "Baixar CCe", id: guid(), metodo: baixarCCeNotaFiscalEletronica, icone: "", visibilidade: VisibilidadeAutorizado };
    var baixarXMLCCe = { descricao: "Baixar XML CCe", id: guid(), metodo: baixarXMLCCeNotaFiscalEletronica, icone: "", visibilidade: VisibilidadeAutorizadoCancelado };
    var reenviarEmail = { descricao: "Reenviar e-mails", id: guid(), metodo: reenviarEmailNotaFiscalEletronica, icone: "", visibilidade: VisibilidadeAutorizadoCancelado };
    var baixarPreDANFE = { descricao: "Baixar Pré-DANFE", id: guid(), metodo: baixarPreDANFENotaFiscalEletronica, icone: "", visibilidade: VisibilidadeEmitidoRejeitado };
    var reenviarSMS = { descricao: "Reenviar SMS", id: guid(), metodo: reenviarSMSNotaFiscalEletronica, icone: "", visibilidade: VisibilidadeEnvioSMS };
    var imprimirEtiqueta = { descricao: "Imprimir Etiqueta", id: guid(), metodo: imprimirEtiquetaNotaFiscalEletronica, icone: "", visibilidade: VisibilidadeEtiqueta };
    var mostrarIntegracoes = { descricao: "Integrações", id: guid(), metodo: mostrarIntegracoesNotaFiscalEletronica, icone: "", visibilidade: VisibilidadeAutorizadoCancelado };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [editarNFE, duplicarNFe, emitirNFe, cancelarNFe, inutilizarNFe, cartaCorrecaoNFe,
            baixarDANFE, baixarXML, baixarXMLCancelamento, baixarCCe, baixarXMLCCe, reenviarEmail, baixarPreDANFE, reenviarSMS, imprimirEtiqueta, mostrarIntegracoes]
    };

    _gridNotaFiscalEletronica = new GridView(_pesquisaNotaFiscalEletronica.Pesquisar.idGrid, "NotaFiscalEletronica/Pesquisa", _pesquisaNotaFiscalEletronica, menuOpcoes, null);
    _gridNotaFiscalEletronica.CarregarGrid();
}

function editarNotaFiscalEletronica(notaFiscalEletronicaGrid) {
    if (notaFiscalEletronicaGrid.Status === EnumStatusNFe.Emitido || notaFiscalEletronicaGrid.Status === EnumStatusNFe.Rejeitado) {
        limparCamposNotaFiscalEletronica();
        _pesquisaNotaFiscalEletronica.Codigo.val(notaFiscalEletronicaGrid.Codigo);
        abrirModalNFe(notaFiscalEletronicaGrid.Codigo);
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFePesquisa[notaFiscalEletronicaGrid.Status].text, 60000);
}

function duplicarNotaFiscalEletronica(notaFiscalEletronicaGrid) {
    exibirConfirmacao("Confirmação", "Realmente deseja duplicar a nota de número " + notaFiscalEletronicaGrid.Numero + " e série " + notaFiscalEletronicaGrid.Serie + "?", function () {
        var dados = { Codigo: notaFiscalEletronicaGrid.Codigo };
        executarReST("NotaFiscalEletronica/DuplicarNFe", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    _gridNotaFiscalEletronica.CarregarGrid();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "NF-e duplicada com sucesso");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function emitirNotaFiscalEletronica(notaFiscalEletronicaGrid) {
    if (notaFiscalEletronicaGrid.Status === EnumStatusNFe.Emitido || notaFiscalEletronicaGrid.Status === EnumStatusNFe.Rejeitado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.EmProcessamento || notaFiscalEletronicaGrid.Status === EnumStatusNFe.AguardandoAssinar) {
        var data = {
            NaturezaOperacao: notaFiscalEletronicaGrid.CodigoNaturezaOperacao,
            DataEmissao: notaFiscalEletronicaGrid.DataEmissao
        };

        executarReST("PlanoOrcamentario/ValidaPlanoOrcamentarioEmpresa", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != true && arg.Data.Mensagem != "") {
                    if (arg.Data.TipoLancamentoFinanceiroSemOrcamento == EnumTipoLancamentoFinanceiroSemOrcamento.Avisar) {
                        exibirConfirmacao("Confirmação", arg.Data.Mensagem, function () {
                            emitirNotaFiscalEletronicaAprovada(notaFiscalEletronicaGrid);
                        });
                    } else
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                } else {
                    emitirNotaFiscalEletronicaAprovada(notaFiscalEletronicaGrid);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFePesquisa[notaFiscalEletronicaGrid.Status].text, 60000);
}

function emitirNotaFiscalEletronicaAprovada(notaFiscalEletronicaGrid) {
    var dados = { Codigo: notaFiscalEletronicaGrid.Codigo };
    executarReST("NotaFiscalEletronica/EnviarNFe", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _gridNotaFiscalEletronica.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "NF-e emitido com sucesso");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function cancelarNotaFiscalEletronica(notaFiscalEletronicaGrid) {
    if (notaFiscalEletronicaGrid.Status === EnumStatusNFe.Autorizado) {
        LimparCampos(_justificativaCancelamento);
        _justificativaCancelamento.DataEmissao.val(Global.DataHoraAtual());
        _justificativaCancelamento.Codigo.val(notaFiscalEletronicaGrid.Codigo);
        Global.abrirModal('divModalCancelarNFe');
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFePesquisa[notaFiscalEletronicaGrid.Status].text, 60000);
}

function inutilizarNotaFiscalEletronica(notaFiscalEletronicaGrid) {
    if (notaFiscalEletronicaGrid.Status === EnumStatusNFe.Rejeitado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.Emitido) {
        exibirConfirmacao("Confirmação", "Realmente deseja inutilizar a nota de número " + notaFiscalEletronicaGrid.Numero + " e série " + notaFiscalEletronicaGrid.Serie + "?", function () {
            var dados = { Codigo: notaFiscalEletronicaGrid.Codigo };
            executarReST("NotaFiscalEletronica/InutilizarNFe", dados, function (arg) {
                if (arg.Success) {
                    if (arg.Data != false) {
                        _gridNotaFiscalEletronica.CarregarGrid();
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "NF-e inutlizada com sucesso");
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFePesquisa[notaFiscalEletronicaGrid.Status].text, 60000);
}

function cartaCorrecaoNotaFiscalEletronica(notaFiscalEletronicaGrid) {
    if (notaFiscalEletronicaGrid.Status === EnumStatusNFe.Autorizado) {
        LimparCampos(_correcaoCartaCorrecao);
        _correcaoCartaCorrecao.DataEmissao.val(Global.DataHoraAtual());
        _correcaoCartaCorrecao.Codigo.val(notaFiscalEletronicaGrid.Codigo);
        Global.abrirModal('divModalCartaCorrecaoNFe');
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFePesquisa[notaFiscalEletronicaGrid.Status].text, 60000);
}

function baixarDANFENotaFiscalEletronica(notaFiscalEletronicaGrid) {
    if (notaFiscalEletronicaGrid.Status === EnumStatusNFe.Autorizado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.Cancelado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.AguardandoCancelarAssinar || notaFiscalEletronicaGrid.Status === EnumStatusNFe.AguardandoCartaCorrecaoAssinar) {

        var dados = { Codigo: notaFiscalEletronicaGrid.Codigo };
        executarDownload("NotaFiscalEletronica/DownloadPDF", dados, null, function (arg) {
           
            if (arg.Success) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else
            {
                _gridDANFERelatorio = new GridView("qualquercoisa", "Relatorios/DANFE/Pesquisa", _pesquisaNotaFiscalEletronica);
                _pesquisaNotaFiscalEletronica.Codigo.val(notaFiscalEletronicaGrid.Codigo);
                var _relatorioDANFE = new RelatorioGlobal("Relatorios/DANFE/BuscarDadosRelatorio", _gridDANFERelatorio, function () {
                    _relatorioDANFE.loadRelatorio(function () {
                        _relatorioDANFE.gerarRelatorio("Relatorios/DANFE/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
                    });
                }, null, null, _pesquisaNotaFiscalEletronica);
            }
        });
        
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFePesquisa[notaFiscalEletronicaGrid.Status].text, 60000);
}

function baixarCCeNotaFiscalEletronica(notaFiscalEletronicaGrid) {
    if (notaFiscalEletronicaGrid.Status === EnumStatusNFe.Autorizado) {
        _gridCCeNFeRelatorio = new GridView("qualquercoisa", "Relatorios/CCeNFe/Pesquisa", _pesquisaNotaFiscalEletronica);
        _pesquisaNotaFiscalEletronica.Codigo.val(notaFiscalEletronicaGrid.Codigo);
        var _relatorioDANFE = new RelatorioGlobal("Relatorios/CCeNFe/BuscarDadosRelatorio", _gridCCeNFeRelatorio, function () {
            _relatorioDANFE.loadRelatorio(function () {
                _relatorioDANFE.gerarRelatorio("Relatorios/CCeNFe/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
            });
        }, null, null, _pesquisaNotaFiscalEletronica);
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFePesquisa[notaFiscalEletronicaGrid.Status].text, 60000);
}

function baixarXMLCCeNotaFiscalEletronica(notaFiscalEletronicaGrid) {
    if (notaFiscalEletronicaGrid.Status === EnumStatusNFe.Autorizado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.Cancelado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.AguardandoCancelarAssinar) {
        var dados = { Codigo: notaFiscalEletronicaGrid.Codigo };
        executarDownload("NotaFiscalEletronica/DownloadXMLCCe", dados);
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFePesquisa[notaFiscalEletronicaGrid.Status].text, 60000);
}

function baixarXMLNotaFiscalEletronica(notaFiscalEletronicaGrid) {
    if (notaFiscalEletronicaGrid.Status === EnumStatusNFe.Autorizado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.Cancelado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.AguardandoCancelarAssinar || notaFiscalEletronicaGrid.Status === EnumStatusNFe.AguardandoCartaCorrecaoAssinar) {
        var dados = { Codigo: notaFiscalEletronicaGrid.Codigo };
        executarDownload("NotaFiscalEletronica/DownloadXML", dados);
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFePesquisa[notaFiscalEletronicaGrid.Status].text, 60000);
}

function baixarXLMCancelamentoNotaFiscalEletronica(notaFiscalEletronicaGrid) {
    if (notaFiscalEletronicaGrid.Status === EnumStatusNFe.Cancelado) {
        var dados = { Codigo: notaFiscalEletronicaGrid.Codigo };
        executarDownload("NotaFiscalEletronica/DownloadXMLCancelamento", dados);
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFePesquisa[notaFiscalEletronicaGrid.Status].text, 60000);
}

function reenviarEmailNotaFiscalEletronica(notaFiscalEletronicaGrid) {
    if (notaFiscalEletronicaGrid.Status === EnumStatusNFe.Autorizado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.Cancelado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.AguardandoCancelarAssinar || notaFiscalEletronicaGrid.Status === EnumStatusNFe.AguardandoCartaCorrecaoAssinar) {
        var dados = { Codigo: notaFiscalEletronicaGrid.Codigo };
        executarReST("NotaFiscalEletronica/EnviarEmailNFe", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo de envio de e-mail ativado.");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFePesquisa[notaFiscalEletronicaGrid.Status].text, 60000);
}

function baixarPreDANFENotaFiscalEletronica(notaFiscalEletronicaGrid) {
    if (notaFiscalEletronicaGrid.Status === EnumStatusNFe.Emitido || notaFiscalEletronicaGrid.Status === EnumStatusNFe.AguardandoAssinar || notaFiscalEletronicaGrid.Status === EnumStatusNFe.AguardandoInutilizarAssinar) {
        _gridDANFERelatorio = new GridView("qualquercoisa", "Relatorios/DANFE/Pesquisa", _pesquisaNotaFiscalEletronica);
        _pesquisaNotaFiscalEletronica.Codigo.val(notaFiscalEletronicaGrid.Codigo);
        var _relatorioDANFE = new RelatorioGlobal("Relatorios/DANFE/BuscarDadosRelatorio", _gridDANFERelatorio, function () {
            _relatorioDANFE.loadRelatorio(function () {
                _relatorioDANFE.gerarRelatorio("Relatorios/DANFE/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
            });
        }, null, null, _pesquisaNotaFiscalEletronica);
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFePesquisa[notaFiscalEletronicaGrid.Status].text, 60000);
}

function imprimirEtiquetaNotaFiscalEletronica(notaFiscalEletronicaGrid) {
    var dados = { Codigo: notaFiscalEletronicaGrid.Codigo };
    executarReST("NotaFiscalEletronica/ImprimirEtiquetaNFe", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo de geração da etiqueta iniciada.");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function reenviarSMSNotaFiscalEletronica(notaFiscalEletronicaGrid) {
    if (notaFiscalEletronicaGrid.Status === EnumStatusNFe.Autorizado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.Cancelado) {
        var dados = { Codigo: notaFiscalEletronicaGrid.Codigo };
        executarReST("NotaFiscalEletronica/EnviarSMSNFe", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo de envio de SMS ativado.");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFePesquisa[notaFiscalEletronicaGrid.Status].text, 60000);
}

function limparCamposNotaFiscalEletronica() {

}

function mostrarIntegracoesNotaFiscalEletronica(notaFiscalEletronicaGrid) {
    recarregarNotaFiscalEletronicaIntegracoes(notaFiscalEletronicaGrid);
}