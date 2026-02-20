using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.AcertoViagem
{
    public class BonificacaoAcertoViagem : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioBonificacaoAcertoViagem, Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.BonificacaoAcertoViagem>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Acerto.AcertoBonificacao _repositorioAcertoBonificacao;

        #endregion

        #region Construtores

        public BonificacaoAcertoViagem(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioAcertoBonificacao = new Repositorio.Embarcador.Acerto.AcertoBonificacao(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList, metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.BonificacaoAcertoViagem> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioBonificacaoAcertoViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioAcertoBonificacao.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioBonificacaoAcertoViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioAcertoBonificacao.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/AcertoViagem/BonificacaoAcertoViagem";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioBonificacaoAcertoViagem filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(_unitOfWork);

            Dominio.Entidades.Usuario motorista = filtrosPesquisa.CodigoMotorista > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", motorista?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Periodo", filtrosPesquisa.DataInicialAcerto, filtrosPesquisa.DataFinalAcerto));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroAcerto", filtrosPesquisa.NumeroAcerto));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataBonificacao", filtrosPesquisa.DataInicialBonificacao, filtrosPesquisa.DataFinalBonificacao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoBonificacao", filtrosPesquisa.TipoBonificacao > 0 ? repJustificativa.BuscarPorCodigo(filtrosPesquisa.TipoBonificacao)?.Descricao ?? "" : ""));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "PeriodoAcertoFormatada")
                return "PeriodoAcerto";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}