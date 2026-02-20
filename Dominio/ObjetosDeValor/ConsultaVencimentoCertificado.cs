using System;

namespace Dominio.ObjetosDeValor
{
    public class ConsultaVencimentoCertificado
    {
        public string Cnpj { get; set; }

        public string Nome { get; set; }

        public DateTime Vencimento { get; set; }

        public string Email { get; set; }

        public Dominio.Enumeradores.StatusVendaCertificado StatusVenda { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao? _Satisfacao { get; set; }

        public int? Satisfacao {
            set
            {
                if (value.HasValue) this._Satisfacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao)value;
            }
        }

        public string Localidade { get; set; }

        public string EmpresaAdmin { get; set; }

        public string Telefone { get; set; }

        public string Ambiente { get; set; }

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
                    case Enumeradores.StatusVendaCertificado.Inativado:
                        return "Inativado";
                    case Enumeradores.StatusVendaCertificado.Atualizado:
                        return "Atualizado";
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
