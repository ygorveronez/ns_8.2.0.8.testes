using System.Collections.Generic;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.Excecoes.Embarcador;
using System.Linq;
using System.IO;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk;
using Dominio.ObjetosDeValor.WebService.Pedido;


namespace Servicos.Embarcador.Relatorios.PreCTes
{
    public class PreCTe : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.PreCTes.FiltroPesquisaRelatorioPreCTe, Dominio.Relatorios.Embarcador.DataSource.PreCTes.PreCTe>
    {
        #region Atributos

        private readonly Repositorio.PreConhecimentoDeTransporteEletronico _repositorioPreCTe;

        #endregion

        #region Construtores

        public PreCTe(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPreCTe = new Repositorio.PreConhecimentoDeTransporteEletronico(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.PreCTes.PreCTe> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.PreCTes.FiltroPesquisaRelatorioPreCTe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioPreCTe.ConsultarRelatorioPreCTe(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.PreCTes.FiltroPesquisaRelatorioPreCTe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioPreCTe.ContarConsultaRelatorioPreCTe(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/PreCTes/PreCTe";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.PreCTes.FiltroPesquisaRelatorioPreCTe filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

            List<string> empresas = filtrosPesquisa.CodigosTransportadores?.Count > 0 ? repEmpresa.BuscarDescricoesPorCodigos(filtrosPesquisa.CodigosTransportadores) : null;
            Dominio.Entidades.Estado estadoOrigem = !string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoEstadoOrigem) && filtrosPesquisa.CodigoEstadoOrigem != "0" ? repEstado.BuscarPorSigla(filtrosPesquisa.CodigoEstadoOrigem) : null;
            Dominio.Entidades.Estado estadoDestino = !string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoEstadoDestino) && filtrosPesquisa.CodigoEstadoDestino != "0" ? repEstado.BuscarPorSigla(filtrosPesquisa.CodigoEstadoDestino) : null;
            List<string> tiposOperacao = filtrosPesquisa.CodigosTiposOperacao?.Count > 0 ? repTipoOperacao.BuscarDescricoesPorCodigos(filtrosPesquisa.CodigosTiposOperacao) : null;
            List<string> tipoOcorrencia = filtrosPesquisa.CodigosTiposOcorrencia.Count > 0 ? repTipoOcorrencia.BuscarDescricoesPorCodigos(filtrosPesquisa.CodigosTiposOcorrencia) : new List<string>();
            List<string> filiais = filtrosPesquisa.CodigosFiliais?.Count > 0 ? repFilial.BuscarDescricoesPorCodigos(filtrosPesquisa.CodigosFiliais) : null;
            List<string> cargas = filtrosPesquisa.CodigosCargas?.Count > 0 ? repCarga.BuscarNumerosPorCodigos(filtrosPesquisa.CodigosCargas) : null;
            Dominio.Entidades.Localidade origem = filtrosPesquisa.CodigoOrigem > 0 ? repLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoOrigem) : null;
            Dominio.Entidades.Localidade destino = filtrosPesquisa.CodigoDestino > 0 ? repLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoDestino) : null;
            List<string> tiposDeCarga = filtrosPesquisa.CodigosTiposDeCarga?.Count > 0 ? repTipoDeCarga.BuscarDescricoesPorCodigos(filtrosPesquisa.CodigosTiposDeCarga) : null;
            List<Dominio.Entidades.ModeloVeiculo> modeloVeiculo = filtrosPesquisa.CodigosModelosVeiculos.Count > 0 ? repModeloVeiculo.BuscarPorCodigos(filtrosPesquisa.CodigosModelosVeiculos, false) : null;
            List<Dominio.Entidades.Cliente> destinatarios = filtrosPesquisa.CodigosDestinatarios.Count > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CodigosDestinatarios) : new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> remetentes = filtrosPesquisa.CodigosRemetentes.Count > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CodigosRemetentes) : new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> recebedores = filtrosPesquisa.CodigosRecebedores.Count > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CodigosRecebedores) : new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> expedidores = filtrosPesquisa.CodigosExpedidores.Count > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CodigosExpedidores) : new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> tomadores = filtrosPesquisa.CodigosTomadores.Count > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CodigosTomadores) : new List<Dominio.Entidades.Cliente>();


            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresas));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicialEmissao", filtrosPesquisa.DataEmissaoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalEmissao", filtrosPesquisa.DataEmissaoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tiposOperacao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOcorrencia", tipoOcorrencia));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", filiais));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", cargas));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Origem", origem?.DescricaoCidadeEstado));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", destino?.DescricaoCidadeEstado));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoOrigem", estadoOrigem?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoDestino", estadoDestino?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoDeCarga", tiposDeCarga));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NFe", filtrosPesquisa.NumeroNFe));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeiculo", modeloVeiculo?.Select(x => x?.Descricao ?? "")));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoTomador", filtrosPesquisa.TipoTomador?.Select(o => ((Dominio.Enumeradores.TipoTomador)o).ObterDescricao())));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", filtrosPesquisa.Situacao?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PossuiFRS", filtrosPesquisa.PossuiFRS));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", string.Join(",", destinatarios.Select(x => x.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", string.Join(",", remetentes.Select(x => x.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Recebedor", string.Join(",", recebedores.Select(x => x.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Expedidor", string.Join(",", expedidores.Select(x => x.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", string.Join(",", tomadores.Select(x => x.Descricao))));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}