using System.IO;

namespace Dominio.NDDigital
{
    public abstract class ArquivoBase
    {
        #region Construtores

        public ArquivoBase(Stream arquivo)
        {
            this.ReaderArquivo = new StreamReader(arquivo);
        }

        #endregion

        #region Propriedades

        protected StreamReader ReaderArquivo { get; set; }

        #endregion

        #region Métodos

        protected virtual void LerArquivoEGerarRegistros()
        {
        }

        #endregion
    }
}
