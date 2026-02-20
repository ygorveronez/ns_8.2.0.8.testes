/// <reference path="../../../../wwwroot/js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.blockui.js" />
/// <reference path="../../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../Consultas/Empresa.js" />
/// <reference path="../ParametrosOfertas.js" />
/// <reference path="../Constantes.js" />

var _parametrosOfertasDadosOferta;
var _gridParametrosOfertasDadosOferta;

var ParametrosOfertasDadosOferta = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.HoraInicio = PropertyEntity({ text: "*Horário de Início", val: ko.observable(""), def: "", getType: typesKnockout.time, required: true });
    this.HoraTermino = PropertyEntity({ text: "*Horário de Término", val: ko.observable(""), def: "", getType: typesKnockout.time, required: true });
    this.Raio = PropertyEntity({ text: "Raio (metros)", val: ko.observable(""), def: "", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "", maxlength: 9 }, required: true });
    this.DiasSemana = PropertyEntity({ text: "*Dias da Semana", getType: typesKnockout.selectMultiple, val: ko.observable([EnumDiaSemana.Todos]), options: EnumDiaSemana.obterOpcoes(), def: [EnumDiaSemana.Todos], visible: ko.observable(true) });

    this.Grid = PropertyEntity({ type: types.local, id: guid() });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarParametrosOfertasDadosOfertaClick, type: types.event, text: "Adicionar Dados da Oferta", visible: ko.observable(true) });
}

function ValidarDadosOferta(podo, listaPOPODO) {

    if (
        !podo.DiasSemana.length ||
        !podo.HoraInicio ||
        !podo.HoraTermino
    ) {
        return {
            liberado: false,
            mensagem: {
                titulo: MSG_AVISO_TITULO_CAMPOS_OBRIGATORIOS,
                corpo: "Por favor, informe os campos obrigatórios dos Dados da Oferta."
            }
        };
    }

    if (podo.HoraInicio === podo.HoraTermino) {
        return {
            liberado: false,
            mensagem: {
                titulo: MSG_AVISO_TITULO_INCONSISTENCIA,
                corpo: "O horário de início é o mesmo que o horário de término."
            }
        };
    }
    else if (horarioAMaisTardioQueB(podo.HoraInicio, podo.HoraTermino)) {
        return {
            liberado: false,
            mensagem: {
                titulo: MSG_AVISO_TITULO_INCONSISTENCIA,
                corpo: "O horário de início é mais tarde que o horário de término."
            }
        };
    }

    const listaMesmoRaio = listaPOPODO.filter((d) => d.Raio == podo.Raio);

    if (listaMesmoRaio.length > 0) {
        for (const doMesmoRaio of listaMesmoRaio) {
            for (const diaDaSemanaDeMesmoRaio of doMesmoRaio.DiasSemana) {
                if (
                    podo.DiasSemana.includes(diaDaSemanaDeMesmoRaio) &&
                    (horariosDeDadosOfertaAEstaContidoEmB(doMesmoRaio, podo) || horariosDeDadosOfertaAEstaContidoEmB(podo, doMesmoRaio))
                ) {
                    return {
                        liberado: false,
                        mensagem: {
                            titulo: MSG_AVISO_TITULO_INCONSISTENCIA,
                            corpo: `Existe um conflito de horários entre dias da semana com o mesmo raio definido. Verificado: ${EnumDiaSemana.obterOpcoes().find(ds => ds.value == diaDaSemanaDeMesmoRaio).text}`
                        }
                    };
                }
            }
        }
    }

    return {
        liberado: true,
        mensagem: {}
    };
}

function horariosDeDadosOfertaAEstaContidoEmB(a, b) {
    return (a.HoraInicio === b.HoraInicio || horarioAMaisTardioQueB(a.HoraInicio, b.HoraInicio)) && (a.HoraTermino === b.HoraTermino || !horarioAMaisTardioQueB(a.HoraTermino, b.HoraTermino))
}

function horarioAMaisTardioQueB(a, b) {
    const paraMinutos = (tempoString) => {
        const [horas, minutos] = tempoString.split(":").map(Number);

        return horas * 60 + minutos;
    }

    return paraMinutos(a) > paraMinutos(b);
}

