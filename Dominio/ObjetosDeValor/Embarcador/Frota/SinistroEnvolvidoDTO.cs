using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class SinistroEnvolvidoDTO
    {
        public int Codigo { get; set; }
        
        public TipoEnvolvidoSinistro Tipo { get; set; }

        public string Nome { get; set; }

        public string CPF { get; set; }

        public string RG { get; set; }

        public string CNH { get; set; }

        public string TelefonePrincipal { get; set; }

        public string TelefoneSecundario { get; set; }

        public string Veiculo { get; set; }
        
        public string Observacao { get; set; }
    }
}
