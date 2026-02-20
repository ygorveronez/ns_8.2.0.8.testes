using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Excecoes.Embarcador
{
    public class CustomException : BaseException
    {
        public CustomException(string message) : base(message)
        {
        }
    }
}
