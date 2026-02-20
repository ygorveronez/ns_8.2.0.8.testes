using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Seguros
{
    public class ReportApolices
    {
        public int Codigo { get; set; }
        public string NumeroApolice { get; set; }
        public string NumeroAverbacao { get; set; }
        public string InicioVigencia { get; set; }
        public string FimVigencia { get; set; }
        public decimal ValorLimite { get; set; }
        public string Seguradora { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro ResponsavelSeguro { get; set; }
        public string Responsavel
        {
            get
            {
                switch (ResponsavelSeguro)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro.Transportador:
                        return "Transportador";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro.Embarcador:
                        return "Embarcador";
                    default:
                        return "";
                }
            }
        }

        public string Empresa
        {
            get
            {
                if (!string.IsNullOrEmpty(CNPJTransportadorSemFormato))
                {
                    return String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJTransportadorSemFormato)) + " - " + this.NomeEmpresa;
                }
                else
                {
                    return this.NomeEmpresa; ;
                }
            }
        }

        public string Pessoa { get; set; }
        public string GrupoPessoa { get; set; }

        public string CNPJTransportadorSemFormato { get; set; }

        public string NomeEmpresa { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao TipoSeguradoraAverbacao { get; set; }
        public string Averbadora
        {
            get
            {
                switch (TipoSeguradoraAverbacao)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.ATM:
                        return "ATM";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.Bradesco:
                        return "Bradesco";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.PortoSeguro:
                        return "Porto Seguro";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.NaoDefinido:
                        return "Não Definido";
                    default:
                        return string.Empty;
                }
            }
        }

        public string VencimentoCertificadoDigital { get; set; }

    }
}
