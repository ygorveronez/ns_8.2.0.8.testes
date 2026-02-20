namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar
{
    public enum enumVeiculoTipo
    {
        Indefinido = 1,
        Tres_Por_Quatro = 2,
        Carreta_Quatro_Eixos = 3,
        Carreta_Cinco_Eixos = 4,
        Carreta_Seis_Eixos = 5,
        Remolque = 6,
        Toco = 7,
        Tractor = 8,
        Truck = 9,
        Truck_Alongado = 10,
        Utilitario = 11,
        Van = 12,
        Vuc = 13
    }

    public enum enumRastreadorTipo
    {
        Sem_Rastreador = 1,
        Autotrac = 2
    }

    public enum enumModalidade
    {
        Simples = 1,
        Duplo = 2
    }

    public class envVeiculo
    {
        public int? id { get; set; }
        public int? transportadorID { get; set; }
        public enumVeiculoTipo veiculoTipoID { get; set; }
        public enumRastreadorTipo? rastreadorTipoID { get; set; }
        public string placa { get; set; }
        public int? quantidadeEixos { get; set; }
        public enumModalidade? modalidade { get; set; }
        public string anoFabricacao { get; set; }
        public string codigoRenavam { get; set; }
        public string numeroChassis { get; set; }
        public string corPredominante { get; set; }
        public string cidadeDaPlaca { get; set; }
        public string estadoDaPlaca { get; set; }
        public string codigoRastreador { get; set; }
        public bool frotaPropria { get; set; }
        public bool permitirValePedagioCartao { get; set; }
    }
}