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
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Atividade.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Endereco.js" />
/// <reference path="../../Consultas/Licenca.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Consultas/MesoRegioa.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoCTeDocumentos.js" />
/// <reference path="../../Enumeradores/EnumTipoLocalizacao.js" />
/// <reference path="../../Enumeradores/EnumModalidadePessoa.js"/>
/// <reference path="../../Enumeradores/EnumPaises.js"/>
/// <reference path="../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../Configuracao/Fatura/Fatura.js" />
/// <reference path="ListaEmail.js" />
/// <reference path="Subarea.js" />
/// <reference path="SubareaAcoesFluxoDePatio.js" />
/// <reference path="ListaEndereco.js" />
/// <reference path="Documento.js" />
/// <reference path="DadoBancario.js" />
/// <reference path="Emissao.js" />
/// <reference path="GeoLocalizacaoGoogleMaps.js" />
/// <reference path="GeolocalizacaoMapRequest.js" />
/// <reference path="Descarga.js" />
/// <reference path="Componente.js" />
/// <reference path="PessoaAdicional.js" />
/// <reference path="ImportacaoNFe.js" />
/// <reference path="PessoaAnexo.js" />
/// <reference path="CamposObrigatorios.js" />
/// <reference path="AreaRedex.js" />
/// <reference path="PortalAcessoGrupoPessoas.js" />
/// <reference path="Comprovante.js" />
/// <reference path="Integracoes.js" />
/// <reference path="FiliaisCliente.js" />
/// <reference path="UsuarioAdicional.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _gridPessoa, _pessoa, _pessoaBotoes, _pesquisaPessoa, _pesquisaEndereco, _pesquisaDadosReceita, _configuracaoEmissaoCTe, _configuracaoLayoutEDI, _configuracaoFatura;
var _PermissoesPersonalizadas;


var PesquisaPessoa = function () {
    this.Nome = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Nome.getFieldDescription() });
    this.NomeFantasia = PropertyEntity({ text: Localization.Resources.Gerais.Geral.NomeFantasia.getFieldDescription() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), visible: ko.observable(true) });
    this.CNPJ_CPF = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.CNPJCPF.getFieldDescription(), getType: _CONFIGURACAO_TMS.Pais == EnumPaises.Brasil ? typesKnockout.cpfCnpj : typesKnockout.string, cssClass: ko.observable("col col-xs-12 col-sm-4 col-md-2 col-lg-2") });
    this.LocalidadePrincipal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Cidade.getFieldDescription(), idBtnSearch: guid() });
    this.Estado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Estado.getFieldDescription(), idBtnSearch: guid() });
    this.Pais = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Pais.getFieldDescription(), idBtnSearch: guid() });
    this.TelefonePrincipal = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Telefone.getFieldDescription(), getType: typesKnockout.phone });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoa.Todas), eventChange: tipoPessoaChange, options: EnumTipoPessoa.obterOpcoesPesquisa(), text: Localization.Resources.Gerais.Geral.Tipo.getRequiredFieldDescription(), def: EnumTipoPessoa.Todas });
    this.SomenteSemValorDescarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.ClientesSemValorDeDescarga, issue: 932, def: false, visible: ko.observable(false) });
    this.SomenteSemGeolocalizacao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.SomenteSemGeolocalizacao, def: false, visible: ko.observable(false) });
    this.PossuiExcecaoCheckinFilaH = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.PossuiExcecaoCheckinFilaH, def: false, visible: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoFilaH) });
    this.GeoLocalizacaoTipo = PropertyEntity({ val: ko.observable(EnumGeoLocalizacaoTipo.Todos), options: EnumGeoLocalizacaoTipo.obterOpcoes(), def: true, text: Localization.Resources.Pessoas.Pessoa.TipoGeolocalizacao.getFieldDescription() });
    this.GrupoPessoas = PropertyEntity({ text: Localization.Resources.Gerais.Geral.GrupoPessoas.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Categoria = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Categoria.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoModalidadePessoa = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TipoDePessoa.getFieldDescription(), val: ko.observable(EnumModalidadePessoa.Todas), options: EnumModalidadePessoa.obterOpcoesPesquisa(), def: EnumModalidadePessoa.Todas });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPessoa.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var PesquisaEndereco = function () {
    this.Logradouro = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Endereco.getFieldDescription()), required: false, maxlength: 80, enable: ko.observable(true) });
    this.Bairro = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Bairro.getFieldDescription()), required: false, maxlength: 80, enable: ko.observable(true) });
    this.CEP = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.CEP.getFieldDescription()), required: false });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Gerais.Geral.Cidade.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });
    this.CodigoIBGE = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.CodigoIBGE.getFieldDescription()), required: false });

    this.Pesquisar = PropertyEntity({ eventClick: pesquisarEnderecosClick, type: types.event, text: Localization.Resources.GErais.Geral.Pesquisar, visible: ko.observable(true) });

    this.Enderecos = PropertyEntity({ type: types.local, idGrid: guid() });
};

var PesquisaDadosReceita = function () {

    this.ImagemCaptcha = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.DigiteCodigoDaImagemAoLado.getRequiredFieldDescription(), src: ko.observable(""), maxlength: 6, required: true });
    this.EnviarCatptch = PropertyEntity({ eventClick: enviarCNPJClick, type: types.event, text: Localization.Resources.Pessoas.Pessoa.Consultar, visible: ko.observable(true) });
    this.Captcha = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Captcha.getFieldDescription()), enable: ko.observable(true), maxlength: 6, visible: ko.observable(true) });
    //this.Captcha = PropertyEntity({ text: ko.observable("Captcha: "), enable: ko.observable(true), maxlength: 6, visible: ko.observable(false) });
    this.BuscarNovoCaptcha = PropertyEntity({ eventClick: validarCNPJClick, type: types.event, text: Localization.Resources.Pessoas.Pessoa.Continuar, visible: ko.observable(false) });
};

