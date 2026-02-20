using Dominio.ObjetosDeValor.Enumerador;

namespace Dominio.Excecoes.Embarcador
{
    public class WebServiceException : BaseException
    {
        public WebServiceException(string message) : base(message)
        {
        }

        public WebServiceException(string message, CodigoExcecao errorCode) : base(message, errorCode)
        {
        }
    }
}
