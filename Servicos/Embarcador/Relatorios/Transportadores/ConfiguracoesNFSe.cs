using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Transportadores
{
    public class ConfiguracoesNFSe : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioConfiguracoesNFSe, Dominio.Relatorios.Embarcador.DataSource.Transportadores.ConfiguracoesNFSe>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe _repositorioConfiguracoesNFSe;

        #endregion

        #region Construtores

        public ConfiguracoesNFSe(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioConfiguracoesNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(_unitOfWork);
        }

        public ConfiguracoesNFSe(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioConfiguracoesNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(_unitOfWork, cancellationToken);
        }

        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Transportadores.ConfiguracoesNFSe>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioConfiguracoesNFSe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioConfiguracoesNFSe.ConsultarRelatorioAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Transportadores.ConfiguracoesNFSe> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioConfiguracoesNFSe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioConfiguracoesNFSe.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioConfiguracoesNFSe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioConfiguracoesNFSe.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Transportadores/ConfiguracoesNFSe";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioConfiguracoesNFSe filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.ServicoNFSe repServico = new Repositorio.ServicoNFSe(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);



            Dominio.Entidades.Empresa transportador = filtrosPesquisa.CodigoTransportador > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador) : null;
            Dominio.Entidades.Localidade  localidade = filtrosPesquisa.CodigoLocalidadePrestacaoServico > 0 ? repLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoLocalidadePrestacaoServico) : null;
            Dominio.Entidades.ServicoNFSe servico = filtrosPesquisa.CodigoServico > 0 ? repServico.BuscarPorCodigo(filtrosPesquisa.CodigoServico) : null;
            Dominio.Entidades.Cliente clienteTomador = filtrosPesquisa.CPFCNPJClienteTomador > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CPFCNPJClienteTomador) : null;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoTomador = filtrosPesquisa.CodigoGrupoTomador > 0 ? repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoTomador) : null;
            Dominio.Entidades.Localidade localidadeTomador = filtrosPesquisa.CodigoLocalidadeTomador > 0 ? repLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoLocalidadeTomador) : null;
            Dominio.Entidades.Estado UFTomador = filtrosPesquisa.CodigoUFTomador > 0 ? repEstado.BuscarPorCodigo(filtrosPesquisa.CodigoUFTomador, false) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", transportador?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Localidade", localidade?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Servico", servico?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ClienteTomador", clienteTomador?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoTomador", grupoTomador?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("LocalidadeTomador", localidadeTomador?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("UFTomador", UFTomador?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tipoOperacao?.Descricao));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            propriedadeOrdenarOuAgrupar.Replace("Formatado", "");
            propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}