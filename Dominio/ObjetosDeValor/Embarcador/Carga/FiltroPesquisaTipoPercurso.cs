using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class FiltroPesquisaTipoPercurso
    {
        public string CodigoIntegracao { get; set; }

        public string Descricao { get; set; }

        public Vazio? Vazio { get; set; }
    }
}
