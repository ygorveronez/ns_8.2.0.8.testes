using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.CTes
{
    public class TakeOrPay : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioTakeOrPay, Dominio.Relatorios.Embarcador.DataSource.CTe.TakeOrPay>
    {
        #region Atributos

        private readonly Repositorio.ConhecimentoDeTransporteEletronico _repositorioTakeOrPay;

        #endregion

        #region Construtores

        public TakeOrPay(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioTakeOrPay = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
        }

        public TakeOrPay(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioTakeOrPay = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork, cancellationToken);
        }

        #endregion

        #region Métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.CTe.TakeOrPay>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioTakeOrPay filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioTakeOrPay.ConsultarRelatorioTakeOrPayAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.CTe.TakeOrPay> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioTakeOrPay filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioTakeOrPay.ConsultarRelatorioTakeOrPay(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioTakeOrPay filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioTakeOrPay.ContarConsultaRelatorioTakeOrPay(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/CTe/TakeOrPay";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioTakeOrPay filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repViagem = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = filtrosPesquisa.CodigoCarga > 0 ? repCarga.BuscarPorCodigo(filtrosPesquisa.CodigoCarga) : null;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = filtrosPesquisa.CodigoGrupoPessoas > 0 ? repGrupoPessoa.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoPessoas) : null;
            Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem = filtrosPesquisa.CodigoPortoOrigem > 0 ? repPorto.BuscarPorCodigo(filtrosPesquisa.CodigoPortoOrigem) : null;
            Dominio.Entidades.Embarcador.Pedidos.Porto portoDestino = filtrosPesquisa.CodigoPortoDestino > 0 ? repPorto.BuscarPorCodigo(filtrosPesquisa.CodigoPortoDestino) : null;
            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = filtrosPesquisa.CodigoViagem > 0 ? repViagem.BuscarPorCodigo(filtrosPesquisa.CodigoViagem) : null;

            parametros.Add(new Parametro("DataFatura", filtrosPesquisa.DataInicialFatura, filtrosPesquisa.DataFinalFatura));
            parametros.Add(new Parametro("NumeroFatura", filtrosPesquisa.NumeroFatura));
            parametros.Add(new Parametro("NumeroBoleto", filtrosPesquisa.NumeroBoleto));
            parametros.Add(new Parametro("Carga", carga?.CodigoCargaEmbarcador));
            parametros.Add(new Parametro("GrupoPessoas", grupoPessoas?.Descricao));
            parametros.Add(new Parametro("TipoProposta", string.Join(", ", filtrosPesquisa.TipoProposta.Select(o => o.ObterDescricao()))));
            parametros.Add(new Parametro("PortoOrigem", portoOrigem?.Descricao));
            parametros.Add(new Parametro("PortoDestino", portoDestino?.Descricao));
            parametros.Add(new Parametro("Viagem", viagem?.Descricao));
            parametros.Add(new Parametro("Agrupamento", parametrosConsulta != null ? parametrosConsulta.PropriedadeAgrupar : string.Empty));
            parametros.Add(new Parametro("DataPrevisaoSaidaNavio", filtrosPesquisa.DataInicialPrevisaoSaidaNavio, filtrosPesquisa.DataFinalPrevisaoSaidaNavio));
            parametros.Add(new Parametro("SituacaoFatura", filtrosPesquisa.SituacaoFatura.HasValue ? filtrosPesquisa.SituacaoFatura.Value.ObterDescricao() : "Todas"));


            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}
