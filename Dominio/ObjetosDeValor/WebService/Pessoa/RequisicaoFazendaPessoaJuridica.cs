using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Pessoa
{
    public class RequisicaoFazendaPessoaJuridica
    {
        public List<CookieDinamico> Cookies;

        public byte[] Captcha { get; set; }

    }
}
