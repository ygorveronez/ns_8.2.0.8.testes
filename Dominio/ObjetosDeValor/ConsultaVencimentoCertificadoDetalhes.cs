using System;

namespace Dominio.ObjetosDeValor
{
    public class ConsultaVencimentoCertificadoDetalhes
    {
        public DateTime DataLancamento { get; set; }

        public Dominio.Enumeradores.TipoHistorico Tipo { get; set; }

        public Dominio.Enumeradores.StatusVendaCertificado StatusVenda { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao? _Satisfacao { get; set; }

        public int? Satisfacao
        {
            set
            {
                if (value.HasValue) this._Satisfacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao)value;
            }
        }

        public string Usuario { get; set; }

        public string Detalhes { get; set; }

        public string DescricaoTipo {
            get
            {
                switch(Tipo)
                {
                    case Enumeradores.TipoHistorico.Atualizacao:
                        return "Atualização";
                    case Enumeradores.TipoHistorico.Inativacao:
                        return "Inativação";
                    case Enumeradores.TipoHistorico.Informacao:
                        return "Informação";
                    default:
                        return "";
                }
            }
        }
        public string DescricaoStatusVenda
        {
            get
            {
                switch (StatusVenda)
                {
                    case Enumeradores.StatusVendaCertificado.AguardandoDadosParaAdesao:
                        return "Aguardando Dados para Adesão";
                    case Enumeradores.StatusVendaCertificado.CertificadoVendido:
                        return "Certificado Vendido";
                    case Enumeradores.StatusVendaCertificado.SemContato:
                        return "Sem Contato";
                    case Enumeradores.StatusVendaCertificado.Retornar:
                        return "Retornar";
                    case Enumeradores.StatusVendaCertificado.Providenciando:
                        return "Providenciando";
                    case Enumeradores.StatusVendaCertificado.Bloqueado:
                        return "Bloqueado";
                    default:
                        return "";
                }
            }
        }

        public string DescricaoSatisfacao
        {
            get
            {
                switch (_Satisfacao)
                {
                    case Embarcador.Enumeradores.NivelSatisfacao.Otimo:
                        return "Ótimo";
                    case Embarcador.Enumeradores.NivelSatisfacao.Bom:
                        return "Bom";
                    case Embarcador.Enumeradores.NivelSatisfacao.Ruim:
                        return "Ruim";
                    case Embarcador.Enumeradores.NivelSatisfacao.NaoAvaliado:
                        return "Não Avaliado";
                    default:
                        return "";
                }
            }
        }
    }
}
