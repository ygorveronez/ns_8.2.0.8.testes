using Dominio.ObjetosDeValor.Enumerador;

namespace Dominio.Excecoes.Embarcador
{
    public class ServicoException : BaseException
    {
        public ServicoException(string message) : base(message)
        {
        }

        public ServicoException(string message, CodigoExcecao errorCode) : base(message, errorCode)
        {
        }
    }
}
