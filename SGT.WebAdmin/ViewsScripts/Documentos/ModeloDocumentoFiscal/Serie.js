//*******MAPEAMENTO KNOUCKOUT*******

var _gridSerieModeloDocumentoFiscal;
var _serieModeloDocumentoFiscal;

var SerieModeloDocumentoFiscal = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa/Filial:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Serie = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Série:", idBtnSearch: guid(), required: true, enable: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarSerieClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(false) });
}

//*******EVENTOS*******

function LoadSerieModeloDocumentoFiscal() {

    $("#knockoutCadastroModeloDocumentoFiscal").append($("#knockoutSerie"));

    _serieModeloDocumentoFiscal = new SerieModeloDocumentoFiscal();
    KoBindings(_serieModeloDocumentoFiscal, "knockoutSerie");

    new BuscarEmpresa(_serieModeloDocumentoFiscal.Empresa, RetornoConsultaEmpresa);
    new BuscarSerieEmpresa(_serieModeloDocumentoFiscal.Serie, null, null, null, _serieModeloDocumentoFiscal.Empresa, EnumTipoSerie.OutrosDocumentos);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirSerieClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Empresa", title: "Empresa/Filial", width: "60%" },
        { data: "Serie", title: "Série", width: "30%" }
    ];

    _gridSerieModeloDocumentoFiscal = new BasicDataTable(_serieModeloDocumentoFiscal.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridSerie();
}

function RetornoConsultaEmpresa(empresa) {
    _serieModeloDocumentoFiscal.Empresa.val(empresa.RazaoSocial);
    _serieModeloDocumentoFiscal.Empresa.codEntity(empresa.Codigo);
    LimparCampoEntity(_serieModeloDocumentoFiscal.Serie);
    _serieModeloDocumentoFiscal.Serie.enable(true);
}

function RecarregarGridSerie() {

    var data = new Array();

    $.each(_modeloDocumentoFiscal.Series.list, function (i, serie) {
        var serieGrid = new Object();

        //if (serie.Serie.codEntity === undefined) {
        //    serieGrid.Codigo = serie.Serie.Codigo;
        //    serieGrid.Empresa = serie.Empresa;
        //    serieGrid.Serie = serie.Serie.Descricao;
        //}
        //else {
        serieGrid.Codigo = serie.Serie.codEntity;
        serieGrid.Empresa = serie.Empresa.val;
        serieGrid.Serie = serie.Serie.val;
        //}

        data.push(serieGrid);
    });

    _gridSerieModeloDocumentoFiscal.CarregarGrid(data);
}


function ExcluirSerieClick(data) {
    for (var i = 0; i < _modeloDocumentoFiscal.Series.list.length; i++) {
        //if (data.Serie.codEntity == _modeloDocumentoFiscal.Series.list[i].Codigo.val) {
        if (data.Codigo == _modeloDocumentoFiscal.Series.list[i].Serie.codEntity) {
            _modeloDocumentoFiscal.Series.list.splice(i, 1);
            break;
        }
    }

    RecarregarGridSerie();
}

function AdicionarSerieClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_serieModeloDocumentoFiscal);

    if (valido) {

        for (var i = 0; i < _modeloDocumentoFiscal.Series.list.length; i++) {
            if (_serieModeloDocumentoFiscal.Serie.codEntity() == _modeloDocumentoFiscal.Series.list[i].Serie.codEntity) {
                exibirMensagem(tipoMensagem.aviso, "Série já existente", "Esta série já está cadastrada.");
                return;
            }
        }

        _modeloDocumentoFiscal.Series.list.push(SalvarListEntity(_serieModeloDocumentoFiscal));

        RecarregarGridSerie();
        LimparCamposSerie();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function LimparCamposSerie() {
    LimparCampos(_serieModeloDocumentoFiscal);
    _serieModeloDocumentoFiscal.Serie.enable(false);
}