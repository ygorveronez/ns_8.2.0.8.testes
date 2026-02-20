/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _knoutDetalheDevolucao;
var _gridAnexosDevolucaoDetalhes;

/*
 * Declaração das Classes
 */

var DetalheDevolucao = function () {
    var isTMS = (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS);
    var isCTe = (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe);

    this.Chave = PropertyEntity({ text: "Chave NF-e: ", val: ko.observable(""), def: "", visible: !isTMS, idGrid: guid() });
    this.Numero = PropertyEntity({ text: "Número NF-e:", val: ko.observable(""), def: "", visible: !isTMS });
    this.NumeroDevolucao = PropertyEntity({ text: "Nº da Devolução:", val: ko.observable(""), def: "", visible: true, idGrid: guid() });
    this.DataEmissao = PropertyEntity({ text: "Data Emissão:", val: ko.observable(""), def: "", visible: true });
    this.Destinatario = PropertyEntity({ text: "Destinatário: ", val: ko.observable(""), def: "", visible: true, idGrid: guid(), visible: !isCTe });
    this.Empresa = PropertyEntity({ text: "Empresa: ", val: ko.observable(""), def: "", visible: !isCTe });
    this.Valor = PropertyEntity({ text: "Valor: ", val: ko.observable(""), def: "", visible: true });
    this.Peso = PropertyEntity({ text: "Peso: ", val: ko.observable(""), def: "", visible: true });
    this.NaturezaOP = PropertyEntity({ text: "Natureza da Operação: ", val: ko.observable(""), def: "", visible: false });
    this.Carga = PropertyEntity({ text: "Carga: ", val: ko.observable(""), def: "", visible: !isCTe });
    this.Motoristas = PropertyEntity({ text: "Motoristas: ", val: ko.observable(""), def: "", visible: !isCTe });
    this.Filial = PropertyEntity({ text: "Filial: ", val: ko.observable(""), def: "", visible: (!isTMS && !isCTe) });
    this.Emitente = PropertyEntity({ text: "Emitente: ", val: ko.observable(""), def: "", visible: !isCTe });
    this.Observacao = PropertyEntity({ text: "Observação: ", val: ko.observable(""), def: "", visible: !isCTe });
    this.QuantidadePallets = PropertyEntity({ text: "Quantidade Pallets Devolução: ", val: ko.observable(""), def: "", visible: !isCTe });
    this.QuantidadePalletsValePallet = PropertyEntity({ text: "Quantidade Pallets Vale Pallet: ", val: ko.observable(""), def: "", visible: (!isTMS && !isCTe) });
    this.QuantidadeTotalPallets = PropertyEntity({ text: "Quantidade Total Pallets: ", val: ko.observable(""), def: "", visible: !isCTe });
    this.PesoTotalPallets = PropertyEntity({ text: "Peso Pallets: ", val: ko.observable(""), def: "", visible: !isCTe });
    this.ValorTotalPallets = PropertyEntity({ text: "Valor Total Pallets: ", val: ko.observable(""), def: "", visible: !isCTe });

    this.Anexos = PropertyEntity({ type: types.local });
    
    this.ExibirSituacoesDevolucao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadDetalheDevolucao() {
    _knoutDetalheDevolucao = new DetalheDevolucao();
    KoBindings(_knoutDetalheDevolucao, "KnoutDetalheDevolucao");

    loadGridAnexoDevolucaoPalletDetalhes();
}

function loadGridAnexoDevolucaoPalletDetalhes() {
    var linhasPorPaginas = 5;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoDevolucaoPalletClick, icone: "", visibilidade: true };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "50%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "50%", className: "text-align-left" }
    ];

    _gridAnexosDevolucaoDetalhes = new BasicDataTable(_knoutDetalheDevolucao.Anexos.id, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosDevolucaoDetalhes.CarregarGrid([]);
}

/*
 * Declaração das Funções
 */

function BuscarDetalheDevolucao(codigo, callback) {
    var dados = {
        Codigo: codigo
    }

    executarReST("Devolucao/BuscarDetalhesDevolucao", dados, function (arg) {
        if (arg.Success) {
            var retorno = { Data: arg.Data };

            PreencherObjetoKnout(_knoutDetalheDevolucao, retorno);
            _gridAnexosDevolucaoDetalhes.CarregarGrid(retorno.Data.Anexos);

            if ((retorno.Data.Situacoes.length > 0) && (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe)) {
                $("#liTabSituacoesDevolucao").find("a:first").tab("show")
                
                var html = "<tr><th style='width:60%'>Situação</th><th style='text-align:right'>Quantidade</th><th style='text-align:right'>Valor Unitário</th><th style='text-align:right'>Total</th></tr>";

                for (var i = 0; i < retorno.Data.Situacoes.length; i++) {
                    html += "<tr><td>" + retorno.Data.Situacoes[i].Situacao + "</td><td  style='text-align:right'>" + retorno.Data.Situacoes[i].Quantidade + "</td><td  style='text-align:right'>" + retorno.Data.Situacoes[i].ValorUnitario + "</td><td  style='text-align:right'>" + retorno.Data.Situacoes[i].ValorTotal + "</td></tr>"
                }

                $("#" + _knoutDetalheDevolucao.NumeroDevolucao.idGrid).html(html);

                _knoutDetalheDevolucao.ExibirSituacoesDevolucao.val(true);
            }
            else {
                _knoutDetalheDevolucao.ExibirSituacoesDevolucao.val(false);
                $("#liTabAnexosDevolucaoPalletDetalhes").find("a:first").tab("show")
            }

            callback();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    })
}
