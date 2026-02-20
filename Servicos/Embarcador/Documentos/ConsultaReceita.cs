using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Documentos
{
    public class ConsultaReceita
    {
        public static Dominio.Entidades.WebServicesConsultaNFe ObterWebService(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceConsultaReceita tipoWebService, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.WebServicesConsultaNFe repWebServicesConsultaNFe = new Repositorio.WebServicesConsultaNFe(unitOfWork);

            List<Dominio.Entidades.WebServicesConsultaNFe> listaWS = repWebServicesConsultaNFe.BuscarNaoBloqueadas(15, tipoWebService);

            Dominio.Entidades.WebServicesConsultaNFe webServicesConsultaNFe = null;


            if (listaWS.Count > 0)
                webServicesConsultaNFe = listaWS.FirstOrDefault();
            else
            {
                listaWS = repWebServicesConsultaNFe.BuscarPorNumeroDeConsultas(15, tipoWebService);

                if (listaWS.Count > 0)
                {
                    foreach (Dominio.Entidades.WebServicesConsultaNFe ws in listaWS)
                    {
                        ws.Consultas = 0;
                        repWebServicesConsultaNFe.Atualizar(ws);
                    }
                }

                listaWS = repWebServicesConsultaNFe.BuscarBloqueadasPorData(DateTime.Now.AddHours(-24), tipoWebService);

                if (listaWS.Count > 0)
                {
                    foreach (Dominio.Entidades.WebServicesConsultaNFe ws in listaWS)
                    {
                        ws.DataBloqueio = null;
                        repWebServicesConsultaNFe.Atualizar(ws);
                    }
                }

                listaWS = repWebServicesConsultaNFe.BuscarNaoBloqueadas(15, tipoWebService);

                if (listaWS != null && listaWS.Count > 0)
                    webServicesConsultaNFe = listaWS.FirstOrDefault();
            }

            return webServicesConsultaNFe;
        }

        public static void AjustarConsulta(Dominio.Entidades.WebServicesConsultaNFe webService, bool sucesso, Repositorio.UnitOfWork unitOfWork)
        {
            if (webService != null)
            {
                Repositorio.WebServicesConsultaNFe repWebServicesConsultaNFe = new Repositorio.WebServicesConsultaNFe(unitOfWork);

                if (sucesso)
                    webService.Consultas++;
                else
                    webService.DataBloqueio = DateTime.Now;

                repWebServicesConsultaNFe.Atualizar(webService);
            }
        }
    }
}
