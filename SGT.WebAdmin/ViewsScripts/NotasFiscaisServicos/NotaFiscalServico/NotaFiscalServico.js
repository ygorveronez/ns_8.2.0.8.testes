/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../../Areas/Relatorios/ViewsScripts/Relatorios/Global/Relatorio.js" />
/// <reference path="../../Enumeradores/EnumTipoArquivoRelatorio.js" />
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
/// <reference path="../../Consultas/NaturezaNFSe.js" />
/// <reference path="../../Enumeradores/EnumPermissoesEdicaoNFSe.js" />
/// <reference path="../NFSe/NFSe.js" />
/// <reference path="../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../Consultas/PedidoVenda.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridNotaFiscalServico;
var _pesquisaNotaFiscalServico;
var _gridPedidos;
var _protocoloNotaFiscalServico;
var _editarNumeroNFSeENumeroRPS;
var _gridDANFSERelatorio;

var _statusNFSePesquisa = [
    { text: "Todos", value: EnumStatusCTe.TODOS },
    { text: "Em Digitação", value: EnumStatusCTe.EMDIGITACAO },
    { text: "Autorizado", value: EnumStatusCTe.AUTORIZADO },
    { text: "Cancelado", value: EnumStatusCTe.CANCELADO },
    { text: "Rejeição", value: EnumStatusCTe.REJEICAO },
    { text: "Enviado", value: EnumStatusCTe.ENVIADO },
    { text: "Em Cancelamento", value: EnumStatusCTe.EMCANCELAMENTO }
]

var PesquisaNotaFiscalServico = function () {
    var dataInicial = moment().add(-1, 'days').format("DD/MM/YYYY");
    var dataFinal = moment().format("DD/MM/YYYY");
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa (Tomador): ", idBtnSearch: guid(), visible: true });    
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(dataInicial), def: dataInicial, getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", val: ko.observable(dataFinal), def: dataFinal, getType: typesKnockout.date });
    this.Serie = PropertyEntity({ text: "Série: ", getType: typesKnockout.int });
    this.NaturezaOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Natureza da Operação: ", idBtnSearch: guid(), visible: true });
    this.NumeroProtocolo = PropertyEntity({ text: "Número Protocolo: ", getType: typesKnockout.string });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusCTe.TODOS), options: _statusNFSePesquisa, def: EnumStatusCTe.TODOS, text: "Status:" });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador", idBtnSearch: guid(), visible: ko.observable(true) });
    this.LocalidadePrincipalTransportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cidade do Transportador", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });

    this.NovaNotaFiscalServico = PropertyEntity({ eventClick: NovaNotaServicoClick, type: types.event, text: "Nova NFS-e", icon: ko.observable("fal fa-plus"), idGrid: guid(), visible: ko.observable(true) });
    this.ImportarPedido = PropertyEntity({ type: types.map, required: false, text: "Importar de Pedido/OS", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
    

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridNotaFiscalServico.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var ProtocoloNotaFiscalServico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Protocolo = PropertyEntity({ getType: typesKnockout.string, required: true, maxlength: 100, text: "*Protocolo:" });

    this.Consultar = PropertyEntity({ type: types.event, eventClick: consultarProtocoloClick, text: "Consultar NFS-e" });
};

var EditarNumeroNFSeENumeroRPS = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroNFSe = PropertyEntity({ getType: typesKnockout.string, required: true, maxlength: 100, text: "*Nº NFSe:" });
    this.NumeroRPS = PropertyEntity({ getType: typesKnockout.string, required: true, maxlength: 100, text: "*Nº RPS:" });

    this.Editar = PropertyEntity({ type: types.event, eventClick: editarNumeroNFSeENumeroRPS, text: "Confimar" });
};

//*******EVENTOS*******

