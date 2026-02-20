using System.IO;

namespace Dominio.NDDigital.v104.Cancelamento
{
    public class Arquivo : ArquivoBase
    {
        #region Construtores

        public Arquivo(Stream arquivo)
            : base(arquivo)
        {
            this.LerArquivoEGerarRegistros();
        }

        #endregion

        #region Propriedades

        public Registro00000 R00000 { get; set; }

        public Registro00010 R00010 { get; set; }

        #endregion

        #region Métodos

        protected override void LerArquivoEGerarRegistros()
        {
            while (!this.ReaderArquivo.EndOfStream)
            {
                string linha = this.ReaderArquivo.ReadLine();

                Registro registro = this.InstanciarRegistro(linha);

                if (registro != null)
                {
                    if (registro.Identificador == "00000")
                    {
                        this.R00000 = (Registro00000)registro;
                    }
                    else if (registro.Identificador == "00010")
                    {
                        this.R00010 = (Registro00010)registro;
                    }
                }
            }

            this.ReaderArquivo.Dispose();
        }

        private Registro InstanciarRegistro(string registro)
        {
            switch (registro.Substring(0, 5))
            {
                case "00000":
                    return new Registro00000(registro);
                case "00010":
                    return new Registro00010(registro);
                default:
                    return null;
            }
        }

        #endregion
    }
}
