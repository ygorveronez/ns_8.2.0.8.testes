using System;
using System.Collections.Generic;
using System.Linq;
using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Relatorios.Ocorrencias
{
    public class RegrasAutorizacaoOcorrencia : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioRegrasAutorizacaoOcorrencia, Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.RegrasAutorizacaoOcorrencia>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia _regrasAutorizacaoOcorrencia;

        public RegrasAutorizacaoOcorrencia(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _regrasAutorizacaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia(unitOfWork);
        }
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.RegrasAutorizacaoOcorrencia> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioRegrasAutorizacaoOcorrencia filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _regrasAutorizacaoOcorrencia.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioRegrasAutorizacaoOcorrencia filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _regrasAutorizacaoOcorrencia.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Ocorrencias/RegrasAutorizacaoOcorrencia";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioRegrasAutorizacaoOcorrencia filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);

            Dominio.Entidades.Usuario aprovador = filtrosPesquisa.CodigoAprovador > 0 ? repositorioUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoAprovador) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Vigencia", filtrosPesquisa.DataVgenciaInicial, filtrosPesquisa.DataVigenciaLimite));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Aprovador", aprovador?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Ativo", filtrosPesquisa.Ativo.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Descricao", filtrosPesquisa.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EtapaAutorizacao", filtrosPesquisa.EtapaAutorizacao?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ExibirAlcadas", filtrosPesquisa.ExibirAlcadas ? "Sim" : string.Empty));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}
