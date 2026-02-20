using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace Servicos.Embarcador.Carga
{
    public class MICDTA : ServicoBase
    {        
        public MICDTA(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public string EmitirMICDTA(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            string mensagem = "";
            Repositorio.Embarcador.Cargas.CargaMICDTA repCargaMICDTA = new Repositorio.Embarcador.Cargas.CargaMICDTA(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaMICDTA cargaMICDTA = repCargaMICDTA.BuscarPorCarga(carga.Codigo);
            if (cargaMICDTA != null)
                return mensagem;

            Dominio.ObjetosDeValor.Embarcador.Localidade.DadosPaisOrigemDestino dadosPaisOrigemDestino = repCargaPedido.BuscarDadosPaisOrigemDestinoPorCarga(carga);

            bool cargaDeslocamentoVazio = carga?.TipoOperacao?.DeslocamentoVazio ?? false;

            Dominio.Entidades.Embarcador.Cargas.CargaMICDTA micdta = new Dominio.Entidades.Embarcador.Cargas.CargaMICDTA()
            {
                Carga = carga,
                CargaOrigem = carga,
                DataEmissao = DateTime.Now,
                Numero = "",
                SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado,
                NumeroSequencial = 0,
                SiglaPaisOrigem = cargaDeslocamentoVazio ? "BR" : dadosPaisOrigemDestino?.AbreviacaoOrigem ?? "BR",
                NumeroLicencaTNTI = dadosPaisOrigemDestino?.LicencaTNTIOrigem ?? "5525"
            };

            if (micdta.SiglaPaisOrigem == "BR" && (dadosPaisOrigemDestino?.AbreviacaoDestino ?? "BR") != "BR")
                micdta.NumeroLicencaTNTI = dadosPaisOrigemDestino?.LicencaTNTIDestino ?? "5525";

            if (!string.IsNullOrWhiteSpace(micdta.NumeroLicencaTNTI) && micdta.NumeroLicencaTNTI.Contains("/"))
                micdta.NumeroLicencaTNTI = micdta.NumeroLicencaTNTI.Split('/').FirstOrDefault();

            repCargaMICDTA.Inserir(micdta);

            micdta.NumeroSequencial = repCargaMICDTA.ProximoNumeroSequencial(micdta.SiglaPaisOrigem, micdta.NumeroLicencaTNTI);
            micdta.Numero = micdta.SiglaPaisOrigem + micdta.NumeroLicencaTNTI + micdta.NumeroSequencial.ToString().PadLeft(5, '0');

            repCargaMICDTA.Atualizar(micdta);

            return mensagem;
        }

        public byte[] ObterPdfMicDta(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.CargaMICDTA micdta, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            int codigoCargaIntegracao = cargaCargaIntegracao?.Codigo ?? 0;

            return ReportRequest.WithType(ReportType.MicDta)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoCarga", carga.Codigo.ToString())
                .AddExtraData("CodigoMicdta", micdta.Codigo.ToString())
                .AddExtraData("CodigoCargaIntegracao", codigoCargaIntegracao)
                .CallReport()
                .GetContentFile();
        }


    }
}
