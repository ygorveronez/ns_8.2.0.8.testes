var EnumMonitoramentoStatusViagemTipoRegraHelper = function () {
    this.Nenhum = null;
    this.Todos = 0;
    this.SemViagem = 1;
    this.EmViagem = 2;
    this.Retornando = 3;
    this.Concluida = 4;
    this.DeslocamentoParaPlanta = 5;
    this.AguardandoHorarioCarregamento = 6;
    this.AguardandoCarregamento = 7;
    this.EmCarregamento = 8;
    this.EmLiberacao = 9;
    this.Transito = 10;
    this.AguardandoHorarioDescarga = 11;
    this.AguardandoDescarga = 12;
    this.Descarga = 13;
    this.DescargaFinalizada = 14;
    this.DeslocamentoParaColetarEquipamento = 15;
    this.DeslocamentoComEquipamentoParaPlanta = 16;
    this.DeslocamentoComEquipamentoECargaParaEntrega = 17;
    this.EmColeta = 18;
    this.Cancelada = 19;
    this.EmParqueamento = 20;
    this.EmFronteira = 21;
};

EnumMonitoramentoStatusViagemTipoRegraHelper.prototype = {
    obterOpcoes: function () {
        return [{
            value: this.SemViagem,
            description: "Sem viagem",
            text: this.SemViagem + " - Sem viagem: Não foi iniciado o monitoramento.",
        }, {
            value: this.EmViagem,
            description: "Em viagem",
            text: this.EmViagem + " - Em viagem: O monitoramento foi iniciado e encontra-se em viagem.",
        }, {
            value: this.Retornando,
            description: "Retornando",
            text: this.Retornando + " - Retornando: Retorno do veículo para a origem após finalizar um monitoramento prévio.",
        }, {
            value: this.Concluida,
            description: "Concluída",
            text: this.Concluida + " - Concluída: O monitoramento foi finalizado e a viagem concluída.",
        }, {
            value: this.DeslocamentoParaPlanta,
            description: "Deslocamento para a planta",
            text: this.DeslocamentoParaPlanta + " - Deslocamento para a planta: Iniciou o monitoramento, não chegou na planta da origem para carregar.",
        }, {
            value: this.AguardandoHorarioCarregamento,
            description: "Aguardando horário de carregamento",
            text: this.AguardandoHorarioCarregamento + " - Aguardando horário de carregamento: Chegou adiantado na origem para carregar e não está na área de carregamento.",
        }, {
            value: this.AguardandoCarregamento,
            description: "Aguardando carregamento",
            text: this.AguardandoCarregamento + " - Aguardando carregamento: Está na origem, após o horário agendado e não está na área de carregamento.",
        }, {
            value: this.EmCarregamento,
            description: "Em carregamento",
            text: this.EmCarregamento + " - Em carregamento: Está na origem e dentro da área de carregamento.",
        }, {
            value: this.EmLiberacao,
            description: "Em liberação",
            text: this.EmLiberacao + " - Em liberação: Está na origem, já entrou e saiu da área de carregamento.",
        }, {
            value: this.Transito,
            description: "Trânsito",
            text: this.Transito + " - Trânsito: Viagem iniciada, monitoramento em andamento, saiu da origem tendo passado pelo carregamento.",
        }, {
            value: this.AguardandoHorarioDescarga,
            description: "Aguardando horário de descarga",
            text: this.AguardandoHorarioDescarga + " - Aguardando horário de descarga: Chegou adiantado no destino para descarregar e não está na área de descarregamento.",
        }, {
            value: this.AguardandoDescarga,
            description: "Aguardando descarga",
            text: this.AguardandoDescarga + " - Aguardando descarga: Está no destino, após o horário agendado e não está na área de descarregamento.",
        }, {
            value: this.Descarga,
            description: "Descarga",
            text: this.Descarga + " - Descarga: Está no destino, está na área de descarregamento caso exista.",
        }, {
            value: this.DescargaFinalizada,
            description: "Descarga finalizada",
            text: this.DescargaFinalizada + " - Descarga finalizada: Saiu da área de descarga e ainda está no destino.",
        }, {
            value: this.DeslocamentoParaColetarEquipamento,
            description: "Deslocamento para o porto para coletar container",
            text: this.DeslocamentoParaColetarEquipamento + " - Deslocamento para coletar o equipamento: Está se deslocando em direção à coleta do equipamento.",
        }, {
            value: this.DeslocamentoComEquipamentoParaPlanta,
            description: "Deslocamento com o container até a planta para coleta da carga",
            text: this.DeslocamentoComEquipamentoParaPlanta + " - Deslocamento com o equipamento até a planta para coleta da carga: Já coletou o equipamento e está se deslocando em direção a planta para carregamento.",
        }, {
            value: this.DeslocamentoComEquipamentoECargaParaEntrega,
            description: "Deslocamento com a carga em container para entrega",
            text: this.DeslocamentoComEquipamentoECargaParaEntrega + " - Deslocamento com equipamento e carga para entrega: O equipamento e a carga foram coletados e está se deslocando para a entrega.",
        }, {
            value: this.EmColeta,
            description: "Está em alguns dos destinos para coleta",
            text: this.EmColeta + " - Está em alguns dos destinos para coleta: Está dentro da área de algum dos desinos marcados como coleta sem subárea de descarregamento.",
        }, {
            value: this.EmParqueamento,
            description: "Está em local de Parqueamento",
            text: this.EmParqueamento + " - Está em local de parqueamento: Está dentro da área de algum dos desinos marcados como Parqueamento.",
        }, {
            value: this.EmFronteira,
            description: "Está em local de Fronteira",
            text: this.EmFronteira + " - Está em local de fronteira: Está dentro da área de algum dos pontos de passagens marcados como fronteira.",
        }, {
            value: this.Cancelada,
            description: "Cancelada",
            text: this.Cancelada + " - O monitoramento foi cancelado.",
        },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesCadastroOpcional: function () {
        return [{ text: "Nenhum", value: this.Nenhum }].concat(this.obterOpcoes());
    },
    obterDescricao: function (tipo) {
        lista = this.obterOpcoes();
        for (var i = 0; i < lista.length; i++) {
            if (lista[i].value == tipo)
                return lista[i].text;
        }
    },
    obterDescricaoLonga: function (tipo) {
        lista = this.obterOpcoes();
        for (var i = 0; i < lista.length; i++) {
            if (lista[i].value == tipo)
                return lista[i].description;
        }
    }
}

var EnumMonitoramentoStatusViagemTipoRegra = Object.freeze(new EnumMonitoramentoStatusViagemTipoRegraHelper());