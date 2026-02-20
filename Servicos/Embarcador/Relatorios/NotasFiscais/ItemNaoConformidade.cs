using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.NotasFiscais
{
    public class ItemNaoConformidade : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaRelatorioItemNaoConformidade, Dominio.Relatorios.Embarcador.DataSource.NotasFiscais.ItemNaoConformidade>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade _repositorioItemNaoConformidade;

        #endregion

        #region Construtores

        public ItemNaoConformidade(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioItemNaoConformidade = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.NotasFiscais.ItemNaoConformidade> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaRelatorioItemNaoConformidade filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioItemNaoConformidade.ConsultarRelatorioItemNaoConformidade(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaRelatorioItemNaoConformidade filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioItemNaoConformidade.ContarConsultaRelatorioItemNaoConformidade(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/NotasFiscais/ItemNaoConformidade";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaRelatorioItemNaoConformidade filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            parametros.Add(new Parametro("Descricao", filtrosPesquisa.Descricao));
            parametros.Add(new Parametro("Situacao", filtrosPesquisa?.Situacao));
            parametros.Add(new Parametro("Grupo", filtrosPesquisa?.Grupo?.ObterDescricao()));
            parametros.Add(new Parametro("SubGrupo", filtrosPesquisa?.SubGrupo?.ObterDescricao()));
            parametros.Add(new Parametro("Area", filtrosPesquisa?.Area?.ObterDescricao()));
            parametros.Add(new Parametro("IrrelevanteNaoConformidade", filtrosPesquisa?.IrrelevanteParaNC?.ObterDescricao()));
            parametros.Add(new Parametro("PermiteContingencia", filtrosPesquisa?.PermiteContingencia?.ObterDescricao()));

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