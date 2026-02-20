using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Veiculos
{
    public class Veiculos : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculo, Dominio.Relatorios.Embarcador.DataSource.Veiculos.Veiculo>
    {
        #region Atributos

        private readonly Repositorio.Veiculo _repositorioVeiculo;

        #endregion

        #region Construtores

        public Veiculos(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
        }

        public Veiculos(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork, cancellationToken);
        }

        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Veiculo>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioVeiculo.ConsultarRelatorioAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList meotodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Veiculo> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioVeiculo.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioVeiculo.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Veiculos/Veiculo";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(_unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = filtrosPesquisa.CodigoCentroCarregamento > 0 ? repCentroCarregamento.BuscarPorCodigo(filtrosPesquisa.CodigoCentroCarregamento) : null;
            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = filtrosPesquisa.CodigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(filtrosPesquisa.CodigoCentroResultado) : null;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Placa", filtrosPesquisa.Placa.ToUpper(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Placa", false));

            if (filtrosPesquisa.CpfcnpjProprietario > 0)
            {
                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfcnpjProprietario);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Proprietario", $"{cliente.CPF_CNPJ_Formatado} - {cliente.Nome}", true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Proprietario", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoVeiculo) && filtrosPesquisa.TipoVeiculo != "-1")
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoVeiculo", filtrosPesquisa.TipoVeiculo == "0" ? "Tração" : "Reboque", true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoVeiculo", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Tipo) && filtrosPesquisa.Tipo != "A")
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoPropriedade", filtrosPesquisa.Tipo == "P" ? "Própria" : "Terceiros", true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoPropriedade", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chassi))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Chassi", filtrosPesquisa.Chassi.ToUpper(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Chassi", false));

            if (filtrosPesquisa.CodigoEmpresa > 0)
            {
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoEmpresa);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresa.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", usuario.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", false));

            if (filtrosPesquisa.CodigosSegmento != null && filtrosPesquisa.CodigosSegmento.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo> segmentos = filtrosPesquisa.CodigosSegmento.Count > 0 ? repSegmentoVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigosSegmento.ToArray()) : new List<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo>();
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Segmento", string.Join(", ", from obj in segmentos select obj.Descricao), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Segmento", false));

            if (filtrosPesquisa.CodigosFuncionarioResponsavel != null && filtrosPesquisa.CodigosFuncionarioResponsavel.Count > 0)
            {
                List<Dominio.Entidades.Usuario> funcionarios = filtrosPesquisa.CodigosFuncionarioResponsavel.Count > 0 ? repUsuario.BuscarUsuariosPorCodigos(filtrosPesquisa.CodigosFuncionarioResponsavel.ToArray(), null) : new List<Dominio.Entidades.Usuario>();
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("FuncionarioResponsavel", string.Join(", ", from obj in funcionarios select obj.Nome), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("FuncionarioResponsavel", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroCarregamento", centroCarregamento?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroResultado", centroResultado?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VeiculoPossuiTagValePedagio", filtrosPesquisa.VeiculoPossuiTagValePedagio ? "Sim" : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCadastro", filtrosPesquisa.DataCadastroInicial, filtrosPesquisa.DataCadastroFinal, false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Bloqueado", filtrosPesquisa.Bloqueado));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TagSemParar", filtrosPesquisa.TagSemParar));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "CnpjTransportadorFormatado")
                return "CnpjTransportador";

            if (propriedadeOrdenarOuAgrupar == "DataAtualizacaoFormatada")
                return "DataAtualizacao";

            if (propriedadeOrdenarOuAgrupar == "DataValidadeGerenciadoraRiscoFormatada")
                return "DataValidadeGerenciadoraRisco";

            if (propriedadeOrdenarOuAgrupar == "DataValidadeLiberacaoSeguradoraFormatada")
                return "DataValidadeLiberacaoSeguradora";

            if (propriedadeOrdenarOuAgrupar == "DataAquisicaoFormatada")
                return "DataAquisicao";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}