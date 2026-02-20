namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public class ResultadoConsultaViagem
    {
        public string NumeroSM { get; set; }
        public Integracao.Buonny.RetornoViagemVeiculo Retorno { get; set; }
        public string MensagemErro { get; set; }
        public bool Sucesso { get; set; }
    }
}

