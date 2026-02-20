using System.Collections.Generic;

namespace Dominio.ObjetosDeValor
{
    public class Veiculo
    {
        public int Id { get; set; }
        public int Codigo { get; set; }
        public int CapacidadeKG { get; set; }
        public int CapacidadeM3 { get; set; }
        public string UF { get; set; }
        public string Placa { get; set; }
        public string Renavam { get; set; }
        public int Tara { get; set; }
        public string DescricaoTipo { get; set; }
        public string DescricaoTipoCarroceria { get; set; }
        public string DescricaoTipoCombustivel { get; set; }
        public string DescricaoTipoVeiculo { get; set; }
        public string DescricaoTipoRodado { get; set; }
        public string CodigoModeloVeicularEmbarcador { get; set; }
        public string TipoDoVeiculo { get; set; }
        public bool Ativo { get; set; } 
        public bool Excluir { get; set; }
        public List<Veiculo> VeiculosVinculados { get; set; }
        public List<Dominio.ObjetosDeValor.Motorista> Motoristas { get; set; }
        public Dominio.ObjetosDeValor.Empresa Empresa { get; set; }
        public string NumeroEquipamentoRastreador { get; set; }
    }
}
