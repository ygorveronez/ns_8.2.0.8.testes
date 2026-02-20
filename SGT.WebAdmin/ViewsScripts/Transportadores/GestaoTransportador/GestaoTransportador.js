/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../Enumeradores/EnumTipoSerie.js" />
/// <reference path="../../Consultas/SerieTransportador.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoIntramunicipal.js" />
/// <reference path="../../Enumeradores/EnumResponsavelSeguro.js" />
/// <reference path="../../Consultas/ApoliceSeguro.js" />
// ...

var _gridTransportadorGestaoConfiguracaoNFSe;
var _gridSerie;
var _crudTransportador;
var _transportadorGestao;
var _CteMdfe;
var _pesquisaconfiguracaoNFE;
var _pesquisaTransportador;
var _CadastroSerie;
var _configuracaoNFE;
var _certificado;
var _serie;
var _seguro;

var _statusSerie = [
    { text: "Ativo", value: "A" },
    { text: "Inativo", value: "I" }
];

var _statusChar = [
    { text: "Ativo", value: "A" },
    { text: "Inativo", value: "I" }
];

//Objetos

var Transportador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.RazaoSocial = PropertyEntity({ text: "*Razão Social", required: true, maxlength: 80 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração", issue: 15, required: false, maxlength: 50 });
    this.NomeFantasia = PropertyEntity({ text: "*Nome Fantasia", required: true });
    this.TipoEmpresa = PropertyEntity({ val: ko.observable("J"), /*options: ObterTiposEmpresa(),*/ text: "Tipo Empresa", /*eventChange: tipoEmpresaChange,*/ visible: ko.observable(true) });
    this.CNPJ = PropertyEntity({ text: "*CNPJ", issue: 4, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.RegistroANTT = PropertyEntity({ text: "*RNTRC", issue: 660, maxlength: 8, required: true, cssClass: ko.observable("col-12 col-md-4") });
    this.InscricaoEstadual = PropertyEntity({ text: "*Inscrição Estadual", issue: 744, maxlength: 20, required: true });
    this.InscricaoMunicipal = PropertyEntity({ text: "Inscrição Municipal", issue: 750, maxlength: 20, required: false, visible: true });
    this.CNAE = PropertyEntity({ text: "CNAE", issue: 746, maxlength: 20, required: false });
    this.CEP = PropertyEntity({ text: " *CEP", maxlength: 10, required: true });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Cidade", idBtnSearch: guid() });
    this.Endereco = PropertyEntity({ text: "*Endereço", required: true });
    this.Numero = PropertyEntity({ text: "*Número", required: true });
    this.Bairro = PropertyEntity({ text: "*Bairro", required: true });
    this.Complemento = PropertyEntity({ text: "Complemento", maxlength: 100 });
    this.Telefone = PropertyEntity({ text: "*Telefone", issue: 749, required: true, getType: typesKnockout.phone });
    this.Status = PropertyEntity({ val: ko.observable("A"), options: _statusChar, def: "A", text: "Status", issue: 557, required: true });
    this.Series = PropertyEntity({ type: types.listEntity, list: new Array(), val: ko.observable(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Certificado = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.CTEMDF = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.configuracaoNFE = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.UsarTipoOperacaoApolice = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.Email = PropertyEntity({ text: "E-mail:", getType: typesKnockout.multiplesEmails, maxlength: 1000 });
    this.EmailAdministrativo = PropertyEntity({ text: "E-mail Administrativo:", getType: typesKnockout.multiplesEmails, maxlength: 1000 });
    this.EmailContador = PropertyEntity({ text: "E-mail Contador:", getType: typesKnockout.multiplesEmails, maxlength: 1000 });

    this.EnviarEmail = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: " XML" });
    this.EnviarEmailAdministrativo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: " XML" });
    this.EnviarEmailContador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: " XML" });
};

var CRUDTransportador = function () {
    this.ReenviarDadosAcessoaCliente = PropertyEntity({ eventClick: reenviarDadosAcessoaClienteClickGestao, type: types.event, text: "Reenviar dados de acesso", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClickGestao, type: types.event, text: "Atualizar", visible: ko.observable(true) });
};

