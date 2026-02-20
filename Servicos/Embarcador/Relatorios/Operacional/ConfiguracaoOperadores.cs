using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Operacional
{
    public class ConfiguracaoOperadores : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Operacional.FiltroPesquisaRelatorioConfiguracaoOperadores, Dominio.Relatorios.Embarcador.DataSource.Operacional.ConfiguracaoOperadores>
    {

        #region Propriedades

        private readonly Repositorio.Embarcador.Operacional.OperadorLogistica _repositorioConfiguracaoOperadores;

        #endregion

        #region Construtores

        public ConfiguracaoOperadores(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioConfiguracaoOperadores = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Operacional.ConfiguracaoOperadores> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Operacional.FiltroPesquisaRelatorioConfiguracaoOperadores filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioConfiguracaoOperadores.ConsultarRelatorioConfiguracaoOperadores(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Operacional.FiltroPesquisaRelatorioConfiguracaoOperadores filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioConfiguracaoOperadores.ContarConsultaRelatorioConfiguracaoOperadores(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Operacional/ConfiguracaoOperadores";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Operacional.FiltroPesquisaRelatorioConfiguracaoOperadores filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Operacional.OperadorLogistica repositorioUsuario = new Repositorio.Embarcador.Operacional.OperadorLogistica(_unitOfWork);
            Repositorio.Embarcador.Operacional.OperadorFilial repositorioFilial= new Repositorio.Embarcador.Operacional.OperadorFilial(_unitOfWork);
            Repositorio.Embarcador.Operacional.OperadorTipoCarga repositorioTipoCarga = new Repositorio.Embarcador.Operacional.OperadorTipoCarga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);

            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorUsuario = filtrosPesquisa.CodigoUsuario > 0 ? repositorioUsuario.BuscarPorUsuario(filtrosPesquisa.CodigoUsuario) : null;
            Dominio.Entidades.Embarcador.Operacional.OperadorFilial operadorFilial = filtrosPesquisa.CodigoFilial > 0 ? repositorioFilial.BuscarPorCodigo(filtrosPesquisa.CodigoFilial) : null;
            Dominio.Entidades.Embarcador.Operacional.OperadorTipoCarga operadorTipoCarga = filtrosPesquisa.CodigoTipoCarga > 0 ? repositorioTipoCarga.BuscarPorCodigo(filtrosPesquisa.CodigoTipoCarga) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao operadorTipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? repositorioTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao) : null;

            parametros.Add(new Parametro("Usuario", operadorUsuario?.Usuario.Descricao ?? string.Empty));
            parametros.Add(new Parametro("Filial", operadorFilial?.Filial.Descricao ?? string.Empty));
            parametros.Add(new Parametro("TipoCarga", operadorTipoCarga?.TipoDeCarga.Descricao ?? string.Empty));
            parametros.Add(new Parametro("TipoOperacao", operadorTipoOperacao?.Descricao ?? string.Empty));

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
