using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.WMS;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.WMS
{
    public class Armazenagem : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaArmazenagem, Dominio.Relatorios.Embarcador.DataSource.WMS.Armazenagem>
    {
        #region Atributos Privados

        private readonly Repositorio.Embarcador.WMS.RecebimentoMercadoria _repositorioArmazenagem;

        #endregion

        #region Construtores

        public Armazenagem(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultiSoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultiSoftware) 
        {
            _repositorioArmazenagem = new Repositorio.Embarcador.WMS.RecebimentoMercadoria(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.WMS.Armazenagem> ConsultarRegistros(FiltroPesquisaArmazenagem filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioArmazenagem.RelatorioArmazenagem(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaArmazenagem filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioArmazenagem.ContarRelatorioArmazenagem(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/WMS/Armazenagem";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaArmazenagem filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            parametros.Add(new Parametro("DataInicial", filtrosPesquisa.DataInicial > DateTime.MinValue ? filtrosPesquisa.DataInicial.ToString("dd/MM/yyyy") : ""));
            parametros.Add(new Parametro("DataFinal", filtrosPesquisa.DataFinal > DateTime.MinValue ? filtrosPesquisa.DataFinal.ToString("dd/MM/yyyy") : ""));
            parametros.Add(new Parametro("TipoNF", filtrosPesquisa.Tipo?.ObterDescricao()));
            parametros.Add(new Parametro("StatusNF", filtrosPesquisa.StatusNF.Value.ObterDescricao()));
            parametros.Add(new Parametro("Agrupamento", ""));

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
