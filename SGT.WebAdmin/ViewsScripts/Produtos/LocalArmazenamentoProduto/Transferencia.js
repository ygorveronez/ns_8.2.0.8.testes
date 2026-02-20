/// <reference path="../../Enumeradores/EnumSituacaoLocalArmazenamentoLocalTransferencia.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTransferencias;
var _transferencia;
 
var TransferenciaMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.DataTransferencia = PropertyEntity({ text: "Data Transferência", required: true, getType: typesKnockout.date });
    this.CodigoLocalArmazenamentoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Tanque destino"), idBtnSearch: guid(), required: true });
    this.DescricaoLocalArmazenamentoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Tanque destino"), idBtnSearch: guid(), required: true });
    this.QuantidadeTransferida = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "Quantidade" });

    this.Descricao = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoSituacao = PropertyEntity({ type: types.map, val: "" });
    this.CodigoSituacao = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoTransferencia = PropertyEntity({ type: types.map, val: "" });
};

//*******EVENTOS*******

function loadTransferencia() {

    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarTransferencia, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoLocalArmazenamentoDestino", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "20%", className: "text-align-left" },
        { data: "DescricaoLocalArmazenamentoDestino", title: "Local Armazenamento Destino", width: "20%", className: "text-align-left" },
        { data: "DataTransferencia", title: "Data Transferência", width: "25%", className: "text-align-center" },
        { data: "DescricaoSituacao", title: "Situação", width: "25%", className: "text-align-center" },
        { data: "CodigoSituacao", visible: false },
        { data: "DescricaoTransferencia", title: "Descrição Transferência", width: "15%", className: "text-align-center" },
    ];
    _gridTransferencias = new BasicDataTable(_localArmazenamentoProduto.Transferencias.idGrid, header, menuOpcoes);
    recarregarGridTransferencias();
}


