namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar
{
    public class retVeiculo
    {
        public int? id { get; set; }
        public int? transportadorID { get; set; }
        public int? rastreadorTipoID { get; set; }
        public int? veiculoTipoID { get; set; }
        public string placa { get; set; }
        public int? quantidadeEixos { get; set; }
        public int? modalidade { get; set; }
        public string anoFabricacao { get; set; }
        public string codigoRenavam { get; set; }
        public string numeroChassis { get; set; }
        public string corPredominante { get; set; }
        public string cidadeDaPlaca { get; set; }
        public string estadoDaPlaca { get; set; }
        public string codigoRastreador { get; set; }
        public bool frotaPropria { get; set; }
        public bool ativo { get; set; }
        public bool permitirValePedagioCartao { get; set; }
    }
}