function loadNotaFiscalServico() {
    _pesquisaNotaFiscalServico = new PesquisaNotaFiscalServico();
    KoBindings(_pesquisaNotaFiscalServico, "knockoutPesquisaNotaFiscalServico", false, _pesquisaNotaFiscalServico.Pesquisar.id);

    _protocoloNotaFiscalServico = new ProtocoloNotaFiscalServico();
    KoBindings(_protocoloNotaFiscalServico, "knoutModalProtocoloNFSe");

    _editarNumeroNFSeENumeroRPS = new EditarNumeroNFSeENumeroRPS();
    KoBindings(_editarNumeroNFSeENumeroRPS, "knoutModalEditarNFSeERPS");

    //HeaderAuditoria("ConhecimentoDeTransporteEletronico", {});

    CarregarGridPedidos();

    new BuscarClientes(_pesquisaNotaFiscalServico.Pessoa);
    new BuscarNaturezaNFSe(_pesquisaNotaFiscalServico.NaturezaOperacao, null, null, null, null, "S");
    new BuscarPedidosVendas(_pesquisaNotaFiscalServico.ImportarPedido, retornoPedidosVendas, null, _gridPedidos, true);
    new BuscarTransportadores(_pesquisaNotaFiscalServico.Transportador);
    new BuscarLocalidades(_pesquisaNotaFiscalServico.LocalidadePrincipalTransportador);

    buscarNotaFiscalServicos();
}

function retornoPedidosVendas(pedidos) {
    var codigos = new Array();
    $.each(pedidos, function (i, pedido) {
        codigos.push({ Codigo: pedido.Codigo });
    });

    abrirModalNFSe(0, codigos);

}

function NovaNotaServicoClick(e, sender) {
    abrirModalNFSe(0, null);
}

//*******MÉTODOS*******

function CarregarGridPedidos() {
    var pedidos = new Array();

    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", visible: false },
        { data: "DataEmissao", visible: false },
        { data: "Cliente", visible: false },
        { data: "DescricaoTipo", visible: false },
        { data: "DescricaoStatus", visible: false },
        { data: "ValorTotal", visible: false }
    ];

    _gridPedidos = new BasicDataTable(_pesquisaNotaFiscalServico.ImportarPedido.idGrid, header);
    _gridPedidos.CarregarGrid(pedidos);
}

function abrirModalNFSe(codigoNFSe, codigosPedido, desabilitado) {
    var permissoesTotal = new Array();
    permissoesTotal.push(EnumPermissoesEdicaoNFSe.total);  
    var instanciaEmissao = new EmissaoNFSe(codigoNFSe, function () {
        instanciaEmissao.CRUDNFSe.Salvar.visible(true);
        instanciaEmissao.CRUDNFSe.Emitir.visible(true);
        instanciaEmissao.CRUDNFSe.Salvar.eventClick = function () {
            var objetoNFSe = ObterObjetoNFSe(instanciaEmissao);
            SalvarNFSe(objetoNFSe, instanciaEmissao);
        }
        instanciaEmissao.CRUDNFSe.Emitir.eventClick = function () {
            var objetoNFSe = ObterObjetoNFSe(instanciaEmissao);
            EmitirNFSe(objetoNFSe, instanciaEmissao);
        }

        if (desabilitado) {
            instanciaEmissao.DestivarNFSe();
            instanciaEmissao.DestivarCRUDNFSe();
            instanciaEmissao.ListaServico.DestivarListaServico();
            instanciaEmissao.Servico.DestivarServico();
            instanciaEmissao.Valor.DestivarTotalizador();
            instanciaEmissao.Substituicao.DestivarObservacao();
            instanciaEmissao.DetalheParcela.DestivarParcelas();
            instanciaEmissao.Parcelamento.DestivarParcelamento();
        }

    }, permissoesTotal, codigosPedido);
}

