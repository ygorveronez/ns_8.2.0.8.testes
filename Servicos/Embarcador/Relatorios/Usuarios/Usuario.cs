using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Usuarios
{
    public class Usuario : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Usuarios.FiltroPesquisaRelatorioUsuario, Dominio.Relatorios.Embarcador.DataSource.Usuarios.Usuario>
    {
        #region Atributos

        private readonly Repositorio.Usuario _repositorioUsuario;

        #endregion

        #region Construtores

        public Usuario(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
        }

        public Usuario(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioUsuario = new Repositorio.Usuario(_unitOfWork, cancellationToken);
        }

        #endregion

        #region Métodos Privados

        private string ObterDescricaoAmbiente(TipoAcesso? ambiente)
        {
            if (ambiente == TipoAcesso.Emissao)
                return "Transportador";

            if (ambiente == TipoAcesso.Embarcador)
                return "Embarcador";

            return "";
        }

        #endregion Métodos Privados

        #region Métodos Protegidos Sobrescritos
        //Async Methods
        //Lista Usuários
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Usuarios.Usuario>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Usuarios.FiltroPesquisaRelatorioUsuario filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioUsuario.ConsultarRelatorioUsuariosAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros);
        }
        //Conta usuários para listagem
        public async Task<int> ContarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Usuarios.FiltroPesquisaRelatorioUsuario filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return await _repositorioUsuario.ContarConsultaRelatorioUsuariosAsync(filtrosPesquisa, propriedadesAgrupamento);
        }
        // TODO: ToList meotodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Usuarios.Usuario> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Usuarios.FiltroPesquisaRelatorioUsuario filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioUsuario.ConsultarRelatorioUsuarios(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Usuarios.FiltroPesquisaRelatorioUsuario filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioUsuario.ContarConsultaRelatorioUsuarios(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Usuarios/Usuario";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Usuarios.FiltroPesquisaRelatorioUsuario filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Embarcador.Usuarios.PerfilAcesso repPerfilAcesso = new Repositorio.Embarcador.Usuarios.PerfilAcesso(_unitOfWork);

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCadastroInicial", filtrosPesquisa.DataCadastroInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCadastroFinal", filtrosPesquisa.DataCadastroFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("UltimoAcessoInicial", filtrosPesquisa.UltimoAcessoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("UltimoAcessoFinal", filtrosPesquisa.UltimoAcessoFinal));

            if (filtrosPesquisa.CodigoLocalidade > 0)
            {
                Dominio.Entidades.Localidade localidadePar = repLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoLocalidade);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Localidade", localidadePar.DescricaoCidadeEstado, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Localidade", false));

            if (filtrosPesquisa.CodigoPerfilAcesso > 0)
            {
                Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfilAcessoPar = repPerfilAcesso.BuscarPorCodigo(filtrosPesquisa.CodigoPerfilAcesso);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PerfilAcesso", perfilAcessoPar.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PerfilAcesso", false));

            if (filtrosPesquisa.Operador.HasValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Operador", (filtrosPesquisa.Operador.Value ? "Sim" : "Não"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Operador", false));

            if (filtrosPesquisa.AcessoSistema.HasValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("AcessoSistema", (filtrosPesquisa.AcessoSistema.Value ? "Liberado" : "Bloqueado"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("AcessoSistema", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Status))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", (filtrosPesquisa.Status == "A" ? "Ativo" : "Inativo"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", false));

            if (filtrosPesquisa.Ambiente.HasValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Ambiente", ObterDescricaoAmbiente(filtrosPesquisa.Ambiente.Value), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Ambiente", false));

            if (filtrosPesquisa.SituacaoColaborador != SituacaoColaborador.Todos)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoColaborador", filtrosPesquisa.SituacaoColaborador.ObterDescricao(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoColaborador", false));

            if (filtrosPesquisa.Aposentadoria != Aposentadoria.Todos)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Aposentadoria", filtrosPesquisa.Aposentadoria.ObterDescricao(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Aposentadoria", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoUsuario", filtrosPesquisa.TipoUsuario.ObterDescricao()));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SomenteUsuariosAtivo", filtrosPesquisa.SomenteUsuariosAtivo ? "Sim" : string.Empty));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatad"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "").Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}