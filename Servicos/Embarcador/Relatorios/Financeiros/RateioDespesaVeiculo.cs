using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Financeiros
{
    public class RateioDespesaVeiculo : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRateioDespesaVeiculo, Dominio.Relatorios.Embarcador.DataSource.Financeiros.RateioDespesaVeiculo>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo _repositorioRateioDespesaVeiculo;

        #endregion

        #region Construtores

        public RateioDespesaVeiculo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioRateioDespesaVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RateioDespesaVeiculo> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRateioDespesaVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioRateioDespesaVeiculo.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRateioDespesaVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioRateioDespesaVeiculo.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Financeiros/RateioDespesaVeiculo";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRateioDespesaVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);

            Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira repTipoDespesa = new Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira repGrupoDespesa = new Repositorio.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(_unitOfWork);
            Repositorio.Cliente repPessoa = new Repositorio.Cliente(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira> tiposDespesas = filtrosPesquisa.CodigosTipoDespesa?.Count > 0 ? repTipoDespesa.BuscarPorCodigos(filtrosPesquisa.CodigosTipoDespesa) : null;
            List<Dominio.Entidades.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira> gruposDespesas = filtrosPesquisa.CodigosGrupoDespesa?.Count > 0 ? repGrupoDespesa.BuscarPorCodigos(filtrosPesquisa.CodigosGrupoDespesa) : null;
            List<Dominio.Entidades.Veiculo> veiculos = filtrosPesquisa.CodigosVeiculo?.Count > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigosVeiculo) : null;
            List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> centrosResultados = filtrosPesquisa.CodigosCentroResultado?.Count > 0 ? repCentroResultado.BuscarPorCodigos(filtrosPesquisa.CodigosCentroResultado) : null;
            List<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo> segmentosVeiculos = filtrosPesquisa.CodigosSegmentoVeiculo?.Count > 0 ? repSegmentoVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigosSegmentoVeiculo) : null;
            Dominio.Entidades.Cliente pessoa = filtrosPesquisa.CpfCnpjPessoa > 0 ? repPessoa.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjPessoa) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Periodo", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoDespesa", tiposDespesas?.Select(o => string.Join(", ", o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoDespesa", gruposDespesas?.Select(o => string.Join(", ", o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculos?.Select(o => string.Join(", ", o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroResultado", centrosResultados?.Select(o => string.Join(", ", o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Segmento", segmentosVeiculos?.Select(o => string.Join(", ", o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", pessoa?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataLancamento", filtrosPesquisa.DataLancamentoInicial, filtrosPesquisa.DataLancamentoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("OrigemRateio", filtrosPesquisa.OrigemRateio?.ObterDescricao()));

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