using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao
{
    public sealed class Usuario
    {
        public int Codigo { get; set; }

        public string Nome { get; set; }

        public string Sessao { get; set; }

        public bool RequerContraSenha { get; set; }

        public bool ContraSenhaValida { get; set; }

        public List<Empresa> Empresas { get; set; }

        public Enumeradores.TipoAcessoMobile TipoAcessoMobile { get; set; }

        public bool PermissaoAdministrador { get; set; }

        public List<Modulo> Permissoes { get; set; }

        public bool UtilizaControleEntrega { get; set; }
    }
}
