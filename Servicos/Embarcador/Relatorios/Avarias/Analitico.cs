using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Relatorios.Avarias
{
    public class Analitico : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaRelatorioAvarias, Dominio.Relatorios.Embarcador.DataSource.Avarias.Avaria.ReportAvaria>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Avarias.SolicitacaoAvaria _repositorioAvaria;

        #endregion

        #region Construtores

        public Analitico(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(_unitOfWork);
        }

        public Analitico(
    Repositorio.UnitOfWork unitOfWork,
    AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
    AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
    CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(_unitOfWork, cancellationToken);
        }

        #endregion

        #region métodos assíncronos
        public async Task<List<Dominio.Relatorios.Embarcador.DataSource.Avarias.Avaria.ReportAvaria>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaRelatorioAvarias filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioAvaria.ConsultarRelatorioAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos



        protected override List<Dominio.Relatorios.Embarcador.DataSource.Avarias.Avaria.ReportAvaria> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaRelatorioAvarias filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioAvaria.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaRelatorioAvarias filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioAvaria.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Avarias/Analitico";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaRelatorioAvarias filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

            Dominio.Entidades.Usuario solicitate = filtrosPesquisa.CodigoSolicitante > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoSolicitante) : null;
            Dominio.Entidades.Empresa transportador = filtrosPesquisa.CodigoTransportador > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroAvaria", filtrosPesquisa.NumeroAvaria));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCarga", filtrosPesquisa.CodigoCargaEmbarcador));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAvariaInicial", filtrosPesquisa.DataSolicitacaoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAvariaFinal", filtrosPesquisa.DataSolicitacaoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Solicitante", solicitate?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", transportador?.RazaoSocial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", string.Join(", ", filtrosPesquisa.SituacaoAvaria.Select(o => o.ObterDescricao()))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataGeracaoLote", filtrosPesquisa.DataGeracaoLoteInicial, filtrosPesquisa.DataGeracaoLoteFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataIntegracaoLote", filtrosPesquisa.DataIntegracaoLoteInicial, filtrosPesquisa.DataIntegracaoLoteFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Etapa", filtrosPesquisa.Etapa.ObterDescricao()));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "Avaria")
                return "SolicitacaoAvaria.NumeroAvaria";
            
            if (propriedadeOrdenarOuAgrupar == "Lote")
                return "SolicitacaoAvaria.Lote.Numero";
            
            if (propriedadeOrdenarOuAgrupar == "CodigoProduto")
                return "ProdutoEmbarcador.CodigoProdutoEmbarcador";
            
            if (propriedadeOrdenarOuAgrupar == "DescricaoProduto")
                return "ProdutoEmbarcador.Descricao";
            
            if (propriedadeOrdenarOuAgrupar == "QuantidadeCaixas")
                return "CaixasAvariadas";
            
            if (propriedadeOrdenarOuAgrupar == "QuantidadeUnidades")
                return "UnidadesAvariadas";
            
            if (propriedadeOrdenarOuAgrupar == "DataSolicitacao")
                return "SolicitacaoAvaria.DataSolicitacao";
            
            if (propriedadeOrdenarOuAgrupar == "DataCriacao")
                return "SolicitacaoAvaria.Lote.DataGeracao";
            
            if (propriedadeOrdenarOuAgrupar == "EtapaLote")
                return "SolicitacaoAvaria.Lote.Etapa";
            
            if (propriedadeOrdenarOuAgrupar == "ValorDescontoAvaria")
                return "SolicitacaoAvaria.ValorDesconto";
            
            if (propriedadeOrdenarOuAgrupar == "Filial")
                return "SolicitacaoAvaria.Carga.Filial.Descricao";
            
            if (propriedadeOrdenarOuAgrupar == "Transportadora")
                return "SolicitacaoAvaria.Transportador.RazaoSocial";
            
            if (propriedadeOrdenarOuAgrupar == "Criador")
                return "SolicitacaoAvaria.Solicitante.Nome";
            
            if (propriedadeOrdenarOuAgrupar == "NotasFiscais")
                return "NotaFiscal";
            
            if (propriedadeOrdenarOuAgrupar == "Viagem")
                return "SolicitacaoAvaria.Carga.CodigoCargaEmbarcador";
            
            if (propriedadeOrdenarOuAgrupar == "TipoOperacao")
                return "SolicitacaoAvaria.Carga.TipoOperacao.Descricao";
            
            if (propriedadeOrdenarOuAgrupar == "MotivoAvaria")
                return "SolicitacaoAvaria.MotivoAvaria.Descricao";
            
            if (propriedadeOrdenarOuAgrupar == "Placa")
                return "SolicitacaoAvaria.Carga.Veiculo.Placa";
            
            if (propriedadeOrdenarOuAgrupar == "SituacaoAvaria")
                return "SolicitacaoAvaria.Situacao";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}