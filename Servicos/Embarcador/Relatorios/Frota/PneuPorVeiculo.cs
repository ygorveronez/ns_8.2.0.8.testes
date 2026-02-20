using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Frota;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Frota
{
    public class PneuPorVeiculo : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPneuPorVeiculo, Dominio.Relatorios.Embarcador.DataSource.Frota.PneuPorVeiculo>
    {

        #region Propriedades

        private readonly Repositorio.Embarcador.Frota.Pneu _repositorioPneuPorVeiculo;

        #endregion

        #region Construtores

        public PneuPorVeiculo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPneuPorVeiculo = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);
        }

        public PneuPorVeiculo(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPneuPorVeiculo = new Repositorio.Embarcador.Frota.Pneu(unitOfWork, cancellationToken);
        }

        #endregion

        #region
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuPorVeiculo>> ConsultarRegistrosAsync(FiltroPesquisaRelatorioPneuPorVeiculo filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioPneuPorVeiculo.ConsultarRelatorioPneuPorVeiculo(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuPorVeiculo> ConsultarRegistros(FiltroPesquisaRelatorioPneuPorVeiculo filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioPneuPorVeiculo.ConsultarRelatorioPneuPorVeiculo(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioPneuPorVeiculo filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioPneuPorVeiculo.ContarConsultaRelatorioPneuPorVeiculo(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Frota/PneuPorVeiculo";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaRelatorioPneuPorVeiculo filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Frota.Pneu repositorioPneu = new Repositorio.Embarcador.Frota.Pneu(_unitOfWork);
            Repositorio.Embarcador.Frota.ModeloPneu repositorioModeloPneu = new Repositorio.Embarcador.Frota.ModeloPneu(_unitOfWork);
            Repositorio.Embarcador.Frota.MarcaPneu repositorioMarcaPneu = new Repositorio.Embarcador.Frota.MarcaPneu(_unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repositorioSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repositorioCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);

            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repositorioVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;
            Dominio.Entidades.Embarcador.Frota.ModeloPneu modeloPneu = filtrosPesquisa.CodigoModeloPneu > 0 ? repositorioModeloPneu.BuscarPorCodigo(filtrosPesquisa.CodigoModeloPneu) : null;
            Dominio.Entidades.Embarcador.Frota.MarcaPneu marcaPneu = filtrosPesquisa.CodigoMarcaPneu > 0 ? repositorioMarcaPneu.BuscarPorCodigo(filtrosPesquisa.CodigoMarcaPneu) : null;
            Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista veiculoMotorista = filtrosPesquisa.CodigoMotorista > 0 ? repositorioVeiculoMotorista.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista) : null;
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = filtrosPesquisa.CodigoModeloVeicular > 0 ? repositorioModeloVeicularCarga.BuscarPorCodigo(filtrosPesquisa.CodigoModeloVeicular) : null;
            List<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo> segmentoVeiculo = filtrosPesquisa.CodigoSegmento.Count > 0 ? repositorioSegmentoVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoSegmento) : null;
            List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> centroResultado = filtrosPesquisa.CodigoCentroResultado.Count > 0 ? repositorioCentroResultado.BuscarPorCodigos(filtrosPesquisa.CodigoCentroResultado) : null;

            parametros.Add(new Parametro("Veiculo", veiculo?.Placa_Formatada ?? string.Empty));
            parametros.Add(new Parametro("MostrarSomentePosicoesVazias", filtrosPesquisa.CodigoMostrarSomentePosicoesVazias));
            parametros.Add(new Parametro("ModeloPneu", modeloPneu?.Descricao ?? string.Empty));
            parametros.Add(new Parametro("MarcaPneu", marcaPneu?.Descricao ?? string.Empty));
            parametros.Add(new Parametro("Motorista", veiculoMotorista?.Descricao ?? string.Empty));
            parametros.Add(new Parametro("ModeloVeicular", modeloVeicularCarga?.Descricao ?? string.Empty));
            parametros.Add(new Parametro("Reboque", filtrosPesquisa.CodigoReboque));
            parametros.Add(new Parametro("Segmento", segmentoVeiculo?.Select(o => o.Descricao)));
            parametros.Add(new Parametro("CentroResultado", centroResultado?.Select(o => o.Descricao)));

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
