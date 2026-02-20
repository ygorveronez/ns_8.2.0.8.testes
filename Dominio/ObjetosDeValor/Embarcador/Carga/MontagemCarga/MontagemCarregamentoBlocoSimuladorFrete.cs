using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga
{
    public class MontagemCarregamentoBlocoSimuladorFrete
    {
        public MontagemCarregamentoBlocoSimuladorFrete()
        {
            this.Pedidos = new List<Entidades.Embarcador.Pedidos.Pedido>();
        }

        public virtual Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco Bloco { get; set; }

        public virtual Dominio.Entidades.Empresa Transportador { get; set; }

        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        public virtual ObjetosDeValor.Embarcador.Enumeradores.CentroCarregamentoTipoOperacaoTipo Tipo { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        /// <summary>
        /// Contém o Grupo que consiste em um bloco de simulação.
        /// Ex: 50.000 toneladas
        ///     Grupo   Qtde    Modelo  Capacidade
        ///     Grupo 1: 1 - CARRETA = 35.000
        ///              1 - VUC     = 15.000
        ///              
        ///     Grupo 2: 2 - 4º EIXO = 25.000
        ///         
        ///     Grupo 3: 1 - 4º EIXO = 25.000
        ///              1 - Truck   = 20.000
        ///              1 - Fiorino =  5.000
        /// </summary>
        public virtual int Grupo { get; set; }

        /// <summary>
        /// Quantidade de viagens do Transportador, Modelo Veicular e Tipo de Operação.
        /// </summary>
        public virtual int Quantidade { get; set; }

        public virtual int Ranking { get; set; }

        /// <summary>
        /// Valor TOTAL DO FRETE do Bloco de Simulação pelo Transportador, Tipo Operação e Modelo veicular...
        /// </summary>
        public virtual decimal ValorTotal { get; set; }

        public virtual int LeadTime { get; set; }

        public virtual bool ExigeIsca { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        public virtual bool Vencedor { get; set; }

        /// <summary>
        /// Valor do Frete * Quantidade do modelo...
        /// </summary>
        public virtual decimal ValorTotalSimulacao { get { return this.ValorTotal * this.Quantidade; } }

        public virtual decimal LeadTimeTotalSimulacao { get { return this.LeadTime * this.Quantidade; } }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> Pedidos { get; set; }
    }
}
