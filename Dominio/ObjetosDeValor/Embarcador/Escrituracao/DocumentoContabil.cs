using System;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class DocumentoContabil
    {
        public double CNPJCPFTomador { get; set; }
        public string TipoTomador { get; set; }
        public string CodigoContaContabil { get; set; }
        public string DescricaoContaContabil { get; set; }
        public string CodigoCentroResultado { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao TipoContabilizacao { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil TipoContaContabil { get; set; }
        public decimal ValorContabilizacao { get; set; }

        public Dominio.Enumeradores.TipoDocumento? TipoDocumentoEmissao { get; set; }


        public int CodigoCTe { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public string CodigoTomador { get; set; }
        public string GrupoTomador { get; set; }
        public string NumeroModelo { get; set; }
        public string PrefixoOcorrenciaOutrosDocumentos { get; set; }
        public string CST { get; set; }
        public string CNPJEmpresa { get; set; }
        public string CodigoEmpresa { get; set; }

        public string NumeroCarga { get; set; }
        public int? NumeroOcorrencia { get; set; }

        public DateTime DataEmissao { get; set; }
        public DateTime? DataVencimento { get; set; }

        public string TipoRemetente { get; set; }
        public double CNPJRemetente { get; set; }
        public string CodigoRemetente { get; set; }

        public string TipoDestinatario { get; set; }
        public double CNPJDestinatario { get; set; }
        public string CodigoDestinatario { get; set; }

        public decimal ValorAReceber { get; set; }

        public decimal BaseCalculoICMS { get; set; }
        public decimal BaseCalculoIIS { get; set; }

        public decimal AliquotaICMS { get; set; }
        public decimal AliquotaISS { get; set; }

        public decimal ValorICMS { get; set; }
        public decimal ValorISS { get; set; }
        public string TipoOperacao { get; set; }
        public string TipoOcorrencia { get; set; }
        public int? CodigoCarga { get; set; }

        public virtual decimal AliquotaImposto
        {
            get
            {
                if (TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    return AliquotaICMS;
                else
                    return AliquotaISS;
            }
        }


        public virtual decimal BaseCalculoImposto
        {
            get
            {
                if (TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    return BaseCalculoICMS;
                else
                    return BaseCalculoIIS;
            }
        }


        public virtual decimal ValorImposto
        {
            get
            {
                if (TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    return ValorICMS;
                else
                    return ValorISS;
            }
        }


        public virtual string CPF_CNPJ_Destinatario_SemFormato
        {
            get
            {
                if (this.TipoDestinatario != null)
                {
                    if (this.TipoDestinatario != null && this.TipoDestinatario.Equals("E"))
                    {
                        return "00000000000000";
                    }
                    else
                    {
                        return this.TipoDestinatario != null && this.TipoDestinatario.Equals("J") ? String.Format(@"{0:00000000000000}", this.CNPJDestinatario) : String.Format(@"{0:00000000000}", this.CNPJDestinatario);
                    }
                }
                else
                    return "";

            }
        }

        public virtual string CPF_CNPJ_Destinatario_Formatado
        {
            get
            {
                if (this.TipoDestinatario != null)
                {
                    if (this.TipoDestinatario.Equals("E"))
                    {
                        return "00.000.000/0000-00";
                    }
                    else
                    {
                        return this.TipoDestinatario.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CNPJDestinatario) : String.Format(@"{0:000\.000\.000\-00}", this.CNPJDestinatario);
                    }
                }
                else
                    return "";
            }
        }

        public virtual string CPF_CNPJ_Tomador_SemFormato
        {
            get
            {
                if (this.TipoDestinatario != null)
                {
                    if (this.TipoTomador.Equals("E"))
                    {
                        return "00000000000000";
                    }
                    else
                    {
                        return this.TipoTomador.Equals("J") ? String.Format(@"{0:00000000000000}", this.CNPJCPFTomador) : String.Format(@"{0:00000000000}", this.CNPJCPFTomador);
                    }
                }
                else
                    return "";
            }
        }

        public virtual string CPF_CNPJ_Tomador_Formatado
        {
            get
            {
                if (this.TipoDestinatario != null)
                {
                    if (this.TipoTomador.Equals("E"))
                    {
                        return "00.000.000/0000-00";
                    }
                    else
                    {
                        return this.TipoTomador.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CNPJCPFTomador) : String.Format(@"{0:000\.000\.000\-00}", this.CNPJCPFTomador);
                    }
                }
                else
                    return "";
            }
        }

        public virtual string CPF_CNPJ_Remetente_SemFormato
        {
            get
            {
                if (this.TipoDestinatario != null)
                {
                    if (this.TipoRemetente.Equals("E"))
                    {
                        return "00000000000000";
                    }
                    else
                    {
                        return this.TipoRemetente.Equals("J") ? String.Format(@"{0:00000000000000}", this.CNPJRemetente) : String.Format(@"{0:00000000000}", this.CNPJRemetente);
                    }
                }
                else
                    return "";
            }
        }

        public virtual string CPF_CNPJ_Remetente_Formatado
        {
            get
            {
                if (this.TipoDestinatario != null)
                {
                    if (this.TipoRemetente.Equals("E"))
                    {
                        return "00.000.000/0000-00";
                    }
                    else
                    {
                        return this.TipoRemetente.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CNPJRemetente) : String.Format(@"{0:000\.000\.000\-00}", this.CNPJRemetente);
                    }
                }
                else
                    return "";
            }
        }

        public string DescricaoTipoContabilizacao
        {
            get
            {
                switch (TipoContabilizacao)
                {
                    case Enumeradores.TipoContabilizacao.Todos:
                        return "";
                    case Enumeradores.TipoContabilizacao.Credito:
                        return "Crédito";
                    case Enumeradores.TipoContabilizacao.Debito:
                        return "Débito";
                    default:
                        return "";
                }
            }
        }

        public string ValorContabilizacaoFormatado
        {
            get
            {
                return this.ValorContabilizacao.ToString("n2");
            }
        }
    }
}