function AdicionarParametrosOfertasDadosOfertaClick() {

    let listaPOPODO = _parametrosOfertas.ParametrosOfertasDadosOferta.val();
    const id = listaPOPODO.length + 1

    const podo = {
        Id: id,
        Codigo: 0,
        DiasSemana: _parametrosOfertasDadosOferta.DiasSemana.val(),
        HoraInicio: _parametrosOfertasDadosOferta.HoraInicio.val(),
        HoraTermino: _parametrosOfertasDadosOferta.HoraTermino.val(),
        Raio: _parametrosOfertasDadosOferta.Raio.val() === "" ? 0 : _parametrosOfertasDadosOferta.Raio.val(),
    }

    const validacao = ValidarDadosOferta(podo, listaPOPODO);

    if (!validacao.liberado) {
        exibirMensagem(tipoMensagem.atencao, validacao.mensagem.titulo, validacao.mensagem.corpo);
        return;
    }

    listaPOPODO.push(podo);

    RecarregarGridParametrosOfertasDadosOferta(listaPOPODO);
    LimparCamposParametrosOfertasDadosOferta();
}

function RecarregarGridParametrosOfertasDadosOferta(listaPOPODO) {
    let dadosOfertasPorDiaSemana = [];

    listaPOPODO.forEach(poPODO => {
        poPODO.DiasSemana.forEach(dia => dadosOfertasPorDiaSemana.push({
            Id: poPODO.Id,
            Codigo: poPODO.Codigo,
            DiaSemana: EnumDiaSemana.obterOpcoes().find(ds => ds.value == dia).text,
            HoraInicio: poPODO.HoraInicio,
            HoraTermino: poPODO.HoraTermino,
            Raio: poPODO.Raio,
        }))
    });

    _gridParametrosOfertasDadosOferta.CarregarGrid(dadosOfertasPorDiaSemana);
}

function PreencherParametrosDadosOferta(listaPODOs) {
    listaPODOs = listaPODOs.map((o, i) => {
        o.Id = i;
        return o;
    });

    _parametrosOfertas.ParametrosOfertasDadosOferta.val(listaPODOs);
    RecarregarGridParametrosOfertasDadosOferta(listaPODOs);
}

function LoadParemterosOfertasDadosOferta() {
    _parametrosOfertasDadosOferta = new ParametrosOfertasDadosOferta();
    KoBindings(_parametrosOfertasDadosOferta, "knockoutParametrosOfertasDadosOferta");

    let excluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: function (data) { ExcluirParametrosOfertasDadosOfertaClick(data); }, tamanho: "10", icone: "" };

    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [excluir], tamanho: 10 };

    var header = [
        { data: "Id", visible: false },
        { data: "Codigo", visible: false },
        { data: "DiaSemana", title: "Dias da Semana", width: "25%" },
        { data: "HoraInicio", title: "Horário de Início", width: "25%" },
        { data: "HoraTermino", title: "Horário de Término", width: "25%" },
        { data: "Raio", title: "Raio", width: "25%" },
    ];

    _gridParametrosOfertasDadosOferta = new BasicDataTable(_parametrosOfertasDadosOferta.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridParametrosOfertasDadosOferta.CarregarGrid([]);
}

function ExcluirParametrosOfertasDadosOfertaClick(data) {
    const poPODO = _parametrosOfertas.ParametrosOfertasDadosOferta.val().find(podo => podo.Id == data.Id);
    const listaPOPODO = _parametrosOfertas.ParametrosOfertasDadosOferta.val().filter(podo => podo.Id != data.Id);

    if (poPODO.DiasSemana.length > 1) {
        poPODO.DiasSemana = poPODO.DiasSemana.filter(dia => EnumDiaSemana.obterOpcoes().find(ds => ds.value == dia).text !== data.DiaSemana);
        listaPOPODO.push(poPODO);
    }
    _parametrosOfertas.ParametrosOfertasDadosOferta.val(listaPOPODO);

    RecarregarGridParametrosOfertasDadosOferta(listaPOPODO);
}

function LimparCamposParametrosOfertasDadosOferta(comGrid) {
    LimparCampos(_parametrosOfertasDadosOferta);

    if (comGrid) {
        _gridParametrosOfertasDadosOferta.CarregarGrid(new Array());
    }
}