/*****MAPEAMENTO******/
var __Auditoria = {
    Entidade: "",
    Codigo: 0
};
var __DetalheAuditoria = {
    Codigo: 0,
    Dicionario: 0
};

var __GridAuditoria;
var __GridDetalheAuditoria;



/*****LOADER******/
$(document).ready(function LoadAuditoria() {
    $("#divAuditoriaGlobal").on('click', 'a', __AbrirModalAuditoria);
});

function CarregarGridAuditoria() {
    var opcoes = [
        {
            Descricao: "Detalhes", Evento: function (data) {
                __DetalheAuditoria.Codigo = data.data.Codigo;
                __AbrirModalDetalhesAuditoria();
            }
        }
    ];
    
    CriarGridView("/Auditoria/Pesquisa?callback=?", __Auditoria, "tbl_auditoria_table", "tbl_auditoria", "tbl_paginacao_auditoria", opcoes, [0], null);
}

function CarregarGridDetalheAuditoria() {
    CriarGridView("/Auditoria/PesquisaComposAlterados?callback=?", __DetalheAuditoria, "tbl_detalhes_auditoria_table", "tbl_detalhes_auditoria", "tbl_paginacao_detalhes_auditoria", null, [0], null);
}


/*****MODAL******/
function __AbrirModalAuditoria(e, callback) {
    if (e && e.preventDefault)
        e.preventDefault();

    CarregarGridAuditoria();
    $("#divModalAuditoria").modal({ show: true, keyboard: true, backdrop: 'static' });
    if (callback) {
        $("#divModalAuditoria").one("hidden.bs.modal", function (e) {
            callback(e);
        });
    }
}

function __AbrirModalDetalhesAuditoria() {
    CarregarGridDetalheAuditoria();
    $("#divModalDetalhesAuditoria").modal({ show: true, keyboard: true, backdrop: true });
}


/*****FUNCOES******/
function __GerarDicionario(dicionario, asString) {
    var _map = {};

    // Se vier um dicionario, sobrepoe 
    if (!$.isArray(dicionario)) {
        for (var i in dicionario) {
            _map[i] = dicionario[i].trim();

        }
    }

    // Transforma o obj em mapeamento pro servidor (um array de objetos)
    //var _mapeamendo = [];
    //for (var k in _map)
    //    _mapeamendo.push({
    //        Nome: k,
    //        Descricao: _map[k]
    //    });

    // Retorna como string
    if (asString)
        return JSON.stringify(_map);
    else
        return _map;
}


/*****CONSTRUTORES******/
function HeaderAuditoria(entidade, dicionario) {
    /**
     * Entidade: string que contem o nome da entidade mapeada no servidor
     * PropCodigo: Nome da propriedade do objeto knockout para controlar código único (Valor Padrão: Codigo)
     * Dicionario: Objeto com "palavra" "valor" usado para melhor visualização da auditoria "De" "Para" (Parâmetro Opcional)
     *
     * Descrição:
     * Esse construtor é utilizado em telas que possuem CRUD simples
     * É a função autonoma que seta os valores relacionados e exibe o botão para 
     * visualizar as auditorias. 
     * 
     * A abertura do modal é controlado pelo acionamento do botão
     */
    if (!PermiteAuditar())
        return false;
    
    __Auditoria.Entidade = entidade;

    var stringDicionario = __GerarDicionario(dicionario || {}, true);
    __DetalheAuditoria.Dicionario = stringDicionario;

    $("#divAuditoriaGlobal").show();
}

function HeaderAuditoriaCodigo(novoCodigo) {
    if (arguments.length == 0)
        novoCodigo = 0;
    __Auditoria.Codigo = novoCodigo;
}


//function OpcaoAuditoria(entidade, propCodigo, dicionario) {
//    /**
//    * Não usar
//    */
//    if (!_CONFIGURACAO_TMS.PermiteAuditar)
//        return function () { };

//    propCodigo = propCodigo || "Codigo";

//    return function (dataRow) {
//        if ("data" in dataRow)
//            dataRow = dataRow.data;

//        if (!(propCodigo in dataRow))
//            return false;

//        // Salva Entidade e Ko do header para setar quando fechar o modal do grid
//        var _bk_Entidade = __Auditoria.Entidade.val();
//        var _bk_Codigo = __Auditoria.Codigo.val();
//        var _bk_knout = __Auditoria.knout.val();
//        var _bk_Dicionario = __DetalheAuditoria.Dicionario.val();

//        __Auditoria.Entidade.val(entidade);
//        __Auditoria.knout.val({});
//        __Auditoria.Codigo.val(dataRow[propCodigo]);

//        var stringDicionario = __GerarDicionario(dicionario || {}, true);
//        __DetalheAuditoria.Dicionario.val(stringDicionario);

//        __AbrirModalAuditoria(null, function () {
//            __Auditoria.Entidade.val(_bk_Entidade);
//            __Auditoria.Codigo.val(_bk_Codigo);
//            __Auditoria.knout.val(_bk_knout);
//            __DetalheAuditoria.Dicionario.val(_bk_Dicionario);
//        });
//    }
//}

function PermiteAuditar() {
    return $(_CONFIGURACAO_AUDITORIA).val() == "true";
}

function VisibilidadeOpcaoAuditoria() {
    return PermiteAuditar();
}