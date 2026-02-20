namespace System.Text
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder Append(this StringBuilder valor, string texto, bool condicao)
        {
            if (condicao)
                valor.Append(texto);

            return valor;
        }

        public static StringBuilder AppendLine(this StringBuilder valor, string texto, bool condicao)
        {
            if (condicao)
                valor.AppendLine(texto);

            return valor;
        }

        public static bool Contains(this StringBuilder valor, string textoBuscar)
        {
            return valor.IndexOf(textoBuscar) != -1;
        }

        public static int IndexOf(this StringBuilder valor, string textoBuscar)
        {
            if ((valor == null) || (textoBuscar == null))
                throw new ArgumentNullException();

            if (textoBuscar.Length == 0)
                return 0;

            for (int indiceValor = 0; indiceValor < valor.Length; indiceValor++)
            {
                if ((valor.Length - indiceValor) < textoBuscar.Length)
                    return -1;

                for (int indiceTextoBuscar = 0; indiceTextoBuscar < textoBuscar.Length; indiceTextoBuscar++)
                {
                    if (valor[indiceValor + indiceTextoBuscar] != textoBuscar[indiceTextoBuscar])
                        break;

                    if (indiceTextoBuscar == (textoBuscar.Length - 1))
                        return indiceValor;
                }
            }

            return -1;
        }
    }
}
