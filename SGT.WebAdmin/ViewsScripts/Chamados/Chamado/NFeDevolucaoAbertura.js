/// <reference path="Abertura.js" />

var _gridNFeDevolucaoAbertura;
var _chamadoOcorrenciaModalObservacaoMotorista;

var NFeDevolucaoAberturaMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.CodigoNotaOrigem = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });

    this.Chave = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Numero = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Serie = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.DataEmissao = PropertyEntity({ type: types.map, val: ko.observable(null), getType: typesKnockout.date  });
    this.ValorTotalProdutos = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorTotal = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.PesoDevolvido = PropertyEntity({ type: types.map, val: ko.observable("") });

    this.PossuiImagem = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.NotaOrigem = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ObservacaoMotorista = PropertyEntity({ type: types.map, val: ko.observable("") });
};

function loadNFeDevolucaoAbertura() {
    if (_gridNFeDevolucaoAbertura != null && _gridNFeDevolucaoAbertura.Destroy)
        _gridNFeDevolucaoAbertura.Destroy();

    var verObservacao = { descricao: "Observacao", id: guid(), metodo: exibirObservacaoMotoristaAbertura, icone: "", visibilidade: visibilidadeVisualizarJustificativaAbertura };
    var baixarNFe = { descricao: "Baixar Imagem NF-e", id: guid(), metodo: baixarImagemNotaDevolucaoAberturaClick, icone: "", visibilidade: visibilidadeBaixarImagemAbertura };
    var excluir = { descricao: "Excluir", id: guid(), metodo: excluirNFeDevolucaoAberturaClick, icone: "", visibilidade: visibilidadeExcluirAbertura };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [baixarNFe, verObservacao, excluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "PossuiImagem", visible: false },
        { data: "ObservacaoMotorista", visible: false },
        { data: "Chave", title: "Chave", width: "30%" },
        { data: "Numero", title: "Número", width: "10%" },
        { data: "Serie", title: "Série", width: "10%" },
        { data: "DataEmissao", title: "Data Emissão", width: "10%" },
        { data: "ValorTotalProdutos", title: "Valor Total Produto", width: "15%" },
        { data: "ValorTotal", title: "Valor Total NF", width: "15%" },
        { data: "PesoDevolvido", title: "Peso Devolvido", width: "20%" },
        { data: "NotaOrigem", title: "NF-e Origem", width: "15%" },
        { data: "CodigoNotaOrigem", visible: false },

    ];

    _gridNFeDevolucaoAbertura = new BasicDataTable(_koNfdAbertura.GridNFeDevolucaoAbertura.id, header, menuOpcoes);

    _chamadoOcorrenciaModalObservacaoMotorista = new bootstrap.Modal(document.getElementById("divModalObservacaoMotorista"), { backdrop: 'static' });
}
function visibilidadeExcluirAbertura() {
    var status = _chamado.Codigo.val() === 0
    return status;
}

function visibilidadeBaixarImagemAbertura(data) {
    return data.PossuiImagem;
}

function visibilidadeVisualizarJustificativaAbertura(data) {
    return !string.IsNullOrWhiteSpace(data.ObservacaoMotorista);
}

function exibirObservacaoMotoristaAbertura(e) {
    $('#PMensagemObservacaoMotorista').html(e.ObservacaoMotorista);
    _chamadoOcorrenciaModalObservacaoMotorista.show();
}

function baixarImagemNotaDevolucaoAberturaClick(e) {
    var data = { Codigo: e.Codigo };
    executarDownload("ChamadoAnalise/DownloadImagemNFDevolucao", data);
}

