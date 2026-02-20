using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao
{
    public sealed class Empresa
    {
        public int Codigo { get; set; }

        public string Descricao { get; set; }

        public bool JornadaMotoristaIniciada { get; set; }

        public string Placa { get; set; }

        public List<string> Reboques { get; set; }

        public string Transportadora { get; set; }

        public string UrlEmbarcador { get; set; }

        public string UrlMobile { get; set; }

        public bool VeiculoTipoManobra { get; set; }

        #region Configurações

        public bool ExigirDataEntregaNotaClienteCanhotos { get; set; }

        public string LinkVideoMobile { get; set; }

        #endregion
    }
}
