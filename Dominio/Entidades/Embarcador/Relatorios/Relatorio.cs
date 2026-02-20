using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Relatorios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RELATORIO", EntityName = "Relatorio", Name = "Dominio.Entidades.Embarcador.Relatorios.Relatorio", NameType = typeof(Relatorio))]
    public class Relatorio : EntidadeBase, IEquatable<Relatorio>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "REL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoControleRelatorios", Column = "RCG_CODIGO_CONTROLE_RELATORIO", TypeType = typeof(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios), NotNull = true)]
        public virtual Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Titulo", Column = "REL_TITULO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "REL_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoRelatorio", Column = "REL_CAMINHO_RELATORIO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string CaminhoRelatorio { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "ArquivoRelatorio", Column = "REL_ARQUIVO_RELATORIO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string ArquivoRelatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "REL_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TimeOutMinutos", Column = "REL_TIME_OUT_MINUTOS", TypeType = typeof(int), NotNull = true)]
        public virtual int TimeOutMinutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Padrao", Column = "REL_PADRAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Padrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PadraoMultisoftware", Column = "REL_PADRAO_MULTISOFTWARE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PadraoMultisoftware { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirSumarios", Column = "REL_EXIBIR_SUMARIOS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExibirSumarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CortarLinhas", Column = "REL_CORTAR_LINHAS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CortarLinhas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FundoListrado", Column = "REL_FUNDO_LISTRADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool FundoListrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TamanhoPadraoFonte", Column = "REL_TAMANHO_PADRAO_FONTE", TypeType = typeof(int), NotNull = true)]
        public virtual int TamanhoPadraoFonte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FontePadrao", Column = "REL_FONTE_PADRAO", TypeType = typeof(string), Length = 30, NotNull = true)]
        public virtual string FontePadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeOrdena", Column = "REL_PROPRIEDADE_ORDENA", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string PropriedadeOrdena { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeAgrupa", Column = "REL_PROPRIEDADE_AGRUPA", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string PropriedadeAgrupa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrdemOrdenacao", Column = "REL_ORDEM_ORDENACAO", TypeType = typeof(string), Length = 10, NotNull = true)]
        public virtual string OrdemOrdenacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrdemAgrupamento", Column = "REL_ORDEM_AGRUPAMENTO", TypeType = typeof(string), Length = 10, NotNull = true)]
        public virtual string OrdemAgrupamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrientacaoRelatorio", Column = "REC_ORIENTACAO_RELATORIO", TypeType = typeof(Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio), NotNull = true)]
        public virtual Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio OrientacaoRelatorio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAlteracao", Column = "REL_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServicoMultisoftware", Column = "REL_TIPO_SERVICO_MULTISOFTWARE", TypeType = typeof(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware), NotNull = false)]
        public virtual AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? TipoServicoMultisoftware { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OcultarDetalhe", Column = "REL_OCULTAR_DETALHE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcultarDetalhe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RelatorioParaTodosUsuarios", Column = "REL_RELATORIO_PARA_TODOS_USUARIOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RelatorioParaTodosUsuarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NovaPaginaAposAgrupamento", Column = "REL_NOVA_PAGINA_APOS_AGRUPAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NovaPaginaAposAgrupamento { get; set; }

        public virtual IList<Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna> Colunas { get; set; }

        public virtual bool Equals(Relatorio other)
        {
            return (other.Codigo == this.Codigo);
        }

        public virtual string DescricaoPadrao
        {
            get
            {
                return this.Padrao ? "Sim" : "Não";
            }
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                return this.Ativo ? "Ativo" : "Inativo";
            }
        }

        public virtual ObjetosDeValor.Embarcador.Consulta.ParametroConsulta ObterParametrosConsulta()
        {
            return ObterParametrosConsulta(ObterPropriedadeOrdenar: null);
        }

        public virtual ObjetosDeValor.Embarcador.Consulta.ParametroConsulta ObterParametrosConsulta(Func<string, string> ObterPropriedadeOrdenar)
        {
            ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
            {
                DirecaoAgrupar = OrdemAgrupamento,
                DirecaoOrdenar = OrdemOrdenacao,
                InicioRegistros = 0,
                LimiteRegistros = 0,
                PropriedadeAgrupar = PropriedadeAgrupa
            };

            parametrosConsulta.PropriedadeOrdenar = (ObterPropriedadeOrdenar == null) ? PropriedadeOrdena : ObterPropriedadeOrdenar(PropriedadeOrdena);

            return parametrosConsulta;
        }
    }
}
