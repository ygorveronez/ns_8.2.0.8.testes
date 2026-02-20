namespace Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega
{
    public class Nota
    {
        public int Codigo;
        public string NumeroNota;
        public string Serie;
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Emitente;
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Destinatario;
        public decimal Peso;
        public decimal Valor;
        public string Chave;
        public bool DigitalizacaoCanhotoInteiro;
    }
}
