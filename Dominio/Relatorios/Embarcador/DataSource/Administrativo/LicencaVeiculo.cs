using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Administrativo
{
    public class LicencaVeiculo
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string NumeroLicenca { get; set; }
        private DateTime DataEmissao { get; set; }
        private DateTime DataVencimento { get; set; }
        public string TipoLicenca { get; set; }
        public string NumeroFrota { get; set; }
        private bool StatusVeiculo { get; set; }
        public string CentroResultado { get; set; }
        public string FuncionarioResponsavel { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Placa { get; set; }
        public string Motorista { get; set; }
        private StatusLicenca StatusLicenca { get; set; }
        public string Renavam { get; set; }


        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataVencimentoFormatada
        {
            get { return DataVencimento != DateTime.MinValue ? DataVencimento.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string StatusVeiculoDescricao
        {
            get { return StatusVeiculo ? "Ativo" : "Inativo"; }
        }

        public string StatusLicencaDescricao
        {
            get { return StatusLicenca.ObterDescricao(); }
        }
    }
}