/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/ModeloVeiculo.js" />
/// <reference path="../../Consultas/Equipamento.js" />
/// <reference path="../../Consultas/MarcaVeiculo.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/OrdemServico.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Enumeradores/EnumSituacaoVeiculo.js" />
/// <reference path="../../Cargas/Carga/DadosCarga/Carga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _painelVeiculoLavacao;
var gridHistoricoLavacao;

var PainelVeiculoLavacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataLavacao = PropertyEntity({ text: "Data da lavação: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: ko.observable(true) });
    this.AnexoAntesLavacao = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo antes da lavação:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true), required: ko.observable(true) });
    this.NomeArquivoAnexoAntesLavacao = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.AnexoDepoisLavacao = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo depois da lavação:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true), required: ko.observable(true) });
    this.NomeArquivoAnexoDepoisLavacao = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.HistoricoLavacao = PropertyEntity({ text: "Histórico Lavação", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.HistoricoLavacao.val.subscribe(recarregarGridHstoricoLavacao);
    this.AnexoAntesLavacao.val.subscribe(function (novoValor) { _painelVeiculoLavacao.NomeArquivoAnexoAntesLavacao.val(novoValor.replace('C:\\fakepath\\', '')); });
    this.AnexoDepoisLavacao.val.subscribe(function (novoValor) { _painelVeiculoLavacao.NomeArquivoAnexoDepoisLavacao.val(novoValor.replace('C:\\fakepath\\', '')); });

    this.AdicionarLavacao = PropertyEntity({ eventClick: adicionarLavacaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

//*******EVENTOS*******
function loadPainelVeiculoLavacao() {
    _painelVeiculoLavacao = new PainelVeiculoLavacao();
    KoBindings(_painelVeiculoLavacao, "knoutPainelVeiculoLavacao", false);

    loadGridHistoricoLavacao();
}

function lavacaoClick(e) {
    limparCampos();
    // Seta o codigo do objeto
    _indicacaoVeiculo.Codigo.val(e.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_indicacaoVeiculo, "PainelVeiculo/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.PainelVeiculoViagem != null)
                    PreencherObjetoKnout(_painelVeiculoViagem, { Data: arg.Data.PainelVeiculoViagem });
                if (arg.Data.PainelVeiculoManutencao != null)
                    PreencherObjetoKnout(_painelVeiculoManutencao, { Data: arg.Data.PainelVeiculoManutencao });
                if (arg.Data.HistoricoLavacao != null)
                    _painelVeiculoLavacao.HistoricoLavacao.val(arg.Data.HistoricoLavacao);
                else 
                    gridHistoricoLavacao.CarregarGrid([]);
                _naoPaginar = true;
                RecarregarListaReboques();
                _indicacaoVeiculo.AvisadoCarregamento.visible(true);
                _indicacaoVeiculo.VeiculoVazio.visible(false);

                _indicacaoVeiculo.IndicacaoVeiculoVazio.val(false);
                _indicacaoVeiculo.IndicacaoAvisoCarregamento.val(true);
                _indicacaoVeiculo.IndicacaoManutencao.val(false);
                _indicacaoVeiculo.IndicacaoViagem.val(false);

                _painelVeiculoModalIndicacaoVeiculo.show();
                $("#liTabManutencao").hide();
                $("#liTabViagem").hide();
                $("#liTabLavacao").show();
                Global.ExibirAba("divLavacao");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function adicionarLavacaoClick(e, sender) {
    const anexoAntesLavacao = document.getElementById(_painelVeiculoLavacao.AnexoAntesLavacao.id);
    const anexoDepoisLavacao = document.getElementById(_painelVeiculoLavacao.AnexoDepoisLavacao.id);

    if (!ValidarCamposObrigatorios(_painelVeiculoLavacao))
        return exibirMensagem(tipoMensagem.aviso, "Aviso", "Favor informe os campos obrigatórios.");

    if (anexoAntesLavacao.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Selecione um arquivo para antes da lavação");

    if (anexoDepoisLavacao.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Selecione um arquivo para depois da lavação");

    const anexo = {
        Codigo: guid(),
        DataLavacao: _painelVeiculoLavacao.DataLavacao.val(),
        NomeArquivoAnexoAntesLavacao: _painelVeiculoLavacao.NomeArquivoAnexoAntesLavacao.val(),
        AnexoAntesLavacao: anexoAntesLavacao.files[0],
        NomeArquivoAnexoDepoisLavacao: _painelVeiculoLavacao.NomeArquivoAnexoDepoisLavacao.val(),
        AnexoDepoisLavacao: anexoDepoisLavacao.files[0]
    };

    if (_indicacaoVeiculo.Codigo.val() > 0)
        enviarAnexos(_indicacaoVeiculo.Codigo.val(), [anexo]);

    LimparCampos(_painelVeiculoLavacao);

    anexoAntesLavacao.value = null;
    anexoDepoisLavacao.value = null;
}

function loadGridHistoricoLavacao() {
    const linhasPorPaginas = 5;
    const opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoClick, icone: "" };
    const opcaoExcluir = { descricao: "Excluir", tamanho: 15, id: guid(), metodo: excluirLavacaoClick, icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [opcaoDownload, opcaoExcluir] };

    const header = [
        { data: "Codigo", visible: false },
        { data: "DataLavacao", title: "Data lavação", width: "15%", className: "text-align-center", orderable: true },
        { data: "NomeArquivoAnexoAntesLavacao", title: "Anexo antes da lavação", width: "30%", className: "text-align-center", orderable: false },
        { data: "NomeArquivoAnexoDepoisLavacao", title: "Anexo depois da lavação", width: "30%", className: "text-align-center", orderable: false }
    ];

    gridHistoricoLavacao = new BasicDataTable(_painelVeiculoLavacao.HistoricoLavacao.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    gridHistoricoLavacao.CarregarGrid([]);
}

function obterAnexos() {
    return _painelVeiculoLavacao.HistoricoLavacao.val().slice();
}

function obterFormDataAnexo(anexos) {
    if (anexos.length > 0) {
        var formData = new FormData();

        anexos.forEach(function (anexo) {
            formData.append("DataLavacao", anexo.DataLavacao);
            formData.append("AnexoAntesLavacao", anexo.AnexoAntesLavacao);
            formData.append("AnexoDepoisLavacao", anexo.AnexoDepoisLavacao);
            formData.append("NomeArquivoAnexoAntesLavacao", anexo.NomeArquivoAnexoAntesLavacao);
            formData.append("NomeArquivoAnexoDepoisLavacao", anexo.NomeArquivoAnexoDepoisLavacao);
        });

        return formData;
    }

    return undefined;
}

function recarregarGridHstoricoLavacao() {
    let historicoLavacao = obterAnexos();

    gridHistoricoLavacao.CarregarGrid(historicoLavacao);
}

function downloadAnexoClick(registroSelecionado) {
    executarDownload("PainelVeiculoLavacaoAnexo/DownloadLavacaoAnexos", { Codigo: registroSelecionado.Codigo });
}

function excluirLavacaoClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoLocal(registroSelecionado);
    else {
        executarReST("PainelVeiculoLavacaoAnexo/ExcluirLavacaoEAnexos", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivos excluidos com sucesso");
                    removerAnexoLocal(registroSelecionado.Codigo);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function enviarAnexos(codigo, anexos) {
    const formData = obterFormDataAnexo(anexos);

    if (formData) {
        enviarArquivo("PainelVeiculoLavacaoAnexo/AdicionarLavacaoEAnexos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _painelVeiculoLavacao.HistoricoLavacao.val(retorno.Data.Anexos);

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivos anexados com sucesso");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Não foi possivel anexar", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function removerAnexoLocal(registroSelecionado) {
    let listaAnexos = obterAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _painelVeiculoLavacao.HistoricoLavacao.val(listaAnexos);
}