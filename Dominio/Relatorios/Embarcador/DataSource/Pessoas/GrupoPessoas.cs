using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pessoas
{
    public class GrupoPessoas
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        private bool Ativo { get; set; }
        private TipoGrupoPessoas TipoGrupo { get; set; }
        public string Email { get; set; }
        public string CondicaoPedido { get; set; }
        private SituacaoFinanceira SituacaoFinanceira { get; set; }
        public decimal ValorLimiteFaturamento { get; set; }
        public int DiasEmAbertoAposVencimento { get; set; }
        public int DiasDePrazoFatura { get; set; }
        private bool Bloqueado { get; set; }
        private bool ExigeCanhotoFisico { get; set; }
        public string Vendedor { get; set; }


        public string AtivoDescricao 
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }

        public string TipoGrupoDescricao 
        {
            get 
            {
                switch (TipoGrupo)
                {
                    case TipoGrupoPessoas.Ambos: return "Ambos";
                    case TipoGrupoPessoas.Clientes: return "Cliente";
                    case TipoGrupoPessoas.Fornecedores: return "Fornecedor";
                    default: return string.Empty;
                }
            }
        }

        public string SituacaoFinanceiraDescricao 
        {
            get { return SituacaoFinanceira.ObterDescricao(); }
        }

        public string BloqueadoDescricao 
        {
            get { return Bloqueado ? "Sim" : "Não"; }
        } 
        public string ExigeCanhotoFisicoDescricao
        {
            get { return ExigeCanhotoFisico ? "Sim" : "Não"; }
        }
    }
}
