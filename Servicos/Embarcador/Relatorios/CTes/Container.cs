using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.CTes
{
    public class Container : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioContainer, Dominio.Relatorios.Embarcador.DataSource.CTe.Container>
    {
        #region Atributos

        private readonly Repositorio.ContainerCTE _repositorioContainerCTe;

        #endregion

        #region Construtores

        public Container(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioContainerCTe = new Repositorio.ContainerCTE(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList meotodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.CTe.Container> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioContainer filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioContainerCTe.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioContainer filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioContainerCTe.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/CTe/Container";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioContainer filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repViagem = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem = filtrosPesquisa.CodigoPortoOrigem > 0 ? repPorto.BuscarPorCodigo(filtrosPesquisa.CodigoPortoOrigem) : null;
            Dominio.Entidades.Embarcador.Pedidos.Porto portoDestino = filtrosPesquisa.CodigoPortoDestino > 0 ? repPorto.BuscarPorCodigo(filtrosPesquisa.CodigoPortoDestino) : null;
            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = filtrosPesquisa.CodigoViagem > 0 ? repViagem.BuscarPorCodigo(filtrosPesquisa.CodigoViagem) : null;
            Dominio.Entidades.Embarcador.Pedidos.Container container = filtrosPesquisa.CodigoContainer > 0 ? repContainer.BuscarPorCodigo(filtrosPesquisa.CodigoContainer) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem = filtrosPesquisa.CodigoTerminalOrigem > 0 ? repTipoTerminalImportacao.BuscarPorCodigo(filtrosPesquisa.CodigoTerminalOrigem) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino = filtrosPesquisa.CodigoTerminalDestino > 0 ? repTipoTerminalImportacao.BuscarPorCodigo(filtrosPesquisa.CodigoTerminalDestino) : null;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa = filtrosPesquisa.CodigoGrupoPessoa > 0 ? repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoPessoa) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao) : null;
            Dominio.Entidades.Embarcador.Pedidos.Porto portoTransbordo = filtrosPesquisa.CodigoPortoTransbordo > 0 ? repPorto.BuscarPorCodigo(filtrosPesquisa.CodigoPortoTransbordo) : null;
            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagemTransbordo = filtrosPesquisa.CodigoViagemTransbordo > 0 ? repViagem.BuscarPorCodigo(filtrosPesquisa.CodigoViagemTransbordo) : null;
            Dominio.Entidades.Embarcador.Pedidos.Navio balsa = filtrosPesquisa.CodigoBalsa > 0 ? repNavio.BuscarPorCodigo(filtrosPesquisa.CodigoBalsa) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", filtrosPesquisa.DataEmissaoInicial, filtrosPesquisa.DataEmissaoFinal));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroBooking", filtrosPesquisa.NumeroBooking));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroOS", filtrosPesquisa.NumeroOS));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroControle", filtrosPesquisa.NumeroControle));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCTe", filtrosPesquisa.NumeroCTe));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroNota", filtrosPesquisa.NumeroNota));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroSerie", filtrosPesquisa.NumeroSerie));

            if (filtrosPesquisa.SituacoesCargaMercante != null && filtrosPesquisa.SituacoesCargaMercante.Count > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoCarga", string.Join(", ", filtrosPesquisa.SituacoesCargaMercante.Select(o => o.ObterDescricao()))));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoCarga", string.Join(", ", filtrosPesquisa.SituacaoCarga.Select(o => o.ObterDescricao()))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoProposta", string.Join(", ", filtrosPesquisa.TipoProposta.Select(o => o.ObterDescricao()))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoModal", string.Join(", ", filtrosPesquisa.TipoModal.Select(o => o.ObterDescricao()))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoServico", string.Join(", ", filtrosPesquisa.TipoServico.Select(o => o.ObterDescricao()))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoCTe", Servicos.Embarcador.CTe.CTe.ObterDescricaoSituacao(filtrosPesquisa.SituacaoCTe)));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PortoOrigem", portoOrigem?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PortoDestino", portoDestino?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Viagem", viagem?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Container", container?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TerminalOrigem", terminalOrigem?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TerminalDestino", terminalDestino?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoa", grupoPessoa?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tipoOperacao?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta != null ? parametrosConsulta.PropriedadeAgrupar : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TiposCTe", string.Join(", ", filtrosPesquisa.TiposCTe.Select(o => o.ObterDescricao()))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VeioPorImportacao", filtrosPesquisa.VeioPorImportacao.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SomenteCTeSubstituido", filtrosPesquisa.SomenteCTeSubstituido ? "Sim" : "Não"));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PortoTransbordo", portoTransbordo?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ViagemTransbordo", viagemTransbordo?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Balsa", balsa?.Descricao));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataEmissaoFormatada")
                return "DataEmissao";

            if (propriedadeOrdenarOuAgrupar == "DataAutorizacaoFormatada")
                return "DataAutorizacao";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}