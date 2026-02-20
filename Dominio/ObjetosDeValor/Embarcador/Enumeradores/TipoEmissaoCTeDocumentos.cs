namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEmissaoCTeDocumentos
    {
        /// <summary>
        /// Não possui o tipo de emissão de documentos informado, se não possuir por padrão é utilizado o tipo 2 para TMS e o tipo 1 para Embarcador
        /// </summary>
        NaoInformado = 0,
        /// <summary>
        /// Emite 1 conhecimento por remetente, destinatario e demais parametros com base nos pedidos
        /// </summary>
        EmitePorPedidoAgrupado = 1,
        /// <summary>
        /// Emite 1 conhecimento por remetente, destinatario e demais parametros com base nas Notas fiscais
        /// </summary>
        EmitePorNotaFiscalAgrupada = 2,
        /// <summary>
        /// Emite 1 conhecimento por nota fiscal
        /// </summary>
        EmitePorNotaFiscalIndividual = 3,
        /// <summary>
        /// Emite 1 conhecimento por pedido
        /// </summary>
        EmitePorPedidoIndividual = 4,
        ///// <summary>
        ///// Emite 1 conhecimento por nota, porém o recebedor de todos é o destinatario do pedido
        ///// </summary>
        //EmitePorNotaFiscalIndividualComRecebedorDestinatarioPedido = 4,
        ///// <summary>
        ///// Emite 1 conhecimento por remetente, destinatario e demais parametros com base nas Notas fiscais, porém o recebedor de todos é o destinatario do pedido
        ///// </summary>
        //EmitePorNotaFiscalAgrupadaComRecebedorDestinatarioPedido = 5,
        ///// <summary>
        ///// Emite 1 conhecimento por nota, porém vai ocorrer um transbordo na mercadoria, ou seja, vai mudar de veiculo antes de chegar no cliente final.
        ///// </summary>
        //EmitePorNotaFiscalIndividualComTransbordo = 6,
        ///// <summary>
        ///// Emite 1 conhecimento por remetente,  porém vai ocorrer um transbordo na mercadoria, ou seja, vai mudar de veiculo antes de chegar no cliente final.
        ///// </summary>
        //EmitePorNotaFiscalAgrupadaComTransbordo = 7
        /// <summary>
        /// Emite 1 conhecimento por remetente, destinatario e demais parametros com base nas Notas fiscais entre todos os pedidos da carga
        /// </summary>
        EmitePorNotaFiscalAgrupadaEntrePedidos = 5,
    }
}
