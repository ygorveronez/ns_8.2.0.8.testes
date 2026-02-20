using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class Veiculo
    {
        public Veiculo()
        {
            this.TipoVeiculo = Enumeradores.TipoVeiculo.Tracao;
            this.TipoPropriedadeVeiculo = Enumeradores.TipoPropriedadeVeiculo.Proprio;
        }
        public string Placa { get; set; }
        public int Protocolo { get; set; }
        public string Renavam { get; set; }
        public string UF { get; set; }
        public string RNTC { get; set; }
        public int Tara { get; set; }
        public int CapacidadeKG { get; set; }
        public int CapacidadeM3 { get; set; }
        public string NumeroFrota { get; set; }
        public string NumeroChassi { get; set; }
        public string NumeroMotor { get; set; }
        public string DataAquisicao { get; set; }
        public int AnoFabricacao { get; set; }
        public int AnoModelo { get; set; }
        public bool Ativo { get; set; }
        public string DataValidadeGR { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa Transportador { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria TipoCarroceria { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo TipoVeiculo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado TipoRodado { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular ModeloVeicular { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa GrupoPessoaSegmento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo TipoPropriedadeVeiculo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frota.Proprietario Proprietario { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista> Motoristas { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frota.Modelo Modelo { get; set; }
        public int Codigo { get; set; }
        public List<Veiculo> Reboques { get; set; }
        public string XTexto { get; set; }
        public string XCampo { get; set; }
        public string MotivoBloqueio { get; set; }
        public string DataSuspensaoInicio { get; set; }
        public string DataSuspensaoFim { get; set; }
        public bool? PossuiTagValePedagio { get; set; }
        public int OrdemReboque { get; set; }
        public string DataRetiradaCtrn { get; set; }
        public string NumeroContainer { get; set; }
        public int TaraContainer { get; set; }
        public int MaxGross { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.Anexo> Anexos { get; set; }
        public decimal ValorContainerAverbacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoCompraValePedagioTarget? TipoTagValePedagio { get; set; }
        public string NumeroCartaoValePedagio { get; set; }
        public string Cor { get; set; }
        public bool? PossuiRastreador { get; set; }
        public string NumeroEquipamentoRastreador { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frota.TecnologiaRastreador TecnologiaRastreador { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frota.TipoComunicacaoRastreador TipoComunicacaoRastreador { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frota.SegmentoVeiculo SegmentoVeiculo { get; set; }
        public int KilometragemAtual { get; set; }
        public decimal CapacidadeMaximaTanque { get; set; }
        public string DescricaoModeloVeiculo { get; set; }
        public string DescricaoMarcaVeiculo { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Frota.Equipamento> Equipamentos { get; set; }
        public List<CentroCarregamento> CentroCarregamentos { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFrota? TipoFrota { get; set; }
    }
}
