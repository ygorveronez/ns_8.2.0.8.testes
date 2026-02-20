namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EtapaFluxoColetaEntrega
    {
        Todas = 0,
        AgSenha = 5,
        PendenciaAlocarVeiculo = 10,
        VeiculoAlocado = 20,
        SaidaCD = 30,
        Integracao = 35,
        ChegadaFornecedor = 40,
        CTe = 50,
        MDFe = 60,
        CTeSubcontratacao = 70,
        SaidaFornecedor = 80,
        ChegadaCD = 90,
        Ocorrencia = 100,
        Finalizado = 110
    }
}
