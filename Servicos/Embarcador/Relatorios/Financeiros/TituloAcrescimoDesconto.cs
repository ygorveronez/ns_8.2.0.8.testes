using System.Collections.Generic;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Financeiros
{
    public class TituloAcrescimoDesconto : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTituloAcrescimoDesconto, Dominio.Relatorios.Embarcador.DataSource.Financeiros.TituloAcrescimoDesconto>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto _repositorioTituloAcrescimoDesconto;

        #endregion

        #region Construtores

        public TituloAcrescimoDesconto(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioTituloAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.TituloAcrescimoDesconto> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTituloAcrescimoDesconto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioTituloAcrescimoDesconto.ConsultarRelatorioTituloAcrescimoDesconto(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTituloAcrescimoDesconto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioTituloAcrescimoDesconto.ContarConsultaRelatorioTituloAcrescimoDesconto(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Financeiros/TituloAcrescimoDesconto";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTituloAcrescimoDesconto filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(_unitOfWork);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Bordero repBordero = new Repositorio.Embarcador.Financeiro.Bordero(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = filtrosPesquisa.CodigoFatura > 0 ? repFatura.BuscarPorCodigo(filtrosPesquisa.CodigoFatura) : null;
            Dominio.Entidades.Embarcador.Financeiro.Bordero bordero = filtrosPesquisa.CodigoBordero > 0 ? repBordero.BuscarPorCodigo(filtrosPesquisa.CodigoBordero) : null;
            List<Dominio.Entidades.Embarcador.Fatura.Justificativa> justificativas = filtrosPesquisa.CodigosJustificativa.Count > 0 ? repJustificativa.BuscarPorCodigos(filtrosPesquisa.CodigosJustificativa) : null;
            Dominio.Entidades.Cliente pessoa = filtrosPesquisa.CpfCnpjPessoa > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjPessoa) : null;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = filtrosPesquisa.CodigoGrupoPessoas > 0 ? repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoPessoas) : null;
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = filtrosPesquisa.CodigoCTe > 0 ? repCTe.BuscarPorCodigo(filtrosPesquisa.CodigoCTe) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", filtrosPesquisa.Tipo?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoJustificativa", filtrosPesquisa.TipoJustificativa?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoTitulo", filtrosPesquisa.SituacaoTitulo.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Documento", cte?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fatura", fatura?.Numero.ToString()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Bordero", bordero?.Numero.ToString()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Justificativa", justificativas != null ? string.Join(", ", justificativas.Select(o => o.Descricao)) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", pessoa != null ? pessoa.CPF_CNPJ_Formatado + " - " + pessoa.Nome : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", grupoPessoas?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoInicial", filtrosPesquisa.DataEmissaoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoFinal", filtrosPesquisa.DataEmissaoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataLiquidacaoInicial", filtrosPesquisa.DataLiquidacaoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataLiquidacaoFinal", filtrosPesquisa.DataLiquidacaoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataBaseLiquidacaoInicial", filtrosPesquisa.DataBaseLiquidacaoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataBaseLiquidacaoFinal", filtrosPesquisa.DataBaseLiquidacaoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoDeTitulo", filtrosPesquisa.TipoDeTitulo.ObterDescricao()));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}