var CertificadoObj = function () {
    this.PossuiCertificado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(true), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(true), getType: typesKnockout.date });
    this.SerieCertificado = PropertyEntity({ text: "Serie:", visible: ko.observable(true), required: false });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Certificado Digital:", val: ko.observable(""), visible: ko.observable(true) });
    this.Senha = PropertyEntity({ text: "Senha:", visible: ko.observable(true), required: false });

    this.Enviar = PropertyEntity({ eventClick: enviarCertificadoClick, type: types.event, text: "Enviar", visible: ko.observable(false) });
    this.Remover = PropertyEntity({ eventClick: removerCertificadoClick, type: types.event, text: "Remover", visible: ko.observable(false) });
    this.Baixar = PropertyEntity({ eventClick: baixarCertificadoGestaoClick, type: types.event, text: "Baixar", visible: ko.observable(false) });
};

function baixarCertificadoGestaoClick() {
    executarDownload("Transportador/DownloadCertificado", { CodigoTransportador: _transportadorGestao.Codigo.val() });
}

var SeguroObj = function () {
    this.Transportador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.UsarTipoOperacaoApolice = PropertyEntity({ getType: typesKnockout.bool, text: "Usar Apólices para Tipo de Operação", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarTipoOperacaoApolice.val.subscribe(function (usar) {
        _transportadorGestao.UsarTipoOperacaoApolice.val(usar);
    });
    this.Apolice = PropertyEntity({ text: "Apólice:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ text:"Tipo de Operação:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Apolices = PropertyEntity({ type: types.local, text: "Apólices", idGrid: guid() });
    this.Adicionar = PropertyEntity({ eventClick: adicionarSeguroClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}
function LoadSeguro() {
    _seguro = new SeguroObj();
    KoBindings(_seguro, "knockoutSeguros");
    new BuscarApolicesSeguroMultitransportador(_seguro.Apolice, null, null, null, apoliceSeguroChange);
    new BuscarTiposOperacao(_seguro.TipoOperacao);

    //-- Grid Seguros
    // Opcoes
    var remover = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: removerSeguroClick, icone: "" };

    // Menu
    var menuOpcoes = { tipo: TypeOptionMenu.link, descricao: "Opções", tamanho: 1, opcoes: [remover] };

    // Grid
    var linhasPorPaginas = 7;
    _seguro.Transportador.val(_transportadorGestao.Codigo.val());
    _seguro.UsarTipoOperacaoApolice.val(_transportadorGestao.UsarTipoOperacaoApolice.val());
    _gridSeguro = new GridView(_seguro.Apolices.idGrid, "Transportador/PesquisaSeguros", _seguro, menuOpcoes, null, linhasPorPaginas);
    _gridSeguro.CarregarGrid();
}

function apoliceSeguroChange(apolice) {
    _seguro.Apolice.val(apolice.NumeroApolice + ' - ' + apolice.Seguradora);
    _seguro.Apolice.codEntity(apolice.Codigo);
}

function removerSeguroClick(dataRow, row) {
    executarReST("Transportador/ExcluirSeguroPorCodigo", dataRow, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                _gridSeguro.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}
function adicionarSeguroClick(e, sender) {
    Salvar(_seguro, "Transportador/AdicionarSeguro", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                _gridSeguro.CarregarGrid();
                limparCamposSeguro();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}
var CTeMDFeObj = function () {
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.SerieIntraestadual = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Série CT-e dentro do Estado:", idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.SerieInterestadual = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Série CT-e fora do Estado:", idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.SerieMDFe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Série MDF-e:", idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.documentoFretes = PropertyEntity({ val: ko.observable(EnumTipoEmissaoIntramunicipal.NaoEspecificado), options: EnumTipoEmissaoIntramunicipal.obterOpcoes(), def: EnumTipoEmissaoIntramunicipal.NaoEspecificado, text: "Documento emitido para fretes municipais:", required: true, visible: ko.observable(true) });
    this.tempoEmissao = PropertyEntity({ text: "Tempo em horas para emitir documentos automaticamente (somente cargas segundo trecho):" });
    this.aliquotaPIS = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal, text: "Alíquota do PIS:", maxlength: 5, visible: ko.observable(true), configDecimal: { precision: 2, allowZero: false, allowNegative: false } });
    this.aliquotaCOFINS = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal, text: "Alíquota do COFINS:", maxlength: 5, visible: ko.observable(true), configDecimal: { precision: 2, allowZero: false, allowNegative: false } });
    this.fraseNFS = PropertyEntity({ text: "Frase NFS-e:" });
    this.senhaPrefeitura = PropertyEntity({ text: "Senha da Prefeitura (NFS-e):" });
    this.quantidadeEmailRPS = PropertyEntity({ text: "Quantidade Máxima Email RPS:", val: ko.observable(0), def: 0, type: types.int, });
    this.aliquotaICMS = PropertyEntity({ text: "Alíquota ICMS Negociada:", val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal });
    this.observacaoCTe = PropertyEntity({ text: "Observação CTe:" });
};


var ConfiguracaoNFEObj = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ idGrid: guid() })
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.AliquotaISS = PropertyEntity({ text: "Aliquota ISS", issue: 770, required: true, getType: typesKnockout.decimal, def: "", enable: ko.observable(true), configDecimal: { precision: 4, allowZero: true, allowNegative: false } });
    this.FraseSecreta = PropertyEntity({ text: "Frase", required: false, maxlength: 200 });
    this.LocalidadePrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Localidade de Prestação do Serviço", issue: 766, idBtnSearch: guid() });
    this.LoginSitePrefeitura = PropertyEntity({ text: "Login no Site da Prefeitura:", issue: 772, required: false, maxlength: 500 });
    this.NaturezaNFSe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Natureza", issue: 769, idBtnSearch: guid() });
    this.RetencaoISS = PropertyEntity({ text: "*Retenção do ISS:", issue: 771, maxlength: 6, required: ko.observable(true), enable: ko.observable(true), getType: typesKnockout.decimal, def: "", configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ClienteTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Cliente Tomador", idBtnSearch: guid() });
    this.GrupoTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Grupo Tomador", idBtnSearch: guid() });
    this.LocalidadeTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Localidade do Tomador" });
    this.UFTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "UF do Tomador", idBtnSearch: guid() });
    this.SenhaSitePrefeitura = PropertyEntity({ text: "Senha do site da Prefeitura", issue: 772, required: false, maxlength: 500 });
    this.SerieNFSe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Serie:", issue: 756, idBtnSearch: guid() });
    this.SerieRPS = PropertyEntity({ text: "Série do RPS:", issue: 767, required: false, maxlength: 10 });
    this.ServicoNFSe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: 'Serviço', issue: 768, idBtnSearch: guid() });
    this.URLPrefeitura = PropertyEntity({ text: "Site da Prefeitura", issue: 772, required: false, maxlength: 500 });
    this.IncluirISSBaseCalculo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Incluuir o valor do ISS na base do cálculo:" });
    this.PermiteAnular = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Permite Anular a NFS-e", issue: 1570 });
    this.PrazoCancelamento = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.int, def: "", text: "*Prazo para Cancelamento (dias):" });
    this.DiscriminacaoNFSe = PropertyEntity({ text: "Discriminação da NFS-e:", required: false, maxlength: 2000 });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Tipo de Operação:", idBtnSearch: guid() });

    this.TagCodigoDestinatario = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#CodigoDestinatario"); }, type: types.event, text: "Codigo Destinatario" });
    this.TagCodigoRemetente = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#CodigoRemetente"); }, type: types.event, text: "Codigo Remetente" });
    this.TagDataCarga = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#DataCarga"); }, type: types.event, text: "Transportador DataCarga" });
    this.TagDestino = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#Destino"); }, type: types.event, text: "Destino" });
    this.TagOrigem = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#Origem"); }, type: types.event, text: "Origem" });
    this.TagNotasFiscais = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#NotasFiscais"); }, type: types.event, text: "Notas Fiscais" });
    this.TagTipoCarga = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#TipoCarga"); }, type: types.event, text: "Tipo Carga" });
    this.TagPesoCarga = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#PesoCarga"); }, type: types.event, text: "Peso Carga" });
    this.TagCnpjRemetente = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#CnpjRemetente"); }, type: types.event, text: "CNPJ Remetente" });
    this.TagNomeRemetente = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#NomeRemetente"); }, type: types.event, text: "Nome Remetente" });
    this.TagCnpjDestinatario = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#CnpjDestinatario"); }, type: types.event, text: "CNPJ Destinatario" });
    this.TagNomeDestinatario = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#NomeDestinatario"); }, type: types.event, text: "Nome Destinatario" });
    this.TagValorMercadoria = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#ValorMercadoria"); }, type: types.event, text: "Valor Mercadoria" });
    this.TagPlacaVeiculo = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#PlacaVeiculo"); }, type: types.event, text: "Placa Veiculo" });
    this.TagNomeMotorista = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#NomeMotorista"); }, type: types.event, text: "Nome Motorista" });
    this.TagNumeroCarga = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#NumeroCarga"); }, type: types.event, text: "Numero Carga" });
    this.TagNumeroPedidoEmbarcador = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#NumeroPedidoEmbarcador"); }, type: types.event, text: "Numero Pedido Embarcador" });
    this.TagCPFMotorista = PropertyEntity({ eventClick: function () { InserirTag(self.DiscriminacaoNFSe.id, "#CPFMotorista"); }, type: types.event, text: "CPF Motorista" });

    this.Adicionar = PropertyEntity({ eventClick: adicionarGestaoConfiguracaoNFSeClick, type: "Adicionar", visible: ko.observable(true), text: "Adicionar" });
};

