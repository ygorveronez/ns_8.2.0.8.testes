using System.Collections.Generic;
using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IEmpresa" in both code and config file together.
    [ServiceContract]
    public interface IEmpresa
    {
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.TabelaPosto>> BuscarTabelaPostoPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoTabelaPosto(int protocolo);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Equipamento>> BuscarEquipamentosPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoEquipamento(int protocolo);

        [OperationContract]
        Retorno<int> SalvarOrdemCompra(Dominio.ObjetosDeValor.Embarcador.Financeiro.OrdemCompra ordemCompra);

        [OperationContract]
        Retorno<bool> InformarMacroVeiculo(Dominio.ObjetosDeValor.Embarcador.Frota.Macro macroIntegracao, Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao);

        [OperationContract]
        Retorno<bool> SalvarVeiculo(Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao);

        [OperationContract]
        Retorno<string> SalvarVeiculoProtocolo(Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao);

        [OperationContract]
        Retorno<bool> SalvarMotorista(Dominio.ObjetosDeValor.Embarcador.Carga.Motorista motoristaIntegracao);

        [OperationContract]
        Retorno<string> SalvarMotoristaProtocolo(Dominio.ObjetosDeValor.Embarcador.Carga.Motorista motoristaIntegracao);

        [OperationContract]
        Retorno<bool> SalvarVeiculoValePedagioCIOT(Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa empresa, Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculo, Dominio.ObjetosDeValor.MDFe.ValePedagio valePedagio, Dominio.ObjetosDeValor.MDFe.CIOT ciot);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa> ConsultarTransportador(string cnpj);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>> BuscarTransportadores(int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>> BuscarVeiculosPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>> BuscarMotoristasPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>> BuscarTransportadoresPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoVeiculo(int protocolo);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoMotorista(int protocolo);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoTransportador(string cnpj);

        [OperationContract]
        Retorno<bool> SalvarTransportador(Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa transportador);

        [OperationContract]
        Retorno<bool> SalvarFeriado(Dominio.ObjetosDeValor.Embarcador.Configuracoes.Feriado feriado);

        [OperationContract]
        Retorno<bool> InformarDadosBancariosTransportador(string cnpjTransportador, string codigoBanco, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco tipoContaBanco, string numeroConta, string agencia, string digitoAgencia);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.ConsultaAbastecimento>> BuscarAbastecimentosPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoAbastecimento(int protocolo);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.TipoMovimento>> BuscarTipoMovimento(int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Navio>> BuscarNaviosPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoNavio(int protocolo);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Viagem>> BuscarViagemPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoViagem(int protocolo);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Porto>> BuscarPortoPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoPorto(int protocolo);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoContainer>> BuscarTipoContainerIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoTipoContainer(int protocolo);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto>> BuscarTerminalPortoPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoTerminalPorto(int protocolo);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>> BuscarProdutoPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoProduto(int protocolo);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.OrdemServico>> BuscarOrdemServicoFinalizacaPendenteIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoOrdemServicoFinalizada(int protocolo);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>> BuscarTransportadoresPendentesIntegracaoERP(int? quantidade);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoTransportadorIntegracaoERP(List<int> protocolos);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>> BuscarVeiculosPendentesIntegracaoERP(int? quantidade);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoVeiculoERP(List<int> protocolos);

        [OperationContract]
        Retorno<bool> SalvarSituacaoColaborador(Dominio.ObjetosDeValor.Embarcador.Carga.SituacaoColaboradorIntegracaoWS situacaoColaboradorIntegracao);

        [OperationContract]
        Retorno<bool> InformarDisponibilidadeFrota(Dominio.ObjetosDeValor.WebService.Logistica.Veiculo.FilaCarregamentoVeiculo FilaCarregamentoVeiculo);

    }
}
