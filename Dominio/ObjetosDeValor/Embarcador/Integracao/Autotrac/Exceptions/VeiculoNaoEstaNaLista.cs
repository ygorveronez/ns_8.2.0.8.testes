using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Exceptions
{
    public class VeiculoNaoEstaNaLista : Exception
    {
        public VeiculoNaoEstaNaLista() : base("Veículo não cadastrado na lista")
        {
        }
    }
}
