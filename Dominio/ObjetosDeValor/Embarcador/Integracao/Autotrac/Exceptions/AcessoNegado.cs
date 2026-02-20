using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Exceptions
{
    public class AcessoNegado : Exception
    {
        public AcessoNegado() : base("Acesso a requisição negada.") 
        {
        }
    }
}
