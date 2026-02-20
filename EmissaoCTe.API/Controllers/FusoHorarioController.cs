using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class FusoHorarioController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult ObterListaDeFusos()
        {
            try
            {
                ICollection<TimeZoneInfo> timezones = TimeZoneInfo.GetSystemTimeZones();
                return Json(timezones, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os fusos horários.");
            }
        }

    }
}
