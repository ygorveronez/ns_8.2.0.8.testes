namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class AnuenciaTransportador
    {
        public virtual string NomeEmpresa { get; set; }

        public virtual string CNPJ { get; set; }

        public virtual string Endereco { get; set; }
        public virtual string Bairro { get; set; }
        public virtual string Numero { get; set; }

        public virtual string Cidade { get; set; }
        public virtual string Estado { get; set; }

        public virtual string ValorConciliacao { get; set; }
        public virtual string DataInicial { get; set; }
        public virtual string DataFinal { get; set; }

        public virtual string CertificadoEmitidoPor { get; set; }
        public virtual string ValidadeCertificado { get; set; }
        public virtual string ImpressaoDigitalCertificado { get; set; }
        public virtual string DataAssinatura { get; set; }
    }
}
