using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Frota
{
    public class ManutencaoVeiculo : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioManutencaoVeiculo, Dominio.Relatorios.Embarcador.DataSource.Frota.ManutencaoVeiculo>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Frota.OrdemServicoFrota _repositorioOrdemServicoFrota;

        #endregion

        #region Construtores

        public ManutencaoVeiculo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(_unitOfWork);
        }

        public ManutencaoVeiculo(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(_unitOfWork, cancellationToken);
        }

        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.ManutencaoVeiculo>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioManutencaoVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioOrdemServicoFrota.RelatorioManutencaoVeiculoAsync(filtrosPesquisa, parametrosConsulta);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Frota.ManutencaoVeiculo> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioManutencaoVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioOrdemServicoFrota.RelatorioManutencaoVeiculo(filtrosPesquisa, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioManutencaoVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioOrdemServicoFrota.ContarRelatorioManutencaoVeiculo(filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Frotas/ManutencaoVeiculo";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioManutencaoVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(_unitOfWork);
            Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiculoFrota = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(_unitOfWork);
            Repositorio.MarcaVeiculo repMarcaVeiculo = new Repositorio.MarcaVeiculo(_unitOfWork);
            Repositorio.Embarcador.Veiculos.ModeloEquipamento repModeloEquipamento = new Repositorio.Embarcador.Veiculos.ModeloEquipamento(_unitOfWork);
            Repositorio.Embarcador.Veiculos.MarcaEquipamento repMarcaEquipamento = new Repositorio.Embarcador.Veiculos.MarcaEquipamento(_unitOfWork);

            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);

            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(_unitOfWork);

            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;
            Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = filtrosPesquisa.CodigoEquipamento > 0 ? repEquipamento.BuscarPorCodigo(filtrosPesquisa.CodigoEquipamento) : null;
            List<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota> servicos = filtrosPesquisa.CodigosServico.Count > 0 ? repServicoVeiculoFrota.BuscarPorCodigo(filtrosPesquisa.CodigosServico.ToArray()) : new List<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota>();
            Dominio.Entidades.Cliente localManutencao = filtrosPesquisa.CnpjCpfLocalManutencao > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CnpjCpfLocalManutencao) : null;
            Dominio.Entidades.ModeloVeiculo modeloVeiculo = filtrosPesquisa.CodigoModeloVeiculo > 0 ? repModeloVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoModeloVeiculo, false) : null;
            Dominio.Entidades.MarcaVeiculo marcaVeiculo = filtrosPesquisa.CodigoMarcaVeiculo > 0 ? repMarcaVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoMarcaVeiculo, false) : null;
            Dominio.Entidades.Embarcador.Veiculos.ModeloEquipamento modeloEquipamento = filtrosPesquisa.CodigoModeloEquipamento > 0 ? repModeloEquipamento.BuscarPorCodigo(filtrosPesquisa.CodigoModeloEquipamento) : null;
            Dominio.Entidades.Embarcador.Veiculos.MarcaEquipamento marcaEquipamento = filtrosPesquisa.CodigoMarcaEquipamento > 0 ? repMarcaEquipamento.BuscarPorCodigo(filtrosPesquisa.CodigoMarcaEquipamento) : null;
            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = filtrosPesquisa.CodigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(filtrosPesquisa.CodigoCentroResultado) : null;
            Dominio.Entidades.Usuario funcionario = filtrosPesquisa.CodigoFuncionarioResponsavel > 0 ? repFuncionario.BuscarPorCodigo(filtrosPesquisa.CodigoFuncionarioResponsavel) :null;
            List<string> segmentosVeiculo = filtrosPesquisa.CodigosSegmentoVeiculo.Count > 0 ? repSegmentoVeiculo.BuscarDescricoesPorCodigos(filtrosPesquisa.CodigosSegmentoVeiculo) : new List<string>();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo?.Placa));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Equipamento", equipamento?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Servico", string.Join(", ", from obj in servicos select obj.Descricao)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PendentesManutencao", filtrosPesquisa.VisualizarServicosPendentesManutencao ? "Sim" : "Não"));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoManutencao", string.Join(", ", filtrosPesquisa.TiposManutencao?.Select(o => o.ObterDescricao()))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("LocalManutencao", localManutencao?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SomenteServicosExecutadosAnteriormente", filtrosPesquisa.VisualizarSomenteServicosExecutadosAnteriormente ? "Sim" : "Não"));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("KMAtual", filtrosPesquisa.KMAtual));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("HorimetroAtual", filtrosPesquisa.HorimetroAtual));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VeiculosEquipamentosAcoplados", filtrosPesquisa.VisualizarVeiculosEquipamentosAcoplados ? "Sim" : "Não"));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeiculo", modeloVeiculo?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("MarcaVeiculo", marcaVeiculo?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloEquipamento", modeloEquipamento?.Descricao ?? ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("MarcaEquipamento", marcaEquipamento?.Descricao ?? ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SegmentoVeiculo", segmentosVeiculo));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("FuncionarioResponsavel", funcionario?.Nome ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroResultados", centroResultado?.Descricao ?? string.Empty)) ;


            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}