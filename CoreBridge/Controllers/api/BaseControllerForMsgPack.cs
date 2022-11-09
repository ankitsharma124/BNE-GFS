using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace CoreBridge.Controllers.api
{
    public class BaseControllerForMsgPack : ControllerBase
    {
        protected IHostEnvironment _env;
        public BaseControllerForMsgPack(IHostEnvironment env)
        {
            _env = env;
        }

        protected IActionResult ReturnMsgPack(object data)
        {
            bool useJson = false;
            if (!_env.IsProduction())
            {
                IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .AddEnvironmentVariables()
                .Build();
                try
                {
                    useJson = (bool)config.GetRequiredSection("DebugConfig")!.GetValue(typeof(bool), "UseJson");
                }
                catch (Exception ex) { }

                if (useJson)
                {
                    return new JsonResult(data);
                }
            }
            byte[] pack = MessagePackSerializer.Serialize(data);
            return new ObjectResult(pack);

        }

    }
}
