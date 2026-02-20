namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar
{
    public enum enumCarretaTipo
    {
        Nao_Aplicavel = 1,
        Aberta = 2,
        Fechada_Baú = 3,
        Granelera = 4,
        Porta_Container = 5,
        Sider = 6,
    }

    public class envCarreta
    {
        public int? id { get; set; }
        public int? transportadorID { get; set; }
        public enumCarretaTipo carretaTipoID { get; set; }
        public string placa { get; set; }
        public int? quantidadeEixos { get; set; }
        public string anoFabricacao { get; set; }
        public string codigoRenavam { get; set; }
        public string numeroChassis { get; set; }
        public string corPredominante { get; set; }
        public string cidadeDaPlaca { get; set; }
        public string estadoDaPlaca { get; set; }
        public int? peso { get; set; }
        public int? volume { get; set; }
        public int? tara { get; set; }
        public bool frotaPropria { get; set; }
    }
}