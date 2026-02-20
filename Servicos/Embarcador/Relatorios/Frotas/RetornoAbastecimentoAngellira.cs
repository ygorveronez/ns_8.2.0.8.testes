using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Frotas
{
    public class RetornoAbastecimentoAngellira : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioRetornoAbastecimentoAngellira, Dominio.Relatorios.Embarcador.DataSource.Frota.RetornoAbastecimentoAngellira>
    {
        #region Atributos

        private readonly Repositorio.Abastecimento _repositorioAbastecimento;

        #endregion

        #region Construtores

        public RetornoAbastecimentoAngellira(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioAbastecimento = new Repositorio.Abastecimento(_unitOfWork);
        }

        public RetornoAbastecimentoAngellira(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioAbastecimento = new Repositorio.Abastecimento(_unitOfWork, cancellationToken);
        }

        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.RetornoAbastecimentoAngellira>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioRetornoAbastecimentoAngellira filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioAbastecimento.RelatorioRetornoAbastecimentoAngelliraAsync(filtrosPesquisa, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Frota.RetornoAbastecimentoAngellira> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioRetornoAbastecimentoAngellira filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioAbastecimento.RelatorioRetornoAbastecimentoAngellira(filtrosPesquisa, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioRetornoAbastecimentoAngellira filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioAbastecimento.ContarRelatorioRetornoAbastecimentoAngellira(filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Frotas/RetornoAbastecimentoAngellira";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioRetornoAbastecimentoAngellira filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);            
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(_unitOfWork);
            
            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;
            Dominio.Entidades.Usuario motorista = filtrosPesquisa.CodigoMotorista > 0 ? repMotorista.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista) : null;
            Dominio.Entidades.Cliente fornecedor = filtrosPesquisa.Fornecedor > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.Fornecedor) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo?.Placa));            
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", motorista?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fornecedor", fornecedor?.Descricao));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            if (propriedadeOrdenarOuAgrupar == "CpfCnpjFornecedorFormatado")
                return "CNPJCPFPosto";

            if (propriedadeOrdenarOuAgrupar == "DescricaoSituacaoIntegracao")
                return "SituacaoIntegracao";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}