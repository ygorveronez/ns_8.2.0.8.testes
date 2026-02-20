namespace Dominio.ObjetosDeValor.Enumerador
{
    public enum StatusDocumentoTransporteNatura
    {
        EmDigitacao = 0, //usuário deve digitar os dados de motorista, veículo e valor frete
        EmEmissao = 1, //ct-es sendo emitidos
        Emitido = 2, //ctes emitidos
        Retornado = 3, //ctes enviados para a natura
        Finalizado = 4, //ocorrencias enviadas
        Cancelado = 5, //DT Cancelada
        AguardandoEmissaoAutomatica = 6, //Status para as DT consultadas automaticamente
        AguardandoRetornoAutomatico = 7, //Status para as DT consultadas automaticamente
        Erro = 9 //Erro na consulta da Natura
    }
}