var PessoaCadastro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoa.Juridica), eventChange: tipoPessoaChange, options: EnumTipoPessoa.obterOpcoes(), text: Localization.Resources.Gerais.Geral.Tipo.getRequiredFieldDescription(), def: EnumTipoPessoa.Juridica, enable: ko.observable(true) });
    this.CNPJ = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.CNPJ.getRequiredFieldDescription()), issue: 4, required: true, getType: _CONFIGURACAO_TMS.Pais == EnumPaises.Brasil ? typesKnockout.cnpj : typesKnockout.string, enable: ko.observable(true), visible: ko.observable(true) });
    this.CPF = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.CPF.getRequiredFieldDescription()), required: false, getType: typesKnockout.cpf, enable: ko.observable(true), visible: ko.observable(false) });
    this.IE_RG = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.IE.getRequiredFieldDescription()), issue: 744, required: true, visible: ko.observable(true), maxlength: 14, enable: ko.observable(true) });
    this.Nome = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.RazaoSocial.getRequiredFieldDescription()), required: true, maxlength: 80, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-12 col-lg-6"), enable: ko.observable(true) });
    this.CodigoIntegracao = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.CodigoIntegracao.getFieldDescription()), issue: 15, required: false, maxlength: 50, visible: ko.observable(true), enable: ko.observable(true) });

    this.Fantasia = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Fantasia.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 150, enable: ko.observable(true) });
    this.NomeVisoesBI = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.NomeVisoesBI.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 150, enable: ko.observable(true) });

    this.TelefonePrincipal = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.TelefonePrincipal.getRequiredFieldDescription()), issue: 27, required: true, getType: typesKnockout.phone, cssClass: ko.observable("col col-xs-12 col-sm-6 col-md-3 col-lg-2"), enable: ko.observable(true) });
    this.TelefoneSecundario = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TelefoneSecundario.getFieldDescription(), issue: 27, required: false, getType: typesKnockout.phone, cssClass: ko.observable("col col-xs-12 col-sm-6 col-md-3 col-lg-2"), enable: ko.observable(true) });
    this.Atividade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Atividade.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, issue: 47, enable: ko.observable(true) });
    this.Endereco = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.EnderecoPrincipal.getRequiredFieldDescription()), required: true, maxlength: 120, enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-12 col-lg-5") });
    this.Numero = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Numero.getFieldDescription()), required: true, maxlength: 60, enable: ko.observable(true) });
    this.Bairro = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Bairro.getRequiredFieldDescription()), required: true, maxlength: 40, enable: ko.observable(true) });
    this.Complemento = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Complemento.getFieldDescription()), required: false, maxlength: 60, enable: ko.observable(true) });
    this.CEP = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.CEP.getRequiredFieldDescription()), required: true, enable: ko.observable(true) });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: ko.observable(Localization.Resources.Gerais.Geral.Cidade.getRequiredFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });
    this.RG = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.RG.getFieldDescription()), required: false, visible: ko.observable(false), maxlength: 50, enable: ko.observable(true) });
    this.Passaporte = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Passaporte.getFieldDescription()), required: false, visible: ko.observable(false), maxlength: 20, enable: ko.observable(true) });
    this.NumeroCUITRUT = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.NumeroCuitRuit.getFieldDescription()), required: false, visible: ko.observable(false), maxlength: 150, enable: ko.observable(true) });

    this.Email = PropertyEntity({ text: ko.observable((_CONFIGURACAO_TMS.ExigirEmailPrincipalCadastroPessoa ? "*" : "") + Localization.Resources.Pessoas.Pessoa.EmailPrincipal.getFieldDescription()), issue: 30, required: ko.observable(_CONFIGURACAO_TMS.ExigirEmailPrincipalCadastroPessoa), getType: typesKnockout.multiplesEmails, maxlength: 1000, enable: ko.observable(true) });
    this.EnviarEmail = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: Localization.Resources.Pessoas.Pessoa.XML, enable: ko.observable(true) });
    this.EmailInterno = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.EmailExclusivoPorEmpresa.getFieldDescription()), issue: 2707, required: false, getType: typesKnockout.multiplesEmails, maxlength: 1000, visible: ko.observable(false), enable: ko.observable(true) });
    this.EnviarEmailInterno = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Pessoas.Pessoa.XML, visible: ko.observable(false), enable: ko.observable(true) });

    this.Observacao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Observacao.getFieldDescription(), issue: 593, required: false, maxlength: 500, enable: ko.observable(true) });
    this.TipoLogradouro = PropertyEntity({ val: ko.observable(EnumTipoLogradouro.Rua), options: EnumTipoLogradouro.obterOpcoes(), def: EnumTipoLogradouro.Rua, text: Localization.Resources.Pessoas.Pessoa.TipoLogradouro.getFieldDescription(), issue: 18, required: true, enable: ko.observable(true) });
    this.TipoEndereco = PropertyEntity({ val: ko.observable(EnumTipoEndereco.Comercial), options: EnumTipoEndereco.obterOpcoes(), def: EnumTipoEndereco.Comercial, text: Localization.Resources.Pessoas.Pessoa.TipoEndereco.getFieldDescription(), issue: 17, required: true, enable: ko.observable(true) });
    this.TipoEmail = PropertyEntity({ val: ko.observable(EnumTipoEmail.Principal), options: EnumTipoEmail.obterOpcoes(), def: EnumTipoEmail.Principal, text: Localization.Resources.Pessoas.Pessoa.TipoEmail.getFieldDescription(), issue: 29, required: true, enable: ko.observable(true) });

    this.Categoria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: (_CONFIGURACAO_TMS.ExigirCategoriaCadastroPessoa ? "*" : "") + Localization.Resources.Pessoas.Pessoa.Categoria.getFieldDescription(), visible: ko.observable(true), idBtnSearch: guid(), required: _CONFIGURACAO_TMS.ExigirCategoriaCadastroPessoa, enable: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.GrupoDePessoas.getFieldDescription(), issue: 58, visible: ko.observable(true), idBtnSearch: guid(), enable: ko.observable(true), eventChange: grupopessoasBlur });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false, enable: ko.observable(true) });
    this.Latitude = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Latitude.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 20, enable: ko.observable(true) });
    this.Longitude = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Longitude.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 20, enable: ko.observable(true) });
    this.AtualizarPontoApoioMaisProximoAutomaticamente = PropertyEntity({ val: ko.observable(false), def: ko.observable(false), getType: typesKnockout.bool });
    this.PontoDeApoio = PropertyEntity({ val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });

    this.GeoLocalizacaoRaioLocalidade = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.RaioGeoLocalizacaoLocalidade = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.decimal, text: Localization.Resources.Pessoas.Pessoa.RaioGeolocalizacaoLocalidade, enable: false });

    this.BuscarLatitudeLongitude = PropertyEntity({ eventClick: BuscarLatitudeLongitude, type: types.event, text: Localization.Resources.Pessoas.Pessoa.BuscarLatitudeLongitude, visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoArea = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TipoArea.getFieldDescription(), val: ko.observable(true), options: EnumTipoArea.obterOpcoes(), def: EnumTipoArea.Raio, enable: ko.observable(true) });
    this.TipoArea.val.subscribe(function () { setarTipoArea(); });
    this.Area = PropertyEntity();
    this.RaioEmMetros = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.RaioMetros), required: false, visible: ko.observable(true), maxlength: 10, enable: ko.observable(true) });
    this.RaioEmMetros.val.subscribe(function () { setarRaioEmMetros(); });
    this.AlvoEstrategico = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.AlvoEstrategico, visible: ko.observable(true), enable: ko.observable(true) });



    this.LatitudeTransbordo = PropertyEntity({ text: ko.observable(" "), required: false, visible: ko.observable(false), maxlength: 20, enable: ko.observable(true) });
    this.LongitudeTransbordo = PropertyEntity({ text: ko.observable(" "), required: false, visible: ko.observable(false), maxlength: 20, enable: ko.observable(true) });

    this.UsuarioPortal = PropertyEntity({ text: ko.observable(""), required: false, visible: ko.observable(false), maxlength: 200, enable: ko.observable(false) });
    this.SenhaUsuarioPortal = PropertyEntity({ text: ko.observable(""), required: false, visible: ko.observable(false), maxlength: 200, enable: ko.observable(false) });

    this.TipoLocalizacao = PropertyEntity({ val: ko.observable(EnumTipoLocalizacao.naoEncontrado), required: false, visible: ko.observable(false), def: EnumTipoLocalizacao.naoEncontrado, enable: ko.observable(true) });

    this.IE_ISENTO = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.Isento, def: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.SN_Numero = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.SNNumero, def: ko.observable(false), enable: ko.observable(true) });

    this.EnderecoDigitado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.DigitarEndereco, def: ko.observable(false), enable: ko.observable(true) });
    this.ConsultarCEP = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pessoas.Pessoa.NaoSeiCEP.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });

    this.TipoCliente = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.Cliente, def: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoFornecedor = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.Fornecedor, def: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoTransportador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.TransportadorTerceiro, def: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.Fornecedor = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.AcessoPortal = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.UsuarioTerceiro = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.TransportadorTerceiro = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.DadosBancarios = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.ListaEmail = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaSubarea = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });
    this.SuprimentoGas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });
    this.ListaEndereco = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaDocumento = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.DadosAdicionais = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.AreasRedex = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() })
    this.TipoOperacaoRedespacho = PropertyEntity({ val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.FrequenciasCarregamento = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() })

    this.ConfiguracaoEmissaoCTe = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "" });
    this.ConfiguracaoLayoutEDI = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "" });
    this.ConfiguracaoFatura = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "" });

    this.ValidarCNPJ = PropertyEntity({ eventClick: validarCNPJClick, type: types.event, text: Localization.Resources.Gerais.Geral.Validar, enable: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Descarga = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic });
    this.Restricoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });

    this.IndicadorIE = PropertyEntity({ val: ko.observable(EnumIndicadorIE.UmContribuinteICMSInformarIEDoDestinatario), options: EnumIndicadorIE.obterOpcoes(), def: EnumIndicadorIE.UmContribuinteICMSInformarIEDoDestinatario, text: Localization.Resources.Pessoas.Pessoa.IndicadorDeIE.getFieldDescription(), issue: 931, required: ko.observable(true), enable: ko.observable(true) });
    this.InscricaoSuframa = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.IESuframa.getFieldDescription()), issue: 742, required: false, maxlength: 100, enable: ko.observable(true) });
    this.InscricaoMunicipal = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.InscricaoMunicipal.getFieldDescription(), issue: 750, required: false, maxlength: 100, enable: ko.observable(true) });
    this.Pais = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Pais.getFieldDescription(), idBtnSearch: guid() });

    this.NaoUsarConfiguracaoEmissaoGrupo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.NaoUsarConfiguracaoFaturaGrupo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.ExigirComprovantesLiberacaoPagamentoContratoFrete = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription(), issue: 557, enable: ko.observable(true), visible: ko.observable(true) });
    this.RegimeTributario = PropertyEntity({ val: ko.observable(EnumRegimeTributario.NaoSelecionado), options: EnumRegimeTributario.obterOpcoesNaoSelecionado(), def: EnumRegimeTributario.NaoSelecionado, text: Localization.Resources.Pessoas.Pessoa.RegimeTributario.getFieldDescription(), required: false, enable: ko.observable(true) });

    this.ListaContatos = PropertyEntity({ val: ko.observable(""), def: "" });
    this.ListaUsuariosAdicionais = PropertyEntity({ val: ko.observable(""), def: "" });
    this.Contatos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.UsuariosAdicionais = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.ListaOutrasDescricoesPessoaExterior = PropertyEntity({ val: ko.observable(""), def: "" });
    this.OutrasDescricoesPessoaExterior = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.ListaLicencas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
    this.ListaDadosArmador = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
    this.ListaVendedores = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
    this.ListaRecebedoresAutorizados = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
    this.ListaComponentes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
    this.Rota = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.RestricoesFilaCarregamento = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });
    this.FilialCliente = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Comprovantes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Integracoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.ObservacoesCTes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.GrupoPessoasAcessoPortal = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.NaoEmitirCTeFilialEmissora = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.NaoEmitirCTeFilialEmissora), val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });
    this.DigitalizaCanhoto = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.DigitalizaCanhoto, def: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.VerificarUnidadeNegocioPorDestinatario = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.VerificarAnidadeDeNegocioParaExportacaoContabilPeloDestinatario, def: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.ExcecaoCheckinFilaH = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.ExcecaoCheckinFilaH, def: false, visible: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoFilaH), enable: ko.observable(true) });
    this.NaoAtualizarDados = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.NaoAtualizarOsDadosDestaPessoaEmImportacoesDeNotasFiscaisOuIntegracoes, issue: 0, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.Funcionario = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ReplicarEsteCadastroComoFuncionario, issue: 0, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.Motorista = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.AlterarComoMotorista, issue: 0, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.PossuiRestricaoTrafego = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.PossuiRestricaoDeTrafego, issue: 1458, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.PontoTransbordo = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.LocalDeTransbordo, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador), enable: ko.observable(true) });
    this.AguardandoConferenciaInformacao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.AguardandoConferenciaDeInformacao, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.ObrigarInformarDataEntregaClienteAoBaixarCanhotos = PropertyEntity({ text: "Obrigatório informar data entrega no cliente ao baixar Canhotos", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.OutrosCodigosIntegracao = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.Bloqueado = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Bloquear, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.MotivoBloqueio = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.MotivoDoBloqueio, enable: ko.observable(true) });

    this.HabilitarSolicitacaoGas = PropertyEntity({ getType: typesKnockout.bool, def: false, val: ko.observable(false) });

    this.AcrescimoDescontoAutomatico = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Funcionario.val.subscribe(function (novoValor) {
        if (novoValor)
            _pessoa.Motorista.visible(true);
        else
            _pessoa.Motorista.visible(false);
    });

    this.DataFixaVencimento = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
};

var PessoaEmissao = function () {
    this.NaoUsarConfiguracaoEmissaoGrupo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.NaoUsarAsConfiguracoesDeEmissaoDoGrupoDePessoas, def: false, visible: ko.observable(true) });
};

var PessoaFatura = function () {
    this.NaoUsarConfiguracaoFaturaGrupo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.NaoUsarAsConfiguracoesDeFaturasDoGrupoDePessoas, def: false, visible: ko.observable(true) });
};



var PessoaBotoes = function () {
    this.Bloquear = PropertyEntity({ eventClick: BloquearClick, type: types.event, text: Localization.Resources.Pessoas.Pessoa.Bloquear, visible: ko.observable(false), enable: ko.observable(true) });
    this.Desbloquear = PropertyEntity({ eventClick: DesbloquearClick, type: types.event, text: Localization.Resources.Pessoas.Pessoa.Desbloquear, visible: ko.observable(false), enable: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });

    this.ImportarClienteParcial = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Pessoas.Pessoa.ImportarParcial,
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default me-2",
        visible: ko.observable(_CONFIGURACAO_TMS.UsuarioMultisoftware),
        UrlImportacao: "Pessoa/ImportarPessoaParcial",
        UrlConfiguracao: "Pessoa/ConfiguracaoImportacaoPessoaParcial",
        CodigoControleImportacao: EnumCodigoControleImportacao.O009_Pessoa,
        CallbackImportacao: function () {
            _gridPessoa.CarregarGrid();
        }
    });

    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Gerais.Geral.Importar,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default me-2",

        UrlImportacao: "Pessoa/Importar",
        UrlConfiguracao: "Pessoa/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O009_Pessoa,
        CallbackImportacao: function () {
            _gridPessoa.CarregarGrid();
        }
    });


    //BOTÕES PARA TESTAR IMPORTAÇÃO DE VENDEDORES DANONE
    this.ImportarVendedor = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Pessoas.Pessoa.ImportarVendedor,
        visible: ko.observable(_CONFIGURACAO_TMS.AtivarBotaoImportacao),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default me-2",

        UrlImportacao: "Pessoa/ImportarVendedores",
        UrlConfiguracao: "Pessoa/ConfiguracaoImportacaoVendedores",
        CodigoControleImportacao: EnumCodigoControleImportacao.O009_Pessoa,
        CallbackImportacao: function () {
            _gridPessoa.CarregarGrid();
        }
    });

    this.ImportarSupervisor = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Pessoas.Pessoa.ImportarSupervisores,
        visible: ko.observable(_CONFIGURACAO_TMS.AtivarBotaoImportacao),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default me-2",
        UrlImportacao: "Pessoa/ImportarSupervisores",
        UrlConfiguracao: "Pessoa/ConfiguracaoImportacaoVendedores",
        CodigoControleImportacao: EnumCodigoControleImportacao.O009_Pessoa,
        CallbackImportacao: function () {
            _gridPessoa.CarregarGrid();
        }
    });

    this.ImportarHierarquia = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Pessoas.Pessoa.ImportarHierarquia,
        visible: ko.observable(_CONFIGURACAO_TMS.AtivarBotaoImportacao),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default me-2",
        UrlImportacao: "Pessoa/ImportarHierarquia",
        UrlConfiguracao: "Pessoa/ConfiguracaoImportacaoVendedores",
        CodigoControleImportacao: EnumCodigoControleImportacao.O009_Pessoa,
        CallbackImportacao: function () {
            _gridPessoa.CarregarGrid();
        }
    });

    this.ImportarFrequenciaCarregamento = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Pessoas.Pessoa.ImportarFrequenciaDeCarregamento,
        visible: ko.observable(false),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default me-2",
        UrlImportacao: "Pessoa/ImportarFrequenciaCarregamento",
        UrlConfiguracao: "Pessoa/ConfiguracaoImportacaoFrequenciasCarregamento",
        CodigoControleImportacao: EnumCodigoControleImportacao.O009_Pessoa,
        CallbackImportacao: function () { }
    });
};

