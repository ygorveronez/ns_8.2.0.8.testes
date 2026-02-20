using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Logistica
{
    public class ControleTempoViagem : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioControleTempoViagem, Dominio.Relatorios.Embarcador.DataSource.Logistica.ControleTempoViagem>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Logistica.ControleViagem _repositorioControleViagem;

        #endregion

        #region Construtores

        public ControleTempoViagem(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioControleViagem = new Repositorio.Embarcador.Logistica.ControleViagem(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Logistica.ControleTempoViagem> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioControleTempoViagem filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioControleViagem.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioControleTempoViagem filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioControleViagem.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Logistica/ControleTempoViagem";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioControleTempoViagem filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = filtrosPesquisa.CodigosCargas?.Count > 0 ? repCarga.BuscarPorCodigos(filtrosPesquisa.CodigosCargas) : null;
            List<Dominio.Entidades.Localidade> localidades = filtrosPesquisa.Destinos?.Count > 0 ? repLocalidade.BuscarPorCodigos(filtrosPesquisa.Destinos) : null;
            List<Dominio.Entidades.Empresa> empresas = filtrosPesquisa.Transportadores?.Count > 0 ? repEmpresa.BuscarPorCodigos(filtrosPesquisa.Transportadores) : null;
            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal> notasFiscais = filtrosPesquisa.NumerosNota?.Count > 0 ? repNotaFiscal.BuscarPorCodigo(filtrosPesquisa.NumerosNota) : null;

            string valoresNotas = string.Empty;
            if (filtrosPesquisa.ValorNotaInicial > 0 || filtrosPesquisa.ValorNotaFinal > 0)
                valoresNotas = filtrosPesquisa.ValorNotaInicial.ToString() + " até " + filtrosPesquisa.ValorNotaFinal.ToString();

            parametros.Add(new Parametro("PeriodoFatura", filtrosPesquisa.DataFaturaInicial, filtrosPesquisa.DataFaturaFinal));
            parametros.Add(new Parametro("PeriodoEntrega", filtrosPesquisa.PrevisaoEntregaInicial, filtrosPesquisa.PrevisaoEntregaFinal));
            parametros.Add(new Parametro("PeriodoEntregaReal", filtrosPesquisa.DataEntregaRealInicial, filtrosPesquisa.DataEntregaRealFinal));
            parametros.Add(new Parametro("PeriodoRetornoComprovante", filtrosPesquisa.DataRetornoComprovanteInicial, filtrosPesquisa.DataRetornoComprovanteFinal));
            parametros.Add(new Parametro("TempoViagem", filtrosPesquisa.TempoViagem));
            parametros.Add(new Parametro("Performance", filtrosPesquisa.Performance));
            parametros.Add(new Parametro("DiasRetornoComprovante", filtrosPesquisa.DiasRetornoComprovante));
            parametros.Add(new Parametro("DocumentoVenda", filtrosPesquisa.DocumentoVenda));
            parametros.Add(new Parametro("RazaoSocialDestinatario", filtrosPesquisa.RazaoSocialDestinatario));
            parametros.Add(new Parametro("ValoresNota", valoresNotas));
            parametros.Add(new Parametro("Carga", cargas != null ? string.Join(", ", cargas.Select(o => o.CodigoCargaEmbarcador)) : string.Empty));
            parametros.Add(new Parametro("Transportador", empresas != null ? string.Join(", ", empresas.Select(o => o.RazaoSocial)) : string.Empty));
            parametros.Add(new Parametro("Destino", localidades != null ? string.Join(", ", localidades.Select(o => o.Descricao)) : string.Empty));
            parametros.Add(new Parametro("NumeroNota", notasFiscais != null ? string.Join(", ", notasFiscais.Select(o => o.Numero)) : string.Empty));

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