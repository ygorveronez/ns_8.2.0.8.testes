using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class OrdemServico
    {
        public int Codigo { get; set; }
        public int Numero { get; set; }
        public DateTime DataProgramada { get; set; }
        public TipoManutencao TipoManutencao { get; set; }
        public Veiculo Veiculo { get; set; }
        public int KMAtual { get; set; }
        public Equipamento Equipamento { get; set; }
        public int Horimetro { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Carga.Motorista Motorista { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa LocalManutencao { get; set; }
        public string CondicaoPagamento { get; set; }
        public GrupoServico GrupoServico { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pedido.CentroResultado CentroResultado { get; set; }
        public Usuarios.Usuario Responsavel { get; set; }
        public string Observacao { get; set; }
        public DateTime? DataFechamento { get; set; }
        public Usuarios.Usuario Operador { get; set; }
        public decimal ValorOrcado { get; set; }
        public decimal ValorRealiazdo { get; set; }
        public decimal DiferencaOrcadoRealizado { get; set; }
        public List<ServicoManutencao> ServicosManutencao { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> Produtos { get; set; }
        public string DescricaoSituacao { get; set; }
    }
}
