namespace Dominio.ObjetosDeValor.Embarcador.Importacao
{
    public class RetonoLinha
    {
        #region Propriedades

        public int codigo { get; set; }

        public int indice { get; set; }

        public bool processou { get; set; }

        public string mensagemFalha { get; set; }

        public bool contar { get; set; }

        public bool isTimeOut { get; set; }

        public bool isAlterado { get; set; }

        #endregion

        #region Construtores

        public static RetonoLinha CriarRetornoFalha(string mensagem, int indice, bool istimeOut = false)
        {
            return new RetonoLinha()
            {
                codigo = 0,
                indice = indice,
                mensagemFalha = mensagem,
                processou = false,
                contar = false,
                isTimeOut = istimeOut
            };
        }

        public static RetonoLinha CriarRetornoSucesso(int indice, int codigo = 0, bool contar = true, bool isAlterado = true)
        {
            return new RetonoLinha()
            {
                codigo = codigo,
                indice = indice,
                mensagemFalha = "",
                processou = true,
                contar = contar,
                isAlterado = isAlterado
            };
        }

        #endregion
    }
}