var CadastroSerieObj = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local, idGrid: guid() });
    this.Numero = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: "*Serie", def: "", getType: typesKnockout.int, maxlength: 3, required: true });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoSerie.CTe), enable: ko.observable(true), text: "*Tipo", options: EnumTipoSerie.obterOpcoes(), def: EnumTipoSerie.CTe, required: true });
    this.Status = PropertyEntity({ val: ko.observable("A"), text: "*Status", options: _statusSerie, def: "A", required: true });
    this.ProximoNumeroDocumento = PropertyEntity({ val: ko.observable("1"), enable: ko.observable(true), text: "ProximoNumero Documento", def: "1", getType: typesKnockout.int, maxlength: 10, required: true, visible: ko.observable(false) });
    this.NaoGerarCargaAutomaticamente = PropertyEntity({ val: ko.observable(false), def: false, text: "Nao Gerar Carga Automaticamente", getType: typesKnockout.bool, visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarItemGridSerie, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarSerieGestaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarSerieClickGestao, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

function adicionarGestaoConfiguracaoNFSeClick(e, sender) {
    Salvar(e, "TransportadorConfiguracaoNFSe/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado");
                _gridTransportadorConfiguracaoNFSe.CarregarGrid();
                limparCamposTransportadorGestaoConfiguracaoNFSe();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarGestaoConfiguracaoNFSeClick(e, sender) {
    Salvar(e, "TransportadorConfiguracaoNFSe/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                _gridTransportadorConfiguracaoNFSe.CarregarGrid();
                limparCamposTransportadorGestaoConfiguracaoNFSe();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function excluirGestaoConfiguracaoNFSeClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "RealmenteDesejaExcluirConfiguracaoNFSe", function () {
        ExcluirPorCodigo(_transportadorConfiguracaoNFSe, "TransportadorConfiguracaoNFSe/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridTransportadorConfiguracaoNFSe.CarregarGrid();
                    limparCamposTransportadorGestaoConfiguracaoNFSe();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}


function limparCamposTransportadorGestaoConfiguracaoNFSe() {
    _configuracaoNFE.Atualizar.visible(false);
    _configuracaoNFE.Cancelar.visible(false);
    _configuracaoNFE.Excluir.visible(false);
    _configuracaoNFE.Adicionar.visible(true);
    LimparCampos(_configuracaoNFE);
    _configuracaoNFE.Empresa.val(_transportadorGestao.Codigo.val());
    _configuracaoNFE.Empresa.codEntity(_transportadorGestao.Codigo.val());
    _pesquisaconfiguracaoNFE.Empresa.val(_transportadorGestao.Codigo.val());
    _pesquisaconfiguracaoNFE.Empresa.codEntity(_transportadorGestao.Codigo.val());
}


var PesquisaConfiguracaoNFSe = function () {
    this.Empresa = PropertyEntity({ val: ko.observable(0), def: 0, type: types.entity, codEntity: ko.observable(0) });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Serviço", issue: 768, idBtnSearch: guid() });
    this.LocalidadePrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Localidade de Prestação", issue: 766, idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Tipo de Operação", idBtnSearch: guid() });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridConfiguracaoNFSes, type: types.event, text: "Pesquisar", visible: ko.observable(true) });
}


function loadGestaoTransportador() {

    _crudTransportador = new CRUDTransportador();
    KoBindings(_crudTransportador, "knockoutCRUDTransportador");

    _transportadorGestao = new Transportador();
    KoBindings(_transportadorGestao, "knockoutCadastroTransportador");

    _certificado = new CertificadoObj();
    KoBindings(_certificado, "knockoutCertificado");

    _CteMdfe = new CTeMDFeObj();
    KoBindings(_CteMdfe, "knockoutCtemdfe");

    _pesquisaconfiguracaoNFE = new PesquisaConfiguracaoNFSe();
    KoBindings(_pesquisaconfiguracaoNFE, "knockoutPesquisaConfiguracaoNFE");

    _configuracaoNFE = new ConfiguracaoNFEObj();
    KoBindings(_configuracaoNFE, "knoutConfiguracaoNFSe");

    LoadGridSeries();

    BuscarSeriesCTeTransportador(_CteMdfe.SerieIntraestadual, null, null, null, null, _CteMdfe.Empresa);
    BuscarSeriesCTeTransportador(_CteMdfe.SerieInterestadual, null, null, null, null, _CteMdfe.Empresa);
    BuscarSeriesMDFeTransportador(_CteMdfe.SerieMDFe, null, null, null, null, _CteMdfe.Empresa);

    BuscarLocalidades(_transportadorGestao.Localidade);

    BuscarLocalidades(_configuracaoNFE.LocalidadePrestacao, null, null, ValidaLocalidade);
    BuscarLocalidades(_configuracaoNFE.LocalidadeTomador);
    BuscarNaturezaNFSe(_configuracaoNFE.NaturezaNFSe, _transportadorGestao.Localidade);
    BuscarServicoNFSe(_configuracaoNFE.ServicoNFSe, _transportadorGestao.Localidade);
    BuscarSeriesNFSeTransportador(_configuracaoNFE.SerieNFSe, null, null, null, null, _configuracaoNFE.Empresa);
    BuscarClientes(_configuracaoNFE.ClienteTomador);
    BuscarGruposPessoas(_configuracaoNFE.GrupoTomador);
    BuscarEstados(_configuracaoNFE.UFTomador);
    BuscarTiposOperacao(_configuracaoNFE.TipoOperacao);

    BuscarServicoNFSe(_pesquisaconfiguracaoNFE.Servico, _transportadorGestao.Localidade);
    BuscarLocalidades(_pesquisaconfiguracaoNFE.LocalidadePrestacao);
    BuscarTiposOperacao(_pesquisaconfiguracaoNFE.TipoOperacao);
    buscarTransportadorConfiguracaoGestaoNFSes();

    buscarTransportador();
}

function ValidaLocalidade(dadosRetorno) {
    _configuracaoNFE.LocalidadePrestacao.codEntity(dadosRetorno.Codigo);
    _configuracaoNFE.LocalidadePrestacao.val(dadosRetorno.Descricao);
}


function buscarTransportador() {
    executarReST("GestaoTransportador/BuscarPorEmpresaLogada", null, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                let data = { Data: retorno.Data }
                let dataCertificado = { Data: retorno.Data.Certificado }
                let cte_mdf = { Data: retorno.Data.CTEMDF }
                let configuracaoNFE = { Data: retorno.Data.TransportadorConfiguracaoNFSe }

                PreencherObjetoKnout(_transportadorGestao, data);
                PreencherObjetoKnout(_certificado, dataCertificado);
                PreencherObjetoKnout(_CteMdfe, cte_mdf)
                PreencherObjetoKnout(_configuracaoNFE, configuracaoNFE)

                _CteMdfe.Empresa.val(_transportadorGestao.Codigo.val());
                _CteMdfe.Empresa.codEntity(_transportadorGestao.Codigo.val());
                estadoInicialCertificado();

                recarregarGridSerieGestao();

                LoadSeguro();
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Erro", retorno.Msg);
        }
    });
}