//*******EVENTOS*******

var _pessoaEmissao;
var _pessoaFatura;

function loadPessoa() {
    if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Pessoa_PermiteAlterarConfiguracaoEmissao, _PermissoesPersonalizadas))
        $("#liTabConfiguracaoFatura").show();

    if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Pessoa_PermiteAlterarConfiguracaoFatura, _PermissoesPersonalizadas))
        $("#liEmissao").show();

    _pessoa = new PessoaCadastro();
    KoBindings(_pessoa, "knockoutCadastroPessoa");

    _pessoaBotoes = new PessoaBotoes();
    KoBindings(_pessoaBotoes, "knockoutCadastroPessoaBotoes");

    _pessoaEmissao = new PessoaEmissao();
    KoBindings(_pessoaEmissao, "knockoutEmissao");

    _pessoaFatura = new PessoaFatura();
    KoBindings(_pessoaFatura, "knockoutConfiguracaoFatura");

    HeaderAuditoria(Localization.Resources.Pessoas.Pessoa.Cliente, _pessoa);

    $("#" + _pessoa.CEP.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });
    $("#liTransportadorTerceiro").hide();
    $("#liUsuarioTerceiro").hide();
    $("#liFornecedor").hide();
    $("#liDocumento").hide();

    _pesquisaPessoa = new PesquisaPessoa();
    KoBindings(_pesquisaPessoa, "knockoutPesquisaPessoa", false, _pesquisaPessoa.Pesquisar.id);

    $("#" + _pessoa.TipoCliente.id).click(verificarTipoPessoa);
    $("#" + _pessoa.TipoFornecedor.id).click(verificarTipoPessoa);
    $("#" + _pessoa.TipoTransportador.id).click(verificarTipoPessoa);

    $("#" + _pessoa.SN_Numero.id).click(verificarSNNumero);
    $("#" + _pessoa.IE_ISENTO.id).click(verificarIEISENTO);

    _pessoa.IE_RG.val("");
    _pessoa.IE_RG.enable(true);

    _pessoa.Numero.val("S/N");
    _pessoa.Numero.enable(false);

    $("#" + _pessoa.EnderecoDigitado.id).click(digitarEndereco);

    new BuscarLocalidades(_pesquisaPessoa.LocalidadePrincipal);
    new BuscarEstados(_pesquisaPessoa.Estado);
    new BuscarPaises(_pesquisaPessoa.Pais);
    new BuscarGruposPessoas(_pesquisaPessoa.GrupoPessoas);
    new BuscarCategoriaPessoa(_pesquisaPessoa.Categoria);

    new BuscarAtividades(_pessoa.Atividade);
    new BuscarLocalidades(_pessoa.Localidade);
    new BuscarEnderecos(_pessoa.ConsultarCEP, null, null, selecionarEnderecoClick);
    new BuscarGruposPessoas(_pessoa.GrupoPessoas, retornoGrupoPessoa);
    new BuscarCategoriaPessoa(_pessoa.Categoria);
    new BuscarPaises(_pessoa.Pais);

    loadListaEmail();
    loadListaSubarea();
    loadListaEndereco();
    loadListaDocumento();
    loadDadoBancario();
    LoadContatoPessoa();
    loadTransportadorTerceiro();
    loadPessoaLicenca();
    loadPessoaArmador();
    loadPessoaVendedor();
    loadPessoaRecebedorAutorizado();
    LoadRota();
    loadPessoaComponente();
    loadUsuarioTerceiro();
    loadRestricaoFilaCarregamento();
    loadPessoaAdicional();
    LoadObservacaoCTe();
    LoadConfiguracaoGeralCTe();
    LoadImportacaoNFe();
    loadAnexo();
    LoadOutraDescricaoPessoaExteriorPessoa();
    LoadBloqueioPessoa();
    loadPortalAcesso();
    loadAreaRedex();
    loadOutrosCodigos();
    loadSuprimentoDeGas();
    loadFrequenciaCarregamento();
    LoadGrupoPessoas();
    loadComprovante();
    loadPessoaIntegracoes();
    loadFilialCliente();
    LoadDataFixaVencimento();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        loadValorDescarga();

        _pessoa.TipoTransportador.visible(true);
        //_pessoa.TipoFornecedor.visible(false);
        _pessoa.TipoCliente.visible(true);
        _pessoa.TipoCliente.val(true);
        _pessoa.PossuiRestricaoTrafego.visible(false);
        _pessoa.PossuiRestricaoTrafego.val(false);
        _pesquisaPessoa.SomenteSemValorDescarga.visible(true);
        _pesquisaPessoa.SomenteSemGeolocalizacao.visible(true);
        _pessoaBotoes.ImportarFrequenciaCarregamento.visible(true);
        $("#liTransportadorTerceiro").hide();
        $("#liTabLayoutsEDI").show();
        $("#liTabRota").show();
        $("#liTabFrequenciaCarregamento").show();

        if (_CONFIGURACAO_TMS.UtilizarComponentesCliente)
            $("#liComponente").show();
    } else {
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
            _pessoa.Ativo.enable(false);
            _pessoa.TipoCliente.visible(false);
            _pessoa.TipoCliente.val(true);
            _pessoa.TipoTransportador.val(true);
            _pessoa.GrupoPessoas.visible(false);
            _pessoa.PossuiRestricaoTrafego.visible(false);
            desabilitarOpcoesPercentuais();
            $("#liTabListaEndereco").hide();
            $("#liTabListaEmail").hide();
            $("#liTabDadosBancarios").hide();
            $("#liTabOutrosCodigos").hide();
        } else {
            $("#liTabLayoutsEDI").show();
        }
    }

    VisualizacaoPessoaPorTipoServico();

    if (_CONFIGURACAO_TMS.Pais == EnumPaises.Exterior) {
        _pessoa.IndicadorIE.required(false);
    }

    if (_CONFIGURACAO_TMS.UtilizarFilaCarregamento)
        $("#liTabRestricaoFilaCarregamento").show();

    if (_CONFIGURACAO_TMS.UtilizaSuprimentoDeGas)
        $("#liTabSuprimentoDeGas").show();

    //if (_CONFIGURACAO_TMS.ValidarCamposReferenteCIOT) {
    //    _pessoa.TelefonePrincipal.text("*" + _pessoa.TelefonePrincipal.text());
    //    _pessoa.TelefonePrincipal.required = true;
    //}

    buscarPessoas();
    desabilitaCamposEndereco();
    carregarConteudosHTML();

    executarReST("Integracao/ObterIntegracoesConfiguradas", {}, function (r) {
        if (r.Success && r.Data) {
            if (r.Data.TiposExistentes != null && r.Data.TiposExistentes.length > 0) {
                //todo: tratar para carregar o mapa apenas nas integrações corretas.
                if (r.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.AngelLira; })) {
                    $("#liGeoLocalizacao").show();
                }
            }
        }
    });

    if (_CONFIGURACAO_TMS.PermiteAdicionarNotaManualmente) {
        $("#liGeoLocalizacao").show();
    }
    //$("#LinkGeolocalizacao").on("click", CarregarMapa);

    loadGeolocalizacao();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFeAdmin) {
        $("#liTransportadorTerceiro").hide();
        $("#liUsuarioTerceiro").hide();
        $("#liFornecedor").hide();
        $("#liDescarga").hide();
        $("#liGeoLocalizacao").hide();
        $("#liEmissao").hide();
        $("#liTabConfiguracaoFatura").hide();
        $("#liTabLayoutsEDI").hide();
        $("#liTabObservacaoCTe").hide();
        _pessoa.TipoTransportador.visible(true);
        $("#liTabContatos").show();
        _pessoa.EmailInterno.visible(true);
        _pessoa.EnviarEmailInterno.visible(true);
        _pessoa.NaoAtualizarDados.visible(false);
        _pessoa.Funcionario.visible(false);
        _pessoa.Motorista.visible(false);
        _pessoa.DigitalizaCanhoto.visible(false);
        _pessoa.Ativo.visible(false);
        _pesquisaPessoa.Ativo.visible(false);
        _pesquisaPessoa.CNPJ_CPF.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-4");
        ValidarPermissaoPersonalizada();
    } else {
        _configuracaoEmissaoCTe = new ConfiguracaoEmissaoCTe("divConfiguracaoEmissaoCTe", _pessoa.ConfiguracaoEmissaoCTe, _pessoa.Pessoa, _pessoa.GrupoPessoas, null, function () {
            verificarTipoPessoa();
        });
        _configuracaoLayoutEDI = new ConfiguracaoLayoutEDI("divConfiguracaoLayoutEDI", _pessoa.ConfiguracaoLayoutEDI);
        _configuracaoFatura = new ConfiguracaoFatura("divConfiguracaoFatura", _pessoa.ConfiguracaoFatura, function () {
            _configuracaoFatura.Configuracao.Banco.visible(false);
            _configuracaoFatura.Configuracao.Agencia.visible(false);
            _configuracaoFatura.Configuracao.Digito.visible(false);
            _configuracaoFatura.Configuracao.NumeroConta.visible(false);
            _configuracaoFatura.Configuracao.TipoConta.visible(false);


            ValidarPermissaoPersonalizada();
        });
    }

    habilitarAbaDadosArmador();
}

function VisualizacaoPessoaPorTipoServico() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        // Esconde todos campos
        for (var i in _fornecedor) {
            if ($.isFunction(_fornecedor[i].visible)) _fornecedor[i].visible(false);
        }

        // Exibe somente campos para usuário        
        _portalAcesso.AtivarAcessoPortal.visible(true);
        _portalAcesso.Usuario.visible(true);
        _portalAcesso.Senha.visible(true);
        _portalAcesso.ConfirmaSenha.visible(true);
        _pessoa.NaoEmitirCTeFilialEmissora.visible(true);

        $("#liAcessoPortal").show();
        $("#liTabRecebedorAutorizado").show();

    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        $("#liTabContatos").show();
        $("#liTransportadorTerceiro").hide();
        $("#liTabObservacaoNFe").show();
        $("#liAcessoPortal").show();//deve deixar pois o tms usa o portal para resposta de cotação
        $("#liTabComprovante").show();
        _pessoa.PossuiRestricaoTrafego.visible(true);
        _pessoa.AguardandoConferenciaInformacao.visible(true);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe) {
        loadFornecedor();
    }
}

function retornoGrupoPessoa(data) {
    _pessoa.GrupoPessoas.codEntity(data.Codigo);
    _pessoa.GrupoPessoas.val(data.Descricao);
    _pessoa.VerificarUnidadeNegocioPorDestinatario.visible(data.DisponibilizarDocumentosParaLoteEscrituracao);
}

function grupopessoasBlur() {
    if (_pessoa.GrupoPessoas.val() == "") {
        _pessoa.GrupoPessoas.codEntity(0);
        _pessoa.VerificarUnidadeNegocioPorDestinatario.visible(false);
    }
}

function abrirModaConsultarCEPClick(e, sender) {
    Global.abrirModal('divModalConsultaEndereco');
}

var cookies;
var chaptcha;

