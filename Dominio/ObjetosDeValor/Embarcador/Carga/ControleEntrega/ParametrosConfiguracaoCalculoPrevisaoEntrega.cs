using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega
{
    public sealed class ParametrosConfiguracaoCalculoPrevisaoEntrega
    {
        #region Propriedades

        public DateTime? DataBase { get; set; }
        public bool ArmazenarComposicoesPrevisoes { get; set; }

        #endregion Propriedades
    }
}
