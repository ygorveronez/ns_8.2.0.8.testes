/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _baixaPallets;
var _gridAnexoDevolucaoPallets;

/*
 * Declaração das Classes
 */

var BaixaPallets = function () {
    var isTMS = (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS);

    this.Codigo = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int, def: 0 });
    this.Carga = PropertyEntity({ text: "Carga:" });
    this.NotaFiscal = PropertyEntity({ text: "Nota Fiscal:", visible: !isTMS });
    this.Transportador = PropertyEntity({ text: (isTMS ? "Empresa/Filial:" : "Transportador:") });
    this.Motorista = PropertyEntity({ text: "Motorista:" });
    this.Veiculo = PropertyEntity({ text: "Veículo:" });
    this.NumeroPallets = PropertyEntity({ text: "Número de Pallets Devolução:", getType: typesKnockout.int, val: ko.observable(0), def: 0, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.NumeroPalletsValePallet = PropertyEntity({ text: "Número de Pallets Vale Pallet:", getType: typesKnockout.int, val: ko.observable(0), def: 0, configInt: { precision: 0, allowZero: false, thousands: "" }, visible: !isTMS });
    this.NumeroTotalPallets = PropertyEntity({ text: "Número Total de Pallets:", getType: typesKnockout.int, val: ko.observable(0), def: 0, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.ValorTotal = PropertyEntity({ text: "Total de Reembolso:" });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 400 });

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial:", idBtnSearch: guid(), required: !isTMS, visible: !isTMS });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cliente:", idBtnSearch: guid(), required: isTMS, visible: isTMS });
    this.DataBaixa = PropertyEntity({ type: types.map, text: "*Data:", getType: typesKnockout.dateTime, idBtnSearch: guid(), required: false, visible: ko.observable(false) });
    this.Situacoes = ko.observableArray();
    this.QuantidadeSituacoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Anexos = PropertyEntity({ type: types.local });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexosDevolucaoPallet();
    });

    this.AdicionarAnexo = PropertyEntity({
        eventClick: function (e) {
            adicionarAnexoDevolucaoPalletClick()
        }, type: types.event, text: "Adicionar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Baixar = PropertyEntity({
        eventClick: function (e) {
            BaixarPalletsClick()
        }, type: types.event, text: "Baixar Pallets", idGrid: guid(), visible: ko.observable(true)
    });
}

