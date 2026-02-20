using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class ItemAjusteTabelaFrete
    {
        public ItemAjusteTabelaFrete()
        {
        }

        public ItemAjusteTabelaFrete(int codigo, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete tipo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponenteFrete)
        {
            Codigo = codigo;
            Descricao = descricao;
            Tipo = tipo;
        }

        public ItemAjusteTabelaFrete(string descricao, TipoParametroAjusteTabelaFrete tipo)
        {
            Descricao = descricao;
            Tipo = tipo;
        }

        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete Tipo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete TipoComponente { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoOperacao { get; set; }
        public bool Aumenta { get; set; }
        public decimal Valor { get; set; }
        public bool Arredondar { get; set; }
    }
}
