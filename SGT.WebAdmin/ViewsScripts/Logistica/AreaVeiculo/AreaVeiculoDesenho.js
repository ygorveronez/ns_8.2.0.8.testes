var _veiculoDesenho = null;

function Desenho(idCanvas) {
    var canvas = null
    
    var inicializar = function () {
        canvas = new fabric.Canvas(idCanvas);
        canvas.hoverCursor = 'pointer';
        canvas.setHeight(500);
        canvas.setWidth(1024);
    }();

    this.adicionarObjeto = function (id, obj) {
        var o = JSON.parse(obj)

        var componenteTexto = new fabric.Textbox(o.text,
            {
                left: o.left,
                top: o.top,
                width: o.width,
                height: o.height,
                textAlign: 'center',
                fill: '#fff',
                backgroundColor: '#77f',
                editable: false,
                angle: o.angle,
                id: id
            });

        canvas.add(componenteTexto);
    };

    this.adicionar = function (id, texto) {

        var componenteTexto = new fabric.Textbox(texto,
            {
                left: 10,
                top: 50,
                width: 50,
                height: 70,
                textAlign: 'center',
                fill: '#fff',
                backgroundColor: '#77f',
                editable: false,
                id: id
            });

        canvas.add(componenteTexto);
        
    }

    var redesenhar = function () {
        canvas.renderAll();
    }

    this.editar = function (id, texto) {

        var objs = canvas.getObjects();

        for (var i = 0; i < objs.length; i++) {
            if (objs[i].id == id) {
                objs[i].text = texto;
                redesenhar();
            }
        }
    }

    this.excluir = function (id) {
        var objs = canvas.getObjects();

        for (var i = 0; i < objs.length; i++) {

            if (objs[i].id == id) {
                canvas.remove(objs[i]);
                
                return;
            }
        }
    }

    this.obterJson = function() {
        return JSON.stringify(canvas);
    }

    this.obterJsonPorID = function (id) {
        var objs = canvas.getObjects();

        for (var i = 0; i < objs.length; i++) {

            if (objs[i].id == id) {
                return JSON.stringify(objs[i]);
            }
        }

        return null;
    }


    this.limpar = function() {
        canvas.clear();
    }

    this.carregarJson = function (json) {
        canvas.loadFromJSON(json, canvas.renderAll.bind(canvas));
    }
};


function limparCamposAreaVeiculoDesenho() {
    _veiculoDesenho.clear();
}

function limparCamposAreaVeiculoDesenho() {
    _veiculoDesenho.limpar();
}

function loadAreaVeiculoDesenho() {
    if (_veiculoDesenho == null)
        _veiculoDesenho = new Desenho('canvas');

    _veiculoDesenho.limpar();
};

function carregarListaDesenho(lista) {
    for (var i = 0; i < lista.length; i++) {
        if ((lista[i].Desenho == "") || (lista[i].Desenho == null)) 
            _veiculoDesenho.adicionar(lista[i].Codigo, lista[i].Descricao);
        else
            _veiculoDesenho.adicionarObjeto(lista[i].Codigo, lista[i].Desenho);

    }
}

function preencherAreaVeiculoDesenho(lista) {
    _veiculoDesenho.limpar();
    carregarListaDesenho(lista);
}

function adicionarAreaVeiculoDesenho(area) {
    _veiculoDesenho.adicionar(area.Codigo, area.Descricao);
}

function editarAreaVeiculoDesenho(area) {
    _veiculoDesenho.editar(area.Codigo, area.Descricao);
}
function excluirAreaVeiculoDesenho(codigo) {
    _veiculoDesenho.excluir(codigo);
}

function obterJsonAreaVeiculoDesenho() {
    return _veiculoDesenho.obterJson();
}

function obterJsonAreaVeiculoDesenhoPorID(id) {
    return _veiculoDesenho.obterJsonPorID(id);
}