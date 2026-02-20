using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Exceptions
{
    public class RequerAutorizacao : Exception
    {
        public RequerAutorizacao() : base("Requer autenticação") 
        {
        }
    }
}
