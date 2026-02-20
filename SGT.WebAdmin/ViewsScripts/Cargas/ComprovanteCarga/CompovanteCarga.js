/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/TipoComprovante.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _comprovanteCarga;
var _gridComprovanteCarga;
var _pesquisaComprovanteCarga;
var _modalComprovanteDetalhes;
var _modalComprovanteEnviar;
var _modalComprovanteJustificar;
var _modalComprovanteReverter;

//*******CLASSES KNOCKOUT**********
var PesquisaComprovanteCarga = function () {
    var dataAtual = moment().format("DD/MM/YYYY");
    
    this.Codigo = PropertyEntity({ text: "Codigo:", val: ko.observable(0) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable(""), options: EnumSituacaoComprovanteCarga.obterOpcoesPesquisa(), def: "" });
    this.TipoComprovante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Comprovante:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.MotoristaCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.VeiculosCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Veiculo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataCarga = PropertyEntity({ text: "Data da Carga:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridComprovanteCarga.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

};

var DetalhesComprovanteCarga = function () {
    this.Codigo = PropertyEntity({ text: "Codigo:", val: ko.observable(0) });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable("") });

    this.Carga = PropertyEntity({ text: "Carga:", val: ko.observable(0) });
    this.Remetente = PropertyEntity({ text: "Remetente:", val: ko.observable("") });
    this.Destinatario = PropertyEntity({ text: "Destinatário:", val: ko.observable("") });
    this.Motorista = PropertyEntity({ text: "Motorista:", val: ko.observable("") });

    this.DataJustificativa = PropertyEntity({ text: "Data da Justificativa:", val: ko.observable(""), visible: ko.observable(true) });
    this.MotivoJustificativa = PropertyEntity({ text: "Motivo da Justificativa:", val: ko.observable(""), visible: ko.observable(true) });
    this.DataEntrega = PropertyEntity({ text: "Data da Entrega:", val: ko.observable(""), visible: ko.observable(true) });

    this.Origem = PropertyEntity({ text: "Origem:", val: ko.observable("") });
    this.Destino = PropertyEntity({ text: "Destino:", val: ko.observable("") });
    this.Veiculo = PropertyEntity({ text: "Veículo:", val: ko.observable("") });

    this.Fechar = PropertyEntity({
        eventClick: function (e) {
            _modalComprovanteDetalhes.hide();
        }, type: types.event, text: "Fechar", idGrid: guid(), visible: ko.observable(true)
    });
};

var EnviarComprovanteCarga = function () {
    this.Codigo = PropertyEntity({ text: "Codigo:", val: ko.observable(0) });
    this.Carga = PropertyEntity({ text: "Carga:", val: ko.observable(0) });
    this.DataEntrega = PropertyEntity({ text: "*Data Atualização:", enable: false, getType: typesKnockout.date, val: ko.observable(""), enable: false, required: true });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Arquivo:", val: ko.observable(""), accept: ".jpg,.tif,.pdf,.png", enable: ko.observable(true), visible: ko.observable(true), required: true });
    this.NomeArquivo = PropertyEntity({ text: ko.observable("Selecione um arquivo para anexar"), val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _enviarComprovanteCarga.NomeArquivo.text(nomeArquivo);
        _enviarComprovanteCarga.NomeArquivo.val(nomeArquivo);
    });

    this.Operacao = PropertyEntity({ val: ko.observable("Enviar") });
    
    this.Enviar = PropertyEntity({
        eventClick: function (e) {
            if (validarEnviarComprovante(_enviarComprovanteCarga)) {
                uploadComprovante(_enviarComprovanteCarga);
                _modalComprovanteEnviar.hide();
                _gridComprovanteCarga.CarregarGrid();
            }

        }, type: types.event, text: "Enviar", idGrid: guid(), visible: ko.observable(true)
    });
};

var JustificarComprovanteCarga = function () {
    this.Codigo = PropertyEntity({ text: "Codigo:", val: ko.observable(0) });
    this.Carga = PropertyEntity({ text: "Carga:", val: ko.observable(0) });
    this.DataJustificativa = PropertyEntity({ text: "*Data Justificativa:", enable: false, getType: typesKnockout.date, val: ko.observable(""), enable: false, required: true });
    this.MotivoJustificativa = PropertyEntity({ text: "*Motivo:", val: ko.observable(""), required: true, maxlength: 500 });
    this.Operacao = PropertyEntity({ val: ko.observable("Justificar") });

    this.Justificar = PropertyEntity({
        eventClick: function (e, sender) {
            if (validarJustificarComprovante(_justificarComprovanteCarga)) {
                if (atualizarComprovante(_justificarComprovanteCarga, sender)) {
                    _modalComprovanteJustificar.hide();
                    _gridComprovanteCarga.CarregarGrid();
                }
            }
        }, type: types.event, text: "Justificar", idGrid: guid(), visible: ko.observable(true)
    });
};

