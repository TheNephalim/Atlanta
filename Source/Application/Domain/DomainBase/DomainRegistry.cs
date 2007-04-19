
using System;

using NHibernate;

namespace Atlanta.Application.Domain.DomainBase
{


    /// <summary>
    /// Registry class to allow the domain to find system settings and objects
    /// </summary>
    public class DomainRegistry
    {

        [ThreadStatic]
        static private ISession _session;

        /// <summary> thread-local session </summary>
        static public ISession Session
        {
            get { return _session; }
            set { _session = value; }
        }

    }

}
