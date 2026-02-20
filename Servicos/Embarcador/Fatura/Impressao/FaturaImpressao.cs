using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Servicos.Embarcador.Fatura
{
    public abstract class FaturaImpressao
    {
        #region Atributos

        protected readonly CodigoControleRelatorios _codigoControleRelatorios;
        protected readonly Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        protected readonly string _nomeRelatorio;
        protected readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        protected readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores 

        public FaturaImpressao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, string nomeRelatorio, CodigoControleRelatorios codigoControleRelatorios)
        {
            _codigoControleRelatorios = codigoControleRelatorios;
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _nomeRelatorio = nomeRelatorio;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Protegidos

        protected List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            if (fatura.Usuario != null)
            {
                Dominio.Entidades.Usuario operador = repositorioUsuario.BuscarPorCodigo(fatura.Usuario.Codigo);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Operador", operador?.Nome ?? "", true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Operador", "Automático", true));

            return parametros;
        }

        protected Dominio.Entidades.Embarcador.Relatorios.Relatorio ObterRelatorioTemporarioBase(Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio)
        {
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = new Dominio.Entidades.Embarcador.Relatorios.Relatorio();

            relatorioTemporario.ArquivoRelatorio = relatorio.ArquivoRelatorio;
            relatorioTemporario.Ativo = relatorio.Ativo;
            relatorioTemporario.CaminhoRelatorio = relatorio.CaminhoRelatorio;
            relatorioTemporario.CodigoControleRelatorios = relatorio.CodigoControleRelatorios;
            relatorioTemporario.TimeOutMinutos = relatorio.TimeOutMinutos;
            relatorioTemporario.CortarLinhas = relatorio.CortarLinhas;
            relatorioTemporario.Descricao = relatorio.Descricao;
            relatorioTemporario.ExibirSumarios = relatorio.ExibirSumarios;
            relatorioTemporario.FontePadrao = relatorio.FontePadrao;
            relatorioTemporario.PropriedadeAgrupa = relatorio.PropriedadeAgrupa;
            relatorioTemporario.OrdemAgrupamento = relatorio.OrdemAgrupamento;
            relatorioTemporario.PropriedadeOrdena = relatorio.PropriedadeOrdena;
            relatorioTemporario.OrdemOrdenacao = relatorio.OrdemOrdenacao;
            relatorioTemporario.FundoListrado = relatorio.FundoListrado;
            relatorioTemporario.OrientacaoRelatorio = relatorio.OrientacaoRelatorio;
            relatorioTemporario.TamanhoPadraoFonte = relatorio.TamanhoPadraoFonte;
            relatorioTemporario.Titulo = relatorio.Titulo;
            relatorioTemporario.Padrao = relatorio.Padrao;
            relatorioTemporario.TipoServicoMultisoftware = _tipoServicoMultisoftware;
            relatorioTemporario.OcultarDetalhe = relatorio.OcultarDetalhe;
            relatorioTemporario.RelatorioParaTodosUsuarios = relatorio.RelatorioParaTodosUsuarios;

            return relatorioTemporario;
        }

        protected Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna ObterRelatorioTemporarioColuna(Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio, string propriedade, int posicao)
        {
            return ObterRelatorioTemporarioColuna(relatorio, propriedade, posicao, titulo: "", alinhamento: Alinhamento.left, tamanhoColuna: 0, tipoSumarizacao: TipoSumarizacao.nenhum);
        }

        protected Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna ObterRelatorioTemporarioColuna(Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio, string propriedade, int posicao, string titulo, Alinhamento alinhamento, decimal tamanhoColuna)
        {
            return ObterRelatorioTemporarioColuna(relatorio, propriedade, posicao, titulo, alinhamento, tamanhoColuna, tipoSumarizacao: TipoSumarizacao.nenhum);
        }

        protected Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna ObterRelatorioTemporarioColuna(Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio, string propriedade, int posicao, string titulo, Alinhamento alinhamento, decimal tamanhoColuna, TipoSumarizacao tipoSumarizacao)
        {
            Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna coluna = new Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna();

            coluna.Alinhamento = alinhamento;
            coluna.Posicao = posicao;
            coluna.Propriedade = propriedade;
            coluna.Relatorio = relatorio;
            coluna.Tamanho = tamanhoColuna;
            coluna.TipoSumarizacao = tipoSumarizacao;
            coluna.Titulo = titulo;
            coluna.Visivel = tamanhoColuna > 0m;

            return coluna;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Relatorios.Relatorio ObterRelatorio()
        {
            Relatorios.Relatorio servicoRelatorio = new Relatorios.Relatorio(_unitOfWork);
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorios, _tipoServicoMultisoftware, "Fatura Duplicata", "Fatura", _nomeRelatorio, OrientacaoRelatorio.Retrato, "Codigo", "asc", "", "", 0, _unitOfWork, true, true);

            return relatorio;
        }

        public string ObterGuidArquivoUltimoRelatorioGerado(Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repositorioRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao ultimoRelatorioGerado = repositorioRelatorioControleGeracao.BuscarUltimoRelatorioPorEntidade(fatura.Codigo, _codigoControleRelatorios);

            return ultimoRelatorioGerado?.GuidArquivo ?? string.Empty;
        }

        #endregion

        #region Métodos Públicos Abstratos

        public abstract void GerarRelatorio(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura? tipoImpressaoFatura = null);

        public abstract Dominio.Entidades.Embarcador.Relatorios.Relatorio ObterRelatorioTemporario(Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio);

        #endregion
    }
}
