namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public class FiltroPesquisaCausas
    {
        public string Descricao { get; set; }
        public bool Ativo {  get; set; }
        public int CodigoTipoOcorrencia { get; set; }
        public int CodigoMotivoChamado { get; set; }
        public bool BuscarTodasCausasDesconsiderandoTipoOcorrencia { get; set; }
        public bool BuscarTodasCausasDesconsiderandoMotivoChamado { get; set; }
    }
}
