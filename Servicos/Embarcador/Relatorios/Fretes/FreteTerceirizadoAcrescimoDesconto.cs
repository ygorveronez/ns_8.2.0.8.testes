using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Fretes
{
    public class FreteTerceirizadoAcrescimoDesconto : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoAcrescimoDesconto, Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoAcrescimoDesconto>
    {
        #region Atributos
        private readonly Repositorio.Embarcador.Terceiros.ContratoFrete _repositorioContratoFrete;

        #endregion

        #region Construtores

        public FreteTerceirizadoAcrescimoDesconto(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
        }

        public FreteTerceirizadoAcrescimoDesconto(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork, cancellationToken);
        }
        #endregion

        #region Métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoAcrescimoDesconto>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoAcrescimoDesconto filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioContratoFrete.ConsultarRelatorioFreteTerceirizadoAcrescimoDescontoAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #endregion


        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoAcrescimoDesconto> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoAcrescimoDesconto filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioContratoFrete.ConsultarRelatorioFreteTerceirizadoAcrescimoDesconto(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoAcrescimoDesconto filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioContratoFrete.ContarConsultaRelatorioFreteTerceirizadoAcrescimoDesconto(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Fretes/FreteTerceirizadoAcrescimoDesconto";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoAcrescimoDesconto filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            Dominio.Entidades.Cliente terceiro = filtrosPesquisa.CpfCnpjTerceiro > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjTerceiro) : null;
            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.Veiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.Veiculo) : null;

            parametros.Add(new Parametro("Terceiro", terceiro?.Descricao));
            parametros.Add(new Parametro("DataEmissaoContratoInicial", filtrosPesquisa.DataEmissaoContratoInicial));
            parametros.Add(new Parametro("DataEmissaoContratoFinal", filtrosPesquisa.DataEmissaoContratoFinal));
            parametros.Add(new Parametro("NumeroContrato", filtrosPesquisa.NumeroContrato));
            parametros.Add(new Parametro("NumeroCarga", filtrosPesquisa.NumeroCarga));
            parametros.Add(new Parametro("Veiculo", veiculo?.Placa));
            parametros.Add(new Parametro("Situacao", filtrosPesquisa.Situacao?.Select(o => o.ObterDescricao())));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}
