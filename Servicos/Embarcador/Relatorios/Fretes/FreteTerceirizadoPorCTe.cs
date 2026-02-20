using System.Linq;
using System.Collections.Generic;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Fretes
{
    public class FreteTerceirizadoPorCTe : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoPorCTe, Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoPorCTe>
    {
        #region Atributos
        private readonly Repositorio.Embarcador.Terceiros.ContratoFrete _repositorioContratoFrete;

        #endregion

        #region Construtores

        public FreteTerceirizadoPorCTe(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
        }

        public FreteTerceirizadoPorCTe(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork, cancellationToken);
        }

        #endregion

        #region
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoPorCTe>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoPorCTe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioContratoFrete.ConsultarRelatorioFreteTerceirizadoPorCTeAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoPorCTe> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoPorCTe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioContratoFrete.ConsultarRelatorioFreteTerceirizadoPorCTe(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoPorCTe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioContratoFrete.ContarConsultaRelatorioFreteTerceirizadoPorCTe(filtrosPesquisa, propriedadesAgrupamento, null, null, null, null, 0, 0);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Fretes/FreteTerceirizadoPorCTe";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoPorCTe filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);

            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.Veiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.Veiculo) : null;
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = filtrosPesquisa.ModeloVeicular > 0 ? repModeloVeicularCarga.BuscarPorCodigo(filtrosPesquisa.ModeloVeicular) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.TipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.TipoOperacao) : null;
            Dominio.Entidades.Empresa empresa = filtrosPesquisa.Empresa > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.Empresa) : null;

            List<string> DescricaoTerceiros = new List<string>();
            string terceiros = string.Empty;

            if (filtrosPesquisa.CpfCnpjTerceiros.Count > 0)
            {
                foreach (var cpfCnpjTerceiro in filtrosPesquisa.CpfCnpjTerceiros)
                {
                    Dominio.Entidades.Cliente terceiro = cpfCnpjTerceiro > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjTerceiro) : null;

                    if (terceiro != null)
                        DescricaoTerceiros.Add(terceiro.Descricao);
                }

                terceiros = string.Join(",", DescricaoTerceiros);
            }

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Terceiro", terceiros));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoContratoInicial", filtrosPesquisa.DataEmissaoContratoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoContratoFinal", filtrosPesquisa.DataEmissaoContratoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroContrato", filtrosPesquisa.NumeroContrato));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCarga", filtrosPesquisa.NumeroCarga));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCTe", filtrosPesquisa.NumeroCTe));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo?.Placa));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", filtrosPesquisa.Situacao?.Select(o => o.ObterDescricao())));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeicular", modeloVeicularCarga?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAprovacaoInicial", filtrosPesquisa.DataAprovacaoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAprovacaoFinal", filtrosPesquisa.DataAprovacaoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEncerramentoInicial", filtrosPesquisa.DataEncerramentoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEncerramentoFinal", filtrosPesquisa.DataEncerramentoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAberturaCIOT", filtrosPesquisa.DataAberturaCIOTInicial, filtrosPesquisa.DataAberturaCIOTFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEncerramentoCIOT", filtrosPesquisa.DataEncerramentoCIOTInicial, filtrosPesquisa.DataEncerramentoCIOTFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tipoOperacao?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCIOT", filtrosPesquisa.NumeroCIOT));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoProprietario", filtrosPesquisa.TipoProprietario?.ObterDescricao()));

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