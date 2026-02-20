using Dominio.ObjetosDeValor.Embarcador.Carga;
using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class Pacotes : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPacotes, Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.RelatorioPacotes>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.Pacote _repositorioPacote;

        #endregion

        #region Construtores

        public Pacotes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPacote = new Repositorio.Embarcador.Cargas.Pacote(_unitOfWork);
        }

        public Pacotes(Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPacote = new Repositorio.Embarcador.Cargas.Pacote(_unitOfWork, cancellationToken);
        }

        #endregion

        #region Métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.RelatorioPacotes>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPacotes filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, CancellationToken cancellationToken)
        {
            return await _repositorioPacote.ConsultarRelatorioPacotesAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta, cancellationToken);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.RelatorioPacotes> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPacotes filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioPacote.ConsultarRelatorioPacotes(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPacotes filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioPacote.ContarConsultaRelatorioPacotes(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/Pacotes";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPacotes filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);



            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = filtrosPesquisa.CodigoPedido > 0 ? repPedido.BuscarPorCodigo(filtrosPesquisa.CodigoPedido) : null;
            Dominio.Entidades.Embarcador.Cargas.Carga carga = filtrosPesquisa.CodigoCarga > 0 ? repCarga.BuscarPorCodigo(filtrosPesquisa.CodigoCarga) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao) : null;
            Dominio.Entidades.Cliente contratante = filtrosPesquisa.CodigoContratante > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CodigoContratante) : null;
            Dominio.Entidades.Cliente origem = filtrosPesquisa.CodigoOrigem > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CodigoOrigem) : null;
            Dominio.Entidades.Cliente destino = filtrosPesquisa.CodigoDestino > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CodigoDestino) : null;


            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("LogKey", filtrosPesquisa?.LogKey));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pedido", pedido?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", carga?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tipoOperacao?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Origem", origem?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", destino?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataRecebimento", filtrosPesquisa.DataRecebimentoInicial, filtrosPesquisa.DataRecebimentoFinal, true));;
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Contratante", contratante?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Cubagem", filtrosPesquisa?.Cubagem));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Peso", filtrosPesquisa?.Peso));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCTe", filtrosPesquisa?.NumeroCTe > 0 ? filtrosPesquisa?.NumeroCTe : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ChaveCTe", filtrosPesquisa?.ChaveCTe));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataRecebimentoFormatada")
                return "DataRecebimento";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}