function validarCNPJClick(e) {
    //if ($("#" + _pessoa.CNPJ.id).val().match(/\d/g) != null && $("#" + _pessoa.CNPJ.id).val().match(/\d/g).join("").length == 14) {
    //    if (ValidarCNPJ(_pessoa.CNPJ.val(), _pessoa.CNPJ.required)) {
    //        _pessoa.CNPJ.requiredClass("form-control");
    //        _pesquisaDadosReceita.ImagemCaptcha.val("");
    //        _pesquisaDadosReceita.Captcha.val("");
    //        var data = { CNPJ: _pessoa.CNPJ.val() };
    //        executarReST("Pessoa/RequisicaoConsultaCNPJ", data, function (arg) {
    //            if (arg.Success) {
    //                chaptcha = arg.Data.chaptcha;
    //                cookies = arg.Data.Cookies;
    //                $('#divModalConsultaPessoa').modal({ keyboard: true, backdrop: 'static' });

    //                _pesquisaDadosReceita.ImagemCaptcha.src(chaptcha);
    //                $("#" + _pesquisaDadosReceita.Captcha.id).focus();
    //            } else {
    //                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    //            }
    //        });
    //    } else {
    //        _pessoa.CNPJ.requiredClass("form-control is-invalid");
    //        exibirMensagem(tipoMensagem.atencao, "Atenção", "O número do CNPJ não é válido. Verifique se o mesmo foi digitado corretamente!");
    //    }
    //} else {
    //    exibirMensagem(tipoMensagem.atencao, "Atenção", "Por favor digite um CNPJ válido antes de consultar os dados na receita!");
    //}

    //if ($("#" + _pessoa.CNPJ.id).val().match(/\d/g) != null && $("#" + _pessoa.CNPJ.id).val().match(/\d/g).join("").length == 14) {
    //    var data = { CNPJ: _pessoa.CNPJ.val() };
    //    iniciarRequisicao();
    //    executarReST("Pessoa/ConsultaCNPJCentralizada", data, function (arg) {
    //        if (arg.Success) {
    //            var argDados = { Data: arg.Data };
    //            if (argDados.Data != null) {
    //                PreencherObjetoKnout(_pessoa, argDados);
    //                habilitaCamposEndereco();
    //                verificaIENumero(_pessoa);
    //                BuscarCoordenadas(function () {
    //                    finalizarRequisicao();
    //                });
    //            }
    //        } else {
    //            exibirMensagem(tipoMensagem.falha, "Atenção", arg.Msg);
    //        }
    //    });
    //} else {
    //    exibirMensagem(tipoMensagem.atencao, "Atenção", "Por favor digite um CNPJ válido antes de consultar os dados na receita!");
    //    $('#divModalConsultaPessoa').modal('hide');
    //}

    if ($("#" + _pessoa.CNPJ.id).val().match(/\d/g) != null && $("#" + _pessoa.CNPJ.id).val().match(/\d/g).join("").length == 14) {
        var data = { CNPJ: _pessoa.CNPJ.val() };
        iniciarRequisicao();
        executarReST("Pessoa/ConsultaCNPJReceitaWS", data, function (arg) {
            if (arg.Success) {
                var argDados = { Data: arg.Data };
                if (argDados.Data != null) {
                    PreencherObjetoKnout(_pessoa, argDados);
                    habilitaCamposEndereco();
                    verificaIENumero(_pessoa);
                    BuscarCoordenadas(function () {
                        finalizarRequisicao();
                    });
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pessoas.Pessoa.PorFavorDigiteUmCNPJValidoAntesDeConsultarOsDadosNaReceita);
        Global.fecharModal('divModalConsultaPessoa');
    }
}

function enviarCNPJClick(e) {
    if ($("#" + _pessoa.CNPJ.id).val().match(/\d/g) != null && $("#" + _pessoa.CNPJ.id).val().match(/\d/g).join("").length == 14) {
        if (_pesquisaDadosReceita != null && _pesquisaDadosReceita.Captcha.val() != null && _pesquisaDadosReceita.Captcha.val() != "") {
            var data = { CNPJ: _pessoa.CNPJ.val(), Captcha: _pesquisaDadosReceita.Captcha.val(), Cookies: JSON.stringify(cookies) };
            iniciarRequisicao();
            executarReST("Pessoa/InformarCaptchaConsultaCNPJ", data, function (arg) {
                if (arg.Success) {
                    var argDados = { Data: arg.Data };
                    if (argDados.Data != null) {
                        PreencherObjetoKnout(_pessoa, argDados);
                        habilitaCamposEndereco();
                        verificaIENumero(_pessoa);
                        BuscarCoordenadas(function () {
                            finalizarRequisicao();
                        });
                    }
                    Global.fecharModal('divModalConsultaPessoa');
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                    validarCNPJClick();
                }
            });
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pessoas.Pessoa.PorFavorDigiteCaptchaAntesDeConsultarOsDadosNaReceita);
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pessoas.Pessoa.PorFavorDigiteUmCNPJValidoAntesDeConsultarOsDadosNaReceita);
        Global.fecharModal('divModalConsultaPessoa');
    }
}

function BloquearClick() {
    AbrirTelaBloqueioPessoaClick();
}

function DesbloquearClick() {
    DesbloquearPessoaClick();
}

function consultaEnderecoCEP(e) {
    if ($("#" + _pessoa.CEP.id).val().match(/\d/g) != null && $("#" + _pessoa.CEP.id).val().match(/\d/g).join("").length == 8) {
        var data = { CEP: $("#" + _pessoa.CEP.id).val() };
        executarReST("Localidade/BuscarEnderecoPorCEP", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != null && arg.Data.DescricaoCidadeEstado != null && arg.Data.DescricaoCidadeEstado != "" && arg.Data.CodigoCidade > 0) {
                    _pessoa.Bairro.val(arg.Data.Bairro);
                    _pessoa.Endereco.val(arg.Data.Logradouro);
                    _pessoa.Localidade.codEntity(arg.Data.CodigoCidade);
                    _pessoa.Localidade.val(arg.Data.DescricaoCidadeEstado);
                    _pessoa.Latitude.val(arg.Data.Latitude);
                    _pessoa.Longitude.val(arg.Data.Longitude);
                    // Comentado pois essas informações não retornam nessa consulta...
                    //_pessoa.LatitudeTransbordo.val(arg.Data.LatitudeTransbordo);
                    //_pessoa.LongitudeTransbordo.val(arg.Data.LongitudeTransbordo);
                    if (arg.Data.TipoLogradouro != null && arg.Data.TipoLogradouro != "") {
                        if (arg.Data.TipoLogradouro == "Rua")
                            _pessoa.TipoLogradouro.val(1);
                        else if (arg.Data.TipoLogradouro == "Avenida")
                            _pessoa.TipoLogradouro.val(2);
                        else if (arg.Data.TipoLogradouro == "Rodovia")
                            _pessoa.TipoLogradouro.val(3);
                        else if (arg.Data.TipoLogradouro == "Estrada")
                            _pessoa.TipoLogradouro.val(4);
                        else if (arg.Data.TipoLogradouro == "Praca")
                            _pessoa.TipoLogradouro.val(5);
                        else if (arg.Data.TipoLogradouro == "Praça")
                            _pessoa.TipoLogradouro.val(5);
                        else if (arg.Data.TipoLogradouro == "Travessa")
                            _pessoa.TipoLogradouro.val(6);
                        else
                            _pessoa.TipoLogradouro.val(99);
                    }
                    $("#" + _pessoa.Complemento.id).focus();
                    verificaEnderecoUnico(arg.Data.DescricaoCidade);
                } else if (_pessoa.EnderecoDigitado.val() == false) {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.Pessoa.ConsultaDeCEP, Localization.Resources.Pessoas.Pessoa.CEPInformadoNaoExisteNaBaseDeDados);
                    _pessoa.Bairro.val("");
                    _pessoa.Endereco.val("");
                    LimparCampoEntity(_pessoa.Localidade);
                    _pessoa.EnderecoDigitado.val(true);
                    habilitaCamposEndereco();
                } else if (_pessoa.EnderecoDigitado.val() == true) {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.Pessoa.ConsultaDeCEP, Localization.Resources.Pessoas.Pessoa.CEPInformadoNaoExisteNaBaseDeDados);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

function veriticaEnderecoDigitado(e, sender) {
    _pessoa.EnderecoDigitado.val(_pessoa.EnderecoDigitado.val());
}

function verificaIENumero(e, sender) {
    if (_pessoa.Numero.val() == "S/N" || _pessoa.Numero.val() == "") {
        _pessoa.SN_Numero.val(false);
        _pessoa.Numero.val("S/N");
        _pessoa.Numero.enable(false);
    } else {
        _pessoa.SN_Numero.val(true);
        _pessoa.Numero.enable(true);
    }
    if (_pessoa.IE_RG.val() == "ISENTO" || _pessoa.IE_RG.val() == "") {
        _pessoa.IE_ISENTO.val(false);
        _pessoa.IE_RG.val("ISENTO");
        _pessoa.IE_RG.enable(false);
    } else {
        _pessoa.IE_ISENTO.val(true);
        _pessoa.IE_RG.enable(true);
    }
}

function verificarSNNumero(e, sender) {
    if (_pessoa.SN_Numero.val() == false) {
        _pessoa.Numero.val("S/N");
        _pessoa.Numero.enable(false);
    } else {
        if (_pessoa.Numero.val("S/N"))
            _pessoa.Numero.val("");
        _pessoa.Numero.enable(true);
    }
}

function verificarIEISENTO(e, sender) {
    if (_pessoa.IE_ISENTO.val() == false) {
        _pessoa.IE_RG.val("ISENTO");
        _pessoa.IE_RG.enable(false);
    } else {
        if (_pessoa.IE_RG.val() == "ISENTO")
            _pessoa.IE_RG.val("");
        _pessoa.IE_RG.enable(true);
    }
}

function digitarEndereco(e, sender) {
    verificaDigitarEndereco();
}

function verificarTipoPessoa(e, sender) {
    if (_pessoa.TipoTransportador.val()) {
        if (_pessoa.Codigo.val() == 0 && _CONFIGURACAO_TMS.GerarCIOTMarcadoAoCadastrarTransportadorTerceiro)
            preencherDadosTransportadorTerceiro();
        preencherUsuarioAutomatico();
        $("#liTransportadorTerceiro").show();

        if (!_CONFIGURACAO_TMS.NaoUtilizarUsuarioTransportadorTerceiro)
            $("#liUsuarioTerceiro").show();
        else
            $("#liUsuarioTerceiro").hide();
    } else {
        $("#liTransportadorTerceiro").hide();
        $("#liUsuarioTerceiro").hide();
        if (_usuarioTerceiro)
            LimparCampos(_usuarioTerceiro);
    }

    if (_pessoa.TipoFornecedor.val()) {
        $("#liFornecedor").show();
        $("#liDocumento").show();
    } else {
        $("#liFornecedor").hide();
        $("#liDocumento").hide();
    }

    preencherUsuarioPortalAcessoAutomatico();

    SetarCamposObrigatoriosPessoa();
}

function tipoPessoaChange(e, sender) {
    if (_pessoa.TipoPessoa.val() == EnumTipoPessoa.Juridica) {
        setarCNPJPadrao(e);
        $("#divClearJuridica").show();
        $("#liTabExterior").hide();
    } else if (_pessoa.TipoPessoa.val() == EnumTipoPessoa.Fisica) {
        $("#liTabExterior").hide();
        _pessoa.Nome.text(Localization.Resources.Pessoas.Pessoa.Nome.getRequiredFieldDescription());
        _pessoa.Nome.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-8");
        $("#divClearJuridica").hide();
        _pessoa.CNPJ.visible(false);
        _pessoa.CNPJ.required = false;

        _pessoa.CPF.visible(true);
        _pessoa.CPF.required = true;

        _pessoa.CEP.text(Localization.Resources.Pessoas.Pessoa.CEP.getRequiredFieldDescription());
        _pessoa.CEP.required = true;

        _pessoa.CPF.getType = typesKnockout.cpf;
        _pessoa.CNPJ.getType = typesKnockout.string;

        _pessoa.RG.visible(true);
        if (_pessoa.Codigo.val() === 0)
            _pessoa.IndicadorIE.val(9);
        _pessoa.RG.text(Localization.Resources.Pessoas.Pessoa.RG.getFieldDescription());
        _pessoa.Passaporte.visible(false);
        _pessoa.NumeroCUITRUT.visible(false);
        _pessoa.Passaporte.text(Localization.Resources.Pessoas.Pessoa.Passaporte.getFieldDescription());
        _pessoa.IE_RG.visible(true);
        _pessoa.IE_ISENTO.visible(false);
        _pessoa.Fantasia.visible(false);
        _pessoa.IE_RG.required = true;

        _pessoa.Email.required = false;
        _pessoa.Email.text(Localization.Resources.Pessoas.Pessoa.EmailPrincipal.getFieldDescription());
    } else {
        $("#divClearJuridica").hide();
        _pessoa.Nome.text(Localization.Resources.Pessoas.Pessoa.RazaoSocial.getRequiredFieldDescription());
        _pessoa.Nome.cssClass("col col-xs-12 col-sm-12 col-md-12 col-lg-6");
        _pessoa.Endereco.cssClass("col col-xs-12 col-sm-12 col-md-12 col-lg-5");
        _pessoa.TelefonePrincipal.cssClass("col col-xs-12 col-sm-6 col-md-3 col-lg-2");
        _pessoa.TelefoneSecundario.cssClass("col col-xs-12 col-sm-6 col-md-3 col-lg-2");
        if (_pessoa.Codigo.val() === 0)
            _pessoa.IndicadorIE.val(9);
        _pessoa.CNPJ.visible(false);
        _pessoa.CNPJ.required = false;

        _pessoa.CEP.text(Localization.Resources.Pessoas.Pessoa.CEP.getFieldDescription());
        _pessoa.CEP.required = false;

        _pessoa.CPF.visible(false);
        _pessoa.CPF.required = false;

        _pessoa.CPF.getType = typesKnockout.string;
        _pessoa.CNPJ.getType = typesKnockout.string;

        _pessoa.RG.visible(false);
        _pessoa.RG.text(Localization.Resources.Pessoas.Pessoa.RG.getFieldDescription());
        _pessoa.Passaporte.visible(true);
        _pessoa.NumeroCUITRUT.visible(true);
        _pessoa.Passaporte.text(Localization.Resources.Pessoas.Pessoa.Passaporte.getFieldDescription());
        _pessoa.IE_RG.visible(false);
        _pessoa.IE_ISENTO.visible(true);
        _pessoa.Fantasia.visible(true);
        _pessoa.IE_RG.required = false;

        _pessoa.Email.required = false;
        _pessoa.Email.text(Localization.Resources.Pessoas.Pessoa.EmailPrincipal.getFieldDescription());

        $("#liTabExterior").show();
    }
}

function selecionarEnderecoClick(enderecoSelecionado) {
    if (enderecoSelecionado != null) {
        Global.fecharModal('divModalConsultaEndereco');

        _pessoa.CEP.val(enderecoSelecionado.CEP);
        _pessoa.Bairro.val(enderecoSelecionado.Bairro);
        _pessoa.Endereco.val(enderecoSelecionado.Logradouro);
        _pessoa.Localidade.codEntity(enderecoSelecionado.CodigoCidade);
        _pessoa.Localidade.val(enderecoSelecionado.Descricao);
        _pessoa.Latitude.val(enderecoSelecionado.Latitude);
        _pessoa.Longitude.val(enderecoSelecionado.Longitude);
        if (enderecoSelecionado.TipoLogradouro != null && enderecoSelecionado.TipoLogradouro != "") {
            if (enderecoSelecionado.TipoLogradouro == "Rua")
                _pessoa.TipoLogradouro.val(1);
            else if (enderecoSelecionado.TipoLogradouro == "Avenida")
                _pessoa.TipoLogradouro.val(2);
            else if (enderecoSelecionado.TipoLogradouro == "Rodovia")
                _pessoa.TipoLogradouro.val(3);
            else if (enderecoSelecionado.TipoLogradouro == "Estrada")
                _pessoa.TipoLogradouro.val(4);
            else if (enderecoSelecionado.TipoLogradouro == "Praca")
                _pessoa.TipoLogradouro.val(5);
            else if (enderecoSelecionado.TipoLogradouro == "Praça")
                _pessoa.TipoLogradouro.val(5);
            else if (enderecoSelecionado.TipoLogradouro == "Travessa")
                _pessoa.TipoLogradouro.val(6);
            else
                _pessoa.TipoLogradouro.val(99);
        }

        _pessoa.EnderecoDigitado.val(false);
        desabilitaCamposEndereco();

        verificaEnderecoUnico(enderecoSelecionado.DescricaoCidade);

        $("#" + _pessoa.Complemento.id).focus();
    }
}

function pesquisarEnderecosClick(e, sender) {
    var data = {
        Logradouro: _pesquisaEndereco.Logradouro.val(),
        CEP: _pesquisaEndereco.CEP.val(),
        Bairro: _pesquisaEndereco.Bairro.val(),
        Descricao: _pesquisaEndereco.Localidade.val(),
        CodigoIBGE: _pesquisaEndereco.CodigoIBGE.val(),
        CodigoCidade: _pesquisaEndereco.Localidade.codEntity(),
        NomeCidade: _pesquisaEndereco.Localidade.val()
    };
    var realizaConsulta = false;

    realizaConsulta = data.Logradouro != "" || data.CEP != "" || data.Bairro != "" || data.Descricao != "" || data.CodigoIBGE != "" || data.CodigoCidade != "" || data.NomeCidade != "";

    if (realizaConsulta) {
        executarReST("Localidade/BuscarEnderecosCorreio", data, function (arg) {
            if (arg.Success) {
                $.each(arg.Data, function (i, endereco) {

                    var selecionar = { descricao: Localization.Resources.Pessoas.Pessoa.Selecionar, id: guid(), evento: "onclick", metodo: selecionarEnderecoClick, tamanho: "15", icone: "" };
                    var menuOpcoes = new Object();
                    menuOpcoes.tipo = TypeOptionMenu.link;
                    menuOpcoes.opcoes = new Array();
                    menuOpcoes.opcoes.push(selecionar);

                    var header = [
                        { data: "Descricao", title: Localization.Resources.Pessoas.Pessoa.Cidade, width: "20%", className: "text-align-left" },
                        { data: "Logradouro", title: Localization.Resources.Pessoas.Pessoa.Endereco, width: "20%", className: "text-align-left" },
                        { data: "Bairro", title: Localization.Resources.Pessoas.Pessoa.Bairro, width: "15%" },
                        { data: "CEP", title: Localization.Resources.Pessoas.Pessoa.CEP, width: "10%", className: "text-align-right" },
                        { data: "CodigoIBGE", title: Localization.Resources.Pessoas.Pessoa.CodigoIBGE, width: "10%", className: "text-align-right" },
                        { data: "TipoLogradouro", title: Localization.Resources.Pessoas.Pessoa.Tipo, width: "10%", className: "text-align-left" },
                        { data: "CodigoCidade", visible: false },
                        { data: "Latitude", visible: false },
                        { data: "Longitude", visible: false },
                        { data: "DescricaoCidade", visible: false }
                    ];
                    var gridEnderecos = new BasicDataTable(_pesquisaEndereco.Enderecos.idGrid, header, menuOpcoes);
                    gridEnderecos.CarregarGrid(endereco);
                });
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    } else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pessoas.Pessoa.PorFavorInformeAoMenosUmDosCamposParaRealizarConsulta);
}

function cancelarEnderecoClick(e, sender) {
    Global.fecharModal('divModalConsultaEndereco');
}

function preencherOutrosDadosPessoa() {
    var tudoCerto = true;
    var fornecedorDATA = {
        Fornecedor: RetornarObjetoPesquisa(_fornecedor),
        TabelaValores: _fornecedor != null ? _fornecedor.TabelaValores.list : [],
        TabelaMultiplosVencimentos: _fornecedor != null ? _fornecedor.TabelaMultiplosVencimentos.list : []
    };
    _pessoa.Fornecedor.val(JSON.stringify(fornecedorDATA));
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _pessoa.UsuarioTerceiro.val(JSON.stringify(RetornarObjetoPesquisa(_usuarioTerceiro)));
        _pessoa.TransportadorTerceiro.val(JSON.stringify(RetornarObjetoPesquisa(_transportadorTerceiro)));
        _transportadorTerceiro.DescontoPadrao.requiredClass("form-control");
        _transportadorTerceiro.RNTRC.requiredClass("form-control");
        _transportadorTerceiro.PercentualAdiantamentoFretesTerceiro.requiredClass("form-control");
        _transportadorTerceiro.TiposPagamentoCIOTOperadora.val(JSON.stringify(_transportadorTerceiro.TiposPagamentoCIOTOperadora.list));
        _transportadorTerceiro.DiasFechamentoCIOTPeriodo.val(JSON.stringify(_transportadorTerceiro.DiasFechamentoCIOTPeriodo.list));

        if (_CONFIGURACAO_TMS.SolicitarValorFretePorTonelada && _pessoa.TipoTransportador.val()) {
            _transportadorTerceiro.PercentualAdiantamentoFretesTerceiro.required(true);
            _transportadorTerceiro.PercentualAdiantamentoFretesTerceiro.text(Localization.Resources.Pessoas.Pessoa.PorcentagemDeAdiantamentoNoValorDoFreteQuandoSubcontrataComoTerceiroEsseTransportador.getRequiredFieldDescription());
        }

        tudoCerto = _pessoa.TipoTransportador.val() ? ValidarCamposObrigatorios(_transportadorTerceiro) : true;
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        _pessoa.UsuarioTerceiro.val(JSON.stringify(RetornarObjetoPesquisa(_usuarioTerceiro)));
        _pessoa.TransportadorTerceiro.val(JSON.stringify(RetornarObjetoPesquisa(_transportadorTerceiro)));
        _transportadorTerceiro.RNTRC.requiredClass("form-control");
        _transportadorTerceiro.PercentualAdiantamentoFretesTerceiro.requiredClass("form-control");
        tudoCerto = _pessoa.TipoTransportador.val() ? ValidarCamposObrigatorios(_transportadorTerceiro) : true;
    } else {
        _pessoa.UsuarioTerceiro.val(JSON.stringify(RetornarObjetoPesquisa(_usuarioTerceiro)));
        _pessoa.TransportadorTerceiro.val(JSON.stringify(RetornarObjetoPesquisa(_transportadorTerceiro)));
        _transportadorTerceiro.DescontoPadrao.requiredClass("form-control");
        _transportadorTerceiro.RNTRC.requiredClass("form-control");
        _transportadorTerceiro.PercentualAdiantamentoFretesTerceiro.requiredClass("form-control");
        tudoCerto = _pessoa.TipoTransportador.val() ? ValidarCamposObrigatorios(_transportadorTerceiro) : true;
        _pessoa.Descarga.val(JSON.stringify(RetornarObjetoPesquisa(_descarga)));
    }
    let listaAcrecimoDescontoAutomatico = new Array();

    $.each(_acrescimoDescontoAutomatico.Regra.basicTable.BuscarRegistros(), function (i, acrecimoDescontoAutomatico) {
        listaAcrecimoDescontoAutomatico.push({ Regra: acrecimoDescontoAutomatico });
    });

    _pessoa.AcrescimoDescontoAutomatico.val(JSON.stringify(listaAcrecimoDescontoAutomatico));

    _dadoBancario.ContasBancarias.val(JSON.stringify(_dadoBancario.ContaBancaria.basicTable.BuscarRegistros()));

    _pessoa.DadosBancarios.val(JSON.stringify(RetornarObjetoPesquisa(_dadoBancario)));
    _pessoa.ListaContatos.val(JSON.stringify(_pessoa.Contatos.val()));
    _pessoa.ListaOutrasDescricoesPessoaExterior.val(JSON.stringify(_pessoa.OutrasDescricoesPessoaExterior.val()));

    if (!_CONFIGURACAO_TMS.NaoUtilizarUsuarioTransportadorTerceiro) {
        _pessoa.ListaUsuariosAdicionais.val(JSON.stringify(_usuarioAdicional.GridUsuarioAdicionais.basicTable.BuscarRegistros()));
    }

    _pessoa.AcessoPortal.val(JSON.stringify(RetornarObjetoPesquisa(_portalAcesso)));

    let jsonPoligonoGeoLocalizacao = obterJsonPoligonoGeoLocalizacao();
    if (jsonPoligonoGeoLocalizacao)
        _pessoa.Area.val(jsonPoligonoGeoLocalizacao);

    if (_marker != null) {
        var latLng = _marker.getPosition();
        _pessoa.Latitude.val(latLng.lat().toString());
        _pessoa.Longitude.val(latLng.lng().toString());
    }

    if (_markerTransbordo != null) {
        var latLng = _markerTransbordo.getPosition();
        if (latLng) {
            _pessoa.LatitudeTransbordo.val(latLng.lat().toString());
            _pessoa.LongitudeTransbordo.val(latLng.lng().toString());
        }
    }

    _pessoa.PontoDeApoio.val(_pesquisaGeolocalizacao.PontoDeApoio.codEntity());
    _pessoa.AtualizarPontoApoioMaisProximoAutomaticamente.val(_pesquisaGeolocalizacao.AtualizarPontoApoioMaisProximoAutomaticamente.val());
    _pessoa.DadosAdicionais.val(JSON.stringify(RetornarObjetoPesquisa(_pessoaAdicional)));
    _pessoa.TipoOperacaoRedespacho.val(_areaRedex.TipoOperacaoRedespacho.codEntity());

    return tudoCerto;
}

function adicionarClick(e, sender) {
    preencherListasGrupoPessoas();
    if (!validarRegrasPessoa())
        return;

    if (ClienteSemCoordenadas()) {
        iniciarRequisicao();
        BuscarCoordenadas(function () {
            adicionarPessoa(function () {
                finalizarRequisicao();
            });
        });
    } else {
        ValidaCoordenadaRaioLocalidade(_pessoa.Localidade.codEntity(), function () {
            adicionarPessoa();
        });
    }
}

function adicionarPessoa(callback) {
    if (_pessoa.TipoTransportador.val() == true && _usuarioTerceiro != null && _usuarioTerceiro.Senha.val() != _usuarioTerceiro.ConfirmaSenha.val()) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.Pessoa.SenhasUsuarioTerceiro, Localization.Resources.Pessoas.Pessoa.SenhasDoUsuarioTerceiroNaoConferemVerifiqueTenteNovamente);
        if (callback != null)
            callback();
    }
    else {
        _pessoa.NaoUsarConfiguracaoFaturaGrupo.val(_pessoaFatura.NaoUsarConfiguracaoFaturaGrupo.val());
        _pessoa.NaoUsarConfiguracaoEmissaoGrupo.val(_pessoaEmissao.NaoUsarConfiguracaoEmissaoGrupo.val());
        _pessoa.RestricoesFilaCarregamento.val(obterRestricaoFilaCarregamentos());
        _pessoa.Comprovantes.val(obterListaTipoComprovanteSalvar());
        _pessoa.NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa.val(_comprovante.NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa.val());
        _pessoa.ExigirComprovantesLiberacaoPagamentoContratoFrete.val(_comprovante.ExigirComprovantesLiberacaoPagamentoContratoFrete.val());
        _pessoa.ListaSubarea.val(obterListaSubareas());
        _pessoa.SuprimentoGas.val(obterSuprimentoGas());

        Salvar(_pessoa, "Pessoa/Adicionar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                    _gridPessoa.CarregarGrid();
                    limparCamposPessoa();
                    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                        _transportadorTerceiro.DescontoPadrao.requiredClass("form-control");
                        _transportadorTerceiro.RNTRC.requiredClass("form-control");
                        _transportadorTerceiro.PercentualAdiantamentoFretesTerceiro.requiredClass("form-control");
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
            if (callback != null)
                callback();
        }, null, function () {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
            finalizarRequisicao();
        });
    }
}

function atualizarClick(e, sender) {
    preencherListasGrupoPessoas();
    if (!validarRegrasPessoa())
        return;

    if (ClienteSemCoordenadas()) {
        iniciarRequisicao();
        BuscarCoordenadas(function () {
            atualizarPessoa(function () {
                finalizarRequisicao();
            });
        });
    } else {
        ValidaCoordenadaRaioLocalidade(_pessoa.Localidade.codEntity(), function () {
            atualizarPessoa();
        });
    }
}

function atualizarPessoa(callback) {
    if (_pessoa.TipoTransportador.val() == true && _usuarioTerceiro != null && _usuarioTerceiro.Senha.val() != _usuarioTerceiro.ConfirmaSenha.val()) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.Pessoa.SenhasUsuarioTerceiro, Localization.Resources.Pessoas.Pessoa.SenhasDoUsuarioTerceiroNaoConferemVerifiqueTenteNovamente);
        if (callback != null)
            callback();
    }
    else {
        _pessoa.NaoUsarConfiguracaoFaturaGrupo.val(_pessoaFatura.NaoUsarConfiguracaoFaturaGrupo.val());
        _pessoa.NaoUsarConfiguracaoEmissaoGrupo.val(_pessoaEmissao.NaoUsarConfiguracaoEmissaoGrupo.val());
        _pessoa.RestricoesFilaCarregamento.val(obterRestricaoFilaCarregamentos());
        _pessoa.NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa.val(_comprovante.NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa.val());
        _pessoa.ExigirComprovantesLiberacaoPagamentoContratoFrete.val(_comprovante.ExigirComprovantesLiberacaoPagamentoContratoFrete.val());
        _pessoa.ListaSubarea.val(obterListaSubareas());
        _pessoa.SuprimentoGas.val(obterSuprimentoGas());
        _pessoa.Comprovantes.val(obterListaTipoComprovanteSalvar());

        Salvar(_pessoa, "Pessoa/Atualizar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
                    _gridPessoa.CarregarGrid();
                    limparCamposPessoa();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
            if (callback != null)
                callback();
        }, null, function (e) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
            finalizarRequisicao();
        });
    }
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.RealmenteDesejaExcluirContatoSelecionado + _pessoa.Nome.val(), function () {
        ExcluirPorCodigo(_pessoa, "Pessoa/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridPessoa.CarregarGrid();
                    limparCamposPessoa();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposPessoa();
}

//*******MÉTODOS*******

function validarRegrasPessoa() {
    //setarPessoaEmissao();
    if (!_pessoa.TipoTransportador.val() && !_pessoa.TipoCliente.val() && !_pessoa.TipoFornecedor.val()) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pessoas.Pessoa.NecessarioInformarClienteFornecedorTerceiro);
        return false;
    }


    var tudoCerto = preencherOutrosDadosPessoa();
    if (!tudoCerto) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        $("#liTransportadorTerceiro a").tab("show");
        if (_transportadorTerceiro.DescontoPadrao.val() == "0,00")
            _transportadorTerceiro.DescontoPadrao.requiredClass("form-control is-invalid");
        if (_transportadorTerceiro.DescontoPadrao.val() == "")
            _transportadorTerceiro.DescontoPadrao.requiredClass("form-control is-invalid");
        if (_transportadorTerceiro.RNTRC.val() == "")
            _transportadorTerceiro.RNTRC.requiredClass("form-control is-invalid");

        if (_CONFIGURACAO_TMS.SolicitarValorFretePorTonelada && _pessoa.TipoTransportador.val()) {
            if (_transportadorTerceiro.PercentualAdiantamentoFretesTerceiro.val() == "")
                _transportadorTerceiro.PercentualAdiantamentoFretesTerceiro.requiredClass("form-control is-invalid");
        }
        return false;
    }

    _pessoa.IndicadorIE.requiredClass("form-control");

    if (_pessoa.IndicadorIE.val() == 1 && _pessoa.IE_RG.val() == "ISENTO") {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.Pessoa.IndicadorDeIE, Localization.Resources.Pessoas.Pessoa.ParaCadastrosComIsencaoDeInscricaoEstadualNecessarioInformarUmIndicadorDeIEDiferenteDeUmContribuinteICMSPoisCNPJCPFIsentosNaoSaoContribuintesDeICMS);
        _pessoa.IndicadorIE.requiredClass("form-control is-invalid");
        return false;
    } else if ((_pessoa.IndicadorIE.val() === null || _pessoa.IndicadorIE.val() === undefined || _pessoa.IndicadorIE.val() === EnumIndicadorIE.NaoInformado) && _CONFIGURACAO_TMS.Pais != EnumPaises.Exterior) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.Pessoa.IndicadorDeIE, Localization.Resources.Pessoas.Pessoa.NecessarioInformarIndicadorDeIE);
        _pessoa.IndicadorIE.requiredClass("form-control is-invalid");
        return false;
    } else if (!validarCamposPessoaAdicional())
        return false;
    else if (!validarCamposPessoaDadoBancario())
        return false;
    else if (!validarCamposPessoaFornecedor())
        return false;
    else if (!validarCamposPessoaPortalAcesso())
        return false;

    if (_pessoaEmissao.NaoUsarConfiguracaoEmissaoGrupo.val()) {
        tudoCerto = ValidarCamposObrigatorios(_configuracaoEmissaoCTe.Configuracao);
        if (!tudoCerto) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
            $("#liEmissao a").tab("show");
            return false;
        }
    }

    if (_pessoaFatura.NaoUsarConfiguracaoFaturaGrupo.val()) {
        tudoCerto = ValidarCamposObrigatorios(_configuracaoFatura.Configuracao);
        if (!tudoCerto) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
            $("#liTabConfiguracaoFatura a").tab("show");
            return false;
        }
    }

    return true;
}

