using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Exceptions
{
    public class ParametroNoFormatoErrado : Exception
    {
        public ParametroNoFormatoErrado() : base("Parâmetros no formato errado.") 
        {
        }
    }
}
