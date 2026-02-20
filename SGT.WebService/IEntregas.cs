using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IEntregas" in both code and config file together.
    [ServiceContract]
    public interface IEntregas
    {
        [OperationContract]
        Retorno<bool> ConfirmarEntregaPedido(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, string dataEntrega, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint pontoEntrega);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>> BuscarEntregasRealizadasPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarEntregaRealizadasPendentesIntegracao(int ProtocoloEntrega);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.EDI.AGRO.AGRO> BuscarDadosColetaAGRO(int protocoloCarga);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.EDI.GTA.SUINO> BuscarDadosColetaSUINO(int protocoloCarga);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.EDI.GTA.AVES> BuscarDadosColetaAVES(int protocoloCarga);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaColetaEntrega>> BuscarOcorrenciasColetaEntregaPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarOcorrenciaColetaEntregaPendenteIntegracao(int protocoloOcorrenciaColetaEntrega);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Entrega.SituacaoNotaFiscalCarga> ConsultaSituacaoNotaFiscal(Dominio.ObjetosDeValor.WebService.Entrega.ConsultaSituacaoNotaFiscal consultaSituacaoNotaFiscal);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>> ConsultarDetalhesEntregaPorProtocoloCarga(int protocoloCarga);

        [OperationContract]
        Retorno<bool> EnviarMensagemChatEntrega(Dominio.ObjetosDeValor.WebService.Entrega.MensagemChat mensagemChat);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.CargaEntrega>> BuscarProtocolosEntregasPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>> ConsultarDetalhesEntregaPorProtocoloEntrega(int protocoloEntrega);

        [OperationContract]
        Retorno<bool> AdicionarOcorrencia(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaEntrega ocorrencia);

        [OperationContract]
        Retorno<bool> EnviarImagensEntrega(Dominio.ObjetosDeValor.WebService.Entrega.EnvioImagemEntrega envioImagemEntrega);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.ImagemEntrega>> BuscarImagensEntregaPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoImagemEntrega(int protocoloImagem);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.ImagemOcorrencia>> BuscarImagensOcorrenciaPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoImagemOcorrencia(int protocoloImagem);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ObterEntregasPorPeriodoResponse>> ObterEntregasPorPeriodo(Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ObterEntregasPorPeriodo obterEntregasPorPeriodo);

        [OperationContract]
        Retorno<bool> AtualizarPrevisaoEntrega(Dominio.ObjetosDeValor.Embarcador.ControleEntrega.AtualizarPrevisaoEntrega atualizarPrevisaoEntrega);

        //[OperationContract]
        //Retorno<bool> receberEventoFerroviario(Dominio.ObjetosDeValor.Embarcador.Ferroviario.EventoFerroviario eventoFerroviario);

        //[OperationContract]
        //Retorno<bool> EventoFerroviarioMRS(string TEventoFerroviario);
    }

}
