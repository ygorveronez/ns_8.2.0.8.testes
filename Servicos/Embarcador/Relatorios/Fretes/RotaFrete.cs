using System.Collections.Generic;
using System.Linq;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.Fretes
{
    public class RotaFrete : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRotaFreteRelatorio, Dominio.Relatorios.Embarcador.DataSource.Fretes.RotaFrete>
    {
        #region Atributos
        private readonly Repositorio.RotaFrete _repositorioRotaFrete;
        #endregion

        #region Construtores
        public RotaFrete(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioRotaFrete = new Repositorio.RotaFrete(_unitOfWork);
        }
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Fretes.RotaFrete> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRotaFreteRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioRotaFrete.ConsultarRelatorio(filtrosPesquisa,propriedadesAgrupamento,parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRotaFreteRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioRotaFrete.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Fretes/RotaFrete";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRotaFreteRelatorio filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Cliente repPessoa = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(_unitOfWork);

            Dominio.Entidades.Cliente remetente = filtrosPesquisa.Remetente > 0 ? repPessoa.BuscarPorCPFCNPJ(filtrosPesquisa.Remetente) : null;
            Dominio.Entidades.Cliente destinatario = filtrosPesquisa.Destinatario > 0 ? repPessoa.BuscarPorCPFCNPJ(filtrosPesquisa.Destinatario) : null;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = filtrosPesquisa.GrupoPessoas > 0 ? repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.GrupoPessoas) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.TipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.TipoOperacao) : null;
            List<Dominio.Entidades.Localidade> origens = filtrosPesquisa.CodigosOrigem?.Count > 0 ? repLocalidade.BuscarPorCodigos(filtrosPesquisa.CodigosOrigem) : new List<Dominio.Entidades.Localidade>();
            List<Dominio.Entidades.Localidade> destinos = filtrosPesquisa.CodigosDestino?.Count > 0 ? repLocalidade.BuscarPorCodigos(filtrosPesquisa.CodigosDestino) : new List<Dominio.Entidades.Localidade>();
            List<Dominio.Entidades.Estado> UFDestino = filtrosPesquisa.CodigosUFDestino?.Count > 0 ? repEstado.BuscarPorSiglas(filtrosPesquisa.CodigosUFDestino) : new List<Dominio.Entidades.Estado>();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", filtrosPesquisa.DescricaoSituacao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", remetente?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", destinatario?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", grupoPessoas?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tipoOperacao?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CodigosOrigem", origens?.Select(o => o.Descricao).ToList()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CodigosDestino", destinos?.Select(o => o.Descricao).ToList()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CodigosUFDestino", UFDestino?.Select(o => o.Sigla).ToList()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Descricao", filtrosPesquisa.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("RotaExclusivaCompraValePedagio", filtrosPesquisa.RotaExclusivaCompraValePedagio.HasValue ? filtrosPesquisa.RotaExclusivaCompraValePedagio.Value ? "Sim" : "Não" : string.Empty));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DescricaoTipoRota")
                return "TipoRota";
            else if (propriedadeOrdenarOuAgrupar == "DescricaoTipoCarregamentoIda")
                return "TipoCarregamentoIda";
            else if (propriedadeOrdenarOuAgrupar == "DescricaoTipoCarregamentoVolta")
                return "TipoCarregamentoVolta";

            return propriedadeOrdenarOuAgrupar;
        }
        #endregion
    }
}
