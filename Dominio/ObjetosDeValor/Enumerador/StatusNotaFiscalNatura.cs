namespace Dominio.ObjetosDeValor.Enumerador
{
    public enum StatusNotaFiscalNatura
    {
        Pendente = 0, //aguardando emissao do ct-e
        Emitido = 1, //ct-e emitido
        Retornado = 2, // ct-e retornado para a natura
        Finalizado = 3 // ocorrencia do ct-e retornada para a natura
    }
}
