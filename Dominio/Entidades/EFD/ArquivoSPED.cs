using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominio.Entidades.EFD
{
    public class ArquivoSPED
    {

        #region Construtores

        public ArquivoSPED()
        {
            this.Blocos = new List<Bloco>();
        }

        #endregion

        #region Propriedades

        public List<Bloco> Blocos { get; set; }

        #endregion

        #region Métodos

        public string ObterDadosParaArquivo()
        {
            this.Blocos = this.Blocos.OrderBy(o => o.Identificador).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (Bloco bloco in this.Blocos)
            {
                sb.Append(bloco.ObterDadosParaArquivo());
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

        #endregion

    }
}
