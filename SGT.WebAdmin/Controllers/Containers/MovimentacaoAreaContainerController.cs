using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Containers
{
    [CustomAuthorize("Containers/MovimentacaoAreaContainer")]
    public class MovimentacaoAreaContainerController : BaseController
    {
		#region Construtores

		public MovimentacaoAreaContainerController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        private static readonly string _caminhoImagem = "../../../../img/controle-entrega/";

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarMovimentacaoAreaContainer()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaMovimentacaoAreaContainer filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = ObterParametrosConsulta();

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Pedidos.RetiradaContainer repositorioRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(unitOfWork);
                Repositorio.Embarcador.Pessoas.ClienteAreaRedex repClienteAreaRedex = new Repositorio.Embarcador.Pessoas.ClienteAreaRedex(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                int totalRegistros = repositorioCargaEntrega.ContarConsultaMovimentacaoAreaContainer(filtrosPesquisa);
                IList<Dominio.ObjetosDeValor.Embarcador.Container.MovimentacaoAreaContainer> cargasEntrega = new List<Dominio.ObjetosDeValor.Embarcador.Container.MovimentacaoAreaContainer>();
                List<Dominio.Entidades.Embarcador.Pessoas.ClienteAreaRedex> ListaAreaRedexCargaEntrega = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteAreaRedex>();

                if (totalRegistros > 0)
                {
                    cargasEntrega = repositorioCargaEntrega.ConsultarMovimentacaoAreaContainer(filtrosPesquisa, parametrosConsulta);
                    ListaAreaRedexCargaEntrega = repClienteAreaRedex.BuscarPorListaCargaEntrega(cargasEntrega.Select(x => x.Codigo).ToList());
                }

                cargasEntrega = filtrarResultados(cargasEntrega);

                var listaRetorno = (
                    from cargaEntrega in cargasEntrega
                    select new
                    {
                        cargaEntrega.Codigo,
                        cargaEntrega.CodigoCarga,
                        cargaEntrega.CodigoContainerRetirar,
                        cargaEntrega.DescricaoContainerRetirar,
                        cargaEntrega.Situacao,
                        cargaEntrega.Armador,
                        cargaEntrega.Carga,
                        cargaEntrega.TipoOperacao,
                        cargaEntrega.Veiculo,
                        cargaEntrega.NumeroExp,
                        AreasRedex = ObterAreasRedex(ListaAreaRedexCargaEntrega, repCliente.BuscarPorCPFCNPJ(cargaEntrega.CPFCNPJCliente)),
                        ClientePossuiAreaRedex = !cargaEntrega.Coleta && (cargaEntrega.LocalRetiradaContainer > 0 || PossuiAreasRedex(ListaAreaRedexCargaEntrega, cargaEntrega.CPFCNPJCliente)),
                        TipoDescricao = cargaEntrega.TipoCargaEntregaDescricao,
                        IsColeta = cargaEntrega.Coleta,
                        TipoContainerCarga = cargaEntrega.CodigoTipoContainerCarga,
                        DescricaoTipoContainerCarga = cargaEntrega.TipoContainerCarga,
                        ColetaDeContainer = cargaEntrega.ColetaDeContainer,
                        LocalRetiradaContainer = cargaEntrega.LocalRetiradaContainerDescricao
                    }
                ).ToList();

                return new JsonpResult(new
                {
                    CargasEntrega = listaRetorno
                }, totalRegistros);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.Embarcador.Container.MovimentacaoAreaContainer> filtrarResultados(IList<Dominio.ObjetosDeValor.Embarcador.Container.MovimentacaoAreaContainer> listaEntregas)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Container.MovimentacaoAreaContainer> listaRetorno = new List<Dominio.ObjetosDeValor.Embarcador.Container.MovimentacaoAreaContainer>();

            foreach (var cargaEntrega in listaEntregas)
            {
                if (cargaEntrega.Coleta && cargaEntrega.ColetaEquipamento)
                    listaRetorno.Add(cargaEntrega);
                else if (cargaEntrega.Coleta && !cargaEntrega.ColetaEquipamento && listaEntregas.Where(x => x.Coleta && x.CodigoCarga == cargaEntrega.CodigoCarga).Count() == 1)
                    listaRetorno.Add(cargaEntrega);
                else if (!cargaEntrega.Coleta)
                    listaRetorno.Add(cargaEntrega);
            }

            return listaRetorno;
        }

        private dynamic ObterAreasRedex(List<Dominio.Entidades.Embarcador.Pessoas.ClienteAreaRedex> listaAreaRedex, Dominio.Entidades.Cliente cliente)
        {
            if (cliente == null)
                return new { };

            var areas = (from area in listaAreaRedex
                         where area.Cliente.CPF_CNPJ == cliente.CPF_CNPJ
                         select new
                         {
                             text = area.AreaRedex.NomeCNPJ,
                             value = area.AreaRedex.Codigo
                         }).ToList();

            if (areas != null)
                areas.Insert(0, new { text = cliente.NomeCNPJ, value = cliente.Codigo });

            return areas;
        }

        private bool PossuiAreasRedex(List<Dominio.Entidades.Embarcador.Pessoas.ClienteAreaRedex> listaAreaRedex, double Cpf_cnpj)
        {
            var areas = listaAreaRedex.Any(x => x.Cliente.CPF_CNPJ == Cpf_cnpj);
            return areas;
        }

        private Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta ObterParametrosConsulta()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
            {
                DirecaoOrdenar = "desc",
                InicioRegistros = Request.GetIntParam("inicio"),
                LimiteRegistros = Request.GetIntParam("limite"),
                PropriedadeOrdenar = "Codigo"
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaMovimentacaoAreaContainer ObterFiltrosPesquisa(UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaMovimentacaoAreaContainer filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaMovimentacaoAreaContainer()
            {
                NumeroCarga = Request.GetStringParam("Carga"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoDestino = Request.GetIntParam("Destino"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                SituacaoEntrega = Request.GetNullableEnumParam<SituacaoEntrega>("SituacaoEntrega"),
                CpfCnpjAreaContainer = Usuario.AreaContainer?.CPF_CNPJ ?? 0,
                TipoCargaEntrega = Request.GetNullableEnumParam<TipoCargaEntrega>("TipoCargaEntrega"),
                NumeroEXP = Request.GetStringParam("NumeroEXP"),
                NumeroContainer = Request.GetStringParam("NumeroContainer")
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && this.Empresa != null)
            {
                filtroPesquisa.CodigoEmpresa = this.Empresa.Codigo;
                filtroPesquisa.CpfCnpjEmpresaColeta = new List<double>();

                Dominio.Entidades.Empresa empresaCTe = repEmpresa.BuscarPorCodigo(filtroPesquisa.CodigoEmpresa);

                filtroPesquisa.CpfCnpjEmpresaColeta.Add(double.Parse(empresaCTe.CNPJ));
                filtroPesquisa.CpfCnpjEmpresaColeta.AddRange(repCliente.BuscarCpfCnpjClienteArmazemResponsavel(double.Parse(empresaCTe.CNPJ)));
            }

            return filtroPesquisa;
        }

        private string ObterImagemEntregaColeta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega situacaoEntrega, bool coleta, bool fronteira, bool reentrega)
        {
            if (fronteira)
            {
                switch (situacaoEntrega)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue:
                        return $"Content/TorreControle/Icones/alertas/fronteira-realizada.svg";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado:
                        return $"Content/TorreControle/Icones/alertas/fronteira-nao-realizada.svg";
                    default:
                        return "Content/TorreControle/Icones/alertas/fronteira-pendente.svg";
                }
            }

            if (coleta)
            {
                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue)
                    return _caminhoImagem + $"coleta-realizada.png";

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado)
                    return _caminhoImagem + $"coleta-nao-realizada.png";

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Revertida)
                    return _caminhoImagem + $"coleta-revertida.png";

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Reentergue)
                    return _caminhoImagem + $"coleta-reentregue.png";

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.AgAtendimento)
                    return _caminhoImagem + $"coleta-atendimento.png";

                if (reentrega)
                    return _caminhoImagem + $"coleta-reentregue.png";

                return _caminhoImagem + "coleta-pendente.png";
            }

            if (!coleta)
            {
                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue)
                    return _caminhoImagem + $"entrega-realizada.png";

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado)
                    return _caminhoImagem + $"entrega-devolvida.png";

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Revertida)
                    return _caminhoImagem + $"entrega-revertida.png";

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Reentergue)
                    return _caminhoImagem + $"entrega-reentregue.png";

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.AgAtendimento)
                    return _caminhoImagem + $"entrega-atendimento.png";

                if (reentrega)
                    return _caminhoImagem + $"entrega-reentregue.png";

                return _caminhoImagem + "entrega-pendente.png";
            }

            return "coleta-pendente.png";
        }

        #endregion Métodos Privados
    }
}
