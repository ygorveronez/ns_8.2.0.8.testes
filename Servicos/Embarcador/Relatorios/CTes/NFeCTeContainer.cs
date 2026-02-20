using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.CTes
{
    public class NFeCTeContainer : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioNFeCTeContainer, Dominio.Relatorios.Embarcador.DataSource.CTe.NFeCTeContainer>
    {
        #region Atributos

        private readonly Repositorio.ContainerCTE _repositorioNFeCTeContainer;

        #endregion

        #region Construtores

        public NFeCTeContainer(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioNFeCTeContainer = new Repositorio.ContainerCTE(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList meotodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.CTe.NFeCTeContainer> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioNFeCTeContainer filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioNFeCTeContainer.ConsultarRelatorioNFeCTeContainer(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioNFeCTeContainer filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioNFeCTeContainer.ContarConsultaRelatorioNFeCTeContainer(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/CTe/NFeCTeContainer";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioNFeCTeContainer filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repViagem = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem = filtrosPesquisa.CodigoPortoOrigem > 0 ? repPorto.BuscarPorCodigo(filtrosPesquisa.CodigoPortoOrigem) : null;
            Dominio.Entidades.Embarcador.Pedidos.Porto portoDestino = filtrosPesquisa.CodigoPortoDestino > 0 ? repPorto.BuscarPorCodigo(filtrosPesquisa.CodigoPortoDestino) : null;
            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = filtrosPesquisa.CodigoViagem > 0 ? repViagem.BuscarPorCodigo(filtrosPesquisa.CodigoViagem) : null;
            Dominio.Entidades.Embarcador.Pedidos.Container container = filtrosPesquisa.CodigoContainer > 0 ? repContainer.BuscarPorCodigo(filtrosPesquisa.CodigoContainer) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem = filtrosPesquisa.CodigoTerminalOrigem > 0 ? repTipoTerminalImportacao.BuscarPorCodigo(filtrosPesquisa.CodigoTerminalOrigem) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino = filtrosPesquisa.CodigoTerminalDestino > 0 ? repTipoTerminalImportacao.BuscarPorCodigo(filtrosPesquisa.CodigoTerminalDestino) : null;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa = filtrosPesquisa.CodigoGrupoPessoa > 0 ? repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoPessoa) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", filtrosPesquisa.DataEmissaoInicial, filtrosPesquisa.DataEmissaoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroBooking", filtrosPesquisa.NumeroBooking));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroOS", filtrosPesquisa.NumeroOS));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroControle", filtrosPesquisa.NumeroControle));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCTe", filtrosPesquisa.NumeroCTe));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroNota", filtrosPesquisa.NumeroNota));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroSerie", filtrosPesquisa.NumeroSerie));
            if (filtrosPesquisa.SituacoesCargaMercante != null && filtrosPesquisa.SituacoesCargaMercante.Count > 1)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoCarga", filtrosPesquisa.SituacoesCargaMercante.Select(o => o.ObterDescricao())));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoCarga", filtrosPesquisa.SituacaoCarga.Select(o => o.ObterDescricao())));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoProposta", filtrosPesquisa.TipoProposta.Select(o => o.ObterDescricao())));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoModal", filtrosPesquisa.TipoModal.Select(o => o.ObterDescricao())));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoServico", filtrosPesquisa.TipoServico.Select(o => o.ObterDescricao())));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoCTe", Servicos.Embarcador.CTe.CTe.ObterDescricaoSituacao(filtrosPesquisa.SituacaoCTe)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PortoOrigem", portoOrigem?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PortoDestino", portoDestino?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Viagem", viagem?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Container", container?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TerminalOrigem", terminalOrigem?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TerminalDestino", terminalDestino?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoa", grupoPessoa?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tipoOperacao?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCTe", filtrosPesquisa.TiposCTe.Select(o => o.ObterDescricao())));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("FoiAnulado", filtrosPesquisa.FoiAnulado.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("FoiSubstituido", filtrosPesquisa.FoiSubstituido.ObterDescricao()));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataEmissaoNotaFormatada")
                return "DataEmissaoNota";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}