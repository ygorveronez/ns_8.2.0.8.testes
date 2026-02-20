using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Frotas
{
    public class Motorista : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotorista, Dominio.Relatorios.Embarcador.DataSource.Frota.Motorista>
    {
        #region Atributos

        private readonly Repositorio.Usuario _repositorioUsuario;

        #endregion

        #region Construtores

        public Motorista(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
        }

        public Motorista(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioUsuario = new Repositorio.Usuario(_unitOfWork, cancellationToken);
        }

        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.Motorista>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotorista filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioUsuario.ConsultarRelatorioMotoristaAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Frota.Motorista> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotorista filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioUsuario.ConsultarRelatorioMotorista(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotorista filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioUsuario.ContarConsultaRelatorioMotorista(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Frotas/Motorista";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotorista filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Configuracoes.Licenca repLicenca = new Repositorio.Embarcador.Configuracoes.Licenca(_unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);

            Dominio.Entidades.Usuario motorista = filtrosPesquisa.CodigoMotorista > 0 ? repMotorista.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista) : null;
            Dominio.Entidades.Usuario gestor = filtrosPesquisa.CodigoGestor > 0 ? repMotorista.BuscarPorCodigo(filtrosPesquisa.CodigoGestor) : null;
            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;
            Dominio.Entidades.Embarcador.Configuracoes.Licenca licenca = filtrosPesquisa.CodigoTipoLicenca > 0 ? repLicenca.BuscarPorCodigo(filtrosPesquisa.CodigoTipoLicenca) : null;
            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = filtrosPesquisa.CodigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(filtrosPesquisa.CodigoCentroResultado) : null;

            parametros.Add(new Parametro("Motorista", motorista?.Nome));
            parametros.Add(new Parametro("Nome", filtrosPesquisa.Nome));
            parametros.Add(new Parametro("CPF", filtrosPesquisa.CPF));
            parametros.Add(new Parametro("CodigoIntegracao", filtrosPesquisa.CodigoIntegracao));

            if (filtrosPesquisa.TipoMotorista == TipoMotorista.Proprio)
                parametros.Add(new Parametro("TipoMotorista", "Próprio", true));
            else if (filtrosPesquisa.TipoMotorista == TipoMotorista.Terceiro)
                parametros.Add(new Parametro("TipoMotorista", "Terceiro", true));
            else
                parametros.Add(new Parametro("TipoMotorista", "Todos", true));

            if (filtrosPesquisa.Status == SituacaoAtivoPesquisa.Ativo)
                parametros.Add(new Parametro("Situacao", "Ativo", true));
            else if (filtrosPesquisa.Status == SituacaoAtivoPesquisa.Inativo)
                parametros.Add(new Parametro("Situacao", "Inativo", true));
            else
                parametros.Add(new Parametro("Situacao", "Todos", true));

            parametros.Add(new Parametro("Veiculo", veiculo?.Placa));
            parametros.Add(new Parametro("Licenca", licenca?.Descricao));
            parametros.Add(new Parametro("SituacaoColaborador", filtrosPesquisa.SituacaoColaborador.ObterDescricao()));
            parametros.Add(new Parametro("Aposentadoria", filtrosPesquisa.Aposentadoria.ObterDescricao()));
            parametros.Add(new Parametro("CentroResultado", centroResultado?.Descricao));
            parametros.Add(new Parametro("Bloqueado", filtrosPesquisa.Bloqueado));
            parametros.Add(new Parametro("UsuarioMobile", filtrosPesquisa.UsuarioMobile));
            parametros.Add(new Parametro("NaoBloquearAcessoSimultaneo", filtrosPesquisa.NaoBloquearAcessoSimultaneo));
            parametros.Add(new Parametro("Gestor", gestor?.Nome));

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