function habilitaCamposEndereco() {
    _pessoa.Bairro.enable(true);
    _pessoa.Endereco.enable(true);
    _pessoa.Localidade.enable(true);
    _pessoa.Localidade.required(true);
}

function desabilitaCamposEndereco() {
    _pessoa.Bairro.enable(false);
    _pessoa.Endereco.enable(false);
    _pessoa.Localidade.enable(false);
    _pessoa.Localidade.required(false);
}

function buscarPessoas() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarPessoa, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe) {
        var configExportacao = {
            url: "Pessoa/ExportarPesquisa",
            titulo: Localization.Resources.Pessoas.Pessoa.DescricaoPessoas
        };
    }

    _gridPessoa = new GridViewExportacao(_pesquisaPessoa.Pesquisar.idGrid, "Pessoa/Pesquisa", _pesquisaPessoa, menuOpcoes, configExportacao, { column: 6, dir: orderDir.asc });
    _gridPessoa.CarregarGrid();
}

function editarPessoa(pessoaGrid) {
    limparCamposPessoa();
    _pessoa.Codigo.val(pessoaGrid.Codigo);
    BuscarPorCodigo(_pessoa, "Pessoa/BuscarPorCodigo", function (arg) {

        tipoPessoaChange(_pessoa);

        verificarTipoPessoa(_pessoa);
        verificaIENumero(_pessoa);
        veriticaEnderecoDigitado(_pessoa);
        verificaDigitarEndereco();


        recarregarGridListaEmail();
        recarregarGridListaSubarea();
        recarregarGridListaEndereco();
        recarregarGridListaDocumento();
        RecarregarGridContato();
        recarregarGridPessoaLicencas();
        recarregarGridPessoaArmador();
        recarregarGridPessoaVendedores();
        recarregarGridPessoaRecebedoresAutorizados();
        recarregarGridPessoaComponentes();
        recarregarGridRestricaoFilaCarregamento();
        RecarregarGridObservacaoCTe();
        RecarregarGridOutraDescricaoPessoaExterior();
        recarregarGridAreaRedex(); //
        recarregarGridFrequenciaCarregamento();
        recarregarGridAcrescimoDescontoAutomatico();
        RecarregarGridGrupoPessoas();
        recarregarGridComprovante();
        recarregarPessoaIntegracoes();
        recarregarGridFilialCliente();
        RecarregarGridDataFixaVencimento();

        _dadoBancario.ContaBancaria.basicTable.CarregarGrid(arg.Data.DadosBancarios.ContasBancarias);

        if (arg.Data.TransportadorTerceiro != null) {
            _transportadorTerceiro.TiposPagamentoCIOTOperadora.list = arg.Data.TransportadorTerceiro.TiposPagamentoCIOTOperadora;
            _transportadorTerceiro.DiasFechamentoCIOTPeriodo.list = arg.Data.TransportadorTerceiro.DiasFechamentoCIOTPeriodo;
        }
        recarregarGridTiposPagamentoCIOTConsulta();
        recarregarGridDiasFechamentoCIOTPeriodoConsulta();

        _pessoaFatura.NaoUsarConfiguracaoFaturaGrupo.val(_pessoa.NaoUsarConfiguracaoFaturaGrupo.val());
        _pessoaEmissao.NaoUsarConfiguracaoEmissaoGrupo.val(_pessoa.NaoUsarConfiguracaoEmissaoGrupo.val());
        _comprovante.NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa.val(arg.Data.NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa);
        _comprovante.ExigirComprovantesLiberacaoPagamentoContratoFrete.val(arg.Data.ExigirComprovantesLiberacaoPagamentoContratoFrete);


        if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Pessoas_PermiteBloquearDesbloquearPessoa, _PermissoesPersonalizadas)) {
            _pessoaBotoes.Bloquear.visible(!arg.Data.Bloqueado);
            _pessoaBotoes.Desbloquear.visible(arg.Data.Bloqueado);
            _pessoaBotoes.Bloquear.enable(true);
            _pessoaBotoes.Desbloquear.enable(true);
        }

        if (arg.Data.Bloqueado) {
            $("#txtMotivoBloqueio").text(arg.Data.MotivoBloqueio);
            $("#divMotivoBloqueioPessoa").removeClass("d-none");
        }

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
            _pessoa.TipoTransportador.val(true);

            if (arg.Data.ConfiguracaoLayoutEDI != null)
                _configuracaoLayoutEDI.SetarValores(arg.Data.ConfiguracaoLayoutEDI);
        }

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) {
            PreencherDadosAcessoPortal(arg.Data.AcessoPortal);
        }

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
            if (arg.Data.Descarga != null)
                PreencherObjetoKnout(_descarga, { Data: arg.Data.Descarga });

            if (arg.Data.ConfiguracaoLayoutEDI != null)
                _configuracaoLayoutEDI.SetarValores(arg.Data.ConfiguracaoLayoutEDI);

            _pessoa.TipoCliente.val(true);
            _configuracaoFatura.SetarValores(arg.Data.ConfiguracaoFatura);

            if (arg.Data.Fornecedor != null) {
                PreencherDadosFornecedor(arg.Data.Fornecedor);
                EditarListaAnexosFornecedor(arg.Data.Fornecedor.Anexos);
                recarregarGridTabelaValores();
            }
        } else {
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
                if (arg.Data.Fornecedor != null) {
                    PreencherDadosFornecedor(arg.Data.Fornecedor);
                    recarregarGridTabelaValores();
                    recarregarGridTabelaMultiplosVencimentos();
                }

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                    _configuracaoEmissaoCTe.SetarValores(arg.Data.ConfiguracaoEmissaoCTe);
                    _configuracaoLayoutEDI.SetarValores(arg.Data.ConfiguracaoLayoutEDI);
                    _configuracaoFatura.SetarValores(arg.Data.ConfiguracaoFatura);
                }
            }
        }

        if (arg.Data.UsuarioTerceiro != null)
            PreencherObjetoKnout(_usuarioTerceiro, { Data: arg.Data.UsuarioTerceiro });

        if (arg.Data.TransportadorTerceiro != null)
            PreencherObjetoKnout(_transportadorTerceiro, { Data: arg.Data.TransportadorTerceiro });

        setarCoordenadas();
        setarTipoArea();
        setarRaioEmMetros();
        SetarAreaGeoLocalizacao();
        setarPontoDeApoio(arg.Data.PontoDeApoio, arg.Data.AtualizarPontoApoioMaisProximoAutomaticamente);

        ListarRotas(arg.Data);
        ListarRestricoes(arg.Data);

        if (arg.Data.DadosBancarios != null)
            PreencherObjetoKnout(_dadoBancario, { Data: arg.Data.DadosBancarios });

        if (arg.Data.DadosAdicionais != null)
            PreencherObjetoKnout(_pessoaAdicional, { Data: arg.Data.DadosAdicionais });

        _pessoa.TipoPessoa.enable(false);
        _pessoa.ValidarCNPJ.enable(false);
        _pessoa.CNPJ.enable(false);
        _pessoa.CPF.enable(false);

        _pesquisaPessoa.ExibirFiltros.visibleFade(false);
        _pessoaBotoes.Atualizar.visible(true);
        if (_usuarioTerceiro != null)
            _usuarioTerceiro.Email.visible(true);
        _pessoaBotoes.Cancelar.visible(true);
        _pessoaBotoes.Excluir.visible(true);
        _pessoaBotoes.Adicionar.visible(false);

        verificarTipoPessoa();
        ValidarPermissaoPersonalizada();
        habilitarAreaRedex();
        preencherAreaRedex(arg.Data);
        habilitarAbaDadosArmador();
        preencherFilialSuprimentoDeGas(arg.Data.SuprimentoGas);
        recarregarGridOutroCodigoIntegracao();
       
        if (arg.Data.GrupoPessoas != null && arg.Data.GrupoPessoas.DisponibilizarDocumentosParaLoteEscrituracao)
            _pessoa.VerificarUnidadeNegocioPorDestinatario.visible(true);
        else
            _pessoa.VerificarUnidadeNegocioPorDestinatario.visible(false);
        EditarListarAnexos(arg);
        _pessoa.Motorista.visible(false);
        verificarDataFixaVencimento();

        if (!_CONFIGURACAO_TMS.NaoUtilizarUsuarioTransportadorTerceiro)
        {
            preencherGridUsuariosAdicionais();
        }

    }, null);
}

