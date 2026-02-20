using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Exceptions
{
    public class RegraDeNegocioInvalida : Exception
    {
        public RegraDeNegocioInvalida() : base("Regra de negócio inválida.") 
        {
        }

        public RegraDeNegocioInvalida(string message) : base(message)
        {
        }
    }
}
