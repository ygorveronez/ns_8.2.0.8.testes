using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Usuarios
{
    public class UsuarioFilial
    {
        public Usuario Usuario { get; set; }
        
        public List<Filial.Filial> Filial { get; set; }
    }
}