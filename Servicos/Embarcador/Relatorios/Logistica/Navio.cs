using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Logistica
{
    public class Navio : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioNavio, Dominio.Relatorios.Embarcador.DataSource.Logistica.Navio>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Pedidos.Navio _repositorioNavio;

        #endregion

        #region Construtores

        public Navio(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioNavio = new Repositorio.Embarcador.Pedidos.Navio(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Logistica.Navio> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioNavio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioNavio.ConsultarRelatorioNavio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioNavio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioNavio.ContarConsultaRelatorioNavio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Logistica/Navio";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioNavio filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string tiposEmbarcacao = string.Join(", ", filtrosPesquisa.TipoEmbarcacao.Select(o => o.ObterDescricao()));

            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>
            {
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Descricao", filtrosPesquisa.Descricao),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CodigoIntegracao", filtrosPesquisa.CodigoIntegracao),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", filtrosPesquisa.Status.ObterDescricao()),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CodigoIrin", filtrosPesquisa.CodigoIrin),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CodigoEmbarcacao", filtrosPesquisa.CodigoEmbarcacao),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoEmbarcacao", tiposEmbarcacao),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CodigoDocumentacao", filtrosPesquisa.CodigoDocumentacao),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CodigoIMO", filtrosPesquisa.CodigoIMO),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NavioID", filtrosPesquisa.NavioID),
            };

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Descricao"))
                return propriedadeOrdenarOuAgrupar.Replace("Descricao", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}