function EmitirNFSe(nfse, instancia) {
    ValidarRegrasNFSe(instancia, function () {
        if (ValidarCamposObrigatorios(instancia.NFSe)) {
            var data = {
                NaturezaOperacaoServico: instancia.NFSe.NaturezaOperacao.codEntity(),
                DataEmissao: instancia.NFSe.DataEmissao.val()
            };

            executarReST("PlanoOrcamentario/ValidaPlanoOrcamentarioEmpresa", data, function (arg) {
                if (arg.Success) {
                    if (arg.Data != true && arg.Data.Mensagem != "") {
                        if (arg.Data.TipoLancamentoFinanceiroSemOrcamento == EnumTipoLancamentoFinanceiroSemOrcamento.Avisar) {
                            exibirConfirmacao("Confirmação", arg.Data.Mensagem, function () {
                                EmitirNFSeAprovada(nfse, instancia)
                            });
                        } else
                            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    } else {
                        EmitirNFSeAprovada(nfse, instancia)
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        }
        else
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
    });
}

function EmitirNFSeAprovada(nfse, instancia) {
    var dados = { NFSe: nfse };
    executarReST("NotaFiscalServico/EmitirNFSe", dados, function (arg) {
        if (arg.Success) {
            instancia.FecharModal();
            _gridNotaFiscalServico.CarregarGrid();
            if (arg.Data != false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "NFS-e enviado ao integrador");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    });
}

function SalvarNFSe(nfse, instancia) {
    ValidarRegrasNFSe(instancia, function () {
        if (ValidarCamposObrigatorios(instancia.NFSe)) {
            var dados = { NFSe: nfse }
            executarReST("NotaFiscalServico/SalvarNFSe", dados, function (arg) {
                if (arg.Success) {
                    instancia.FecharModal();
                    _gridNotaFiscalServico.CarregarGrid();
                    if (arg.Data != false) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "NFS-e salva com sucesso");
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
                }
            });
        }
        else
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
    });
}

function consultarProtocoloClick(e) {
    if (ValidarCamposObrigatorios(e)) {
        var dados = { Codigo: e.Codigo.val(), Protocolo: e.Protocolo.val() };
        executarReST("NotaFiscalServico/ConsultarProtocoloNFSe", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    LimparCampos(_protocoloNotaFiscalServico);
                    Global.fecharModal('divModalProtocoloNFSe');
                    _gridNotaFiscalServico.CarregarGrid();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Consulta de NFS-e enviada ao integrador");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else
        exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
}

function editarNumeroNFSeENumeroRPS(e) {
    if (ValidarCamposObrigatorios(e)) {
        var dados = { Codigo: e.Codigo.val(), NumeroNFSe: e.NumeroNFSe.val(), NumeroRPS: e.NumeroRPS.val() };
        executarReST("NotaFiscalServico/EditarNumeroNFSeENumeroRPS", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    LimparCampos(_editarNumeroNFSeENumeroRPS);
                    Global.fecharModal('divModalEditarNumeroNFSeENumeroRPS');
                    _gridNotaFiscalServico.CarregarGrid();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados alterados com sucesso!");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else
        exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
}

function ValidarRegrasNFSe(instancia, callback) {
    var valorTotalParcelas = 0;

    for (var i = 0; i < instancia.Parcelas.length; i++) {
        valorTotalParcelas += Globalize.parseFloat(instancia.Parcelas[i].Valor);
    }
    valorTotalParcelas = parseFloat(valorTotalParcelas.toFixed(2));

    if (instancia.Servico.Codigo.val() > 0)
        exibirMensagem(tipoMensagem.atencao, "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
    if (instancia.Servico.ServicoItem.val() !== "")
        exibirMensagem(tipoMensagem.atencao, "Item em Edição", "Por favor, verifique pois existe um Serviço sem salvar.");
    else if (Globalize.parseFloat(instancia.Valor.ValorTotalLiquido.val()) < valorTotalParcelas)
        exibirMensagem(tipoMensagem.atencao, "Parcelamento da NFS-e", "Favor gerar novamente as parcelas, pois a soma delas está maior que o valor total líquido da nota.");
    else if (Globalize.parseFloat(instancia.Valor.ValorTotalLiquido.val()) > valorTotalParcelas && valorTotalParcelas > 0)
        exibirConfirmacao("Confirmação", "Realmente deseja salvar a nota com a soma do valor das parcelas menor que o valor total líquido da nota?", callback);
    else
        callback();
}

function VisibilidadeRejeitado(notaFiscalServicoGrid) {
    return notaFiscalServicoGrid.Status == EnumStatusCTe.REJEICAO;
}

function VisibleBuscarNotaFiscalServicos() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador)
        return true;
    else
        return false;
}

function buscarNotaFiscalServicos() {

    var editarNFSe = { descricao: "Editar", id: guid(), metodo: editarNotaFiscalServico, icone: "" };
    var duplicarNFSe = { descricao: "Duplicar NFS-e", id: guid(), metodo: duplicarNotaFiscalServico, icone: "", visibilidade: VisibleBuscarNotaFiscalServicos };
    var emitirNFSe = { descricao: "Emitir/Autorizar NFS-e", id: guid(), metodo: emitirNotaFiscalServico, icone: "" };
    var cancelarNFSe = { descricao: "Cancelar NFS-e", id: guid(), metodo: cancelarNotaFiscalServico, icone: "", visibilidade: VisibleBuscarNotaFiscalServicos };
    var baixarDANFSE = { descricao: "Baixar DANFSE", id: guid(), metodo: baixarDANFSENotaFiscalServico, icone: "" };
    var baixarXMLNFSe = { descricao: "Baixar XML Autorização", id: guid(), metodo: baixarXMLNotaFiscalServico, icone: "" };
    var retornoNFSe = { descricao: "Visualizar Retorno", id: guid(), metodo: retornoNotaFiscalServico, icone: "" };
    var consultarProtocoloNFSe = { descricao: "Consultar Protocolo", id: guid(), metodo: consultaProtocoloNotaFiscalServico, icone: "", visibilidade: VisibilidadeRejeitado };
    var editarNumeroNFSeRPS = { descricao: "Editar Nº NFSe e Nº RPS", id: guid(), metodo: editarNumeroNotaFiscalServicoENumeroRPS, icone: "", visibilidade: VisibilidadeRejeitado };
    var ordenacaoNFSe = null;

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiNFe)
        ordenacaoNFSe = { column: 1, dir: orderDir.desc }

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [editarNFSe, duplicarNFSe, emitirNFSe, cancelarNFSe, baixarDANFSE, baixarXMLNFSe, retornoNFSe, consultarProtocoloNFSe, editarNumeroNFSeRPS] };

    _gridNotaFiscalServico = new GridView(_pesquisaNotaFiscalServico.Pesquisar.idGrid, "NotaFiscalServico/Pesquisa", _pesquisaNotaFiscalServico, menuOpcoes, ordenacaoNFSe);
    _gridNotaFiscalServico.CarregarGrid();
}

function editarNotaFiscalServico(notaFiscalServicoGrid) {
    if (notaFiscalServicoGrid.Status == EnumStatusCTe.AUTORIZADO) {
        _pesquisaNotaFiscalServico.Codigo.val(notaFiscalServicoGrid.Codigo);
        var somenteLeitura = true;
        abrirModalNFSe(notaFiscalServicoGrid.Codigo, null, somenteLeitura);
    }
    else if (notaFiscalServicoGrid.Status == EnumStatusCTe.EMDIGITACAO || notaFiscalServicoGrid.Status == EnumStatusCTe.REJEICAO) {
        _pesquisaNotaFiscalServico.Codigo.val(notaFiscalServicoGrid.Codigo);
        abrirModalNFSe(notaFiscalServicoGrid.Codigo);
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal já foi processada pela prefeitura, não pode mais ser alterada!", 60000);
}

function duplicarNotaFiscalServico(notaFiscalServicoGrid) {
    exibirConfirmacao("Confirmação", "Realmente deseja duplicar a nota de número " + notaFiscalServicoGrid.Numero + " e série " + notaFiscalServicoGrid.Serie + "?", function () {
        var dados = { Codigo: notaFiscalServicoGrid.Codigo }
        executarReST("NotaFiscalServico/DuplicarNFSe", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    _gridNotaFiscalServico.CarregarGrid();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "NFS-e duplicada com sucesso");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function emitirNotaFiscalServico(notaFiscalServicoGrid) {
    if (notaFiscalServicoGrid.Status == EnumStatusCTe.EMDIGITACAO || notaFiscalServicoGrid.Status == EnumStatusCTe.REJEICAO
        || notaFiscalServicoGrid.Status == EnumStatusCTe.PENDENTE || notaFiscalServicoGrid.Status == EnumStatusCTe.ENVIADO) {
        var data = {
            NaturezaOperacaoServico: notaFiscalServicoGrid.CodigoNaturezaOperacao,
            DataEmissao: notaFiscalServicoGrid.DataEmissao
        };

        executarReST("PlanoOrcamentario/ValidaPlanoOrcamentarioEmpresa", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != true && arg.Data.Mensagem != "") {
                    if (arg.Data.TipoLancamentoFinanceiroSemOrcamento == EnumTipoLancamentoFinanceiroSemOrcamento.Avisar) {
                        exibirConfirmacao("Confirmação", arg.Data.Mensagem, function () {
                            emitirNotaFiscalServicoAprovada(notaFiscalServicoGrid)
                        });
                    } else
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                } else {
                    emitirNotaFiscalServicoAprovada(notaFiscalServicoGrid)
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal já foi processada pela prefeitura, não pode mais ser alterada!", 60000);
}

function emitirNotaFiscalServicoAprovada(notaFiscalServicoGrid) {
    var dados = { Codigo: notaFiscalServicoGrid.Codigo };
    executarReST("NotaFiscalServico/EnviarNFSe", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _gridNotaFiscalServico.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "NFS-e enviado ao integrador");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function cancelarNotaFiscalServico(notaFiscalServicoGrid) {
    if (notaFiscalServicoGrid.Status == EnumStatusCTe.AUTORIZADO) {
        exibirConfirmacao("Confirmação", "Realmente deseja cancelar a nota de número " + notaFiscalServicoGrid.Numero + " e série " + notaFiscalServicoGrid.Serie + "?", function () {
            var dados = { Codigo: notaFiscalServicoGrid.Codigo };
            executarReST("NotaFiscalServico/CancelarNFSe", dados, function (arg) {
                if (arg.Success) {
                    if (arg.Data != false) {
                        _gridNotaFiscalServico.CarregarGrid();
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelamento de NFS-e enviado ao integrador");
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Só é possível cancelar nota com o status autorizada", 60000);
}

function baixarDANFSENotaFiscalServico(notaFiscalServicoGrid) {
    if (notaFiscalServicoGrid.Status == EnumStatusCTe.AUTORIZADO || notaFiscalServicoGrid.Status == EnumStatusCTe.CANCELADO) {
        var dados = { Codigo: notaFiscalServicoGrid.Codigo };
        executarDownload("NotaFiscalServico/DownloadDANFSE", dados);
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Só é possível baixar DANFSE de nota com o status autorizada ou cancelada", 60000);
}

function baixarXMLNotaFiscalServico(notaFiscalServicoGrid) {
    if (notaFiscalServicoGrid.Status == EnumStatusCTe.AUTORIZADO || notaFiscalServicoGrid.Status == EnumStatusCTe.CANCELADO) {
        var dados = { Codigo: notaFiscalServicoGrid.Codigo }
        executarDownload("NotaFiscalServico/DownloadXML", dados);
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Só é possível baixar o xml de nota com status autorizada ou cancelada", 60000);
}

function retornoNotaFiscalServico(notaFiscalServicoGrid) {
    exibirMensagem(tipoMensagem.aviso, "Retorno Sefaz", notaFiscalServicoGrid.MensagemRetornoSefaz);
}

function consultaProtocoloNotaFiscalServico(notaFiscalServicoGrid) {
    if (notaFiscalServicoGrid.Status == EnumStatusCTe.REJEICAO) {
        LimparCampos(_protocoloNotaFiscalServico);
        _protocoloNotaFiscalServico.Codigo.val(notaFiscalServicoGrid.Codigo);
        Global.abrirModal('divModalProtocoloNFSe');
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Só é possível consulta a nota por protocolo com o status rejeição", 60000);
}

function editarNumeroNotaFiscalServicoENumeroRPS(notaFiscalServicoGrid) {
    if (notaFiscalServicoGrid.Status == EnumStatusCTe.REJEICAO) {
        LimparCampos(_protocoloNotaFiscalServico);
        _editarNumeroNFSeENumeroRPS.Codigo.val(notaFiscalServicoGrid.Codigo);
        _editarNumeroNFSeENumeroRPS.NumeroNFSe.val(notaFiscalServicoGrid.Numero);
        _editarNumeroNFSeENumeroRPS.NumeroRPS.val(notaFiscalServicoGrid.NumeroRPS);
        Global.abrirModal('divModalEditarNumeroNFSeENumeroRPS');
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal já foi processada pela prefeitura, não pode mais ser alterada!", 60000);
}