using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Administrativo
{
    public class Licenca
    {
        public int Codigo { get; set; }
        public string Identificador { get; set; }
        public string Entidade { get; set; }
        public string Descricao { get; set; }
        public string NumeroLicenca { get; set; }
        private DateTime DataEmissao { get; set; }
        private DateTime DataVencimento { get; set; }
        public string TipoLicenca { get; set; }
        public string NumeroFrota { get; set; }
        private bool StatusEntidade { get; set; }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataVencimentoFormatada
        {
            get { return DataVencimento != DateTime.MinValue ? DataVencimento.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string StatusEntidadeDescricao
        {
            get { return StatusEntidade ? "Ativo" : "Inativo"; }
        }
    }
}