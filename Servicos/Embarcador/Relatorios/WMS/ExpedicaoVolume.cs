using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.WMS
{
    public class ExpedicaoVolume : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioExpedicaoVolume, Dominio.Relatorios.Embarcador.DataSource.WMS.ExpedicaoVolume>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.WMS.RecebimentoMercadoria _repositorioExpedicaoVolume;

        #endregion

        #region Construtores

        public ExpedicaoVolume(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioExpedicaoVolume = new Repositorio.Embarcador.WMS.RecebimentoMercadoria(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.WMS.ExpedicaoVolume> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioExpedicaoVolume filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioExpedicaoVolume.RelatorioExpedicaoVolume(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioExpedicaoVolume filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioExpedicaoVolume.ContarRelatorioExpedicaoVolume(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/WMS/ExpedicaoVolume";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioExpedicaoVolume filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga codigoCarga = filtrosPesquisa.CodigoCarga > 0 ? repCarga.BuscarPorCodigo(filtrosPesquisa.CodigoCarga) : null;
            Dominio.Entidades.Usuario codigoConferente = filtrosPesquisa.CodigoConferente > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoConferente) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", codigoCarga?.CodigoCargaEmbarcador));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataExpedicaoInicial", filtrosPesquisa.DataExpedicaoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataExpedicaoFinal", filtrosPesquisa.DataExpedicaoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmbarqueInicial", filtrosPesquisa.DataEmbarqueInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmbarqueFinal", filtrosPesquisa.DataEmbarqueFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Conferente", codigoConferente?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroPedido", filtrosPesquisa.NumeroPedido));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroNota", filtrosPesquisa.NumeroNota));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CodigoBarras", filtrosPesquisa.CodigoBarras));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta.PropriedadeAgrupar));

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
