using Newtonsoft.Json;
using System.Web.Http;
using LTM.Test.DTO;
using System.Collections.Generic;

namespace WebApplication2.Controllers
{
    public class ListaController : ApiController
    {
        [Authorize]
        [AcceptVerbs("Get", "Post")]
        public List<Telefones> Get()
        {
            return new LTM.Test.BLL.Telefones().Listar();
        }
    }
}
