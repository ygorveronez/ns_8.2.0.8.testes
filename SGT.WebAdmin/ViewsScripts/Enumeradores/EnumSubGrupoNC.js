var EnumSubGrupoNCHelper = function () {
    this.Todos = null;
    this.NaoSelecionado = 0;
    this.DTOCCancelada = 1;
    this.EtapaDivergente = 2;
    this.NotaFiscalCancelada = 3;
    this.CNPJDivergente = 4;
    this.CodigoNodeDivergente = 5;
    this.TransportadoraDivergente = 6;
    this.PedidoNaoEncontrado = 7;
    this.CentroSAPDivergente = 8;
    this.DivergenciaCarregamento = 9;
    this.ChaveamentoPendente = 10;
    this.DeParaNaoLocalizado = 11;
    this.StatusInativo = 12;
    this.ItemNaoExpandido = 13;
    this.PedidoSemSaldo = 14;
    this.PesoNaoLocalizado = 15;
    this.CapacidadeExcedida = 16;
};

EnumSubGrupoNCHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "DT/OC Cancelada", value: this.DTOCCancelada },
            { text: "Etapa Divergente", value: this.EtapaDivergente },
            { text: "Nota Fiscal Cancelada", value: this.NotaFiscalCancelada },
            { text: "CNPJ Divergente", value: this.CNPJDivergente },
            { text: "Código Node Divergente", value: this.CodigoNodeDivergente },
            { text: "Transportadora Divergente", value: this.TransportadoraDivergente },
            { text: "Pedido não Encontrado", value: this.PedidoNaoEncontrado },
            { text: "Centro SAP Divergente", value: this.CentroSAPDivergente },
            { text: "Divergência Carregamento", value: this.DivergenciaCarregamento },
            { text: "Chaveamento Pendente", value: this.ChaveamentoPendente },
            { text: "De/Para não Localizado", value: this.DeParaNaoLocalizado },
            { text: "Status Inativo", value: this.StatusInativo },
            { text: "Item não Expandido", value: this.ItemNaoExpandido },
            { text: "Pedido sem Saldo", value: this.PedidoSemSaldo },
            { text: "Peso não Localizado", value: this.PesoNaoLocalizado },
            { text: "Capacidade Excedida", value: this.CapacidadeExcedida },
        ];
    },
    obterOpcoesNaoSelecionado: function () {
        return [
            { text: "DT/OC Cancelada", value: this.DTOCCancelada },
            { text: "Etapa Divergente", value: this.EtapaDivergente },
            { text: "Nota Fiscal Cancelada", value: this.NotaFiscalCancelada },
            { text: "CNPJ Divergente", value: this.CNPJDivergente },
            { text: "Código Node Divergente", value: this.CodigoNodeDivergente },
            { text: "Transportadora Divergente", value: this.TransportadoraDivergente },
            { text: "Pedido não Encontrado", value: this.PedidoNaoEncontrado },
            { text: "Centro SAP Divergente", value: this.CentroSAPDivergente },
            { text: "Divergência Carregamento", value: this.DivergenciaCarregamento },
            { text: "Chaveamento Pendente", value: this.ChaveamentoPendente },
            { text: "De/Para não Localizado", value: this.DeParaNaoLocalizado },
            { text: "Status Inativo", value: this.StatusInativo },
            { text: "Item não Expandido", value: this.ItemNaoExpandido },
            { text: "Pedido sem Saldo", value: this.PedidoSemSaldo },
            { text: "Peso não Localizado", value: this.PesoNaoLocalizado },
            { text: "Capacidade Excedida", value: this.CapacidadeExcedida },
            { text: "Nenhum", value: this.NaoSelecionado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSubGrupoNC = Object.freeze(new EnumSubGrupoNCHelper());