var SituacaoBaixa = function (situacao) {
    var $this = this;

    $this.Codigo = PropertyEntity({ val: ko.observable(situacao.Codigo), getType: typesKnockout.int, def: situacao.Codigo });
    $this.Quantidade = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, def: "", maxlength: 7, text: situacao.Descricao + ":" });
    $this.ValorUnitario = PropertyEntity({ text: "Valor Unitário:", val: ko.observable(Globalize.format(situacao.ValorUnitario, "n2")), def: Globalize.format(situacao.ValorUnitario, "n2"), visible: ko.observable(situacao.ValorUnitario > 0 ? true : false) });
    $this.ValorTotal = PropertyEntity({ text: "Valor Total:", val: ko.observable("0,00"), def: "0,00", visible: ko.observable(situacao.ValorUnitario > 0 ? true : false) });
    $this.QuebrarLinha = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), def: true });

    $this.Quantidade.val.subscribe(function (novoValor) {
        var quantidade = Globalize.parseInt(novoValor.toString());
        var valorUnitario = Globalize.parseFloat($this.ValorUnitario.val());

        if (isNaN(quantidade))
            quantidade = 0;
        if (isNaN(valorUnitario))
            valorUnitario = 0;

        $this.ValorTotal.val(Globalize.format(quantidade * valorUnitario, "n2"));

        AtualizarValorTotal();
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function LoadBaixaPallets() {
    _baixaPallets = new BaixaPallets();
    KoBindings(_baixaPallets, "divModalDevolucaoPallets");

    new BuscarFilial(_baixaPallets.Filial);
    new BuscarClientes(_baixaPallets.Cliente);

    BuscarSituacoes();
    DefineCamposBaixaPallets();
    loadGridAnexoDevolucaoPallet();
    loadDevolucaoPalletAnexo();
}


function loadGridAnexoDevolucaoPallet() {
    var linhasPorPaginas = 5;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoDevolucaoPalletClick, icone: "", visibilidade: true };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: excluirAnexoDevolucaoPalletClick, icone: "", visibilidade: true };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "50%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "50%", className: "text-align-left" }
    ];

    _gridAnexoDevolucaoPallets = new BasicDataTable(_baixaPallets.Anexos.id, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoDevolucaoPallets.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AbrirTelaBaixaPalletsClick(devolucaoPalletsGrid) {
    LimparCamposBaixaPallets();
    _baixaPallets.Codigo.val(devolucaoPalletsGrid.Codigo);
    BuscarPorCodigo(_baixaPallets, "Devolucao/BuscarPorCodigo", function (r) {
        if (r.Success) {
            if (r.Data) {
                _baixaPallets.DataBaixa.val(Global.DataHoraAtual());
                _baixaPallets.Anexos.val(r.Data.Anexos);
                Global.abrirModal('divModalDevolucaoPallets');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function adicionarAnexoDevolucaoPalletClick() {
    $("#divModalDevolucaoPalletsAnexo")
        .modal("show")
        .on('bs.hidden.modal', function () {
            LimaprCampos(_anexoDevolucaoPallet);
        });
}

function BaixarPalletsClick() {
    var valido = ValidarCamposObrigatorios(_baixaPallets);

    if (!valido) {
        exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    var numeroPallets = 0;
    var situacoes = new Array();

    for (var i = 0; i < _baixaPallets.Situacoes().length; i++) {
        var situacao = _baixaPallets.Situacoes()[i];
        var quantidade = Globalize.parseInt(situacao.Quantidade.val());

        if (isNaN(quantidade))
            quantidade = 0;

        numeroPallets += quantidade;

        situacoes.push({
            Codigo: situacao.Codigo.val(),
            Quantidade: quantidade
        });
    }

    if (numeroPallets < _baixaPallets.NumeroPallets.val()) {
        exibirMensagem(tipoMensagem.atencao, "Quantidade de Pallets Inválida", "A quantidade de pallets informada é menor que a quantidade Total.");
        return;
    } else if (numeroPallets > _baixaPallets.NumeroPallets.val()) {
        exibirConfirmacao("Confirmação", "A quantidade de pallets baixadas é maior que a quantidade existente na nota, realmente deseja continuar?", function () {
            salvarBaixaPallets(situacoes);
        });
    } else {
        salvarBaixaPallets(situacoes);
    }
}

/*
 * Declaração das Funções
 */

function DefineCamposBaixaPallets() {
    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Pallets_PermiteDataRetroativa_DevolucaoPallet, _PermissoesPersonalizadasPallets)) {
        _baixaPallets.DataBaixa.visible(true);
        _baixaPallets.DataBaixa.required = true;
    }
}

function AtualizarValorTotal() {
    var valorTotal = 0;

    for (var i = 0; i < _baixaPallets.Situacoes().length; i++) {
        var situacao = _baixaPallets.Situacoes()[i];

        var valor = Globalize.parseFloat(situacao.ValorTotal.val());

        if (isNaN(valor))
            valor = 0;

        valorTotal += valor;
    }

    _baixaPallets.ValorTotal.val(Globalize.format(valorTotal, "n2"));
}

function BuscarSituacoes() {
    executarReST("Devolucao/BuscarSituacoes", {}, function (r) {
        if (r.Success) {
            if (r.Data) {
                for (var i = 0; i < r.Data.length; i++) {
                    var situacaoBaixa = new SituacaoBaixa(r.Data[i]);

                    if (i < (r.Data.length - 1)) {
                        if (r.Data[i + 1].ValorUnitario <= 0) {
                            situacaoBaixa.QuebrarLinha.val(false);
                            situacaoBaixa.QuebrarLinha.def = false;
                        }
                    }

                    _baixaPallets.Situacoes.push(situacaoBaixa);
                    $("#" + situacaoBaixa.Quantidade.id).maskMoney(situacaoBaixa.Quantidade.configInt);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function LimparCamposBaixaPallets() {
    _baixaPallets.Codigo.val(0);
    _baixaPallets.Carga.val("");
    _baixaPallets.NotaFiscal.val("");
    _baixaPallets.Transportador.val("");
    _baixaPallets.Veiculo.val("");
    _baixaPallets.Motorista.val("");
    _baixaPallets.NumeroPallets.val(0);
    _baixaPallets.QuantidadeSituacoes.val("");
    _baixaPallets.Filial.val("");
    _baixaPallets.Filial.codEntity(0);
    _baixaPallets.Cliente.val("");
    _baixaPallets.Cliente.codEntity(0);
    _baixaPallets.ValorTotal.val("0,00");
    _baixaPallets.Observacao.val("");

    Global.ResetarAbas("#divModalDevolucaoPallets");

    for (var i = 0; i < _baixaPallets.Situacoes().length; i++) {
        var situacao = _baixaPallets.Situacoes()[i];

        situacao.Quantidade.val("");
    }
}

function salvarBaixaPallets(situacoes) {
    _baixaPallets.QuantidadeSituacoes.val(JSON.stringify(situacoes));

    Salvar(_baixaPallets, "Devolucao/Salvar", function (r) {
        if (r.Success) {
            if (r.Data) {
                Global.fecharModal('divModalDevolucaoPallets');
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Baixa realizada com sucesso!");
                _gridDevolucaoPallets.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}
