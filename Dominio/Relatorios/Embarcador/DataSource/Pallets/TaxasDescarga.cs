using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pallets
{
    public class TaxasDescarga
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Filial { get; set; }
        private string ModeloVeicular { get; set; }
        public string TipoCarga { get; set; }
        private bool Status { get; set; }
        private DateTime DataInicioVigencia { get; set; }
        private DateTime DataFimVigencia { get; set; }
        public string GrupoCliente { get; set; }
        public string TipoOperacao { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorTonelada { get; set; }
        public decimal ValorUnidade { get; set; }
        public decimal ValorPallet { get; set; }
        private string TipoPessoa { get; set; }
        private string Cliente { get; set; }
        public string ClienteCodigoIntegracao { get; set; }
        private double CpfCnpj { get; set; }
        private string NomeFantasia { get; set; }
        private bool PontoTransbordo { get; set; }
        private string ModeloVeicularCodigoIntegracao { get; set; }
        public decimal ValorAjudante { get; set; }
        public string Cidade { get; set; }
        public string UFCidade { get; set; }

        private DateTime DataCriacao { get; set; }
        private DateTime DataAlteracao { get; set; }
        private DateTime DataAprovacao { get; set; }
        private DateTime DataInativacao { get; set; }
        public string Usuario { get; set; }

        public SituacaoAjusteConfiguracaoDescargaCliente Situacao { get; set; }

        #endregion

        #region Propriedades com regras

        public string ClienteFormatado
        {
            get
            {
                string clienteFormatado = "";

                string nome = Cliente;
                if (PontoTransbordo)
                    nome = this.NomeFantasia;

                if (!string.IsNullOrWhiteSpace(ClienteCodigoIntegracao))
                    clienteFormatado += ClienteCodigoIntegracao + " - ";
                if (!string.IsNullOrWhiteSpace(nome))
                    clienteFormatado += nome;
                if (!string.IsNullOrWhiteSpace(TipoPessoa))
                    clienteFormatado += " (" + CpfCnpjFormatado + ")";

                return clienteFormatado;
            }
        }
        public string DataInicioVigenciaFormatada
        {
            get
            {
                return DataInicioVigencia != DateTime.MinValue ? DataInicioVigencia.ToString("dd/MM/yyyy") : "";
            }
        }
        public string DataCriacaoFormatada
        {
            get
            {
                return DataCriacao != DateTime.MinValue ? DataCriacao.ToString("dd/MM/yyyy") : "";
            }
        }
        public string DataAlteracaoFormatada
        {
            get
            {
                return DataAlteracao != DateTime.MinValue ? DataAlteracao.ToString("dd/MM/yyyy") : "";
            }
        }
        public string DataAprovacaoFormatada
        {
            get
            {
                return DataAprovacao != DateTime.MinValue ? DataAprovacao.ToString("dd/MM/yyyy") : "";
            }
        }
        public string DataInativacaoFormatada
        {
            get
            {
                return DataInativacao != DateTime.MinValue ? DataInativacao.ToString("dd/MM/yyyy") : "";
            }
        }

        public string DataFimVigenciaFormatada
        {
            get
            {
                return DataFimVigencia != DateTime.MinValue ? DataFimVigencia.ToString("dd/MM/yyyy") : "";
            }
        }
        public string StatusDescricao
        {
            get
            {
                return Status.ObterDescricaoAtivo();
            }
        }
        public string SituacaoDescricao
        {
            get
            {
                return Situacao.ObterDescricao();
            }
        }
        public string CpfCnpjFormatado
        {
            get { return CpfCnpj.ToString().ObterCpfOuCnpjFormatado(); }
        }
        public string ModeloVeicularFormatado
        {
            get { return !string.IsNullOrEmpty(ModeloVeicular) ? $"{ModeloVeicularCodigoIntegracao} - {ModeloVeicular}" : string.Empty; }
        }


        #endregion
    }


}
