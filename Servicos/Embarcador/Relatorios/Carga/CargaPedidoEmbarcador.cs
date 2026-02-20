using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class CargaPedidoEmbarcador : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaPedidoEmbarcador, Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaPedidoEmbarcador>
    {
        #region Atributos
        private readonly Repositorio.Embarcador.Cargas.Carga _repositorioCarga;

        #endregion

        #region Construtores
        public CargaPedidoEmbarcador(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaPedidoEmbarcador> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaPedidoEmbarcador filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioCarga.ConsultarRelatorioCargaPedidoEmbarcador(filtrosPesquisa, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaPedidoEmbarcador filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCarga.ContarRelatorioCargaPedidoEmbarcador(filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/CargaPedidoEmbarcador";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaPedidoEmbarcador filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Pedidos.CanalEntrega repositorioCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(_unitOfWork);
            Repositorio.TipoCarga repositorioTipoCarga = new Repositorio.TipoCarga(_unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProduto repositorioGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPesosa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega> canaisEntrega = new List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega>();
            List<Dominio.Entidades.TipoCarga> tiposCarga = new List<Dominio.Entidades.TipoCarga>();
            List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> gruposProduto = new List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtos = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            if (filtrosPesquisa.CodigosFiliais?.Count > 0)
                filiais = repositorioFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFiliais);

            if (filtrosPesquisa.CodigosCanaisEntrega?.Count > 0)
                canaisEntrega = repositorioCanalEntrega.BuscarPorCodigos(filtrosPesquisa.CodigosCanaisEntrega);

            if (filtrosPesquisa.CodigosTiposCarga?.Count > 0)
                tiposCarga = repositorioTipoCarga.BuscarPorCodigos(filtrosPesquisa.CodigosTiposCarga);

            if (filtrosPesquisa.CodigosDestinatario?.Count > 0)
                destinatarios = repositorioCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CodigosDestinatario);

            if (filtrosPesquisa.CodigosGrupoProduto?.Count > 0)
                gruposProduto = repositorioGrupoProduto.BuscarPorCodigos(filtrosPesquisa.CodigosGrupoProduto);

            if (filtrosPesquisa.CodigosGrupoPessoa?.Count > 0)
                grupoPessoas = repositorioGrupoPesosa.BuscarPorCodigo(filtrosPesquisa.CodigosGrupoPessoa.ToArray());

            if (filtrosPesquisa.CodigosProduto?.Count > 0)
                produtos = repositorioProdutoEmbarcador.BuscarPorCodigo(filtrosPesquisa.CodigosProduto.ToArray());

            if (filtrosPesquisa.TipoOperacao?.Count > 0)
                tiposOperacao = repositorioTipoOperacao.BuscarPorCodigos(filtrosPesquisa.TipoOperacao);

            parametros.Add(new Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Parametro("Filial", string.Join(", ", filiais.Select(o => o.Descricao))));
            parametros.Add(new Parametro("CanalEntrega", string.Join(", ", canaisEntrega.Select(o => o.Descricao))));
            parametros.Add(new Parametro("TipoCarga", string.Join(", ", tiposCarga.Select(o => o.Descricao))));
            parametros.Add(new Parametro("Destinatario", string.Join(", ", destinatarios.Select(o => o.Descricao))));
            parametros.Add(new Parametro("GrupoProduto", string.Join(", ", gruposProduto.Select(o => o.Descricao))));
            parametros.Add(new Parametro("Pedido", filtrosPesquisa.PedidoEmbarcador));
            parametros.Add(new Parametro("TipoOperacao", string.Join(", ", tiposOperacao.Select(o => o.Descricao))));
            parametros.Add(new Parametro("Produto", string.Join(", ", produtos.Select(o => o.Descricao))));
            parametros.Add(new Parametro("GrupoPessoa", string.Join(", ", grupoPessoas.Select(o => o.Descricao))));
            parametros.Add(new Parametro("StatusPedido", string.Join(", ", filtrosPesquisa.StatusPedido.Select(o => o.ObterDescricao()))));

            return parametros;
        }


        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataPedido")
                return "DataPedido";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}