function adicionarTransferenciaClick() {
    if (_localArmazenamentoProduto.DataTransferencia.val() === "" || _localArmazenamentoProduto.DescricaoTransferencia.val() === "" || _localArmazenamentoProduto.LocalArmazenamentoDestino.val() === "") {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    let map = new TransferenciaMap();

    map.Codigo.val = guid();

    map.DataTransferencia.val = _localArmazenamentoProduto.DataTransferencia.val();
    map.CodigoLocalArmazenamentoDestino.val = _localArmazenamentoProduto.LocalArmazenamentoDestino.codEntity();
    map.DescricaoLocalArmazenamentoDestino.val = _localArmazenamentoProduto.LocalArmazenamentoDestino.val();
    map.Descricao.val = _localArmazenamentoProduto.DescricaoTransferencia.val();
    map.DescricaoSituacao.val = "Ag. Transferência";
    map.CodigoSituacao.val = EnumSituacaoLocalArmazenamentoLocalTransferencia.AgTransferencia;
    map.DescricaoTransferencia.val = "";
    map.QuantidadeTransferida.val = _localArmazenamentoProduto.QuantidadeTransferida.val();

    _localArmazenamentoProduto.Transferencias.list.push(map);
    console.log(_localArmazenamentoProduto.Transferencias);
    recarregarGridTransferencias();

    limparCamposTransferencias();
}
function excluirTransferenciaClick() {
    if (_localArmazenamentoProduto.SituacaoTransferencia.val() !== EnumSituacaoLocalArmazenamentoLocalTransferencia.AgTransferencia) {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, "Não é possivel excluir o registro na situação atual");
        return;
    }
    _localArmazenamentoProduto.Transferencias.list = _localArmazenamentoProduto.Transferencias.list.filter(transferencia => transferencia.Codigo.val != _localArmazenamentoProduto.CodigoTransferencia.val());
    recarregarGridTransferencias();
    limparCamposTransferencias();

}
function atualizarTransferenciaClick() {

    if (_localArmazenamentoProduto.DataTransferencia.val() === "" || _localArmazenamentoProduto.DescricaoTransferencia.val() === "" || _localArmazenamentoProduto.LocalArmazenamentoDestino.val() === "") {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    if (_localArmazenamentoProduto.SituacaoTransferencia.val() !== EnumSituacaoLocalArmazenamentoLocalTransferencia.AgTransferencia) {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, "Não é possivel atualizar o registro na situação atual");
        return;
    }
    $.each(_localArmazenamentoProduto.Transferencias.list, function (i, item) {
        if (item.Codigo.val == _localArmazenamentoProduto.CodigoTransferencia.val()) {
            item.DataTransferencia.val = _localArmazenamentoProduto.DataTransferencia.val();
            item.CodigoLocalArmazenamentoDestino.val = _localArmazenamentoProduto.LocalArmazenamentoDestino.codEntity();
            item.DescricaoLocalArmazenamentoDestino.val = _localArmazenamentoProduto.LocalArmazenamentoDestino.val();
            item.Descricao.val = _localArmazenamentoProduto.DescricaoTransferencia.val();
            item.DescricaoSituacao.val = "Ag. Transferência";
            item.CodigoSituacao.val = EnumSituacaoLocalArmazenamentoLocalTransferencia.AgTransferencia;
            item.DescricaoTransferencia.val = "";
            item.QuantidadeTransferida.val = _localArmazenamentoProduto.QuantidadeTransferida.val();
            return false;
        }
    });

    recarregarGridTransferencias();
    limparCamposTransferencias();
}
function editarTransferencia(data) {

    limparCamposTransferencias();

    $.each(_localArmazenamentoProduto.Transferencias.list, function (i, item) {
        if (item.Codigo.val == data.Codigo) {
            _localArmazenamentoProduto.CodigoTransferencia.val(item.Codigo.val);
            _localArmazenamentoProduto.DescricaoTransferencia.val(item.Descricao.val);
            _localArmazenamentoProduto.LocalArmazenamentoDestino.val(item.DescricaoLocalArmazenamentoDestino.val);
            _localArmazenamentoProduto.LocalArmazenamentoDestino.codEntity(item.CodigoLocalArmazenamentoDestino.val);
            _localArmazenamentoProduto.DataTransferencia.val(item.DataTransferencia.val);
            _localArmazenamentoProduto.SituacaoTransferencia.val(item.CodigoSituacao.val);
            _localArmazenamentoProduto.QuantidadeTransferida.val(item.QuantidadeTransferida.val);
            return false;
        }
    });

    _localArmazenamentoProduto.AdicionarTransferencia.visible(false);
    _localArmazenamentoProduto.AtualizarTransferencia.visible(true);
    _localArmazenamentoProduto.ExcluirTransferencia.visible(true);
    _localArmazenamentoProduto.CancelarTransferencia.visible(true);
}
function recarregarGridTransferencias() {

    let dados = _localArmazenamentoProduto.Transferencias.list.map(item => ({
        Codigo: item.Codigo.val,
        Descricao: item.Descricao.val,
        CodigoLocalArmazenamentoDestino: item.CodigoLocalArmazenamentoDestino.val,
        DescricaoLocalArmazenamentoDestino: item.DescricaoLocalArmazenamentoDestino.val,
        DataTransferencia: item.DataTransferencia.val,
        DescricaoSituacao: item.DescricaoSituacao.val,
        CodigoSituacao: item.CodigoSituacao.val,
        DescricaoTransferencia: item.DescricaoTransferencia.val
    }));

    _gridTransferencias.CarregarGrid(dados);
}

function carregarGridTransferencias(lista) {

    if (lista === "undefined" || lista === null || lista.length === 0) {
        return;
    }

    for (let i = 0; i <= lista.length - 1; i++) {

        let map = new TransferenciaMap();

        map.DataTransferencia = lista[i].DataTransferencia;
        map.CodigoLocalArmazenamentoDestino = lista[i].CodigoLocalArmazenamentoDestino;
        map.DescricaoLocalArmazenamentoDestino = lista[i].DescricaoLocalArmazenamentoDestino;
        map.Descricao = lista[i].DescricaoTransferencia;
        map.DescricaoSituacao = lista[i].DescricaoSituacao;
        map.CodigoSituacao = lista[i].CodigoSituacao;
        map.DescricaoTransferencia = lista[i].DescricaoTransferencia;

    }

    recarregarGridTransferencias();
}

function limparCamposTransferencias() {
    _localArmazenamentoProduto.DataTransferencia.val("");
    _localArmazenamentoProduto.DescricaoTransferencia.val("");

    _localArmazenamentoProduto.AdicionarTransferencia.visible(true);
    _localArmazenamentoProduto.AtualizarTransferencia.visible(false);
    _localArmazenamentoProduto.ExcluirTransferencia.visible(false);
    _localArmazenamentoProduto.CancelarTransferencia.visible(false);

    LimparCampoEntity(_localArmazenamentoProduto.LocalArmazenamentoDestino);
}