var ReverterComprovanteCarga = function () {
    this.Codigo = PropertyEntity({ text: "Codigo:", val: ko.observable(0) });
    this.Carga = PropertyEntity({ text: "Carga:", val: ko.observable(0) });
    this.Mensagem = PropertyEntity({ text: ko.observable("") });
    this.Operacao = PropertyEntity({ val: ko.observable("Reverter") });
    
    this.Reverter = PropertyEntity({
        eventClick: function (e, sender) {
            if (atualizarComprovante(_reverterComprovanteCarga, sender)) {
                _modalComprovanteReverter.hide();
                _gridComprovanteCarga.CarregarGrid();
            }
        }, type: types.event, text: "Reverter", idGrid: guid(), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadComprovanteCarga() {
    _pesquisaComprovanteCarga = new PesquisaComprovanteCarga();
    KoBindings(_pesquisaComprovanteCarga, "knockoutPesquisaComprovanteCarga", false, _pesquisaComprovanteCarga.Pesquisar.id);

    HeaderAuditoria("ComprovanteCarga");

    buscarComprovanteCarga();

    new BuscarCargas(_pesquisaComprovanteCarga.Carga);
    new BuscarTipoComprovante(_pesquisaComprovanteCarga.TipoComprovante);
    new BuscarMotoristas(_pesquisaComprovanteCarga.MotoristaCarga);
    new BuscarVeiculos(_pesquisaComprovanteCarga.VeiculosCarga);

    _modalComprovanteDetalhes = new bootstrap.Modal(document.getElementById("divModalComprovanteDetalhes"), { backdrop: true, keyboard: true });
    _modalComprovanteEnviar = new bootstrap.Modal(document.getElementById("divModalComprovanteEnviar"), { backdrop: true, keyboard: true });
    _modalComprovanteJustificar = new bootstrap.Modal(document.getElementById("divModalComprovanteJustificar"), { backdrop: true, keyboard: true });
    _modalComprovanteReverter = new bootstrap.Modal(document.getElementById("divModalComprovanteReverter"), { backdrop: true, keyboard: true });
}


function BuscarAuditoriaOrdemServico(documento) {
    _AuditoriaOrdemServico = new AuditoriaOrdemServico();
    KoBindings(_AuditoriaOrdemServico, "divModalVisualizarAuditoriaOrdemServico")

    _AuditoriaOrdemServico.Codigo.val(documento.CodigoDocumentoEntrada);
    _AuditoriaOrdemServico.FornecedorDocumentoEntrada.val(documento.FornecedorDocumentoEntrada);
    _AuditoriaOrdemServico.FornecedorOrdemCompra.val(documento.FornecedorOrdemCompra);

    _gridAuditoriaOrdemServico = new GridView("tblAuditoriaOrdemServico", "DocumentoDestinadoEmpresa/PesquisaAuditoriaOrdemServico", _AuditoriaOrdemServico, null, { column: 1, dir: orderDir.desc });
    _gridAuditoriaOrdemServico.CarregarGrid();
}

function validarEnviarComprovante(knockoutEnviar) {
    if (knockoutEnviar.Arquivo.val() == "") {
        return exibirMensagem(tipoMensagem.atencao, "Arquivo", "Nenhum arquivo selecionado.");
    }
    return ValidarCamposObrigatorios(knockoutEnviar);
}

function validarJustificarComprovante(knockoutJustificar) {
    if (knockoutJustificar.MotivoJustificativa.val() == "") {
        return exibirMensagem(tipoMensagem.atencao, "Justificativa", "Motivo da justificativa é obrigatório.");
    }
    return ValidarCamposObrigatorios(knockoutJustificar);
}

function atualizarComprovante(knockout, sender) {
    Salvar(knockout, "ComprovanteCarga/AtualizarComprovante", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
    return true;
}

//*******MÉTODOS*******

function buscarComprovanteCarga() {
    var detalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: ExibirDetalhes, tamanho: "20", icone: "", visibilidade: true };
    var enviarComprovante = { descricao: "Enviar Comprovante", id: guid(), evento: "onclick", metodo: EnviarComprovante, tamanho: "20", icone: "", visibilidade: VisibilidadeEnviar };
    var justificar = { descricao: "Justificar", id: guid(), evento: "onclick", metodo: JustificarComprovante, tamanho: "20", icone: "", visibilidade: VisibilidadeJustificar };
    var baixarComprovante = { descricao: "Baixar Comprovante", id: guid(), evento: "onclick", metodo: BaixarComprovante, tamanho: "20", icone: "", visibilidade: VisibilidadeBaixarComprovante };
    var reverter = { descricao: "Reverter", id: guid(), evento: "onclick", metodo: ReverterComprovante, tamanho: "20", icone: "", visibilidade: VisibilidadeReverter };
    var auditar = { descricao: "Auditar", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("ComprovanteCarga", null, _comprovanteCarga), tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [detalhes, enviarComprovante, justificar, baixarComprovante, reverter, auditar],
        tamanho: 7
    };

    var configExportacao = {
        url: "ComprovanteCarga/ExportarPesquisa",
        titulo: "Comprovantes de Cargas"
    };

    _gridComprovanteCarga = new GridViewExportacao("grid-comprovantes-carga", "ComprovanteCarga/Pesquisa", _pesquisaComprovanteCarga, menuOpcoes, configExportacao, { column: 5, dir: orderDir.desc }, 10);
    _gridComprovanteCarga.SetPermitirEdicaoColunas(true);
    _gridComprovanteCarga.SetSalvarPreferenciasGrid(true);
    _gridComprovanteCarga.CarregarGrid();
}

function VisibilidadeEnviar(dataRow) {
    return dataRow.Situacao == EnumSituacaoComprovanteCarga.obterOpcoes()[EnumSituacaoComprovanteCarga.Pendente].text;
}

function VisibilidadeJustificar(dataRow) {
    return dataRow.Situacao == EnumSituacaoComprovanteCarga.obterOpcoes()[EnumSituacaoComprovanteCarga.Pendente].text;
}

function VisibilidadeBaixarComprovante(dataRow) {
    return dataRow.Situacao == EnumSituacaoComprovanteCarga.obterOpcoes()[EnumSituacaoComprovanteCarga.Recebido].text;
}

function VisibilidadeReverter(dataRow) {
    return dataRow.Situacao != EnumSituacaoComprovanteCarga.obterOpcoes()[EnumSituacaoComprovanteCarga.Pendente].text && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ComprovanteCarga_PermiteReverterComprovante, _PermissoesPersonalizadas);
}

function ExibirDetalhes(comprovante) {
    _detalhesComprovanteCarga = new DetalhesComprovanteCarga();
    KoBindings(_detalhesComprovanteCarga, "knockoutComprovanteCargaDetalhes", false, _detalhesComprovanteCarga.Fechar.id);

    _detalhesComprovanteCarga.Codigo.val(comprovante.Codigo);

    BuscarPorCodigo(_detalhesComprovanteCarga, "ComprovanteCarga/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _detalhesComprovanteCarga.DataEntrega.visible(_detalhesComprovanteCarga.Situacao.val() == 'Recebido');
                _detalhesComprovanteCarga.DataJustificativa.visible(_detalhesComprovanteCarga.Situacao.val() == 'Justificado')
                _detalhesComprovanteCarga.MotivoJustificativa.visible(_detalhesComprovanteCarga.Situacao.val() == 'Justificado')
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);


    _modalComprovanteDetalhes.show();
}

function EnviarComprovante(comprovante) {
    _enviarComprovanteCarga = new EnviarComprovanteCarga()
    KoBindings(_enviarComprovanteCarga, "knockoutComprovanteCargaEnviar", false, _enviarComprovanteCarga.Enviar.id);

    _enviarComprovanteCarga.Codigo.val(comprovante.Codigo);
    _enviarComprovanteCarga.Carga.val(comprovante.Carga);

    _modalComprovanteEnviar.show();
}

function JustificarComprovante(comprovante) {
    _justificarComprovanteCarga = new JustificarComprovanteCarga()
    KoBindings(_justificarComprovanteCarga, "knockoutComprovanteCargaJustificar", false, _justificarComprovanteCarga.Justificar.id);

    _justificarComprovanteCarga.Codigo.val(comprovante.Codigo);
    _justificarComprovanteCarga.Carga.val(comprovante.Carga);

    _modalComprovanteJustificar.show();
}

function ReverterComprovante(comprovante) {
    _reverterComprovanteCarga = new ReverterComprovanteCarga()
    KoBindings(_reverterComprovanteCarga, "knockoutComprovanteCargaReverter", false, _reverterComprovanteCarga.Reverter.id);

    _reverterComprovanteCarga.Codigo.val(comprovante.Codigo);
    _reverterComprovanteCarga.Carga.val(comprovante.Carga);
    _reverterComprovanteCarga.Mensagem.text("Deseja reverter a situação '" + comprovante.Situacao + "' do comprovante?");
    
    _modalComprovanteReverter.show();
}

function BaixarComprovante(comprovante) {
    executarDownload("ComprovanteCarga/DownloadComprovante", { Codigo: comprovante.Codigo });
}

function uploadComprovante(knockoutEnviar) {

    var arquivo = document.getElementById(knockoutEnviar.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

    var anexo = {
        DataEntrega: knockoutEnviar.DataEntrega,
        Codigo: knockoutEnviar.Codigo,
        Arquivo: arquivo.files[0]
    };


    var formData = new FormData();
    formData.append("Arquivo", arquivo.files[0]);
    formData.append("Codigo", knockoutEnviar.Codigo.val());
    formData.append("DataEntrega", knockoutEnviar.DataEntrega.val());

    var data = {
        //DataEntrega1: knockoutEnviar.DataEntrega.val(),
        //Codigo1: knockoutEnviar.Codigo.val()
    };

    enviarArquivo("ComprovanteCarga/EnviarComprovante?callback=?", data, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                
            } else {
                exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}