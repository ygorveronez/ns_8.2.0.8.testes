namespace Servicos.WebServiceCarrefour.Conversores.Carga
{
    public sealed class ProtocolosConverter
    {
        public Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.Protocolos Converter(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolosConverter)
        {
            if (protocolosConverter == null)
                return null;

            Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.Protocolos protocolos = new Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.Protocolos()
            {
                Destinatario = null,
                Remetente = null,
                ParametroIdentificacaoCliente = protocolosConverter.ParametroIdentificacaoCliente,
                protocoloIntegracaoCarga = protocolosConverter.protocoloIntegracaoCarga,
                protocoloIntegracaoPedido = protocolosConverter.protocoloIntegracaoPedido
            };

            return protocolos;
        }
    }
}
