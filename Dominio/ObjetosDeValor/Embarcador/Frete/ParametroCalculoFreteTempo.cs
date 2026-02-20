using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class ParametroCalculoFreteTempo
    {
        public Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }

        /// <summary>
        /// Informa se será sumarizado o tempo nos dados do frete (depois isso vai para a carga/notas/pedidos e é utilizado em relatórios)
        /// </summary>
        public virtual bool SumarizarTempoDadosFrete { get; set; }

        public virtual bool MultiplicarPorHora { get; set; }
        public virtual bool MultiplicarPorAjudante { get; set; }
        public virtual bool MultiplicarPorDeslocamento { get; set; }
        public virtual bool MultiplicarPorDiaria { get; set; }
        public virtual bool MultiplicarPorEntrega { get; set; }
        public virtual bool UtilizarArredondamentoHoras { get; set; }
        public virtual bool UtilizarMinutosInformadosComoCorteArredondamentoHoraExata { get; set; }
        public virtual int? MinutosArredondamentoHoras { get; set; }
        public virtual bool PossuiHorasMinimasCobranca { get; set; }
        public virtual TimeSpan? HorasMinimasCobranca { get; set; }
        public List<ParametroCalculoFreteTempoFaixa> Faixas { get; set; }
    }
}
