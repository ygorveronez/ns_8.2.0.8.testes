using Dominio.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.CTes
{
    public class CustoRentabilidadeCteCrt : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCustoRentabilidadeCteCrt, Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CustoRentabilidadeCteCrt>
    {
        #region Atributos

        private readonly Repositorio.ConhecimentoDeTransporteEletronico _repositorioCTe;

        #endregion

        #region Construtores

        public CustoRentabilidadeCteCrt(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CustoRentabilidadeCteCrt> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCustoRentabilidadeCteCrt filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioCTe.ConsultarRelatorioCustoRentabilidadeCteCrt(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCustoRentabilidadeCteCrt filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCTe.ContarConsultaRelatorioCustoRentabilidadeCteCrt(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/CTe/CustoRentabilidadeCteCrt";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCustoRentabilidadeCteCrt filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.ModeloDocumentoFiscal repositorioModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);

            Dominio.Entidades.Cliente remetente = filtrosPesquisa.CpfCnpjRemetente > 0D ? repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjRemetente) : null;
            List<Dominio.Entidades.Cliente> destinatario = filtrosPesquisa.CpfCnpjDestinatarios.Count > 0 ? repositorioCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CpfCnpjDestinatarios) : new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> tomadores = filtrosPesquisa.CpfCnpjTomadores.Count > 0 ? repositorioCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CpfCnpjTomadores) : new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.ModeloDocumentoFiscal> modelosDocumento = filtrosPesquisa.CodigosModeloDocumento?.Count > 0 ? repositorioModeloDocumentoFiscal.BuscarPorCodigo(filtrosPesquisa.CodigosModeloDocumento.ToArray()) : new List<Dominio.Entidades.ModeloDocumentoFiscal>();

            List<string> veiculos = filtrosPesquisa.CodigosVeiculo?.Count > 0 ? repositorioVeiculo.BuscarPlacas(filtrosPesquisa.CodigosVeiculo) : null;
            List<string> cargas = filtrosPesquisa.CodigosCarga?.Count > 0 ? repositorioCarga.BuscarNumerosPorCodigos(filtrosPesquisa.CodigosCarga) : null;
            List<string> filiais = filtrosPesquisa.CodigoFilial > 0 ? repositorioFilial.BuscarDescricoesPorCodigos(new List<int>() { filtrosPesquisa.CodigoFilial }) : null;
            List<string> empresas = filtrosPesquisa.CodigosTransportador?.Count > 0 ? repositorioEmpresa.BuscarDescricoesPorCodigos(filtrosPesquisa.CodigosTransportador) : null;
            List<string> tiposOperacao = filtrosPesquisa.CodigosTipoOperacao?.Count > 0 ? repositorioTipoOperacao.BuscarDescricoesPorCodigos(filtrosPesquisa.CodigosTipoOperacao) : null;
            List<int> ctes = filtrosPesquisa.CodigosCTe?.Count > 0 ? _repositorioCTe.BuscarNumerosPorCodigos(filtrosPesquisa.CodigosCTe) : null;

            parametros.Add(new Parametro("DataEmissao", filtrosPesquisa.DataInicialEmissao, filtrosPesquisa.DataFinalEmissao));
            parametros.Add(new Parametro("Numero", filtrosPesquisa.NumeroInicial, filtrosPesquisa.NumeroFinal));
            parametros.Add(new Parametro("Pedido", filtrosPesquisa.Pedido));
            parametros.Add(new Parametro("NotaFiscal", filtrosPesquisa.NotaFiscal));
            parametros.Add(new Parametro("Serie", filtrosPesquisa.Serie));
            parametros.Add(new Parametro("TipoServico", string.Join(", ", filtrosPesquisa.TipoServico.Select(o => o.ObterDescricao()))));
            parametros.Add(new Parametro("Situacao", Servicos.Embarcador.CTe.CTe.ObterDescricaoSituacao(filtrosPesquisa.Situacao)));
            parametros.Add(new Parametro("CTeVinculadoACarga", filtrosPesquisa.CTeVinculadoACarga));
            parametros.Add(new Parametro("Remetente", remetente?.Descricao));
            parametros.Add(new Parametro("Destinatarios", destinatario.Select(x => x.Descricao)));
            parametros.Add(new Parametro("Tomadores", tomadores.Select(x => x.Descricao)));
            parametros.Add(new Parametro("Veiculo", veiculos));
            parametros.Add(new Parametro("Carga", cargas));
            parametros.Add(new Parametro("Filial", filiais));
            parametros.Add(new Parametro("Transportador", empresas));
            parametros.Add(new Parametro("TipoOperacao", tiposOperacao));
            parametros.Add(new Parametro("CTe", ctes));
            parametros.Add(new Parametro("ModeloDocumento", modelosDocumento.Select(obj => $"{obj.Descricao} ({obj.Abreviacao})")));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}