function limparCamposPessoa() {

    $("#divMotivoBloqueioPessoa").addClass("d-none");

    setarCNPJPadrao(_pessoa);
    _pessoaBotoes.Atualizar.visible(false);
    if (_usuarioTerceiro != null)
        _usuarioTerceiro.Email.visible(false);

    _pessoaBotoes.Cancelar.visible(false);
    _pessoaBotoes.Excluir.visible(false);
    _pessoaBotoes.Adicionar.visible(true);
    _pessoaBotoes.Bloquear.visible(false);
    _pessoaBotoes.Desbloquear.visible(false);
    _pessoa.VerificarUnidadeNegocioPorDestinatario.visible(false);

    _pessoa.TipoPessoa.enable(true);
    _pessoa.ValidarCNPJ.enable(true);
    _pessoa.CNPJ.enable(true);
    _pessoa.CPF.enable(true);
    resetarTabs();

    _pessoaFatura.NaoUsarConfiguracaoFaturaGrupo.val(_pessoaFatura.NaoUsarConfiguracaoFaturaGrupo.def);
    _pessoaEmissao.NaoUsarConfiguracaoEmissaoGrupo.val(_pessoaEmissao.NaoUsarConfiguracaoEmissaoGrupo.def);


    LimparCampos(_pessoa);
    LimparCamposAcessoPortal();
    LimparCamposPessoaContaBancaria();
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe)
        LimparCamposFornecedor();
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        LimparCamposDescarga();
        if (_usuarioTerceiro != null)
            LimparCampos(_usuarioTerceiro);
        _configuracaoLayoutEDI.Limpar();
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        if (_usuarioTerceiro != null)
            LimparCampos(_usuarioTerceiro);
        LimparCampos(_dadoBancario);
        LimparCamposTabelaValor();
        LimparCamposTabelaVencimentos();

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
            _configuracaoEmissaoCTe.Limpar();
            _configuracaoLayoutEDI.Limpar();
            _configuracaoFatura.Limpar();
        }

        $("#" + _fornecedor.PagoPorFatura.id).prop("checked", false);
        $("#" + _fornecedor.PostoConveniado.id).prop("checked", false);

    }
    LimparCampos(_transportadorTerceiro);
    limparCamposListaEmail();
    limparCamposListaSubarea();
    limparCamposListaEndereco();
    limparCamposListaDocumento();
    limparCamposDadoBancario();
    LimparCamposContato();
    LimparCamposPessoaLicencas();
    LimparCamposPessoaArmador();
    LimparCamposPessoaVendedores();
    LimparCamposPessoaRecebedoresAutorizados();
    LimparCamposRota();
    LimparCamposPessoaComponentes();
    limparCamposPessoaAdicional();
    LimparCamposObservacaoCTe();
    LimparCamposOutraDescricaoPessoaExterior();
    limparCamposAreaRedex();
    limparCamposOutrosCodigos();
    limparCamposAcrescimoDescontoAutomatico();
    LimparCamposGrupoPessoas();
    limparCamposComprovante();

    _pessoa.ListaEmail.list = new Array();
    _pessoa.ListaSubarea.list = new Array();
    _pessoa.ListaEndereco.list = new Array();
    _pessoa.ListaDocumento.list = new Array();
    _pessoa.ListaLicencas.list = new Array();
    _pessoa.ListaDadosArmador.list = new Array();
    _pessoa.ListaVendedores.list = new Array();
    _pessoa.DataFixaVencimento.list = new Array();
    _pessoa.ListaRecebedoresAutorizados.list = new Array();
    _pessoa.ListaComponentes.list = new Array();
    _pessoa.RestricoesFilaCarregamento.list = new Array();
    _pessoa.AreasRedex.list = new Array();
    _pessoa.FrequenciasCarregamento.list = new Array();
    _pessoa.AcrescimoDescontoAutomatico.list = new Array();
    _transportadorTerceiro.TiposPagamentoCIOTOperadora.list = new Array();
    _transportadorTerceiro.DiasFechamentoCIOTPeriodo.list = new Array();
    _transportadorTerceiro.ConfiguracaoCIOT.enable(true);
    _transportadorTerceiro.ReterImpostosContratoFrete.enable(true);

    recarregarGridListaEmail();
    recarregarGridListaSubarea();
    recarregarGridListaEndereco();
    recarregarGridListaDocumento();
    RecarregarGridContato();
    recarregarGridPessoaLicencas();
    recarregarGridPessoaArmador();
    recarregarGridPessoaVendedores();
    recarregarGridPessoaRecebedoresAutorizados();
    recarregarGridPessoaComponentes();
    recarregarGridRestricaoFilaCarregamento();
    RecarregarGridObservacaoCTe();
    RecarregarGridOutraDescricaoPessoaExterior();
    recarregarGridAreaRedex();
    recarregarGridOutroCodigoIntegracao();
    recarregarGridAcrescimoDescontoAutomatico();
    RecarregarGridGrupoPessoas();
    limparCamposFrequenciaCarregamento();
    recarregarGridComprovante();
    RecarregarGridDataFixaVencimento();
    recarregarGridTiposPagamentoCIOT();
    recarregarGridDiasFechamentoCIOTPeriodo();

    _gridSuprimentoGas.CarregarGrid([]);

    $("#" + _pessoa.EnderecoDigitado.id).prop("checked", false);

    $("#" + _pessoa.SN_Numero.id).prop("checked", false);
    $("#" + _pessoa.IE_ISENTO.id).prop("checked", true);

    _pessoa.SN_Numero.val(false);
    _pessoa.Numero.val("S/N");
    _pessoa.Numero.enable(false);

    _pessoa.IE_ISENTO.val(true);
    _pessoa.IE_RG.val("ISENTO");
    _pessoa.IE_RG.enable(false);

    _pessoa.EnderecoDigitado.val(false);
    desabilitaCamposEndereco();
    limparCamposMapaRequest();
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        _pessoa.TipoTransportador.val(true);
        verificarTipoPessoa();
    }
    ValidarPermissaoPersonalizada();
    limparCamposAnexo();
    limparCamposAreaRedex();
    habilitarAbaDadosArmador();
    verificarDataFixaVencimento();

    _pessoa.Motorista.visible(false);

    $("#liTabExterior").hide();
    $("#liFornecedorAvisos").hide();
    $("#fornecedorTabs li:visible:first:visible a").click()
}

