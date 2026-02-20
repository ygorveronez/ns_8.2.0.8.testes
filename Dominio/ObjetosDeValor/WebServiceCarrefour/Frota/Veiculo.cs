using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebServiceCarrefour.Frota
{
    public sealed class Veiculo
    {
        public Veiculo()
        {
            this.TipoVeiculo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Tracao;
            this.TipoPropriedadeVeiculo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo.Proprio;
        }

        public string Placa { get; set; }

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

        public Pessoas.Empresa Transportador { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria TipoCarroceria { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo TipoVeiculo { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado TipoRodado { get; set; }

        public Carga.ModeloVeicular ModeloVeicular { get; set; }

        public Pessoas.GrupoPessoa GrupoPessoaSegmento { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo TipoPropriedadeVeiculo { get; set; }

        public Proprietario Proprietario { get; set; }

        public List<Carga.Motorista> Motoristas { get; set; }

        public Modelo Modelo { get; set; }

        public int Codigo { get; set; }

        public List<Veiculo> Reboques { get; set; }

        public string XTexto { get; set; }

        public string XCampo { get; set; }

        public string MotivoBloqueio { get; set; }

        public string DataSuspensaoInicio { get; set; }

        public string DataSuspensaoFim { get; set; }
    }
}
