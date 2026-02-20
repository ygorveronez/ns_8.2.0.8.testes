using NHibernate.Mapping.Attributes;
using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [Class(0, Table = "T_GRUPO_MOTORISTAS_INTEGRACAO_ARQUIVOS", EntityName = "GrupoMotoristasIntegracaoArquivos", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracaoArquivos", NameType = typeof(GrupoMotoristasIntegracaoArquivos))]
    public class GrupoMotoristasIntegracaoArquivos : Integracao.IntegracaoArquivo, Interfaces.Embarcador.Logistica.GrupoMotoristas.IEntidadeRelacionamentoGrupoMotoristasIntegracao
    {
        [Id(Name = "Codigo", Type = "int", Column = "GMA_CODIGO")]
        [Generator(Class = "native")]
        public override int Codigo { get; set; }

        [Obsolete("Não sera usado.")]
        [ManyToOne(0, Class = "GrupoMotoristasIntegracao", Column = "GMI_CODIGO", NotNull = false, Lazy = Laziness.Proxy)]
        public virtual GrupoMotoristasIntegracao GrupoMotoristasIntegracao { get; set; }

        [Property(Name = "Data", Column = "GMA_DATA_HORA", TypeType = typeof(DateTime), NotNull = true)]
        public override DateTime Data { get; set; }

        [Property(Name = "Tipo", Column = "GMA_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga), NotNull = true)]
        public override ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga Tipo { get; set; }

        [Property(Name = "Mensagem", Column = "GMA_MENSAGEM", TypeType = typeof(string), Length = 500, NotNull = false)]
        public override string Mensagem { get; set; }

        [ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_REQUISICAO", NotNull = true, Lazy = Laziness.Proxy)]
        public override Integracao.ArquivoIntegracao ArquivoRequisicao { get; set; }

        [ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_RESPOSTA", NotNull = true, Lazy = Laziness.Proxy)]
        public override Integracao.ArquivoIntegracao ArquivoResposta { get; set; }

    }
}