function setarCNPJPadrao(e) {
    _pessoa.Nome.text(Localization.Resources.Pessoas.Pessoa.RazaoSocial.getRequiredFieldDescription());
    _pessoa.Nome.cssClass("col col-xs-12 col-sm-12 col-md-12 col-lg-6");
    _pessoa.Endereco.cssClass("col col-xs-12 col-sm-12 col-md-12 col-lg-5");
    _pessoa.TelefonePrincipal.cssClass("col col-xs-12 col-sm-6 col-md-3 col-lg-2");
    _pessoa.TelefoneSecundario.cssClass("col col-xs-12 col-sm-6 col-md-3 col-lg-2");
    $("#divClearJuridica").show();
    _pessoa.CNPJ.visible(true);
    _pessoa.CNPJ.required = true;

    _pessoa.CPF.visible(false);
    _pessoa.CPF.required = false;

    _pessoa.CPF.getType = typesKnockout.string;
    _pessoa.CNPJ.getType = typesKnockout.cnpj;

    _pessoa.CEP.text(Localization.Resources.Pessoas.Pessoa.CEP.getRequiredFieldDescription());
    _pessoa.CEP.required = true;

    if (_pessoa.Codigo.val() === 0) {
        if (!_pessoa.IE_ISENTO.val())
            _pessoa.IndicadorIE.val(2);
        else
            _pessoa.IndicadorIE.val(1);
    }

    _pessoa.RG.visible(false);
    _pessoa.Passaporte.visible(false);
    _pessoa.NumeroCUITRUT.visible(false);
    _pessoa.IE_RG.visible(true);
    _pessoa.IE_ISENTO.visible(false);
    _pessoa.Fantasia.visible(true);
    _pessoa.IE_RG.required = true;

    if (_pessoa.EnderecoDigitado.val() == false)
        desabilitaCamposEndereco();

    _pessoa.Email.required = _CONFIGURACAO_TMS.ExigirEmailPrincipalCadastroPessoa;
    _pessoa.Email.text((_CONFIGURACAO_TMS.ExigirEmailPrincipalCadastroPessoa ? "*" : "") + Localization.Resources.Pessoas.Pessoa.EmailPrincipal.getFieldDescription());
}

