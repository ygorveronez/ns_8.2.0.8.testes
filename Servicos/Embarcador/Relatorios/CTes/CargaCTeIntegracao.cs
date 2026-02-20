using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.CTes
{
    public class CargaCTeIntegracao : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCargaCTeIntegracao, Dominio.Relatorios.Embarcador.DataSource.CTe.Integracao.CargaCTeIntegracao>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.CargaCTeIntegracao _repositorioCargaCTeIntegracao;

        #endregion

        #region Construtores

        public CargaCTeIntegracao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(_unitOfWork);
        }

        public CargaCTeIntegracao(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(_unitOfWork, cancellationToken);
        }

        #endregion

        #region
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.CTe.Integracao.CargaCTeIntegracao>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCargaCTeIntegracao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioCargaCTeIntegracao.ConsultarRelatorioIntegracaoAsync(propriedadesAgrupamento, filtrosPesquisa.DataEmissaoInicial, filtrosPesquisa.DataEmissaoFinal, filtrosPesquisa.DataIntegracaoInicial, filtrosPesquisa.DataIntegracaoFinal, filtrosPesquisa.GrupoPessoas, filtrosPesquisa.TipoIntegracao, filtrosPesquisa.Carga, filtrosPesquisa.CTe, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.CTe.Integracao.CargaCTeIntegracao> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCargaCTeIntegracao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioCargaCTeIntegracao.ConsultarRelatorioIntegracao(propriedadesAgrupamento, filtrosPesquisa.DataEmissaoInicial, filtrosPesquisa.DataEmissaoFinal, filtrosPesquisa.DataIntegracaoInicial, filtrosPesquisa.DataIntegracaoFinal, filtrosPesquisa.GrupoPessoas, filtrosPesquisa.TipoIntegracao, filtrosPesquisa.Carga, filtrosPesquisa.CTe, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCargaCTeIntegracao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCargaCTeIntegracao.ContarConsultaRelatorioIntegracao(propriedadesAgrupamento, filtrosPesquisa.DataEmissaoInicial, filtrosPesquisa.DataEmissaoFinal, filtrosPesquisa.DataIntegracaoInicial, filtrosPesquisa.DataIntegracaoFinal, filtrosPesquisa.GrupoPessoas, filtrosPesquisa.TipoIntegracao, filtrosPesquisa.Carga, filtrosPesquisa.CTe);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/CTes/CargaCTeIntegracao";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCargaCTeIntegracao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoInicial", filtrosPesquisa.DataEmissaoInicial.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoInicial", false));

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoFinal", filtrosPesquisa.DataEmissaoFinal.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoFinal", false));

            if (filtrosPesquisa.DataIntegracaoInicial != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataIntegracaoInicial", filtrosPesquisa.DataIntegracaoInicial.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataIntegracaoInicial", false));

            if (filtrosPesquisa.DataIntegracaoFinal != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataIntegracaoFinal", filtrosPesquisa.DataIntegracaoFinal.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataIntegracaoFinal", false));

            if (filtrosPesquisa.GrupoPessoas > 0)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.GrupoPessoas);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", grupoPessoas.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", false));

            if (filtrosPesquisa.Carga > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(filtrosPesquisa.Carga);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", carga.CodigoCargaEmbarcador, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", false));

            if (filtrosPesquisa.CTe > 0)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(filtrosPesquisa.CTe);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTe", cte.Numero + " - " + cte.Serie.Numero, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTe", false));

            if (filtrosPesquisa.TipoIntegracao > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorCodigo(filtrosPesquisa.TipoIntegracao, false);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoIntegracao", tipoIntegracao.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoIntegracao", false));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}