function DescricaoStatus(status) {
    for (let i = 0; i < _statusSerie.length; i++) {
        if (_statusSerie[i].value == status)
            return _status[i].text;
    }
}

function reenviarDadosAcessoaClienteClickGestao(e, sender) {
    let data = { Codigo: _transportadorGestao.Codigo.val() };
    executarReST("Transportador/ReenviarDadosAcessoaCliente", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "EnviadoComSucesso");
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}


function obterTransportadorSalvarGestao() {
    _transportadorGestao.configuracaoNFE.val(JSON.stringify(RetornarObjetoPesquisa(_configuracaoNFE)));
    _transportadorGestao.Certificado.val(JSON.stringify(RetornarObjetoPesquisa(_certificado)));
    _transportadorGestao.CTEMDF.val(JSON.stringify(RetornarObjetoPesquisa(_CteMdfe)));
    _transportadorGestao.Series.val(JSON.stringify(RetornarObjetoPesquisa(_serie)));

    let transportadorGestao = RetornarObjetoPesquisa(_transportadorGestao);
    return transportadorGestao;
}


function atualizarClickGestao() {
    if (!ValidarCamposObrigatorios(_CteMdfe)) {
        exibirMensagemCamposObrigatorioGestao();
        return false;
    }

    executarReST("Transportador/AtualizarTransportadorGestao", obterTransportadorSalvarGestao(), function (retorno) {

        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                LoadGridSeries();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function recarregarGridConfiguracaoNFSes() {
    _gridTransportadorGestaoConfiguracaoNFSe.CarregarGrid();
}

function buscarTransportadorConfiguracaoGestaoNFSes() {
    let editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: () => { }, tamanho: "15", icone: "" };
    let menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTransportadorGestaoConfiguracaoNFSe = new GridView(_configuracaoNFE.Grid.idGrid, "TransportadorConfiguracaoNFSe/Pesquisa", _pesquisaconfiguracaoNFE, menuOpcoes, null);
    _gridTransportadorGestaoConfiguracaoNFSe.CarregarGrid();

}

function exibirMensagemCamposObrigatorioGestao() {
    exibirMensagem(tipoMensagem.atencao, "Aviso","Informe os campos obrigatorios");
}

// SERIE

function recarregarGridSerieGestao() {
    let data = new Array();

    $.each(_transportadorGestao.Series.list, function (i, serie) {
        let serieGrid = new Object();
        serieGrid.Codigo = serie.Codigo.val;
        serieGrid.Status = serie.Status.val;
        serieGrid.Numero = serie.Numero.val;
        serieGrid.ProximoNumeroDocumento = serie.ProximoNumeroDocumento.val;
        serieGrid.DescricaoTipo = EnumTipoSerie.obterDescricao(serie.Tipo.val);
        serieGrid.DescricaoStatus = DescricaoStatus(serie.Status.val);
        serieGrid.Tipo = serie.Tipo.val;
        data.push(serieGrid);
    });

    _gridSerie.CarregarGrid(data);
}

function limparCamposSerieGestao() {
    _serie.Codigo.val(guid());
    _serie.Atualizar.visible(false);
    _serie.Cancelar.visible(false);
    _serie.Adicionar.visible(true);
    LimparCampos(_serie);
    _serie.Numero.enable(true);
    _serie.Tipo.enable(true);
}

function editarSerieClickGestao(data) {
    _serie.Cancelar.visible(true);
    _serie.Atualizar.visible(true);
    _serie.Adicionar.visible(false);

    EditarListEntity(_serie, data);

    _serie.Numero.enable(false);
    _serie.Tipo.enable(false);
}

function LoadGridSeries() {
    _serie = new CadastroSerieObj();

    KoBindings(_serie, "knockoutCadastroSerie");

    let menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarSerieClickGestao }] };
    let header = null;

    header = [
        { data: "Codigo", visible: false },
        { data: "Status", visible: false },
        { data: "Tipo", visible: false },
        { data: "Numero", title: "Número", width: "10%" },
        { data: "DescricaoTipo", title: "Tipo", width: "20%" },
        { data: "DescricaoStatus", title: "Status", width: "35%" },
        { data: "ProximoNumeroDocumento", title: "Próximo Número do Documento", width: "15%" }
    ];

    _gridSerie = new BasicDataTable(_serie.Grid.idGrid, header, menuOpcoes, { column: 3, dir: orderDir.asc });

    recarregarGridSerieGestao();
}

