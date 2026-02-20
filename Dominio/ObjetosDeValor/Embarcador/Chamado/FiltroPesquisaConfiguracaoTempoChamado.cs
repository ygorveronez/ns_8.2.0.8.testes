using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Chamado
{
    public class FiltroPesquisaConfiguracaoTempoChamado
    {
        public string Descricao { get; set; }
        public SituacaoAtivoPesquisa Ativo { get; set; }
        public double CnpjCpfCliente { get; set; }
        public int CodigoFilial { get; set; }
        public int CodigoTipoOperacao { get; set; }
    }
}
