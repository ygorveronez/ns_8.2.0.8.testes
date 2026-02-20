using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Relatorios
{
    public class ConfiguracaoRelatorio
    {
        #region Propriedades

        public virtual Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorios { get; set; }

        public virtual string Titulo { get; set; }

        public virtual string Descricao { get; set; }

        public virtual string CaminhoRelatorio { get; set; }

        public virtual string ArquivoRelatorio { get; set; }

        public virtual bool Ativo { get; set; }

        public virtual int TimeOutMinutos { get; set; }

        public virtual bool Padrao { get; set; }

        public virtual bool PadraoMultisoftware { get; set; }

        public virtual bool ExibirSumarios { get; set; }

        public virtual bool CortarLinhas { get; set; }

        public virtual bool FundoListrado { get; set; }

        public virtual int TamanhoPadraoFonte { get; set; }

        public virtual string FontePadrao { get; set; }

        public virtual string PropriedadeOrdena { get; set; }

        public virtual string PropriedadeAgrupa { get; set; }

        public virtual string OrdemOrdenacao { get; set; }

        public virtual string OrdemAgrupamento { get; set; }

        public virtual Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio OrientacaoRelatorio { get; set; }

        public virtual bool OcultarDetalhe { get; set; }

        public virtual bool NovaPaginaAposAgrupamento { get; set; }

        public virtual AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? TipoServicoMultisoftware { get; set; }

        public virtual bool RelatorioParaTodosUsuarios { get; set; }

        public virtual IList<ConfiguracaoRelatorioColuna> Colunas { get; set; }

        #endregion Propriedades

        #region Métodos Públicos

        public virtual Consulta.ParametroConsulta ObterParametrosConsulta()
        {
            Consulta.ParametroConsulta parametrosConsulta = new Consulta.ParametroConsulta()
            {
                DirecaoAgrupar = OrdemAgrupamento,
                DirecaoOrdenar = OrdemOrdenacao,
                InicioRegistros = 0,
                LimiteRegistros = 0,
                PropriedadeAgrupar = PropriedadeAgrupa,
                PropriedadeOrdenar = PropriedadeOrdena
            };

            return parametrosConsulta;
        }

        public virtual Consulta.ParametroConsulta ObterParametrosConsulta(Func<string, string> ObterPropriedadeOrdenar)
        {
            Consulta.ParametroConsulta parametrosConsulta = new Consulta.ParametroConsulta()
            {
                DirecaoAgrupar = OrdemAgrupamento,
                DirecaoOrdenar = OrdemOrdenacao,
                InicioRegistros = 0,
                LimiteRegistros = 0,
                PropriedadeAgrupar = PropriedadeAgrupa,
                PropriedadeOrdenar = PropriedadeOrdena
            };

            parametrosConsulta.PropriedadeOrdenar = (ObterPropriedadeOrdenar == null) ? PropriedadeOrdena : ObterPropriedadeOrdenar(PropriedadeOrdena);

            return parametrosConsulta;
        }

        #endregion Métodos Públicos
    }
}
