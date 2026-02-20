using System;
using System.Text;

namespace Servicos
{
    public class Senha
    {
        public string GerarSenha(int tamanho)
        {
            StringBuilder senha = new StringBuilder();
            Random randomizer = new Random();
            for (int i = 0; i < tamanho; i++)
            {
                // Índices (range de códigos da tabela ASCII).
                // 0: números -> 48 ~ 57;
                // 1: maiúsculas -> 65 ~ 90;
                // 2: minúsculas -> 97 ~ 122;
                int[] caracteresAleatorios = { randomizer.Next(48, 57 + 1), randomizer.Next(65, 90 + 1), randomizer.Next(97, 122 + 1) };
                senha.Append((char)caracteresAleatorios[randomizer.Next(0, 3)]);
            }
            return senha.ToString();
        }
    }
}
