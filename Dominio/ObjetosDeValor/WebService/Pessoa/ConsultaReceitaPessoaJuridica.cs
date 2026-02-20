namespace Dominio.ObjetosDeValor.WebService.Pessoa
{
    public class ConsultaReceitaPessoaJuridica
    {
        public bool ConsultaValida { get; set; }
        public string MensagemReceita { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Pessoa { get; set; }
    }
}
