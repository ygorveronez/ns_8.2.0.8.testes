using Dominio.ObjetosDeValor.Enumerador;

namespace Dominio.Excecoes.Embarcador
{
    public class RepositorioException : BaseException
    {
        public RepositorioException(string message) : base(message)
        {
        }

        public RepositorioException(string message, CodigoExcecao errorCode) : base(message, errorCode)
        {
        }
    }
}