using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Veiculos
{
    public class FiltroPesquisaEquipamento
    {
        public string Descricao { get; set; }
        public string Numero { get; set; }
        public string Chassi { get; set; }
        public SituacaoAtivoPesquisa Ativo { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoMarcaEquipamento { get; set; }
        public int Codigo { get; set; }
    }
}
