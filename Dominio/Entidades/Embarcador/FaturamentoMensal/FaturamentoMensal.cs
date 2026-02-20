using System;

namespace Dominio.Entidades.Embarcador.FaturamentoMensal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURAMENTO_MENSAL", EntityName = "FaturamentoMensal", Name = "Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal", NameType = typeof(FaturamentoMensal))]
    public class FaturamentoMensal : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FME_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProcessamento", Column = "FME_DATA_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalizacao", Column = "FME_DATA_FINALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusFaturamentoMensal", Column = "FME_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal StatusFaturamentoMensal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GruposFaturamento", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + ISNULL(G.FMG_DESCRICAO, '')
	                                                                                    FROM T_FATURAMENTO_MENSAL_CLIENTE_SERVICO CS
                                                                                        JOIN T_FATURAMENTO_MENSAL_CLIENTE FC ON FC.FMC_CODIGO = CS.FMC_CODIGO
                                                                                        JOIN T_FATURAMENTO_MENSAL_GRUPO G ON G.FMG_CODIGO = FC.FMG_CODIGO
                                                                                        WHERE CS.FME_CODIGO = FME_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string GruposFaturamento { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.DataProcessamento?.ToString("dd/MM/yyyy") ?? string.Empty;
            }
        }

        public virtual string DescricaStatusFaturamentoMensal
        {
            get
            {
                switch (this.StatusFaturamentoMensal)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.DocumentosAutorizados:
                        return "Documentos Autorizados";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.Finalizado:
                        return "Finalizado";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.GeradoBoletos:
                        return "Gerado Boletos";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.GeradoDocumentos:
                        return "Gerado Documentos";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.Iniciada:
                        return "Iniciada";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.Cancelado:
                        return "Cancelado";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.AguardandoAutorizacaoDocumento:
                        return "Aguardando Autorização dos Documentos";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.AguardandoEnvioEmail:
                        return "Aguardando Envio dos E-mails";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.EmGeracaoAutorizacaoDocumento:
                        return "Autorizando Documentos";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.EmGeracaoEnvioEmail:
                        return "Enviando E-mails";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(FaturamentoMensal other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
