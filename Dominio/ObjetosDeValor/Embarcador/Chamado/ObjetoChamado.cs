using Dominio.Entidades.Embarcador.Chamados;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Chamado
{
    public class ObjetoChamado
    {
        public Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }
        public Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }
        public Dominio.Entidades.Empresa Empresa { get; set; }
        public Dominio.Entidades.Embarcador.Chamados.MotivoChamado MotivoChamado { get; set; }
        public Dominio.Entidades.Cliente Cliente { get; set; }
        public Dominio.Entidades.Cliente Tomador { get; set; }
        public Dominio.Entidades.Cliente Destinatario { get; set; }
        public string Observacao { get; set; }
        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }
        public decimal NumeroPallet { get; set; }
        public decimal QuantidadeItens { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelChamado ResponsavelChamado { get; set; }
        public decimal Valor { get; set; }
        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal NotaFiscal { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> NotasFiscais { get; set; }
        public Dominio.Entidades.Cliente ClienteDestino { get; set; }
        public Dominio.Enumeradores.TipoTomador TipoCliente { get; set; }
        public bool RetencaoBau { get; set; }
        public bool? AtendimentoRegistradoPeloMotorista { get; set; }
        public DateTime? DataReentrega { get; set; }
        public DateTime? DataRetencaoInicio { get; set; }
        public DateTime? DataRetencaoFim { get; set; }
        public DateTime? DataRegistroMotorista { get; set; }
        public decimal TempoRetencao { get; set; }
        public decimal ValorReferencia { get; set; }
        public string PlacaReboque { get; set; }
        public Dominio.Entidades.Cliente ClienteResponsavel { get; set; }
        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoasResponsavel { get; set; }
        public TipoPessoa? TipoPessoaResponsavel { get; set; }
        public decimal ValorDesconto { get; set; }
        public Dominio.Entidades.Usuario Motorista { get; set; }
        public bool PagoPeloMotorista { get; set; }
        public decimal SaldoDescontadoMotorista { get; set; }
        public int QuantidadeImagens { get; set; }
        public int Quantidade { get; set; }
        public Dominio.Entidades.TipoDeOcorrenciaDeCTe RealMotivo { get; set; }
        public Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }
        public string NumeroEmbarcador { get; set; }
        public GrupoMotivoChamado GrupoMotivoChamado { get; set; }
        public bool DevolucaoParcial { get; set; }
        public Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega MotivoDaDevolucao { get; set; }
        public Dominio.Entidades.Embarcador.Ocorrencias.TiposCausadoresOcorrencia TiposCausadoresOcorrencia { get; set; }
        public Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas CausasMotivoChamado { get; set; }
    }
}
