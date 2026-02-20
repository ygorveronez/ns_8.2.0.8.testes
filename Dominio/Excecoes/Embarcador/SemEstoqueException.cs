using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Excecoes.Embarcador
{
    public class SemEstoqueException : BaseException
    {
        public SemEstoqueException(string message) : base(message)
        {

        }
    }
}
