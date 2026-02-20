using Dominio.ObjetosDeValor.Enumerador;

namespace Dominio.Excecoes.Embarcador
{
    public class ControllerException : BaseException
    {
        public ControllerException(string message) : base(message)
        {
        }

        public ControllerException(string message, CodigoExcecao errorCode) : base(message, errorCode)
        {
        }
    }
}
