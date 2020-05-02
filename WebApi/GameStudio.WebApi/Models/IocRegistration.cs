using System.Collections;
using System.Collections.Generic;

namespace GameStudio.WebApi
{
    public class IocRegistration
    {
        public string Lifestyle { get; set; }
        public string Service { get; set; }
        public string Implementation { get; set; }
    }

    public class IocRegistrations
    {
        public IList<IocRegistration> Container { get; set; } = new List<IocRegistration>();
        public IList<IocRegistration> ServiceCollection { get; set; } = new List<IocRegistration>();
    }
}