function adicionarNFeDevolucaoAberturaClick(e, sender) {
    if (!ValidarCamposObrigatorios(_koNfdAbertura))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.ControleEntrega.CamposObrigatorios, Localization.Resources.Cargas.ControleEntrega.PorfavorInformeCamposObrigatorios);

    if (!_koNfdAbertura.NumeroNFeOrigemAbertura.val())
        return exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Informe o Número da NF-e Origem.");

    if (Boolean(_koNfdAbertura.ChaveNFeDevolucaoAbertura.val()) && !ValidarChaveAcesso(_koNfdAbertura.ChaveNFeDevolucaoAbertura.val()))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Chave informada é inválida!");

    var lista = new NFeDevolucaoAberturaMap();

    lista.Codigo.val = guid();
    lista.Chave.val = _koNfdAbertura.ChaveNFeDevolucaoAbertura.val();
    lista.Numero.val = _koNfdAbertura.NumeroNFeDevolucaoAbertura.val();
    lista.Serie.val = _koNfdAbertura.SerieNFeDevolucaoAbertura.val();
    lista.DataEmissao.val = _koNfdAbertura.DataEmissaoNFeDevolucaoAbertura.val();
    lista.ValorTotalProdutos.val = _koNfdAbertura.ValorTotalProdutosNFeDevolucaoAbertura.val();
    lista.ValorTotal.val = _koNfdAbertura.ValorTotalNFeDevolucaoAbertura.val();
    lista.PesoDevolvido.val = _koNfdAbertura.PesoDevolvidoNFeDevolucaoAbertura.val();
    lista.NotaOrigem.val = _koNfdAbertura.NumeroNFeOrigemAbertura.val();
    lista.CodigoNotaOrigem.val = _koNfdAbertura.NumeroNFeOrigemAbertura.codEntity();
    _koNfdAbertura.NFeDevolucaoAbertura.list.push(lista);


    $("#" + _koNfdAbertura.ChaveNFeDevolucaoAbertura.id).focus();
    recarregarGridNFeDevolucaoAbertura();
    LimparCamposNFeDevolucaoAbertura();
}

function excluirNFeDevolucaoAberturaClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a NF-e de Devolução?", function () {
        $.each(_koNfdAbertura.NFeDevolucaoAbertura.list, function (i, lista) {
            if (data.Codigo == lista.Codigo.val) {
                _koNfdAbertura.NFeDevolucaoAbertura.list.splice(i, 1);
                return false;
            }
        });

        recarregarGridNFeDevolucaoAbertura();
    });
}
//*******MÉTODOS*******

function recarregarGridNFeDevolucaoAbertura() {
    var data = new Array();

    $.each(_koNfdAbertura.NFeDevolucaoAbertura.list, function (i, lista) {
        var listaGrid = new Object();

        listaGrid.Codigo = lista.Codigo.val;
        listaGrid.CodigoNotaOrigem = lista.CodigoNotaOrigem.val;
        listaGrid.Chave = lista.Chave.val;
        listaGrid.Numero = lista.Numero.val;
        listaGrid.Serie = lista.Serie.val;
        listaGrid.DataEmissao = lista.DataEmissao.val;
        listaGrid.ValorTotalProdutos = lista.ValorTotalProdutos.val;
        listaGrid.ValorTotal = lista.ValorTotal.val;
        listaGrid.PesoDevolvido = lista.PesoDevolvido.val;
        listaGrid.PossuiImagem = lista.PossuiImagem.val;
        listaGrid.NotaOrigem = lista.NotaOrigem.val;
        listaGrid.ObservacaoMotorista = lista.ObservacaoMotorista.val;

        data.push(listaGrid);
    });

    _gridNFeDevolucaoAbertura.CarregarGrid(data);
}

function ControleCamposNFeDevolucaoAbertura(status) {
    if (typeof status === 'undefined')
        status = _chamado.Codigo.val() === 0;
    
    _koNfdAbertura.ChaveNFeDevolucaoAbertura.enable(status);
    _koNfdAbertura.NumeroNFeOrigemAbertura.enable(status);
    _koNfdAbertura.NumeroNFeDevolucaoAbertura.enable(status);
    _koNfdAbertura.SerieNFeDevolucaoAbertura.enable(status);
    _koNfdAbertura.DataEmissaoNFeDevolucaoAbertura.enable(status);
    _koNfdAbertura.ValorTotalProdutosNFeDevolucaoAbertura.enable(status);
    _koNfdAbertura.ValorTotalNFeDevolucaoAbertura.enable(status);
    _koNfdAbertura.PesoDevolvidoNFeDevolucaoAbertura.enable(status);
    _koNfdAbertura.NumeroNFeOrigemAbertura.enable(status);
    _koNfdAbertura.AdicionarNFeDevolucaoAbertura.enable(status);
    _koNfdAbertura.ImportarXmlNotaFiscalDevolucaoAbertura.enable(status);
}

function LimparCamposNFeDevolucaoAbertura() {
    _koNfdAbertura.NumeroNFeDevolucaoAbertura.val("");
    _koNfdAbertura.SerieNFeDevolucaoAbertura.val("");
    _koNfdAbertura.ValorTotalProdutosNFeDevolucaoAbertura.val("");
    _koNfdAbertura.ValorTotalNFeDevolucaoAbertura.val("");
    _koNfdAbertura.PesoDevolvidoNFeDevolucaoAbertura.val("");
    _koNfdAbertura.NumeroNFeOrigemAbertura.val("");
    _koNfdAbertura.NumeroNFeOrigemAbertura.codEntity(0);

    LimparCampoData(_koNfdAbertura);

}