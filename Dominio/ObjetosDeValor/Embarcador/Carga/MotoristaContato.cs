using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class MotoristaContato
    {
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string DataNascimento { get; set; }
        public TipoParentesco TipoParentesco { get; set; }
    }
}
