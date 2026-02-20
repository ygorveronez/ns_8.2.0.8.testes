using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize(new string[] { "BuscarAtendimentos" }, "Cargas/ControleEntrega", "Logistica/Monitoramento")]
    public class ControleEntregaAtendimentoController : BaseControleEntregaController
    {
		#region Construtores

		public ControleEntregaAtendimentoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		public async Task<IActionResult> BuscarAtendimentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repCarga.BuscarListaPorCargaEntregaPendentes(filtrosPesquisa, this.Usuario);

                return new JsonpResult(new
                {
                    Atendimento = (from o in chamados select FormatarRetornoChamado(o)).ToList()
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as entregas");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic FormatarRetornoChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado)
        {
            Dominio.Entidades.Usuario motorista = chamado.Carga.Motoristas.FirstOrDefault();

            return new
            {
                chamado.Codigo,
                Carga = chamado.CargaEntrega.Carga.Codigo,
                NumeroMotorista = ObterNumeroCelularCompleto(motorista),
                NomeMotorista = motorista?.Nome ?? string.Empty,
                Timestamp = long.Parse(chamado.DataCriacao.ToString("yyyyMMddHHmmssffff")),
                Data = chamado.DataCriacao.ToString("dd/MM"),
                Hora = chamado.DataCriacao.ToTimeString(),
                Responsavel = chamado.Responsavel?.Nome,
                Numero = chamado.Numero,
            };
        }

        private string ObterNumeroCelularCompleto(Dominio.Entidades.Usuario motorista)
        {
            if (motorista == null)
                return string.Empty;

            if (string.IsNullOrWhiteSpace(motorista.Celular))
                return string.Empty;

            string celular = Utilidades.String.OnlyNumbers(motorista.Celular ?? string.Empty);

            bool numeroBrasileiro = motorista.Localidade == null || motorista.Localidade.Pais?.Abreviacao == "BR";

            return $"{(numeroBrasileiro ? "+55" : "")}{celular}";
        }

        #endregion
    }
}