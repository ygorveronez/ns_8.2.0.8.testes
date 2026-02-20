/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Enumeradores/EnumTipoCliente.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />

var _datasPreferenciasFornecedorCategoria;

var DatasPreferenciaisFornecedorCategoria = function () {
    this.TipoFornecedor = PropertyEntity({ val: ko.observable(EnumTipoCliente.Pessoa), type: types.local, options: EnumTipoCliente.obterOpcoesSemCategoria(), def: EnumTipoCliente.Pessoa, text: "Tipo Fornecedor: ", required: false });
    this.Fornecedor = PropertyEntity({ text: "*Fornecedor:", val: ko.observable(""), type: types.entity, required: true, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoFornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor:", required:false, idBtnSearch: guid(), visible: ko.observable(false) });
    this.Categoria = PropertyEntity({ text: "*Categoria:", val: ko.observable(""), type: types.entity, required: true, codEntity: ko.observable(0), idBtnSearch: guid() });
    
    this.TipoFornecedor.val.subscribe(tipoFornecedorChange);

    this.Adicionar = PropertyEntity({ text: "Adicionar", idBtnSearch: guid(), eventClick: adicionarDescargaFornecedorCategoriaClick });
}

function loadDatasPreferenciasDescargaFornecedorCategoria() {
    _datasPreferenciasFornecedorCategoria = new DatasPreferenciaisFornecedorCategoria();

    KoBindings(_datasPreferenciasFornecedorCategoria, "knockoutAdicionarDataPreferencialDescarga");
    
    new BuscarClientes(_datasPreferenciasFornecedorCategoria.Fornecedor);
    new BuscarGruposPessoas(_datasPreferenciasFornecedorCategoria.GrupoFornecedor);
    new BuscarProdutos(_datasPreferenciasFornecedorCategoria.Categoria);
}

function adicionarDescargaFornecedorCategoriaClick() {
    if (!ValidarCamposObrigatorios(_datasPreferenciasFornecedorCategoria)) {
        exibirMensagem(tipoMensagem.atencao, "Falha", "Preencha os campos obrigatórios.");
        return;
    }
    
    _datasPreferenciaisDescarga.ListaFornecedorCategoria.val().push({
        Codigo: guid(),
        CodigoFornecedor: _datasPreferenciasFornecedorCategoria.Fornecedor.codEntity(),
        CodigoGrupoFornecedor: _datasPreferenciasFornecedorCategoria.GrupoFornecedor.codEntity(),
        CodigoCategoria: _datasPreferenciasFornecedorCategoria.Categoria.codEntity(),
        Fornecedor: _datasPreferenciasFornecedorCategoria.Fornecedor.val(),
        GrupoFornecedor: _datasPreferenciasFornecedorCategoria.GrupoFornecedor.val(),
        Categoria: _datasPreferenciasFornecedorCategoria.Categoria.val()
    });
    
    recarregarGridDatasPreferenciaisDescarga();

    Global.fecharModal("modalAdicionarDatasPreferenciasDescarga");
}

function tipoFornecedorChange() {
    if (_datasPreferenciasFornecedorCategoria.TipoFornecedor.val() == 1)
        setarFornecedor();
    else
        setarGrupoFornecedor();
}

function setarFornecedor() {
    _datasPreferenciasFornecedorCategoria.GrupoFornecedor.required = false;
    _datasPreferenciasFornecedorCategoria.GrupoFornecedor.val("");
    _datasPreferenciasFornecedorCategoria.GrupoFornecedor.codEntity(0);
    _datasPreferenciasFornecedorCategoria.GrupoFornecedor.visible(false);
    _datasPreferenciasFornecedorCategoria.Fornecedor.visible(true);
    _datasPreferenciasFornecedorCategoria.Fornecedor.required = true;
}
function setarGrupoFornecedor() {
    _datasPreferenciasFornecedorCategoria.Fornecedor.required = false;
    _datasPreferenciasFornecedorCategoria.Fornecedor.val("");
    _datasPreferenciasFornecedorCategoria.Fornecedor.codEntity(0);
    _datasPreferenciasFornecedorCategoria.GrupoFornecedor.visible(true);
    _datasPreferenciasFornecedorCategoria.Fornecedor.visible(false);
    _datasPreferenciasFornecedorCategoria.GrupoFornecedor.required = true;
}