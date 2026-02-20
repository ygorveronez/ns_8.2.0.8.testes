using Dominio.ObjetosDeValor.Enumerador;

namespace Dominio.Excecoes.Embarcador
{
    public class ImportacaoException : BaseException
    {
        public ImportacaoException(string message) : base(message)
        {
        }

        public ImportacaoException(string message, CodigoExcecao errorCode) : base(message, errorCode)
        {
        }
    }
}