function verificaEnderecoUnico(nomeCidade) {
    if (_pessoa.Endereco.val() == null || _pessoa.Endereco.val() == "" || removeAcento(_pessoa.Endereco.val().toUpperCase()) == removeAcento(nomeCidade.toUpperCase())) {
        _pessoa.Bairro.enable(true);
        _pessoa.Endereco.enable(true);
        $("#" + _pessoa.Endereco.id).focus();
    } else {
        _pessoa.Bairro.enable(false);
        _pessoa.Endereco.enable(false);
    }
}

function removeAcento(strToReplace) {
    str_acento = "áàãâäéèêëíìîïóòõôöúùûüçÁÀÃÂÄÉÈÊËÍÌÎÏÓÒÕÖÔÚÙÛÜÇ";
    str_sem_acento = "aaaaaeeeeiiiiooooouuuucAAAAAEEEEIIIIOOOOOUUUUC";
    var nova = "";
    for (var i = 0; i < strToReplace.length; i++) {
        if (str_acento.indexOf(strToReplace.charAt(i)) != -1) {
            nova += str_sem_acento.substr(str_acento.search(strToReplace.substr(i, 1)), 1);
        } else {
            nova += strToReplace.substr(i, 1);
        }
    }
    return nova;
}

function resetarTabs() {
    $("#myTab a:eq(0)").tab("show");
    $("#liTransportadorTerceiro").hide();
    $("#liUsuarioTerceiro").hide();
    $("#liFornecedor").hide();
    $("#liDocumento").hide();
}

function carregarConteudosHTML(callback) {
    //$.get("Content/Static/Localidade/Localidade.html?dyn=" + guid(), function (data) {
    //    $("#ConsultaEndereco").html(data);
    //    _pesquisaEndereco = new PesquisaEndereco();
    //    KoBindings(_pesquisaEndereco, "knoutConsultaEndereco");
    //    new BuscarLocalidadesBrasil(_pesquisaEndereco.Localidade);
    //    $("#" + _pesquisaEndereco.CEP.id).mask("99.999-999");
    //});

    $.get("Content/Static/Pessoa/ConsultaPessoa.html?dyn=" + guid(), function (data) {
        $("#ConsultaDadosReceita").html(data);
        _pesquisaDadosReceita = new PesquisaDadosReceita();
        KoBindings(_pesquisaDadosReceita, "knoutConsultaPessoa");
    });
}

function verificaDigitarEndereco() {
    if (_pessoa.EnderecoDigitado.val() == true) {
        habilitaCamposEndereco();
    } else {
        desabilitaCamposEndereco();
    }
}

function ValidarPermissaoPersonalizada() {
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.GrupoPessoas_PermiteAlterarApenasObservacoes, _PermissoesPersonalizadas)) {
        $("#liTabAdicionais").hide();
        $("#liTabListaEndereco").hide();
        $("#liTabListaEmail").hide();
        $("#liTabDadosBancarios").hide();
        $("#liTabOutrosCodigos").hide();
        $("#liTabObservacaoNFe").hide();
        $("#liTransportadorTerceiro").hide();
        $("#liUsuarioTerceiro").hide();
        $("#liFornecedor").hide();
        $("#liDocumento").hide();
        $("#liEmissao").hide();
        $("#liTabLayoutsEDI").hide();
        $("#liDescarga").hide();
        $("#liComponente").hide();
        $("#liGeoLocalizacao").hide();
        $("#liTabContatos").hide();
        $("#liTabLicenca").hide();
        $("#liTabVendedor").hide();
        $("#liTabRecebedorAutorizado").hide();
        $("#liTabRota").hide();
        $("#liTabRestricaoFilaCarregamento").hide();

        SetarEnableCamposKnockout(_pessoa, false);
        _pessoa.CodigoIntegracao.enable(false);
        _pessoa.Observacao.enable(true);
        DestabilitarHabilitarCamposFatura(false);
    }
}

function DestabilitarHabilitarCamposFatura(enable) {
    _configuracaoFatura.Configuracao.DiaSemana.enable(enable);
    _configuracaoFatura.Configuracao.DiaMes.enable(enable);
    _configuracaoFatura.Configuracao.PermiteFinalSemana.enable(enable);
    _configuracaoFatura.Configuracao.ExigeCanhotoFisico.enable(enable);
    _configuracaoFatura.Configuracao.NaoGerarFaturaAteReceberCanhotos.enable(enable);
    _configuracaoFatura.Configuracao.Banco.enable(enable);
    _configuracaoFatura.Configuracao.Agencia.enable(enable);
    _configuracaoFatura.Configuracao.Digito.enable(enable);
    _configuracaoFatura.Configuracao.NumeroConta.enable(enable);
    _configuracaoFatura.Configuracao.TipoConta.enable(enable);
    _configuracaoFatura.Configuracao.TomadorFatura.enable(enable);
    _configuracaoFatura.Configuracao.ObservacaoFatura.enable(true);
    _configuracaoFatura.Configuracao.TipoPrazoFaturamento.enable(enable);
    _configuracaoFatura.Configuracao.DiasDePrazoFatura.enable(enable);
    _configuracaoFatura.Configuracao.FormaPagamento.enable(enable);
    _configuracaoFatura.Configuracao.GerarTituloPorDocumentoFiscal.enable(enable);
    _configuracaoFatura.Configuracao.GerarTituloAutomaticamente.enable(enable);
    _configuracaoFatura.Configuracao.GerarFaturaAutomaticaCte.enable(enable);
    _configuracaoFatura.Configuracao.GerarFaturamentoAVista.enable(enable);
    _configuracaoFatura.Configuracao.SomenteOcorrenciasFinalizadoras.enable(enable);
    _configuracaoFatura.Configuracao.FaturarSomenteOcorrenciasFinalizadoras.enable(enable);
    _configuracaoFatura.Configuracao.ArmazenaCanhotoFisicoCTe.enable(enable);
    _configuracaoFatura.Configuracao.AssuntoEmailFatura.enable(true);
    _configuracaoFatura.Configuracao.CorpoEmailFatura.enable(true);
    _configuracaoFatura.Configuracao.GerarBoletoAutomaticamente.enable(enable);
    _configuracaoFatura.Configuracao.EnviarArquivosDescompactados.enable(enable);
    _configuracaoFatura.Configuracao.TipoEnvioFatura.enable(enable);
    _configuracaoFatura.Configuracao.TipoAgrupamentoFatura.enable(enable);
    _configuracaoFatura.Configuracao.DiasSemanaFatura.enable(enable);
    _configuracaoFatura.Configuracao.DiasMesFatura.enable(enable);
    _configuracaoFatura.Configuracao.GerarFaturaPorCte.enable(enable);
}

function BuscarLatitudeLongitude() {
    var lat = parseFloat(String(_pessoa.Latitude.val()).replace(',', '.'));
    var long = parseFloat(String(_pessoa.Longitude.val()).replace(',', '.'));
    if (!isNaN(lat) != 0 && !isNaN(long) != 0) {
        _pessoa.Latitude.val(lat);
        _pessoa.Longitude.val(long);
        var position = new google.maps.LatLng(lat, long);
        if (_marker != null) {
            _marker.setPosition(position);
            _map.panTo(position);
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pessoas.Pessoa.LatitudeOuLongitudeInformadaEstaInvalida);
    }
}

function preencherListasGrupoPessoas() {
    _pessoa.GrupoPessoasAcessoPortal.val(JSON.stringify(_gridGrupoPessoas.BuscarRegistros()));
    _pessoa.FilialCliente.val(JSON.stringify(ObterRegistrosFilialCliente()));
}