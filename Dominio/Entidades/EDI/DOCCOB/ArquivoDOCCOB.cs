using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.EDI.DOCCOB
{
    public class ArquivoDOCCOB
    {
        #region Construtores

        public ArquivoDOCCOB()
        {
            this.Registros = new List<Registro>();
        }

        #endregion

        #region Propriedades

        public List<Registro> Registros { get; set; }

        #endregion

        #region Métodos

        public string ObterDadosParaArquivo()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Registro registro in this.Registros)
            {
                sb.Append(registro.ObterDadosParaArquivo());
            }
            return sb.ToString();
        }

        public System.IO.MemoryStream ObterArquivo()
        {
            string arquivo = this.ObterDadosParaArquivo();
            System.IO.MemoryStream memoStream = new System.IO.MemoryStream();
            memoStream.Write(System.Text.Encoding.Default.GetBytes(arquivo), 0, arquivo.Length);
            memoStream.Position = 0;
            return memoStream;
        }

        public string ObterStringArquivo()
        {
            return this.ObterDadosParaArquivo();
        }

        #endregion
    }
}
