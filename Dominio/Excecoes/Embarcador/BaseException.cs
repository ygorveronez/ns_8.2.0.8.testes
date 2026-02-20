using Dominio.ObjetosDeValor.Enumerador;
using System;

namespace Dominio.Excecoes.Embarcador
{
    public abstract class BaseException : Exception
    {
        public CodigoExcecao ErrorCode { get; private set; }

        public BaseException(string message) : base(message)
        {
        }

        public BaseException(string message, CodigoExcecao errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