function adicionarItemGridSerie() {
    let tudoCerto = ValidarCamposObrigatorios(_serie);
    if (tudoCerto) {
        let existe = false;
        $.each(_transportadorGestao.Series.list, function (i, serie) {
            if (serie.Numero.val == _serie.Numero.val() && serie.Tipo.val == _serie.Tipo.val()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _transportadorGestao.Series.list.push(SalvarListEntity(_serie));
            recarregarGridSerieGestao();
            $("#" + _serie.Numero.id).focus();
        } else {
            exibirMensagem(tipoMensagem.aviso, "Série Existente", "Série já cadastrada" + _serie.Numero.val());
        }
        limparCamposSerieGestao();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios");
    }
}

function atualizarSerieGestaoClick(e, sender) {
    let tudoCerto = ValidarCamposObrigatorios(_serie);
    if (tudoCerto) {
        $.each(_transportadorGestao.Series.list, function (i, serie) {
            if (serie.Codigo.val == _serie.Codigo.val()) {
                AtualizarListEntity(_serie, serie);
                return false;
            }
        });

        recarregarGridSerieGestao();
        limparCamposSerieGestao();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatorio", "Informe Campos Obrigatórios");
    }
}

function cancelarSerieClickGestao(e) {
    limparCamposSerieGestao();
}

function estadoInicialCertificado() {
    if (!_certificado.PossuiCertificado.val()) {
        _certificado.Arquivo.visible(true);
        _certificado.Senha.visible(true);
        _certificado.Enviar.visible(true);

        _certificado.Baixar.visible(false);
        _certificado.Remover.visible(false);
        _certificado.DataFinal.visible(false);
        _certificado.DataInicial.visible(false);
        _certificado.SerieCertificado.visible(false);
        _certificado.DataFinal.val("");
        _certificado.DataInicial.val("");
        _certificado.SerieCertificado.val("");
    } else {
        _certificado.Baixar.visible(true);
        _certificado.Remover.visible(true);
        _certificado.DataFinal.visible(true);
        _certificado.DataInicial.visible(true);
        _certificado.SerieCertificado.visible(true);

        _certificado.Arquivo.visible(false);
        _certificado.Senha.visible(false);
        _certificado.Enviar.visible(false);
    }
}

function enviarCertificadoClick() {
    let file = document.getElementById(_certificado.Arquivo.id);

    let formData = new FormData();
    formData.append("upload", file.files[0]);

    enviarArquivo("Transportador/EnviarCertificado?callback=?",
        {
            CodigoTransportador: _transportadorGestao.Codigo.val(),
            SenhaCertificado: _certificado.Senha.val(),
            SerieCertificado: _certificado.SerieCertificado.val(),
            DataInicialCertificado: _certificado.DataInicial.val(),
            DataFinalCertificado: _certificado.DataFinal.val()
        }, formData, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Certificado validado e salvo com sucesso.");

                _certificado.DataInicial.val(arg.Data.DataInicialCertificado);
                _certificado.DataFinal.val(arg.Data.DataFinalCertificado);
                _certificado.SerieCertificado.val(arg.Data.SerieCertificado);
                _certificado.Senha.val(arg.Data.SenhaCertificado);

                _certificado.Baixar.visible(true);
                _certificado.Remover.visible(true);
                _certificado.DataFinal.visible(true);
                _certificado.DataInicial.visible(true);
                _certificado.SerieCertificado.visible(true);

                _certificado.Arquivo.val("");
                _certificado.Arquivo.visible(false);
                _certificado.Senha.visible(false);
                _certificado.Enviar.visible(false);

                _certificado.PossuiCertificado.val(true);

            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
}

function removerCertificadoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, "Deseja realmente remover o certificado deste transportador" + "<b>" + "Este processo é irreversível!" + "</b >", function () {
        executarReST("Transportador/RemoverCertificado", { CodigoTransportador: _transportadorGestao.Codigo.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Certificado removido com sucesso.");
                _certificado.Baixar.visible(false);
                _certificado.PossuiCertificado.val(false);
                _certificado.Remover.visible(false);
                _certificado.DataFinal.visible(false);
                _certificado.DataInicial.visible(false);
                _certificado.SerieCertificado.visible(false);
                _certificado.DataFinal.val("");
                _certificado.DataInicial.val("");
                _certificado.SerieCertificado.val("");

                _certificado.Senha.val("");
                _certificado.Arquivo.visible(true);
                _certificado.Senha.visible(true);
                _certificado.Enviar.visible(true);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}
function limparCamposSeguro() {
    _seguro.Apolice.val(_seguro.Apolice.def);
    _seguro.Apolice.codEntity(_seguro.Apolice.defCodEntity);
    _seguro.TipoOperacao.val(_seguro.TipoOperacao.def);
    _seguro.TipoOperacao.codEntity(_seguro.TipoOperacao.defCodEntity);
}

