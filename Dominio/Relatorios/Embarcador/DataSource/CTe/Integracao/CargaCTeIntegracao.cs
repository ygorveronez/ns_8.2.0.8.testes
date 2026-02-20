using System;

namespace Dominio.Relatorios.Embarcador.DataSource.CTe.Integracao
{
    public class CargaCTeIntegracao
    {
        public int Codigo { get; set; }
        public int NumeroCTe { get; set; }
        public int SerieCTe { get; set; }
        public string NumeroCarga { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataIntegracao { get; set; }
        public decimal AliquotaICMS { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorAReceber { get; set; }
        public int Tentativas { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao Situacao { get; set; }
        public string Mensagem { get; set; }
        public string TipoIntegracao { get; set; }
        public string GrupoPessoas { get; set; }
        public string NumeroDocumentoTransporte { get; set; }
        public string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao:
                        return "Ag. Integração";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado:
                        return "Integrado";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao:
                        return "Problema na Integração";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno:
                        return "Ag. Retorno";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
