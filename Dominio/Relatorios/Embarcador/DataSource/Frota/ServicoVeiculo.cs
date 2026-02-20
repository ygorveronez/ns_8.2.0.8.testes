using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class ServicoVeiculo
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string CodigoIntegracao { get; set; }
        public string Situacao { get; set; }
        public int ValidadeKM { get; set; }
        public int ToleranciaKM { get; set; }
        public int ValidadeDias { get; set; }
        public int ToleranciaDias { get; set; }
        public string Observacao { get; set; }
        public string ExecucaoUnica { get; set; }
        public string PermiteLancamentoSemValor { get; set; }
        public string ObrigatorioParaRealizarCarga { get; set; }
        private MotivoServicoVeiculo Motivo { get; set; }
        private TipoServicoVeiculo Tipo { get; set; }
        private TipoManutencaoServicoVeiculo TipoManutencao { get; set; }
        public int TempoEstimado { get; set; }
        public int ValidadeHorimetro { get; set; }
        public int ToleranciaHorimetro { get; set; }
        public string ServicoParaEquipamento { get; set; }
        public string GrupoServico { get; set; }
        public string ValidadeKMGrupo { get; set; }
        public string ToleranciaKMGrupo { get; set; }
        public string ValidadeDiasGrupo { get; set; }
        public string ToleranciaDiasGrupo { get; set; }

        #endregion

        #region Propriedades com Regras

        public string MotivoFormatado
        {
            get { return Motivo.ObterDescricao(); }
        }

        public string TipoFormatado
        {
            get { return Tipo.ObterDescricao(); }
        }

        public string TipoManutencaoFormatado
        {
            get { return TipoManutencao.ObterDescricao(); }
        }

        #endregion
    }
}
