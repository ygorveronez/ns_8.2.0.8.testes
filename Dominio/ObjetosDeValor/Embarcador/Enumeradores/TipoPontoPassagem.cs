namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPontoPassagem
    {
        Coleta = 1,
        Entrega = 2,
        Pedagio = 3,
        Passagem = 4,
        Retorno = 5,
        Apoio = 6, // Utilizado DPA para fazer sub-rotas, onde é deixado o reboque para fazer sub-trechos de coleta.
        Balanca = 7,
        Fronteira = 8,
        Balsa = 9,
        PostoFiscal = 10
    }
}
