using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.NFe
{
    public sealed class NotasEmitidasItens
    {
        public int CodigoNF { get; set; }
        public int TipoNota { get; set; }
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public UnidadeDeMedida UnidadeMedida { get; set; }
        public string UnidadeMedidaSigla
        {
            get { return UnidadeDeMedidaHelper.ObterSigla(UnidadeMedida); }
        }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
