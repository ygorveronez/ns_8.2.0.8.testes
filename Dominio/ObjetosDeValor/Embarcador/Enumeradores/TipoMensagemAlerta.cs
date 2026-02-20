namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoMensagemAlerta
    {
        AlteracaoDadosPreCarga = 1,
        AlteracaoOrdemEmbarqueNaoIntegrada = 2,
        CargaSemProdutos = 3,
        ImportacaoCargaAtrasada = 4,
        ClienteSemLocalidade = 5,
        CargaSemVeiculoInformado = 6,
        CargaNaoAgendada = 7,
        VeiculoSemRegistroChegada = 8,
        CargaSemConfirmacaoMotorista = 9,
        CargaRecusadaMotorista = 10,
        CargaSemInformacaoContainer = 11,
        AlteracaoPedidos = 12,
        NaoPodeHerdarDadosTransporte = 13,
        ProblemaComTipoOperacao = 14,
        ProblemaIntegracaoFrete = 15,
        GerenciadoraRisco = 16,
        NotaVendaNaoRecebida = 17,
        AjusteTabelaFreteCliente = 18,
        ContratoFreteTransportadorNaoAprovado = 19,
        ProblemaEmissaoCarga = 20,
        DivergenciaValorLimiteApolice = 21,
        CargaSemCargaOrganizacaoVinculada = 22,
        CargaSemRegraAutorizacaoTolerenciaPesagem = 23,
        CargaAguardandoAprovacaoPesagem = 24,
        CargaSemSaldoContratoFreteCliente = 25,
        CargaAguardandoDesbloqueio = 26,
        ContratoFreteClienteFechado = 27,
        ProblemaConfirmarEnvioDosDocumentos = 28,
        ProblemaNaCriacaoChamado = 29,
        ProblemaRoterizacao = 30,
        CargaPedidoSemPacote = 31,
        ProblemaConsultaTagValePedagioVeiculo = 32,
        ProblemaCarga = 33,
        CargaSemValorFreteOperador = 34,
    }

    public static class TipoMensagemAlertaHelper
    {
        public static string ObterTituloMensagem(this TipoMensagemAlerta tipo)
        {
            switch (tipo)
            {
                case TipoMensagemAlerta.AjusteTabelaFreteCliente: return "Ajuste de Valores da Tabela de Frete";
                case TipoMensagemAlerta.AlteracaoDadosPreCarga: return "Alterações da Pré Carga";
                case TipoMensagemAlerta.AlteracaoOrdemEmbarqueNaoIntegrada: return "Alterações de Ordem de Embarque não integradas";
                case TipoMensagemAlerta.AlteracaoPedidos: return "Alterações de Pedidos";
                case TipoMensagemAlerta.CargaSemProdutos: return "Carga sem produtos";
                case TipoMensagemAlerta.ImportacaoCargaAtrasada: return "Importação Atrasada";
                case TipoMensagemAlerta.ClienteSemLocalidade: return "Cliente sem Localidade";
                case TipoMensagemAlerta.CargaSemVeiculoInformado: return "Carga sem Veículo Informado";
                case TipoMensagemAlerta.CargaNaoAgendada: return "Carga não Agendada";
                case TipoMensagemAlerta.VeiculoSemRegistroChegada: return "Veículo sem Registro de Chegada";
                case TipoMensagemAlerta.CargaSemConfirmacaoMotorista: return "Carga sem confirmação do motorista";
                case TipoMensagemAlerta.CargaRecusadaMotorista: return "Carga recusada pelo motorista";
                case TipoMensagemAlerta.CargaSemInformacaoContainer: return "Necessário vínculo de container";
                case TipoMensagemAlerta.NaoPodeHerdarDadosTransporte: return "Carga que não pode herdar dados de transporte";
                case TipoMensagemAlerta.ProblemaComTipoOperacao: return "Não foi encontrado um tipo de operação";
                case TipoMensagemAlerta.ProblemaEmissaoCarga: return "Problema na Emissão";
                case TipoMensagemAlerta.ProblemaIntegracaoFrete: return "Problema ao Integrar Frete";
                case TipoMensagemAlerta.GerenciadoraRisco: return "Gerenciadora de Risco";
                case TipoMensagemAlerta.NotaVendaNaoRecebida: return "Nota Venda não recebida";
                case TipoMensagemAlerta.ContratoFreteTransportadorNaoAprovado: return "Contrato de Frete do Transportador não Aprovado";
                case TipoMensagemAlerta.DivergenciaValorLimiteApolice: return "Divergência no valor limite da apólice";
                case TipoMensagemAlerta.CargaSemCargaOrganizacaoVinculada: return "Carga não tem nenhuma pré carga vinculada";
                case TipoMensagemAlerta.CargaSemRegraAutorizacaoTolerenciaPesagem: return "Carga sem regra de aprovação pesagem";
                case TipoMensagemAlerta.CargaAguardandoAprovacaoPesagem: return "Carga possui aprovação pendente";
                case TipoMensagemAlerta.CargaSemSaldoContratoFreteCliente: return "Carga está sem saldo no contrato de frete cliente";
                case TipoMensagemAlerta.CargaAguardandoDesbloqueio: return "Carga aguardando desbloqueio";
                case TipoMensagemAlerta.ContratoFreteClienteFechado: return "O contrato de frete cliente está fechado";
                case TipoMensagemAlerta.ProblemaNaCriacaoChamado: return "Problema na criação do chamado";
                case TipoMensagemAlerta.ProblemaConfirmarEnvioDosDocumentos: return "Problema na confirmação dos documentos";
                case TipoMensagemAlerta.ProblemaRoterizacao: return "Problema na Roterização, verifique o cadastro dos clientes";
                case TipoMensagemAlerta.CargaPedidoSemPacote: return "Carga Pedido sem Pacotes";
                case TipoMensagemAlerta.ProblemaConsultaTagValePedagioVeiculo: return "Problema na consulta da Tag do Vale Pedágio do Veículo";
                case TipoMensagemAlerta.ProblemaCarga: return "Problema na Carga";
                case TipoMensagemAlerta.CargaSemValorFreteOperador: return "Carga sem valor de frete";
                default: return string.Empty;
            }
        }

        public static bool UtilizarConfirmacaoMensagem(this TipoMensagemAlerta tipo)
        {
            return (
                (tipo == TipoMensagemAlerta.AlteracaoDadosPreCarga) ||
                (tipo == TipoMensagemAlerta.AlteracaoPedidos) ||
                (tipo == TipoMensagemAlerta.CargaAguardandoDesbloqueio) ||
                (tipo == TipoMensagemAlerta.ProblemaConfirmarEnvioDosDocumentos)
            );
        }
    }
}