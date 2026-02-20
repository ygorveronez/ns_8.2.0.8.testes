using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class DescricaoEValor
    {
        public string Descricao { get; set; }
        public decimal Valor { get; set; }

        public List<DescricaoEValor> ObterListaDeValores()
        {
            return new List<DescricaoEValor>();
        }
    }
}
